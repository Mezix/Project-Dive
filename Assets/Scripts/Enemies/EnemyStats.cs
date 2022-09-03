using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/EnemyStats")]
public class EnemyStats : ScriptableObject
{
    public float _attacksPerSecond;
    public float _enemyHealth;

    public int _stacksUntilFreeze;
    public float _maxFrozenTime;
}
