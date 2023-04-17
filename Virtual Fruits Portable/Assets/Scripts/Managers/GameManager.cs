using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles the logic behind events that "disrupt" the gameplay
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Reference to an ad manager so it can play ads
    /// </summary>
    public AdsManager AdsManager;

    private void Start()
    {
        //Should the game manager handle the playing of the main theme logic?
        //AudioManager.Instance.Play(gameObject, SoundList.LevelTheme);
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
