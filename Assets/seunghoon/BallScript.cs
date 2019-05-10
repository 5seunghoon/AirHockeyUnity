using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using seunghoon;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    private DateTime prevTime = DateTime.Now;

    public static bool isSendBallPosition = false;

    public static int scorePoint = 1;

    // Start is called before the first frame update
    void Start()
    {
    }

    private void setBallPosition(string data)
    {
        Debug.Log("setBallPosition data : " + data);
        BallPositionModel ballPositionModel = JsonUtility.FromJson<BallPositionModel>(data);
        Debug.Log("bpm x : " + ballPositionModel.x + ", bpm y : " + ballPositionModel.y + ", bpm z : " +
                  ballPositionModel.z);
        
        //change ball position
        Vector3 newPositionVector = new Vector3(ballPositionModel.x, ballPositionModel.y, ballPositionModel.z);
        transform.position = newPositionVector;
    }

    private string ballPositionToJson()
    {

        var position = transform.position;
        var x = position.x;
        var y = position.y;
        var z = position.z;

        // 반올림
        x = Mathf.Round(x * 100000f) * 0.00001f;
        y = Mathf.Round(y * 100000f) * 0.00001f;
        z = Mathf.Round(z * 100000f) * 0.00001f;

        var positionJsonStr = "{\"x\":" + x + ", \"y\":" + y + ", \"z\":" + z + "}";
        return positionJsonStr;
    }

    // Update is called once per frame
    void Update()
    {
        // Player 1일 경우, 공 위치를 서버로 송신
        if (ControllMove.playerType == "P1" && isSendBallPosition &&
            Math.Abs(DateTime.Now.Ticks - prevTime.Ticks) > 1000000) //100ms당 1회 
        {
            prevTime = DateTime.Now;
            var positionJsonStr = ballPositionToJson();
            //Debug.Log("position Json Str : " + positionJsonStr);
            //공의 위치를 Socket Emit
            ControllMove.socket.EmitJson("ballPosition", positionJsonStr);
        }

        // Player 2일 경우, 공 위치를 수신받아서 공 위치 변경
        if (ControllMove.playerType == "P2")
        {
            ControllMove.socket.On("ballPositionEmit", setBallPosition);
        }

        Vector3 nowVelocity = GetComponent<Rigidbody>().velocity;
        //Debug.Log("Ball velocity : " + nowVelocity);
    }


    public void ballAddForce(Vector3 inNormal, Vector3 stickForceVector)
    {
        inNormal.x += stickForceVector.x;
        inNormal.z += stickForceVector.z;
        GetComponent<Rigidbody>().AddForce(inNormal, ForceMode.VelocityChange);
    }
}