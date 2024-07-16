using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.SceneManagement;

public class PongNetworkManager : NetworkManager
{
    public List<PongPlayer> Players { get; } = new List<PongPlayer>();

    public static event Action aClientOnConnected; 
    public static event Action aClientOnDisconnected;

    [SerializeField] GameObject playerBarPrefab;

    public override void OnStartServer()
    {
        base.OnStartServer();
        for (int i = 0; i < Players.Count; i++)
        {
            GameObject barInstance =
                Instantiate(playerBarPrefab, GetStartPosition().position, Quaternion.identity);

            NetworkServer.Spawn(barInstance, Players[i].connectionToClient);
        }
    }

    public override void OnServerSceneChanged(string newSceneName)
    {
        for(int i = 0; i < Players.Count; i++)
        {
            GameObject barInstance =
                Instantiate(playerBarPrefab, GetStartPosition().position, Quaternion.identity);

            NetworkServer.Spawn(barInstance, Players[i].connectionToClient);
        }
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        PongPlayer player = conn.identity.GetComponent<PongPlayer>();
        Players.Add(player);

        player.SetPlayerName($"Player {Players.Count}");

        //player.SetIsPartyOwner(Players.Count == 1);
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        PongPlayer player = conn.identity.GetComponent<PongPlayer>();
        Players.Add(player);

        player.SetPlayerName($"Player {Players.Count}");

        base.OnServerConnect(conn);
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        PongPlayer player = conn.identity.GetComponent<PongPlayer>();

        Players.Remove(player);

        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        Players.Clear();
    }

    public void StartGame()
    {
        if (Players.Count != 2) return;

        ServerChangeScene("Game");
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();

        aClientOnConnected?.Invoke();
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();

        aClientOnDisconnected?.Invoke();
    }
}
