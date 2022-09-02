using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pufferfish : AEnemy
{
    public Transform _projectileSpot;
    private GameObject _projectilePrefab;
    private int _damage;
    private float _projectileSpeed;
    private void Awake()
    {
        _projectilePrefab = (GameObject) Resources.Load("Pufferfish Shot");
    }
    void Start()
    {
        AttacksPerSecond = 1f;
        _damage = 10;
        _projectileSpeed = 100f;
        InitStats();
    }
    void Update()
    {
        RotateTowardsPlayer();
        Fire();
        //EnemyBehaviour();
        TimeElapsedBetweenLastAttack += Time.deltaTime;
    }

    private void EnemyBehaviour()
    {
        throw new NotImplementedException();
    }

    public void RotateTowardsPlayer()
    {
        transform.LookAt(REF.PCon.transform);
    }
    public bool Fire()
    {
        if (TimeElapsedBetweenLastAttack >= TimeBetweenAttacks)
        {
            SpawnProjectile();

            return true;
        }
        return false;
    }
    public void SpawnProjectile()
    {
        GameObject proj = ProjectilePool.Instance.GetProjectileFromPool(_projectilePrefab.tag);
        AProjectile projectile = proj.GetComponent<AProjectile>();

        projectile.Damage = _damage;
        projectile.ProjectileSpeed = _projectileSpeed;
        projectile.transform.position = _projectileSpot.position;
        projectile.transform.rotation = _projectileSpot.rotation;
        projectile.HitPlayer = true;
        if (projectile.trailRenderer) projectile.trailRenderer.Clear();

        proj.SetActive(true);
        TimeElapsedBetweenLastAttack = 0;
    }
}
