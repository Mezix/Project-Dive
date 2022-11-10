using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IceMusket : AWeapon
{
    public List<GameObject> _shotsReady;
    private Animator iceMusketAnimator;
    public GameObject IceMusketSFX;

    //  Weapon UI
    public Transform _hLayoutGroup;
    private List<Image> bulletUIList = new List<Image>();

    public override void Awake()
    {
        InitSystemStats();
        iceMusketAnimator = GetComponentInChildren<Animator>();
        ProjectilePrefab = (GameObject) Resources.Load("Weapons/Ice Ball");
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
        InitBulletsUI();
    }
    public override void Update()
    {
        base.Update();
        UpdateAmmoUI();

        if (ShouldRegenerateAmmo)
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
            UpdateAmmoDisplay(MagazineSize - ChargeLevel);
        }
    }
    private void InitBulletsUI()
    {
        for (int i = 0; i < MagazineSize; i++)
        {
            GameObject bulletUI = Instantiate((GameObject)Resources.Load("Weapons/Weapon UI/IceMusketBullet"));
            bulletUI.transform.SetParent(_hLayoutGroup, false);
            Image bulletImg = bulletUI.GetComponent<Image>();
            bulletUIList.Add(bulletImg);
        }
    }
    public override void TryFire()
    {
        if (TimeElapsedBetweenLastAttack >= TimeBetweenAttacks)
        {
            if(AmmoLeft > 0)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0) || (Input.GetKey(KeyCode.Mouse0) && !ChargeBegun))
                {
                    ChargeTime = 0; //reset charge time for the first frame
                    ChargeBegun = true;
                }
                Charging = Input.GetKey(KeyCode.Mouse0);  //keep holding the charge as long as we press charge down
                IceMusketSFX.SetActive(Charging);
                if (ChargeBegun && !Charging) //once we let go, fire!
                {
                    ChargeBegun = false;
                    _weaponFireSFX.Play();
                    SpawnProjectile();
                    REF.CamScript.StartShake(RecoilDuration, Recoil);
                }
            }
        }
    }
    public override void SpawnProjectile()
    {
        int bulletsFired = 0;
        foreach (Transform t in _projectileSpots[ChargeLevel - 1].UpgradeTierSpots)
        {
            bulletsFired++;
            GameObject proj = ProjectilePool.Instance.GetProjectileFromPool(ProjectilePrefab.tag);
            proj.GetComponent<AProjectile>().SetBulletStatsAndTransformToWeaponStats(this, t);
            proj.SetActive(true);
        }
        bulletsFired = Mathf.Min(MagazineSize, bulletsFired);
        SubtractAmmo(bulletsFired);

        iceMusketAnimator.SetFloat("ReloadMultiplier", AttacksPerSecond);
        if(ChargeLevel == 3)
        {
            iceMusketAnimator.SetTrigger("ReloadInitiated");
            if(REF.PCon._weaponDirection == 1) REF.PCon._playerRB.velocity = Vector3.zero; //if facing forward, kill all velocity
            REF.PCon.ApplyKnockback(KnockbackForce * bulletsFired);
        }
        else
        {
            iceMusketAnimator.SetTrigger("Fired");
            REF.PCon.ApplyKnockback(KnockbackForce * bulletsFired);
        }
        //Reloading = true;
        ChargeLevel = 1;
        TimeElapsedBetweenLastAttack = 0;
    }

    public void SubtractAmmo(int amount)
    {
        AmmoLeft -= amount;
        ShouldRegenerateAmmo = true;
        UpdateAmmoDisplay(AmmoLeft);
    }

    public void RegenerateAmmo(int amount)
    {
        AmmoLeft += amount;
        if(AmmoLeft >= MagazineSize)
        {
            ShouldRegenerateAmmo = false;
            TimeElapsedBetweenAmmoRegeneration = 0;
        }
        UpdateAmmoDisplay(AmmoLeft);
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

    public void UpdateAmmoDisplay(int ammoRemaining)
    {
        for (int i = 0; i < ammoRemaining; i++)
        {
            _shotsReady[i].SetActive(true);
        }
        for (int i = ammoRemaining; i < MagazineSize; i++)
        {
            _shotsReady[i].SetActive(false);
        }
    }
    public void UpdateAmmoUI()
    {
        for (int i = 0; i < MagazineSize - AmmoLeft; i++)
        {
            bulletUIList[i].sprite = Resources.Load("Graphics/UI/Weapons/Ammo/Ice Musket Empty", typeof(Sprite)) as Sprite;
        }
        for (int i = MagazineSize - AmmoLeft; i < MagazineSize; i++)
        {
            bulletUIList[i].sprite = Resources.Load("Graphics/UI/Weapons/Ammo/Ice Musket Full", typeof(Sprite)) as Sprite;
        }
    }
}
