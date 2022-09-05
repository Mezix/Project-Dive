using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarpoonProjectile : AProjectile
{
    public bool _launched;
    public bool _stuck;
    public AEnemy _stuckEnemy;
    public HarpoonGun _harpoonGun;
    public LineRenderer _lr;
    public override void Start()
    {
        base.Awake();
        _trailRenderer.gameObject.SetActive(false);
        Events.instance.EnemyDead += CheckHarpoonStuckInDeadEnemy;
    }
    public override void FixedUpdate()
    {
        if (_launched)
        {
            if(!_stuck) MoveProjectile();
            else
            {
                UpdateLineRenderer();
                _lr.gameObject.SetActive(true);
            }
        }
        else _lr.gameObject.SetActive(false);
    }

    private void UpdateLineRenderer()
    {
        _lr.transform.LookAt(_harpoonGun._projectileSpots[0].UpgradeTierSpots[0].transform.position);
        _lr.SetPosition(0, Vector3.zero);
        _lr.SetPosition(1, new Vector3(0,0,Vector3.Distance(_harpoonGun._projectileSpots[0].UpgradeTierSpots[0].transform.position, _lr.transform.parent.transform.position)));
    }

    public override void Update()
    {
        //CurrentLifeTime += Time.deltaTime;
        //CheckLifetime();
    }
    public override void OnEnable()
    {
        // Physics.IgnoreCollision(col, REF.PCon.playerCol);
        // CurrentLifeTime = 0;
        // despawnAnimationPlaying = false;
        // hasDoneDamage = false;
    }
    public override void OnTriggerEnter(Collider col)
    {
        if (!HasDoneDamage)
        {
            if (!HitPlayer)
            {
                if (col.GetComponentInChildren<AEnemy>())
                {
                    StickInEnemy(col.GetComponentInChildren<AEnemy>());
                }
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
        HasDoneDamage = true;
    }
    public void Unstick()
    {
        _projectileRB.isKinematic = false;
        _stuckEnemy.TakeDamage(Damage);
        _stuck = false;
        _stuckEnemy = null;
    }
    private void CheckHarpoonStuckInDeadEnemy(AEnemy enemy)
    {
        // print("enemy killed: " + enemy);
        //print("enemy stuck: " + _stuckEnemy);
        if (_stuckEnemy)
            if (enemy.Equals(_stuckEnemy))
            {
                HasDoneDamage = false;
                _harpoonGun.ForceRetract();
            }
    }
}
