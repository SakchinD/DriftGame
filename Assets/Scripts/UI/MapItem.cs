using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class MapItem : MonoBehaviour
{
    private readonly string MaxPlayerCountText = "Max {0} players";

    [SerializeField] private Image mapArt;
    [SerializeField] private Button playButton;
    [SerializeField] private Button createRoomButton;
    [SerializeField] private TMP_Text mapName;
    [SerializeField] private TMP_Text maxPlayerCount;

    private Map map;
    private UiController uiController;
    private IGameSettingsManager gameSettingsManager;
    private AudioManager audioManager;

    [Inject]
    private void Construct(UiController uiController,
        IGameSettingsManager gameSettingsManager,
        AudioManager audioManager)
    {
        this.uiController = uiController;
        this.gameSettingsManager = gameSettingsManager;
        this.audioManager = audioManager;
    }

    public void SetupMapItem(Map map)
    {
        mapName.text = map.MapName;
        maxPlayerCount.text = string.Format(MaxPlayerCountText, map.MaxPlayerCount);
        this.map = map;
        mapArt.sprite = map.MapArt;
    }

    private void Awake()
    {
        playButton.onClick.AddListener(OnPlayBuutonClick);
        createRoomButton.onClick.AddListener(OnCreateRoomButtonClick);
    }

    private void OnDestroy()
    {
        playButton.onClick.RemoveListener(OnPlayBuutonClick);
        createRoomButton.onClick.RemoveListener(OnCreateRoomButtonClick);
    }

    private void OnPlayBuutonClick()
    {
        PlayClickSound();
        gameSettingsManager.CreateGameSettings(false, map);
        SceneManager.LoadScene("Game");
    }

    private void OnCreateRoomButtonClick()
    {
        PlayClickSound();
        TypedLobby customLobby = new TypedLobby(map.name, LobbyType.Default);

        NetworkManager.lastLobby = customLobby;
        
        gameSettingsManager.CreateGameSettings(true, map);
        
        if (!PhotonNetwork.InLobby)
            PhotonNetwork.JoinLobby(customLobby);
        
        uiController.ToUi(UiType.Lobby);
    }

    private void PlayClickSound()
    {
        audioManager.PlayAudio(BaseSound.Click);
    }
}
