using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/EnemyManager", order = 1)]
public class EnemyManager : ScriptableObject
{
    [Header("Enemy")]
    public int enemyAverageSpeed = 1;
    public int enemyAverageDamage = 1;
    public int enemyAverageHealth = 10;

    [Header("Wave")]
    public Vector3[] spawnPoints;
    public int[] enemiesPerWave;

    [HideInInspector] public int enemiesRemainingInWave = 0;
    [HideInInspector] public int currentWave = 0;

    public int ObjectPoolerEnemiesToSpawn()
    {
        int num = 0;
        foreach (int i in enemiesPerWave)
        {
            if (i > num)
            {
                num = i;
            }
        }
        return num;
    }

    public void SetEnemiesRemaining()
    {
        enemiesRemainingInWave = enemiesPerWave[currentWave];
    }

}
