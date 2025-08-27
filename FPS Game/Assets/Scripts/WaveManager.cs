using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("--- Wave Settings ---")]
    [SerializeField] Wave[] waves;
    [SerializeField] float timeBetweenWaves = 5f;

    private int currentWaveIndex = 0;
    private bool isSpawning = false;

    private bool gameManagerReady = false;

    void Start()
    {
        StartCoroutine(WaitForGameManager());
    }

    IEnumerator WaitForGameManager()
    {
        while (gamemanager.instance == null)
        {
            yield return null;
        }
        gameManagerReady = true;

        if (waves.Length > 0)
        {
            StartCoroutine(StartWave());
        }
    }

    void Update()
    {
        if (gameManagerReady)
        {
            if (gamemanager.instance.gameGoalCount <= 0 && !isSpawning)
            {
                currentWaveIndex++;
                if (currentWaveIndex < waves.Length)
                {
                    StartCoroutine(StartWave());
                }
                else
                {
                    gamemanager.instance.youWin();
                }
            }
        }
    }

    IEnumerator StartWave()
    {
        isSpawning = true;

        // This is the key change: Wait for playerSpawnPOS to be assigned.
        while (gamemanager.instance.playerSpawnPOS == null)
        {
            yield return null;
        }

        if (gamemanager.instance != null)
        {
            gamemanager.instance.updateGameGoal(waves[currentWaveIndex].enemiesToSpawn.Length);
        }

        yield return new WaitForSeconds(timeBetweenWaves);

        for (int i = 0; i < waves[currentWaveIndex].enemiesToSpawn.Length; i++)
        {
            Instantiate(waves[currentWaveIndex].enemiesToSpawn[i], gamemanager.instance.playerSpawnPOS.transform.position, Quaternion.identity);

            yield return new WaitForSeconds(waves[currentWaveIndex].timeBetweenEnemies);
        }

        isSpawning = false;
    }
}

[System.Serializable]
public class Wave
{
    public GameObject[] enemiesToSpawn;
    public float timeBetweenEnemies;
}