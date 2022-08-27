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

    private void Start()
    {
        _maxHealth = 100;
        _currentHealth = _maxHealth;
        _timeInVengeance = 0;
        _drainPct = 0.1f;
        _vengeanceDrainPercentage = 1;

        Events.instance.EnemyDead += ResetVengeance;
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha0))
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
        _currentHealth -= dmg;
        if(_currentHealth <= 0)
        {
            _currentHealth = 0;
            EnterVengeanceMode();
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
            print("Game Over");
        }
        REF.PCon._pUI.UpdateHealthBar(_currentHealth, _maxHealth, _vengeanceDrainPercentage);
    }
    private void EnterVengeanceMode()
    {
        _vengeanceModeActive = true;
    }
    private void ResetVengeance(GameObject enemy = null)
    {
        if(_vengeanceModeActive)
        {
            _vengeanceModeActive = false;
            Heal(0.3f * _maxHealth * _vengeanceDrainPercentage);
        }
    }
}
