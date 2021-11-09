using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MeleeAttack : MonoBehaviour
{
    [HideInInspector] public bool currentlyMeleeAttacking = false;
    private bool nearPlayer = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DoAttack()
    {
        //Debug.Log("DOING ATTACK");
        Sequence mySequence = DOTween.Sequence();
        mySequence.AppendInterval(transform.parent.GetComponent<Enemy>().meleeAttackInterval).OnComplete(() => AttackHelper());
    }

    private void AttackHelper()
    {
        if (!currentlyMeleeAttacking && nearPlayer)
        {
            transform.parent.GetComponent<Enemy>().MeleeAttack();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            nearPlayer = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !currentlyMeleeAttacking)
        {
            nearPlayer = true;
            transform.parent.GetComponent<Enemy>().MeleeAttack();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            nearPlayer = false;
        }
    }


}
