using System;
using System.Collections;
using System.Collections.Generic;
using seunghoon;
using UnityEngine;

public class GoalScript : MonoBehaviour
{

    private DateTime prevTime = DateTime.Now;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnTriggerEnter(Collider col)
    {
        // 벽에 닿으면 공 위치 초기화
        if (col.CompareTag("BALL"))
        {
            if (DateTime.Now.Ticks - prevTime.Ticks > 10000000) // 1초에 한번만 Emit
            {
                prevTime = DateTime.Now;

                Debug.Log("Ball Trigger Enter");
                string getScorePlayer = "P1";
                if (CompareTag("P2GOAL"))
                {
                    getScorePlayer = "P2";
                }

                var jsonStr = "{\"player\":\"" + getScorePlayer +
                              "\",\"score\":\"" + BallScript.scorePoint + "\"}";
                ControllMove.socket.EmitJson("scoreUp", jsonStr);

                GameObject o;
                (o = col.gameObject).GetComponent<Rigidbody>().velocity = Vector3.zero;

                if (BallScript.whoPush == "P2")
                {
                    o.transform.position = BallScript.player2ResetVector3;
                }
                else
                {
                    o.transform.position = BallScript.player1ResetVector3;
                }
            }
        }
    }
}