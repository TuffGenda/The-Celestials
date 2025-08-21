using UnityEngine;

public class gunStats : MonoBehaviour
{
    public GameObject model;
    public WeaponType weaponType = WeaponType.Rifle;

    [Range(1, 100)] public int shootDamage = 25;
    [Range(5, 1000)] public int shootDist = 200;
    [Range(0.1f, 5f)] public float shootRate = 1f;
    [Range(0f, 50f)] public float critChance = 5f;

    public int ammoCur = 30;
    [Range(5, 50)] public int ammoMax = 30;
    [Range(0.5f, 5f)] public float reloadTime = 2f;

    [Range(0f, 2f)] public float recoil = 0.3f;
    [Range(0.5f, 1f)] public float moveSpeed = 0.9f;

    [Range(0, 20000)] public int price = 1000;
    [Range(1, 50)] public int unlockLevel = 1;
    public WeaponRarity rarity = WeaponRarity.Common;

    private void Start()
    {
        ApplyRarityBonus();
    }

    void ApplyRarityBonus()
    {
        switch (rarity)
        {
            case WeaponRarity.Common:
                break; // No bonus
            case WeaponRarity.Rare:
                shootDamage += 5;
                critChance += 5f;
                break;
            case WeaponRarity.Epic:
                shootDamage += 10;
                critChance += 10f;
                recoil *= 0.8f;
                break;
            case WeaponRarity.Legendary:
                shootDamage += 20;
                critChance += 15f;
                recoil *= 0.6f;
                break;
        }
    }

    public Color GetRarityColor()
    {
        switch (rarity)
        {
            case WeaponRarity.Common: return Color.white;
            case WeaponRarity.Rare: return Color.blue;
            case WeaponRarity.Epic: return new Color(0.6f, 0.2f, 0.8f);
            case WeaponRarity.Legendary: return Color.yellow;
            default: return Color.white;

        }
    }
}

public enum WeaponType
{
    Pistol,
    Rifle,
    Shotgun,
    SMG
}

public enum WeaponRarity
{
    Common,
    Rare,
    Epic,
    Legendary
}
