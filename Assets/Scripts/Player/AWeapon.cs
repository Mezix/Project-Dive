using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AWeapon : MonoBehaviour
{
    public WeaponStats _weaponStats;
    public float AttacksPerSecond { get; set; }
    public float TimeBetweenAttacks { get; private set; }
    public float TimeElapsedBetweenLastAttack { get; protected set; }
    public int Damage { get; set; }
    public float Recoil { get; set; }
    public float RecoilDuration { get; set; }
    public float _knockbackForce { get; set; }
    public AudioSource _weaponFireSFX;
    public GameObject ProjectilePrefab { get; set; }
    public List<Transform> _projectileSpots = new List<Transform>();

    private void Awake()
    {
        InitSystemStats();
        ProjectilePrefab = (GameObject) Resources.Load("MusketBall");
    }
    private void Update()
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
    public bool Fire()
    {
        if (TimeElapsedBetweenLastAttack >= TimeBetweenAttacks)
        {
            _weaponFireSFX.Play();
            SpawnProjectile();
            
            return true;
        }
        return false;
    }
    public void SpawnProjectile()
    {
        foreach (Transform t in _projectileSpots)
        {
            GameObject proj = ProjectilePool.Instance.GetProjectileFromPool(ProjectilePrefab.tag);
            proj.GetComponent<AProjectile>().SetBulletStatsAndTransformToWeaponStats(this, t);
            proj.SetActive(true);
            TimeElapsedBetweenLastAttack = 0;
        }
    }
    
}
