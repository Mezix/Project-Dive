using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GravelRifle : AWeapon
{
    public List<GameObject> _shotsReady;
    private Animator gravelRifleAnimator;
    public ParticleSystem gravelRifleFiredParticleSystem;

    public override void Awake()
    {
        InitSystemStats();
        gravelRifleAnimator = GetComponentInChildren<Animator>();
        ProjectilePrefab = (GameObject)Resources.Load("Weapons/Gravel Projectile");
        //ChargeBegun = false;
        //ChargeTime = 0;
        //ChargeLevel = 1;
        //FullChargeTime = 2f;

        MagazineSize = 30;
        AmmoLeft = 30;
        ShouldRegenerateAmmo = false;
        TimeBetweenAmmoRegeneration = 2;
        AmmoRegenAmount = MagazineSize;

        WeaponLevel = 1;
        //MaxLevel = 3;
        //gravelRifleFiredParticleSystem.gameObject.SetActive(false);
    }
    public void Start()
    {
        SetUpgradeLevel(WeaponLevel);
    }
    public override void Update()
    {
        base.Update();
        //TryFire();

        if(Input.GetKeyDown(KeyCode.R) && !Reloading && AmmoLeft < MagazineSize)
        {
            Reloading = true;
            gravelRifleAnimator.SetFloat("ReloadMultiplier", 1/TimeBetweenAmmoRegeneration);
            gravelRifleAnimator.SetTrigger("ReloadInitiated");
        }
        if (Reloading)
        {
            TimeElapsedBetweenAmmoRegeneration += Time.deltaTime;
            if (TimeElapsedBetweenAmmoRegeneration >= TimeBetweenAmmoRegeneration)
            {
                RegenerateAmmo(AmmoRegenAmount);
            }
        }
    }
    public override void TryFire()
    {
        if (TimeElapsedBetweenLastAttack >= TimeBetweenAttacks && !Reloading)
        {
            if (AmmoLeft > 0)
            {
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    //gravelRifleFiredSFX.SetActive(Charging);
                    _weaponFireSFX.Play();
                    SpawnProjectile();
                    gravelRifleFiredParticleSystem.Play();
                    REF.CamScript.StartShake(RecoilDuration, Recoil);
                }
            }
        }
    }
    public override void SpawnProjectile()
    {
        int bulletsFired = 0;
        foreach (Transform t in _projectileSpots[0].UpgradeTierSpots)
        {
            bulletsFired++;
            GameObject proj = ProjectilePool.Instance.GetProjectileFromPool(ProjectilePrefab.tag);
            proj.GetComponent<AProjectile>().SetBulletStatsAndTransformToWeaponStats(this, t);
            proj.SetActive(true);
        }
        bulletsFired = Mathf.Min(MagazineSize, bulletsFired);
        SubtractAmmo(bulletsFired);
        gravelRifleAnimator.SetFloat("ReloadMultiplier", AttacksPerSecond);
        gravelRifleAnimator.SetTrigger("Fired");
         TimeElapsedBetweenLastAttack = 0;
        TimeElapsedBetweenAmmoRegeneration = 0;
        REF.PCon.ApplyKnockback(_knockbackForce * bulletsFired);
    }

    public void SubtractAmmo(int amount)
    {
        AmmoLeft -= amount;
        UpdateAmmoDisplay(AmmoLeft);
        Reloading = false;
    }

    public void RegenerateAmmo(int amount)
    {
        AmmoLeft += amount;
        if (AmmoLeft >= MagazineSize)
        {
            Reloading = false;
            TimeElapsedBetweenAmmoRegeneration = 0;
        }
        UpdateAmmoDisplay(AmmoLeft);
    }

    public void SetUpgradeLevel(int lvl)
    {
        foreach (UpgradeObjectList list in _upgradeObjects)
        {
            //foreach (GameObject g in list.UpgradeTier) g.SetActive(false);
        }
        for (int i = 0; i < lvl; i++)
        {
            if (i >= _upgradeObjects.Count) return;
            //foreach (GameObject g in _upgradeObjects[i].UpgradeTier) g.SetActive(true);
        }
    }

    public void UpdateAmmoDisplay(int ammoRemaining)
    {
        for (int i = 0; i < ammoRemaining; i++)
        {
            //_shotsReady[i].SetActive(true);
        }
        for (int i = ammoRemaining; i < MagazineSize; i++)
        {
           // _shotsReady[i].SetActive(false);
        }
    }
}
