using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public EnemyManager enemyManager;
    public GameObject entityToSpawn;
    private int instanceNumber = 0;
    public GameObject[] enemies;
    public GameObject playerObj;
    private bool combatTime = false;


    // Start is called before the first frame update
    void Start()
    {
        enemies = new GameObject[enemyManager.ObjectPoolerEnemiesToSpawn()];
        playerObj = FindObjectOfType<PlayerController>().gameObject;
        enemyManager.currentWave = -1;
        enemyManager.enemiesRemainingInWave = -1;
        InitEnemies();
    }

    // Update is called once per frame
    void Update()
    {
        bool enemiesAllDead = true;
        foreach (GameObject enemy in enemies)
        {
            if (enemy.activeSelf)
            {
                enemiesAllDead = false;
            }
        }
        if (enemiesAllDead && combatTime)
        {
            enemyManager.currentWave++;
            Debug.Log("SPAWNING WAVE");
            SpawnWave();
        }
    }

    private void InitEnemies()
    {

        for (int i = 0; i < enemyManager.ObjectPoolerEnemiesToSpawn(); i++)
        {
            // Creates an instance of the prefab at the current spawn point.
            GameObject currentEntity = Instantiate(entityToSpawn, enemyManager.spawnPoints[i % enemyManager.spawnPoints.Length], Quaternion.identity);

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
        //Debug.Log("current wave: " + enemyManager.currentWave);
        if (enemyManager.currentWave < enemyManager.enemiesPerWave.Length)
        {
            enemyManager.SetEnemiesRemaining();
            Debug.Log("New Wave");
            for (int i = 0; i < enemyManager.enemiesPerWave[enemyManager.currentWave]; i++)
            {
                enemies[i].GetComponent<Enemy>().health = enemyManager.enemyAverageHealth;
                enemies[i].GetComponent<Enemy>().firstDelayedDeathInvoke = false;
                enemies[i].SetActive(true);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            combatTime = true;
            //SpawnWave();
        }
    }
}
