using System.Collections;
using UnityEngine;

public class healing : MonoBehaviour
{
    enum healingType
    {
        temporary, permanent, cooldown, regen
    }

    [SerializeField] healingType type;
    [SerializeField] Rigidbody rb;

    [SerializeField] int healingAmount;
    [SerializeField] int healingRate;
    [SerializeField] int uses;
    [SerializeField] int cooldown;

    bool isHealing;
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
            dmg.HealDamage(healingAmount);
        }

        if (type == healingType.temporary && uses <= 0)
        {
            Destroy(gameObject);
        }

        if (type == healingType.cooldown && uses <= 0)
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

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldown);
        uses = originalUses;
    }

    IEnumerator HealOther(IAllowDamage d)
    {
        isHealing = true;
        d.HealDamage(healingAmount);
        yield return new WaitForSeconds(healingRate);
        isHealing = false;
    }
}
