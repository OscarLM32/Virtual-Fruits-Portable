using System.Collections;
using DefaultNamespace;
using UnityEngine;

public class PlayerCollisionManager : MonoBehaviour, IKillable
{
    public PlayerStateMachine PlayerController;
    //Not the best way to use int
    public int[] LayersToIgnoreOnCollision;
    
    
    private const float _bounceBackForce = 400f;
    private const float _controlRegainTime = 0.7f;

    private void Start()
    {
        IgnoreCollisions(false);
    }

    private void AddBackForce(GameObject other)
    {
        float difference = transform.position.x - other.transform.position.x;
        int direction = difference >= 0 ? 1 : -1;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        
        rb.AddForce( new Vector2(_bounceBackForce * direction, 200));
    }

    public IEnumerator BounceBack(GameObject other)
    {
        PlayerController.enabled = false;
        AudioManager.Instance.Play(gameObject,SoundList.PlayerCollision);
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

    private IEnumerator OnKillAction()
    {
        PlayerController.enabled = false;
        AudioManager.Instance.Play(gameObject,SoundList.PlayerDie);
        yield return new WaitForSeconds(1f);
        GameplayEvents.PlayerDeath?.Invoke();
    }

    private void IgnoreCollisions(bool ignore)
    {
        foreach (var layer in LayersToIgnoreOnCollision)
        {
            Physics2D.IgnoreLayerCollision((int)LayerValues.PlayerLayer, layer, ignore);
        }
    }
}
