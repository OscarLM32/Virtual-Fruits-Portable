using System.Collections;
using DG.Tweening;
using UnityEngine;

public class SnailInShell : MonoBehaviour
{
    private static class SnailInShellAnimations
    {
        public static readonly string ShellIdle  = "ShellIdle";
        public static readonly string ShellOut  = "ShellOut";
        public static readonly string ShellHit  = "ShellHit";
    }
    
    private SnailStateMachine _context;
    private float _hitTime = 1;
    private float _timeElapsed = 0;
    private bool _playerCollided = false;

    private void Awake()
    {
        _context = GetComponent<SnailStateMachine>();
    }

    private void Update()
    {
        if(_playerCollided)
            HandleHit();
        else if (!_context.HazardInTrigger)
            StartCoroutine(Exit());
    }
    
    private void HandleHit()
    {
        if (_timeElapsed > _hitTime)
        {
            _timeElapsed = 0;
            _context.SnailAnimator.Play(SnailInShellAnimations.ShellIdle);
            _playerCollided = false;
            return;
        }
        _timeElapsed += Time.deltaTime;
        _context.SnailAnimator.Play(SnailInShellAnimations.ShellHit);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        //TODO: Make a IBounceBack interface or something
        PlayerCollisionManager playerCollisionManager = col.gameObject.GetComponent<PlayerCollisionManager>();
        if (playerCollisionManager != null)
            StartCoroutine(playerCollisionManager.BounceBack(gameObject));
    }

    private IEnumerator Exit()
    {
        _context.SnailAnimator.Play(SnailInShellAnimations.ShellOut);
        yield return new WaitForSeconds(_context.ShellOutAnimationTime);
        DOTween.Play(_context.PatrolId);
        _context.ChangeState(_context.ShellOutState);
    }


}
