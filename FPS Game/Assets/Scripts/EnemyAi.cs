using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IAllowDamage, IOpen
{
    [Header("--- Components ---")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;
    [SerializeField] AudioSource audioSource;
    private Rigidbody rb;

    [Header("--- Health ---")]
    [SerializeField] float HP = 100f;
    Material defaultMaterial;
    Color defaultColor;
    Color flashColor = Color.red;

    [Header("--- Movement & Detection ---")]
    [SerializeField] float detectionRange = 10f;
    [SerializeField] float attackRange = 2f;
    [SerializeField] float faceTargetSpeed = 5f;
    [SerializeField] float wanderRadius = 15f;
    [SerializeField] float wanderTimer = 5f;

    Vector3 playerDir;
    bool playerInDetectionRange;
    bool playerInAttackRange;
    float currentWanderTimer;

    [Header("--- Combat Settings ---")]
    [SerializeField] bool isShooter;
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float shootRate = 1f;
    float lastShootTime;

    [SerializeField] bool isJumpy;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float jumpInterval = 3.0f;
    float jumpTimer;

    [Header("--- Audio Clips ---")]
    [SerializeField] AudioClip damageSound;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip attackSound;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning("Rigidbody component not found on " + gameObject.name + ". 'isJumpy' will not work.");
        }

        if (model != null)
        {
            defaultMaterial = model.material;
            defaultColor = model.material.color;
        }

        currentWanderTimer = wanderTimer;

        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
            if (agent == null)
            {
                Debug.LogError("NavMeshAgent component not found on " + gameObject.name);
                enabled = false;
            }
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (gamemanager.instance != null)
        {
            gamemanager.instance.updateGameGoal(1);
        }
    }

    void Update()
    {
        if (gamemanager.instance == null || gamemanager.instance.player == null)
        {
            if (agent != null) agent.isStopped = true;
            return;
        }

        playerDir = gamemanager.instance.player.transform.position - transform.position;
        float playerDistance = playerDir.magnitude;

        playerInDetectionRange = playerDistance <= detectionRange;
        playerInAttackRange = playerDistance <= attackRange;

        // Normal state: Use NavMeshAgent for movement
        if (agent != null && agent.enabled)
        {
            if (playerInDetectionRange)
            {
                if (agent.isOnNavMesh)
                {
                    agent.SetDestination(gamemanager.instance.player.transform.position);
                    agent.isStopped = false;
                }

                if (agent.remainingDistance <= agent.stoppingDistance || playerInAttackRange)
                {
                    faceTarget();
                }

                if (isShooter && Time.time - lastShootTime > shootRate && playerInAttackRange)
                {
                    Shoot();
                    lastShootTime = Time.time;
                }
            }
            else
            {
                Wander();
            }

            // Jump trigger � set to detection range instead of melee
            if (isJumpy && playerInDetectionRange)
            {
                jumpTimer += Time.deltaTime;
                if (jumpTimer >= jumpInterval)
                {
                    StartCoroutine(JumpRoutine());
                    jumpTimer = 0;
                }
            }
            else
            {
                jumpTimer = 0;
            }
        }

        if (animator != null && agent != null)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
    }

    void Shoot()
    {
        if (bulletPrefab != null && shootPos != null)
        {
            Vector3 directionToPlayer = (gamemanager.instance.player.transform.position - shootPos.position).normalized;
            Instantiate(bulletPrefab, shootPos.position, Quaternion.LookRotation(directionToPlayer));

            if (audioSource != null && attackSound != null)
            {
                audioSource.PlayOneShot(attackSound);
            }
        }
        else
        {
            Debug.LogWarning("Bullet prefab or shoot position is not assigned for " + gameObject.name);
        }
    }

    private IEnumerator JumpRoutine()
    {
        if (rb == null)
        {
            Debug.LogError("Rigidbody is missing, cannot perform jump.");
            yield break;
        }

        if (audioSource != null && attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }

        // Disable agent for jump
        agent.enabled = false;
        yield return new WaitForFixedUpdate();

        // Reset vertical velocity
        Vector3 vel = rb.linearVelocity;
        vel.y = 0f;
        rb.linearVelocity = vel;

        // Apply jump instantly
        rb.linearVelocity += Vector3.up * jumpForce;

        // Wait until grounded again
        yield return new WaitUntil(() => IsGrounded());
        yield return new WaitForSeconds(0.1f);

        agent.enabled = true;
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    void Wander()
    {
        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            currentWanderTimer += Time.deltaTime;

            if (currentWanderTimer >= wanderTimer)
            {
                Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                agent.SetDestination(newPos);
                currentWanderTimer = 0;
            }

            if (!agent.pathPending && agent.remainingDistance < 0.5f && agent.hasPath)
            {
                currentWanderTimer = wanderTimer;
            }
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * dist;
        randomDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, dist, layermask);
        return navHit.position;
    }

    IEnumerator DamageFlash()
    {
        if (model != null)
        {
            model.material.color = flashColor;
            yield return new WaitForSeconds(0.1f);
            if (gameObject != null && model != null)
            {
                model.material.color = defaultColor;
            }
        }
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;

        if (audioSource != null && damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);
        }

        if (HP <= 0)
        {
            if (gamemanager.instance != null)
            {
                gamemanager.instance.updateGameGoal(-1);
            }

            if (audioSource != null && deathSound != null)
            {
                audioSource.PlayOneShot(deathSound);
                Destroy(gameObject, deathSound.length);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            StartCoroutine(DamageFlash());
        }
    }

    public void HealDamage(int amount, bool onCooldown)
    {
        if (!onCooldown)
        {
            HP += amount;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
