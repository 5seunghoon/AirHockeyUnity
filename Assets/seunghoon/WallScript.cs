using System;
using System.Collections;
using System.Collections.Generic;
using Leap;
using seunghoon;
using UnityEngine;
using UnityEngine.Serialization;

public class WallScript : MonoBehaviour
{

    public WallTypeEnum wallTypeType;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("BALL"))
        {
            switch (wallTypeType)
            {
                case WallTypeEnum.LEFT:
                    ReflectionVelocity(other.rigidbody, Vector3.right);
                    break;
                case WallTypeEnum.RIGHT:
                    ReflectionVelocity(other.rigidbody, Vector3.left);
                    break;
            }
        }
    }

    private void ReflectionVelocity(Rigidbody r, Vector3 inNormal)
    {
        r.velocity = Vector3.Reflect(r.velocity, inNormal);
    }
}
