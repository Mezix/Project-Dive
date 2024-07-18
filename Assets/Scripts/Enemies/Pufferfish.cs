using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pufferfish : AEnemy
{
    private GameObject _projectilePrefab;
    private int _damage;
    private float _projectileSpeed;
    private float VisionRange = 150;
    bool playerHasBeenSpotted = false;
    public override void Awake()
    {
        base.Awake();
        _projectilePrefab = (GameObject)Resources.Load("Weapons/Pufferfish Shot");
    }
    public override void Start()
    {
        base.Start();
        AttacksPerSecond = 1f;
        _damage = 10;
        _projectileSpeed = 100f;
        InitStats();
    }
    public override void Update()
    {
        base.Update();
        EnemyBehaviour();
        TimeElapsedBetweenLastAttack += Time.deltaTime;
    }

    private void EnemyBehaviour()
    {
        if (!_frozen && !_enemyDead)
        {
            if (playerHasBeenSpotted)
            {
                MoveTowardsPlayer();
            }
            if (CanSeePlayer())
            {
                RotateTowardsPlayer();
                Fire();
            }
        }
        else
        {
            _enemyAnimator.speed = 0;
        }
    }

    private void MoveTowardsPlayer()
    {
        if (_enemyRB.velocity.magnitude < 10)
        {
            Vector3 moveDir = (REF.PCon.transform.position - _projectileSpots[0].position).normalized;
            _enemyRB.AddForce(moveDir * 50);
        }
    }

    public bool CanSeePlayer()
    {
        RaycastHit hit;
        if (Physics.Raycast(_projectileSpots[0].position, (REF.PCon.transform.position - _projectileSpots[0].position).normalized, out hit, VisionRange))
        {
            if (hit.transform.GetComponent<PlayerController>())
            {
                if (!playerHasBeenSpotted) SpotPlayerFirstTime();
                return true;
            }
        }
        else
        {
            //Debug.Log("nope");
        }
        return false;
    }

    private void SpotPlayerFirstTime()
    {
        Debug.Log("Player spotted for the first time!");
        playerHasBeenSpotted = true;
    }

    public void RotateTowardsPlayer()
    {
        transform.LookAt(REF.PCon.transform);
    }
    public bool Fire()
    {
        if (TimeElapsedBetweenLastAttack >= TimeBetweenAttacks)
        {
            SpawnProjectiles();
            return true;
        }
        return false;
    }
    public void SpawnProjectiles()
    {
        foreach (Transform spot in _projectileSpots)
        {
            GameObject proj = ProjectilePool.Instance.GetProjectileFromPool(_projectilePrefab.tag);
            AProjectile projectile = proj.GetComponent<AProjectile>();

            projectile.Damage = _damage;
            projectile.ProjectileSpeed = _projectileSpeed;
            projectile.transform.position = spot.position;
            projectile.transform.rotation = spot.rotation;
            projectile.HitPlayer = true;
            if (projectile._trailRenderer) projectile._trailRenderer.Clear();
            proj.SetActive(true);
            TimeElapsedBetweenLastAttack = 0;
        }

    }
}
