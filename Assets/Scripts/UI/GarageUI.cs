using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GarageUI : MonoBehaviour
{
    [SerializeField] private Button backButton;
    [Header("Player currency")]
    [SerializeField] private TMP_Text playerCurrncyText;
    [Header("Car select")]
    [SerializeField] private Button previosCarButton;
    [SerializeField] private Button nextCarButton;
    [SerializeField] private TMP_Text selectedCarNameText;
    [Header("Car buy")]
    [SerializeField] private GameObject carBuyLayout;
    [SerializeField] private TMP_Text carBuyPriceText;
    [SerializeField] private Button carBuyButton;
    [Header("Car modifications")]
    [SerializeField] private GameObject modificationLayout;
    [SerializeField] private TMP_Dropdown modificationDropdown;
    [SerializeField] private Button previosModificationButton;
    [SerializeField] private Button nextModificationButton;
    [SerializeField] private Button actionModificationButton;
    [SerializeField] private TMP_Text actionModificationButtonText;
    [SerializeField] private TMP_Text modificationName;
    [SerializeField] private TMP_Text modificationPrice;
    [SerializeField] private Toggle modificationTogle;
    [Header("Color piker")]
    [SerializeField] private Button openColorPickerButton;
    [SerializeField] private Button confirmColorButton;
    [SerializeField] private Button cancelColorButton;
    [SerializeField] private ColorPicker modificationColorPicker;

    private List<Car> garageCars = new();

    private Car selectedCar;
    private List<CarModification> selectedCarMods;
    private CarModification selectedModification;

    private CarsExamples carsExamples;
    private IPlayerInventory playerInventory;
    private ISaveManager saveManager;
    private UiController uiController;
    private AudioManager audioManager;

    [Inject]
    private void Construct(CarsExamples carsExamples,
        IPlayerInventory playerInventory,
        ISaveManager saveManager,
        UiController uiController,
        AudioManager audioManager)
    {
        this.carsExamples = carsExamples;
        this.playerInventory = playerInventory;
        this.saveManager = saveManager;
        this.uiController = uiController;
        this.audioManager = audioManager;
    }

    private void OnDestroy()
    {
        backButton.onClick.RemoveAllListeners();
        nextCarButton.onClick.RemoveAllListeners();
        previosCarButton.onClick.RemoveAllListeners();
        carBuyButton.onClick.RemoveAllListeners();
        modificationDropdown.onValueChanged.RemoveAllListeners();
        nextModificationButton.onClick.RemoveAllListeners();
        previosModificationButton.onClick.RemoveAllListeners();
        openColorPickerButton.onClick.RemoveAllListeners();
        confirmColorButton.onClick.RemoveAllListeners();
        cancelColorButton.onClick.RemoveAllListeners();

        modificationColorPicker.OnColorChange -= OnColorPickerColorChange;
    }

    private void Awake()
    {
        backButton.onClick.AddListener(OnBackButtonClick);
        nextCarButton.onClick.AddListener(OnNextCarClick);
        previosCarButton.onClick.AddListener(OnPreviosCarClick);
        carBuyButton.onClick.AddListener(OnCarBuyClick);
        modificationDropdown.onValueChanged.AddListener(OnModificationDropdownClange);
        nextModificationButton.onClick.AddListener(OnNextModification);
        previosModificationButton.onClick.AddListener(OnPrevoisModification);
        openColorPickerButton.onClick.AddListener(OnOpenColorPickerClick);
        confirmColorButton.onClick.AddListener(OnConfirmColorButtonClick);
        cancelColorButton.onClick.AddListener(OnCancelColorButtonClick);

        modificationColorPicker.Init();
        modificationColorPicker.OnColorChange += OnColorPickerColorChange;

        carsExamples.Cars.ForEach(car =>
        {
            var garageCar = Instantiate(car.GarageCar, Vector3.zero, Quaternion.identity);
            garageCar.name = car.Name;
            garageCars.Add(garageCar);
        });
    }

    private void OnBackButtonClick()
    {
        saveManager.SavePlayerData();
        uiController.ToUi(UiType.StartUI);
        PlayClickSound();
    }

    private void OnColorPickerColorChange(Color color)
    {
        if (selectedCar)
            selectedCar.SetCarColcor(color);
    }

    private void OnEnable()
    {
        if (string.IsNullOrEmpty(playerInventory.CurrentCarName))
            return;

        SelectCar(playerInventory.CurrentCarName);
        UpdatePlayerCurrencyText();
    }

    private void UpdatePlayerCurrencyText()
    {
        playerCurrncyText.text = $"{playerInventory.Money}$";
    }

    private void OnOpenColorPickerClick()
    {
        modificationColorPicker.gameObject.SetActive(true);
        PlayClickSound();
    }

    private void OnConfirmColorButtonClick()
    {
        selectedCar.InstallColor();
        playerInventory.SetNewCarColor(selectedCar.name, modificationColorPicker.CurrentColor);
        modificationColorPicker.gameObject.SetActive(false);
        PlayClickSound();
    }

    private void  OnCancelColorButtonClick()
    {
        selectedCar.ResetColor();
        modificationColorPicker.gameObject.SetActive(false);
        PlayClickSound();
    }

    private void OnCarBuyClick()
    {
        var car = carsExamples.Cars
            .First(x => x.Name == selectedCar.name);

        playerInventory.SpendMoney(car.CarPrice);
        playerInventory.BuyCar(selectedCar.name);
        UpdatePlayerCurrencyText();
        SetupCarInfo();
        PlayBuySound();
    }

    private void OnNextCarClick()
    {
        var index = GetNextIndex(garageCars.IndexOf(garageCars.First(x => x.name == selectedCar.name)), garageCars.Count);

        SelectCar(garageCars[index].name);
        PlayClickSound();
    }

    private void OnPreviosCarClick()
    {
        var index = GetPreviosIndex(garageCars.IndexOf(garageCars.First(x => x.name == selectedCar.name)), garageCars.Count);

        SelectCar(garageCars[index].name);
        PlayClickSound();
    }

    private void SelectCar(string carName)
    {
        garageCars.ForEach(garageCar =>
        {
            var isCurrentCar = garageCar.name == carName;
            garageCar.gameObject.SetActive(isCurrentCar);

            if (isCurrentCar)
            {
                selectedCar = garageCar;
                selectedCarNameText.text = garageCar.name;
                garageCar.SetSettings(GetPlayerCarSettings(garageCar.name));
            }
        });

        SetupCarInfo();
    }

    private void SetupCarInfo()
    {
        var isBought = playerInventory.Cars.Contains(selectedCar.name);

        if (isBought)
        {
            playerInventory.SetCurrentCar(selectedCar.name);
            carBuyLayout.SetActive(false);
            modificationLayout.SetActive(true);
            modificationColorPicker.gameObject.SetActive(false);

            var modificationTypes = carsExamples.Cars.First(x => x.Name == selectedCar.name)
                .CarModifications
                    .Select(x=>x.Place.ToString())
                    .Distinct()
                    .ToList();
            modificationDropdown.ClearOptions();
            modificationDropdown.AddOptions(modificationTypes);
            OnModificationDropdownClange(modificationDropdown.value);
            return;
        }

        carBuyLayout.SetActive(true);
        modificationLayout.SetActive(false);

        var car = carsExamples.Cars
            .First(x => x.Name == selectedCar.name);

        carBuyPriceText.text = $"{car.CarPrice}$";
        carBuyButton.interactable = playerInventory.HasEnoughMoney(car.CarPrice);
    }

    private void OnModificationDropdownClange(int value)
    {
        var modificationPlace = modificationDropdown.options[value].text;
        selectedCarMods = carsExamples.Cars
            .First(x => x.Name == selectedCar.name)
            .CarModifications
                .Where(x=>x.Place.ToString() == modificationPlace)
                .ToList();

 
        var carSettings = playerInventory.CarsSettings
            .FirstOrDefault(x => x.CarName == selectedCar.name);

        var carMods = carSettings.PlayerCarModifications.Where(x => x.Place.ToString() == modificationPlace).ToArray();

        var modificationSelectedName = carMods == null
            ? selectedCarMods[0].Name
            : carMods.First(x => x.Installed).ModificationName;

        SetupModificationInfo(modificationSelectedName);
        PlayClickSound();
    }

    private void SetupModificationInfo(string modificationSelectedName)
    {
        var carModificstion = selectedCarMods.First(x => x.Name == modificationSelectedName);

        selectedModification = carModificstion;

        modificationName.text = carModificstion.Name;
        var playerCarModification = playerInventory
            .GetCarModification(selectedCar.name, modificationSelectedName);

        var isBought = playerCarModification.Bought;

        var car = garageCars.First(x => x.name == selectedCar.name);
        car.InstallingModifications(selectedModification.Place, selectedModification.Type);

        if (!isBought)
        {
            modificationPrice.text = $"{carModificstion.Price}$";
            modificationTogle.isOn = false;
            actionModificationButton.gameObject.SetActive(true);
            actionModificationButtonText.text = "Buy";

            if(!playerInventory.HasEnoughMoney(carModificstion.Price))
            {
                actionModificationButton.interactable = false;
                return;
            }

            actionModificationButton.interactable = true;
            actionModificationButton.onClick.RemoveAllListeners();
            actionModificationButton.onClick.AddListener(BuyModification);
            return;
        }

        var isInstalled = playerCarModification.Installed;

        modificationPrice.text = string.Empty;
        modificationTogle.isOn = isInstalled;
        actionModificationButton.gameObject.SetActive(!isInstalled);
        actionModificationButtonText.text = "Install";
        actionModificationButton.onClick.RemoveAllListeners();
        actionModificationButton.onClick.AddListener(InstallModification);
    }

    private void BuyModification()
    {
        playerInventory.SpendMoney(selectedModification.Price);
        playerInventory.BuyModification(selectedCar.name, selectedModification.Name);
        playerInventory.InstalligModification(selectedCar.name, selectedModification.Name, selectedModification.Place);
        modificationPrice.text = string.Empty;
        modificationTogle.isOn = true;
        actionModificationButton.gameObject.SetActive(false);
        UpdatePlayerCurrencyText();
        PlayBuySound();
    }

    private void InstallModification()
    {
        playerInventory.InstalligModification(selectedCar.name, selectedModification.Name, selectedModification.Place);
        modificationTogle.isOn = true;
        actionModificationButton.gameObject.SetActive(false);
        PlayClickSound();
    }

    private void OnNextModification()
    {
        var index = GetNextIndex(selectedCarMods.IndexOf(selectedModification), selectedCarMods.Count);

        SetupModificationInfo(selectedCarMods[index].Name);
        PlayClickSound();
    }

    private void OnPrevoisModification()
    {
        var index = GetPreviosIndex(selectedCarMods.IndexOf(selectedModification), selectedCarMods.Count);

        SetupModificationInfo(selectedCarMods[index].Name);
        PlayClickSound();
    }

    private int GetNextIndex(int curIndex, int count)
    {
        return (curIndex + 1) % count;
    }

    private int GetPreviosIndex(int curIndex, int count)
    {
        return (curIndex - 1 + count) % count;
    }

    private PlayerCarSettings GetPlayerCarSettings(string carName)
    {
        return playerInventory.CarsSettings.FirstOrDefault(x => x.CarName == carName);
    }

    private void PlayClickSound()
    {
        audioManager.PlayAudio(BaseSound.Click);
    }

    private void PlayBuySound()
    {
        audioManager.PlayAudio(BaseSound.Buy);
    }
}
