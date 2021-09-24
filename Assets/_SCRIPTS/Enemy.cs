using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int health;
    private bool isDead;

    private void Start()
    {
        isDead = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Weapon")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            TakeDamage(weapon.GetDamage());
            weapon.ToggleCollider(false);
        }
    }

    /// <summary>
    /// removes the health amount by weapon damage amount
    /// </summary>
    /// <param name="amount"></param> the amount of damage to take/health to lose
    private void TakeDamage(int amount)
    {
        health -= amount;
        CheckIsDead();
    }

    /// <summary>
    /// checks to see if the health is at or below 0. if so, kills it
    /// </summary>
    private void CheckIsDead()
    {
        if(health <= 0)
        {
            isDead = true;

            Destroy(this.gameObject);
        }
    }    
}
