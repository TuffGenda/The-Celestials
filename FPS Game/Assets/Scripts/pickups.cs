using UnityEngine;

public class pickups : MonoBehaviour
{
    enum pickupType
    {
        guns, coins
    }
    [Header("Components")]
    [SerializeField] pickupType type;

    [Header("For Guns")]
    [SerializeField] gunStats gun;

    // Will add once I know what the coin system is.
    /*[Header("For Coins")]
    [SerializeField] coinStats coin;*/

    private void OnTriggerEnter(Collider other)
    {
        IAllowPickup pickupable = other.GetComponent<IAllowPickup>();

        if (pickupable != null && type == pickupType.guns)
        {
            pickupable.GetGunStats(gun);

            gun.ammoCur = gun.ammoMax;

            Destroy(gameObject);
        }
    }
}
