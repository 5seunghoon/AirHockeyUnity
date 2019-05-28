using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using UnityEngine;
using Leap;
using Leap.Unity;
using seunghoon;
using socket.io;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class ControllMove : NetworkBehaviour
{
    // socket.io for unity : https://github.com/nhnent/socket.io-client-unity3d/

    private Controller _controller;

    public Text youScore;
    public Text rivalScore;
    public Text gameTime;
    public Text gameReadyText;
    public Image gameReadyTextBgImage;
    public GameObject feverTimeText;
    public GameObject gameOverText;
    public GameObject winnerText;

    public IpModel ipModel;

    public GameObject doubleGameItem;
    public GameObject bigGoalGameItem;
    public GameObject smallGoalGameItem;
    public GameObject penaltyKickGameItem;

    public GameObject hockeyBall;

    public Camera player1Camera;
    public Camera player2Camera;

    private static bool _alreadyReady = false;
    private static bool _isConnect = false;

    private string _url = "http://127.0.0.1:3000";

    public static Socket WebSocket;

    public static String PlayerType = "P1";

    private static DateTime _pastTestDateTime;

    IEnumerator ShowFlickering(GameObject flickeringObject)
    {
        int count = 0;
        while (count < 3)
        {
            flickeringObject.SetActive(true);
            yield return new WaitForSeconds(0.2f);
            flickeringObject.SetActive(false);
            yield return new WaitForSeconds(0.2f);
            count++;
        }

        flickeringObject.SetActive(true);
    }

    void Start()
    {
        try
        {
            string path = System.IO.Path.Combine(Application.dataPath + "/server_url.txt");
            Debug.Log("path : " + path);

            byte[] byteText = System.IO.File.ReadAllBytes(path);
            string urlText = Encoding.Default.GetString(byteText);
            _url = "http://" + urlText + "/";
            Debug.Log("url text : " + urlText);
            gameReadyText.text = "Connecting With Server...\n" + urlText;
        }
        catch (FileNotFoundException e)
        {
            Debug.Log("SERVER URL TEXT FILE NOT FOUND");
            gameReadyText.fontSize = 18;
            gameReadyText.text = "SERVER URL TEXT FILE NOT FOUND, please write server ip in \"build\\AirHockey_Data\\server_url.txt\"";
        }
        
        WebSocket = Socket.Connect(_url);
        WebSocket.On("connect", ConnectCallback);
        WebSocket.On("userReadyOn", UserReadyCallback);
        WebSocket.On("gameStart", GameStartCallback);
        WebSocket.On("scoreUpEmit", ScoreUpCallback);
        WebSocket.On("timerEmit", GameTimerCallback);
        WebSocket.On("itemEmit", ItemCallback);
        WebSocket.On("eatItemEmit", ItemEatCallback);
        WebSocket.On("endItemEmit", ItemEndCallback);
        WebSocket.On("feverTimeEmit", FeverTimeCallback);
        WebSocket.On("gameEndEmit", GameEndCallback);
        
        BallReset(); // 공 위치 초기화 
    }

    private void ConnectCallback()
    {
        _isConnect = true;
        gameReadyText.text = "Press Space Bar To Ready!";
    }

    private void ShowPlayer1Camera()
    {
        player2Camera.enabled = false;
        player1Camera.enabled = true;
    }

    private void ShowPlayer2Camera()
    {
        player1Camera.enabled = false;
        player2Camera.enabled = true;
    }

    private void ItemEatCallback(string data)
    {
        ItemModel itemModel = JsonUtility.FromJson<ItemModel>(data);

        switch (ItemModel.ParseStringToItemNameEnum(itemModel.itemName))
        {
            case ItemNameEnum.None:
                break;
            case ItemNameEnum.DoubleScore:
                GameObject.FindWithTag("BALL").GetComponent<BallScript>().ChangeToDoubleScoreBall(itemModel.player);
                break;
            case ItemNameEnum.BigGoal:
                GoalScript bigGoal = null;
                if (itemModel.player == "P1") bigGoal = GameObject.FindWithTag("P1GOAL").GetComponent<GoalScript>();
                else bigGoal = GameObject.FindWithTag("P2GOAL").GetComponent<GoalScript>();
                bigGoal.BigGoalModeStart(itemModel.player);
                break;
            case ItemNameEnum.SmallGoal:
                GoalScript smallGoal = null;
                if (itemModel.player == "P1") smallGoal = GameObject.FindWithTag("P2GOAL").GetComponent<GoalScript>();
                else smallGoal = GameObject.FindWithTag("P1GOAL").GetComponent<GoalScript>();
                smallGoal.SmallGoalModeStart(itemModel.player);
                break;
            case ItemNameEnum.PenaltyKick:
                hockeyBall.GetComponent<BallScript>().PenaltyKickMode(itemModel.player);
                break;
        }
    }

    private void ItemEndCallback(string data)
    {
        ItemModel itemModel = JsonUtility.FromJson<ItemModel>(data);

        switch (ItemModel.ParseStringToItemNameEnum(itemModel.itemName))
        {
            case ItemNameEnum.None:
                break;
            case ItemNameEnum.DoubleScore:
                GameObject.FindWithTag("BALL").GetComponent<BallScript>().ChangeToSingleScoreBall();
                break;
            case ItemNameEnum.BigGoal:
                GoalScript bigGoal = null;
                if (itemModel.player == "P1") bigGoal = GameObject.FindWithTag("P1GOAL").GetComponent<GoalScript>();
                else bigGoal = GameObject.FindWithTag("P2GOAL").GetComponent<GoalScript>();
                bigGoal.BigGoalModeEnd(itemModel.player);
                break;
            case ItemNameEnum.SmallGoal:
                GoalScript smallGoal = null;
                if (itemModel.player == "P1") smallGoal = GameObject.FindWithTag("P2GOAL").GetComponent<GoalScript>();
                else smallGoal = GameObject.FindWithTag("P1GOAL").GetComponent<GoalScript>();
                smallGoal.SmallGoalModeEnd(itemModel.player);
                break;
            case ItemNameEnum.PenaltyKick:
                break;
        }
    }

    private void ItemCallback(string data)
    {
        if (PlayerType == "P2") return;

        ItemModel itemModel = JsonUtility.FromJson<ItemModel>(data);
        GameItemScript gameItemScript = null;

        Debug.Log("item name : " + itemModel.itemName);

        switch (ItemModel.ParseStringToItemNameEnum(itemModel.itemName))
        {
            case ItemNameEnum.None:
                break;
            case ItemNameEnum.DoubleScore:
                gameItemScript = doubleGameItem.GetComponent<GameItemScript>();
                break;
            case ItemNameEnum.BigGoal:
                gameItemScript = bigGoalGameItem.GetComponent<GameItemScript>();
                break;
            case ItemNameEnum.SmallGoal:
                gameItemScript = smallGoalGameItem.GetComponent<GameItemScript>();
                break;
            case ItemNameEnum.PenaltyKick:
                gameItemScript = penaltyKickGameItem.GetComponent<GameItemScript>();
                break;
        }

        if (gameItemScript != null)
        {
            if (!(gameItemScript.GetIsAliveItem())) gameItemScript.RespawnItem(itemModel);
        }
    }

    public void FeverTimeCallback(string data)
    {
        StartCoroutine(ShowFlickering(feverTimeText));
    }

    private void GameEndCallback(string data)
    {
        GameEndModel gameEndModel = JsonUtility.FromJson<GameEndModel>(data);

        hockeyBall.SetActive(false);

        ShowWinner(gameEndModel);
    }

    private void ShowWinner(GameEndModel gameEndModel)
    {
        gameOverText.SetActive(true);
        Text winnerTextObj = winnerText.GetComponent<Text>();
        if (gameEndModel.IsDraw())
        {
            winnerTextObj.text = "DRAW!";
        }
        else if ((gameEndModel.winner == "P1" && PlayerType == "P1") ||
                 (gameEndModel.winner == "P2" && PlayerType == "P2"))
        {
            winnerTextObj.text = "YOU WIN!!!";
        }
        else
        {
            winnerTextObj.text = "YOU LOSE..";
        }

        StartCoroutine(ShowFlickering(winnerText));
        StartCoroutine(ShowFlickering(gameOverText));
    }

    private void ScoreUpCallback(string data)
    {
        ScoreModel scoreModel = JsonUtility.FromJson<ScoreModel>(data);
        if (PlayerType == "P1")
        {
            youScore.text = scoreModel.p1Score;
            rivalScore.text = scoreModel.p2Score;
        }
        else
        {
            youScore.text = scoreModel.p2Score;
            rivalScore.text = scoreModel.p1Score;
        }
    }

    private void GameTimerCallback(string data)
    {
        GameTimeModel gameTimeModel = JsonUtility.FromJson<GameTimeModel>(data);
        gameTime.text = gameTimeModel.ToString();
    }

    private void UserReadyCallback(string data) // Player1, Player2 지정에 대한 대기
    {
        PlayerModel playerModel = JsonUtility.FromJson<PlayerModel>(data);
        PlayerType = playerModel.player;
        Debug.Log("userReadyOn : " + PlayerType);

        if (PlayerType == "P1") NetworkCustomManager.StartHostCustom();
    }

    private void GameStartCallback(string data) // 게임 시작 신호 대기
    {
        Debug.Log("game start");
        
        BallReset();
        
        ipModel = JsonUtility.FromJson<IpModel>(data);

        gameReadyText.text = "";
        gameReadyTextBgImage.enabled = false;

        if (PlayerType == "P2")
        {
            NetworkCustomManager.StartClientCustom(ipModel.hostIp, 7777);
            Debug.Log("P2, Host ip : " + ipModel.hostIp);
            ShowPlayer2Camera();
            InvalidCollider(); // Player2일 경우 물리 계산 disable
        }
        else
        {
            ShowPlayer1Camera();
        }
    }

    private void InvalidCollider() // Player 2일 경우에, 공과 벽은 충돌하면 안됨
    {
        Debug.Log("invalid collider");

        // 벽과 바닥의 object에 대해 충돌 disable
        var walls = GameObject.FindGameObjectsWithTag("WALL");
        var floors = GameObject.FindGameObjectsWithTag("FLOOR");
        var balls = GameObject.FindGameObjectsWithTag("BALL");
        var items = GameObject.FindGameObjectsWithTag("ITME");
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

        foreach (var item in items)
        {
            item.GetComponent<Rigidbody>().isKinematic = true;
            item.GetComponent<Rigidbody>().detectCollisions = false;
        }
    }

    private void BallReset()
    {
        hockeyBall.GetComponent<Rigidbody>().velocity = Vector3.zero;
        hockeyBall.transform.position = new Vector3(BallScript.PLAYER1_RESET_X, hockeyBall.transform.position.y,
            (BallScript.PLAYER1_RESET_Z + BallScript.PLAYER2_RESET_Z) / 2f);
    }

    private static void ReadyGame()
    {
        if (!_alreadyReady && _isConnect)
        {
            string myIp = IPManager.GetIP(ADDRESSFAM.IPv4);
            Debug.Log("myIp : " + myIp);
            var emitJsonStr = "{\"ip\": \"" + myIp + "\"}";
            WebSocket.EmitJson("userReady", emitJsonStr);
            _alreadyReady = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 스페이스 바 눌리면 레디 (1회만)
        if (Input.GetKeyDown(KeyCode.Space) && _isConnect)
        {
            gameReadyText.text = "Waiting for rival...";
            ReadyGame();
        }

        // R키 눌리면 공 위치 초기화
        if (Input.GetKeyDown(KeyCode.R) && PlayerType == "P1") BallReset();

        _controller = new Controller();
        Frame frame = _controller.Frame();
        List<Hand> hands = frame.Hands;

        if (frame.Hands.Count > 0)
        {
            var handPosition = hands[0].PalmPosition;
            string stickTag = "";
            if (PlayerType == "P1")
            {
                stickTag = "STICK1";
                var x = handPosition.x / 700f;
                var y = StickScript.StickYPosition;
                var z = handPosition.z / -700f + 0.75f;
                foreach (var o in GameObject.FindGameObjectsWithTag(stickTag))
                {
                    o.transform.position = new Vector3(x, y, z);
                }
            }
            else
            {
                stickTag = "STICK2";

                var x = handPosition.x * -1f / 700f;
                var y = StickScript.StickYPosition;
                var z = (handPosition.z / -700f + 0.2f) * -1f + 1.4f;
                foreach (var o in GameObject.FindGameObjectsWithTag(stickTag))
                {
                    o.transform.position = new Vector3(x, y, z);
                }
            }
        }
    }
}