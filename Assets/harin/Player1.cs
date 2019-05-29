using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Player1 : MonoBehaviour
{
    public Text player1;
    // Start is called before the first frame update
    private void Start()
    {
        player1 = GetComponent<Text>();
    }
    // Update is called once per frame
    void Update()
    {
        player1.text = "Player 1";
    }
}
