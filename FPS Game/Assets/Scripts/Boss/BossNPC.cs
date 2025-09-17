using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class BossNPC : MonoBehaviour, IAllowDamage
{
    [SerializeField] Renderer model;
    [SerializeField] public NavMeshAgent agent;
    [SerializeField] public int faceTargetSpeed;
    [SerializeField] int HP;
    [SerializeField] int FOV;

    [SerializeField] GameObject bullet;
    [SerializeField] public float shootRate;
    [SerializeField] Transform shootPos;

    // --- Drop Loot Variables ---
    [Header("--- Drop Loot ---")]
    [SerializeField] GameObject[] lootDrops; // Array of loot items the enemy can drop
    [SerializeField] int lootDropChance; // A percentage chance (0-100) that loot will drop

    public MoveToPlayer moveToPlayer = new MoveToPlayer();
    public Idle idle = new Idle();
    public Attack attack = new Attack();
    public TurnToPlayer turnToPlayer = new TurnToPlayer();
    public float shootTimer;
    public float angleToPlayer;
    public float stoppingDistanceOriginal;
    public bool playerInTrigger;
    public Vector3 playerDirection;

    private string currentStateName;
    private IBossInterface currentState;
    private Color colorOriginal;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOriginal = model.material.color;
        stoppingDistanceOriginal = agent.stoppingDistance;
        playerInTrigger = false;
        currentState = idle;
    }

    // Update is called once per frame
    void Update()
    {
        shootTimer += Time.deltaTime;
        playerDirection = gamemanager.instance.player.transform.position - transform.position;

        currentState = currentState.doState(this);
        currentStateName = currentState.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
        }

        agent.stoppingDistance = 0;
    }

    public bool canSeePlayer()
    {
        if (playerInTrigger)
        {
            angleToPlayer = Vector3.Angle(playerDirection, transform.forward);

            RaycastHit hit;
            if (Physics.Raycast(transform.position, playerDirection, out hit))
            {
                if (hit.collider.CompareTag("Player") && angleToPlayer <= FOV)
                {
                    agent.stoppingDistance = stoppingDistanceOriginal;
                    return true;
                }
            }
        }

        agent.stoppingDistance = 0;
        return false;
    }

    public void createBullet()
    {
        Instantiate(bullet, shootPos.position, transform.rotation);
    }

    public void TakeDamage(int amount)
    {
        if (HP > 0)
        {
            HP -= amount;
            StartCoroutine(flashRed());

            agent.SetDestination(gamemanager.instance.player.transform.position);
        }

        if (HP <= 0)
        {
            dropLoot(); // Call the dropLoot function on death
            Destroy(gameObject);
        }
    }

    public void HealDamage(int amount, bool onCooldown)
    {

    }

    /// <summary>
    /// Handles the dropping of loot when the enemy is defeated.
    /// </summary>
    void dropLoot()
    {
        // Check if a random number is less than or equal to the drop chance
        if (Random.Range(0, 101) <= lootDropChance)
        {
            // Check if there are any loot items to drop
            if (lootDrops.Length > 0)
            {
                // Select a random loot item from the array
                int randomIndex = Random.Range(0, lootDrops.Length);
                GameObject itemToDrop = lootDrops[randomIndex];

                // Instantiate the selected loot item at the enemy's position
                Instantiate(itemToDrop, transform.position, Quaternion.identity);
            }
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOriginal;
    }
}
