using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AProjectile : MonoBehaviour //the interface for all projectiles fired from ranged Weapons
{
    public float CurrentLifeTime { get; protected set; } //Check IProjectile for explanations
    public float MaxLifetime { get; set; }
    public int Damage { get; set; }
    public float ProjectileSpeed { get; set; }
    public bool HitPlayer { get; set; }

    public bool HasDoneDamage { get; set; }

    //  Status Effects

    public bool _shouldFreeze;
    public int _freezeStacksAppliedOnHit;

    public bool _shouldStun;
    public float _stunDuration;

    public bool _shouldBleed;
    public int _bleedDamage;
    public float _bleedDuration;
    public float _timeBetweenBleedTicks;
    public float _timeSinceLastBleedTick;

    //  Misc

    [HideInInspector] public Rigidbody _projectileRB;
    [HideInInspector] public Collider _projectileCollider;
    [HideInInspector] public bool _despawnAnimationPlaying;

    [HideInInspector] public TrailRenderer _trailRenderer;
    [HideInInspector] public AWeapon _weaponFromWhichProjectileWasFired;

    // AUDIO

    public string HitTargetEventID;

    public virtual void Awake()
    {
        _projectileRB = GetComponentInChildren<Rigidbody>();
        _projectileCollider = GetComponentInChildren<Collider>();
        _trailRenderer = GetComponentInChildren<TrailRenderer>();
        MaxLifetime = 3;
    }
    public virtual void Start()
    {

    }
    public virtual void FixedUpdate()
    {
        if (!_despawnAnimationPlaying) MoveProjectile();
    }
    public virtual void OnEnable()
    {
        CurrentLifeTime = 0;
        _despawnAnimationPlaying = false;
        HasDoneDamage = false;
    }
    public virtual void Update()
    {
        CurrentLifeTime += Time.deltaTime;
        CheckLifetime();
    }
    public virtual void MoveProjectile()
    {
        _projectileRB.MovePosition(transform.position + transform.forward * ProjectileSpeed * Time.deltaTime);
    }
    protected void CheckLifetime() //a function that checks if our projectile has reached the end of its lifespan, and then decides what to do now
    {
        if (CurrentLifeTime >= MaxLifetime && !_despawnAnimationPlaying)
        {
            StartCoroutine(DespawnAnimation());
        }
    }
    protected void DespawnBullet()
    {
        ProjectilePool.Instance.AddToPool(gameObject);
    }
    public virtual void SetBulletStatsAndTransformToWeaponStats(AWeapon weapon, Transform t)
    {
        HitPlayer = false;
        _weaponFromWhichProjectileWasFired = weapon;
        Damage = weapon._weaponStats._damage;
        ProjectileSpeed = weapon._weaponStats._projectileSpeed;
        //if (REF.PCon) ProjectileSpeed += REF.PCon.playerRB.velocity.magnitude;
        transform.position = t.transform.position;
        transform.rotation = t.transform.rotation;
        if (_trailRenderer) _trailRenderer.Clear();
    }
    public virtual IEnumerator DespawnAnimation()
    {
        _despawnAnimationPlaying = true;
        yield return new WaitForSeconds(0f);
        DespawnBullet();
    }
    public virtual void OnTriggerEnter(Collider col)
    {
        if (!HasDoneDamage)
        {
            if (HitPlayer)
            {
                if (!(col.GetComponentInChildren<AEnemy>() 
                    || col.GetComponentInChildren<AProjectile>()
                    || col.GetComponentInChildren<SoundtrackChangerCollider>()
                    || col.GetComponentInChildren<BossManager>())) // dont hit ourselves
                {
                    StartCoroutine(DespawnAnimation());
                }
                if (col.GetComponent<PlayerController>())
                {
                    REF.PCon._pHealth.TakeDamage(Damage);
                    HasDoneDamage = true;
                }
            }
            else // not supposed to hit player
            {
                if (!(col.GetComponent<PlayerController>() 
                    || col.GetComponentInChildren<AProjectile>()
                    || col.GetComponentInChildren<SoundtrackChangerCollider>()
                    || col.GetComponentInChildren<BossManager>())) // dont hit ourselves
                {
                    StartCoroutine(DespawnAnimation());
                    //Debug.Log(col.name);
                }
                if (col.GetComponent<AEnemy>())
                {
                    AEnemy enemy = col.GetComponentInChildren<AEnemy>();
                    if(_shouldFreeze) enemy.AddFreezeStack(_freezeStacksAppliedOnHit);
                    enemy.TakeDamage(Damage);
                    HasDoneDamage = true;
                    AkSoundEngine.PostEvent(HitTargetEventID, gameObject);
                   // StartCoroutine(DespawnAnimation());
                }
            }
        }
    }
}
