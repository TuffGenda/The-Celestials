using UnityEngine;

public class pickups : MonoBehaviour
{
    enum pickupType
    {
        guns, money, objective
    }
    [Header("Components")]
    [SerializeField] pickupType type;

    [Header("For Guns")]
    [SerializeField] gunStats gun;

    [Header("For Money")]
    [SerializeField] int moneyAmount = 10;

    private void OnTriggerEnter(Collider other)
    {
        IAllowPickup pickupable = other.GetComponent<IAllowPickup>();

        if (pickupable != null && type == pickupType.guns)
        {
            pickupable.GetGunStats(gun);

            gun.ammoCur = gun.ammoMax;

            Destroy(gameObject);
        }

        else if (pickupable != null && other.CompareTag("Player") && type == pickupType.money)
        {
            currencyManager cm = Object.FindFirstObjectByType<currencyManager>();
            if (cm != null)
            {
                cm.AddMoney(moneyAmount);
            }

            Destroy(gameObject);
        }

        else if (pickupable != null && other.CompareTag("Player") && type == pickupType.objective)
        {
            levelManager.instance.CollectItem();

            gamemanager.instance.updateGameGoal(0);

            Destroy(gameObject);
        }
    }
}
