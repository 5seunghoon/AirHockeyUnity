using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadyText : MonoBehaviour
{
    public Text readyText;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        readyText.text = "Press the spacebar to be ready";

        if (Input.GetButtonDown("Jump"))   //스페이스바 입력 인식
        {
            Destroy(readyText);            //Ready Text 제거
        }
    }
}
