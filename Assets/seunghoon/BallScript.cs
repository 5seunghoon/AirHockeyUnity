using System;
using System.Collections;
using System.Collections.Generic;
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
    }

    // Update is called once per frame
    void Update()
    {
        if (ControllMove.playerType == "P1" && isSendBallPosition &&
            Math.Abs(DateTime.Now.Ticks - prevTime.Ticks) > 1000000)
        {
            prevTime = DateTime.Now;

            var position = transform.position;
            var x = position.x;
            var y = position.y;
            var z = position.z;

            // 반올림
            x = Mathf.Round(x * 100000f) * 0.00001f;
            y = Mathf.Round(y * 100000f) * 0.00001f;
            z = Mathf.Round(z * 100000f) * 0.00001f;

            var positionJsonStr = "{\"x\":" + x + ", \"y\":" + y + ", \"z\":" + z + "}";
            Debug.Log("position Json Str : " + positionJsonStr);
            ControllMove.socket.EmitJson("ballPosition", positionJsonStr);
        }

        if (ControllMove.playerType == "P2")
        {
            // 공 위치를 수신받아서 위치 변경
            ControllMove.socket.On("ballPositionToP2", setBallPosition);
        }
    }
}