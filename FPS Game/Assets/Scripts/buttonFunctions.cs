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

    // This starts a new game starting at the first level. - Tuff Genda
    public void newGame()
    {
        resume(); // This needs to be changed once we have levels to start at the tutorial. - Tuff Genda
    }

    // This loads into a level based off of a save file. - Tuff Genda
    public void loadGame()
    {
        // This needs to be added to once we have the save system so that we can laod each level. - Tuff Genda
    }

    // This loads into the credits menu. - Tuff Genda
    public void credits()
    { 
        gamemanager.instance.credits();
    }

    // This goes back to the main menu from the credits. - Tuff Genda
    public void back()
    {
        gamemanager.instance.closeMenu();
    }
}