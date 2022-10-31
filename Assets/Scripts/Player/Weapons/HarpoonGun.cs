using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HarpoonGun : AWeapon
{
    public List<GameObject> _shotsReady;
    private Animator harpoonAnimator;
    public ParticleSystem harpoonFiredParticleSystem;
    public bool _harpoonFired;
    public HarpoonProjectile _harpoonProjectile;

    //  Weapon UI
    public Text _magazineSize;
    public Text _ammoLeft;

    public override void Awake()
    {
        InitSystemStats();
        harpoonAnimator = GetComponentInChildren<Animator>();
        ProjectilePrefab = (GameObject) Resources.Load("Weapons/Gravel Projectile");

        ShouldRegenerateAmmo = false;
        TimeBetweenAmmoRegeneration = 2;
        AmmoLeft = MagazineSize;
        WeaponLevel = 1;
        _harpoonProjectile._harpoonGun = this;
    }
    public void Start()
    {
        SetUpgradeLevel(WeaponLevel);
    }
    public override void Update()
    {
        base.Update();
        UpdateAmmoDisplay();
        if (Input.GetKeyDown(KeyCode.R) && !Reloading)
        {
            RetractHarpoon();
        }
    }

    public override void TryFire()
    {
        if(!_harpoonFired)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (TimeElapsedBetweenLastAttack >= TimeBetweenAttacks && !Reloading)
                {
                    FireHarpoon();
                }
            }
        }
    }

    private void FireHarpoon()
    {
        AmmoLeft = 0;
        _harpoonFired = true;
        harpoonAnimator.SetTrigger("Fired");
        _weaponFireSFX.Play();
        _harpoonProjectile._projectileRB.isKinematic = false;
        _harpoonProjectile._trailRenderer.Clear();

        _harpoonProjectile.SetBulletStatsAndTransformToWeaponStats(this, _projectileSpots[0].UpgradeTierSpots[0]);
        _harpoonProjectile.transform.parent = null;
        _harpoonProjectile._launched = true;
        _harpoonProjectile._stuck = false;
        TimeElapsedBetweenLastAttack = 0;
        harpoonFiredParticleSystem.Play();
        REF.PCon.ApplyKnockback(KnockbackForce);
        REF.CamScript.StartShake(RecoilDuration, Recoil);
    }

    public void ForceRetract()
    {
        RetractHarpoon();
    }
    private void RetractHarpoon()
    {
        AmmoLeft = MagazineSize;
        if (_harpoonProjectile._stuckEnemy) _harpoonProjectile.Unstick();

        harpoonAnimator.SetBool("Reloading", true);
        Reloading = true;
        _harpoonFired = false;
        _harpoonProjectile.HasDoneDamage = false;
        _harpoonProjectile._launched = false;
        _harpoonProjectile._stuck = false;
        _harpoonProjectile._projectileRB.isKinematic = true;
        _harpoonProjectile._trailRenderer.Clear();

        _harpoonProjectile.transform.SetParent(_projectileSpots[0].UpgradeTierSpots[0].transform, false);
        _harpoonProjectile.transform.localPosition = Vector3.zero;
        _harpoonProjectile.transform.localScale = Vector3.one;
        HM.RotateLocalTransformToAngle(_harpoonProjectile.transform, Vector3.zero);
        // harpoonAnimator.SetBool("Reloading", false);
        Reloading = false;
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
        _ammoLeft.text = AmmoLeft.ToString();
    }
}
