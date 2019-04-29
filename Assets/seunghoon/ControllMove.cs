using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;
using seunghoon;
using socket.io;

public class ControllMove : MonoBehaviour
{
    // socket.io for unity : https://github.com/nhnent/socket.io-client-unity3d/

    Controller controller;
    float HandPalmPitch;
    float HandPalmYam;
    float HandPalmRoll;
    float HandWristRot;

    private DateTime prevTime = DateTime.Now;

    string url = "http://b5def756.ngrok.io";
    public static Socket socket;

    public static String playerType = "P1";

    void Start()
    {
        socket = Socket.Connect(url);
        socket.On("connect", (string data) => { Debug.Log("connect with server"); });
        socket.On("gameStartEmit", gameStart);
        socket.On("playerOnEmit", playerOn);
        ballReset(); // 공 위치 초기화 
    }

    private void playerOn(String data)
    {
        PlayerModel playerModel = JsonUtility.FromJson<PlayerModel>(data);
        playerType = playerModel.player;
        Debug.Log("player on : " + playerType);
    }

    private void gameStart(String data)
    {
        //playerType = data.playerType
        BallScript.isSendBallPosition = true;
        Debug.Log("game start");

        if (playerType == "P2") invalidCollider();
    }

    private void invalidCollider()
    {
        // Player 2일 경우, 충돌이 안되게 해야함.
        Debug.Log("invalid collider");
        var walls = GameObject.FindGameObjectsWithTag("WALL");
        var floors = GameObject.FindGameObjectsWithTag("FLOOR");

        foreach (var wall in walls)
        {
            wall.GetComponent<Collider>().isTrigger = true;
        }

        foreach (var floor in floors)
        {
            floor.GetComponent<Collider>().isTrigger = true;
        }
    }

    void ballReset()
    {
        GameObject ball = GameObject.FindWithTag("BALL");
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
            var readyJsonStr = "{\"player\":\"" + playerType + "\"}";
            socket.EmitJson("userReady", readyJsonStr);
            Debug.Log("space bar down, user Ready : " + playerType);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            socket.EmitJson("playerOn", "");
        }


        controller = new Controller();
        Frame frame = controller.Frame();
        List<Hand> hands = frame.Hands;
        //if (frame.Hands.Count > 0)
        if (false)
        {
            var handPosition = hands[0].PalmPosition;
            Debug.Log("x : " + handPosition.x + ", y : " + handPosition.y + ", z : " + handPosition.z);
            GameObject.FindGameObjectWithTag("STICK1").transform.position = new Vector3(
                handPosition.x / 700f,
                //handPosition.y / 1000f - 0.25f, 
                -0.1f,
                handPosition.z / -700f + 0.2f);
        }

        //HandPalmPitch = hands[0].PalmNormal.Pitch;
        //HandPalmRoll = hands[0].PalmNormal.Roll;
        //HandPalmYam = hands[0].PalmNormal.Yaw;
        //HandWristRot = hands[0].WristPosition.Pitch;
        //Debug.Log("Pitch : " + HandPalmPitch + ", Roll : " + HandPalmRoll + ", Yam : " + HandPalmYam);
    }
}