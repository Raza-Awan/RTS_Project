using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSpawner : MonoBehaviour
{
    public static BuildingSpawner Instance;

    private void Awake()
    {
        Instance = this;
    }

    public BuildingsData_SO buildingData_SO;
    public ShopManager shopManager;
    public Transform ground;

    private GameObject buildingPrefab;
    [HideInInspector]public int buildTime;
    [HideInInspector]public bool canSpawnBuilding;

    public float minDistanceBetweenBuildings = 2f;

    public void SpawnBuildingOnClick(int itemID)
    {
        buildTime = buildingData_SO.buildingData[itemID - 1].buildTime;
        buildingPrefab = buildingData_SO.buildingData[itemID - 1].prefab;

        Bounds groundBounds = ground.GetComponent<Collider>().bounds;

        // Generate a random position within the bounds of the ground
        Vector3 randomPosition = GetRandomPosition(groundBounds);

        // Check if the position is clear of other objects
        while (IsPositionOccupied(randomPosition))
        {
            // If occupied, find another random position
            randomPosition = GetRandomPosition(groundBounds);
        }

        shopManager.BuyBuilding(itemID);

        if (canSpawnBuilding == false)
        {
            return;
        }

        // Instantiate a single building at the random position
        Instantiate(buildingPrefab, randomPosition, Quaternion.identity);
    }

    Vector3 GetRandomPosition(Bounds bounds)
    {
        float x = Random.Range(bounds.min.x, bounds.max.x);
        //float y = Random.Range(bounds.min.y, bounds.max.y);
        float y = buildingPrefab.transform.position.y;
        float z = Random.Range(bounds.min.z, bounds.max.z);

        return new Vector3(x, y, z);
    }

    bool IsPositionOccupied(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, minDistanceBetweenBuildings);

        foreach (var collider in colliders)
        {
            // Check if the collider belongs to a building or any other object we want to avoid
            if (collider.CompareTag("Building"))
            {
                return true;
            }
        }

        return false;
    }

    public int GetBuildingPrice(int ID)
    {
        return buildingData_SO.buildingData[ID - 1].price;
    }
}
