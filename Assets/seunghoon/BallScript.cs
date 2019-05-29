using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Leap;
using seunghoon;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    public const float PLAYER1_RESET_X = 0f;
    public const float PLAYER1_RESET_Z = 0.82f;
    public const float PLAYER2_RESET_X = 0f;
    public const float PLAYER2_RESET_Z = 1.139f;

    public static int ScorePoint = 1;

    private const float Y_POS_DEFAULT = -0.112f;

    private const float MAX_VELOCITY = 0.7f;

    public static string WhoPush = "NONE";

    public Material singleScoreMaterial;
    public Material player1StickMaterial;
    public Material player2StickMaterial;

    public GameObject upWall;
    public GameObject downWall;
    public GameObject leftWall;
    public GameObject rightWall;

    private static float _xBoundStart;
    private static float _xBoundEnd;
    private static float _zBoundStart;
    private static float _zBoundEnd;

    // Start is called before the first frame update
    void Awake()
    {
        _xBoundStart = leftWall.transform.position.x;
        _xBoundEnd = rightWall.transform.position.x;
        _zBoundStart = downWall.transform.position.z;
        _zBoundEnd = upWall.transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        var rigidbody = GetComponent<Rigidbody>();
        
        if (ControllMove.PlayerType == "P2")
        {
            rigidbody.detectCollisions = false;
        }

        Vector3 nowVelocity = GetComponent<Rigidbody>().velocity;
        //Debug.Log("Ball velocity : " + nowVelocity);

        if (rigidbody.velocity.magnitude > 0.06f && rigidbody.velocity.magnitude < 0.18f)
        {
            Debug.Log("PUSH");
            rigidbody.AddForce(rigidbody.velocity.normalized * (rigidbody.velocity.magnitude * 2000f + 1f),
                ForceMode.Impulse);
        }
        else if (rigidbody.velocity.magnitude > MAX_VELOCITY)
        {
            Debug.Log("UNPUSH");
            var velocity = rigidbody.velocity;
            rigidbody.velocity = new Vector3(velocity.x * 0.7f, velocity.x * 0.7f, velocity.x * 0.7f);
        }

        if (ControllMove.PlayerType == "P1")
        {
            var position = transform.position;
            var ballX = position.x;
            var ballZ = position.z;

            if (ballX < _xBoundStart || ballX > _xBoundEnd || ballZ < _zBoundStart || ballZ > _zBoundEnd)
            {
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                transform.position = new Vector3(0f, Y_POS_DEFAULT, (_zBoundStart + _zBoundEnd) * 0.5f);
            }
        }
    }


    public void ChangeToDoubleScoreBall(string player)
    {
        ScorePoint = 2;
        GetComponent<MeshRenderer>().material = (player == "P1") ? player1StickMaterial : player2StickMaterial;
    }

    public void ChangeToSingleScoreBall()
    {
        ScorePoint = 1;
        GetComponent<MeshRenderer>().material = singleScoreMaterial;
    }

    public void BallAddForce(Vector3 inNormal, float stickPower)
    {
        inNormal.x *= stickPower + 0.5f;
        inNormal.z *= stickPower + 0.5f;
        GetComponent<Rigidbody>().AddForce(inNormal, ForceMode.VelocityChange);
    }

    public void PenaltyKickMode(string player)
    {
        Debug.Log("penaltyKick Mode");
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        
        if (ControllMove.PlayerType == "P2") return;

        if (player == "P1")
            transform.position = new Vector3(BallScript.PLAYER1_RESET_X, transform.position.y, BallScript.PLAYER1_RESET_Z);
        else transform.position = new Vector3(BallScript.PLAYER2_RESET_X, transform.position.y, BallScript.PLAYER2_RESET_Z);
    }
}