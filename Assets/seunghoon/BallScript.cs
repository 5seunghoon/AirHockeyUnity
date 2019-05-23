using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Leap;
using seunghoon;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    public static Vector3 player1ResetVector3 = new Vector3(0f, yPosDefault, 0.82f);
    public static Vector3 player2ResetVector3 = new Vector3(0f, yPosDefault, 1.139f);
    
    private DateTime prevTime = DateTime.Now;

    public static bool isSendBallPosition = false;

    public static int scorePoint = 1;

    private static float yPosDefault = -0.112f;

    private static float maxVelocity = 0.7f;

    public static String whoPush = "NONE";
    // Start is called before the first frame update
    void Start()
    {
    }

    private void SetBallPosition(string data)
    {
        //Debug.Log("setBallPosition data : " + data);
        BallPositionModel ballPositionModel = JsonUtility.FromJson<BallPositionModel>(data);
        //Debug.Log("bpm x : " + ballPositionModel.x + ", bpm y : " + ballPositionModel.y + ", bpm z : " +
        //          ballPositionModel.z);
        
        //change ball position
        Vector3 newPositionVector = new Vector3(ballPositionModel.x, ballPositionModel.y, ballPositionModel.z);
        transform.position = newPositionVector;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        var jstr = "{\"interval\":" + (DateTime.Now.Ticks - ballPositionModel.tick) + "}";
        ControllMove.socket.EmitJson("TEST_EMIT", jstr);
    }

    private string BallPositionToJson()
    {

        var position = transform.position;
        var x = position.x;
        var y = position.y;
        var z = position.z;

        // 반올림
        x = Mathf.Round(x * 100000f) * 0.00001f;     
        y = Mathf.Round(y * 100000f) * 0.00001f;
        z = Mathf.Round(z * 100000f) * 0.00001f;

        var positionJsonStr = "{\"x\":" + x + ", \"y\":" + y + ", \"z\":" + z + ", \"tick\":" + DateTime.Now.Ticks + "}";
        return positionJsonStr;
    }

    // Update is called once per frame
    void Update()
    {

        var rigidbody = GetComponent<Rigidbody>();
        // Player 1일 경우, 공 위치를 서버로 송신
        if (ControllMove.playerType == "P1" && isSendBallPosition &&
            Math.Abs(DateTime.Now.Ticks - prevTime.Ticks) > 1000000) //100ms당 1회 
        {
            prevTime = DateTime.Now;
            var positionJsonStr = BallPositionToJson();
            //Debug.Log("position Json Str : " + positionJsonStr);
            //공의 위치를 Socket Emit
            ControllMove.socket.EmitJson("ballPosition", positionJsonStr);
        } 

        if (ControllMove.playerType == "P2")
        {
            rigidbody.detectCollisions = false;
        }

        Vector3 nowVelocity = GetComponent<Rigidbody>().velocity;
        //Debug.Log("Ball velocity : " + nowVelocity);

        if (rigidbody.velocity.magnitude > 0.06f && rigidbody.velocity.magnitude < 0.18f)
        {
            Debug.Log("PUSH");
            Debug.Log("a : " + rigidbody.velocity.normalized);
            Debug.Log("b : " + rigidbody.velocity.normalized * (rigidbody.velocity.magnitude * 20f + 1f));
            Debug.Log("c : " + rigidbody.velocity.magnitude);
            rigidbody.AddForce(rigidbody.velocity.normalized * (rigidbody.velocity.magnitude * 10f + 1f) , ForceMode.Impulse);
        }
        else if (rigidbody.velocity.magnitude > maxVelocity)
        {
            Debug.Log("UNPUSH");
            var velocity = rigidbody.velocity;
            rigidbody.velocity = new Vector3(velocity.x * 0.7f, velocity.x * 0.7f, velocity.x * 0.7f);
        }
        
        //y좌표 고정
        var position = transform.position;
        position = new Vector3(position.x, yPosDefault, position.z);
        transform.position = position;
        
        transform.rotation = Quaternion.identity;
    }


    public void BallAddForce(Vector3 inNormal, Vector3 stickForceVector)
    {
        inNormal.x += stickForceVector.x;
        inNormal.z += stickForceVector.z;
        var rigid = GetComponent<Rigidbody>();
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        GetComponent<Rigidbody>().AddForce(inNormal, ForceMode.VelocityChange);
    }
}