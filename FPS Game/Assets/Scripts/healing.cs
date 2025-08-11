using System.Collections;
using UnityEngine;

public class healing : MonoBehaviour
{
    enum healingType
    {
        temporary, permanent, timedResuable, regen
    }

    [SerializeField] healingType type;
    [SerializeField] Rigidbody rb;

    [SerializeField] int healingAmount;
    [SerializeField] float healingRate;
    [SerializeField] int uses;
    [SerializeField] int cooldown;

    bool isHealing;
    bool onCooldown;
    int originalUses;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalUses = uses;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        IAllowDamage dmg = other.GetComponent<IAllowDamage>();

        if (dmg != null && type != healingType.regen)
        {
            if (other.gameObject.CompareTag("Player") && !onCooldown)
            {
                dmg.HealDamage(healingAmount, onCooldown);

                StartCoroutine(Heal(type));
            }
        }

        if (type == healingType.temporary && uses <= 0)
        {
            Destroy(gameObject);
        }

        if (type == healingType.timedResuable && uses <= 0)
        {
            StartCoroutine(Cooldown());
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger) return;

        IAllowDamage dmg = other.GetComponent<IAllowDamage>();

        if (dmg != null && type == healingType.regen)
        {
            if (!isHealing)
            {
                StartCoroutine(HealOther(dmg));
            }
        }
    }

    IEnumerator Heal(healingType ty)
    {
        onCooldown = true;
        if (ty == healingType.temporary || ty == healingType.timedResuable)
        {
            --uses;
        }
        yield return new WaitForSeconds(cooldown);
        onCooldown = false;
    }

    IEnumerator Cooldown()
    {
        onCooldown = true;
        yield return new WaitForSeconds(cooldown);
        uses = originalUses;
        onCooldown = false;
    }

    IEnumerator HealOther(IAllowDamage d)
    {
        isHealing = true;
        d.HealDamage(healingAmount, onCooldown);
        yield return new WaitForSeconds(healingRate);
        isHealing = false;
    }
}
