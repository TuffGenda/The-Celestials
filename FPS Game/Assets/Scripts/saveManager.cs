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
        
    }

    public gameData LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            gameData data = JsonUtility.FromJson<gameData>(json);
            

            // I added this so that it loads the correct level. - Tuff Genda
            ;

            return data;
        }
        else
        {
            
            return null;
        }
    }

    public void DeleteSave()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            gamemanager.instance.stateUnpause();
        }
        else
        {
            
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            gamemanager.instance.stateUnpause();
        }
    }
}

public class gameData
{
    public int health;
    public List<gunStats> gunList;
    public int money;

    // I added this so that we can save the correct level each time. - Tuff Genda
    public int level;
    public bool[] collectibles;
}
