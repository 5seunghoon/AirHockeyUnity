using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    private DateTime prevTime = DateTime.Now;
    
    public static bool isSendBallPosition = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (isSendBallPosition && Math.Abs(DateTime.Now.Ticks - prevTime.Ticks) > 1000000)
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
    }
}