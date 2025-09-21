using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class Attack : IBossInterface
{
    public IBossInterface doState(BossNPC boss)
    {
        shoot(boss);

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

    private void shoot(BossNPC boss)
    {
        boss.shootTimer = 0;

        boss.StartCoroutine(boss.playAnimation("Spit", 1));

        boss.createBullet();
    } 
}
