using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static TypedLobby lastLobby;

    private IPlayerSettings playerSettings;

    [Inject]
    public void Construct(IPlayerSettings playerSettings)
    {
        this.playerSettings = playerSettings;
    }

    public void Start()
    {
        lastLobby = null;
        if (PhotonNetwork.IsConnected)
            return;
        
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = "1";
        PhotonNetwork.NickName = playerSettings.NickName;
        PhotonNetwork.ConnectUsingSettings();
    }

    public static void ConrctLast()
    {
        PhotonNetwork.JoinLobby(lastLobby);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Master Connect");
        if (lastLobby != null)
            PhotonNetwork.JoinLobby(lastLobby);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Lobby Connect");
    }

    public override void OnLeftLobby()
    {
        Debug.Log("Leave Lobby");
    }
}
