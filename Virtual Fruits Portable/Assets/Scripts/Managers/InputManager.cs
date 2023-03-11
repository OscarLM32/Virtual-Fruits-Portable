using System;
using System.Collections;
using System.Collections.Generic;
using Player.StateMachine;
using Unity.VisualScripting;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public delegate void OnTap(float time);
    public static event OnTap Tapped;

    public delegate void OnEnemyTap(Vector2 enemyPos);
    public static event OnEnemyTap EnemyTapped;

    public delegate void OnSlide(float time, bool isSlideUp);
    public static event OnSlide Slided;

    public delegate void OnAccelerationChange(PlayerMovementState playerMovementState);
    public static event OnAccelerationChange AccelerationChanged;

    public LayerMask EnemyLayer;
    
    
    private Dictionary<int, float> _currentPressedTime = new Dictionary<int, float>();

    private const float _maxTapTime = 0.15f;
    private const float _maxSlideTime = 0.5f;
    private const float _minSlideDistance = 25f;

    private PlayerMovementState _lastPlayerMovementState = PlayerMovementState.Run;
    
    
    private void Awake()
    {
        Input.gyro.enabled = true;
        for (int i = 0; i < 10; i++) //Maximum supported number of touches
        {
            _currentPressedTime.Add(i, 0);
        }
    }

    void Update()
    {
        HandleTouches();
        HandleAcceleration();
    }

    private void HandleTouches()
    {
        foreach (var touch in Input.touches)
        {
            _currentPressedTime[touch.fingerId] += Time.deltaTime;
            if (touch.phase == TouchPhase.Ended)
                OnTouchEnd(touch);
        }
    }

    private void HandleAcceleration()
    {
        var acceleration = Input.acceleration.x;
        //This is heavily dependant of how the player controller is programmed.
        if (acceleration <= -0.15f && _lastPlayerMovementState != PlayerMovementState.Sprint)
        {
            AccelerationChanged?.Invoke(PlayerMovementState.Walk);
            _lastPlayerMovementState = PlayerMovementState.Walk;
        }
        else if (acceleration is >= -0.15f and <= 0.15f && _lastPlayerMovementState != PlayerMovementState.Run)
        {
            AccelerationChanged?.Invoke(PlayerMovementState.Run);
            _lastPlayerMovementState = PlayerMovementState.Run;
        }
        else if (acceleration >= 0.15f && _lastPlayerMovementState != PlayerMovementState.Sprint)
        {
            AccelerationChanged?.Invoke(PlayerMovementState.Sprint);
            _lastPlayerMovementState = PlayerMovementState.Sprint;
        }

    }

    private void OnTouchEnd(Touch touch)
    {
        float pressedTime = _currentPressedTime[touch.fingerId];
        float travelledDistance = touch.deltaPosition.y;
        bool distanceBetweenMargins = travelledDistance is >= -_minSlideDistance and <= _minSlideDistance;
        
        if (pressedTime <= _maxTapTime && distanceBetweenMargins)
        {
            if(CheckEnemyTapped(touch.position))
            {
                Vector2 enemyPos = Camera.main.ScreenToWorldPoint(touch.position);
                EnemyTapped?.Invoke(enemyPos);
                return;
            }
            Tapped?.Invoke(Time.time);
        }
        else if (pressedTime <= _maxSlideTime && !distanceBetweenMargins)
        {
            //If the travelled distance (y axis) bigger than 0 is going up
            Slided?.Invoke(Time.time, travelledDistance > 0);
        }
        _currentPressedTime[touch.fingerId] = 0;
    }

    private bool CheckEnemyTapped(Vector2 tapPosition)
    {

        bool exit = false;
        Vector2 tapWorldPosition = Camera.main.ScreenToWorldPoint(tapPosition);
        RaycastHit2D hit = Physics2D.Raycast(tapWorldPosition, Vector2.up, 0.1f, EnemyLayer);

        if (hit.transform != null)
        {
            Debug.Log("Enemy tapped");
            exit = true;
        }

        return exit;
    }
    
}