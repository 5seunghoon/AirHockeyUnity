using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemText : MonoBehaviour
{
    public Text itemText;
    // Start is called before the first frame update
    void Start()
    {
        itemText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(BlinkText());
    }

    public IEnumerator BlinkText()
    {
        while (true)
        {
            itemText.text = "";
            yield return new WaitForSeconds(.5f);
            yield return new WaitForSeconds(.5f);
        }
    }
}
