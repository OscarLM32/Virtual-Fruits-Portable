using TMPro;
using UnityEngine;

/// <summary>
/// Handles the timer of how long has the player been in the level
/// </summary>
public class Timer : MonoBehaviour
{
    /// <summary>
    /// The component that displays the time in the UI
    /// </summary>
    public TextMeshProUGUI TimerText;

    //TODO: design a way to make this time representative in the final score
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

    /// <summary>
    /// Formats the time stores in a total count of seconds into the format "minutes:seconds" 
    /// </summary>
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
