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

    Color colorOriginal;

    int HPOriginal;
    float shootTimer;
    float roamTimer;
    float playerAngle;
    float stoppingDistanceOriginal;

    bool playerInTrigger;

    Vector3 playerDirection;
    Vector3 startPos;

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
        if (agent.remainingDistance < 0.01f && roamPauseTimer != -1)
        {
            roamTimer += Time.deltaTime;
        }

        if (playerInTrigger && !canSeePlayer())
        {
            CheckRoam();
        }
        else if (!playerInTrigger)
        {
            CheckRoam();
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
        if (roamDistance != 0) {
            roamTimer = 0;

            agent.stoppingDistance = 0;

            Vector3 ranPos = Random.insideUnitSphere * roamDistance;
            ranPos += startPos;

            NavMeshHit hit;
            NavMesh.SamplePosition(ranPos, out hit, roamDistance, 1);
            agent.SetDestination(hit.position);
        }
    }

    bool canSeePlayer()
    {
        playerDirection = gamemanager.instance.player.transform.position - transform.position;
        playerAngle = Vector3.Angle(playerDirection, transform.forward);
        Debug.DrawRay(transform.position, playerDirection, Color.red);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerDirection, out hit))
        {

        }
        if (hit.collider.CompareTag("Player") && playerAngle <= fov)
        {
            agent.SetDestination(gamemanager.instance.player.transform.position);

            if (shootTimer >= shootRate)
            {
                Shoot();
            }

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                FaceTarget();
            }

            agent.stoppingDistance = stoppingDistanceOriginal;

            return true;
        }

        agent.stoppingDistance = 0;

        return false;
    }

    void FaceTarget()
    {
        Quaternion rot = transform.rotation = Quaternion.LookRotation(playerDirection);
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

        agent.stoppingDistance = 0;
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

            agent.SetDestination(gamemanager.instance.player.transform.position);
        }

        if (HP <= 0)
        {
            gamemanager.instance.updateGameGoal(-1);
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
}