using System;
using UnityEngine;

/// <summary>
/// Scriptable object that contains the necessary information to represent one item from the game
/// </summary>
[CreateAssetMenu(fileName = "Item")]
public class ItemSO : ScriptableObject
{
    public ItemType ItemType;
    
    /// <summary>
    /// The base sprite this item uses. 
    /// </summary>
    /// <remarks>It has only use in editor mode, since the "Item" script handles the animation of each item</remarks>
    /// <seealso cref="Item"/>
    public Sprite ItemSprite;
}

/// <summary>
/// Enumerator containing a list of all the types of items
/// </summary>
public enum ItemType
{
    Apple = 0,
    Pineapple = 1,
    Orange = 2,
    Melon = 3
} 
