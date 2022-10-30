using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanceProjectile : AProjectile
{
    public bool _launched;
    public bool _stuck;
    public AEnemy _stuckEnemy;
    public SuperCavitationLance _cavitationLance;

    public override void Awake()
    {
        base.Awake();
    }
    public override void Start()
    {
        _timeSinceLastBleedTick = _timeBetweenBleedTicks;
        MaxLifetime = Mathf.Infinity;
        _trailRenderer.gameObject.SetActive(false);
        //Events.instance.EnemyDead += CheckHarpoonStuckInDeadEnemy;
    }
    public override void FixedUpdate()
    {
        _timeSinceLastBleedTick += Time.deltaTime;
        if (_launched)
        {
            if(_stuck && _stuckEnemy && _timeSinceLastBleedTick >= _timeBetweenBleedTicks)
            {
                _stuckEnemy.TakeDamage(_bleedDamage);
                _timeSinceLastBleedTick = 0;
            }
            else
            {
                MoveProjectile();
                _trailRenderer.gameObject.SetActive(true);
            }
        }
    }
    public override void OnTriggerEnter(Collider col)
    {
        if (!HasDoneDamage && !HitPlayer && _launched)
        {
            if (col.GetComponentInChildren<AEnemy>())
            {
                AEnemy enemy = col.GetComponentInChildren<AEnemy>();
                if (!enemy._enemyDead) StickInEnemy(enemy);
            }
        }
    }

    private void StickInEnemy(AEnemy enemy)
    {
        transform.SetParent(enemy.transform, true);
        _stuckEnemy = enemy;
        _stuck = true;
        _projectileRB.isKinematic = true;
        enemy.TakeDamage(Damage);
        if (enemy._enemyDead) _cavitationLance.ForceRetractLance();
        HasDoneDamage = true;
        _timeSinceLastBleedTick = 0;
    }
    public void Unstick()
    {
        _projectileRB.isKinematic = false;
        if (!_stuckEnemy) return;
        _stuckEnemy.TakeDamage(Damage);
        _stuck = false;
        HasDoneDamage = false;
        _stuckEnemy = null;
    }
}
