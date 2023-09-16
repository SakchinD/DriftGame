using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Map : MonoBehaviour
{
    [SerializeField] private string mapName;
    [SerializeField] private int maxPlayerCount;
    [SerializeField] private float playTime;
    [SerializeField] private int scoreInTick;
    [SerializeField] private Transform[] playersStartPositions;
    [SerializeField] Sprite mapArt;
    public string MapName => mapName;
    public int MaxPlayerCount => maxPlayerCount;
    public float PlayTime => playTime;
    public int ScoreInTick => scoreInTick;
    public Transform[] PlayerStartPositions => playersStartPositions;
    public Sprite MapArt => mapArt;
}
