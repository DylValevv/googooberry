using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public EnemyManager enemyManager;
    public GameState gameState;

    private GameObject playerObj;
    private NavMeshAgent navMeshAgent;

    [Header("Attack Ranges")]
    [SerializeField] CapsuleCollider meleeRange;
    [SerializeField] CapsuleCollider rangedRange;

    [Header("Enemy Stats (Don't edit here, set in EnemyManager)")]
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
        SetDestination(); //Reroute towards player 
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
}
