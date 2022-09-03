using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pufferfish : AEnemy
{
    private GameObject _projectilePrefab;
    private int _damage;
    private float _projectileSpeed;
    public override void Awake()
    {
        base.Awake();
        _projectilePrefab = (GameObject) Resources.Load("Weapons/Pufferfish Shot");
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
        if(!_frozen)
        {
            RotateTowardsPlayer();
            Fire();
        }
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
        foreach(Transform spot in _projectileSpots)
        {
            GameObject proj = ProjectilePool.Instance.GetProjectileFromPool(_projectilePrefab.tag);
            AProjectile projectile = proj.GetComponent<AProjectile>();

            projectile.Damage = _damage;
            projectile.ProjectileSpeed = _projectileSpeed;
            projectile.transform.position = spot.position;
            projectile.transform.rotation = spot.rotation;
            projectile.HitPlayer = true;
            if (projectile.trailRenderer) projectile.trailRenderer.Clear();
            proj.SetActive(true);
            TimeElapsedBetweenLastAttack = 0;
        }

    }
}
