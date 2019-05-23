using System;
using System.Collections;
using System.Collections.Generic;
using seunghoon;
using UnityEngine;

public class GameItemScript : MonoBehaviour
{
    public Vector3 alivePosition = new Vector3(0f, ALIVE_Y_POSITION, Z_POSITION);
    public Vector3 removePosition = new Vector3(0f, REMOVE_Y_POSITION, Z_POSITION);
    private const float ALIVE_Y_POSITION = -0.091f;
    private const float REMOVE_Y_POSITION = -0.3f;
    private const float Z_POSITION = 0.976f;

    public bool isAlive = false;

    public ItemNameEnum itemName = ItemNameEnum.None;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = removePosition;
    }

    // Update is called once per frame
    void Update()
    {
        //회전 
        transform.Rotate (new Vector3 (0, 0, 45) * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isAlive && other.CompareTag("BALL"))
        {
            isAlive = false;
            transform.position = removePosition;
            
            if (itemName == ItemNameEnum.DoubleScore)
            {
                //서버로 Item먹었다고 Emit
                var emitJsonStr = "{\"itemName\": \"" + "DoubleScore" + "\"}";
                ControllMove.socket.EmitJson("eatItem", emitJsonStr);
            }
        }
    }

    public void RespawnItem(ItemModel itemModel)
    {
        isAlive = true;
        transform.position = alivePosition;
        itemName = ItemModel.ParseStringToItemNameEnum(itemModel.itemName);
    }
}
