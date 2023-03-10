using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyProjectileType
{
    Stinger,
    Bean
}

public class EnemyProjectilePool : MonoBehaviour, ISerializationCallbackReceiver
{
    //The amount of projectiles of each type that are going to be spawned
    private const int INITIAL_SPAWN_AMOUNT = 5;

    public List<EnemyProjectileType> projectileTypesInLevel;
    public List<GameObject> projectilePrefabs;

    private Dictionary<EnemyProjectileType, GameObject> _projectilesToSpawn = 
        new Dictionary<EnemyProjectileType, GameObject>();

    private Dictionary<EnemyProjectileType, List<GameObject>> _projectiles =
        new Dictionary<EnemyProjectileType, List<GameObject>>();

    private static EnemyProjectilePool _i;

    public static EnemyProjectilePool I => _i;

    private void Awake()
    {
        _i = this;
        InstantiateProjectiles();
    }

    /// <summary>
    /// Get an instance of one of the projectiles in the pool. If all the objects are in use it creates
    /// a new one and returns it.
    /// </summary>
    /// <param name="type"> The type of the projectile you want to get</param>
    /// <returns> The instance of an object in the pool</returns>
    public GameObject GetProjectile(EnemyProjectileType type)
    {
        List<GameObject> projectiles = _projectiles[type];
        for (int i = 0; i < projectiles.Count; i++)
        {
            if (!projectiles[i].activeInHierarchy)
            {
                projectiles[i].SetActive(true);
                return projectiles[i];
            }
        }
        var tmp = AddProjectile(type);
        tmp.SetActive(true);
        return tmp;
    }

    public void DeleteProjectile(GameObject projectileDel)
    {
        projectileDel.SetActive(false);
    }

    private void InstantiateProjectiles()
    {
        foreach (var projectile in _projectilesToSpawn)
        {
            List<GameObject> tmpList = new List<GameObject>();
            for (int i = 0; i < INITIAL_SPAWN_AMOUNT; i++)
            {
                GameObject tmp = Instantiate(projectile.Value, gameObject.transform);
                tmp.SetActive(false);
                tmpList.Add(tmp);
            }

            _projectiles.Add(projectile.Key, tmpList);
        }
    }

    private GameObject AddProjectile(EnemyProjectileType type)
    {
        GameObject tmp = Instantiate(_projectilesToSpawn[type], gameObject.transform);
        tmp.SetActive(false);
        _projectiles[type].Add(tmp);
        return tmp;
    }


    public void OnBeforeSerialize()
    {
        //Don't need to do anything with this method
    }

    public void OnAfterDeserialize()
    {
        _projectilesToSpawn = new Dictionary<EnemyProjectileType, GameObject>();

        for (int i = 0; i != Math.Min(projectileTypesInLevel.Count, projectilePrefabs.Count); i++)
            _projectilesToSpawn.Add(projectileTypesInLevel[i], projectilePrefabs[i]);
    }
}