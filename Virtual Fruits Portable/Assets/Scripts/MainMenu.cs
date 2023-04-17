using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject MainMenuOptions;

    public Transform HighScores;
    


    void Start()
    {
        SetUpHighScores();
    }

    private void SetUpHighScores()
    {
        for (int i = 0; i < HighScores.childCount; i++)
        {
            string finalText = i+1 + ". ";
            finalText += SaveLoadSystem.Instance.GameDataSave.MaxScores[i].ToString();
            HighScores.GetChild(i).GetComponent<TextMeshProUGUI>().text = finalText;
        }
    }

    public void OpenMenu(GameObject menu)
    {
        //AudioManager.Instance.Play(gameObject, SoundList.Button);
        MainMenuOptions.SetActive(false);
        menu.SetActive(true);
    }

    public void CloseMenu(GameObject menu)
    {
        //AudioManager.Instance.Play(gameObject, SoundList.Button);
        menu.SetActive(false);
        MainMenuOptions.SetActive(true);
    }

    public void Play()
    {
        //AudioManager.Instance.Play(gameObject, SoundList.Button);
        SceneManager.LoadScene(1);
    }

    public void CloseApp()
    {
        //AudioManager.Instance.Play(gameObject, SoundList.Button);
        Application.Quit();
    }
    
}
