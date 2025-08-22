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

    public GameObject playerDamageScreen;

    // Player Healing
    public GameObject playerHealScreen;

    // Player refrences
    public GameObject player;
    public playerController playerScript;

    [Header("Player Respawn Point")]
    // Player Spawn Point
    public Vector3 spawnPoint;

    [Header("Is Game Paused")]
    // Game State tracking
    public bool isPaused;

    //Time Management
    float timeScaleOrig;

    int gameGoalCount;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        timeScaleOrig = Time.timeScale;

        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Cancel"))
        {
            if(menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else if(menuActive == menuPause)
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
        menuActive.SetActive(false);
        menuActive = null;
    }

    public void updateGameGoal (int amount)
    {
        gameGoalCount += amount;

        gameGoalCountText.text = gameGoalCount.ToString("F0");

        /*if (gameGoalCount <= 0)
        {
             //You won!
             statePause ();
            menuActive = menuWin;
            menuActive.SetActive(true);
        }*/
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
