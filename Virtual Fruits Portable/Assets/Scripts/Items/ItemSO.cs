using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Item")]
public class ItemSO : ScriptableObject
{
    public ItemType ItemType;
    public Sprite ItemSprite;
}
