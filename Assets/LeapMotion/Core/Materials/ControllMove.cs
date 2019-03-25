using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;

public class ControllMove : MonoBehaviour
{

    Controller controller;
    float HandPalmPitch;
    float HandPalmYam;
    float HandPalmRoll;
    float HandWristRot;

    string url = "http://127.0.0.1:3000/";

    void Awake()
    {
        Debug.Log("start camera");

    }
    
    // Start is called before the first frame update
    void Start()
    {
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
            ballReset();
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
