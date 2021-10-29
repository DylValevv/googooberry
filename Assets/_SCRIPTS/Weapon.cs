using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] int damage;

    new Collider collider;

    private void Start()
    {
        collider = GetComponent<Collider>();
        collider.enabled = false;
    }

    /// <summary>
    /// how much damage the weapon does
    /// </summary>
    /// <returns>the amount of damage the weapon delivers</returns>
    public int GetDamage()
    {
        return damage;
    }

    /// <summary>
    /// turns the collider on and off for the weapon so that it only deals damage when the players presses the attack button
    /// </summary>
    public void ToggleCollider(bool active)
    {
        collider.enabled = active;
    }
}
