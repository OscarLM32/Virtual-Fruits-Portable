using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    
    public AdsManager AdsManager;

    private void Start()
    {
        AudioManager.Instance.Play(gameObject, SoundList.LevelTheme);
    }

    private void OnEnable()
    {
        GameplayEvents.PlayerDeath += ExitLevel;
        GameplayEvents.LevelEndReached += ExitLevel;
    }

    private void OnDisable()
    {
        GameplayEvents.PlayerDeath -= ExitLevel;
        GameplayEvents.LevelEndReached -= ExitLevel;
    }

    private void ExitLevel()
    {
        AdsManager.PlayAd(OnAdEnd);
    }

    private void OnAdEnd()
    {
        SceneManager.LoadScene(0);
    }
    
}
