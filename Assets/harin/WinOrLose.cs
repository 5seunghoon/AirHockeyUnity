using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WinOrLose : MonoBehaviour
{
    public Text winText;
    public Text loseText;
    // Start is called before the first frame update
    void Start()
    {
        winText = GetComponent<Text>();
        loseText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
