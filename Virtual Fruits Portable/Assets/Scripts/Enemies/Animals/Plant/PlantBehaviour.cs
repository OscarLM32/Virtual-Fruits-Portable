using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class PlantBehaviour : ShootingEnemy, IKillable
{
    /// <summary>
    /// Static class containing the name of the animation of the "Plant" enemy
    /// </summary>
    private static class PlantAnimations
    {
        public static readonly string Idle = "PlantIdle";
        public static readonly string Attack = "PlantAttack";
        public static readonly string Hit = "PlantHit";
    }
    
    private const float _attackAnimationSynchronizationTime = 0.35f;
    
    private Animator _animator;
    private bool _hit;

    private int _facingDirection; // 1 left | -1 right


    private new void Start()
    {
        base.Start();
        _animator = GetComponent<Animator>();
        _facingDirection = (int)(transform.localScale.x / Math.Abs(transform.localScale.x));
        
        //This enemy does not move form its' original position, so no need to recalculate this offset 
        var offsetPosition = new Vector3(transform.position.x + (-0.5f * _facingDirection), transform.position.y + 0.15f);
        projectileOffsetShootingPosition = offsetPosition;
    }

    private new void Update()
    {
        if (_hit) return;
        
        base.Update();
    }

    /// <summary>
    /// Attacking behaviour. After some time waiting to synchronize the animation with the attack, it shoot a projectile
    /// </summary>
    /// <returns>Wait time</returns>
    protected override IEnumerator AttackBehaviour()
    {
        _animator.Play(PlantAnimations.Attack);
        yield return new WaitForSeconds(_attackAnimationSynchronizationTime);
        
        //If the enemy gets hit while "charging" the attack, they won't do it
        if (_hit) yield break;
        
        //Shoot the projectile
        Shoot(projectileOffsetShootingPosition);
        
        //AudioManager.Instance.Play(gameObject,SoundList.EnemyShootProjectile);
        _animator.Play(PlantAnimations.Idle);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        IKillable killableComponent = col.gameObject.GetComponent<IKillable>();
        killableComponent?.Kill(gameObject);
    }

    /// <summary>
    /// Kills this enemy by blocking its' attack behaviour and fades it away before destroying it
    /// </summary>
    /// <param name="other">The GameObject that killed this enemy</param>
    public void Kill(GameObject other)
    {
        _hit = true;
        
        GameplayEvents.EnemyKilled?.Invoke();
        
        //Disable collider while in the dying animation
        GetComponent<Collider2D>().enabled = false;
        _animator.Play(PlantAnimations.Hit);
        
        GetComponent<SpriteRenderer>()
            .DOFade(0, 2f)
            .OnComplete(() => Destroy(gameObject));
    }
}
