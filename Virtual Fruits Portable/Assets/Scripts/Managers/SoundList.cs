using System;
using UnityEngine;

/// <summary>
/// An ScriptableObject containing a list of sounds from the class Sound
/// </summary>
/// <seealso cref="Sound"/>
[CreateAssetMenu(fileName = "SoundList")]
public class SoundList : ScriptableObject
{
    public Sound[] sounds;
}

/// <summary>
/// Class representing a sound effect or soundtrack to be played
/// </summary>
[Serializable]
public class Sound
{
    public string name;

    /// <summary>
    /// Represents whether the audio is going to be played continuously or if it can be disposed after playing
    /// </summary>
    public bool disposable;
        
    /// <summary>
    /// The audio track
    /// </summary>
    public AudioClip clip;
        
    [Range(0f,1f)]
    public float volume;
    
    public bool loop;
    
    [Range(0f, 1f)]
    public float spatialBlend;
        
    [HideInInspector]
    public AudioSource source;
}
