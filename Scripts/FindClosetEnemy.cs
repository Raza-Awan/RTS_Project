using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FindClosetEnemy : MonoBehaviour
{
    public static FindClosetEnemy Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }


    public string tagToDetect = "Enemy";
    public List<GameObject> allEnemies;

    GameMode mode;

    public bool allEnemiesDied = false;

    // Start is called before the first frame update
    void Start()
    {
        mode = GameMode.Attack;
    }

    private IEnumerator FindEnemies()
    {
        allEnemiesDied = true;

        yield return new WaitForSeconds(2f);

        if (allEnemies.Count == 0)
        {
            allEnemies = GameObject.FindGameObjectsWithTag(tagToDetect).ToList();
        }

        allEnemiesDied = false;
    }

    public void FindAllEnemies()
    {
        StartCoroutine(FindEnemies());
    }

    // Update is called once per frame
    void Update()
    {
        // Check for missing or destroyed objects and remove them from the list
        allEnemies.RemoveAll(enemy => enemy == null || !enemy.activeSelf);

        OnBattleOver();
    }

    private void OnBattleOver()
    {
        if (allEnemies.Count == 0 && allEnemiesDied == false && mode == GameManager.instance.gameMode && GameManager.instance.attackEnvironmentRef)
        {
            GameManager.instance.ShowVictoryPanel(1f, true);
            allEnemiesDied = true;
        }
    }

    public GameObject ClosestEnemy(Vector3 position)
    {
        GameObject closestHere = null;
        float leastDistance = Mathf.Infinity;
        float distanceHere = 0f;

        foreach (var enemy in allEnemies)
        {
            if (enemy)
            {
                distanceHere = Vector3.Distance(position, enemy.transform.position);
            }
            if (distanceHere < leastDistance)
            {
                leastDistance = distanceHere;
                closestHere = enemy;
            }

        }

        return closestHere;
    }
}
