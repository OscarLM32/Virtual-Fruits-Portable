using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// Class handling the behaviour of the Trunk enemy
/// </summary>
public class TrunkBehaviour : ShootingEnemy, IKillable
{
    private const float _attackAnimationSynchronizationTime = 0.66f; 
    
    private Animator _animator;
    private Collider2D _collider;
    
    /// <summary>
    /// Handles the logic of whether the enemy has been hit or not
    /// </summary>
    private bool _hit;
    
    /// <summary>
    /// The direction the trunk is going to be facing to
    /// </summary>
    private int _facingDirection;
    private string _patrolId;

    private new void Start()
    {
        base.Start();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();
        
        _facingDirection = (int)(transform.localScale.x / Math.Abs(transform.localScale.x));
        _patrolId = GetComponent<EnemyBasicPatrolling>().patrolId;
    }

    protected override IEnumerator AttackBehaviour()
    {
        _animator.Play(Animations.TrunkAttack.ToString());
        //This enemy stops patrolling when it is about to shoot
        DOTween.Pause(_patrolId); 
        
        yield return new WaitForSeconds(_attackAnimationSynchronizationTime);
        
        if (_hit) yield break;
        
        Transform t = transform;
        //The trunk moves along the level, so the position to shoot from has to be calculated each time
        projectileOffsetShootingPosition = new Vector3(t.position.x + (-0.5f * _facingDirection), t.position.y - 0.05f, 0);
        Shoot(projectileOffsetShootingPosition);

        //The attack has finished, so make the trunk patrol again
        DOTween.Play(_patrolId);
        _animator.Play(Animations.TrunkRun.ToString());
    }
    
    /// <summary>
    /// Handles the behaviour of the enemy when it is killed
    /// </summary>
    /// <param name="other">The GameObject that killed this enemy</param>
    public void Kill(GameObject other)
    {
        _hit = true;
        _collider.enabled = false;
        
        GameplayEvents.EnemyKilled?.Invoke();  

        DOTween.Pause(_patrolId);
        //Play the hit animation
        _animator.Play(Animations.TrunkHit.ToString());

        //Fade the enemy before disappearing 
        GetComponent<SpriteRenderer>()
            .DOFade(0, 2f)
            .OnComplete(() => Destroy(gameObject)); 
    }

    /// <summary>
    /// Enum storing the name of the animations of this enemy
    /// </summary>
    private enum Animations
    {
        TrunkRun,
        TrunkAttack,
        TrunkHit
    }
}
