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

    private bool alreadyReady = false;

    private DateTime prevTime = DateTime.Now;

    private string url = "http://ec2-13-58-99-209.us-east-2.compute.amazonaws.com:3000/";
    public static Socket socket;

    public static String playerType = "P1";

    void Start()
    {
        socket = Socket.Connect(url);
        socket.On("connect", (string data) => { });
        socket.On("userReadyOn", userReadyOn); 
        socket.On("gameStart", gameStart); 
        ballReset(); // 공 위치 초기화 
    }

    private void userReadyOn(String data) // Player1, Player2 지정에 대한 대기
    {
        PlayerModel playerModel = JsonUtility.FromJson<PlayerModel>(data);
        playerType = playerModel.player;
        Debug.Log("userReadyOn : " + playerType);
    }

    private void gameStart(String data) // 게임 시작 신호 대기
    {
        BallScript.isSendBallPosition = true;
        Debug.Log("game start");
        if (playerType == "P2") invalidCollider(); // Player2일 경우 물리 계산 disable
    }

    private void invalidCollider() // Player 2일 경우에, 공과 벽은 충돌하면 안됨
    {
        Debug.Log("invalid collider");
        
        // 벽과 바닥의 object에 대해 충돌 disable
        var walls = GameObject.FindGameObjectsWithTag("WALL");
        var floors = GameObject.FindGameObjectsWithTag("FLOOR");
        foreach (var wall in walls) wall.GetComponent<Collider>().isTrigger = true;
        foreach (var floor in floors) floor.GetComponent<Collider>().isTrigger = true;
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
        // 스페이스 바 눌리면 레디 (1회만)
        if (Input.GetKeyDown(KeyCode.Space) && !alreadyReady)
        {
            var readyJsonStr = "{\"player\":\"" + playerType + "\"}";
            //socket.EmitJson("userReady", readyJsonStr);
            socket.EmitJson("userReady", "");
            Debug.Log("space bar down, user Ready : " + playerType);
            alreadyReady = true;
        }
        
        
        // R키 눌리면 공 위치 초기화
        if (Input.GetKeyDown(KeyCode.R) && playerType == "P1")
        {
            ballReset();
            //socket.EmitJson("playerOn", "");
        }
        


        controller = new Controller();
        Frame frame = controller.Frame();
        List<Hand> hands = frame.Hands;
        if (frame.Hands.Count > 0)
        //if (false)
        {
            var handPosition = hands[0].PalmPosition;
            //Debug.Log("x : " + handPosition.x + ", y : " + handPosition.y + ", z : " + handPosition.z);

            string stickTag = "";
            if (playerType == "P1") stickTag = "STICK1";
            else stickTag = "STICK2";
            
            GameObject.FindGameObjectWithTag(stickTag).transform.position = new Vector3(
                handPosition.x / 700f,
                -0.1f,
                handPosition.z / -700f + 0.2f);
        }
    }
}