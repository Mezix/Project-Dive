using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AEnemy : MonoBehaviour
{
    public EnemyStats _enemyStats;

    public float AttacksPerSecond { get; set; }
    public float TimeBetweenAttacks { get; private set; }
    public float TimeElapsedBetweenLastAttack { get; protected set; }
    public float Weight { get; set; }
    public EnemyHealth _enemyHealth;
    public bool _enemyDead;
    public bool _isBoss;

    //  Effects

    public bool _burning;
    public float _burnDamage;
    public float _timeSpentBurning;
    public float _burnDuration;

    public int _currentFreezeStacks;
    public int StacksUntilFreeze { get; private set; }
    public bool _frozen;
    public float _timeSpentFrozen;
    public float FreezeDuration { get; set; }

    //  Misc

    public List<Transform> _projectileSpots;
    [HideInInspector] public Animator _enemyAnimator;
    private List<MeshRenderer> _meshes = new List<MeshRenderer>();
    public Rigidbody _enemyRB;
    public Collider _enemyCol;

    //  Damage Indicator

    public DamagePrefabScript currentDamagePrefabScript;
    public float _timeBetweenDamageStacking;
    public float _timeSinceLastDamage;

    public virtual void Awake()
    {
        _meshes = GetComponentsInChildren<MeshRenderer>().ToList();
        _timeSpentFrozen = 0;
        _enemyAnimator = GetComponentInChildren<Animator>();
        _enemyRB = GetComponentInChildren<Rigidbody>();
        _enemyCol = GetComponentInChildren<Collider>();
        _timeBetweenDamageStacking = 0.3f;
        _timeSinceLastDamage = _timeBetweenDamageStacking;
    }
    public virtual void Start()
    {
    }
    public virtual void Update()
    {
        HandleStatusEffects();
        _timeSinceLastDamage += Time.deltaTime;
        if (_timeSinceLastDamage > _timeBetweenDamageStacking)
            if (currentDamagePrefabScript) currentDamagePrefabScript = null;
    }

    public void InitStats()
    {
        if(_enemyStats)
        {
            TimeBetweenAttacks = 1 / _enemyStats._attacksPerSecond;
            TimeElapsedBetweenLastAttack = TimeBetweenAttacks;
            StacksUntilFreeze = _enemyStats._stacksUntilFreeze;
            FreezeDuration = _enemyStats._maxFrozenTime;
            Weight = _enemyStats._weight;
        }
        else
        {
            TimeBetweenAttacks = 1;
            TimeElapsedBetweenLastAttack = TimeBetweenAttacks;
            StacksUntilFreeze = 1000;
            FreezeDuration = 1;
            Weight = 1;
        }
        _enemyDead = false;
        _enemyHealth.InitHealth(_enemyStats._enemyHealth);
    }

    //  Status Effects

    private void HandleStatusEffects()
    {
        if(_timeSpentFrozen >= FreezeDuration) Freeze(false);
        if (_frozen) _timeSpentFrozen += Time.deltaTime;
    }

    public void AddFreezeStack(int stacks)
    {
        _currentFreezeStacks += stacks;
        if (_currentFreezeStacks >= StacksUntilFreeze) Freeze(true);
    }
    public void Freeze(bool shouldFreeze)
    {
        if (shouldFreeze)
        {
            _timeSpentFrozen = 0;
            if(!_frozen) //for the first time we are frozen, apply the effect
            {
                AkSoundEngine.PostEvent("Play_FreezeEffect", gameObject);
                Material freezeMat = Resources.Load("Materials/Frozen Material") as Material;
                AddMaterialToAllMeshes(true, freezeMat);
                _enemyAnimator.speed = 0;
            }
        }
        else
        {
            if(_frozen) //only for the first unfreeze
            {
                Material freezeMat = Resources.Load("Materials/Frozen Material") as Material;
                AddMaterialToAllMeshes(false, freezeMat);
                _currentFreezeStacks = 0;
                _enemyAnimator.speed = 1;
            }
        }
        _frozen = shouldFreeze;
    }
    //  Damage
    public void TakeDamage(float damage)
    {
        bool enemyHPIsZero = _enemyHealth.TakeDamage(damage);
        if(!_enemyDead)
        {
            AddDamagePrefab(damage);
            InitDamageTakenAnim();
        }
        if(_frozen)
        {
            AkSoundEngine.PostEvent("Play_IceTargetHit", gameObject);
        }
        if (enemyHPIsZero && !_enemyDead)
        {
            KillEnemy();
        }
        
    }

    private void AddDamagePrefab(float damage)
    {
        _timeSinceLastDamage = 0;
        if (currentDamagePrefabScript)
        {
            ProjectilePool.Instance.AddToPool(currentDamagePrefabScript.gameObject);
            damage += currentDamagePrefabScript._damage;
        }
        GameObject g = ProjectilePool.Instance.GetProjectileFromPool("Damage Text");
        currentDamagePrefabScript = g.GetComponent<DamagePrefabScript>();
        currentDamagePrefabScript.InitDamage(damage, this);
        Events.instance.DamageDealtEvent(damage);
    }

    private void InitDamageTakenAnim()
    {
        StartCoroutine(ApplyDamageMaterialAnim());
    }

    private IEnumerator ApplyDamageMaterialAnim()
    {
        Material dmgMaterial = Resources.Load("Materials/Damaged Material") as Material;
        AddMaterialToAllMeshes(true, dmgMaterial);
        yield return new WaitForSecondsRealtime(0.1f);
        AddMaterialToAllMeshes(false, dmgMaterial);
    }

    public void KillEnemy()
    {
        _enemyDead = true;
        AkSoundEngine.PostEvent("Play_PufferfishDeath", gameObject);
        Events.instance.EnemyKilledEvent(this);
    }
    public void InitDeathBehaviour()
    {
        //print("death anim");
        RemoveStuckProjectiles();
        StartCoroutine(DeathAnim());
    }

    private IEnumerator DeathAnim()
    {
        for(int i = 0; i < 100; i++)
        {
            transform.Rotate(new Vector3(0,0,-0.9f), Space.Self);
            _enemyRB.AddForce(Vector3.up * i);
            yield return new WaitForFixedUpdate();
        }
        Destroy(gameObject);
    }

    //  Misc 

    private void AddMaterialToAllMeshes(bool add, Material matToAdd)
    {
        if(add)
        {
            foreach (MeshRenderer mr in _meshes)
            {
                List<Material> mats = mr.sharedMaterials.ToList();
                if (!mats.Contains(matToAdd))
                {
                    mats.Add(matToAdd);
                    mr.sharedMaterials = mats.ToArray();
                }
            }
        }
        else
        {
            foreach (MeshRenderer mr in _meshes)
            {
                List<Material> mats = mr.sharedMaterials.ToList();
                foreach (Material mat in mats)
                {
                    if (mat.Equals(matToAdd))
                    {
                        mats.Remove(matToAdd);
                        break;
                    }
                }
                mr.sharedMaterials = mats.ToArray();
            }
        }
    }

    //  Check for Stuck Projectiles so they dont despawn with the enemy, especially important for the lance and harpoon
    private void RemoveStuckProjectiles()
    {
        LanceProjectile _lance = GetComponentInChildren<LanceProjectile>();
        if(_lance)
        {
            _lance.HasDoneDamage = false;
            _lance._cavitationLance.ForceRetractLance();
        }

        HarpoonProjectile harpoon = GetComponentInChildren<HarpoonProjectile>();
        if (harpoon)
        {
            harpoon.HasDoneDamage = false;
            harpoon._harpoonGun.ForceRetract();
        }
    }
}
