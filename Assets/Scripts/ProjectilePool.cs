using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPoolItem //this is an internal class, which we fill with prefabs of our projectiles
{
    public int _amountToPool; //how many projectiles should be created each time we expand the pool
    public GameObject _projectilePrefab; //the actual prefab of the projectile
}
public class ProjectilePool : MonoBehaviour
{
    private List<GameObject> projectilePool; //where all our different bullets are stored

    public List<ObjectPoolItem> _projectilesToPool; //the list of all our bullets we need to instantiate and keep track of
    public static ProjectilePool Instance { get; private set; } //the reference to the pool itself

    private void Awake()
    {
        Instance = this; //set the reference to ourselves
    }
    void Start()
    {
        projectilePool = new List<GameObject>(); //init the list
        foreach(ObjectPoolItem item in _projectilesToPool) //at the start of the game make sure we create a few of each projectile
        {
            GrowBulletPool(item._projectilePrefab);
        }
    }
    private void GrowBulletPool(GameObject bulletPrefab)
    {
        for (int i = 0; i < 10; i++)
        {
            var bullet = Instantiate(bulletPrefab); //create a bullet of this kind
            //bullet.transform.SetParent(transform); //add all the created bullets to the pool so they aren't loose in the scene
            AddToPool(bullet);
        }
    }
    public void AddToPool(GameObject projectile)
    {
        projectile.transform.SetParent(transform);
        projectile.SetActive(false); //disable the gameobjects so we don't need to see them
        projectilePool.Add(projectile); //add the projectile to the entire pool
    }

    public GameObject GetProjectileFromPool(string tag) //take the tag of the prefab we need, and return an instance of it
    {
        for (int i = 0; i < projectilePool.Count; i++) //go through the entire pool of all bullets to find one we need
        {
            if (!projectilePool[i].activeInHierarchy && projectilePool[i].tag == tag) //only pull objects which are currently inactive, and have the same tags
            {
                projectilePool[i].SetActive(true); //if we found the correct bullet, activate it
                //projectilePool[i].transform.SetParent(null);
                return projectilePool[i];          //...and now return it!
            }
        }

        //this is only called if we havent found a correct bullet

        foreach (ObjectPoolItem item in _projectilesToPool) //check through all possible types of bullets
        {
            if (item._projectilePrefab.tag == tag) //check the tag of the prefab to the one we got
            {
                GrowBulletPool(item._projectilePrefab); //create more of the prefab
                return GetProjectileFromPool(tag); // now get the new projectile, since we created some
            }
        }
        return null;
    }
}