using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class LobbyUI : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button backButton;
    [SerializeField] private Button createRoomButton;
    [SerializeField] private Button joinRoomButton;
    [SerializeField] private TMP_InputField roomNameInput;
    [SerializeField] private RectTransform roomsContent;

    private List<PoolItem> rooms = new();

    private IGameSettingsManager gameSettingsManager;
    private UiController uiController;
    private ObjectPoolController poolController;
    private AudioManager audioManager;

    [Inject]
    private void Construct(IGameSettingsManager gameSettingsManager,
        UiController uiController,
        ObjectPoolController poolController,
        AudioManager audioManager)
    {
        this.gameSettingsManager = gameSettingsManager;
        this.uiController = uiController;
        this.poolController = poolController;
        this.audioManager = audioManager;
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            if (info.RemovedFromList)
            {
                var item = rooms.FirstOrDefault(x => x.Name == info.Name);
                if (item)
                {
                    item.ResetItem();
                    rooms.Remove(item);
                }
            }
            else
            {
                var item = rooms.FirstOrDefault(x => x.Name == info.Name);
                if (!item)
                {
                    var room = poolController.GetPooledObject(PoolType.Room);
                    room.transform.SetParent(roomsContent);
                    room.transform.localScale = Vector3.one;
                    room.SetInfo(new string[]
                        {
                        info.Name, info.MaxPlayers.ToString() 
                        });
                    rooms.Add(room);
                    room.InvokeItem();
                }
            }
        }
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room Create");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log($"Room create failed: {message}");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Join Room");
        uiController.ToUi(UiType.Room);
        rooms.ForEach(x => x.ResetItem());
        rooms.Clear();
    }

    public override void OnLeftLobby()
    {
        Debug.Log("LeftLobby");
        uiController.ToUi(UiType.MapSelect);
        rooms.ForEach(x => x.ResetItem());
        rooms.Clear();
    }

    private void Awake()
    {
        backButton.onClick.AddListener(OnBackButtonClick);
        createRoomButton.onClick.AddListener(OnCreateRoomClick);
        joinRoomButton.onClick.AddListener(OnJointRandomRoomClick);
    }

    private void OnDestroy()
    {
        backButton.onClick.RemoveListener(OnBackButtonClick);
        createRoomButton.onClick.RemoveListener(OnCreateRoomClick);
        joinRoomButton.onClick.RemoveListener(OnJointRandomRoomClick);
    }

    private void OnCreateRoomClick()
    {
        PLayClickSound();
        if (string.IsNullOrEmpty(roomNameInput.text) || !PhotonNetwork.IsConnected)
            return;

        var roomOptions = new RoomOptions
        {
            MaxPlayers = gameSettingsManager.Map.MaxPlayerCount
        };

        PhotonNetwork.CreateRoom(roomNameInput.text, roomOptions);
    }

    private void OnJointRandomRoomClick()
    {
        PLayClickSound();
        PhotonNetwork.JoinRandomRoom();
    }

    private void OnBackButtonClick()
    {
        PLayClickSound();
        if (!PhotonNetwork.InLobby || !PhotonNetwork.IsConnected)
        {
            uiController.ToUi(UiType.MapSelect);
            rooms.ForEach(x => x.ResetItem());
            rooms.Clear();
        }

        PhotonNetwork.LeaveLobby();
    }

    private void PLayClickSound()
    {
        audioManager.PlayAudio(BaseSound.Click);
    }
}
