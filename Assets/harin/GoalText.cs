using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalText : MonoBehaviour
{
    public Text goalText;
    // Start is called before the first frame update
    void Start()
    {
        goalText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
