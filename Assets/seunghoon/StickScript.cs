using System;
using System.Collections;
using System.Collections.Generic;
using Leap;
using UnityEngine;

public class StickScript : MonoBehaviour
{
    public float nowXSpeed;
    public float nowYSpeed;
    public float nowZSpeed;
    public float prevX;
    public float prevY;
    public float prevZ;
    public DateTime prevSpeedTime;
    public DateTime prevTriggerTime;
    // Start is called before the first frame update
    void Start()
    {
        
        nowXSpeed = 0;
        nowYSpeed = 0;
        nowZSpeed = 0;
        var position = transform.position;
        prevX = position.x;
        prevY = position.y;
        prevY = position.z;
        prevSpeedTime = DateTime.Now;
        prevTriggerTime = DateTime.Now;
    }

    // Update is called once per frame
    void Update()
    {
        if(ControllMove.playerType == "P1" && CompareTag("STICK1")) calcSpeed();  
    }

    private void calcSpeed()
    {
        var position = transform.position;
        var x = position.x;
        var y = position.y;
        var z = position.z;
        
        var timeInterval = (DateTime.Now - prevSpeedTime).Milliseconds;

        nowXSpeed = (x - prevX) / timeInterval;
        nowYSpeed = (y - prevY) / timeInterval;
        nowZSpeed = (z - prevZ) / timeInterval;

        prevX = x;
        prevY = y;
        prevZ = z;
        prevSpeedTime = DateTime.Now;
        
        //Debug.Log("nowXSpeed : " + nowXSpeed + ", nowYSpeed : " + nowYSpeed + ", nowZSpeed : " + nowZSpeed);
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (ControllMove.playerType == "P1" && other.CompareTag("BALL") && (DateTime.Now - prevTriggerTime).Ticks >= 2000000) //200ms당 1회
        {
            prevTriggerTime = DateTime.Now;
            Debug.Log("BALL ENTER");
            Vector3 inNormal = other.transform.position - transform.position;
            Debug.Log("inNormal : " + inNormal);
            inNormal.y = 0;
            other.GetComponent<BallScript>().ballAddForce(inNormal, new Vector3(nowXSpeed * 1000,0,nowZSpeed * 1000));
            //Debug.Log("STICK VELOCITY : " + GetComponent<Rigidbody>().velocity);
        }
    }
}
