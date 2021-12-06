using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "State", menuName = "ScriptableObjects/GameState", order = 1)]
public class GameState : ScriptableObject
{

    [Header("Player Values")]
    public int playerHealth;
    public int maxPlayerHealth = 10;
    public int playerDamage;
    public Vector3 spawnPoint;

    [Header("Crystal Values")]
    [Range(0,3)]
    public int crystalProgress;

    public void AddPlayerHealth(int amount)
    {
        playerHealth = Mathf.Clamp(playerHealth + amount, 0, maxPlayerHealth);
    }
}
