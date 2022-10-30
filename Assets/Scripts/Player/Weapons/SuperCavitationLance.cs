using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperCavitationLance : AWeapon
{
    //  Jet Charge
    public float _stabDamage; //the regular "Damage" field in this case refers to the thrown damage
    public bool _toggleOrHoldToJet;
    public bool _jetsOn;
    public float _jetTime;

    // Throw Lance
    public bool _throwPossible;
    public LanceProjectile _lanceProjectile;

    //  Misc
    public GameObject _lanceModel;

    public override void Awake()
    {
        InitSystemStats();
        _lanceProjectile.gameObject.SetActive(false);
        LanceIntoHoldPosition();
        AmmoLeft = MagazineSize;
        ShouldRegenerateAmmo = false;
        TimeBetweenAmmoRegeneration = Mathf.Infinity;
        AmmoRegenAmount = 2;

        WeaponLevel = 3;
        MaxLevel = 3;
        _jetTime = 0;
        _throwPossible = true;
    }
    public void Start()
    {
        SetUpgradeLevel(WeaponLevel);
    }

    public override void Update()
    {
        base.Update();
    }
    public override void TryFire()
    {
        if (TimeElapsedBetweenLastAttack >= TimeBetweenAttacks)
        {
            if(!_jetsOn)
            {
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    if(_throwPossible)
                    {
                        ThrowLance();
                    }
                    else
                    {
                        ForceRetractLance();
                    }
                }
            }

            if (AmmoLeft > 0 && _throwPossible)
            {
                if(_toggleOrHoldToJet) // Toggle Mode: Pressing left click jets you forward until you click again, after which you stab forward
                {
                    bool jetsOldStatus = _jetsOn;
                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        _jetsOn = !_jetsOn;
                        LanceIntoChargePosition();
                    }
                    if (jetsOldStatus && !_jetsOn)
                    {
                        SuperCavStab();
                        LanceIntoHoldPosition();
                    }
                }
                else // Hold mode: Holding left click jets you forward, releasing stabs forwadd
                {
                    bool jetsOldStatus = _jetsOn;
                    if(Input.GetKeyDown(KeyCode.Mouse0)) LanceIntoChargePosition();
                    if (Input.GetKey(KeyCode.Mouse0))
                    {
                        _jetsOn = true;
                    }
                    else
                    {
                        _jetsOn = false;
                        if (jetsOldStatus && !_jetsOn)
                        {
                            SuperCavStab();
                            LanceIntoHoldPosition();
                        }
                    }
                }
            }
        }
    }

    private void ThrowLance()
    {
        //  TODO: LOCK ONTO ENEMY!

        LanceIntoHoldPosition();
        _lanceModel.gameObject.SetActive(false);
        _lanceProjectile.gameObject.SetActive(true);

        _throwPossible = false;

        _lanceProjectile._projectileRB.isKinematic = false;
        _lanceProjectile._trailRenderer.Clear();

        _lanceProjectile.SetBulletStatsAndTransformToWeaponStats(this, _projectileSpots[0].UpgradeTierSpots[0]);
        _lanceProjectile.transform.parent = null;
        _lanceProjectile._launched = true;
        _lanceProjectile._stuck = false;
        TimeElapsedBetweenLastAttack = 0;
        //harpoonFiredParticleSystem.Play();
        REF.PCon.ApplyKnockback(KnockbackForce);
        REF.CamScript.StartShake(RecoilDuration, Recoil);
    }

    public void ForceRetractLance()
    {
        _lanceModel.gameObject.SetActive(true);
        _lanceProjectile.gameObject.SetActive(false);
        _throwPossible = true;

        _lanceProjectile.HasDoneDamage = false;
        _lanceProjectile._launched = false;
        _lanceProjectile._stuck = false;
        _lanceProjectile._projectileRB.isKinematic = true;
        _lanceProjectile._trailRenderer.Clear();

        _lanceProjectile.transform.SetParent(_projectileSpots[0].UpgradeTierSpots[0].transform, false);
        _lanceProjectile.transform.localPosition = Vector3.zero;
        _lanceProjectile.transform.localScale = Vector3.one;
        HM.RotateLocalTransformToAngle(_lanceProjectile.transform, Vector3.zero);
    }

    public void FixedUpdate()
    {
        if(_jetsOn)
        {
            //Drain Fuel
            _jetTime += Time.deltaTime;
            _jetTime = Mathf.Min(1, _jetTime);
            JetForward();
            REF.CamScript.StartShake(RecoilDuration, Recoil);
        }
        else
        {
            _jetTime = 0;
        }
    }
    private void JetForward()
    {
        REF.PCon.ApplyKnockback(KnockbackForce * -1 * (1 + _jetTime));
    }

    private void SuperCavStab()
    {
        RaycastHit hit = HM.RaycastAtPosition(REF.PCon._FPSLayerCam.transform.position, REF.PCon._FPSLayerCam.transform.forward, REF.PCon._meleeDistance, LayerMask.GetMask("Enemy"));
        if (hit.collider)

            if (hit.collider.GetComponentInChildren<AEnemy>())
            {
                AEnemy enemy = hit.collider.GetComponentInChildren<AEnemy>();
                float finalMeleeDamage = _stabDamage;
                finalMeleeDamage *= Mathf.Max(_stabDamage, REF.PCon.playerRB.velocity.magnitude / 15f); // do at minimum base damage
                finalMeleeDamage = Mathf.Min(_stabDamage * 6, finalMeleeDamage); // do maximum of 3 times the damage
                if (enemy._frozen) finalMeleeDamage *= 2; //double damage if frozen
                hit.collider.GetComponentInChildren<AEnemy>().TakeDamage(finalMeleeDamage);
                REF.PCon.playerRB.velocity = Vector3.zero;
                REF.PCon.ApplyKnockback(REF.PCon._meleeKnockback); 
            }
    }

    private void LanceIntoChargePosition()
    {
        _lanceModel.transform.localPosition = new Vector3(-0.78f, 0.2f, -0.8f);
    }

    private void LanceIntoHoldPosition()
    {
        _lanceModel.transform.localPosition = new Vector3(0.1f, 0, -0.8f);
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
}
