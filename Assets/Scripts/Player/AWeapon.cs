using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AWeapon : MonoBehaviour
{
    public WeaponStats _weaponStats;
    public float AttacksPerSecond { get; set; }
    public float TimeBetweenAttacks { get; private set; }
    public float TimeElapsedBetweenLastAttack { get; protected set; }

    public int MagazineSize { get; set; }
    public int AmmoLeft { get; set; }
    public bool ShouldRegenerateAmmo { get; set; }
    public float RegenSpeed { get; set; }
    public float TimeBetweenAmmoRegeneration { get; set; }
    public float TimeElapsedBetweenAmmoRegeneration { get; set; }
    public int AmmoRegenAmount { get; set; }
    public int Damage { get; set; }
    public float Recoil { get; set; }
    public float RecoilDuration { get; set; }
    public float _knockbackForce { get; set; }
    public AudioSource _weaponFireSFX;
    public GameObject ProjectilePrefab { get; set; }
    //public List<Transform> _projectileSpots = new List<Transform>();
    public List<ProjectileSpots> _projectileSpots;
    [Serializable]
    public class ProjectileSpots
    {
        public List<Transform> UpgradeTierSpots;
    }
    [Serializable]
    public class UpgradeObjectList
    {
        public List<GameObject> UpgradeTier;
    }

    public List<UpgradeObjectList> _upgradeObjects;
    //  Charging Shots
    public bool ChargeBegun { get; set; }
    public bool Charging { get; set; }
    public float ChargeTime { get; set; }
    public int ChargeLevel { get; set; }
    public float FullChargeTime { get; set; }
    public int WeaponLevel { get; set; }
    public int MaxLevel { get; set; }

    public virtual void Awake()
    {
        InitSystemStats();
        ProjectilePrefab = (GameObject) Resources.Load("MusketBall");
    }
    public virtual void Update()
    {
        TimeElapsedBetweenLastAttack += Time.deltaTime;
    }
    public void InitSystemStats()
    {
        if (_weaponStats)  //if we have a scriptableobject, use its stats
        {
            AttacksPerSecond = _weaponStats._attacksPerSecond;
            Damage = _weaponStats._damage;
            Recoil = _weaponStats._recoil;
            RecoilDuration = _weaponStats._recoilDuration;
            _knockbackForce = _weaponStats._knockbackForce;
        }
        else  //set default stats
        {
            print("No Weapon Stats found, setting defaults!");
            Damage = 1;
            AttacksPerSecond = 1;
            Recoil = 0.05f;
            RecoilDuration = 0.05f;
            _knockbackForce = 100;
        }
        TimeBetweenAttacks = 1 / AttacksPerSecond;
        TimeElapsedBetweenLastAttack = TimeBetweenAttacks; //make sure we can fire right away
    }
    public virtual void TryFire()
    {
        if (TimeElapsedBetweenLastAttack >= TimeBetweenAttacks)
        {
            _weaponFireSFX.Play();
            SpawnProjectile();
            REF.PCon.ApplyKnockback(_knockbackForce);
        }
    }
    public virtual void SpawnProjectile()
    {
        foreach (Transform t in _projectileSpots[0].UpgradeTierSpots)
        {
            GameObject proj = ProjectilePool.Instance.GetProjectileFromPool(ProjectilePrefab.tag);
            proj.GetComponent<AProjectile>().SetBulletStatsAndTransformToWeaponStats(this, t);
            proj.SetActive(true);
            TimeElapsedBetweenLastAttack = 0;
        }
    }}
