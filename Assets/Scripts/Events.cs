using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Events : MonoBehaviour
{
    public static Events instance;
    private void Awake()
    {
        instance = this;
    }
    public event Action<AEnemy> EnemyDead;
    public event Action PlayerDead;
    public event Action PlayerIsDying;
    public event Action<GameObject> CheckDoubleClick;

    public event Action<float> DamageDealtByPlayer;

    public void EnemyKilledEvent(AEnemy enemy)
    {
        if (EnemyDead != null) EnemyDead(enemy);
    }
    public void PlayerKilledEvent()
    {
        if (PlayerDead != null) PlayerDead();
    }
    public void PlayerDyingEvent()
    {
        if (PlayerIsDying != null)PlayerIsDying();
    }
    public void DoubleClickAttemptedEvent(GameObject obj)
    {
        if (CheckDoubleClick != null) CheckDoubleClick(obj);
    }
    public void DamageDealtEvent(float damage)
    {
        if (DamageDealtByPlayer != null) DamageDealtByPlayer(damage);
    }
}
