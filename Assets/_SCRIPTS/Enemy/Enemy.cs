using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public EnemyManager enemyManager;
    public GameState gameState;
    public MeleeAttack meleeAttack;

    private GameObject playerObj;
    private NavMeshAgent navMeshAgent;
    [HideInInspector] public bool isAttacking = false;
    [HideInInspector] public bool attackSuccessful = false;

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

    // Start is called before the first frame update
    void Start()
    {
        playerObj = GameObject.FindWithTag("Player");
        if (playerObj == null) Debug.LogError("Missing player object in scene");
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent == null) Debug.LogError("The nav mesh agent component is missing from " + gameObject.name);
        else SetDestination();
        SetStats();
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
            //TakeDamage(gameState.playerDamage);
            TakeDamage(1);
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
        health -= hitDamage;
        Debug.Log("Take Damage");
        if (health <= 0)
        {
            Death();
        }
    }

    private void Death()
    {
        enemyManager.enemiesRemainingInWave -= 1;
        gameObject.SetActive(false);
    }

    public void MeleeAttack()
    {
        meleeAttack.currentlyMeleeAttacking = true;
        navMeshAgent.isStopped = true;
        navMeshAgent.ResetPath();
        isAttacking = true;
        SetArc(this.GetComponent<Rigidbody>(), this.transform.position, playerObj.transform.position, meleeAttackSpeed);
        Sequence mySequence = DOTween.Sequence();
        mySequence.AppendInterval(meleeAttackSpeed).OnComplete(()=>FinishedAttack());
    }

    private void FinishedAttack()
    {
        if (attackSuccessful)
        {
            gameState.playerHealth -= damage;

            if (gameState.playerHealth <= 0)
            {
                Debug.Log("Player died");
                gameState.playerHealth = 0;
                playerObj.GetComponent<PlayerController>().Die();
            }
        }

        this.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        //this.GetComponent<Rigidbody>().drag = ogDrag;
        isAttacking = false;
        attackSuccessful = false;
        meleeAttack.currentlyMeleeAttacking = false;

        meleeAttack.DoAttack();
    }

    private void SetArc(Rigidbody obj, Vector3 start, Vector3 end, float t)
    {
        obj.drag = 0;
        obj.gameObject.transform.position = start;
        Vector3 vel = Vector3.zero;

        vel.x = (end.x - start.x) / t;
        vel.z = (end.z - start.z) / t;
        vel.y = ((end.y - start.y) / t) - ((Physics.gravity.y / 2) * t);

        obj.velocity = vel;

        
    }
}
