using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StartText : MonoBehaviour
{
    public Text startText;
    public int count;

    private new AudioSource audio;
    public AudioClip startSound;
    // Start is called before the first frame update
    void Start()
    {
        startText = GetComponent<Text>();

        this.audio = this.gameObject.AddComponent<AudioSource>();
        this.audio.clip = this.startSound;
        this.audio.loop = false;
    }
    // Update is called once per frame
    void Update()
    {

        if (Input.GetButtonDown("Jump"))   //스페이스바 입력 인식
        {
            StartCoroutine(BlinkText(count));
        }

    }

    public IEnumerator BlinkText(int count)
    {
        for (count = 3; count > 0; count--)    //3 2 1 count
        {
            startText.text = "";
            yield return new WaitForSeconds(.5f);
            startText.text = count.ToString();      
            yield return new WaitForSeconds(.5f);
        }
        startText.text = "";
        yield return new WaitForSeconds(.5f);
        this.audio.Play();
        startText.text = "S  T  A  R  T  !";   //start 
        yield return new WaitForSeconds(.5f);
        Destroy(startText);
    }
}
