using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttack : MonoBehaviour
{
    private Coroutine rangedCoroutine;
    [SerializeField] private float range;
    [SerializeField] private float rangedCooldownTime;
    [SerializeField] private float failedReDoRangedCooldownTime;
    [SerializeField] float rangedAttackDuration;
    [SerializeField] private Enemy enemy;
    [SerializeField] private GameObject projectilePrefab;
    private Coroutine BeamCoroutine;
    // Start is called before the first frame update
    void Start()
    {
        rangedCoroutine = StartCoroutine(CooldownCountdown(rangedCooldownTime));
    }

    /// <summary>
    /// raycasts to player to see if enemy has a clear shot
    /// </summary>
    private bool CheckAttack()
    {
        RaycastHit hit;
        if(Physics.Raycast(this.transform.localPosition, enemy.playerObj.transform.position, out hit, Mathf.Infinity))
        {
            return (hit.collider.gameObject.CompareTag("Player"));
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// triggers the VFX/the physical attack itself
    /// </summary>
    public IEnumerator DoRangedAttack()
    {
        enemy.RangedAttack(true);

        yield return new WaitForSeconds(rangedAttackDuration);

        if(!enemy.gameObject.activeSelf)
        {
            StopCoroutine(BeamCoroutine);
        }

        // instantiate the beam
        GameObject beam = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        beam.GetComponent<EnemyProjectile>().InitializeProjectile(enemy);

        // reset the cooldown
        if (rangedCoroutine != null) rangedCoroutine = StartCoroutine(CooldownCountdown(rangedCooldownTime));

        yield return null;
    }

    /// <summary>
    /// calculates the distance between the player and the enemy and determines if it within the specified range to do a ranged attack
    /// </summary>
    public bool CalculateDistanceToPlayer()
    {
        float dist = Vector3.Distance(enemy.playerObj.transform.position, transform.position);
        return dist >= range;
    }

    /// <summary>
    /// determines if the ranged attack should be commited if it meets set standards
    /// </summary>
    public void DetermineRangedAttack()
    {
        // make the enemy stop moving towards the player
        enemy.isAttacking = true;

        // is the enemy far away to justify a ranged attack
        if(CalculateDistanceToPlayer())
        {
            // see if there is a wall in the way
            if(CheckAttack())
            {
                BeamCoroutine = StartCoroutine(DoRangedAttack());
            }
            // else reset timer
            else
            {
                Debug.Log("dont the ranged attack! WALL IN WAY");
                rangedCoroutine = StartCoroutine(CooldownCountdown(failedReDoRangedCooldownTime));
            }
        }
        // else reset timer
        else
        {
            Debug.Log("dont the ranged attack! TOO CLOSE!");
            rangedCoroutine = StartCoroutine(CooldownCountdown(failedReDoRangedCooldownTime));
        }
    }

    /// <summary>
    /// countdown for whether to perform ranged attack
    /// </summary>
    private IEnumerator CooldownCountdown(float cooldownTime)
    {
        // allow the enemy to roam
        enemy.isAttacking = false;

        float totalTime = 0;
        
        // wait time for when the ranged attack should be determined
        while (totalTime <= cooldownTime)
        {
            totalTime += Time.deltaTime;
            yield return null;
        }

        DetermineRangedAttack();
    }
}
