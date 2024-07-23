using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossManager : MonoBehaviour
{
    public GameObject _enemyBossUI;
    public Image _bossHealthbar;
    public Pufferfish _spawnedBoss;
    public bool activated = false;

    void Start()
    {
        _enemyBossUI.SetActive(false);
        Events.instance.EnemyDead += EnemyKilled;
    }
    private void Update()
    {
       if(_spawnedBoss) UpdateBossHealth();
    }
    private void EnemyKilled(AEnemy enemy)
    {
        if (enemy._isBoss) BossKilled();
        else
        {
            //_spawnedEnemies.Remove(enemy);
            //_enemiesSpawned--;
            //_enemiesUntilBoss--;
            //_enemiesKilled++;
        }
        enemy.InitDeathBehaviour();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponentInChildren<PlayerController>() && !activated)
        {
            activated = true;
            _enemyBossUI.SetActive(true);
            _spawnedBoss.eState = Pufferfish.enemyState.FoundPlayer;
        }
    }

    private void BossKilled()
    {
        _enemyBossUI.SetActive(false);
        StartCoroutine(REF.CamScript.StartBossShake());
        AkSoundEngine.SetSwitch("PressureSoundtrackSwitch", "Pressure0", SoundtrackChangerCollider.PressureSoundtrackObject);
        AkSoundEngine.SetSwitch("KillstreakMusicSwitch", "Killstreak0", SoundtrackChangerCollider.PressureSoundtrackObject);
        print("boss killed");
    }
    private void UpdateBossHealth()
    {
        float healthPct = _spawnedBoss._enemyHealth._currentHealth / _spawnedBoss._enemyHealth._maxHealth;
        healthPct = Mathf.Max(healthPct, 0);
        _bossHealthbar.fillAmount = healthPct;
    }
}
