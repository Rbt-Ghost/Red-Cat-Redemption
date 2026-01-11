using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float timeBetweenSpawns = 1.0f;
    [SerializeField] private float timeBetweenWaves = 3.0f;

    [Header("Waves")]
    // Wave 1: 5 enemies, Wave 2: 10, Wave 3: 15
    private int[] waveCounts = new int[] { 5, 10, 15 };
    private int currentWaveIndex = 0;

    // Internal State
    private int enemiesToSpawnLeft = 0; // How many left to spawn in THIS wave
    private List<GameObject> activeEnemies = new List<GameObject>();
    private bool isSpawning = false;
    private bool gameWon = false;

    [Header("Spawn Area")]
    [SerializeField] private float minX = -60f;
    [SerializeField] private float maxX = 30f;
    [SerializeField] private float minY = -33f;
    [SerializeField] private float maxY = 30f;

    [Header("Collision Check")]
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float spawnRadius = 0.5f;

    void Start()
    {
        // Auto-cleanup existing enemies to prevent count errors
        GameObject[] existingEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in existingEnemies) Destroy(enemy);

        StartWave(currentWaveIndex);
    }

    void Update()
    {
        if (gameWon) return;

        // 1. Clean list (remove dead enemies)
        activeEnemies.RemoveAll(x => x == null);

        // 2. Check Logic
        // If we are NOT spawning AND we have NO enemies left
        if (!isSpawning && activeEnemies.Count == 0 && enemiesToSpawnLeft == 0)
        {
            // Wave Cleared!
            currentWaveIndex++;

            if (currentWaveIndex < waveCounts.Length)
            {
                StartCoroutine(WaitAndStartNextWave());
            }
            else
            {
                WinGame();
            }
        }
    }

    void StartWave(int waveIndex)
    {
        isSpawning = true;
        enemiesToSpawnLeft = waveCounts[waveIndex];
        Debug.Log($"Starting Wave {waveIndex + 1} with {enemiesToSpawnLeft} enemies.");
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (enemiesToSpawnLeft > 0)
        {
            TrySpawnEnemy();
            enemiesToSpawnLeft--;
            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        isSpawning = false; // Finished spawning this wave
    }

    IEnumerator WaitAndStartNextWave()
    {
        Debug.Log("Wave Complete! Next wave in " + timeBetweenWaves + " seconds...");
        isSpawning = true; // Block victory check while waiting
        yield return new WaitForSeconds(timeBetweenWaves);
        StartWave(currentWaveIndex);
    }

    void TrySpawnEnemy()
    {
        // Try 10 times to find a valid position
        for (int i = 0; i < 10; i++)
        {
            Vector2 pos = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));

            // Check if position touches a wall
            if (Physics2D.OverlapCircle(pos, spawnRadius, wallLayer) == null)
            {
                GameObject newEnemy = Instantiate(enemyPrefab, pos, Quaternion.identity);
                activeEnemies.Add(newEnemy);
                return;
            }
        }
        Debug.LogWarning("Could not find spawn position for enemy");
    }

    void WinGame()
    {
        gameWon = true;
        Debug.Log("VICTORY!");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TriggerVictory();
        }
    }

    // Debug Lines
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector3((minX + maxX) / 2, (minY + maxY) / 2, 0), new Vector3(maxX - minX, maxY - minY, 0));
    }
}