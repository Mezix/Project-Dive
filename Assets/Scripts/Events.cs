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
    public event Action<GameObject> EnemyDead;
    public event Action PlayerTankDestroyed;
    public event Action PlayerIsDying;
    public event Action<GameObject> CheckDoubleClick;

    public void EnemyKilled(GameObject enemyTank)
    {
        if (EnemyDead != null) EnemyDead(enemyTank);
    }
    public void PlayerDestroyed()
    {
        if (PlayerTankDestroyed != null) PlayerTankDestroyed();
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
