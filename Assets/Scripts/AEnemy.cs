using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AEnemy : MonoBehaviour
{
    public float AttacksPerSecond { get; set; }
    public float TimeBetweenAttacks { get; private set; }
    public float TimeElapsedBetweenLastAttack { get; protected set; }


    public void InitStats()
    {
        TimeBetweenAttacks = 1 / AttacksPerSecond;
        TimeElapsedBetweenLastAttack = TimeBetweenAttacks;
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("Enemy Hit");
        EnemyKilled();
    }
    public void EnemyKilled()
    {
        Events.instance.EnemyKilled(gameObject);
    }
}
