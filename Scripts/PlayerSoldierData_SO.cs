using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/PlayerSoldierData_SO")]

public class PlayerSoldierData_SO : ScriptableObject
{
    public PlayerSoldierData[] playerSoldierData;
}

[System.Serializable]
public class PlayerSoldierData
{
    public string name;
    public int itemID;
    public int price;
    public int trainTime;
    public GameObject prefab;
}
