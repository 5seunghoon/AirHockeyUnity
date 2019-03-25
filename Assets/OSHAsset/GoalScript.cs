using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalScript : MonoBehaviour
{

    public static Vector3 resetVector3 = new Vector3(0f, -0.1145f, 0.304f);

    // Start is called before the first frame update
    void Start()
    {

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
            
            // 여러번 Emit 됨 : 1초에 1회만 Emit되게 수정하면 좋을듯
            ControllMove.socket.EmitJson("SCORE_UP", "{\"player\":\"P1\",\"score\":\"1\"}");
            
            col.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            col.gameObject.transform.position = resetVector3;
            
        }
    }
}
