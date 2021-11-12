using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "State", menuName = "ScriptableObjects/GameState", order = 1)]
public class GameState : ScriptableObject
{

    [Header("Player Values")]
    public int playerHealth;
    public int maxPlayerHealth;
    public int playerDamage;
    public Vector3 spawnPoint;

    [Header("Crystal Values")]
    public Object[] crystalCores;
    public float crystalProgress;

    public void getProgress()
    {
        float progress = 0;
        foreach (Object i in crystalCores) //the line the error is pointing to
        {
            //progress += i.isMined;
        }
        crystalProgress = progress;
    }

    public void AddPlayerHealth(int amount)
    {
        int res = playerHealth + amount;
        playerHealth = Mathf.Clamp(amount, 0, maxPlayerHealth);
    }
}
