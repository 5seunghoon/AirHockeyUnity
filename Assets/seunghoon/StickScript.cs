using System;
using System.Collections;
using System.Collections.Generic;
using Leap;
using UnityEngine;
using UnityEngine.Networking;

public class StickScript : NetworkBehaviour
{
    private float _nowXSpeed;
    private float _nowZSpeed;
    private float _prevX;
    private float _prevZ;
    private DateTime _prevSpeedTime;

    public DateTime prevTriggerTime;
    // Start is called before the first frame update

    public static float StickYPosition = -0.0949f;

    void Start()
    {
        _nowXSpeed = 0;
        _nowZSpeed = 0;
        var position = transform.position;
        _prevX = position.x;
        _prevZ = position.z;
        _prevSpeedTime = DateTime.Now;
        prevTriggerTime = DateTime.Now;
    }

    // Update is called once per frame
    void Update()
    {
        if (ControllMove.PlayerType == "P1") CalcSpeed();
        if (ControllMove.PlayerType == "P2") DisablePhysics();
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }


    private void DisablePhysics()
    {
        //GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().detectCollisions = false;
    }

    private void CalcSpeed()
    {
        var position = transform.position;
        var x = position.x;
        var y = position.y;
        var z = position.z;

        var timeInterval = (DateTime.Now - _prevSpeedTime).Milliseconds;

        _nowXSpeed = (x - _prevX) / timeInterval;
        _nowZSpeed = (z - _prevZ) / timeInterval;

        _prevX = x;
        _prevZ = z;
        _prevSpeedTime = DateTime.Now;

        //Debug.Log("nowXSpeed : " + nowXSpeed + ", nowYSpeed : " + nowYSpeed + ", nowZSpeed : " + nowZSpeed);

        transform.rotation = Quaternion.identity;
    }

    private void OnCollisionEnter(Collision other)
    {
        // 누가 공을 쳤는지 저장
        if (other.gameObject.CompareTag("BALL"))
        {
            if (gameObject.CompareTag("STICK1")) BallScript.WhoPush = "P1";
            else if (gameObject.CompareTag("STICK2")) BallScript.WhoPush = "P2";
        }

        if (ControllMove.PlayerType == "P1" && other.gameObject.CompareTag("BALL") &&
            (DateTime.Now - prevTriggerTime).Ticks >= 2000000) //200ms당 1회
        {
            prevTriggerTime = DateTime.Now;
            Debug.Log("BALL ENTER");
            Vector3 inNormal = other.transform.position - transform.position;
            Debug.Log("inNormal : " + inNormal);
            inNormal.y = 0;

            double xPower = _nowXSpeed * 1500;
            double zPower = _nowZSpeed * 1500;
            float stickPower = (float) Math.Sqrt(xPower * xPower + zPower * zPower) * 2f;
            Debug.Log("stickPower : " + stickPower);
            
            other.gameObject.GetComponent<BallScript>().BallAddForce(inNormal, stickPower);
            Debug.Log("STICK VELOCITY : " + GetComponent<Rigidbody>().velocity);
        }
    }
}