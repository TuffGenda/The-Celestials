using UnityEngine;
using UnityEngine.SceneManagement;

public class levelManager : MonoBehaviour
{
    // Singleton instance for global access
    public static levelManager instance;

    [Header("Level Configuration")]
    [SerializeField] int currentLevel; // Current level player is on
    [SerializeField] int maxLevels; // Total number of levels in the game

    [Header("Level Requirements")]
    [SerializeField] int[] requiredItemsPerLevel; // Items needed for each level
    [SerializeField] int[] requiredEnemiesPerLevel; // Enemies to kill per level

    [Header("Current Progress")]
    [SerializeField] int itemsCollected; // Items collected in current level
    [SerializeField] int enemiesKilled; // Enemies killed in current level

    [Header("Level Objects")]
    [SerializeField] GameObject elevator; // The elevator/door object
    [SerializeField] GameObject[] requiredItems; // Array of items that can be collected

    // Flag to check if current level objectives are complete
    private bool levelComplete = false;

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
            int requiredItems = requiredItemsPerLevel[currentLevel - 1];
            int requiredEnemies = requiredEnemiesPerLevel[currentLevel - 1];

            // Check if all objectives are met
            if (itemsCollected >= requiredItems && enemiesKilled >= requiredEnemies)
            {
                levelComplete = true;
                UnlockElevator();
            }
        }
    }

    // Unlock the elevator when level is complete
    void UnlockElevator()
    {
        if (elevator != null)
        {
            // Enable elevator interaction or activate it
            elevator.SetActive(true);
            Debug.Log("Level " + currentLevel + " complete! Elevator unlocked!");
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
        Debug.Log($"Level {currentLevel}: Items {itemsCollected}/{requiredItemsPerLevel[currentLevel - 1]}, " +
                  $"Enemies {enemiesKilled}/{requiredEnemiesPerLevel[currentLevel - 1]}");
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
        return requiredItemsPerLevel[currentLevel - 1];
    }

    // Get number of enemies required for current level
    public int GetRequiredEnemies()
    {
        return requiredEnemiesPerLevel[currentLevel - 1];
    }
}
