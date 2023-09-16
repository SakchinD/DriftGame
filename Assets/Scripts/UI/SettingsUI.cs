using Cysharp.Threading.Tasks;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using Zenject;

public class SettingsUI : MonoBehaviour
{
    [Header("UI elements")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider soundSlider;
    [SerializeField] private Button backButton;
    [SerializeField] private Button saveEndExitButton;
    [SerializeField] private Button applyNickNameButton;
    [SerializeField] private TMP_InputField nickNameInputText;
    [SerializeField] private TMP_Text playerNickNameText;

    [SerializeField] private AudioMixer audioMixer;

    private CancellationTokenSource cancellation;

    private IPlayerSettings playerSettings;
    private UiController uiController;
    private ISaveManager saveManager;
    private AudioManager audioManager;

    private string nickName;
    private float currentMusicVolume;
    private float currentSoundVolume;
    private float musicVolume;
    private float soundVolume;

    [Inject]
    private void Construct(IPlayerSettings playerSettings,
        UiController uiController,
        ISaveManager saveManager,
        AudioManager audioManager)
    {
        this.playerSettings = playerSettings;
        this.uiController = uiController;
        this.saveManager = saveManager;
        this.audioManager = audioManager;
        SincSetiings().Forget();
    }

    private async UniTask SincSetiings()
    {
        cancellation = new CancellationTokenSource();
        await UniTask.WaitUntil(() => saveManager.SettingsDataLoaded, 
            cancellationToken: cancellation.Token);

        if (cancellation.IsCancellationRequested)
            return;

        SetupSoundSettings();
        StopSettingsSinc();
    }

    private void Awake()
    {
        musicSlider.onValueChanged.AddListener(OnMusicSliderChange);
        soundSlider.onValueChanged.AddListener(OnSoundSliderChange);
        backButton.onClick.AddListener(OnBackButtonClick);
        saveEndExitButton.onClick.AddListener(OnSaveAndExitButtonClick);
        applyNickNameButton.onClick.AddListener(OnApplyNickNameButtonClick);
    }

    private void OnEnable()
    {
        StopSettingsSinc();
        SetPlayerNickNameText(playerSettings.NickName);
        SetupSoundSettings();
        currentMusicVolume = playerSettings.MusicVolume;
        currentSoundVolume = playerSettings.SoundVolume;
        nickName = playerSettings.NickName;
        nickNameInputText.text = string.Empty;
    }

    private void SetupSoundSettings()
    {
        musicSlider.value = playerSettings.MusicVolume;
        soundSlider.value = playerSettings.SoundVolume;
        SetMusicMixerVolum(musicSlider.value);
        SetSoundMixerVolum(soundSlider.value);
    }

    private void OnDestroy()
    {
        StopSettingsSinc();
        musicSlider.onValueChanged.RemoveAllListeners();
        soundSlider.onValueChanged.RemoveAllListeners();
        backButton.onClick.RemoveAllListeners();
        saveEndExitButton.onClick.RemoveAllListeners();
        applyNickNameButton.onClick.RemoveAllListeners();
    }

    private void SetMusicMixerVolum(float value)
    {
        audioMixer.SetFloat("MusicVolume", value);
    }

    private void SetSoundMixerVolum(float value)
    {
        audioMixer.SetFloat("SoundVolume", value);
    }

    private void OnMusicSliderChange(float value)
    {
        SetMusicMixerVolum(value);
        musicVolume = value;
    }

    private void OnSoundSliderChange(float value)
    {
        SetSoundMixerVolum(value);
        soundVolume = value;
        PlayClickSound();
    }

    private void OnBackButtonClick()
    {
        OnMusicSliderChange(currentMusicVolume);
        OnSoundSliderChange(currentSoundVolume);
        uiController.ToUi(UiType.StartUI);
        PlayClickSound();
    }

    private void OnSaveAndExitButtonClick()
    {
        PhotonNetwork.NickName = nickName;
        playerSettings.SetNickName(nickName);
        playerSettings.SetMusicVolume(musicVolume);
        playerSettings.SetSoundVolume(soundVolume);
        saveManager.SaveSettingsData();
        uiController.ToUi(UiType.StartUI);
        PlayClickSound();
    }

    private void OnApplyNickNameButtonClick()
    {
        if (string.IsNullOrEmpty(nickNameInputText.text))
            return;

        nickName = nickNameInputText.text;
        SetPlayerNickNameText(nickName);
        PlayClickSound();
    }

    private void SetPlayerNickNameText(string nickName)
    {
        playerNickNameText.text = nickName;
    }

    private void PlayClickSound()
    {
        audioManager.PlayAudio(BaseSound.Click);
    }

    private void StopSettingsSinc()
    {
        if (cancellation != null)
        {
            cancellation?.Cancel();
            cancellation?.Dispose();
            cancellation = null;
        }
    }
}
