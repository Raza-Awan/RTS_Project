using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerSoldierSpawner : MonoBehaviour
{
    public LayerMask placementLayer;
    public GameObject targetGameObject;

    public Text soldiersCounter_Text;
    public static int soldiersCounter;

    Camera sceneCamera;

    // Start is called before the first frame update
    void Start()
    {
        sceneCamera = Camera.main;
    }

    public static void SetPlayerSoldiersMaxValue(int maxValue)
    {
        soldiersCounter = maxValue;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (targetGameObject != null && !IsPointerOverUI() && soldiersCounter > 0)
            {
                PlaceObjectOnClickPos(targetGameObject);
                soldiersCounter--;
            }
        }

        soldiersCounter_Text.text = soldiersCounter.ToString() + "x";
    }

    public void GetGameObject(GameObject gameObject)
    {
        targetGameObject = gameObject;
    }

    private void PlaceObjectOnClickPos(GameObject gameObject)
    {
        Vector3 mousePos = Input.mousePosition;
        Ray ray = sceneCamera.ScreenPointToRay(mousePos); // for creating a ray from camera to the mouse pos

        RaycastHit hit; // this is for storing the result of the above ray where it hitted 
        if (Physics.Raycast(ray, out hit, 100, placementLayer))
        {
            InstantiateObject(gameObject, hit.point, GameManager.instance.playerSoldiersContainer.transform);
        }
    }

    private bool IsPointerOverUI() => EventSystem.current.IsPointerOverGameObject();

    void InstantiateObject(GameObject gameObject, Vector3 position, Transform parent)
    {
        // Instantiate the object at the specified position
        Instantiate(gameObject, position, Quaternion.identity, parent);
    }
}
