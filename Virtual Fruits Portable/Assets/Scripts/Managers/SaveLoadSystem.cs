using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;


public class SaveLoadSystem : MonoBehaviour
{
    private static SaveLoadSystem _instance;
    private GameData _gameDataSave;

    public static SaveLoadSystem Instance => _instance;
    public GameData GameDataSave => _gameDataSave;

    private void Awake()
    {
        _instance = this;
        _gameDataSave = new GameData();
        Load();
    }

    private void Save()
    {
        string path = Application.persistentDataPath + "/gameData.save";
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.OpenOrCreate);
        
        CheckNewMaxScore(ScoreManager.Score);
        formatter.Serialize(stream, _gameDataSave);
        stream.Close();
    }

    private void Load()
    {
        string path = Application.persistentDataPath + "/gameData.save";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            _gameDataSave = formatter.Deserialize(stream) as GameData;
            stream.Close();
        }
        else
        {
            _gameDataSave = new GameData();
            Debug.Log("The save file was not found in path: " + path);
        }
    }

    private void CheckNewMaxScore(int newScore)
    {
        int aux, newBestValue = newScore;
        for (int i = 0; i < _gameDataSave.MaxScores.Length; i++)
        {
            if (newBestValue > _gameDataSave.MaxScores[i])
            {
                aux = _gameDataSave.MaxScores[i];
                _gameDataSave.MaxScores[i] = newBestValue;
                newBestValue = aux;
            }
        }
    }

    private void OnEnable()
    {
        GameplayEvents.PlayerDeath += Save;
        GameplayEvents.LevelEndReached += Save;
    }

    private void OnDisable()
    {
        GameplayEvents.PlayerDeath -= Save;
        GameplayEvents.LevelEndReached -= Save;
    }
    
    [Serializable]
    public class GameData
    {
        public int[] MaxScores = {0,0,0,0,0};
    }
}
