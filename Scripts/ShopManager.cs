using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public Text coinsText;
    public GameObject notEnoughFundsWindow;

    int coins;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("Coins"))
        {
            coins = PlayerPrefs.GetInt("Coins");
        }
        else
        {
            coins = 1000;
            PlayerPrefs.SetInt("Coins", coins);
        }
        coinsText.text = coins.ToString();
    }

    public void BuyBuilding(int buildingID)
    {
        int buildingPrice = BuildingSpawner.Instance.GetBuildingPrice(buildingID);

        coins = PlayerPrefs.GetInt("Coins");
        if (coins >= buildingPrice)
        {
            coins -= buildingPrice;
            PlayerPrefs.SetInt("Coins", coins);
            BuildingSpawner.Instance.canSpawnBuilding = true;
        }
        else
        {
            notEnoughFundsWindow.SetActive(true);
            BuildingSpawner.Instance.canSpawnBuilding = false;
        }
        coinsText.text = coins.ToString();
    }
}
