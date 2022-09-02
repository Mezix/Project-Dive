using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public List<AEnemy> _enemyTypes;
    public int _enemiesSpawned;
    public int _maxEnemies;
    public void Start()
    {
        Events.instance.EnemyDead += EnemyKilled;
        SpawnAllEnemies();
    }
    private void Update()
    {
        if (_enemiesSpawned >= _maxEnemies) return;
        SpawnEnemy(_enemyTypes[0]);
    }
    private void EnemyKilled(GameObject obj)
    {
        _enemiesSpawned--;
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
        GameObject e = Instantiate(enemy.gameObject);
        e.transform.position = new Vector3(5,8,30) + new Vector3(UnityEngine.Random.Range(-5,5), UnityEngine.Random.Range(-5, 5), UnityEngine.Random.Range(-5, 5));
        _enemiesSpawned++;
    }
}
