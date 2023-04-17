using DG.Tweening;
using UnityEngine;

/// <summary>
/// State of the Snail state machine that handles the logic when the snail is out of the shell
/// </summary>
public class SnailOutShell : MonoBehaviour
{
    /// <summary>
    /// Static class containing the name of the animation when the snail is out of the shell
    /// </summary>
    private static class SnailOutShellAnimations
    {
        public static readonly string Idle = "SnailIdle";
        public static readonly string Walk = "SnailWalk";
        public static readonly string ShellIn  = "ShellIn";
    }

    /// <summary>
    /// Context of the state machine
    /// </summary>
    private SnailStateMachine _context;
    
    private void Awake()
    {
        _context = GetComponent<SnailStateMachine>();
    }

    private void Update()
    {
        _context.SnailAnimator.Play(SnailOutShellAnimations.Walk);
        if (_context.HazardInTrigger)
            Exit();
    }

    /// <summary>
    /// Handles the logic when the snail wants to switch to "OutState" from "InState"
    /// </summary>
    /// <seealso cref="SnailInShell"/>
    private void Exit()
    {
        DOTween.Pause(_context.PatrolId);
        _context.SnailAnimator.Play(SnailOutShellAnimations.ShellIn);
        _context.ChangeState(_context.ShellInState);
    }
}
