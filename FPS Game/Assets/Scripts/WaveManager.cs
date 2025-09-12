using UnityEngine;
using System.Collections.Generic;

// WaveManager script created by Tuff Genda.
public class WaveManager : MonoBehaviour
{
    // This is a serialized struct for keeping a list of enemies.
    [System.Serializable]
    public struct Enemies
    {
        public List<GameObject> enemies;
    }

    // This is the list of waves and the list of transforms for where the spawners are located.
    [Header("Waves and Spawner")]
    [SerializeField] List<Enemies> waves = new List<Enemies>();
    [SerializeField] List<Transform> spawners = new List<Transform>();

    // This is the time in seconds before each wave and enemy spawns in.
    [Header("Spawn Rate for Waves and Enemies")]
    [SerializeField] float waveRate;
    [SerializeField] float spawnRate;

    // These help keep track of the index of each wave and enemy, track whether wave manager should spawn, and the timers for waves and enemies.
    private int wavePos;
    private int enemyPos;
    private bool isSpawning;
    private float spawnTimer;
    private float waveTimer;

    // These help determine how many enemies to spawn and the total enemies for tracking when to spawn a wave.
    private int spawnNumber;
    private int enemyTotal;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // This sets the default values for each variable.
        enemyPos = 0;
        wavePos = 0;
        spawnNumber = waves[wavePos].enemies.Count / spawners.Count;
        isSpawning = true;

        // This updates the wave count for the UI at the start.
        gamemanager.instance.updateWaves(waves.Count);
    }

    // Update is called once per frame
    void Update()
    {
        // This allows for spawning only if the current index is an active wave.
        if (wavePos >= waves.Count)
        {
            isSpawning = false;
        }

        // This checks to see if the number of enemies in the current wave and the spawners are both even. If they are, it then
        // just multiplies the number of enemies in the wave by itself to get the total enemies. Otherwise, it multiplies the
        // number of enemies in the current wave by the number of enemies spawned in each spawner times the number of spawners to
        // get the total enemies spawned.
        if (isSpawning)
        {
            if (waves[wavePos].enemies.Count % 2 == 0 && spawners.Count % 2 == 0)
            {
                enemyTotal = waves[wavePos].enemies.Count * waves[wavePos].enemies.Count;
            }
            else
            {
                enemyTotal = waves[wavePos].enemies.Count * (spawnNumber * spawners.Count);
            }
        }

        // This makes sure the spawn timer only increases when spawning is allowed along with the total number of enemies alive
        // being less than the total enemies spawned as well as the current index of the enemy being less than the total index.
        if (isSpawning && gamemanager.instance.totalEnemyCount < enemyTotal && enemyPos < waves[wavePos].enemies.Count)
        {
            spawnTimer += Time.deltaTime;
        }

        // This checks to see if spawning is allowed, if it is then it goes to a function which checks to see if there are enemies left in the
        // wave. If there are then it starts the wave timer in seconds. Once that reaches the desired time, it iterates the waves and the index
        // of enemies goes to zero. It also changes the spawn namber each time a new wave starts.
        if (isSpawning && gamemanager.instance.totalEnemyCount <= 0 && enemyPos >= waves[wavePos].enemies.Count)
        {
            waveTimer += Time.deltaTime;

            if (waveTimer >= waveRate)
            {
                NextWave();

                if (wavePos <  waves.Count)
                {
                    spawnNumber = waves[wavePos].enemies.Count / spawners.Count;
                }
            }
        }

        // This spawns the enemies only if the spawn timer reaches the desired time.
        if (spawnTimer >= spawnRate)
        {
            Spawn();
        }
    }

    // This is the function that spawns the enemies. It makes sure to spawn them as evenly as possible between spawners.
    void Spawn()
    {
        foreach (var spawnPos in spawners)
        {
            for (int i = 0; i < spawnNumber; i++)
            {
                Instantiate(waves[wavePos].enemies[enemyPos], spawnPos.position, spawnPos.rotation);
            }
        }

        ++enemyPos;
        spawnTimer = 0;
    }

    // This is the function that changes to a new wave through different checks.
    void NextWave()
    {
        ++wavePos;
        enemyPos = 0;
        waveTimer = 0;

        // This lowers the remaining waves by one each time for the UI.
        gamemanager.instance.updateWaves(-1);
    }
}
