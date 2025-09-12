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

    // This is the list of waves and the transform for where the spawner is located.
    [Header("Waves and Spawner")]
    [SerializeField] List<Enemies> waves = new List<Enemies>();
    [SerializeField] Transform Spawner;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // This sets the default values for each variable.
        enemyPos = 0;
        wavePos = 0;
        isSpawning = true;
    }

    // Update is called once per frame
    void Update()
    {
        // This allows for spawning only if the current index is an active wave.
        if (wavePos >= waves.Count)
        {
            isSpawning = false;
        }

        // This makes sure the spawn timer only increases when spawning is allowed.
        if (isSpawning)
        {
            spawnTimer += Time.deltaTime;
        }

        // This checks to see if spawning is allowed, if it is then it goes to a function which checks to see if there are enemies left in the
        // wave. If there are then it starts the wave timer in seconds. Once that reaches the desired time, it iterates the waves and the index
        // of enemies goes to zero.
        if (isSpawning)
        {
            NextWave();
        }

        // This spawns the enemies only if the spawn timer reaches the desired time.
        if (spawnTimer >= spawnRate)
        {
            Spawn();
        }
    }

    // This is the function that spawns the enemies.
    void Spawn()
    {
        Instantiate(waves[wavePos].enemies[enemyPos], Spawner.position, transform.rotation);
        ++enemyPos;
        spawnTimer = 0;
    }

    // This is the function that changes to a new wave through different checks.
    void NextWave()
    {
        if (enemyPos >= waves[wavePos].enemies.Count)
        {
            waveTimer += Time.deltaTime;

            if (waveTimer >= waveRate)
            {
                ++wavePos;
                enemyPos = 0;
            }
        }
    }
}
