using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pufferfish : AEnemy
{
    private GameObject _projectilePrefab;
    private int _damage;
    private float _projectileSpeed;
    private float VisionRange = 10;
    public enemyState eState = enemyState.Patrolling;

    public List<Transform> patrolPositions;
    public int posIndex = 0;
    public enum enemyState
    {
        Patrolling, 
        FoundPlayer
    }
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
            if (eState == enemyState.FoundPlayer)
            {
                MoveTowardsPlayer();
                RotateTowardsPlayer();
                if (CanSeePlayer())
                {
                    Fire();
                }
            }
            else if(eState == enemyState.Patrolling)
            {
                GoToNextPosition();
                CanSeePlayer();
            }
        }
        else
        {
            _enemyAnimator.speed = 0;
        }
    }

    private void GoToNextPosition()
    {
        if(patrolPositions.Count == 0)
        {
            return;
        }
        if (_enemyRB.velocity.magnitude < 10)
        {
            Vector3 moveDir = (patrolPositions[posIndex].transform.position - transform.position).normalized;
            _enemyRB.AddForce(moveDir * 5);
        }
        transform.LookAt(transform.position + _enemyRB.velocity);
        if (Vector3.Distance(patrolPositions[posIndex].transform.position, transform.position) < 10)
        {
            posIndex++;
            if(posIndex >= patrolPositions.Count)
            {
                posIndex = 0;
            }
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
        int layerMask = ~(1 << 2 | 1 << 8);
        if (Physics.Raycast(_projectileSpots[0].position, (REF.PCon.transform.position - _projectileSpots[0].position).normalized, out hit, VisionRange, layerMask))
        {
            if (hit.transform.GetComponent<PlayerController>())
            {
                if (eState == enemyState.Patrolling) SpotPlayerFirstTime();
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
        eState = enemyState.FoundPlayer;
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
