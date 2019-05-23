﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using Leap;
using Leap.Unity;
using seunghoon;
using socket.io;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

public class ControllMove : NetworkBehaviour
{
    // socket.io for unity : https://github.com/nhnent/socket.io-client-unity3d/

    Controller controller;
    float HandPalmPitch;
    float HandPalmYam;
    float HandPalmRoll;
    float HandWristRot;

    public Text youScore;
    public Text rivalScroe;
    public Text gameTime;

    public IpModel ipModel;

    public Camera player1Camera;
    public Camera player2Camera;

    public static bool alreadyReady = false;
    public static bool isConnect = false;

    private DateTime stickPrevTime = DateTime.Now;

    //private string url = "http://ec2-13-58-99-209.us-east-2.compute.amazonaws.com:3000/";
    //private string url = "http://759eb21d.ngrok.io/";
    //private string url = "http://172.30.97.24:3000";
    private string url = "http://127.0.0.1:3000";

    //private string url = "http://192.168.43.12:3000/";
    public static Socket socket;

    public static String playerType = "P1";

    private static DateTime pastTestDateTime;

    void Start()
    {
        socket = Socket.Connect(url);
        socket.On("connect", () => { isConnect = true; });
        socket.On("userReadyOn", UserReadyOn);
        socket.On("gameStart", GameStartOn);
        socket.On("handPositionEmit", HandPositionCallback);
        socket.On("scoreUpEmit", ScoreUpCallback);
        socket.On("timerEmit", GameTimerCallback);
        BallReset(); // 공 위치 초기화 
    }

    public void ShowPlayer1Camera()
    {
        player2Camera.enabled = false;
        player1Camera.enabled = true;
    }

    private void showPlayer2Camera()
    {
        player1Camera.enabled = false;
        player2Camera.enabled = true;
    }

    private void HandPositionCallback(String data)
    {
        if (playerType == "P1")
        {
            HandPositionModel handPositionModel = JsonUtility.FromJson<HandPositionModel>(data);
            GameObject.FindGameObjectWithTag("STICK2").transform.position =
                new Vector3(handPositionModel.x, handPositionModel.y, handPositionModel.z);
        }
    }

    private void ScoreUpCallback(String data)
    {
        ScoreModel scoreModel = JsonUtility.FromJson<ScoreModel>(data);
        if (playerType == "P1")
        {
            youScore.text = scoreModel.p1Score;
            rivalScroe.text = scoreModel.p2Score;
        }
        else
        {
            youScore.text = scoreModel.p2Score;
            rivalScroe.text = scoreModel.p1Score;
        }
    }

    private void GameTimerCallback(String data)
    {
        GameTimeModel gameTimeModel = JsonUtility.FromJson<GameTimeModel>(data);
        gameTime.text = gameTimeModel.ToString();
    }

    private void UserReadyOn(String data) // Player1, Player2 지정에 대한 대기
    {
        PlayerModel playerModel = JsonUtility.FromJson<PlayerModel>(data);
        playerType = playerModel.player;
        Debug.Log("userReadyOn : " + playerType);

        if (playerType == "P1") NetworkCustomManager.StartHostCustom();
    }

    private void GameStartOn(String data) // 게임 시작 신호 대기
    {
        BallScript.isSendBallPosition = true;
        Debug.Log("game start");

        ipModel = JsonUtility.FromJson<IpModel>(data);
        
        if (playerType == "P2")
        {
            NetworkCustomManager.StartClientCustom(ipModel.hostIp, 7777);
            Debug.Log("P2, Host ip : " + ipModel.hostIp);
            showPlayer2Camera();
            InvalidCollider(); // Player2일 경우 물리 계산 disable
            //changeLeapControllerPosition();
        }
    }

    private void InvalidCollider() // Player 2일 경우에, 공과 벽은 충돌하면 안됨
    {
        Debug.Log("invalid collider");

        // 벽과 바닥의 object에 대해 충돌 disable
        var walls = GameObject.FindGameObjectsWithTag("WALL");
        var floors = GameObject.FindGameObjectsWithTag("FLOOR");
        var balls = GameObject.FindGameObjectsWithTag("BALL");
        foreach (var wall in walls)
        {
            wall.GetComponent<Rigidbody>().isKinematic = true;
            wall.GetComponent<Rigidbody>().detectCollisions = false;
        }

        foreach (var floor in floors)
        {
            floor.GetComponent<Rigidbody>().isKinematic = true;
            floor.GetComponent<Rigidbody>().detectCollisions = false;
        }

        foreach (var ball in balls)
        {
            ball.GetComponent<Rigidbody>().isKinematic = true;
            ball.GetComponent<Rigidbody>().detectCollisions = false;
        }
    }

    private void BallReset()
    {
        GameObject ball = GameObject.FindWithTag("BALL");
        ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ball.transform.position = BallScript.player1ResetVector3;
    }

    public static void ReadyGame()
    {
        if (!alreadyReady && isConnect)
        {
            string myIp = IPManager.GetIP(ADDRESSFAM.IPv4);
            Debug.Log("myIp : " + myIp);
            var emitJsonStr = "{\"ip\": \"" + myIp + "\"}";
            socket.EmitJson("userReady", emitJsonStr);
            alreadyReady = true;
        }
    }
   
    // Update is called once per frame
    void Update()
    {
        // 스페이스 바 눌리면 레디 (1회만)
        if (Input.GetKeyDown(KeyCode.Space) ) ReadyGame();

        // R키 눌리면 공 위치 초기화
        if (Input.GetKeyDown(KeyCode.R) && playerType == "P1")
        {
            BallReset();
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
            if (playerType == "P1")
            {
                stickTag = "STICK1";
                var x = handPosition.x / 700f;
                var y = StickScript.stickYPosition;
                var z = handPosition.z / -700f + 0.75f;
                GameObject.FindGameObjectWithTag(stickTag).transform.position = new Vector3(x, y, z);
            }
            else
            {
                stickTag = "STICK2";

                var x = handPosition.x * -1f / 700f;
                var y = StickScript.stickYPosition;
                var z = (handPosition.z / -700f + 0.2f) * -1f + 1.4f;
                GameObject.FindGameObjectWithTag(stickTag).transform.position = new Vector3(x, y, z);

                if ((DateTime.Now - stickPrevTime).Ticks > 1000000)
                {
                    stickPrevTime = DateTime.Now;
                    var positionJsonStr = "{\"x\":" + x + ", \"y\":" + y + ", \"z\":" + z + ", \"player\":" + "\"" +
                                          playerType + "\"" + "}";
                    socket.EmitJson("handPosition", positionJsonStr);
                }
            }
        }
        else
        {
            string stickTag = "";
            if (playerType == "P1")
            {
                stickTag = "STICK1";
                var x = 0f;
                var y = StickScript.stickYPosition;
                var z = 0.5f;
                GameObject.FindGameObjectWithTag(stickTag).transform.position = new Vector3(x, y, z);
            }
            else
            {
                stickTag = "STICK2";

                var x = 0f;
                var y = StickScript.stickYPosition;
                var z = 1.4f;
                var stick = GameObject.FindGameObjectWithTag(stickTag);
                stick.transform.position = new Vector3(x, y, z);
                stick.GetComponent<Rigidbody>().velocity = Vector3.zero;

                if ((DateTime.Now - stickPrevTime).Ticks > 1000000)
                {
                    stickPrevTime = DateTime.Now;
                    var positionJsonStr = "{\"x\":" + x + ", \"y\":" + y + ", \"z\":" + z + ", \"player\":" + "\"" +
                                          playerType + "\"" + "}";
                    socket.EmitJson("handPosition", positionJsonStr);
                }
            }
        }
    }
   
}