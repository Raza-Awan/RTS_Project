using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class PlayerSoldier : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;

    public GameObject enemy;

    public string tagToDetect = "Enemy";

    private float stoppingDistance;
    private float remainingDistance;
    private float agentInitialSpeed;
    public float damageAmount = 1f;

    private const string IDLE = "ELF_spearman_01_idle";
    private const string RUN = "ELF_spearman_03_run";
    private const string ATTACK = "ELF_spearman_07_attack";
    private const string DEAD = "ELF_spearman_10_death_A";

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        stoppingDistance = agent.stoppingDistance;
        agentInitialSpeed = agent.speed;
    }

    // Update is called once per frame
    void Update()
    {
        GetClosetEnemy();

        if (enemy)
        {
            agent.speed = agentInitialSpeed;
            AgentDistanceFromTarget();
        }
        else
        {
            agent.speed = 0f;
            animator.Play(IDLE);
        }
    }

    private void AgentDistanceFromTarget()
    {
        agent.SetDestination(enemy.transform.position);

        remainingDistance = agent.remainingDistance;

        if (remainingDistance <= stoppingDistance)
        {
            animator.Play(ATTACK);
        }
        else
        {
            animator.Play(RUN);
        }
    }

    private void GetClosetEnemy()
    {
        enemy = FindClosetEnemy.Instance.ClosestEnemy(transform.position);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag(tagToDetect))
        {
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            enemyHealth.TakeDamage(damageAmount * 0.1f);
        }
    }
}
