using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkCustomManager : NetworkManager
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public static void StartHostCustom()
    {
        singleton.StartHost();
    }

    public static void StartClientCustom(string hostIp, int hostPort)
    {
        singleton.networkAddress = hostIp;
        singleton.networkPort = hostPort;
        singleton.StartClient();
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        Debug.Log("OnServerConnect");
        base.OnServerConnect(conn);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        Debug.Log("OnClientConnect");
        if (!ControllMove.alreadyReady && ControllMove.isConnect)
        {
            ControllMove.ReadyGame();
        }
        base.OnClientConnect(conn);
    }
}