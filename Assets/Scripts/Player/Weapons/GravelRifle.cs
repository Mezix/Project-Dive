using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GravelRifle : AWeapon
{
    private Animator gravelRifleAnimator;
    public ParticleSystem gravelRifleFiredParticleSystem;

    //  Weapon UI
    public Text _magazineSize;
    public Text _ammoLeft;
    public Text _totalAmmo;

    public override void Awake()
    {
        InitSystemStats();
        gravelRifleAnimator = GetComponentInChildren<Animator>();
        ProjectilePrefab = (GameObject)Resources.Load("Weapons/Gravel Projectile");
        ChargeBegun = false;
        ChargeTime = 0;

        MagazineSize = 30;
        AmmoLeft = 30;
        ShouldRegenerateAmmo = false;
        TimeBetweenAmmoRegeneration = 2;
        AmmoRegenAmount = MagazineSize;

        WeaponLevel = 1;
        MaxLevel = 3;
        gravelRifleFiredParticleSystem.gameObject.SetActive(false);
    }
    public void Start()
    {
        SetUpgradeLevel(WeaponLevel);
        UpdateAmmoDisplay();
    }
    public override void Update()
    {
        base.Update();
        UpdateAmmoDisplay();

        if (Input.GetKeyDown(KeyCode.R) && !Reloading && AmmoLeft < MagazineSize)
        {
            InitiateReload();
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

    private void InitiateReload()
    {
        gravelRifleAnimator.SetFloat("ReloadMultiplier", 1 / TimeBetweenAmmoRegeneration);
        gravelRifleAnimator.SetBool("Firing", false);
        Reloading = true;
        gravelRifleAnimator.SetTrigger("ReloadInitiated");
    }

    public override void TryFire()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (TimeElapsedBetweenLastAttack >= TimeBetweenAttacks && !Reloading)
            {
                if (AmmoLeft > 0)
                {
                    gravelRifleAnimator.SetBool("Firing", true);
                    _weaponFireSFX.Play();
                    SpawnProjectile();
                    gravelRifleFiredParticleSystem.Play();
                    REF.CamScript.StartShake(RecoilDuration, Recoil);
                }
            }
        }
        else
        {
            gravelRifleAnimator.SetBool("Firing", false);
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
        gravelRifleAnimator.SetFloat("FireRate", AttacksPerSecond);
        TimeElapsedBetweenLastAttack = 0;
        TimeElapsedBetweenAmmoRegeneration = 0;

        if (REF.PCon._weaponDirection == 1) REF.PCon.ApplyKnockback(KnockbackForce * bulletsFired);
        else                                REF.PCon.ApplyKnockback(KnockbackForce * bulletsFired * BackwardsKnockbackModifier);
    }

    public void SubtractAmmo(int amount)
    {
        AmmoLeft -= amount;
        Reloading = false;
        if (AmmoLeft <= 0) InitiateReload();
    }

    public void RegenerateAmmo(int amount)
    {
        AmmoLeft += amount;
        if (AmmoLeft >= MagazineSize)
        {
            AmmoLeft = MagazineSize;
            Reloading = false;
            TimeElapsedBetweenAmmoRegeneration = 0;
        }
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

    public void UpdateAmmoDisplay()
    {
        _magazineSize.text = "/" + MagazineSize.ToString();
        _totalAmmo.text = "Inf";
        _ammoLeft.text = AmmoLeft.ToString();
    }
}
