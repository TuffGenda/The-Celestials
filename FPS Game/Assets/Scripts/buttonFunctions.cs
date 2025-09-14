using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.EventSystems;

public class buttonFunctions : MonoBehaviour
{
    public void resume()
    {
        gamemanager.instance.stateUnpause();
        gamemanager.instance.menuClick.Play();
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gamemanager.instance.stateUnpause();
        gamemanager.instance.menuClick.Play();
    }

    public void saveButton()
    {
        gameData data = gamemanager.instance.playerScript.givePlayerData();
        saveManager.instance.SaveGame(data);
        gamemanager.instance.menuClick.Play();
    }

    public void loadButton()
    {
        gameData data = saveManager.instance.LoadGame();
        gamemanager.instance.playerScript.loadPlayerData(data);
        gamemanager.instance.stateUnpause();
        gamemanager.instance.menuClick.Play();
    }

    public void deleteSave()
    {
        gamemanager.instance.menuClick.Play();
        saveManager.instance.DeleteSave();
    }

    public void quit()
    {
        gamemanager.instance.menuClick.Play();
        Time.timeScale = gamemanager.instance.timeScaleOrig;
        Invoke("actuallyQuit", 0.1f);
        
    }

    public void actuallyQuit() {
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
        gamemanager.instance.menuClick.Play();
    }

    public void loadLevel(int lvl)
    {
        gamemanager.instance.menuClick.Play();
        SceneManager.LoadScene(lvl);
        gamemanager.instance.stateUnpause();
    }

    public void openSettings()
    {
        gamemanager.instance.menuClick.Play();
        gamemanager.instance.realDeleteButton.SetActive(false);
        gamemanager.instance.fakeDeleteButton.SetActive(true);
        gamemanager.instance.openSettings();
        EventSystem.current.SetSelectedGameObject(gamemanager.instance.firstButtonSettings);
    }

    // This starts a new game starting at the first level. - Tuff Genda
    public void newGame()
    {

        resume(); // This needs to be changed once we have levels to start at the tutorial. - Tuff Genda
    }

    public void revealRealDelete()
    {
        gamemanager.instance.menuClick.Play();
        gamemanager.instance.realDeleteButton.SetActive(true);
        gamemanager.instance.fakeDeleteButton.SetActive(false);
    }

    // This loads into a level based off of a save file. - Tuff Genda
    public void loadGame()
    {
        loadButton();
    }

    public void returnToTitle() {
        gamemanager.instance.menuClick.Play();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gamemanager.instance.stateUnpause();
    }
    // This loads into the credits menu. - Tuff Genda
    public void credits()
    {
        gamemanager.instance.menuClick.Play();
        gamemanager.instance.credits();
        EventSystem.current.SetSelectedGameObject(gamemanager.instance.firstButtonCredits);
    }

    // This goes back to the main menu from the credits. - Tuff Genda
    public void back()
    {
        gamemanager.instance.menuClick.Play();
        gamemanager.instance.closeMenu();
        EventSystem.current.SetSelectedGameObject(gamemanager.instance.firstButtonMain);
    }
}