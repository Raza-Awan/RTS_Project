using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/BuildingsData_SO")]

public class BuildingsData_SO : ScriptableObject
{
    public BuildingData[] buildingData;
}

[System.Serializable]
public class BuildingData
{
    public string name;
    public int itemID;
    public int price;
    public int buildTime;
    public GameObject prefab;
}
