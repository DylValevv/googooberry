using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private GameObject projectileVFX;
    [SerializeField] private int damage;
    [SerializeField] private float speed = 1;
    private Vector3 dir;


    Vector3 target;
    Enemy enemy;

    void LateUpdate()
    {
        //speed toward target
        float step = speed * Time.deltaTime;

        transform.Translate(dir * step);
    }

    /// <summary>
    /// assigns variables
    /// </summary>
    public void InitializeProjectile(Enemy enemyOG)
    {
        enemy = enemyOG;
        target = enemy.playerObj.transform.position;
        dir = target - transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            enemy.DealDamage(damage);
            Destroy(gameObject);
        }
        if(other.gameObject.CompareTag("Environment"))
        {
            Destroy(gameObject);
        }

    }
}
