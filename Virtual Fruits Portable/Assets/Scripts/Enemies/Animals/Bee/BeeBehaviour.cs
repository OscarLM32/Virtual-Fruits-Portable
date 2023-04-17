using System.Collections;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// Sets the behaviour of the "Bee" enemy
/// </summary>
/// <seealso cref="ShootingEnemy"/>
/// <seealso cref="IKillable"/>
public class BeeBehaviour : ShootingEnemy, IKillable
{
    /// <summary>
    /// Static class containing the name of the animation of the enemy meant to be accessed as an enumerator 
    /// </summary>
    private static class BeeAnimations
    {
        public static readonly string Idle = "BeeIdle";
        public static readonly string Attack = "BeeAttack";
        public static readonly string Hit = "BeeHit";
    }
    
    private Collider2D _collider;
    private Rigidbody2D _rb; 
    private Animator _animator;
    
    private const float _attackAnimationSynchronizationTime = 0.5f;  
    private const float _afterAttackAnimationSynchronizationTime = 0.16f;  
    
    private string _patrolId;
    
    /// <summary>
    /// Stores the logic whether the enemy has been hit by the player or not
    /// </summary>
    private bool _hit = false;
    

    private new void Start()
    {
        base.Start();
        _collider = GetComponent<Collider2D>();
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        
        _patrolId = GetComponent<EnemyBasicPatrolling>().patrolId;
    }

    private new void Update()
    {
        if (_hit) return;
        
        base.Update();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        IKillable killableComponent = col.gameObject.GetComponent<IKillable>();
        killableComponent?.Kill(gameObject);
    }


    protected override IEnumerator AttackBehaviour()
    {
        _animator.Play(BeeAnimations.Attack);
        yield return new WaitForSeconds(_attackAnimationSynchronizationTime);
        
        //In case the bee gets hit while mid animation, we need to cancel this logic
        if (_hit) yield break; 
        
        //The bee moves along the level, so the offset position must be calculated everytime it fires
        projectileOffsetShootingPosition = new Vector2(transform.position.x, transform.position.y - 0.5f); 
        Shoot(projectileOffsetShootingPosition);
        
        //AudioManager.Instance.Play(gameObject,SoundList.EnemyShootProjectile);
        
        yield return new WaitForSeconds(_afterAttackAnimationSynchronizationTime);
        _animator.Play(BeeAnimations.Idle);
    }

    /// <summary>
    /// Behavior of the enemy when it is killed
    /// </summary>
    /// <param name="other">The GameObject that killed this enemy</param>
    /// <returns>Wait times</returns>
    private IEnumerator OnKill(GameObject other)
    {
        _hit = true;
        //Stop patrolling
        DOTween.Pause(_patrolId);

        //Play the proper animation
        _animator.Play(BeeAnimations.Hit);

        _collider.enabled = false;

        //Launch the enemy
        LaunchEnemy(other);
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    /// <summary>
    /// Adds a force to the enemy when it is killed 
    /// </summary>
    /// <param name="other"> The GameObject that killed this enemy </param>
    private void LaunchEnemy(GameObject other)
    {
        Rigidbody2D otherRb = other.GetComponent<Rigidbody2D>();
        int launchDirection = otherRb.velocity.x > 0 ? 1 : -1;

        //TODO: add a little bit of randomness to the launch
        _rb.AddForce(new Vector2(800 * launchDirection, 550));
    }

    public void Kill(GameObject other)
    {
        GameplayEvents.EnemyKilled?.Invoke();
        StartCoroutine(OnKill(other));
    }
}