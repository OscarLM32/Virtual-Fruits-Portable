using System;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Generic class of a projectile created by an enemy
/// </summary>
public class EnemyProjectile : MonoBehaviour
{

    ///<see cref="SoundPlayer"/>
    public SoundPlayer soundPlayer;
    
    /// <summary>
    /// Prefab of the object emitting the specific projectile's particles on collision 
    /// </summary>
    public GameObject particlesObject;
    public float speed;
    
    /// <summary>
    /// Sets up the direction and the position for a new pooled projectile
    /// </summary>
    /// <see cref="EnemyProjectilePool"/>
    /// <param name="direction"> Direction the projectile has to follow: up, down, right or left </param>
    /// <param name="position"> Position from which it is going to be shot</param>
    public void SetUpForShooting(Vector2 direction, Vector3 position)
    {
        GetComponent<Rigidbody2D>().velocity = direction * speed;
        transform.position = position;
    }
    
    /// <summary>
    /// Checks if the collided object is killable and in that case it kills it.
    /// After that, plays the proper sound, instantiates particles and destroys the object from the projectiles pool 
    /// </summary>
    private void OnCollisionEnter2D(Collision2D col)
    {
        IKillable killableComponent = col.gameObject.GetComponent<IKillable>();
        killableComponent?.Kill(gameObject);
        soundPlayer.PlayDisposableAtPosition("ProjectileDestroy", transform.position);
        InstantiateParticles();
        EnemyProjectilePool.I.DeleteProjectile(gameObject); 
    }
    
    /// <summary>
    /// Creates an instance of the particles object in the position where the projectile was destroyed
    /// </summary>
    private void InstantiateParticles()
    {
        var p = Instantiate(particlesObject, gameObject.transform.position, quaternion.identity);
    }
}
