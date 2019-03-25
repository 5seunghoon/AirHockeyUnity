using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalScript : MonoBehaviour
{

    public static Vector3 resetVector3 = new Vector3(0f, -0.1145f, 0.304f);

    // Start is called before the first frame update
    void Start()
    {
        ControllMove.socket.On("connect", (data) => {
            Debug.Log("Hello, socket.io~");
        });

        // "news" 이벤트 처리 Receive
        ControllMove.socket.On("news", (data) => {
            Debug.Log(data);

            // "my other event" 이벤트 Send
            ControllMove.socket.Emit(
                "my other event",       // 이벤트명
                "{ \"my\": \"data\" }"  // 데이터 (Json 텍스트)
                );
        });

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider col)
    {
        // 벽에 닿으면 공 위치 초기화
        if(col.tag == "Ball")
        {
            Debug.Log("Ball Trigger Enter");
            col.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            col.gameObject.transform.position = resetVector3;
        }
    }
}
