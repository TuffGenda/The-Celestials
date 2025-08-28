using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

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
            if (gamemanager.instance.gameGoalTotal <= 0 && !isSpawning)
            {
                currentWaveIndex++;
                if (currentWaveIndex <= waves.Length)
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
        while (gamemanager.instance.enemyTurretSpawnPOS == null || gamemanager.instance.enemyShooterSpawnPOS || gamemanager.instance.enemyMeleeSpawnPOS || gamemanager.instance.enemyAnkleSpawnPOS)
        {
            isSpawning = false;
            yield return null;
        }

        if (gamemanager.instance != null)
        {
            gamemanager.instance.updateGameGoal(waves[currentWaveIndex].enemiesToSpawn.Length);
        }

        yield return new WaitForSeconds(timeBetweenWaves);

        for (int i = 0; i < waves[currentWaveIndex].enemiesToSpawn.Length; i++)
        {
            GameObject spawnPoint = GetSpawnPointForEnemy(waves[currentWaveIndex].enemiesToSpawn[i]);
            Instantiate(waves[currentWaveIndex].enemiesToSpawn[i], spawnPoint.transform.position, Quaternion.identity);

            yield return new WaitForSeconds(waves[currentWaveIndex].timeBetweenEnemies);
        }

        isSpawning = false;
    }

    GameObject GetSpawnPointForEnemy(GameObject enemy)
    {
        string enemyName = enemy.name.ToLower();

        if (enemyName.Contains("turret"))
        {
            return gamemanager.instance.enemyTurretSpawnPOS;
        }
        else if (enemyName.Contains("melee"))
        {
            return gamemanager.instance.enemyMeleeSpawnPOS;
        }
        else if (enemyName.Contains("shooter"))
        {
            return gamemanager.instance.enemyShooterSpawnPOS;
        }
        else if (enemyName.Contains("ankle"))
        {
            return gamemanager.instance.enemyAnkleSpawnPOS;
        }

        // Default to shooter spawn if no match found
        return gamemanager.instance.enemyShooterSpawnPOS;
    }
}

[System.Serializable]
public class Wave
{
    public GameObject[] enemiesToSpawn;
    public float timeBetweenEnemies;
}