using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class PlantBehaviour : MonoBehaviour, IKillable
{
    private static class PlantAnimations
    {
        public static readonly string Idle = "PlantIdle";
        public static readonly string Attack = "PlantAttack";
        public static readonly string Hit = "PlantHit";
    }

    public EnemyProjectileType ProjectileType;
    
    private const float _attackAnimationTime = 0.4f;
    

    private Animator _animator;
    [SerializeField]private float _attackCycleTime = 3f;
    private float _timeElapsed;
    private bool _hit;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (_hit)
            return;
        
        if (_timeElapsed > _attackCycleTime)
        {
            StartCoroutine(AttackBehaviour()); 
            _timeElapsed = 0f;
        }
        _timeElapsed += Time.deltaTime;
    }

    private IEnumerator AttackBehaviour()
    {
        _animator.Play(PlantAnimations.Attack);
        yield return new WaitForSeconds(_attackAnimationTime);

        //If the enemy gets hit while "charging" the attack, they won't do it
        if (_hit) yield break;
        
        GameObject projectile = EnemyProjectilePool.I.GetProjectile(ProjectileType);
        //TODO: solve this race condition in an elegant way
        projectile.SetActive(false);
        
        Bean beanScript = projectile.GetComponent<Bean>();
        //The direction of the sprite is inverted (looking to the left originally) so I have to invert the direction
        int direction = (int)(Math.Abs(transform.localScale.x) / transform.localScale.x) * -1;
        beanScript.Direction = direction;
        projectile.transform.position = new Vector2(transform.position.x + (0.5f*direction), transform.position.y+0.1f);
        
        projectile.SetActive(true);
        AudioManager.Instance.Play(gameObject,SoundList.EnemyShootProjectile);
        _animator.Play(PlantAnimations.Idle);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        IKillable killableComponent = col.gameObject.GetComponent<IKillable>();
        killableComponent?.Kill(gameObject);
    }

    public void Kill(GameObject other)
    {
        GameplayEvents.EnemyKilled?.Invoke();
        
        _hit = true;
        _animator.Play(PlantAnimations.Hit);
        GetComponent<SpriteRenderer>()
            .DOFade(0, 2f)
            .OnComplete(() => Destroy(gameObject));
    }
}
