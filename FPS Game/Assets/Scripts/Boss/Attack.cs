using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class Attack : IBossInterface
{
    public IBossInterface doState(BossNPC boss)
    {
        shoot(boss);

        if (boss.shootTimer >= boss.shootRate)
        {
            return boss.attack;
        }
        else if (boss.agent.remainingDistance <= boss.agent.stoppingDistance)
        {
            return boss.turnToPlayer;
        }
        else if (boss.canSeePlayer())
        {
            return boss.moveToPlayer;
        }
        else
        {
            return boss.idle;
        }
    }

    private void shoot(BossNPC boss)
    {
        boss.shootTimer = 0;
        boss.createBullet();
    } 
}
