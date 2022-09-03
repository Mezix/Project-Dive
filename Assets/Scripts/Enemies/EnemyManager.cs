using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    public List<AEnemy> _enemyTypes;
    public List<AEnemy> _spawnedEnemies;
    public int _enemiesSpawned;
    public int _maxEnemies;
    public int _enemiesUntilBoss;
    public int _enemiesKilled;

    // Boss
    public bool _bossSpawned;
    public AEnemy _spawnedBoss;
    public GameObject _enemyBossUI;
    public GameObject _BossToSpawn;

    public Image _bossHealthbar;

    public void Start()
    {
        _enemyBossUI.SetActive(false);
        Events.instance.EnemyDead += EnemyKilled;
        SpawnAllEnemies();
        _enemiesUntilBoss = 3;
    }
    public void Update()
    {
        if (_bossSpawned) UpdateBossHealth();
        if (_enemiesUntilBoss <= 0 && !_bossSpawned) SpawnBoss();
        if (_enemiesSpawned >= _maxEnemies) return;
        SpawnEnemy(_enemyTypes[0]);
    }

    private void EnemyKilled(AEnemy enemy)
    {
        if (enemy._isBoss) BossKilled();
        else
        {
            _spawnedEnemies.Remove(enemy);
            _enemiesSpawned--;
            _enemiesUntilBoss--;
            _enemiesKilled++;
        }
        enemy.InitDeathBehaviour();
    }


    public void SpawnAllEnemies()
    {
        for(int i = 0; i < _maxEnemies; i++)
        {
            SpawnEnemy(_enemyTypes[0]);
        }
    }
    public void SpawnEnemy(AEnemy enemy)
    {
        if (_enemiesSpawned == _maxEnemies) return;
        _enemiesSpawned++;
        GameObject e = Instantiate(enemy.gameObject);
        e.transform.position = new Vector3(5,8,30) + new Vector3(UnityEngine.Random.Range(-5,5), UnityEngine.Random.Range(-5, 5), UnityEngine.Random.Range(-5, 5));
        _spawnedEnemies.Add(enemy);
    }

    //  Boss

    private void SpawnBoss()
    {
        _bossSpawned = true;
        _enemyBossUI.SetActive(true);
        GameObject boss = Instantiate(_BossToSpawn);
        _spawnedBoss = boss.GetComponent<AEnemy>();
        _spawnedBoss._isBoss = true;
    }
    private void UpdateBossHealth()
    {
        float healthPct = _spawnedBoss._enemyHealth._currentHealth / _spawnedBoss._enemyHealth._maxHealth;
        healthPct = Mathf.Max(healthPct, 0);
        _bossHealthbar.fillAmount = healthPct;
    }
    private void BossKilled()
    {
        _enemiesUntilBoss = 3;
        _bossSpawned = false;
        _enemyBossUI.SetActive(false);
        _spawnedBoss = null;
        //print("boss killed");
    }
}
