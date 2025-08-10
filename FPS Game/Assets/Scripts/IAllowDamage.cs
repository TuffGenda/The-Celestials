using UnityEngine;

public interface IAllowDamage
{
    void TakeDamage(int amount);
    void HealDamage(int amount);
}
