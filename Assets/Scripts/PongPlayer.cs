using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class PongPlayer : NetworkBehaviour
{
    [SyncVar(hook = nameof(ClientHandleScoreUpdated))]
    private int iScore = 0;
    private bool bIsPartyOwner = false;
    private string sPlayerName;

    public int Score { get { return iScore; } }
    public string PlayerName { get { return sPlayerName; } }

    public event Action<int> ClientActionResourceUpdated;

    [Server]
    public void SetScore(int newScore)
    {
        iScore = newScore;
    }

    [Server]
    public void SetPlayerName(string newName)
    {
        sPlayerName = newName;
    }

    private void ClientHandleScoreUpdated(int oldScore, int newScore)
    {
        ClientActionResourceUpdated?.Invoke(newScore);
    }

    [Command]
    public void CmdStartGame()
    {
        ((PongNetworkManager)NetworkManager.singleton).StartGame();
    }

}
