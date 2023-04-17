using System.Collections;
using UnityEngine;

/// <summary>
/// General class that contains the behaviour of every item to be found in the game
/// </summary>
/// <remarks>
/// The script is executed in editor mode so that the new added fruits can be visualized when they are set up
/// </remarks>
[ExecuteInEditMode]
public class Item : MonoBehaviour
{
    /// <summary>
    /// ScriptableObject that contains the specific data of an object
    /// </summary>
    public ItemSO ItemData;
    
    /// <summary>
    /// Name of the collection animation of any object
    /// </summary>
    private const string CollectionAnimation = "ItemCollectionAnimation";
    private const float CollectionAnimationTime = 0.5f;

    [SerializeField]private ItemType _itemType;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    
    private void Awake()
    {
        if (_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        
        _itemType = ItemData.ItemType;
        _animator.Play(_itemType.ToString());
        _spriteRenderer.sprite = ItemData.ItemSprite;
    }
    private IEnumerator OnTriggerEnter2D(Collider2D col)
    {
        //Disable collider that it cannot be collected more tha once
        GetComponent<Collider2D>().enabled = false;
        _animator.Play(CollectionAnimation);

        //Tell the SaveLoadSystem that I have been picked
        GameplayEvents.ItemPicked?.Invoke();
        
        //Destroy the gameObject after playing the animation;
        yield return new WaitForSeconds(CollectionAnimationTime);
        Destroy(gameObject);
    }
}