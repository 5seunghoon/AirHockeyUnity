using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;
using socket.io;

public class ControllMove : MonoBehaviour
{

    // socket.io for unity : https://github.com/nhnent/socket.io-client-unity3d/
        
    Controller controller;
    float HandPalmPitch;
    float HandPalmYam;
    float HandPalmRoll;
    float HandWristRot;

    string url = "http://127.0.0.1:3000/";
    public static Socket socket;

    void Awake()
    {
        Debug.Log("start camera");

    }
    
    // Start is called before the first frame update
    void Start()
    {
        socket = Socket.Connect(url);
        socket.On("HELLO_WORLD", (string data) => {
            Debug.Log("HELLO_WORLD data : " + data);
        });
        ballReset();
    }

    void OnDisable()
    {
        Debug.Log("socket close");
    }


    void ballReset()
    {
        GameObject ball = GameObject.FindWithTag("Ball");
        ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ball.transform.position = GoalScript.resetVector3;
    }
    // Update is called once per frame
    void Update()
    {
        // 스페이스 바 눌리면 공 위치 초기화
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("space bar down");
            ballReset();
            socket.Emit("TEST_EVENT", "");
        }


        /*
        controller = new Controller();
        Frame frame = controller.Frame();
        List<Hand> hands = frame.Hands;
        if (frame.Hands.Count > 0)
        {
            Hand fristHand = hands[0];
        }
        HandPalmPitch = hands[0].PalmNormal.Pitch;
        HandPalmRoll = hands[0].PalmNormal.Roll;
        HandPalmYam = hands[0].PalmNormal.Yaw;

        HandWristRot = hands[0].WristPosition.Pitch;

        Debug.Log("Pitch : " + HandPalmPitch + ", Roll : " + HandPalmRoll + ", Yam : " + HandPalmYam);
        */
    }
}
