using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float _maxHealth;
    public float _currentHealth;

    public void InitHealth(float max)
    {
        _maxHealth = _currentHealth = max;
    }
    /// <summary>
    /// Returns true if dead, otherwise false.
    /// </summary>
    /// <param name="damage"></param>
    /// <returns></returns>
    public bool TakeDamage(float damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0) return true;
        return false;
    }
}
