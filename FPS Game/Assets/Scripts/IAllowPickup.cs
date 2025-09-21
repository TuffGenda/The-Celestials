using UnityEngine;

public interface IAllowPickup
{
    public void GetGunStats(gunStats gun);

    public void GetAllyStats(SurvivorStats survivorStats);
}
