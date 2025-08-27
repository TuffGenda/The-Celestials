using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IAllowDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;

    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int fov;
    [SerializeField] int roamDistance;
    [SerializeField] int roamPauseTimer;

    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    [SerializeField] Transform shootPos;

    // --- Drop Loot Variables ---
    [Header("--- Drop Loot ---")]
    [SerializeField] GameObject[] lootDrops; // Array of loot items the enemy can drop
    [SerializeField] int lootDropChance; // A percentage chance (0-100) that loot will drop

    Color colorOriginal;

    int HPOriginal;
    float shootTimer;
    float roamTimer;
    float playerAngle;
    float stoppingDistanceOriginal;

    bool playerInTrigger;

    Vector3 playerDirection;
    Vector3 startPos;

    // New state variables
    bool playerInDetectionRange;
    bool playerInAttackRange;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPOriginal = HP;
        colorOriginal = model.material.color;
        gamemanager.instance.updateGameGoal(1);
        startPos = transform.position;
        stoppingDistanceOriginal = agent.stoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        shootTimer += Time.deltaTime;

        playerDirection = gamemanager.instance.player.transform.position - transform.position;
        playerAngle = Vector3.Angle(playerDirection, transform.forward);
        Debug.DrawRay(transform.position, playerDirection, Color.red);

        // Use consistent state checks for player detection and attack
        playerInDetectionRange = playerAngle <= fov && playerInTrigger;
        playerInAttackRange = playerInDetectionRange && agent.remainingDistance <= agent.stoppingDistance;

        if (playerInDetectionRange)
        {
            // Player is detected, so pursue them
            agent.SetDestination(gamemanager.instance.player.transform.position);

            if (playerInAttackRange)
            {
                // Player is in attack range, so face and attack
                FaceTarget();
                if (shootTimer >= shootRate)
                {
                    Shoot();
                }
            }

            // Reset stopping distance for pursuit
            agent.stoppingDistance = stoppingDistanceOriginal;
        }
        else
        {
            // Player not detected, roam
            CheckRoam();
            agent.stoppingDistance = 0;
        }

        if (agent.remainingDistance < 0.01f && roamPauseTimer != -1)
        {
            roamTimer += Time.deltaTime;
        }
    }

    void CheckRoam()
    {
        if (roamTimer >= roamPauseTimer && agent.remainingDistance < 0.01f)
        {
            Roam();
        }
    }

    void Roam()
    {
        if (roamDistance != 0)
        {
            roamTimer = 0;
            Vector3 ranPos = Random.insideUnitSphere * roamDistance;
            ranPos += startPos;
            NavMeshHit hit;
            NavMesh.SamplePosition(ranPos, out hit, roamDistance, 1);
            agent.SetDestination(hit.position);
        }
    }

    // `canSeePlayer` has been removed as its logic is now in `Update`.

    void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
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
    }

    void Shoot()
    {
        shootTimer = 0;
        Instantiate(bullet, shootPos.position, transform.rotation);
    }

    public void TakeDamage(int amount)
    {
        if (HP > 0)
        {
            HP -= amount;
            StartCoroutine(FlashRed());
        }

        if (HP <= 0)
        {
            // Call the level manager to update the enemies killed count
            levelManager.instance.EnemyKilled();
            gamemanager.instance.updateGameGoal(-1);
            dropLoot();
            Destroy(gameObject);
        }
    }

    public void HealDamage(int amount, bool onCooldown)
    {
        if (onCooldown == false && HP < HPOriginal)
        {
            HP += amount;

            if (HP > HPOriginal)
            {
                HP = HPOriginal;
            }
        }
    }

    IEnumerator FlashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOriginal;
    }

    void dropLoot()
    {
        if (Random.Range(0, 101) <= lootDropChance)
        {
            if (lootDrops.Length > 0)
            {
                int randomIndex = Random.Range(0, lootDrops.Length);
                GameObject itemToDrop = lootDrops[randomIndex];
                Instantiate(itemToDrop, transform.position, Quaternion.identity);
            }
        }
    }
}