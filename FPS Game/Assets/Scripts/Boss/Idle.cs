using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class Idle : IBossInterface
{
    public IBossInterface doState(BossNPC boss)
    {
        standStill(boss);

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

    private void standStill(BossNPC boss)
    {
        boss.agent.SetDestination(boss.transform.position);
    }
}
