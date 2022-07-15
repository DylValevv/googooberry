using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitCollider : MonoBehaviour
{
    private Enemy enemy;
    // Start is called before the first frame update
    void Start()
    {
        enemy = transform.parent.GetComponent<Enemy>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && enemy.isAttacking)
        {
            //Debug.Log("??????");
            enemy.attackSuccessful = true;
        }
    }
}
