using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

[Serializable]
public class PlayerCarSettings
{
    public string CarName;
    public List<PlayerCarModification> PlayerCarModifications;
    public string Color;
}

[Serializable]
public class PlayerCarModification
{
    public string ModificationName;
    public CarModificationsPlace Place;
    public CarModificationsTypes Type;
    public bool Installed;
    public bool Bought;
}

public interface IPlayerInventory
{
    int Money { get;}

    string CurrentCarName { get;}

    List<string> Cars { get;}
    List<PlayerCarSettings> CarsSettings { get;}

    void SetDefault();
    void SetMoney(int value);
    void SetCurrentCar(string name);
    void SetCars(List<string> cars);
    void SetCarsSettings(List<PlayerCarSettings> playerCarSettings);
    void InstalligModification(string carName, string modificationName, CarModificationsPlace place);
    void BuyCar(string carName);
    void SetNewCarColor(string carName, Color color);
    void BuyModification(string carName, string modificationName);
    PlayerCarModification GetCarModification(string carName, string modificationName);
    PlayerCarSettings GetCurrentCarSettings();
    bool HasEnoughMoney(int price);
    void SpendMoney(int price);
}

public class PlayerInventory : IPlayerInventory
{
    private int money;
    private List<string> cars = new();
    private List <PlayerCarSettings> carsSettings = new();
    private CarsExamples carsExamples;
    private string currentCarName;

    public int Money => money;
    public string CurrentCarName => currentCarName;
    public List<string> Cars => cars;
    public List<PlayerCarSettings> CarsSettings => carsSettings;

    public PlayerInventory(CarsExamples carsExamples)
    {
        this.carsExamples = carsExamples;
    }

    public void SetDefault()
    {
        cars.Clear();
        carsSettings.Clear();

        money = 0;

        BuyCar(carsExamples.Cars.First(x => x.IsStartCar).Name);
    }

    public void SetMoney(int value)
    {
        money += value;
    }

    public void SetCurrentCar(string name)
    {
        currentCarName = name;
    }

    public void SetCars(List<string> cars)
    {
        this.cars = cars;
    }

    public void SetCarsSettings(List<PlayerCarSettings> playerCarSettings)
    {
        carsSettings = playerCarSettings;
    }

    public void InstalligModification(string carName, string modificationName, CarModificationsPlace place)
    {
        var modifications = carsSettings.FirstOrDefault(x => x.CarName == carName)
            .PlayerCarModifications.Where(x => x.Place == place);

        foreach(var modification in modifications)
        {
            modification.Installed = modification.ModificationName == modificationName;
        }
    }

    public void BuyCar(string carName)
    {
        cars.Add(carName);
        currentCarName = carName;
        var baseCar = carsExamples.Cars.First(x => x.Name == carName);

        var playerCarSettings = new PlayerCarSettings();
        playerCarSettings.CarName = baseCar.Name;
        playerCarSettings.PlayerCarModifications = new();

        foreach (var carMod in baseCar.CarModifications)
        {
            var playerCarModification = new PlayerCarModification()
            {
                ModificationName = carMod.Name,
                Place = carMod.Place,
                Type = carMod.Type,
                Installed = carMod.IsStartModification,
                Bought = carMod.Price == 0
            };
            playerCarSettings.PlayerCarModifications.Add(playerCarModification);
        }

        carsSettings.Add(playerCarSettings);
    }

    public void SetNewCarColor(string carName, Color color)
    {
        var stringColor = "#" + ColorUtility.ToHtmlStringRGBA(color);
        var settings = GetCurrentCarSettings();
        settings.Color = stringColor;
    }

    public void BuyModification(string carName, string modificationName)
    {
        var modification = GetCarModification(carName, modificationName);
        modification.Bought = true;
        modification.Installed = true;
    }

    public PlayerCarModification GetCarModification(string carName, string modificationName)
    {
        return carsSettings.FirstOrDefault(x => x.CarName == carName)
            .PlayerCarModifications.FirstOrDefault(x => x.ModificationName == modificationName);
    }

    public PlayerCarSettings GetCurrentCarSettings()
    {
        return carsSettings.FirstOrDefault(x => x.CarName == currentCarName);
    }

    public bool HasEnoughMoney(int price)
    {
        return money >= price;
    }

    public void SpendMoney(int price)
    {
        money -= price;
    }
}
