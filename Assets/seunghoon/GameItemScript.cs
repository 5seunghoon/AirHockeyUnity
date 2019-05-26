using System;
using System.Collections;
using System.Collections.Generic;
using seunghoon;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameItemScript : MonoBehaviour
{
    private Vector3 removePosition;
    public float aliveYPosition = -0.091f;
    private const float REMOVE_Y_POSITION = -0.3f;
    public const float Z_POSITION = 0.976f;

    private bool _isAliveItem;

    public ItemNameEnum itemName = ItemNameEnum.None;

    // Start is called before the first frame update
    void Awake()
    {
        removePosition = new Vector3(0f, REMOVE_Y_POSITION, Z_POSITION);
        transform.position = removePosition;
        _isAliveItem = false;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("On Trigger Enter");
        if (_isAliveItem && other.CompareTag("BALL"))
        {
            _isAliveItem = false;
            transform.position = removePosition;
            //서버로 Item먹었다고 Emit
            string eatItemEmitJson = "{" +
                                     "\"itemName\": \"" + ItemModel.parseItemToString(itemName) + "\"," +
                                     " \"player\":\"" + BallScript.whoPush + "\"" +
                                     "}";
            ControllMove.socket.EmitJson("eatItem", eatItemEmitJson);
        }
    }

    public void RespawnItem(ItemModel itemModel)
    {
        _isAliveItem = true;
        float xRandom = Random.Range(-16f, 19f) * 0.01f;
        float zRandom = Random.Range(87f, 108f) * 0.01f;
        gameObject.transform.position = new Vector3(xRandom, aliveYPosition, zRandom);
    }

    public bool GetIsAliveItem()
    {
        return _isAliveItem;
    }
}