using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameMode { Home, Attack }

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }


    [Header("Select Mode")]
    public GameMode gameMode;

    [Space(10)]
    public GameObject environmentParent;
    public GameObject homeEnvironmentPrefab;
    public GameObject attackEnvironmentPrefab;
    [HideInInspector]public GameObject homeEnvironmentRef;
    [HideInInspector]public GameObject attackEnvironmentRef;

    public GameObject destructionParticles;

    [Header("Windows Ref")]
    public GameObject gameOverlayWindow;
    public GameObject attackOverlayWindow;
    public GameObject windowsContainer;
    public GameObject sceneEnteringWindowPrefab;
    public GameObject resultsWindow;

    [Header("Buttons Ref")]
    public Button attackButtton;
    public Button homeButtton;

    [Header("Objects Container")]
    public GameObject playerSoldiersContainer;

    private void Start()
    {
        Instantiate(sceneEnteringWindowPrefab, windowsContainer.transform);

        attackButtton.onClick.AddListener(OnClickAttackButton);
        homeButtton.onClick.AddListener(OnClickHomeButton);

        SetGameMode(gameMode);
    }

    public void SetGameMode(GameMode mode)
    {
        if (mode == GameMode.Home)
        {
            Destroy(playerSoldiersContainer);
            Instantiate(sceneEnteringWindowPrefab, windowsContainer.transform);
            gameOverlayWindow.SetActive(true);
            attackOverlayWindow.SetActive(false);
            Invoke(nameof(LoadHomeEnviro), 0.5f);
        }
        else if (mode == GameMode.Attack)
        {
            playerSoldiersContainer = new GameObject("PlayerSoldiersContainer");
            playerSoldiersContainer.transform.position = new Vector3(0, 0, 0);
            PlayerSoldierSpawner.SetPlayerSoldiersMaxValue(10);
            Instantiate(sceneEnteringWindowPrefab, windowsContainer.transform);
            attackOverlayWindow.SetActive(true);
            gameOverlayWindow.SetActive(false);
            FindClosetEnemy.Instance.FindAllEnemies();
            Invoke(nameof(LoadAttackEnviro), 0.5f);
        }
    }

    public void OnClickHomeButton()
    {
        SetGameMode(GameMode.Home);
    }

    public void OnClickAttackButton()
    {
        SetGameMode(GameMode.Attack);
    }

    private void LoadAttackEnviro()
    {
        if (homeEnvironmentRef)
        {
            Destroy(homeEnvironmentRef);
        }
        attackEnvironmentRef = Instantiate(attackEnvironmentPrefab, environmentParent.transform);
    }

    private void LoadHomeEnviro()
    {
        if (attackEnvironmentRef)
        {
            Destroy(attackEnvironmentRef);
        }
        homeEnvironmentRef = Instantiate(homeEnvironmentPrefab, environmentParent.transform);
    }

    public void ShowVictoryPanel(float time, bool status)
    {
        ToggleGameObject(resultsWindow, time, status);
    }






    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Method for making game objects turing on and off with respect to time
    /// </summary>

    // Method to toggle GameObject active status on intervals
    public void ToggleGameObject(GameObject obj, float interval, bool activateOnInterval)
    {
        // Ensure the GameObject is not null
        if (obj == null)
        {
            Debug.Log("GameObject is null.");
            return;
        }

        // Start a coroutine to toggle the GameObject on intervals
        StartCoroutine(ToggleCoroutine(obj, interval, activateOnInterval));
    }

    // Coroutine to handle the toggling logic
    IEnumerator ToggleCoroutine(GameObject obj, float interval, bool activateOnInterval)
    {
        // Wait for the specified interval
        yield return new WaitForSeconds(interval);

        // Toggle the GameObject's active status
        obj.SetActive(activateOnInterval);
    }
}
