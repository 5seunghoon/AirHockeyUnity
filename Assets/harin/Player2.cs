using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Player2 : MonoBehaviour
{
    public Text player2;
    // Start is called before the first frame update
    private void Start()
    {
        player2 = GetComponent<Text>();
    }
    // Update is called once per frame
    void Update()
    {
        player2.text = "Player 2";
    }
}
