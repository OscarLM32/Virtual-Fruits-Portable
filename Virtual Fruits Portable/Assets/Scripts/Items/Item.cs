using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class Item : MonoBehaviour
{
    public ItemSO ItemData;
    
    private const string CollectionAnimation = "ItemCollectionAnimation";
    private const float CollectionAnimationTime = 0.5f;

    [SerializeField]private ItemType _itemType;
    private Animator _animator;
    [SerializeField]private SpriteRenderer _spriteRenderer;

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
        GetComponent<Collider2D>().enabled = false;
        _animator.Play(CollectionAnimation);
        AudioManager.Instance.Play(gameObject,SoundList.ItemPickUp);
        //Tell the SaveLoadSystem that I have been picked
        GameplayEvents.ItemPicked?.Invoke();
        //Destroy the gameObject after playing the animation;
        yield return new WaitForSeconds(CollectionAnimationTime);
        Destroy(gameObject);
    }
}