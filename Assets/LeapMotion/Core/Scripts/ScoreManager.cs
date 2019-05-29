using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    Text scoreLabel;

    void Awake()
    {
        scoreLabel = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        scoreLabel.text = DoubleScore.score.ToString();
    }
}
