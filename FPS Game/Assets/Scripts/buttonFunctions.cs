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
        //gamemanager.instance.playerScript.spawnPlayer(); // Note: I commented this out due to it causing errors. Please put this back once it is usable.
        gamemanager.instance.stateUnpause();
    }

    public void loadLevel(int lvl)
    {
        SceneManager.LoadScene(lvl);
        gamemanager.instance.stateUnpause();
    }
}
