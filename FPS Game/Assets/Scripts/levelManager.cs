using UnityEngine;
using UnityEngine.SceneManagement;

public class levelManager : MonoBehaviour
{
    // Singleton instance for global access
    public static levelManager instance;

    [Header("Level Configuration")]
    [SerializeField] int currentLevel; // Current level player is on
    [SerializeField] int maxLevels; // Total number of levels in the game

    [Header("Current Progress")]
    [SerializeField] int itemsCollected; // Items collected in current level
    [SerializeField] int enemiesKilled; // Enemies killed in current level

    // I commented this out since it was not needed for the items.
    /*[Header("Level Objects")]
    [SerializeField] GameObject[] requiredItems; // Array of items that can be collected*/

    // Flag to check if current level objectives are complete
    private bool levelComplete = false;

    // I made this private so that no one has to set it each time they add an enemy. - Tuff Genda
    // I also added the requiredItemsPerLevel here for it's own thing.
    private int requiredEnemiesPerLevel = 0; // Enemies to kill per level
    private int requiredItemsPerLevel = 0; // Items needed for each level

    // Initialize singleton pattern
    void Awake()
    {
        // Singleton pattern - only allow one instance
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            // Destroy duplicate instances
            Destroy(gameObject);
        }
    }

    // Initialize level progress on start
    void Start()
    {
        UpdateLevelUI();
    }

    // Check level completion every frame
    void Update()
    {
        // I added this code so that this can be a private number instead of being set every time. - Tuff Genda
        requiredEnemiesPerLevel = gamemanager.instance.gameGoalTotal;

        CheckLevelCompletion();
    }

    // Called by pickup system when player collects an item
    public void CollectItem()
    {
        itemsCollected++;
        UpdateLevelUI();
        CheckLevelCompletion();
    }

    // Called by enemy scripts when an enemy is killed
    public void EnemyKilled()
    {
        enemiesKilled++;
        UpdateLevelUI();
        CheckLevelCompletion();
    }

    // Check if level objectives are complete and unlock elevator if so
    void CheckLevelCompletion()
    {
        if (!levelComplete)
        {
            // Get requirements for current level
            int requiredItems = requiredItemsPerLevel;
            int requiredEnemies = requiredEnemiesPerLevel;

            // Check if all objectives are met
            if (itemsCollected >= requiredItems && enemiesKilled >= requiredEnemies)
            {
                levelComplete = true;
            }
        }
    }

    // Progress to the next level or complete the game
    public void NextLevel()
    {
        if (levelComplete && currentLevel < maxLevels)
        {
            // Move to next level
            currentLevel++;
            itemsCollected = 0;
            enemiesKilled = 0;
            levelComplete = false;

            // Load next scene or reset level
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else if (currentLevel >= maxLevels)
        {
            // Game completed
            Debug.Log("All levels completed!");
            gamemanager.instance.youWin();
        }
    }

    // Update UI elements showing current progress
    void UpdateLevelUI()
    {
        // Update UI elements showing progress
        Debug.Log($"Level {currentLevel}: Items {itemsCollected}, " +
                  $"Enemies {enemiesKilled}");
    }

    // I added this function to update the required items automatically. - Tuff Genda
    public void updateRequiredItems()
    {
        ++requiredItemsPerLevel;
    }

    // Return whether current level is complete
    public bool IsLevelComplete()
    {
        return levelComplete;
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

    // Get number of enemies killed in current level
    public int GetEnemiesKilled()
    {
        return enemiesKilled;
    }

    // Get number of items required for current level
    public int GetRequiredItems()
    {
        return requiredItemsPerLevel;
    }

    // Get number of enemies required for current level
    public int GetRequiredEnemies()
    {
        return requiredEnemiesPerLevel;
    }
}