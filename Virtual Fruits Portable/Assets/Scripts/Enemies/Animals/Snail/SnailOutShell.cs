using DG.Tweening;
using UnityEngine;

public class SnailOutShell : MonoBehaviour
{
    private static class SnailOutShellAnimations
    {
        public static readonly string Idle = "SnailIdle";
        public static readonly string Walk = "SnailWalk";
        public static readonly string ShellIn  = "ShellIn";
    }

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

    private void Exit()
    {
        DOTween.Pause(_context.PatrolId);
        _context.SnailAnimator.Play(SnailOutShellAnimations.ShellIn);
        _context.ChangeState(_context.ShellInState);
    }
}
