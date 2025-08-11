using UnityEngine;
using System.Collections;
using UnityEngine.AI;

// IDamage interface definition removed from here, as it exists in IDamage.cs


public class enemyAI : MonoBehaviour, IDamage, IOpen
{
    [Header("--- Components ---")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator; // Ensure you assign your Animator component here
    [SerializeField] AudioSource audioSource; // For sound effects

    [Header("--- Health ---")]
    [SerializeField] float HP = 100f; // Default HP
    Material defaultMaterial;
    Color defaultColor;
    Color flashColor = Color.red; // Use Color.red for simplicity or your custom color

    [Header("--- Movement & Detection ---")]
    [SerializeField] float detectionRange = 10f; // How far the enemy can detect the player
    [SerializeField] float attackRange = 2f; // How close the enemy needs to be to attack (if melee)
    [SerializeField] float faceTargetSpeed = 5f; // Speed at which the enemy rotates to face the player
    [SerializeField] float wanderRadius = 15f; // Radius for wandering behavior
    [SerializeField] float wanderTimer = 5f; // How long to wander before finding a new point

    // Internal state variables
    Vector3 playerDir;
    bool playerInDetectionRange;
    bool playerInAttackRange;
    float currentWanderTimer;

    [Header("--- Combat Settings ---")]
    [SerializeField] bool isShooter; // Is this enemy a shooter?
    [SerializeField] Transform shootPos; // Point from where bullets are spawned
    [SerializeField] GameObject bulletPrefab; // The bullet GameObject to instantiate
    [SerializeField] float shootRate = 1f; // How often the enemy can shoot
    float lastShootTime;

    [SerializeField] bool isJumpy; // Does this enemy jump?
    [SerializeField] float jumpForce = 5f; // How high the enemy jumps

    [Header("--- Audio Clips ---")]
    [SerializeField] AudioClip damageSound;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip attackSound;


    void Start()
    {
        // Initialize material colors for damage flash
        defaultMaterial = model.material;
        defaultColor = model.material.color;

        // Set initial wander timer
        currentWanderTimer = wanderTimer;

        // Ensure NavMeshAgent is enabled
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
            if (agent == null)
            {
                Debug.LogError("NavMeshAgent component not found on " + gameObject.name);
                enabled = false; // Disable script if no NavMeshAgent
            }
        }

        // Ensure Animator is assigned or try to get it
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        // Ensure AudioSource is assigned or try to get it
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        // If gameManager exists and has a goal, update it
        if (gameManager.instance != null)
        {
            gameManager.instance.updateGameGoal(1);
        }
    }

    void Update()
    {
        if (gameManager.instance == null || gameManager.instance.player == null)
        {
            // If player or gameManager is not available, do nothing
            agent.isStopped = true;
            return;
        }

        // Calculate player direction and distance
        playerDir = gameManager.instance.player.transform.position - transform.position;
        float playerDistance = playerDir.magnitude;

        // Check if player is in detection range
        playerInDetectionRange = playerDistance <= detectionRange;
        // Check if player is in attack range (useful for melee or close-range shooters)
        playerInAttackRange = playerDistance <= attackRange;

        if (playerInDetectionRange)
        {
            // Player detected: pursue and potentially attack
            agent.SetDestination(gameManager.instance.player.transform.position);
            agent.isStopped = false; // Ensure agent is moving

            // Face the player if within stopping distance or attacking
            if (agent.remainingDistance <= agent.stoppingDistance || playerInAttackRange)
            {
                faceTarget();
            }

            // Attack logic (shoot or jump)
            if (isShooter && Time.time - lastShootTime > shootRate)
            {
                if (playerInAttackRange) // Only shoot if within attack range
                {
                    Shoot();
                    lastShootTime = Time.time;
                }
            }

            if (isJumpy && Time.time - lastShootTime > shootRate) // Using shootRate for jump delay
            {
                if (playerInAttackRange) // Only jump if within attack range
                {
                    Jump();
                    lastShootTime = Time.time;
                }
            }
        }
        else
        {
            // Player not detected: wander
            Wander();
        }

        // Update animator speed parameter
        if (animator != null)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
    }

    /// <summary>
    /// Handles the enemy shooting a projectile.
    /// </summary>
    void Shoot()
    {
        if (bulletPrefab != null && shootPos != null)
        {
            // Instantiate the bullet, adjusting its initial position slightly forward
            Instantiate(bulletPrefab, shootPos.position + (transform.forward * 0.5f), transform.rotation);
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

    /// <summary>
    /// Makes the enemy jump.
    /// </summary>
    void Jump()
    {
        // Add a force to make the enemy jump
        // agent.baseOffset
        Rigidbody rb = GetComponent<Rigidbody>();

        if (audioSource != null && attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }
    }

    /// <summary>
    /// Rotates the enemy to face the player.
    /// </summary>
    void faceTarget()
    {
        // Only rotate around the Y-axis to prevent enemy tilting
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    /// <summary>
    /// Implements wandering behavior when the player is not in detection range.
    /// </summary>
    void Wander()
    {
        agent.isStopped = false; // Ensure agent can move for wandering

        currentWanderTimer += Time.deltaTime;

        if (currentWanderTimer >= wanderTimer)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos);
            currentWanderTimer = 0; // Reset timer
        }

        // If the agent has reached its destination, reset the timer to find a new point sooner
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentWanderTimer = wanderTimer; // Trigger finding a new point immediately
        }
    }

    /// <summary>
    /// Finds a random point on the NavMesh within a specified radius.
    /// </summary>
    /// <param name="origin">The center point for the sphere.</param>
    /// <param name="dist">The radius of the sphere.</param>
    /// <param name="layermask">The NavMesh layer mask.</param>
    /// <returns>A random point on the NavMesh.</returns>
    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * dist;
        randomDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, dist, layermask);
        return navHit.position;
    }

    /// <summary>
    /// Handles visual flash when the enemy takes damage.
    /// </summary>
    IEnumerator DamageFlash()
    {
        model.material.color = flashColor;
        yield return new WaitForSeconds(0.1f); // Slightly longer flash duration
        if (gameObject != null) // Check if the GameObject still exists before accessing its material
        {
            model.material.color = defaultColor;
        }
    }

    /// <summary>
    /// Reduces enemy HP and handles death.
    /// </summary>
    /// <param name="amount">The amount of damage taken.</param>
    public void takeDamage(float amount)
    {
        HP -= amount;

        if (audioSource != null && damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);
        }

        if (HP <= 0)
        {
            if (gameManager.instance != null)
            {
                gameManager.instance.updateGameGoal(-1); // Decrease enemy count
            }

            if (audioSource != null && deathSound != null)
            {
                // Play death sound and then destroy after the sound finishes
                audioSource.PlayOneShot(deathSound);
                Destroy(gameObject, deathSound.length);
            }
            else
            {
                Destroy(gameObject); // Destroy immediately if no death sound
            }
        }
        else
        {
            StartCoroutine(DamageFlash());
        }
    }

    /// <summary>
    /// Gizmos for visualizing detection and attack ranges in the editor.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}