using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI ScoreText;

    private const int _formattingDigitNumber = 5;
    
    private const int _itemPickScore = 100;
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

    private void OnItemPicked()
    {
        UpdateScore(_itemPickScore);
    }

    private void OnEnemyKilled()
    {
        UpdateScore(_enemyKillScore);
    }
    
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
