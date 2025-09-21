using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class AllyAI : MonoBehaviour, IAllowPickup
{
    [SerializeField] Animator anim;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int HP;
    [SerializeField] int FOV;

    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    [SerializeField] Transform shootPos;

    [SerializeField] float detectionRadius = 20f;
    [SerializeField] LayerMask enemyLayerMask;

    Color colorOriginal;

    float shootTimer;
    int animTransitionSpeed;

    // I removed these ints in favor of using the serialized field ones. - Tuff Genda.

    enum AllyState { Follow, Hold }
    AllyState currentState = AllyState.Follow;

    Transform player;
    Vector3 holdPosition;


    void Start()
    {
        player = gamemanager.instance.player.transform;

    }

    void Update()
    {
        SetAnimLoco();

        shootTimer += Time.deltaTime;

        switch (currentState)
        {
            case AllyState.Follow:
                FollowPlayer();
                break;

            case AllyState.Hold:
                HoldPosition();
                break;
        }

        LookForEnemies();
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

    void FollowPlayer()
    {
        if (player != null)
        {
            agent.stoppingDistance = 5f;
            agent.SetDestination(player.position);
        }
    }

    void HoldPosition()
    {
        agent.stoppingDistance = 0f;
        agent.SetDestination(holdPosition);
    }

    void LookForEnemies()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayerMask);
        foreach (Collider hit in hits)
        {
            if (hit.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                Vector3 dir = hit.transform.position - transform.position;
                float angle = Vector3.Angle(dir, transform.forward);
                if (angle <= FOV)
                {
                    if (shootTimer >= shootRate)
                    {
                        Shoot(dir);
                    }

                    Quaternion rot = Quaternion.LookRotation(dir);
                    transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
                }
            }
        }
    }

    void Shoot(Vector3 dir)
    {
        shootTimer = 0f;

        StartCoroutine(playAnimation("Shoot", 1));

        Instantiate(bullet, shootPos.position, Quaternion.LookRotation(dir));
    }

    // --- Commands ---
    public void CommandHold(Vector3 position)
    {
        holdPosition = position;
        currentState = AllyState.Hold;
    }

    public void CommandFollow()
    {
        currentState = AllyState.Follow;
    }

    public void TakeDamage(int amount)
    {
        if (HP > 0)
        {
            HP -= amount;
        }

        if (HP <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void GetGunStats(gunStats gun)
    {
    }

    public void GetAllyStats(SurvivorStats survivorStats)
    {
        HP = survivorStats.HP;
        FOV = survivorStats.FOV;
        faceTargetSpeed = survivorStats.FaceTargetSpeed;
        shootRate = survivorStats.ShootRate;
        detectionRadius = survivorStats.DetectionRadius;
        bullet = survivorStats.Bullet;
        agent.speed = survivorStats.Speed;
        agent.acceleration = survivorStats.Acceleration;
    }
}

