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

    private Rigidbody projectileRB;
    private Collider projectileCollider;
    protected bool despawnAnimationPlaying;
    protected bool hasDoneDamage;

    [HideInInspector] public TrailRenderer trailRenderer;
    [HideInInspector] public AWeapon _weaponFromWhichProjectileWasFired;

    private void Awake()
    {
        projectileRB = GetComponentInChildren<Rigidbody>();
        projectileCollider = GetComponentInChildren<Collider>();
        trailRenderer = GetComponentInChildren<TrailRenderer>();
        MaxLifetime = 3;
    }
    private void OnEnable()
    {
        // Physics.IgnoreCollision(col, REF.PCon.playerCol);
        CurrentLifeTime = 0;
        despawnAnimationPlaying = false;
        hasDoneDamage = false;
    }
    private void Update()
    {
        CurrentLifeTime += Time.deltaTime;
        CheckLifetime();
    }
    public virtual void MoveProjectile()
    {
        projectileRB.MovePosition(transform.position + transform.forward * ProjectileSpeed * Time.deltaTime);
    }
    protected void CheckLifetime() //a function that checks if our projectile has reached the end of its lifespan, and then decides what to do now
    {
        if (CurrentLifeTime >= MaxLifetime && !despawnAnimationPlaying)
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
        if (trailRenderer) trailRenderer.Clear();
    }
    public virtual IEnumerator DespawnAnimation()
    {
        despawnAnimationPlaying = true;
        yield return new WaitForSeconds(0f);
        DespawnBullet();
    }
    private void OnTriggerEnter(Collider col)
    {
        if (!hasDoneDamage)
        {
            if (HitPlayer)
            {
                if (col.GetComponent<PlayerController>())
                {
                    REF.PCon._pHealth.TakeDamage(Damage);
                    hasDoneDamage = true;
                    StartCoroutine(DespawnAnimation());
                }
            }
            else
            {
                if (col.GetComponentInChildren<AEnemy>())
                {
                    col.GetComponentInChildren<AEnemy>().TakeDamage(Damage);
                    hasDoneDamage = true;
                    StartCoroutine(DespawnAnimation());
                }
            }
        }
    }
}
