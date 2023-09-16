using System.Collections.Generic;
using System;
using UnityEngine;

public enum UiType
{
    StartUI,
    MapSelect,
    Lobby,
    Room,
    Garage,
    Settings
}

public class UiController : MonoBehaviour
{
    [Serializable]
    public class UiView
    {
        public UiType type;
        public GameObject view;
    }
    [SerializeField] private List<UiView> viewList = new();

    private void Start()
    {
        ToUi(UiType.StartUI);
    }

    public void ToUi(UiType uiType)
    {
        var isMainMenu = uiType == UiType.StartUI;

        viewList.ForEach(uiView =>
        {
            var isSame = uiView.type == uiType;
            uiView.view.SetActive(isSame);
        });
    }
}
