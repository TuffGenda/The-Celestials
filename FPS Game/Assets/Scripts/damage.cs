using UnityEngine;
using System.Collections;

public class damage : MonoBehaviour
{
    enum damageType
    {
        moving, stationary, DOT, homing, falling
    }

    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb;

    [SerializeField] int damageAmount;
    [SerializeField] float damageRate;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;
    [SerializeField] float fallVelocity;

    bool isDamaging;
    bool isFallDamage;

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

        // Include this when gameManager is finished.
        /*
        if (type == damageType.falling && gameManager.instance.player.GetComponent<CharacterController>().velocity.y <= fallVelocity)
        {
            isFallDamage = true;
        }
        else if (type == damageType.falling && gameManager.instance.player.GetComponent<CharacterController>().velocity.y >= 0 && isFallDamage)
        {
            gameManager.instance.player.GetComponent<IDamage>().TakeDamage(damageAmount);
            Debug.Log("This is fall damage!");
            isFallDamage = false;
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        // Include this when gameManager is finished.

        /*if (type == damageType.homing)
        {
            rb.linearVelocity = (gameManager.instance.player.transform.position - transform.position).normalized * speed * Time.deltaTime;
        }*/
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
