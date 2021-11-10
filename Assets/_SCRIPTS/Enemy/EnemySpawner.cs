using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public EnemyManager enemyManager;
    public GameObject entityToSpawn;
    private int instanceNumber = 0;
    public GameObject[] enemies;

    // Start is called before the first frame update
    void Start()
    {
        enemies = new GameObject[enemyManager.ObjectPoolerEnemiesToSpawn()];
        InitEnemies();
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyManager.enemiesRemainingInWave <= 0)
        {
            enemyManager.currentWave++;
            SpawnWave();
        }
    }

    private void InitEnemies()
    {

        for (int i = 0; i < enemyManager.ObjectPoolerEnemiesToSpawn(); i++)
        {
            // Creates an instance of the prefab at the current spawn point.
            GameObject currentEntity = Instantiate(entityToSpawn, enemyManager.spawnPoints[0], Quaternion.identity);

            // Sets the name of the instantiated entity to be the string defined in the ScriptableObject and then appends it with a unique number. 
            currentEntity.name = "Enemy" + instanceNumber;

            currentEntity.GetComponent<Enemy>().enemyManager = enemyManager;

            enemies[instanceNumber] = currentEntity;

            currentEntity.SetActive(false);

            instanceNumber++;
        }

    }

    private void SpawnWave()
    {
        if (enemyManager.currentWave < enemyManager.enemiesPerWave.Length)
        {
            Debug.Log("New Wave");
            for (int i = 0; i < enemyManager.enemiesPerWave[enemyManager.currentWave]; i++)
            {
                enemies[i].SetActive(true);
            }
            enemyManager.SetEnemiesRemaining();

        }
    }
}
