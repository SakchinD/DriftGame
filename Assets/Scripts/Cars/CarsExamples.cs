using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cars")]
public class CarsExamples : ScriptableObject
{
    [SerializeField] private List<BaseCar> cars;

    public List<BaseCar> Cars => cars;
}

[Serializable]
public struct BaseCar
{
    public bool IsStartCar;
    public string Name;
    public int CarPrice;
    public Car GarageCar;
    public Car GameCar;
    public CarModification[] CarModifications;
}

[Serializable]
public struct CarModification
{
    public string Name;
    public CarModificationsPlace Place;
    public CarModificationsTypes Type;
    public int Price;
    public bool IsStartModification;
}

public enum CarModificationsPlace
{
    Back,
    Top,
}

public enum CarModificationsTypes
{
    None,
    Base,
    Sport
}
