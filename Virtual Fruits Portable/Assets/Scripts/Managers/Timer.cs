using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI TimerText;

    public static float TimeElapsed;

    private void Start()
    {
        TimeElapsed = 0;
    }

    private void Update()
    {
        TimeElapsed += Time.deltaTime;
        FormatTimerText();
    }

    private void FormatTimerText()
    {
        string finalText = "";
        int min = (int)TimeElapsed/60, sec = (int)TimeElapsed % 60;
        
        finalText += min >= 10 ? min : "0" + min;
        finalText += ":";
        finalText += sec >= 10 ? sec : "0" + sec;

        TimerText.text = finalText;
    }
}
