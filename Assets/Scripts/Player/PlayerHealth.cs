using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float _currentHealth;
    public float _maxHealth;
    public float HealthPercentage { get { return _currentHealth / _maxHealth; }}

    public bool _vengeanceModeActive;
    public float _timeInVengeance;
    public float _drainPct;
    public float _vengeanceDrainPercentage;
    [SerializeField]
    private AK.Wwise.RTPC breathingRTPC;

    private const float timeBetweenTakingDamage = 1f;
    private float timeSinceLastTakenDamage = timeBetweenTakingDamage;

    private void Start()
    {
        breathingRTPC.SetGlobalValue(0);
        _maxHealth = 100;
        _currentHealth = _maxHealth;
        _timeInVengeance = 0;
        _drainPct = 0.1f;
        _vengeanceDrainPercentage = 1;

        Events.instance.EnemyDead += EnemyKilled;
        AkSoundEngine.PostEvent("Play_BreathingContainer", gameObject);
    }
    private void Update()
    {
        timeSinceLastTakenDamage += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Heal(0.1f * _maxHealth * _vengeanceDrainPercentage);
        }
        if(_vengeanceModeActive)
        {
            _timeInVengeance += Time.deltaTime;
            HandleVengeanceMode();
        }
    }

    public void TakeDamage(float dmg)
    {
        if(timeSinceLastTakenDamage < timeBetweenTakingDamage)
        {
            return;
        }
        _currentHealth -= dmg;
        if(_currentHealth <= 0)
        {
            _currentHealth = 0;
            EnterVengeanceMode();
        }
        else
        {
            AkSoundEngine.PostEvent("Play_PlayerTakesDamage", gameObject);
            timeSinceLastTakenDamage = 0;
            REF.CamScript.StartShake(0.2f, 0.3f);
            breathingRTPC.SetGlobalValue(0);
        }
        //breathingRTPC.SetGlobalValue(100 * (1 - _currentHealth/_maxHealth));
        float healthPCT = _currentHealth / _maxHealth;
;
        if(healthPCT > 0.77f) // calmest breathing
        {
            AkSoundEngine.SetSwitch("Breathing", "Breathing1", gameObject);
        }
        else if(healthPCT > 0.44f)
        {
            AkSoundEngine.SetSwitch("Breathing", "Breathing2", gameObject);
        }
        else if (healthPCT > 0.22f)
        {
            AkSoundEngine.SetSwitch("Breathing", "Breathing3", gameObject);
        }
        else  // MAX INTENSITY
        {
            AkSoundEngine.SetSwitch("Breathing", "Breathing4", gameObject);
        }
        REF.PlayerUI.UpdateHealthBar(_currentHealth, _maxHealth, _vengeanceDrainPercentage);
    }

    public void Heal(float heal)
    {
        _currentHealth += heal;
        if (_currentHealth >= _maxHealth * _vengeanceDrainPercentage)
        {
            _currentHealth = _maxHealth * _vengeanceDrainPercentage;
            if(_vengeanceDrainPercentage < 1)
            {
                _vengeanceDrainPercentage += heal / _maxHealth;
            }
        }
        _vengeanceModeActive = false;
        REF.PlayerUI.UpdateHealthBar(_currentHealth, _maxHealth, _vengeanceDrainPercentage);
    }

    //  Vengeance Mode

    private void HandleVengeanceMode()
    {
        _vengeanceDrainPercentage -= _drainPct * Time.deltaTime;
        if(_vengeanceDrainPercentage <= 0)
        {
            //Game Over
            HandlePlayerDeath();
        }
        REF.PCon._pUI.UpdateHealthBar(_currentHealth, _maxHealth, _vengeanceDrainPercentage);
    }

    public void HandlePlayerDeath()
    {
        print("Game Over started");
        REF.PCon.InitPlayerDeath();
    }

    private void EnterVengeanceMode()
    {
        _vengeanceModeActive = true;
        REF.PCon.SetVengeanceMode(true);
        REF.PCon._pUI.VeangenceModeUI(true);
    }
    private void EnemyKilled(AEnemy enemy = null)
    {
        if(_vengeanceModeActive)
        {
            _vengeanceModeActive = false;
            REF.PCon._pUI.VeangenceModeUI(false);
            REF.PCon.SetVengeanceMode(false);
            Heal(0.3f * _maxHealth * _vengeanceDrainPercentage);
        }
        else
        {
            Heal(0.1f * _maxHealth);
        }
    }
}
