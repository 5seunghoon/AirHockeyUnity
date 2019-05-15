using System.Collections;
using System.Collections.Generic;
using seunghoon;
using UnityEngine;
using UnityEngine.Networking;

public class Stick2Script : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            //CmdHi(transform.position);
        }
    }

    [Command]
    public void CmdHi(Vector3 position)
    {
        //transform.position = new Vector3(position.x, position.y, position.z);
    }
   
}