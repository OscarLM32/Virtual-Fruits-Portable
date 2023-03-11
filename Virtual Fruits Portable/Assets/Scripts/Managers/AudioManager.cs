using System;
using Player.StateMachine;
using UnityEngine;
using UnityEngine.Audio;

public enum SoundList
{
    Button,
    
    LevelTheme,
    
    PlayerStep,
    PlayerJump,
    PlayerDash,
    PlayerCollision,
    PlayerDie,
    
    EnemyBunnyJump,
    EnemyShootProjectile,
    EnemyProjectileDestroy,
    
    ItemPickUp,
    
    Victory
}

public class AudioManager : MonoBehaviour
{
    public Sound[] Sounds;

    public static AudioManager Instance;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
        }
    }

    public void Play(GameObject soundPlayer ,SoundList name)
    {
        Sound s = Array.Find(Sounds, sound => sound.Name == name);
        if ((s.Source = soundPlayer.GetComponent<AudioSource>()) == null)
        {
            s.Source = soundPlayer.AddComponent<AudioSource>();
        }
        
        s.Source.clip = s.Clip;
        s.Source.volume = s.Volume;
        s.Source.pitch = s.Pitch;
        s.Source.playOnAwake = s.PlayOnAwake;
        s.Source.loop = s.Loop;

        
        s.Source.Play();
    }

    [Serializable]
    public class Sound
    {
        public SoundList Name;
        
        public AudioClip Clip;
        
        [Range(0f,1f)]
        public float Volume;
        [Range(0.1f,3f)]
        public float Pitch;

        public bool PlayOnAwake = true;
        public bool Loop;

        [Range(0f, 1f)]
        public float SpatialBlend;
        
        [HideInInspector]
        public AudioSource Source;
    }
}
