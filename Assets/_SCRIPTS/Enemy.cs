using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyManager enemyManager;

    public int speed = 0;
    public int damage = 0;
    public int health = 0;


    // Start is called before the first frame update
    void Start()
    {
        SetStats();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetStats()
    {
        speed = enemyManager.enemyAverageSpeed;
        damage = enemyManager.enemyAverageDamage;
        health = enemyManager.enemyAverageHealth;
    }

    public void TakeDamage(int hitDamage)
    {
        health -= hitDamage;
        if (health <= 0)
        {
            Death();
        }
    }

    private void Death()
    {
        enemyManager.enemiesRemainingInWave -= 1;
        gameObject.SetActive(false);
    }
}
