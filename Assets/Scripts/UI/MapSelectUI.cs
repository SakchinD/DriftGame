using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MapSelectUI : MonoBehaviour
{
    [SerializeField] private Button backButton;
    [SerializeField] private Transform mapsContent;
    [SerializeField] private List<Map> maps = new List<Map>();
    [SerializeField] private MapItem mapUIPrefab;

    private DiContainer diContainer;
    private UiController uiController;
    private AudioManager audioManager;

    [Inject]
    private void Construct(DiContainer diContainer,
        UiController uiController,
        AudioManager audioManager)
    {
        this.diContainer = diContainer;
        this.uiController = uiController;
        this.audioManager = audioManager;
    }

    private void Awake()
    {
        backButton.onClick.AddListener(OnBackButtonClick);

        maps.ForEach(map =>
        {
            var mapItem = diContainer.InstantiatePrefabForComponent<MapItem>(mapUIPrefab, mapsContent);
            mapItem.SetupMapItem(map);
        });
    }

    private void OnDestroy()
    {
        backButton.onClick.RemoveListener(OnBackButtonClick);
    }

    private void OnBackButtonClick()
    {
        PlayClickSound();
        uiController.ToUi(UiType.StartUI);
    }

    private void PlayClickSound()
    {
        audioManager.PlayAudio(BaseSound.Click);
    }
}
