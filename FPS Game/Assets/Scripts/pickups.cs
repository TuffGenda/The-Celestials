using UnityEngine;

// Pickups script by Tuff Genda.
public class pickups : MonoBehaviour
{
    enum pickupType
    {
        guns, money, objective, collectible, allies
    }

    [Header("Components")]
    [SerializeField] pickupType type;

    [Header("For Guns")]
    [SerializeField] gunStats gun;

    [Header("For Money")]
    [SerializeField] int moneyAmount = 10;

    [Header("For Collectibles")]
    [SerializeField] int collectibleID;

    [Header("For Allies")]
    [SerializeField] SurvivorStats survivorStats;
    [SerializeField] GameObject ally;

    private void Start()
    {
    if (type == pickupType.objective)
        {
            levelManager.instance.updateRequiredItems();
            // I changed this to updateEnemies instead since that is the new function in gamemanager.
            gamemanager.instance.updateItems(1);
        }
    }

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

            // This lowers the items in the UI by one.
            gamemanager.instance.updateItems(-1);

            Destroy(gameObject);
        }

        else if (pickupable != null && other.CompareTag("Player") && type == pickupType.collectible)
        {
            gamemanager.instance.updateCollectibles(collectibleID);
            Destroy(gameObject);
        }

        else if (pickupable != null && other.CompareTag("Player") && type == pickupType.allies)
        {
            Instantiate(ally, transform.position, transform.rotation);

            AllyAI allyAIScript = ally.GetComponent<AllyAI>();
            allyAIScript.GetAllyStats(survivorStats);

            Destroy(gameObject);
        }
    }
}
