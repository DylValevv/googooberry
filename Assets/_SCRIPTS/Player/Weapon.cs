using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Weapon : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] int damage;
    [SerializeField] float sheathDelay;
    private bool unsheathed;

    Animation anim;
    Collider myCollider;

    [SerializeField] private GameObject particleTrail;
    [SerializeField] private VisualEffect impactVFX;

    private Coroutine DelayedSheathCo;

    private void Start()
    {
        myCollider = GetComponent<Collider>();
        myCollider.enabled = false;

        anim = GetComponent<Animation>();

        unsheathed = false;

        particleTrail.GetComponent<TrailRenderer>().emitting = false;
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
        myCollider.enabled = active;

        // sheath weapon
        if(active && !unsheathed)
        {
            particleTrail.GetComponent<TrailRenderer>().emitting = true;

            unsheathed = true;

            anim.Play("anim_weapon_unsheath");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            impactVFX.Play();
        }
    }

    /// <summary>
    /// plays the animation
    /// </summary>
    /// <param name="name">name of the animation</param>
    public void PlayAnim(string name)
    {
        anim.Play(name);
    }

    /// <summary>
    /// sheaths the weapons
    /// </summary>
    public void SheathWeapon()
    {
        // Invoke("DelayedSheath", sheathDelay);
        if (DelayedSheathCo == null) DelayedSheathCo = StartCoroutine(DelayedSheath());
    }

    /// <summary>
    /// stops the delayed sheath coroutine
    /// </summary>
    public void StopSheath()
    {
        if (DelayedSheathCo != null) StopCoroutine(DelayedSheathCo);
        DelayedSheathCo = null;
    }

    /// <summary>
    /// after a set amount of time the weapon will be retracted. called as an Invoke in SheathWeapon()
    /// </summary>
    private IEnumerator DelayedSheath()
    {
        yield return new WaitForSeconds(sheathDelay);

        anim.Play("anim_weapon_sheath");
        unsheathed = false;
        particleTrail.GetComponent<TrailRenderer>().emitting = false;
        DelayedSheathCo = null;
        yield return null;
    }
}