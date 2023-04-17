using System.Collections;
using UnityEngine;

/// <summary>
/// Handles the behaviour of the character when it is hit by an enemy or collides against an environmental hazard
/// </summary>
public class PlayerCollisionManager : MonoBehaviour, IKillable
{
    public PlayerStateMachine PlayerController;
    /// <summary>
    /// A list of layers that need to be ignored after being hit
    /// </summary>
    public int[] LayersToIgnoreOnCollision;

    private Animator _animator;

    private enum Animations
    {
        PlayerBounceBack,
        PlayerDeath
    }
    
    /// <summary>
    /// Force to add to the character when it gets hit
    /// </summary>
    private const float _bounceBackForce = 400f;
    
    /// <summary>
    /// Time to regain control after colliding with non-killer hazards
    /// </summary>
    private const float _controlRegainTime = 0.7f;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        IgnoreCollisions(false);
    }

    /// <summary>
    /// Makes the character be pushed to the direction contrary of the object hitting the player
    /// </summary>
    /// <param name="other">The other GameObject hitting the player</param>
    private void AddBackForce(GameObject other)
    {
        //Check if "other" is to the right or to the left of the character
        float difference = transform.position.x - other.transform.position.x;
        int direction = difference >= 0 ? 1 : -1;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 1;
        rb.velocity = Vector2.zero;
        rb.AddForce( new Vector2(_bounceBackForce * direction, 200));
    }

    /// <summary>
    /// Makes the character bounce back losing the control over the character
    /// </summary>
    /// <param name="other">The GameObject the character has collided with</param>
    /// <returns></returns>
    public IEnumerator BounceBack(GameObject other)
    {
        PlayerController.enabled = false;
        //AudioManager.Instance.Play(gameObject,SoundList.PlayerCollision);
        _animator.Play(Animations.PlayerBounceBack.ToString());
        AddBackForce(other);
        yield return new WaitForSeconds(_controlRegainTime);
        PlayerController.enabled = true;
    }

    public void Kill(GameObject other)
    {
        AddBackForce(other);
        IgnoreCollisions(true);
        StartCoroutine(OnKillAction());
    }

    /// <summary>
    /// The logic to be executed when the player is killed
    /// </summary>
    /// <returns>Wait time</returns>
    private IEnumerator OnKillAction()
    {
        PlayerController.enabled = false;
        _animator.Play(Animations.PlayerDeath.ToString());
        //AudioManager.Instance.Play(gameObject,SoundList.PlayerDie);
        yield return new WaitForSeconds(1f);
        GameplayEvents.PlayerDeath?.Invoke();
    }

    /// <summary>
    /// Sets the collisions between LayerToIgnoreOnCollision to true or false
    /// </summary>
    /// <param name="ignore">Whether the collisions have to be ignored or not</param>
    private void IgnoreCollisions(bool ignore)
    {
        foreach (var layer in LayersToIgnoreOnCollision)
        {
            Physics2D.IgnoreLayerCollision((int)LayerValues.PlayerLayer, layer, ignore);
        }
    }
}
