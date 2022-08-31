using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class IceMusket : AWeapon
{
    public List<GameObject> _shotsReady;
    private Animator iceMusketAnimator;
    public List<UpgradeObjectList> _upgradeObjects;

    [Serializable]
    public class UpgradeObjectList
    {
        public List<GameObject> UpgradeTier;
    }

    public override void Awake()
    {
        InitSystemStats();
        iceMusketAnimator = GetComponentInChildren<Animator>();
        ProjectilePrefab = (GameObject) Resources.Load("Ice Ball");
        ChargeBegun = false;
        ChargeTime = 0;
        ChargeLevel = 1;
        FullChargeTime = 2f;

        MagazineSize = 3;
        AmmoLeft = 3;
        ShouldRegenerateAmmo = false;
        TimeBetweenAmmoRegeneration = 1;
        AmmoRegenAmount = 1;

        WeaponLevel = 3;
        MaxLevel = 3;
    }
    public void Start()
    {
        SetUpgradeLevel(WeaponLevel);
    }
    public override void Update()
    {
        base.Update();
        TryFire();

        if(ShouldRegenerateAmmo)
        {
            TimeElapsedBetweenAmmoRegeneration += Time.deltaTime;
            if(TimeElapsedBetweenAmmoRegeneration >= TimeBetweenAmmoRegeneration)
            {
                RegenerateAmmo(AmmoRegenAmount);
            }
        }
        if (ChargeBegun)
        {
            ChargeTime += Time.deltaTime;
            ChargeLevel = Mathf.Max(1, Mathf.RoundToInt(Mathf.Min(ChargeTime, FullChargeTime)/FullChargeTime * WeaponLevel));
        }
    }
    public override void TryFire()
    {
        if (TimeElapsedBetweenLastAttack >= TimeBetweenAttacks)
        {
            if(AmmoLeft > 0)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    ChargeTime = 0; //reset charge time for the first frame
                    ChargeBegun = true;
                }
                Charging = Input.GetKey(KeyCode.Mouse0);  //keep holding the charge as long as we press charge down

                if (ChargeBegun && !Charging) //once we let go, fire!
                {
                    ChargeBegun = false;
                    _weaponFireSFX.Play();
                    SpawnProjectile();
                    iceMusketAnimator.SetFloat("ReloadMultiplier", AttacksPerSecond);
                    iceMusketAnimator.SetTrigger("ReloadInitiated");
                    REF.CamScript.StartShake(RecoilDuration, Recoil);
                }
            }
        }
    }
    public override void SpawnProjectile()
    {
        if (ChargeLevel == 2)
        {
            GameObject proj = ProjectilePool.Instance.GetProjectileFromPool(ProjectilePrefab.tag);
            proj.GetComponent<AProjectile>().SetBulletStatsAndTransformToWeaponStats(this, _projectileSpots[1]);
            proj.SetActive(true);
            proj = ProjectilePool.Instance.GetProjectileFromPool(ProjectilePrefab.tag);
            proj.GetComponent<AProjectile>().SetBulletStatsAndTransformToWeaponStats(this, _projectileSpots[2]);
            proj.SetActive(true);
            SubtractAmmo(2);
            REF.PCon.ApplyKnockback(_knockbackForce * 2);
        }
        else if (ChargeLevel == 3)
        {
            GameObject proj = ProjectilePool.Instance.GetProjectileFromPool(ProjectilePrefab.tag);
            proj.GetComponent<AProjectile>().SetBulletStatsAndTransformToWeaponStats(this, _projectileSpots[0]);
            proj.SetActive(true);
            proj = ProjectilePool.Instance.GetProjectileFromPool(ProjectilePrefab.tag);
            proj.GetComponent<AProjectile>().SetBulletStatsAndTransformToWeaponStats(this, _projectileSpots[3]);
            proj.SetActive(true);
            proj = ProjectilePool.Instance.GetProjectileFromPool(ProjectilePrefab.tag);
            proj.GetComponent<AProjectile>().SetBulletStatsAndTransformToWeaponStats(this, _projectileSpots[4]);
            proj.SetActive(true);
            proj = ProjectilePool.Instance.GetProjectileFromPool(ProjectilePrefab.tag);
            proj.GetComponent<AProjectile>().SetBulletStatsAndTransformToWeaponStats(this, _projectileSpots[5]);
            proj.SetActive(true);
            SubtractAmmo(3);
            REF.PCon.ApplyKnockback(_knockbackForce * 3);
        }
        else //Fire one bullet by default
        {
            GameObject proj = ProjectilePool.Instance.GetProjectileFromPool(ProjectilePrefab.tag);
            proj.GetComponent<AProjectile>().SetBulletStatsAndTransformToWeaponStats(this, _projectileSpots[0]);
            proj.SetActive(true);
            SubtractAmmo(1);
            REF.PCon.ApplyKnockback(_knockbackForce);
        }

        ChargeLevel = 1;
        TimeElapsedBetweenLastAttack = 0;
    }

    public void SubtractAmmo(int amount)
    {
        AmmoLeft -= amount;
        ShouldRegenerateAmmo = true;
        UpdateAmmoDisplay();
    }

    public void RegenerateAmmo(int amount)
    {
        AmmoLeft += amount;
        if(AmmoLeft >= MagazineSize)
        {
            ShouldRegenerateAmmo = false;
            TimeElapsedBetweenAmmoRegeneration = 0;
        }
        UpdateAmmoDisplay();
    }

    public void SetUpgradeLevel(int lvl)
    {
        foreach (UpgradeObjectList list in _upgradeObjects)
        {
            foreach (GameObject g in list.UpgradeTier) g.SetActive(false);
        }
        for(int i = 0; i < lvl; i++)
        {
            if (i >= _upgradeObjects.Count) return;
            foreach (GameObject g in _upgradeObjects[i].UpgradeTier) g.SetActive(true);
        }
    }

    public void UpdateAmmoDisplay()
    {
        for(int i = 0; i < AmmoLeft; i++)
        {
            _shotsReady[i].SetActive(true);
        }
        for (int i = AmmoLeft; i < MagazineSize; i++)
        {
            _shotsReady[i].SetActive(false);
        }
    }
}
