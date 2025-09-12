using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    public void resume()
    {
        gamemanager.instance.stateUnpause();
    }


    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gamemanager.instance.stateUnpause();
    }

    public void saveButton()
    {
        gameData data = gamemanager.instance.playerScript.givePlayerData();
        saveManager.instance.SaveGame(data);
    }

    public void loadButton()
    {
        gameData data = saveManager.instance.LoadGame();
        gamemanager.instance.playerScript.loadPlayerData(data);
        gamemanager.instance.stateUnpause();
    }

    public void deleteSave()
    {
        saveManager.instance.DeleteSave();
    }

    public void quit()
    {
#if !UNITY_EDITOR
              Application.Quit();

#else
        UnityEditor.EditorApplication.isPlaying = false;

#endif
    }

    public void respawnPlayer()
    {
        gamemanager.instance.playerScript.spawnPlayer();
        gamemanager.instance.stateUnpause();
    }

    public void loadLevel(int lvl)
    {
        SceneManager.LoadScene(lvl);
        gamemanager.instance.stateUnpause();
    }

    public void openSettings()
    {
        gamemanager.instance.openSettings();
    }





}
