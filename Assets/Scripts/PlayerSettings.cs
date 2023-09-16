using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerSettings
{
    string NickName {  get; }
    float MusicVolume { get; }
    float SoundVolume { get; }

    void SetDefault();
    void SetNickName(string nickName);
    void SetMusicVolume(float value);
    void SetSoundVolume(float value);
}
public class PlayerSettings : IPlayerSettings
{
    private string nickName;
    private float musicVolume;
    private float soundVolume;

    public string NickName => nickName;
    public float MusicVolume => musicVolume;
    public float SoundVolume => soundVolume;

    public void SetDefault()
    {
        nickName = "Player" + Random.Range(0, 9999);
        musicVolume = 0;
        soundVolume = 0;
    }

    public void SetNickName(string nickName)
    {
        this.nickName = nickName;
    }

    public void SetMusicVolume(float value)
    { 
        musicVolume = value;
    }

    public void SetSoundVolume(float value)
    {
        soundVolume = value;
    }
}
