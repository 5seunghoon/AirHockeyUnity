using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndText : MonoBehaviour
{
    public Text endText;
    // Start is called before the first frame update
    void Start()
    {
        endText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        endText.text = "E      N      D";
    }
}
