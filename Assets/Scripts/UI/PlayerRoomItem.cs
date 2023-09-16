using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerRoomItem : PoolItem
{
    [SerializeField] private TMP_Text playerNameText;
    
    private string playerName;

    public new string Name => playerName; 

    public override void SetInfo(params string[] value)
    {
        playerName = value[0];
        playerNameText.text = $"{playerName}";
    }
}
