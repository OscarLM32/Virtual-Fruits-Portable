using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Behaviour of the "Bunny" enemy
/// </summary>
public class BunnyBehaviour : MonoBehaviour, IKillable
{
    /// <summary>
    /// Static class containing the animations' names of the Bunny enemy 
    /// </summary>
    private static class BunnyAnimations
    {
        public static readonly string Jump = "BunnyJump";
        public static readonly string Fall = "BunnyFall";
        public static readonly string Hit = "BunnyHit";
    }

    /// <summary>
    /// Initial direction towards where it is gonna start patrolling
    /// </summary>
    /// <remarks> 1 -> right and -1 -> left</remarks>
    public int InitialDirection = 1;
    
    /// <summary>
    /// The number of jumps in the patrol before turning around
    /// </summary>
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

    private bool _isJumping = false;
    private bool _isGrounded = true;
    
    /// <summary>
    /// Stores the logic whether the enemy has been hit by the player or not
    /// </summary> 
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
        if (_isGrounded)
            HandleJump();
        else if(_rb.velocity.y <= 0)
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
            !_isJumping)
        {
            _isGrounded = true;
        }
    }

    /// <summary>
    /// Handles the jump of the bunny by adding an initial upwards velocity calculated in "SetUpJumpVariables" and
    /// horizontal velocity in the actual direction.
    /// </summary>
    /// <seealso cref="SetUpJumpVariables"/>
    private void HandleJump()
    {
        if (!_isGrounded)
            return;

        //Necessary so the "HandleGrounded" does not return true while performing the jump
        _isJumping = true;
        
        _rb.velocity = new Vector2(_speed * _currentDirection, _initialJumpVelocity);
        transform.localScale = new Vector3(_currentDirection * -0.8f, 0.8f, 1f);
        
        _jumpCount++;

        if (_jumpCount >= JumpsLoopCount)
        {
            _currentDirection *= -1;
            _jumpCount = 0;
        }

        //AudioManager.Instance.Play(gameObject, SoundList.EnemyBunnyJump);
        _animator.Play(BunnyAnimations.Jump);
    }

    /// <summary>
    /// Handle the fall of the enemy, setting the _isJumping variable to false and play the proper animation
    /// </summary>
    private void HandleFall()
    {
        _isJumping = false;
        _animator.Play(BunnyAnimations.Fall);
    }

    /// <summary>
    /// Sets up the jump variables based on Verlet's integration method
    /// </summary>
    private void SetUpJumpVariables()
    {
        float timeToApex = _maxJumpTime / 2;
        float desiredGravity = (-2 * _maxJumpHeight) / (float) Math.Pow(timeToApex, 2);
        _initialJumpVelocity = (2 * _maxJumpHeight) / timeToApex;
        _jumpingGravityFactor = (desiredGravity / -9.8f);
    }

    
    /// <summary>
    /// Behaviour when the enemy is killed
    /// </summary>
    /// <param name="other">The GameObject that killed the enemy</param>
    /// <returns>Wait times</returns>
    private IEnumerator OnKill(GameObject other)
    {
        _hit = true;
        //Disable the collider so it does not collide with anything
        GetComponent<Collider2D>().enabled = false;
        LaunchEnemy(other);
        //Play the collision animation
        _animator.Play(BunnyAnimations.Hit);
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }
    
    /// <summary>
    /// Adds a force that throws 
    /// </summary>
    /// <param name="other"></param>
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