using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Zenject;
using System.Collections.Generic;
using System.Data;

public interface ISaveManager
{
    bool PlayerDataLoaded { get; }
    bool SettingsDataLoaded { get; }
    void SavePlayerData();
    void SaveSettingsData();
    void LoadPlayerData();
    void LoadSettingsData();
}

public class SaveManager : ISaveManager, IInitializable
{
    private readonly string playerDataPath = Application.persistentDataPath + "/MySaveData.dat";
    private readonly string settingsDataPath = Application.persistentDataPath + "/MySaveSettingsData.dat";

    private bool playerDataloaded;
    private bool settingsDataloaded;

    private IPlayerInventory playerInventory;
    private IPlayerSettings playerSettings;

    public bool PlayerDataLoaded => playerDataloaded;
    public bool SettingsDataLoaded => settingsDataloaded;

    public SaveManager(IPlayerInventory playerInventory,
        IPlayerSettings playerSettings) 
    {
        this.playerInventory = playerInventory;
        this.playerSettings = playerSettings;
    }

    public void Initialize()
    {
        LoadPlayerData();
        LoadSettingsData();
    }

    public void SavePlayerData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(playerDataPath);
        SaveData data = new SaveData();
        data.saveMoney = playerInventory.Money;
        data.saveCurrentCarName = playerInventory.CurrentCarName;
        data.saveCars = playerInventory.Cars;
        data.saveCarsSettings = playerInventory.CarsSettings;
        bf.Serialize(file, data);
        file.Close();
        Debug.Log("Player data saved!");
    }

    public void SaveSettingsData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(settingsDataPath);
        SaveSettingsData data = new SaveSettingsData();
        data.savePlayerNickName = playerSettings.NickName;
        data.saveMusicVolume = playerSettings.MusicVolume;
        data.saveSoundVolume = playerSettings.SoundVolume;
        bf.Serialize(file, data);
        file.Close();
        Debug.Log("Settings data saved!");
    }

    public void LoadPlayerData()
    {
        if (File.Exists(playerDataPath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(playerDataPath, FileMode.Open);
            SaveData data = (SaveData)bf.Deserialize(file);
            file.Close();
            playerInventory.SetMoney(data.saveMoney);
            playerInventory.SetCurrentCar(data.saveCurrentCarName);
            playerInventory.SetCars(data.saveCars);
            playerInventory.SetCarsSettings(data.saveCarsSettings);
            Debug.Log("Player data loaded!");
            playerDataloaded = true;
        }
        else
        {
            playerInventory.SetDefault();
            SavePlayerData();
        }
    }

    public void LoadSettingsData()
    {
        if (File.Exists(settingsDataPath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(settingsDataPath, FileMode.Open);
            SaveSettingsData data = (SaveSettingsData)bf.Deserialize(file);
            file.Close();
            playerSettings.SetNickName(data.savePlayerNickName);
            playerSettings.SetMusicVolume(data.saveMusicVolume);
            playerSettings.SetSoundVolume(data.saveSoundVolume);
            Debug.Log("Settings data loaded!");
            settingsDataloaded = true;
        }
        else
        {
            playerSettings.SetDefault();
            SaveSettingsData();
        }
    }
}

[Serializable]
class SaveData
{
    public int saveMoney;
    public string saveCurrentCarName;
    public List<string> saveCars;
    public List<PlayerCarSettings> saveCarsSettings;
}

[Serializable]
class SaveSettingsData
{
    public string savePlayerNickName;
    public float saveMusicVolume;
    public float saveSoundVolume;
}