using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class gamemanager : MonoBehaviour
{
    // Singleton instance for global access
    public static gamemanager instance;
    [Header("Menus")]
    // Currently active menu (null when no menu is open)
    [SerializeField] public GameObject menuActive;

    // Menu GameObjects for diffrent game states
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuSettings;
    // I added this one so that I could define the buttons for the main menu. - Tuff Genda
    [SerializeField] GameObject menuMain;
    [SerializeField] GameObject menuCredits;

    // I changed this to allow enemies, waves, and items to be tracked by the UI. - Tuff Genda
    [Header("Waves, Enemies, and Items left in level")]
    // UI Elements for game information
    [SerializeField] TMP_Text WavesLeftText;
    [SerializeField] TMP_Text EnemiesText;
    [SerializeField] TMP_Text ItemsLeftText;

    [Header("Player UI Settings")]
    // Player UI Elements
    public Image playerHPBar;
    public Image playerStaminaBar;
    public Image reloadBar;
    public TMP_Text ammoCountUI;
    public GameObject currentWaypoint;
    public GameObject playerDamageScreen;

    [Header("Player and Enemy Spawn")]
    // Public spawn object
    public GameObject playerSpawnPOS;

    // I added this enemy spawn object so that Elijah's enemy manager does not use playerSpawnPOS. - Tuff Genda
    //public GameObject enemySpawnPOS;

    // Player Healing
    public GameObject playerHealScreen;

    // Shop Interaction
    public GameObject buttonInteract; // 'E' To Interact

    // Player refrences
    public GameObject player;
    public playerController playerScript;
    public GameObject checkpointPopup;

    [Header("Is Game Paused")]
    // Game State tracking
    public bool isPaused;

    //Time Management
    float timeScaleOrig;

    //Temporary Patch Variables
    public bool shopOpen;

    // This tells gamemanager whether the player is in the main menu or the pause menu. - Tuff Genda
    public bool onTitle;

    // I changed this to be able to track waves, enemies, and items. - Tuff Genda
    private int waveCount;
    private int enemyCount;
    private int itemCount;

    // I added this so that waveManager could find the enemy count. - Tuff Genda
    public int totalEnemyCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        timeScaleOrig = Time.timeScale;

        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();

        // I added these for the spawning of players. - Tuff Genda
        playerSpawnPOS = GameObject.FindGameObjectWithTag("Player Spawnpoint");
    }



    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Cancel"))
        {
            
           if (menuActive == null && !shopOpen)
           {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(true);
           }
            else if (menuActive == menuPause)
            {
                stateUnpause();
            }
        }

    }

    public void statePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void stateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // This happens when anything unpasues the game since anytime the title screen is up, it pauses the game. - Tuff Genda
        onTitle = false;

        if(menuActive != null)
        {
            menuActive.SetActive(false);
            menuActive = null;
        }
    }

    // I split UpdateGameGoal into three separate functions for use with waveManager and LevelManager. That way we can keep items
    // as a game goal and not enemies. - Tuff Genda
    public void updateWaves(int amount)
    {
        waveCount += amount;

        WavesLeftText.text = waveCount.ToString("F0");
    }

    public void updateEnemies(int amount)
    {
        enemyCount += amount;

        totalEnemyCount = enemyCount;

        EnemiesText.text = totalEnemyCount.ToString("F0");
    }

    public void updateItems(int amount)
    {
        itemCount += amount;

        ItemsLeftText.text = itemCount.ToString("F0");

        if (levelManager.instance.GetItemsCollected() >= levelManager.instance.GetRequiredItems())
        {
            levelManager.instance.NextLevel();
        }
    }

    // This allows for the title screen to appear when starting the game. - Tuff Genda
    public void titleMenu()
    {
        statePause();

        menuActive = menuMain;
        menuActive.SetActive(true);

        onTitle = true;
    }

    // This allows for seeing the credits of the game. - Tuff Genda
    public void credits()
    {
        statePause();

        menuActive = menuCredits;
        menuActive.SetActive(true);
    }

    public void youLose()
    {
        statePause();

        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    public void youWin()
    {
        statePause();

        menuActive = menuWin;
        menuActive.SetActive(true);
    }

    public void openSettings()
    {
        if (menuActive != null)
        {
            menuActive.SetActive(false);
        }
        menuActive = menuSettings;
        menuActive.SetActive(true);
    }

    public void closeMenu()
    {
        // I added the credits part sos that this worked with the credits. - Tuff Genda
        if (menuActive == menuSettings || menuActive == menuCredits)
        {
            menuActive.SetActive(false);

            // I changed this to check if the player is on the title screen. If so, then redirect them there. If not, then switch
            // to the pause menu instead. - Tuff Genda
            if (onTitle)
            {
                menuActive = menuMain;
            }
            else
            {
                menuActive = menuPause;
            }
            menuActive.SetActive(true);
        }

    }
}