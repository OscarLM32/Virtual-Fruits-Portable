using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BunnyBehaviour : MonoBehaviour, IKillable
{
    private static class BunnyAnimations
    {
        public static readonly string Jump = "BunnyJump";
        public static readonly string Fall = "BunnyFall";
        public static readonly string Hit = "BunnyHit";
    }

    public int InitialDirection = 1;
    public int JumpsLoopCount = 3;
    public Transform GroundChecker;

    public LayerMask GroundLayer;

    private Animator _animator;
    private Rigidbody2D _rb;

    private int _currentDirection;
    private float _speed = 5;

    [SerializeField] private int _jumpCount = 0;
    private float _maxJumpHeight = 2.7f;
    private float _maxJumpTime = 0.7f;
    private float _initialJumpVelocity;
    private float _jumpingGravityFactor;

    private bool _jumping = false;
    private bool _isGrounded = true;
    private bool _hit = false;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();

        SetUpJumpVariables();

        _rb.gravityScale = _jumpingGravityFactor;
        _currentDirection = InitialDirection;
    }

    private void Update()
    {
        if (_hit)
            return;

        HandleGrounded();
        if (_rb.velocity.y >= 0)
            HandleJump();
        else
            HandleFall();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        IKillable killableComponent = col.gameObject.GetComponent<IKillable>();
        killableComponent?.Kill(gameObject);
    }


    private void HandleGrounded()
    {
        _isGrounded = false;
        Vector2 position = GroundChecker.position;
        if (Physics2D.OverlapBox(position, new Vector2(0.45f, 0.1f), 0, GroundLayer) &&
            !_jumping)
        {
            _isGrounded = true;
        }
    }

    private void HandleJump()
    {
        if (!_isGrounded)
            return;

        _jumping = true;
        
        _rb.velocity = new Vector2(_speed * _currentDirection, _initialJumpVelocity);
        transform.localScale = new Vector3(_currentDirection * -0.8f, 0.8f, 1f);
        
        _jumpCount++;

        if (_jumpCount >= JumpsLoopCount)
        {
            _currentDirection *= -1;
            _jumpCount = 0;
        }

        AudioManager.Instance.Play(gameObject, SoundList.EnemyBunnyJump);
        _animator.Play(BunnyAnimations.Jump);
    }

    private void HandleFall()
    {
        _jumping = false;
        _animator.Play(BunnyAnimations.Fall);
    }

    private void SetUpJumpVariables()
    {
        float timeToApex = _maxJumpTime / 2;
        float desiredGravity = (-2 * _maxJumpHeight) / (float) Math.Pow(timeToApex, 2);
        _initialJumpVelocity = (2 * _maxJumpHeight) / timeToApex;
        _jumpingGravityFactor = (desiredGravity / -9.8f);
    }

    private IEnumerator OnKill(GameObject other)
    {
        _hit = true;
        GetComponent<Collider2D>().enabled = false;
        LaunchEnemy(other);
        _animator.Play(BunnyAnimations.Hit);
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }
    
    private void LaunchEnemy(GameObject other)
    {
        //It must have a collider because for the time being it should only be collided by the player 
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