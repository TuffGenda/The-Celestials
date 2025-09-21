using UnityEngine;
using UnityEngine.SceneManagement;

public class levelManager : MonoBehaviour
{
    // Singleton instance for global access
    public static levelManager instance;

    [Header("Level Configuration")]
    [SerializeField] int currentLevel; // Current level player is on
    [SerializeField] int maxLevels; // Total number of levels in the game

    // I removed the enemies from this enirely that way wave manager handles the enemies and this handles the items instead - Tuff Genda
    [Header("Current Progress")]
    [SerializeField] int itemsCollected; // Items collected in current level

    // Flag to check if current level objectives are complete
    public bool levelComplete = false;
    private bool readyForNextLevel = false;

    // I added the requiredItemsPerLevel here for it's own thing.  - Tuff Genda
    private int requiredItemsPerLevel = 0; // Items needed for each level

    public bool goodEnding;
    public bool atEnding;

    // Initialize singleton pattern
    void Awake()
    {
        // Singleton pattern - only allow one instance
        if (instance == null)
        {
            instance = this;

            goodEnding = false;
        }
        else
        {
            // Destroy duplicate instances
            Destroy(gameObject);
        }
    }

    // Initialize level progress on start

    // Check level completion every frame
    void Update()
    {
        CheckLevelCompletion();
    }

    // Called by pickup system when player collects an item
    public void CollectItem()
    {
        itemsCollected++;
        CheckLevelCompletion();
    }

    // Check if level objectives are complete and unlock elevator if so
    void CheckLevelCompletion()
    {
        if (!levelComplete)
        {
            // Get requirements for current level
            int requiredItems = requiredItemsPerLevel;

            // Check if all objectives are met
            if (itemsCollected >= requiredItems)
            {
                levelComplete = true;
            }
        }
    }

    public void CheckEnding()
    {
        int numCollects = 0;

        atEnding = true;

        foreach (bool collectible in gamemanager.instance.sendCollectibleData())
        {
            if (collectible)
            {
                ++numCollects;
            }
        }

        if (numCollects >= 5)
        {
            goodEnding = true;
        }
    }

    // Progress to the next level or complete the game
    public void NextLevel()
    {
        if (levelComplete && currentLevel < maxLevels && !atEnding && !readyForNextLevel)
        {
            gamemanager.instance.levelEnd();

            readyForNextLevel = true;
        }
        else if (currentLevel >= maxLevels)
        {
            // Game completed
            if (goodEnding)
            {
                gamemanager.instance.youWinGood();
            }
            else
            {
                gamemanager.instance.youWinBad();
            }
        }
        else if (readyForNextLevel)
        {
            // Move to next level
            currentLevel++;
            itemsCollected = 0;
            levelComplete = false;

            // Load next scene or reset level
            gameData data = gamemanager.instance.playerScript.givePlayerData();
            saveManager.instance.SaveGame(data);
            SceneManager.LoadScene(currentLevel);
        }
    }

    // Update UI elements showing current progress
    

    // I added this function to update the required items automatically. - Tuff Genda
    public void updateRequiredItems()
    {
        ++requiredItemsPerLevel;
    }

    // Get current level number
    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    // Get number of items collected in current level
    public int GetItemsCollected()
    {
        return itemsCollected;
    }

    // Get number of items required for current level
    public int GetRequiredItems()
    {
        return requiredItemsPerLevel;
    }
}