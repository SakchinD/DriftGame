using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : PoolItem
{
    [SerializeField] private TMP_Text roomNameText;
    [SerializeField] private Button interRoomButton;

    private string roomName;

    private string roomMaxPlayers;

    public new string Name => roomName;

    public override void SetInfo(params string[] values)
    {
        roomName = values[0];
        roomNameText.text = $"{roomName}: {values[1]}";
    }

    private void Awake()
    {
        interRoomButton.onClick.AddListener(OnInterRoomButtonClick);
    }

    private void OnDestroy()
    {
        interRoomButton.onClick.RemoveAllListeners();
    }

    private void OnInterRoomButtonClick()
    {
        PhotonNetwork.JoinRoom(roomName);
    }
}
