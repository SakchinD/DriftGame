using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameSettingsManager
{
    bool IsOnline { get; }
    Map Map { get; }
    void CreateGameSettings(bool isOnline, Map map);
}

public class GameSettingsManager : IGameSettingsManager
{
    private bool isOnline;
    private Map map;

    public bool IsOnline => isOnline;

    public Map Map => map;

    public void CreateGameSettings(bool isOnline, Map map)
    {
        this.isOnline = isOnline;
        this.map = map;
    }
}
