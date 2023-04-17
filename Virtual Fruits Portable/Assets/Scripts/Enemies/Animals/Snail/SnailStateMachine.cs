using UnityEngine;

/// <summary>
/// State machine that handles the information and general logic of the Snail
/// </summary>
public class SnailStateMachine : MonoBehaviour
{
    /// <summary>
    /// Reference to the "SnailInShell" state of the state machine 
    /// </summary>
    /// <seealso cref="SnailInShell"/>
    public MonoBehaviour ShellInState;
    
    /// <summary>
    /// Reference to the "ShellOutShell" state of the state machine
    /// </summary>
    /// <seealso cref="SnailOutShell"/>
    public MonoBehaviour ShellOutState;
    public float ShellOutAnimationTime => _shellOutAnimationTime;
    
    //Getters and setter for the states in the machine
    public Animator SnailAnimator => _animator;
    public string PatrolId => _patrolId;
    public bool HazardInTrigger => _hazardInTrigger;
    
    private const float _shellOutAnimationTime = 0.583f / 2;
    
    private MonoBehaviour _currentState;
    
    private string _patrolId;
    private Animator _animator;
    private bool _hazardInTrigger = false;
    
    /// <summary>
    /// The snails last position used to calculate the speed at which it moves
    /// </summary>
    private Vector2 _lastPosition;
    private Vector2 _velocity = Vector2.zero;
    
    /// <summary>
    /// Rotation on the "Z" axis of the snail
    /// </summary>
    private float _zRotation;

    private void Awake()
    {
        //The snails begins out of the shell
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
        _velocity = ((Vector2)transform.position - _lastPosition) / Time.deltaTime;
        _lastPosition = transform.position;
        HandleDirection();
    }

    /// <summary>
    /// Handles the switching states logic by disabling the previous state before enabling the new one
    /// </summary>
    /// <param name="newState">The new state to be enabled</param>
    public void ChangeState(MonoBehaviour newState)
    {
        //Disable the current state so it does interfere with the new state
        if (_currentState != null)
            _currentState.enabled = false;
        
        //set the new state
        _currentState = newState;
        //Enable the new state
        _currentState.enabled = true;
    }
    
    /// <summary>
    /// Handles the rotation of the Snail moving along different surfaces in different directions having into account
    /// the initial rotation of the Snail
    /// </summary>
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
        //If the player or the weapon enter the "safety field" it enters its' shell
        if (col.gameObject.layer is (int)LayerValues.PlayerLayer or (int)LayerValues.WeaponLayer)
            _hazardInTrigger = true;
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        //If the player and the weapon leave the "safety field" it leaves its' shell 
        if (col.gameObject.layer is (int)LayerValues.PlayerLayer or (int)LayerValues.WeaponLayer)
            _hazardInTrigger = false;
    }
}
