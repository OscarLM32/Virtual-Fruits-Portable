using TMPro;
using UnityEngine;

/// <summary>
/// Handles the logic behind the score of the player
/// </summary>
public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI ScoreText;

    /// <summary>
    /// The number of digits the formatting of the score is going to have 
    /// </summary>
    private const int _formattingDigitNumber = 5;
    
    /// <summary>
    /// The amount of points to get when picking an item
    /// </summary>
    private const int _itemPickScore = 100;
    
    /// <summary>
    /// The amount of points to get when killing an enemy
    /// </summary>
    private const int _enemyKillScore = 300;
    
    public static int Score;
    

    private void Start()
    {
        Score = 0;
        UpdateScore(0);
    }

    private void OnEnable()
    {
        GameplayEvents.ItemPicked += OnItemPicked;
        GameplayEvents.EnemyKilled += OnEnemyKilled;
    }
    
    private void OnDisable()
    {
        GameplayEvents.ItemPicked -= OnItemPicked;
        GameplayEvents.EnemyKilled -= OnEnemyKilled;
    }

    /// <summary>
    /// Logic when an item is picked
    /// </summary>
    private void OnItemPicked()
    {
        UpdateScore(_itemPickScore);
    }

    /// <summary>
    /// Logic when an enemy is killed
    /// </summary>
    private void OnEnemyKilled()
    {
        UpdateScore(_enemyKillScore);
    }
    
    /// <summary>
    /// Adds new points to the total score and updates the display of the these points
    /// </summary>
    /// <param name="value">New score points obtained</param>
    private void UpdateScore(int value)
    {
        Score += value;
        
        //Formatting of text
        int digits = Score.ToString().Length;
        string finalText = "";

        for (int i = 0; i < _formattingDigitNumber - digits; i++)
            finalText += "0";

        finalText += Score.ToString();
        ScoreText.text = finalText;
    }
}
