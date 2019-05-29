using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player2_score : MonoBehaviour
{
    public int player2Score;
    public Text player2ScoreText;
    // Start is called before the first frame update
    void Start()
    {
        player2ScoreText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        player2ScoreText.text = player2Score.ToString();
    }
}
