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
            UpdateLineRenderer();
            _lr.gameObject.SetActive(true);
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
        if (enemy._enemyDead) _harpoonGun.ForceRetract();
        HasDoneDamage = true;
        if(enemy.Weight < 10) StartCoroutine(PullEnemyToPlayer(enemy, true));
        else StartCoroutine(PullEnemyToPlayer(enemy, false));
    }

    private IEnumerator PullEnemyToPlayer(AEnemy enemy, bool pullObjectToPlayer)
    {
        float _timePulled = 0;
        float _maxPullTime = 2;
        float _pullForce = 500;
        float breakDistance = 20f;
        float maxVelocity = 50f;
        float maxPlayerVelocity = 70f;
        if (pullObjectToPlayer) // Pull enemy towards you
        {
            while(enemy && _timePulled < _maxPullTime && Vector3.Distance(enemy.transform.position, REF.PCon.transform.position) > breakDistance)
            {
                _timePulled += Time.deltaTime;
                if(enemy._enemyRB.velocity.magnitude <= maxVelocity)
                    enemy._enemyRB.AddForce((REF.PCon.transform.position - enemy.transform.position).normalized * _pullForce);
                yield return new WaitForFixedUpdate();
            }
        }
        else // Pull you towards enemy
        {
            while (enemy && _timePulled < _maxPullTime && Vector3.Distance(enemy.transform.position, REF.PCon.transform.position) > breakDistance)
            {
                _timePulled += Time.deltaTime;
                if (REF.PCon.playerRB.velocity.magnitude <= maxPlayerVelocity)
                    REF.PCon.playerRB.AddForce((enemy.transform.position - REF.PCon.transform.position).normalized * _pullForce);
                yield return new WaitForFixedUpdate();
            }
        }
        _harpoonGun.ForceRetract();
        yield return null;
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
    private void CheckHarpoonStuckInDeadEnemy(AEnemy enemy)
    {
        //  TODO: move this to enemy, since we can stick a dead enemy and then our harpoon will be deleted!

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
