using System;
using UnityEngine;

/// <summary>
/// Handles the logic behind setting up the audio sources and playing the audios tracks of any GameObject
/// </summary>
public class SoundPlayer : MonoBehaviour
{
    /// <seealso cref="SoundList"/>
    public SoundList soundList;

    private void Awake()
    {
        SetUpSounds();
    }

    /// <summary>
    /// Creates a child GameObject that will contain the audio sources and their set up of the different Sounds
    /// from the list specified in "soundList"
    /// </summary>
    /// <remarks>If no audio source component is added to the new GameObject it will be destroyed</remarks>
    /// <seealso cref="SoundList"/>
    private void SetUpSounds()
    {
        GameObject soundObject = new GameObject("Sounds");
        soundObject.transform.SetParent(transform);
        soundObject.transform.position = new Vector3(0,0,0);

        foreach (var sound in soundList.sounds)
        {
            if(sound.disposable)
                continue;
            
            sound.source = soundObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.loop = sound.loop;
            sound.source.spatialBlend = sound.spatialBlend;
        }
        
        if(soundObject.GetComponent<AudioSource>() == null)
            Destroy(soundObject);
    }

    #region SoundPlayMethods
    
    /// <summary>
    /// Plays the sound with the name specified in the 
    /// </summary>
    /// <param name="clipName">The name of the Sound</param>
    /// <seealso cref="Sound"/>
    public void Play(string clipName)
    {
        Sound sound = GetSound(clipName);
        //Null checker before calling
        sound?.source.Play();
    }

    /// <summary>
    /// Plays a disposable sound (destroyed after playing) at a certain position specified.
    /// </summary>
    /// <param name="clipName">The name of the Sound</param>
    /// <param name="position">The position where to spawn the audio source</param>
    /// <seealso cref="Sound"/>
    public void PlayDisposableAtPosition(string clipName, Vector3 position)
    {
        Sound sound = GetSound(clipName);
        AudioSource.PlayClipAtPoint(sound.clip, position, sound.volume);
    }
    #endregion
    
    /// <summary>
    /// Getter of the Sound specified by its' name
    /// </summary>
    /// <param name="clipName">The name of the Sound</param>
    /// <returns>The sound with the name specified or null if there is no sound with that name</returns>
    /// <seealso cref="Sound"/>
    private Sound GetSound(string clipName)
    {
        return Array.Find(soundList.sounds, sound => sound.name == clipName);
    }
}
