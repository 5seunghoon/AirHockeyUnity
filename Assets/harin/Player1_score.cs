using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player1_score : MonoBehaviour
{
    public int player1Score;
    public Text player1ScoreText;
    // Start is called before the first frame update
    void Start()
    {
        player1ScoreText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        player1ScoreText.text = player1Score.ToString();
    }
}
