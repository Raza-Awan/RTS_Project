using System.Collections;
using System.Collections.Generic;
using System.Net;
using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    private EnemyHealthBar healthBar;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;

        healthBar = GetComponentInChildren<EnemyHealthBar>();
        healthBar.gameObject.SetActive(false);
    }

    public void TakeDamage(float damage)
    {
        if (healthBar.gameObject.activeSelf == false)
        {
            healthBar.gameObject.SetActive(true);
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0f;
            CameraShake.instance.ShakeCamera();
            GameObject destructionParticles = Instantiate(GameManager.instance.destructionParticles, transform.position, Quaternion.identity);
            Destroy(destructionParticles, 3f);
            Destroy(gameObject);
        }
        currentHealth -= damage;
        healthBar.SetProgress(currentHealth * 0.01f);
    }
}
