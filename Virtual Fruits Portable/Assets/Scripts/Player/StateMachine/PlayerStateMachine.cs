using System;
using System.Collections.Generic;
using Player.StateMachine;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    public LayerMask GroundLayer;
    public Transform GroundChecker;

    public PlayerWeapon Weapon;
    
    private PlayerBaseState _currentState;
    private PlayerStateFactory _states;
    
    private const float _gravity = -9.8f;
    
    private const int _playerLayer = 6;
    private const int _enemyLayer = 7;
    private const int _projectileLayer = 8;

    private Rigidbody2D _rb;
    private Animator _animator;

    //Horizontal movement variables
    private readonly Dictionary<PlayerMovementState, float> _speeds = new Dictionary<PlayerMovementState, float>(3);
    private float _currentSpeed;
    private PlayerMovementState _currentPlayerMovementState;

    private float _stepSoundCoolDown = 0.7f;
    private float _lastStepTime;

    //Jumping variables
    private const float _jumpGraceTime = 0.1f;
    private const float _maxJumpTime = 0.7f;
    private const float _maxJumpHeight = 3f;
    private float _initialJumpVelocity;
    private float _jumpPressed = -1; //More safety than 0

    //Vertical dash
    private const float _dashGraceTime = 0.1f;
    private const float _maxDashTime = 0.5f;
    private const float _maxDashHeight = 4f;
    private float _initialDashVelocity;
    private float _dashPressed = -1;
    private bool _requireDashReset;
    private bool _isUpwardsDash;
    
    //Attack variables
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
    public PlayerMovementState CurrentMovementState => _currentPlayerMovementState; 
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
    
    private void PlayStepSound()
    {
        if (_lastStepTime + _stepSoundCoolDown <= Time.time)
        {
            AudioManager.Instance.Play(gameObject,SoundList.PlayerStep);
            _lastStepTime = Time.time;
        }
    }
    
    
    private void SetUpMovementSpeeds()
    {
        _speeds.Add(PlayerMovementState.Walk,   1.5f);
        _speeds.Add(PlayerMovementState.Run,    3.5f);
        _speeds.Add(PlayerMovementState.Sprint, 5.5f);

        _currentSpeed = _speeds[PlayerMovementState.Run];
    }
    
    private void SetUpJumpVariables()
    {
        float timeToApex = _maxJumpTime / 2f; //The time it takes to reach the highest point
        float desiredGravity = (-2 * _maxJumpHeight) / (float) Math.Pow(timeToApex, 2);
        _initialJumpVelocity = (2 * _maxJumpHeight) / timeToApex;
        _jumpingGravityFactor = (desiredGravity / _gravity);
    }

    private void SetUpDashVariables()
    {
        float timeToApex = _maxDashTime / 2f; //The time it takes to reach the highest point
        float desiredGravity = (-2 * _maxDashHeight) / (float) Math.Pow(timeToApex, 2);
        _initialDashVelocity = (2 * _maxDashHeight) / timeToApex;
        _dashingGravityFactor = (desiredGravity / _gravity);
    } 

    private void OnPlayerTap(float pressedTime)
    {
        _jumpPressed = pressedTime;
    }

    private void OnEnemyTap(Vector2 enemyPosition)
    {
        if (!_weaponReady)
            return;
        
        _weaponReady = false;
        Debug.Log("I attack");
        Weapon.Throw(enemyPosition);
    }

    private void OnPlayerSlide(float pressedTime, bool isSlideUp)
    {
        _dashPressed = pressedTime;
        _isUpwardsDash = isSlideUp;
    }

    private void OnAccelerationChange(PlayerMovementState newMovementState)
    {
        _currentSpeed = _speeds[newMovementState];
        _currentPlayerMovementState = newMovementState;
    }
    
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