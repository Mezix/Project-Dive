using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AEnemy : MonoBehaviour
{
    public float AttacksPerSecond { get; set; }
    public float TimeBetweenAttacks { get; private set; }
    public float TimeElapsedBetweenLastAttack { get; protected set; }
    public EnemyHealth _enemyHealth;
    public bool _enemyDead;

    public void InitStats()
    {
        TimeBetweenAttacks = 1 / AttacksPerSecond;
        TimeElapsedBetweenLastAttack = TimeBetweenAttacks;
        _enemyDead = false;
    }
    public void TakeDamage(int damage)
    {
        if(_enemyHealth.TakeDamage(damage))
        {
            EnemyKilled();
        }
        else
        {
            DamageTakenAnim();
        }
    }
    private void DamageTakenAnim()
    {
        Debug.Log("Init Damage Taken Anim");
    }
    public void EnemyKilled()
    {
        _enemyDead = true;
        Debug.Log("Enemy Killed");
        Events.instance.EnemyKilled(gameObject);
        Destroy(gameObject);
    }
}
