using ExitGames.Client.Photon;
using ModestTree;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class NetworkGameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private CameraController CameraController;
    [SerializeField] private Button endGameButton;
    [SerializeField] private Mybutton[] inputButtons;

    private IPlayerInventory playerInventory;
    private IGameSettingsManager gameSettingsManager;
    private IPlayerSettings playerSettings;
    private CarsExamples carsExamples;

    [Inject]
    private void Construct(IGameSettingsManager gameSettingsManager,
        IPlayerInventory playerInventory,
        IPlayerSettings playerSettings,
        CarsExamples carsExamples)
    {
        this.gameSettingsManager = gameSettingsManager;
        this.playerInventory = playerInventory;
        this.playerSettings = playerSettings;
        this.carsExamples = carsExamples;
    }

    private void Awake()
    {
        endGameButton.onClick.AddListener(OnEndGameClick);
        PhotonPeer.RegisterType(typeof(PlayerCarSettings), 242, SerializeData, DeSerializaData);
    }

    private void Start()
    {
        var name = carsExamples.Cars
            .First(x => x.Name == playerInventory.CurrentCarName).GameCar.name;
        Instantiate(gameSettingsManager.Map);
        
        Car car;

        if (PhotonNetwork.InRoom)
        {
            var index = PhotonNetwork.PlayerList.IndexOf(PhotonNetwork.LocalPlayer);
            var pos = gameSettingsManager.Map.PlayerStartPositions[index];

            var carGo = PhotonNetwork.Instantiate(name, pos.position, pos.rotation);
            carGo.name = PhotonNetwork.NickName;
            car = carGo.GetComponent<Car>();

            car.SendPlayercarSettings(playerInventory.GetCurrentCarSettings());
            FinalSetupCar(car);
            return;
        }

        var resourcesCar = Resources.Load<Car>(name);
        car = Instantiate(resourcesCar, gameSettingsManager.Map.PlayerStartPositions[0].position, 
            gameSettingsManager.Map.PlayerStartPositions[0].rotation);
        FinalSetupCar(car);
        car.name = playerSettings.NickName;
    }

    private void FinalSetupCar(Car car)
    {
        car.SetSettings(playerInventory.GetCurrentCarSettings());
        car.SetInputButtons(inputButtons);
        CameraController.Init(car.transform);
    }

    private void OnDestroy()
    {
        endGameButton.onClick.RemoveAllListeners();
    }

    private void OnEndGameClick()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            return;
        }
        LeaveGame();
    }

    public override void OnLeftRoom()
    {
        LeaveGame();
    }

    private void LeaveGame()
    {
        SceneManager.LoadScene("Menu");
    }

    private static byte[] SerializeData(object set)
    {
        var str = JsonUtility.ToJson((PlayerCarSettings)set);

        var result = System.Text.Encoding.UTF8.GetBytes(str);

        return result;
    }

    private static object DeSerializaData(byte[] netData)
    {
        var str = System.Text.Encoding.UTF8.GetString(netData);

        return JsonUtility.FromJson<PlayerCarSettings>(str);
    }
}
