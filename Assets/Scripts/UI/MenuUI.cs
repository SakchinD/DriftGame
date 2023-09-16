using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button garageButton;
    [SerializeField] private Button settingsButton;

    private UiController uiController;
    private AudioManager audioManager;

    public GameObject GameObject => gameObject;

    [Inject]
    private void Construct(UiController uiController,
        AudioManager audioManager)
    {
        this.uiController = uiController;
        this.audioManager = audioManager;
    }

    private void Awake()
    {
        startButton.onClick.AddListener(OnlevelSelectButtonClick);
        garageButton.onClick.AddListener(OnGarageButtonClick);
        settingsButton.onClick.AddListener(OnSettingsButtonClick);
    }

    private void Start()
    {
        audioManager.PlayCrossFadeMusic("Menu");
    }

    private void OnDestroy()
    {
        startButton.onClick.RemoveAllListeners();
        garageButton.onClick.RemoveAllListeners();
        settingsButton.onClick.RemoveAllListeners();
    }

    private void OnlevelSelectButtonClick()
    {
        uiController.ToUi(UiType.MapSelect);
        PlayClickSound();
    }

    private void OnGarageButtonClick()
    {
        uiController.ToUi(UiType.Garage);
        PlayClickSound();
    }

    private void OnSettingsButtonClick()
    {
        uiController.ToUi(UiType.Settings);
        PlayClickSound();
    }

    private void PlayClickSound()
    {
        audioManager.PlayAudio(BaseSound.Click);
    }
}
