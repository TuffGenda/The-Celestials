using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class MoveToPlayer : IBossInterface
{
    public IBossInterface doState(BossNPC boss)
    {
        if (boss.agent == null)
        {
            boss.GetComponent<NavMeshAgent>();
        }

        moveToPlayer(boss);

        if (boss.shootTimer >= boss.shootRate)
        {
            return boss.attack;
        }
        else if (boss.playerInTrigger && boss.agent.remainingDistance <= boss.agent.stoppingDistance)
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

    private void moveToPlayer(BossNPC boss)
    {
        boss.agent.SetDestination(gamemanager.instance.player.transform.position);
    }
}
