using System;
using System.Collections;
using System.Collections.Generic;
using seunghoon;
using UnityEngine;

public class GoalScript : MonoBehaviour
{
    private DateTime prevTime = DateTime.Now;

    private const float X_SCALE_DEFAULT = 0.25f;
    private const float X_SCALE_BIG = 0.425f;
    private const float X_SCALE_SMALL = 0.12f;

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
                              "\",\"scorePoint\":\"" + BallScript.ScorePoint + "\"}";
                ControllMove.WebSocket.EmitJson("scoreUp", jsonStr);

                GameObject o;
                (o = col.gameObject).GetComponent<Rigidbody>().velocity = Vector3.zero;

                if (BallScript.WhoPush == "P2")
                {
                    o.transform.position = new Vector3(BallScript.PLAYER2_RESET_X, o.transform.position.y,
                        BallScript.PLAYER2_RESET_Z);
                }
                else
                {
                    o.transform.position = new Vector3(BallScript.PLAYER1_RESET_X, o.transform.position.y,
                        BallScript.PLAYER1_RESET_Z);
                }
            }
        }
    }

    public void BigGoalModeStart(string player)
    {
        Debug.Log("big goal mode start player : " + player);
        var localScale = transform.localScale;
        transform.localScale = new Vector3(X_SCALE_BIG, localScale.y, localScale.z);
    }

    public void BigGoalModeEnd(string player)
    {
        Debug.Log("big goal mode end player : " + player);
        var localScale = transform.localScale;
        transform.localScale = new Vector3(X_SCALE_DEFAULT, localScale.y, localScale.z);
    }

    public void SmallGoalModeStart(string player)
    {
        Debug.Log("small goal mode start player : " + player);
        var localScale = transform.localScale;
        transform.localScale = new Vector3(X_SCALE_SMALL, localScale.y, localScale.z);
    }

    public void SmallGoalModeEnd(string player)
    {
        Debug.Log("small goal mode end player : " + player);
        var localScale = transform.localScale;
        transform.localScale = new Vector3(X_SCALE_DEFAULT, localScale.y, localScale.z);
    }
}