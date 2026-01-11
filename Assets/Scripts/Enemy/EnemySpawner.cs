using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnInterval = 2f; // Time between individual enemy spawns inside a wave

    [Header("Wave Settings")]
    private int[] waveCounts = new int[] { 5, 10, 15 }; // The 3 waves
    private int currentWaveIndex = 0;
    private int enemiesSpawnedInWave = 0;
    private bool allWavesComplete = false;

    [Header("Spawn Area")]
    [SerializeField] private float minX = -60f;
    [SerializeField] private float maxX = 30f;
    [SerializeField] private float minY = -33f;
    [SerializeField] private float maxY = 30f;

    [Header("Spawn Validation")]
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float spawnCheckRadius = 0.5f;
    [SerializeField] private int maxSpawnAttempts = 10;

    [Header("References")]
    [SerializeField] private Transform enemiesContainer;

    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private float nextSpawnTime;
    private bool isSpawningWave = true;

    void Start()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy Prefab is not assigned!");
            enabled = false;
            return;
        }

        if (enemiesContainer == null)
        {
            GameObject container = new GameObject("Enemies");
            enemiesContainer = container.transform;
        }

        Debug.Log($"Starting Wave {currentWaveIndex + 1} ({waveCounts[currentWaveIndex]} enemies)");
    }

    void Update()
    {
        if (allWavesComplete) return;

        // Remove dead enemies from list
        CleanupDestroyedEnemies();

        // Check if current wave is finished spawning AND all enemies are dead
        if (!isSpawningWave && spawnedEnemies.Count == 0)
        {
            StartNextWave();
            return;
        }

        // Logic to spawn enemies for the current wave
        if (isSpawningWave)
        {
            if (Time.time >= nextSpawnTime)
            {
                TrySpawnEnemy();
                nextSpawnTime = Time.time + spawnInterval;
            }

            // Stop spawning if we reached the count for this wave
            if (enemiesSpawnedInWave >= waveCounts[currentWaveIndex])
            {
                isSpawningWave = false;
                Debug.Log($"Wave {currentWaveIndex + 1} spawning finished. Defeat all enemies to proceed!");
            }
        }
    }

    void StartNextWave()
    {
        currentWaveIndex++;

        if (currentWaveIndex < waveCounts.Length)
        {
            // Reset for next wave
            enemiesSpawnedInWave = 0;
            isSpawningWave = true;
            Debug.Log($"Wave {currentWaveIndex} cleared! Starting Wave {currentWaveIndex + 1} ({waveCounts[currentWaveIndex]} enemies)");
        }
        else
        {
            // All waves finished
            allWavesComplete = true;
            Victory();
        }
    }

    void Victory()
    {
        Debug.Log("All waves defeated!");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TriggerVictory();
        }
    }

    void TrySpawnEnemy()
    {
        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            Vector2 pos = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));

            // Check for walls
            if (Physics2D.OverlapCircle(pos, spawnCheckRadius, wallLayer) == null)
            {
                SpawnEnemy(pos);
                return;
            }
        }
    }

    void SpawnEnemy(Vector2 position)
    {
        GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity, enemiesContainer);
        spawnedEnemies.Add(enemy);
        enemiesSpawnedInWave++;
    }

    void CleanupDestroyedEnemies()
    {
        spawnedEnemies.RemoveAll(x => x == null);
    }

    // Gizmos to see spawn area
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 bl = new Vector3(minX, minY, 0);
        Vector3 br = new Vector3(maxX, minY, 0);
        Vector3 tr = new Vector3(maxX, maxY, 0);
        Vector3 tl = new Vector3(minX, maxY, 0);
        Gizmos.DrawLine(bl, br); Gizmos.DrawLine(br, tr);
        Gizmos.DrawLine(tr, tl); Gizmos.DrawLine(tl, bl);
    }
}