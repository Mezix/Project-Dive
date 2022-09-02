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
    public bool TakeDamage(int damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0) return true;
        return false;
    }
}
