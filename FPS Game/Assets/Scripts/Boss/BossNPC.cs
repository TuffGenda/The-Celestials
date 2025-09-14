using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class BossNPC : MonoBehaviour
{
    [SerializeField] public NavMeshAgent agent;
    [SerializeField] public int faceTargetSpeed;
    [SerializeField] int HP;
    [SerializeField] int FOV;

    [SerializeField] GameObject bullet;
    [SerializeField] public float shootRate;
    [SerializeField] Transform shootPos;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
}
