using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Car : MonoBehaviour
{
    [Serializable]
    public class CarModificationsItems
    {
        public CarModificationsPlace Place;
        public CarModificationsTypes Type;
        public GameObject ModificationItem;
    }

    [SerializeField] private CarModificationsItems[] carModificationsItems;

    [SerializeField] private MeshRenderer carBodyMesh;
    [SerializeField] private PhotonView photonView;
    [SerializeField] private CarController carController;

    private Color startCarColor;

    public void SetInputButtons(Mybutton[] inputButtons)
    {
        carController.SetInputButtons(inputButtons);
    }

    public void SendPlayercarSettings(PlayerCarSettings settings)
    {
        var result = SerializeStringData(settings);
        photonView.RPC("RPC_SendSettings", RpcTarget.All, result);
    }

    public void SetSettings(PlayerCarSettings carModifications)
    {
        if (carModifications == null)
            return;

        var stringColor = carModifications.Color;
        Color color;

        if (ColorUtility.TryParseHtmlString(stringColor, out color))
        {
            SetCarColcor(color);
        }

        startCarColor = carBodyMesh.material.color;

        carModifications.PlayerCarModifications.ForEach(modi =>
        {
            if (modi.Installed)
                InstallingModifications(modi.Place, modi.Type);
        });
    }

    public void InstallingModifications(CarModificationsPlace place, CarModificationsTypes type)
    {
        foreach (var item in carModificationsItems.Where(x => x.Place == place))
        {
            item.ModificationItem.SetActive(item.Type == type);
        }
    }

    public void ResetColor()
    {
        carBodyMesh.material.color = startCarColor;
    }

    public void InstallColor()
    {
        startCarColor = carBodyMesh.material.color;
    }

    public void SetCarColcor(Color newColor)
    {
        carBodyMesh.material.color = newColor;
    }

    [PunRPC]
    private void RPC_SendSettings(string message, PhotonMessageInfo info)
    {
        if (info.Sender != PhotonNetwork.LocalPlayer)
        {
            this.name = info.Sender.NickName;
            SetSettings(DeSerializaStringData(message));
        }
    }

    private string SerializeStringData(PlayerCarSettings set)
    {
        return JsonUtility.ToJson(set);
    }

    private PlayerCarSettings DeSerializaStringData(string netData)
    {
        return JsonUtility.FromJson<PlayerCarSettings>(netData);
    }
}
