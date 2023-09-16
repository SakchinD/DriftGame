using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class RoomUI : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button leaveRoomButton;
    [SerializeField] private RectTransform playersContent;

    private List<PoolItem> players = new();

    private UiController uiController;
    private ObjectPoolController poolController;
    private AudioManager audioManager;

    [Inject]
    private void Construct(UiController uiController,
        ObjectPoolController poolController,
        AudioManager audioManager)
    {
        this.uiController = uiController;
        this.poolController = poolController;
        this.audioManager = audioManager;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddPlayer(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        var item = players.FirstOrDefault(x => x.Name == otherPlayer.NickName);
        if (item)
        {
            item.ResetItem();
            players.Remove(item);
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();
        GetCuurentRoomPlayers();
        startGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
    }

    private void GetCuurentRoomPlayers()
    {
        if (!PhotonNetwork.IsConnected)
            return;

        if (PhotonNetwork.CurrentRoom == null || PhotonNetwork.CurrentRoom.Players == null)
            return;

        foreach (KeyValuePair<int, Player> playerInfo in PhotonNetwork.CurrentRoom.Players)
        {
            AddPlayer(playerInfo.Value);
        }
    }

    private void AddPlayer(Player newPlayer)
    {
        var player = poolController.GetPooledObject(PoolType.Player);
        player.transform.SetParent(playersContent);
        player.transform.localScale = Vector3.one;
        player.SetInfo(new string[]
        { 
            newPlayer.NickName
        });
        players.Add(player);
        player.InvokeItem();
    }

    private void Awake()
    {
        startGameButton.onClick.AddListener(OnStartGameButtonClick);
        leaveRoomButton.onClick.AddListener(OnLeaveRoomButtonClick);
    }

    private void OnDestroy()
    {
        startGameButton.onClick.RemoveListener(OnStartGameButtonClick);
        leaveRoomButton.onClick.RemoveListener(OnLeaveRoomButtonClick);
    }

    private void OnStartGameButtonClick()
    {
        PlayClickSound();
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.LoadLevel("Game");
        }
    }

    private void OnLeaveRoomButtonClick()
    {
        PlayClickSound();
        PhotonNetwork.LeaveRoom(true);
        uiController.ToUi(UiType.Lobby);
        players.ForEach(x => x.ResetItem());
        players.Clear();
    }

    private void PlayClickSound()
    {
        audioManager.PlayAudio(BaseSound.Click);
    }
}
