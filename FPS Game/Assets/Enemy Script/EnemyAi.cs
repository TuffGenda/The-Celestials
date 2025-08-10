using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    Material defaultMaterial;
    Color defaultColor;
    Color flashColor = new Color(1f, 0f, 0f, 1f);

    [SerializeField] NavMeshAgent agent;
    [SerializeField] float faceTargetSpeed;
    bool playerInRange;
    float shootTimer;
    Vector3 playerDir;

    [SerializeField] float HP;

    [SerializeField] bool shooter;
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    void Start()
    {
        defaultMaterial = model.material;
        defaultColor = model.material.color;
        gameManager.instance.updateGameGoal(1);
    }

    void shoot()
    {
        shootTimer = 0;
        Instantiate(bullet, shootPos.position, transform.rotation);
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    void Update()
    {
        playerDir = gameManager.instance.player.transform.position - transform.position;
        shootTimer += Time.deltaTime;
        if (playerInRange)
        {
            agent.SetDestination(gameManager.instance.player.transform.position);
            if (shootTimer > shootRate)
            {
                if (shooter == true)
                {
                    shoot();
                }
            }

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                faceTarget();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    IEnumerator damageFlash()
    {
        model.material.color = flashColor;
        yield return new WaitForSeconds(0.05f);
        if (gameObject != null)
            model.material.color = defaultColor;
    }

    public void takeDamage(float amount)
    {
        HP -= amount;
        if (HP <= 0)
        {
            gameManager.instance.updateGameGoal(-1);
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(damageFlash());
        }
    }
}