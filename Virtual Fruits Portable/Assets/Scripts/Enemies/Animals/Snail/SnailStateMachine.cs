using DefaultNamespace;
using UnityEngine;

public class SnailStateMachine : MonoBehaviour
{
    public MonoBehaviour ShellInState;
    public MonoBehaviour ShellOutState;
    public LayerMask PlayerLayer;
    
    public float ShellInAnimationTime => _shellInAnimationTime;
    public float ShellOutAnimationTime => _shellOutAnimationTime;
    
    public Animator SnailAnimator => _animator;
    public string PatrolId => _patrolId;
    public bool HazardInTrigger => _hazardInTrigger;
    
    private const float _shellInAnimationTime = 0.583f / 3;
    private const float _shellOutAnimationTime = 0.583f / 2;
    
    private MonoBehaviour _currentState;
    
    private string _patrolId;
    private Animator _animator;
    private bool _hazardInTrigger = false;
    
    private Vector2 _lastPosition;
    private Vector2 _velocity = Vector2.zero;
    private float _zRotation;

    private void Awake()
    {
        ShellInState.enabled = false;
        _currentState = ShellOutState;
        _currentState.enabled = true;
        
        _patrolId = GetComponent<EnemyBasicPatrolling>().patrolId;
        _animator = GetComponent<Animator>();
        
        _lastPosition = transform.position;
        _zRotation = transform.eulerAngles.z % 360;
    }

    private void Update()
    {
        _velocity = (Vector2)transform.position - _lastPosition;
        _lastPosition = transform.position;
        HandleDirection();
    }

    public void ChangeState(MonoBehaviour newState)
    {
        if (_currentState != null)
            _currentState.enabled = false;
        _currentState = newState;
        _currentState.enabled = true;
    }
    
    private void HandleDirection()
    {
        if (_velocity == Vector2.zero)
            return;

        bool xPositiveVelocity = _velocity.x > 0 ? true : false;
        bool yPositiveVelocity = _velocity.y > 0 ? true : false;
        
        switch (_zRotation)
        {
            case 0:
                transform.localScale = xPositiveVelocity ? new Vector3(-3, 3, 1) : new Vector3(3, 3, 1);
                break;
            case 90:
                transform.localScale = yPositiveVelocity ? new Vector3(-3, 3, 1) : new Vector3(3, 3, 1);
                break;
            case 180:
                transform.localScale = xPositiveVelocity ? new Vector3(-3, 3, 1) : new Vector3(3, 3, 1);
                break;
            case 270:
                transform.localScale = yPositiveVelocity ? new Vector3(3, 3, 1) : new Vector3(-3, 3, 1);
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    { 
        if (col.gameObject.layer is (int)LayerValues.PlayerLayer or (int)LayerValues.WeaponLayer)
            _hazardInTrigger = true;
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.layer is (int)LayerValues.PlayerLayer or (int)LayerValues.WeaponLayer)
            _hazardInTrigger = false;
    }
}
