using Unity.VisualScripting;
using UnityEngine;
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

        if (boss.shootTimer >= boss.shootRate && boss.playerInTrigger && !boss.death)
        {
            return boss.attack;
        }
        else if (boss.agent.remainingDistance <= boss.agent.stoppingDistance && !boss.death)
        {
            return boss.turnToPlayer;
        }
        else if (boss.playerInTrigger && !boss.death)
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
