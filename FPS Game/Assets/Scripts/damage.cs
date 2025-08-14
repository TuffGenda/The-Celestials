using UnityEngine;
using System.Collections;

public class damage : MonoBehaviour
{
    enum damageType
    {
        moving, stationary, DOT, homing, falling
    }

    [Header("--- Components ---")]
    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb;
    [SerializeField] Collider floorCollider;

    [Header("--- For Everything ---")]
    [SerializeField] int damageAmount;

    [Header("--- For DOT ---")]
    [SerializeField] float damageRate;

    [Header("--- For Homing and Moving ---")]
    [SerializeField] int speed;
    [SerializeField] float destroyTime;

    [Header("--- For Fall Damage ---")]
    [SerializeField] float minimumFallVelocity;

    bool isDamaging;
    bool isOnGround;

    float lastYVelocity;
    float initialGroundY;
    float groundY;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (type == damageType.moving || type == damageType.homing)
        {
            Destroy(gameObject, destroyTime);

            if (type == damageType.moving)
            {
                rb.linearVelocity = transform.forward * speed;
            }
        }

        if (type == damageType.falling)
        {
            initialGroundY = gamemanager.instance.player.transform.position.y - floorCollider.transform.position.y + 0.08f;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (type == damageType.homing)
        {
            rb.linearVelocity = (gamemanager.instance.player.transform.position - transform.position).normalized * speed * Time.deltaTime;
        }

        if (type == damageType.falling)
        {
            CalculateFallDamage();
        }
    }

    void CalculateFallDamage()
    {
        groundY = gamemanager.instance.player.transform.position.y - floorCollider.transform.position.y;

        if (groundY > initialGroundY)
        {
            isOnGround = false;
            lastYVelocity = gamemanager.instance.player.GetComponent<CharacterController>().velocity.y;
        }
        else
        {
            isOnGround = true;
        }

        if (isOnGround && lastYVelocity < minimumFallVelocity)
        {
            IAllowDamage dmg = gamemanager.instance.player.GetComponent<IAllowDamage>();

            if (dmg != null)
            {
                dmg.TakeDamage(damageAmount);
                lastYVelocity = 0.00f;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        IAllowDamage dmg = other.GetComponent<IAllowDamage>();

        if (dmg != null && type != damageType.DOT)
        {
            dmg.TakeDamage(damageAmount);
        }

        if (type == damageType.moving || type == damageType.homing)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        IAllowDamage dmg = other.GetComponent<IAllowDamage>();

        if (dmg != null && type == damageType.DOT)
        {
            if (!isDamaging)
            {
                StartCoroutine(damageOther(dmg));
            }
        }
    }

    IEnumerator damageOther(IAllowDamage d)
    {
        isDamaging = true;
        d.TakeDamage(damageAmount);
        yield return new WaitForSeconds(damageRate);
        isDamaging = false;
    }
}
