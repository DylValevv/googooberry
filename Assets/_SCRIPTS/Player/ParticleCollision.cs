using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ParticleCollision : MonoBehaviour
{
    PlayerController player;
    [SerializeField] private float speed;
    [SerializeField] private float lifetime;
    private int damage;
    [SerializeField] GameObject impactVFX;
    private GameObject particleVis;

    private GameObject impactInstan;

    private BoxCollider col;

    /// <summary>
    /// assign a reference to the player
    /// </summary>
    /// <param name="playerController"></param>
    public void InitializeParticle(PlayerController playerController, int damageToDeal, GameObject particleVisual)
    {
        player = playerController;
        damage = damageToDeal;
        particleVis = particleVisual;

        // destroy the particle specified time
        Invoke("DelayedDeath", lifetime);

        col = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        transform.position += transform.forward * Time.deltaTime * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy>().TakeDamage(damage);
            impactInstan = Instantiate(impactVFX, transform.position, Quaternion.identity);
            impactInstan.GetComponent<VisualEffect>().Play();
            particleVis.SetActive(false);
            col.enabled = false;
        }
        if(other.CompareTag("Environment"))
        {
            impactInstan = Instantiate(impactVFX, transform.position, Quaternion.identity);
            impactInstan.GetComponent<VisualEffect>().Play();
            particleVis.SetActive(false);
            col.enabled = false;
        }

        if(other.CompareTag("Obstacle"))
        {
            Destroy(other.gameObject);

            impactInstan = Instantiate(impactVFX, transform.position, Quaternion.identity);
            impactInstan.GetComponent<VisualEffect>().Play();
            particleVis.SetActive(false);
            col.enabled = false;
        }
    }

    /// <summary>
    /// kill the collider and its garbage after a brief moment so it doesnt keep flying forever
    /// </summary>
    private void DelayedDeath()
    {
        Destroy(impactInstan);
        Destroy(gameObject);
    }
}
