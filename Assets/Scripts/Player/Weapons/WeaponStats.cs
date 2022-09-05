using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/WeaponStats")]
public class WeaponStats : ScriptableObject
{
    public string _weaponName = "";
    public Sprite _UISprite;
    public int _damage;
    public float _projectileSpeed;
    public float _attacksPerSecond;
    public float _recoil;
    public float _recoilDuration;
    public float _knockbackForce;
    public float _backwardsModifier;
    public bool _canReverseWeapon;
    public int _magazineSize;
    public int _maxLevel;
}
