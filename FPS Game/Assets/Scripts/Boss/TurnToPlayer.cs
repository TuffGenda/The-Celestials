using UnityEngine;

public class TurnToPlayer : IBossInterface
{
    public IBossInterface doState(BossNPC boss)
    {
        faceTarget(boss);

        if (boss.shootTimer >= boss.shootRate && boss.canSeePlayer())
        {
            return boss.attack;
        }
        else if (boss.canSeePlayer())
        {
            return boss.moveToPlayer;
        }
        else if (boss.agent.remainingDistance <= boss.agent.stoppingDistance)
        {
            return boss.turnToPlayer;
        }
        else
        {
            return boss.idle;
        }
    }
    
    private void faceTarget(BossNPC boss)
    {
        Quaternion rot = Quaternion.LookRotation(boss.playerDirection);
        boss.transform.rotation = Quaternion.Lerp(boss.transform.rotation, rot, Time.deltaTime * boss.faceTargetSpeed);
    }
}
