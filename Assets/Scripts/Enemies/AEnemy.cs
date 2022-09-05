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
    public float _freezeDuration;

    //Misc

    public List<Transform> _projectileSpots;
    [HideInInspector]public Animator _enemyAnimator;
    private List<MeshRenderer> _meshes = new List<MeshRenderer>();

    public virtual void Awake()
    {
        _meshes = GetComponentsInChildren<MeshRenderer>().ToList();
        _timeSpentFrozen = 0;
        _enemyAnimator = GetComponentInChildren<Animator>();
    }
    public virtual void Start()
    {
    }
    public virtual void Update()
    {
        HandleStatusEffects();
    }

    public void InitStats()
    {
        TimeBetweenAttacks = 1 / _enemyStats._attacksPerSecond;
        TimeElapsedBetweenLastAttack = TimeBetweenAttacks;
        StacksUntilFreeze = _enemyStats._stacksUntilFreeze;
        _freezeDuration = _enemyStats._maxFrozenTime;
        _enemyDead = false;
        _enemyHealth.InitHealth(_enemyStats._enemyHealth);
    }

    //  Status Effects

    private void HandleStatusEffects()
    {
        if(_timeSpentFrozen >= _freezeDuration) Freeze(false);
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
        if(_enemyHealth.TakeDamage(damage))
        {
            if(!_enemyDead) EnemyKilled();
        }
        else
        {
            InitDamageTakenAnim();
        }
    }
    private void InitDamageTakenAnim()
    {
        //Debug.Log("Init Damage Taken Anim");
        StartCoroutine(ApplyDamageMaterialAnim());
    }

    private IEnumerator ApplyDamageMaterialAnim()
    {
        Material dmgMaterial = Resources.Load("Materials/Damaged Material") as Material;
        AddMaterialToAllMeshes(true, dmgMaterial);
        yield return new WaitForSecondsRealtime(0.1f);
        AddMaterialToAllMeshes(false, dmgMaterial);
    }

    public void EnemyKilled()
    {
        _enemyDead = true;
        //  Debug.Log("Enemy Killed");
        Events.instance.EnemyKilled(this);
    }
    public void InitDeathBehaviour()
    {
        //print("death anim");
        StartCoroutine(DeathAnim());
    }

    private IEnumerator DeathAnim()
    {
        yield return new WaitForSeconds(1f);
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
}
