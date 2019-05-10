using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalScript : MonoBehaviour
{
    public static Vector3 resetVector3 = new Vector3(0f, -0.1145f, 0.304f);

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

                var jsonStr = "{\"player\":\"" + ControllMove.playerType +
                              "\",\"score\":\"" + BallScript.scorePoint + "\"}";
                ControllMove.socket.EmitJson("scoreUp", jsonStr);

                col.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                col.gameObject.transform.position = resetVector3;
            }
        }
    }
}