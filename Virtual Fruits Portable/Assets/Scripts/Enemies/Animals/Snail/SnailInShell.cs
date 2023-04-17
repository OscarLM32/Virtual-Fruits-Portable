using System.Collections;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// State of the Snail state machine that handles the logic when the snail is inside the shell
/// </summary>
public class SnailInShell : MonoBehaviour
{
    /// <summary>
    /// Static class containing the animation to be played in the "SnailInShell" state
    /// </summary>
    private static class SnailInShellAnimations
    {
        public static readonly string ShellIdle  = "ShellIdle";
        public static readonly string ShellOut  = "ShellOut";
        public static readonly string ShellHit  = "ShellHit";
    }
    
    /// <summary>
    /// Context of the whole StateMachine
    /// </summary>
    private SnailStateMachine _context;
    
    private float _hitTime = 1;
    private float _timeElapsed = 0;
    private bool _hit = false;

    private void Awake()
    {
        _context = GetComponent<SnailStateMachine>();
    }

    private void Update()
    {
        if(_hit)
            HandleHit();
        else if (!_context.HazardInTrigger)
            StartCoroutine(Exit());
    }
    
    /// <summary>
    /// Handles the behaviour when the snail is collided by something
    /// </summary>
    private void HandleHit()
    {
        if (_timeElapsed > _hitTime)
        {
            _timeElapsed = 0;
            _context.SnailAnimator.Play(SnailInShellAnimations.ShellIdle);
            _hit = false;
            return;
        }
        _timeElapsed += Time.deltaTime;
        _context.SnailAnimator.Play(SnailInShellAnimations.ShellHit);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        //TODO: High dependency detected make it abstract or general. Make an IBounceBack interface or something 
        PlayerCollisionManager playerCollisionManager = col.gameObject.GetComponent<PlayerCollisionManager>();
        if (playerCollisionManager != null)
            StartCoroutine(playerCollisionManager.BounceBack(gameObject));
    }

    /// <summary>
    /// Handles the logic when the Snail switches from the "InState" to "OutState"
    /// </summary>
    /// <returns>Wait time</returns>
    /// <seealso cref="SnailOutShell"/>
    private IEnumerator Exit()
    {
        //Play the coming out animation
        _context.SnailAnimator.Play(SnailInShellAnimations.ShellOut);
        yield return new WaitForSeconds(_context.ShellOutAnimationTime);
        
        //Continue patrolling
        DOTween.Play(_context.PatrolId);
        //Change the current state to "ShellOutState"
        _context.ChangeState(_context.ShellOutState);
    }


}
