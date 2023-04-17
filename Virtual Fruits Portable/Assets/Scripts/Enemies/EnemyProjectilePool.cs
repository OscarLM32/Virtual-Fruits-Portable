using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// All the different enemy projectiles types that exist
/// </summary>
public enum EnemyProjectileType
{
    Stinger,
    Bean,
    Stake
}

/// <summary>
/// Object pool pattern for the enemy projectiles in each level.
/// </summary>
public class EnemyProjectilePool : MonoBehaviour, ISerializationCallbackReceiver
{
    /// <summary>
    /// Initial amount of projectiles of each type that are going to be added to the pool
    /// </summary>
    private const int _initialSpawnAmount = 5;

    /// <summary>
    /// List of the projectiles that can be found in the level
    /// </summary>
    public List<EnemyProjectileType> projectileTypesInLevel;
    
    /// <summary>
    /// Prefabs of the projectiles in the list of projectile types in the level
    /// </summary>
    /// <remarks> "projectileTypesInLevel" list and this list must have the same order</remarks>
    /// <see cref="projectileTypesInLevel"/>
    public List<GameObject> projectilePrefabs;

    /// <summary>
    /// Deserialization of "projectileTypesInLevel" and "projectilePrefabs" into a dictionary
    /// </summary>
    /// <see cref="projectileTypesInLevel"/>
    /// <br></br>
    /// <see cref="projectilePrefabs"/>
    private Dictionary<EnemyProjectileType, GameObject> _projectilesToSpawn = 
        new Dictionary<EnemyProjectileType, GameObject>();

    /// <summary>
    /// Pool of projectiles generated in this level
    /// </summary>
    private Dictionary<EnemyProjectileType, List<GameObject>> _projectiles =
        new Dictionary<EnemyProjectileType, List<GameObject>>();

    /// <summary>
    /// Private static instance of the singleton pattern
    /// </summary>
    private static EnemyProjectilePool _i;

    /// <summary>
    /// Static getter for the instance of this object in the scene
    /// </summary>
    public static EnemyProjectilePool I => _i;

    /// <summary>
    /// Initialization of the class. If _i is not null, then there is already an existing instance of this class
    /// in scene, so the object is deleted. In other case it sets _i to its' own instance and instantiates the projectiles
    /// </summary>
    /// <see cref="InstantiateProjectiles"/>
    private void Awake()
    {
        if(_i != null)
            Destroy(gameObject);
        else
        {
            _i = this;
            InstantiateProjectiles();  
        }
    }

    /// <summary>
    /// Get an instance of one of the projectiles in the pool. If all the objects are in use it creates
    /// a new one and returns it.
    /// </summary>
    /// <param name="type"> The type of the projectile meant to get</param>
    /// <returns> The instance of an object (active) in the pool</returns>
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

    /// <summary>
    /// Sets the projectile in the pool being used by any enemy to inactive
    /// </summary>
    /// <param name="projectileDel"> The object to be deactivated</param>
    public void DeleteProjectile(GameObject projectileDel)
    {
        projectileDel.SetActive(false);
    }

    /// <summary>
    /// Instantiates and adds all the initial projectiles in the pool
    /// </summary>
    private void InstantiateProjectiles()
    {
        foreach (var projectile in _projectilesToSpawn)
        {
            List<GameObject> tmpList = new List<GameObject>();
            for (int i = 0; i < _initialSpawnAmount; i++)
            {
                GameObject tmp = Instantiate(projectile.Value, gameObject.transform);
                tmp.SetActive(false);
                tmpList.Add(tmp);
            }

            _projectiles.Add(projectile.Key, tmpList);
        }
    }

    /// <summary>
    /// Instantiates a new projectile and adds it into the pool
    /// </summary>
    /// <param name="type">The type of thr projectile</param>
    /// <returns>The projectile created</returns>
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

    /// <summary>
    /// Deserialization method to convert "projectileTypesInLevel" and "projectilePrefabs" into a dictionary
    /// </summary>
    public void OnAfterDeserialize()
    {
        _projectilesToSpawn = new Dictionary<EnemyProjectileType, GameObject>();

        for (int i = 0; i != Math.Min(projectileTypesInLevel.Count, projectilePrefabs.Count); i++)
            _projectilesToSpawn.Add(projectileTypesInLevel[i], projectilePrefabs[i]);
    }
}