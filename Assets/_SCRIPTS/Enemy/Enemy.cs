using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using System.Collections;
using System;

public class Enemy : MonoBehaviour
{
    public EnemyManager enemyManager;
    public GameState gameState;
    public MeleeAttack meleeAttack;

    public GameObject playerObj;
    private NavMeshAgent navMeshAgent;
    [HideInInspector] public bool isAttacking = false;
    [HideInInspector] public bool attackSuccessful = false;
    private bool hasTakenDamage = false;

    [Header("Attack Ranges")]
    [SerializeField] CapsuleCollider meleeRange;
    [SerializeField] CapsuleCollider rangedRange;

    [Header("Customize Attack")]
    [SerializeField] float meleeAttackSpeed = 1f;
    public float meleeAttackInterval = 1f;
    //[SerializeField] float ogDrag = 10;

    [Header("Enemy Scalable Stats (Don't edit here, set in EnemyManager)")]
    public int speed = 0;
    public int damage = 0;
    public int health = 0;

    [Header("Animation Variables")]
    [SerializeField] private GameObject animatedMesh;
    [SerializeField] private Animator anim;
    // the clip that the script will look for to replace through the statemachine. everytime it finds an instance of this clip, it will swap it with the new clip
    [SerializeField] private AnimationClip utilClip;
    [SerializeField] private AnimationClip[] anims;
    // this lets you keep the same state machines across all charactersa
    private AnimatorOverrideController animOverride;
    private bool dead;

    // Start is called before the first frame update
    void Start()
    {
        playerObj = GameObject.FindWithTag("Player");
        if (playerObj == null) Debug.LogError("Missing player object in scene");
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent == null) Debug.LogError("The nav mesh agent component is missing from " + gameObject.name);
        else SetDestination();
        SetStats();

        // animation initializing
        // create an animation override controller that is based off of our current animation controller
        animOverride = new AnimatorOverrideController(anim.runtimeAnimatorController);
        // assign the override controller back into the animator so it can be manipulated/be used
        anim.runtimeAnimatorController = animOverride;
    }

    // Update is called once per frame
    void Update()
    {
        if (isAttacking == false) SetDestination(); //Reroute towards player 
        //isCollidingWithPlayer = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Weapon"))
        {
            TakeDamage(gameState.playerDamage);
            //TakeDamage(1);
        }
    }

    private void SetDestination()
    {
        if (playerObj != null)
        {
            //Debug.Log(playerObj.transform.position);
            Vector3 targetVector = playerObj.transform.position;
            navMeshAgent.SetDestination(targetVector);
        }
    }

    private void SetStats()
    {
        speed = enemyManager.enemyAverageSpeed;
        damage = enemyManager.enemyAverageDamage;
        health = enemyManager.enemyAverageHealth;
    }

    public void TakeDamage(int hitDamage)
    {
        if (!hasTakenDamage)
        {
            health -= hitDamage;
            Debug.Log("Take Damage");
            if (health <= 0)
            {
                Death();
            }
            hasTakenDamage = true;
            Sequence mySequence = DOTween.Sequence();
            mySequence.AppendInterval(0.15f).OnComplete(() => CanTakeDamage());
        }
    }

    private void CanTakeDamage()
    {
        hasTakenDamage = false;
    }

    private void Death()
    {
        dead = true;
        anim.SetBool("Death", dead);
        enemyManager.enemiesRemainingInWave -= 1;

        // stop moving 
        navMeshAgent.isStopped = true;
        isAttacking = true;
        this.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);

        // after animation is done add it back to pool
        Invoke("DelayedDeath", 3);
    }

    /// <summary>
    /// a delay to return to pool so the animation can play out
    /// </summary>
    private void DelayedDeath()
    {
        FinishedAttack();
        anim.SetBool("Death", false);
        dead = false;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// turns off navmesh to stop movement
    /// </summary>
    public void RangedAttack(bool active)
    {
        if (active && !dead)
        {
            PlayAnim("anim_lizard_ranged");
            navMeshAgent.isStopped = active;
            navMeshAgent.ResetPath();
            isAttacking = active;
            this.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        }
        else
        {
            if(!dead) FinishedAttack();
        }

    }

    public void MeleeAttack()
    {
        if (this.gameObject.activeSelf && !dead)
        {
            PlayAnim("anim_lizard_melee", false);
            meleeAttack.currentlyMeleeAttacking = true;
            navMeshAgent.isStopped = true;
            navMeshAgent.ResetPath();
            isAttacking = true;
            SetArc(this.GetComponent<Rigidbody>(), this.transform.position, playerObj.transform.position, meleeAttackSpeed);
            Sequence mySequence = DOTween.Sequence();
            mySequence.AppendInterval(meleeAttackSpeed).OnComplete(() => FinishedAttack());
        }
    }

    private void FinishedAttack()
    {
        if (attackSuccessful)
        {
            DealDamage(damage);
        }

        this.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        //this.GetComponent<Rigidbody>().drag = ogDrag;
        isAttacking = false;
        attackSuccessful = false;
        meleeAttack.currentlyMeleeAttacking = false;

        meleeAttack.DoAttack();
    }

    /// <summary>
    /// Function to deal set amount of damage to player and calls player death
    /// </summary>
    /// <param name="damageAmt">the amount of damage</param>
    public void DealDamage(int damageAmt)
    {
        gameState.playerHealth -= damageAmt;

        if (gameState.playerHealth <= 0)
        {
            Debug.Log("Player died");
            gameState.playerHealth = 0;
            playerObj.GetComponent<PlayerController>().Die();
        }
    }

    private void SetArc(Rigidbody obj, Vector3 start, Vector3 end, float t)
    {
        obj.drag = 0;
        obj.gameObject.transform.position = start;
        Vector3 vel = Vector3.zero;

        vel.x = (end.x - start.x) / t;
        vel.z = (end.z - start.z) / t;
        //vel.y = ((end.y - start.y) / t) - ((Physics.gravity.y / 2) * t);

        obj.velocity = vel;
    }

    #region<Animation Functions>
    /// <summary>
    /// called when we want to swap to a new animation
    /// </summary>
    /// <param name="clip">the new clip that we are going to be swapping in</param>
    /// <param name="utilExit">does the animation loop. this is defaulted to false (it does loop)</param>
    public void AnimUtil(AnimationClip clip, bool utilExit = false)
    {
        animOverride[utilClip] = clip;
        anim.SetBool("UtilStop", false);
        anim.SetBool("UtilExit", utilExit);
        anim.SetTrigger("Util");
    }

    /// <summary>
    /// a helper function to make it easier to play specific animations by name
    /// </summary>
    /// <param name="name">the name of the animation to play</param>
    /// <param name="dontLoop">whether to loop the animation or not</param>
    public void PlayAnim(string name, bool dontLoop = false)
    {
        AnimationClip clip = Array.Find(anims, anim => anim.name == name);
        if (clip == null)
        {
            Debug.LogWarning("Animation Clip: " + name + " not found!");
            return;
        }

        AnimUtil(clip, dontLoop);
    }
    #endregion
}
