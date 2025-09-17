using UnityEngine;
using UnityEngine.AI;

public class AllyAI : MonoBehaviour
{
    [SerializeField] Renderer model;
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

    enum AllyState { Follow, Hold }
    AllyState currentState = AllyState.Follow;

    Transform player;
    Vector3 holdPosition;


    void Start()
    {
        colorOriginal = model.material.color;
        player = gamemanager.instance.player.transform;

    }

    void Update()
    {
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
            StartCoroutine(FlashRed());
        }

        if (HP <= 0)
        {
            Destroy(gameObject);
        }
    }

    System.Collections.IEnumerator FlashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOriginal;
    }
}

