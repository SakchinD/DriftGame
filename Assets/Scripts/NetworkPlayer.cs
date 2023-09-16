using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer : MonoBehaviour, IPunObservable
{
    public bool InGame { get; private set; }
    public bool IsLockal { get; private set; }
    public bool IsDrift { get; private set; }

    public int Score { get; private set; }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(Score);
        }
        else
        {
            Score = (int)stream.ReceiveNext();
        }
    }

    public void SetInGame(bool value)
    {
        InGame = value;
    }

    public void SetIsDrifting(bool value)
    {
        IsDrift = value;
    }

    public void SetIsLockal(bool value)
    {
        IsLockal = !value;
    }

    public void SetScore(int score)
    {
        this.Score += score;
    }
}
