using UnityEngine;

public class TurnToPlayer : IBossInterface
{
    public IBossInterface doState(BossNPC boss)
    {
        faceTarget(boss);

        if (boss.shootTimer >= boss.shootRate && boss.playerInTrigger && !boss.death)
        {
            return boss.attack;
        }
        else if (boss.playerInTrigger && !boss.death)
        {
            return boss.moveToPlayer;
        }
        else if (boss.agent.remainingDistance <= boss.agent.stoppingDistance && !boss.death)
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
