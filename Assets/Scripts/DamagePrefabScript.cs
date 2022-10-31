using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamagePrefabScript : MonoBehaviour
{
    public Rigidbody _rb;
    public Text _damageText;
    public float _damage;
    public float _currentLifeTime;
    public float _maxLifetime;
    public void InitDamage(float damage, AEnemy enemy)
    {
        transform.position = enemy.transform.position + new Vector3(UnityEngine.Random.Range(0.2f, -0.2f), UnityEngine.Random.Range(0.2f, -0.2f), UnityEngine.Random.Range(0.2f, -0.2f));
        _damage = damage;
        _damageText.text = HM.FloatToString(damage);
        _damageText.color = Color.red;
        _currentLifeTime = 0;
        _maxLifetime = 1;
    }
    public virtual void FixedUpdate()
    {
        if (REF.PCon) transform.LookAt(REF.PCon.transform);

        _currentLifeTime += Time.deltaTime;
        _currentLifeTime = Mathf.Min(_maxLifetime, _currentLifeTime);
        if (_currentLifeTime < _maxLifetime) MoveDamageText();
        else ProjectilePool.Instance.AddToPool(gameObject);
    }
    private void MoveDamageText()
    {
        _rb.MovePosition(_rb.transform.position += Vector3.up * 0.15f + Vector3.right * UnityEngine.Random.Range(-0.02f, 0.02f));
        _damageText.color = new Color(_damageText.color.r, _damageText.color.g, _damageText.color.b, 1 - _currentLifeTime/_maxLifetime);
    }
}
