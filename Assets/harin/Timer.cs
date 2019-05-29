using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{

    public Text timeText;
    private float time;
    // Start is called before the first frame update
    //void Awake()
    //{
    //    time = 15f;
    //}

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
    }

    void OnGUI()
    {
        string timeStr;
        timeStr = "" + time.ToString("00.00");
        timeStr = timeStr.Replace(".", ":");
        timeText.text = "Time  " + timeStr;
    }
}
