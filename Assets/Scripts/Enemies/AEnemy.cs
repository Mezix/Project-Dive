using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AEnemy : MonoBehaviour
{
    public EnemyStats _enemyStats;

    public float AttacksPerSecond { get; set; }
    public float TimeBetweenAttacks { get; private set; }
    public float TimeElapsedBetweenLastAttack { get; protected set; }
    public EnemyHealth _enemyHealth;
    public bool _enemyDead;

    public bool _isBoss;

    public void InitStats()
    {
        TimeBetweenAttacks = 1 / _enemyStats._attacksPerSecond;
        TimeElapsedBetweenLastAttack = TimeBetweenAttacks;
        _enemyDead = false;
        _enemyHealth.InitHealth(_enemyStats._enemyHealth);
    }
    public void TakeDamage(int damage)
    {
        if(_enemyHealth.TakeDamage(damage))
        {
            if(!_enemyDead) EnemyKilled();
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
        Events.instance.EnemyKilled(this);
    }
    public void InitDeathBehaviour()
    {
        print("death anim");
        Destroy(gameObject);
    }
}
