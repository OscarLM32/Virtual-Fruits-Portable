using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State machine of the player that handles all the logic behind the character's movement
/// </summary>
public class PlayerStateMachine : MonoBehaviour
{
    public LayerMask GroundLayer;
    /// <summary>
    /// Checker for ground collision
    /// </summary>
    public Transform GroundChecker;

    /// <summary>
    /// A reference to the weapon the character uses
    /// </summary>
    public PlayerWeapon Weapon;
    
    private PlayerBaseState _currentState;
    private PlayerStateFactory _states;
    
    private const float _gravity = -9.8f;
    
    //TODO: this variables can be deleted and make use of "LayerValues"
    private const int _playerLayer = 6;
    private const int _enemyLayer = 7;
    private const int _projectileLayer = 8;
    
    private Rigidbody2D _rb;
    private Animator _animator;

    //Horizontal movement variables
    /// <summary>
    /// Contains the different speed values for the different movement types
    /// </summary>
    private readonly Dictionary<PlayerMovementType, float> _speeds = new Dictionary<PlayerMovementType, float>(3);
    private float _currentSpeed;
    private PlayerMovementType _currentPlayerMovementType;

    /// <summary>
    /// Cool down for the "stepping" sound effect
    /// </summary>
    private const float _stepSoundCoolDown = 0.7f;
    /// <summary>
    /// The time the last step was played at
    /// </summary>
    private float _lastStepTime;

    //Jumping variables
    /// <summary>
    /// The error margin the player has when inserting jumping input
    /// </summary>
    private const float _jumpGraceTime = 0.1f;
    private const float _maxJumpTime = 0.6f;
    private const float _maxJumpHeight = 3f;
    private float _initialJumpVelocity;
    /// <summary>
    /// Stores the time at which the jump input was received
    /// </summary>
    private float _jumpPressed = -1; //More safety than 0

    //Vertical dash
    /// <summary>
    /// Teh error margin the player has when inserting the dashing input
    /// </summary>
    private const float _dashGraceTime = 0.1f;
    private const float _maxDashTime = 0.6f;
    private const float _maxDashHeight = 4.5f;
    private float _initialDashVelocity;
    private float _dashPressed = -1;
    /// <summary>
    /// Stores whether the character needs to land on the ground to be able to dash again
    /// </summary>
    private bool _requireDashReset;
    /// <summary>
    /// Is the input for the dash upwards or not
    /// </summary>
    private bool _isUpwardsDash;
    
    //Attack variables
    /// <summary>
    /// Stores if the weapon can be used again
    /// </summary>
    private bool _weaponReady = true;
    
    //Gravity
    private float _jumpingGravityFactor;
    private float _dashingGravityFactor;

    //Checkers
    private bool _isGrounded;

    //Getters and Setters
    public PlayerBaseState CurrentState{ get => _currentState; set => _currentState = value; }
    public int PlayerLayer => _playerLayer;
    public int EnemyLayerID => _enemyLayer;
    public int ProjectileLayerID => _projectileLayer;
    public Rigidbody2D Rb2D => _rb;
    public Animator PlayerAnimator => _animator;
    
    //HORIZONTAL MOVEMENT GETTER AND SETTERS
    public PlayerMovementType CurrentMovementType => _currentPlayerMovementType; 
    ////////////////////////////////////////////////////////////////////////////////////
    
    //JUMPING GETTERS AND SETTERS 
    public float JumpGraceTime => _jumpGraceTime;
    public float InitialJumpVelocity => _initialJumpVelocity;
    public float JumpPressed { get => _jumpPressed;}
    ////////////////////////////////////////////////////////////////////////////////////

    //VERTICAL DASHING GETTERS AND SETTER
    public float DashGraceTime => _dashGraceTime;
    public float InitialDashVelocity => _initialDashVelocity;
    public float DashPressed { get => _dashPressed; set => _dashPressed = value; }
    public bool RequireDashReset { get => _requireDashReset; set => _requireDashReset = value; }
    public bool IsUpwardsDash => _isUpwardsDash;
    ////////////////////////////////////////////////////////////////////////////////////
    
    //GRAVITY GETTER AND SETTERS
    public float JumpingGravityFactor => _jumpingGravityFactor;
    public float DashingGravityFactor => _dashingGravityFactor;
    ///////////////////////////////////////////////////////////////////////////////////// 

    //CHECKERS GETTER AND SETTERS
    public bool IsGrounded => _isGrounded;
    //////////////////////////////////////////////////////////////////////////////////////

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        
        //Setting up default state
        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();

        SetUpMovementSpeeds();
        SetUpJumpVariables();
        SetUpDashVariables();
    }

    /// <summary>
    /// Handles if the character is grounded or not
    /// </summary>
    private void HandleGrounded()
    {
        _isGrounded = false;
        Vector2 position = GroundChecker.position;

        if (Physics2D.OverlapBox(position, new Vector2(0.45f, 0.1f), 0, GroundLayer))
            _isGrounded = true;
    }
    
    private void FixedUpdate()
    {
        HandleGrounded();
        //Handle the velocity at which the character is running
        _rb.velocity = new Vector2(_currentSpeed, _rb.velocity.y);
        
        _currentState.UpdateState();
    }

    private void Update()
    {
        PlayStepSound();
    }
    
    /// <summary>
    /// If enough time has passed since the last step sound was played it will play it again
    /// </summary>
    private void PlayStepSound()
    {
        if (_lastStepTime + _stepSoundCoolDown <= Time.time)
        {
            //AudioManager.Instance.Play(gameObject,SoundList.PlayerStep);
            _lastStepTime = Time.time;
        }
    }
    
    /// <summary>
    /// Sets up the different values in the _speeds dictionary
    /// </summary>
    private void SetUpMovementSpeeds()
    {
        _speeds.Add(PlayerMovementType.Walk,   3.5f);
        _speeds.Add(PlayerMovementType.Run,    4.5f);
        _speeds.Add(PlayerMovementType.Sprint, 5.5f);

        _currentSpeed = _speeds[PlayerMovementType.Run];
    }
    
    /// <summary>
    /// Sets ups the jumping variables making use of Verlet's integration
    /// </summary>
    private void SetUpJumpVariables()
    {
        float timeToApex = _maxJumpTime / 2f; //The time it takes to reach the highest point
        float desiredGravity = (-2 * _maxJumpHeight) / (float) Math.Pow(timeToApex, 2);
        _initialJumpVelocity = (2 * _maxJumpHeight) / timeToApex;
        _jumpingGravityFactor = (desiredGravity / _gravity);
    }

    /// <summary>
    /// Sets up the jumping variables making use of Verlet's integration
    /// </summary>
    private void SetUpDashVariables()
    {
        float timeToApex = _maxDashTime / 2f; //The time it takes to reach the highest point
        float desiredGravity = (-2 * _maxDashHeight) / (float) Math.Pow(timeToApex, 2);
        _initialDashVelocity = (2 * _maxDashHeight) / timeToApex;
        _dashingGravityFactor = (desiredGravity / _gravity);
    } 

    /// <summary>
    /// Logic to be executed when the InputManager signals that the player has tapped
    /// </summary>
    /// <param name="pressedTime">The specific time the tapped was done</param>
    private void OnPlayerTap(float pressedTime)
    {
        _jumpPressed = pressedTime;
    }

    /// <summary>
    /// Logic to be executed when the InputManager signals that the player has tapped an enemy
    /// </summary>
    /// <param name="enemyPosition">The position where it was tapped</param>
    private void OnEnemyTap(Vector2 enemyPosition)
    {
        if (!_weaponReady)
            return;
        
        _weaponReady = false;
        Debug.Log("I attack");
        Weapon.Throw(enemyPosition);
    }

    /// <summary>
    /// Logic to be executed when the InputManager signals that the player has swiped
    /// </summary>
    /// <param name="pressedTime"> The specific time the player did the swipe</param>
    /// <param name="isSlideUp">Boolean specifying if the swipe was made upwards</param>
    private void OnPlayerSlide(float pressedTime, bool isSlideUp)
    {
        _dashPressed = pressedTime;
        _isUpwardsDash = isSlideUp;
    }
    
    /// <summary>
    /// Logic to be executed when the InputManager signals that the player has changed the acceleration
    /// </summary>
    /// <param name="newMovementType">The new type the player has changed to</param>
    private void OnAccelerationChange(PlayerMovementType newMovementType)
    {
        _currentSpeed = _speeds[newMovementType];
        _currentPlayerMovementType = newMovementType;
    }
    
    /// <summary>
    /// Logic to be executed when the PlayerWeapon signals that the weapon has been retrieved
    /// </summary>
    private void OnWeaponRetrieve()
    {
        Debug.Log("I retrieve the weapon");
        _weaponReady = true;
    }

    private void OnEnable()
    {
        InputManager.Tapped += OnPlayerTap;
        InputManager.EnemyTapped += OnEnemyTap;
        InputManager.Slided += OnPlayerSlide;
        InputManager.AccelerationChanged += OnAccelerationChange;
        Weapon.WeaponRetrieved += OnWeaponRetrieve;
    }

    private void OnDisable()
    {
        InputManager.Tapped -= OnPlayerTap;
        InputManager.EnemyTapped -= OnEnemyTap;
        InputManager.Slided -= OnPlayerSlide;
        InputManager.AccelerationChanged -= OnAccelerationChange;
        Weapon.WeaponRetrieved -= OnWeaponRetrieve;
    }
}

/// <summary>
/// Types of horizontal movement by the character
/// </summary>
public enum PlayerMovementType
{
    Walk,
    Run,
    Sprint,
} 