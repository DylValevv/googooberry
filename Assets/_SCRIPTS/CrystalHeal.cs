using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalHeal : MonoBehaviour
{
    private GameState gameState;

    [SerializeField] private int healAmount;
    [SerializeField] private Transform impactVFXSpawnLocation;
    [SerializeField] private GameObject impactVFX;

    private void Start()
    {
        gameState = Resources.Load<GameState>("GameStateObject");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Weapon")
        {
            AudioManager.instance.PlayAction("CrystalHeal");
            gameState.AddPlayerHealth(healAmount);
            Instantiate(impactVFX, impactVFXSpawnLocation);
            Destroy(gameObject);
        }
    }
}
