using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class ReadySignal : MonoBehaviour
{
    public Text Ready;
    public int size = 1;

    void Start()
    {
        Ready = GetComponent<Text>();

    }

    void Update()
    {
        //Ready.fontSize = Screen.height * size / 27;
        if (Input.GetButtonDown("Jump"))  //스페이스바 입력 인식
        {
            StartCoroutine(BlinkText());   
        }
    }

    public IEnumerator BlinkText()
    {
        while (true)
        {
            Ready.text = "";
            yield return new WaitForSeconds(.5f);
            Ready.text = "Ready";         //Ready 표시
            yield return new WaitForSeconds(.5f);
        }
    }
}
