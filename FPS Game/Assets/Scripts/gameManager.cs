using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class gamemanager : MonoBehaviour
{
    // Singleton instance for global access
    public static gamemanager instance;
    [Header("Menus")]
    // Currently active menu (null when no menu is open)
    [SerializeField] GameObject menuActive;

    // Menu GameObjects for diffrent game states
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuSettings;

    [Header("Enemies left to win")]
    // UI Elements for game information
    [SerializeField] TMP_Text gameGoalCountText;

    [Header("Player UI Settings")]
    // Player UI Elements
    public Image playerHPBar;
    public Image playerStaminaBar;
    public Image reloadBar;
    public TMP_Text ammoCountUI;

    public GameObject playerDamageScreen;

    [Header("Player and Enemy Spawn")]
    // Public spawn object
    public GameObject playerSpawnPOS;

    // I added this enemy spawn object so that Elijah's enemy manager does not use playerSpawnPOS. - Tuff Genda
    public GameObject enemySpawnPOS;

    // Player Healing
    public GameObject playerHealScreen;

    // Shop Interaction
    public GameObject buttonInteract; // 'E' To Interact

    // Player refrences
    public GameObject player;
    public playerController playerScript;

    [Header("Is Game Paused")]
    // Game State tracking
    public bool isPaused;

    //Time Management
    float timeScaleOrig;

    int gameGoalCount;

    // I created this so that level manager can see it and use it at the start.
    public int gameGoalTotal;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        timeScaleOrig = Time.timeScale;

        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();

        // I added these for the spawning of players and enemies.
        playerSpawnPOS = GameObject.FindGameObjectWithTag("Player Spawnpoint");
        enemySpawnPOS = GameObject.FindGameObjectWithTag("Enemy Spawnpoint");
    }



    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Cancel"))
        {
            
           if (menuActive == null)
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
        if(menuActive != null)
        {
            menuActive.SetActive(false);
            menuActive = null;
        }
    }

    public void updateGameGoal(int amount)
    {
        gameGoalCount += amount;

        // I added this so that it updates the total amount of enemies so that we do not have to keep track and update it. - Tuff Genda
        gameGoalTotal = gameGoalCount;

        gameGoalCountText.text = gameGoalCount.ToString("F0");

        if (gameGoalCount <= 0)
        {
            levelManager.instance.NextLevel();

            //You won!
            //youWin();
        }
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

    public void closeSettings()
    {
        if (menuActive == menuSettings)
        {
            menuActive.SetActive(false);
            menuActive = menuPause;
            menuActive.SetActive(true);
        }

    }


}
