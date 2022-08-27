using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceMusket : AWeapon
{
    public override void Awake()
    {
        InitSystemStats();
        ProjectilePrefab = (GameObject)Resources.Load("MusketBall");
        ChargeBegun = false;
        ChargeTime = 0;
        ChargeLevel = 1;
        FullChargeTime = 2f;

        WeaponLevel = 3;
        MaxLevel = 3;
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
        }
        else //Fire one bullet by default
        {
            GameObject proj = ProjectilePool.Instance.GetProjectileFromPool(ProjectilePrefab.tag);
            proj.GetComponent<AProjectile>().SetBulletStatsAndTransformToWeaponStats(this, _projectileSpots[0]);
            proj.SetActive(true);
        }

        ChargeLevel = 1;
    }

    public void SubtractAmmo(int amount)
    {
        AmmoLeft -= amount;
        ShouldRegenerateAmmo = true;
    }

    public void RegenerateAmmo(int amount)
    {
        AmmoLeft += amount;
        if(AmmoLeft >= MagazineSize)
        {
            ShouldRegenerateAmmo = false;
        }
    }
}
