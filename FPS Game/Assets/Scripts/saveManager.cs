using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class saveManager : MonoBehaviour
{
    string saveFilePath => Application.persistentDataPath + "/saveFile.json";
    public static saveManager instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void SaveGame(gameData data)
    {
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Game Saved to " + saveFilePath);
    }

    public gameData LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            gameData data = JsonUtility.FromJson<gameData>(json);
            Debug.Log("Game Loaded from " + saveFilePath);
            return data;
        }
        else
        {
            Debug.LogWarning("No save file found at " + saveFilePath);
            return null;
        }
    }

    public void DeleteSave()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            Debug.Log("Save file deleted at " + saveFilePath);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            gamemanager.instance.stateUnpause();
        }
        else
        {
            Debug.LogWarning("No save file found to delete at " + saveFilePath);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            gamemanager.instance.stateUnpause();
        }
    }
}

public class gameData
{
    public int playerLevel; // as in unlocked levels (1-8 for what levels are unlocked. 1 is nothing, 8 is everything)
    public int health;
    public List<gunStats> gunList;
    public int money;
}
