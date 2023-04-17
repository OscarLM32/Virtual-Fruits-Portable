using System.Collections;
using UnityEngine;

/// <summary>
/// Base class for all the shooter enemies and hazards
/// </summary>
public abstract class ShootingEnemy : MonoBehaviour
{
    /// <summary>
    /// Projectile type this enemy uses
    /// </summary>
    public EnemyProjectileType projectileType;
    
    /// <summary>
    /// Direction towards the enemy is going to shoot
    /// </summary>
    public ProjectileDirection projectileDirection;

    /// <summary>
    /// Vector2 version of "projectileDirection"
    /// </summary>
    /// <see cref="projectileDirection"/>
    private Vector2 vectorizedProjectileDirection;
    
    /// <summary>
    /// Time between each attack
    /// </summary>
    public float attackSpeed;
    
    /// <summary>
    /// Time elapsed since the last attack
    /// </summary>
    private float timeElapsed;
    
    /// <summary>
    /// Position form which the projectile has to be shot
    /// </summary>
    protected Vector3 projectileOffsetShootingPosition;

    
    protected void Start()
    {
        vectorizedProjectileDirection = GetProjectileDirectionVector();
    }

    protected void Update()
    {
        if (timeElapsed > attackSpeed)
        {
            StartCoroutine(AttackBehaviour());
            timeElapsed = 0f;
        }
        timeElapsed += Time.deltaTime; 
    }

    /// <summary>
    /// Attack behaviour of each shooting enemy
    /// </summary>
    /// <returns> Time period to wait</returns>
    protected abstract IEnumerator AttackBehaviour();

    /// <summary>
    /// Shoots a projectile of the enemy's specified type retrieved from the projectile pool
    /// </summary>
    /// <param name="shootPosition">The position where the projectile is going to be shot </param>
    /// <see cref="EnemyProjectilePool"/>
    protected void Shoot(Vector2 shootPosition)
    {
        GameObject projectile = EnemyProjectilePool.I.GetProjectile(projectileType);
        Debug.Log(vectorizedProjectileDirection);
        projectile.GetComponent<EnemyProjectile>().SetUpForShooting(vectorizedProjectileDirection, shootPosition);
    }

    /// <summary>
    /// Getter to transform the "projectileDirection" enum type into Vector2
    /// </summary>
    /// <returns>Vector2 representation of shooting direction</returns>
    private Vector2 GetProjectileDirectionVector()
    {
        switch (projectileDirection)
        {
            default:
            case ProjectileDirection.Up:
                return Vector2.up;
            
            case ProjectileDirection.Down:
                return Vector2.down;
            
            case ProjectileDirection.Left:
                return Vector2.left;
            
            case ProjectileDirection.Right:
                return Vector2.right;
        }
    }
    
    public enum ProjectileDirection
    {
        Up,
        Down,
        Right,
        Left
    }
    
    
}
