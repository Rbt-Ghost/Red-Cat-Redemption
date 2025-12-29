using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int maxEnemies = 20;
    [SerializeField] private float minSpawnInterval = 5f;
    [SerializeField] private float maxSpawnInterval = 15f;

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
    private bool isSpawning = true;

    void Start()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy Prefab is not assigned to EnemySpawner!");
            enabled = false;
            return;
        }

        // Create container for enemies if not assigned
        if (enemiesContainer == null)
        {
            GameObject container = new GameObject("Enemies");
            enemiesContainer = container.transform;
        }

        // Set first spawn time
        nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    void Update()
    {
        if (!isSpawning) return;

        // Clean up null references (destroyed enemies)
        CleanupDestroyedEnemies();

        // Check if it's time to spawn and if we haven't reached max enemies
        if (Time.time >= nextSpawnTime && spawnedEnemies.Count < maxEnemies)
        {
            TrySpawnEnemy();
            // Schedule next spawn
            nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
        }
    }

    void TrySpawnEnemy()
    {
        Vector2 spawnPosition;
        bool validPositionFound = false;

        // Try to find a valid spawn position
        for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            spawnPosition = GetRandomSpawnPosition();

            if (IsValidSpawnPosition(spawnPosition))
            {
                SpawnEnemy(spawnPosition);
                validPositionFound = true;
                break;
            }
        }

        if (!validPositionFound)
        {
            Debug.LogWarning("Could not find valid spawn position after " + maxSpawnAttempts + " attempts");
        }
    }

    Vector2 GetRandomSpawnPosition()
    {
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);
        return new Vector2(randomX, randomY);
    }

    bool IsValidSpawnPosition(Vector2 position)
    {
        // Check if there's a wall at this position
        Collider2D hit = Physics2D.OverlapCircle(position, spawnCheckRadius, wallLayer);

        if (hit != null)
        {
            // Position is blocked by a wall
            return false;
        }

        return true;
    }

    void SpawnEnemy(Vector2 position)
    {
        GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity, enemiesContainer);
        spawnedEnemies.Add(enemy);

        Debug.Log($"Enemy spawned at {position}. Total enemies: {spawnedEnemies.Count}/{maxEnemies}");
    }

    void CleanupDestroyedEnemies()
    {
        // Remove null references from the list (destroyed enemies)
        spawnedEnemies.RemoveAll(enemy => enemy == null);
    }

    // Public methods for external control
    public void StartSpawning()
    {
        isSpawning = true;
        nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    public void StopSpawning()
    {
        isSpawning = false;
    }

    public void ClearAllEnemies()
    {
        foreach (GameObject enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        spawnedEnemies.Clear();
    }

    public int GetCurrentEnemyCount()
    {
        CleanupDestroyedEnemies();
        return spawnedEnemies.Count;
    }

    public int GetMaxEnemies()
    {
        return maxEnemies;
    }

    // Gizmos for visualizing spawn area
    void OnDrawGizmos()
    {
        // Draw spawn area bounds
        Gizmos.color = Color.yellow;

        Vector3 bottomLeft = new Vector3(minX, minY, 0);
        Vector3 bottomRight = new Vector3(maxX, minY, 0);
        Vector3 topRight = new Vector3(maxX, maxY, 0);
        Vector3 topLeft = new Vector3(minX, maxY, 0);

        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);

        // Draw center of spawn area
        Vector3 center = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, 0);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(center, 1f);
    }

    void OnDrawGizmosSelected()
    {
        // Draw all current enemy positions
        if (Application.isPlaying && spawnedEnemies != null)
        {
            Gizmos.color = Color.red;
            foreach (GameObject enemy in spawnedEnemies)
            {
                if (enemy != null)
                {
                    Gizmos.DrawWireSphere(enemy.transform.position, 0.5f);
                }
            }
        }
    }
}