using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using Unity.VisualScripting;

public class BossNPC : MonoBehaviour, IAllowDamage
{
    [SerializeField] public NavMeshAgent agent;
    [SerializeField] public Animator anim;
    [SerializeField] AudioClip hurtSound;

    [SerializeField] public int faceTargetSpeed;
    [SerializeField] int HP;
    [SerializeField] int FOV;
    [SerializeField] public int animTransitionSpeed;

    [SerializeField] GameObject bullet;
    [SerializeField] public float shootRate;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;

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
    public bool death;
    public Vector3 playerDirection;

    private string currentStateName;
    private IBossInterface currentState;
    private Color colorOriginal;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        death = false;
        stoppingDistanceOriginal = agent.stoppingDistance;
        playerInTrigger = false;
        currentState = idle;
    }

    // Update is called once per frame
    void Update()
    {
        SetAnimLoco();

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
            playerDirection = gamemanager.instance.player.transform.position - headPos.position;
            angleToPlayer = Vector3.Angle(playerDirection, transform.forward);

            RaycastHit hit;
            if (Physics.Raycast(headPos.position, playerDirection, out hit))
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
            gamemanager.instance.playerScript.plrSoundSource.PlayOneShot(hurtSound);

            HP -= amount;

            StartCoroutine(playAnimation("Damaged", 2));

            agent.SetDestination(gamemanager.instance.player.transform.position);
        }

        if (HP <= 0)
        {
            dropLoot(); // Call the dropLoot function on death

            anim.SetBool("Death", true);

            StartCoroutine(DelayDeath());
        }
    }

    public void HealDamage(int amount, bool onCooldown)
    {

    }

    void SetAnimLoco()
    {
        float agentSpeedCurrent = agent.velocity.normalized.magnitude;
        float animSpeedCurrent = anim.GetFloat("Speed");

        anim.SetFloat("Speed", Mathf.Lerp(animSpeedCurrent, agentSpeedCurrent, Time.deltaTime + animTransitionSpeed));
    }

    public IEnumerator playAnimation(string name, int seconds)
    {
        anim.SetBool(name, true);
        yield return new WaitForSeconds(seconds);
        anim.SetBool(name, false);
    }

    IEnumerator DelayDeath()
    {
        death = true;
        agent.speed = 0;
        agent.acceleration = 0;
        faceTargetSpeed = 0;
        FOV = 0;
        agent.stoppingDistance = 99999;
        agent.angularSpeed = 0;
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
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
}
