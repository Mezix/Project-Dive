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

    public void EnemyKilled(AEnemy enemy)
    {
        if (EnemyDead != null) EnemyDead(enemy);
    }
    public void PlayerKilled()
    {
        if (PlayerDead != null) PlayerDead();
    }
    public void PlayerDying()
    {
        if (PlayerIsDying != null)PlayerIsDying();
    }
    public void DoubleClickAttempted(GameObject obj)
    {
        if (CheckDoubleClick != null) CheckDoubleClick(obj);
    }
}
