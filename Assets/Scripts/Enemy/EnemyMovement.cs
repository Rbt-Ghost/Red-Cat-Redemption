using UnityEngine;
using System.Collections.Generic;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 3f;
    [SerializeField] private float stoppingDistance = 8f;
    [SerializeField] private float movementThreshold = 0.01f;

    [Header("Pathfinding")]
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float gridCellSize = 1f;
    [SerializeField] private float updatePathInterval = 0.5f;
    [SerializeField] private float obstacleCheckRadius = 0.4f;
    [SerializeField] private float pathfindingRange = 30f; // Range for pathfinding grid

    [Header("References")]
    [SerializeField] private EnemyStats enemyStats;
    [SerializeField] private EnemyCombat enemyCombat;
    [SerializeField] private Animator enemyAnimator;
    [SerializeField] private AudioSource footstepAudioSource;

    private Transform player;
    private Rigidbody2D rb;
    private float pitchRange;
    private Vector3 lastPosition;

    // Pathfinding variables
    private Grid pathfindingGrid;
    private List<Point> currentPath;
    private int currentPathIndex;
    private float lastPathUpdateTime;
    private int gridSize;
    private Vector3 lastGridCenter;

    void Awake()
    {
        lastPosition = transform.position;
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("Player not found! Make sure player has 'Player' tag.");
        }

        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D not found on Enemy!");
        }

        if (enemyStats == null)
        {
            enemyStats = GetComponent<EnemyStats>();
        }

        if (enemyCombat == null)
        {
            enemyCombat = GetComponent<EnemyCombat>();
        }

        // Initialize pathfinding grid
        InitializePathfindingGrid();
        lastPathUpdateTime = Time.time;
    }

    void Update()
    {
        if (!enemyStats.IsAlive())
        {
            StopMovement();
            return;
        }

        // Handle footstep sound
        if (IsMoving() && enemyStats.IsAlive())
        {
            if (!footstepAudioSource.isPlaying)
            {
                pitchRange = Random.Range(0.8f, 1.2f);
                footstepAudioSource.pitch = pitchRange;
                footstepAudioSource.Play();
            }
        }
        else
        {
            if (footstepAudioSource.isPlaying)
            {
                footstepAudioSource.Stop();
            }
        }

        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Move towards player if distance is greater than stopping distance
        if (distanceToPlayer > stoppingDistance)
        {
            MoveTowardsPlayer();
        }
        else
        {
            StopMovement();
        }

        enemyAnimator.SetBool("isMoving", IsMoving());
        lastPosition = transform.position;
    }

    void InitializePathfindingGrid()
    {
        // Grid for pathfinding range
        gridSize = Mathf.CeilToInt(pathfindingRange * 2 / gridCellSize) + 10; // +10 buffer
        lastGridCenter = transform.position;

        UpdatePathfindingGrid();
    }

    void UpdatePathfindingGrid()
    {
        bool[,] walkable = new bool[gridSize, gridSize];
        Vector3 gridCenter = transform.position;

        // Verify every cell if is walkable
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                Vector3 worldPos = gridCenter + new Vector3(
                    (x - gridSize / 2) * gridCellSize,
                    (y - gridSize / 2) * gridCellSize,
                    0
                );

                //Use OverlapCircle to check for obstacles
                Collider2D hit = Physics2D.OverlapCircle(worldPos, obstacleCheckRadius, obstacleLayer);
                walkable[x, y] = (hit == null);
            }
        }

        pathfindingGrid = new Grid(gridSize, gridSize, walkable);
        lastGridCenter = gridCenter;
    }

    bool ShouldUpdateGrid()
    {
        // Update grid if enemy has moved significantly
        return Vector3.Distance(transform.position, lastGridCenter) > gridCellSize * 3;
    }

    void UpdatePathToPlayer()
    {
        if (player == null || pathfindingGrid == null) return;

        // If grid needs update, do it
        if (ShouldUpdateGrid())
        {
            UpdatePathfindingGrid();
        }

        Point start = WorldToGridPoint(transform.position);
        Point target = WorldToGridPoint(player.position);


        if (IsPointInGrid(start) && IsPointInGrid(target))
        {
            currentPath = Pathfinding.FindPath(pathfindingGrid, start, target);
            currentPathIndex = 0;

            // Debug: Show path
            if (currentPath != null && currentPath.Count > 0)
            {
                Debug.Log($"Path found with {currentPath.Count} points");
            }
            else
            {
                Debug.Log("No path found or player is unreachable");
            }
        }
    }

    Point WorldToGridPoint(Vector3 worldPos)
    {
        Vector3 gridCenter = lastGridCenter;
        int x = Mathf.RoundToInt((worldPos.x - gridCenter.x) / gridCellSize + gridSize / 2);
        int y = Mathf.RoundToInt((worldPos.y - gridCenter.y) / gridCellSize + gridSize / 2);
        return new Point(x, y);
    }

    Vector3 GridPointToWorld(Point gridPoint)
    {
        Vector3 gridCenter = lastGridCenter;
        return gridCenter + new Vector3(
            (gridPoint.x - gridSize / 2) * gridCellSize,
            (gridPoint.y - gridSize / 2) * gridCellSize,
            0
        );
    }

    bool IsPointInGrid(Point point)
    {
        return point.x >= 0 && point.x < gridSize && point.y >= 0 && point.y < gridSize;
    }

    void MoveTowardsPlayer()
    {
        if (player == null || rb == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // If within stopping distance, stop
        if (distanceToPlayer <= stoppingDistance)
        {
            StopMovement();
            return;
        }

        // Update path periodically
        if (Time.time - lastPathUpdateTime > updatePathInterval)
        {
            UpdatePathToPlayer();
            lastPathUpdateTime = Time.time;
        }

        // If we have a path, follow it
        if (currentPath != null && currentPath.Count > 0 && currentPathIndex < currentPath.Count)
        {
            Vector3 targetWorldPos = GridPointToWorld(currentPath[currentPathIndex]);
            Vector2 direction = (targetWorldPos - transform.position).normalized;

            //Move in that direction
            rb.linearVelocity = direction * movementSpeed;

            // Check if we reached the current node
            float distanceToNode = Vector2.Distance(transform.position, targetWorldPos);
            if (distanceToNode < gridCellSize * 0.5f)
            {
                currentPathIndex++;
                // If reached end of path, update path
                if (currentPathIndex >= currentPath.Count)
                {
                    UpdatePathToPlayer();
                }
            }
        }
        else
        {
            // No path found, move directly towards player with obstacle avoidance
            Vector2 direction = (player.position - transform.position).normalized;

            // Check for obstacles directly ahead
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1f, obstacleLayer);
            if (hit.collider != null)
            {
                // Obstacle detected, try to find alternative direction
                Vector2[] directions = new Vector2[]
                {
                    Quaternion.Euler(0, 0, 90) * direction, // Left
                    Quaternion.Euler(0, 0, -90) * direction, // Right
                    direction + Vector2.up * 0.5f, // Up
                    direction + Vector2.down * 0.5f // Down
                };

                foreach (Vector2 altDirection in directions)
                {
                    RaycastHit2D altHit = Physics2D.Raycast(transform.position, altDirection, 1f, obstacleLayer);
                    if (altHit.collider == null)
                    {
                        direction = altDirection;
                        break;
                    }
                }
            }

            rb.linearVelocity = direction * movementSpeed;
        }
    }

    void StopMovement()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
    // Gizmos for debugging
    void OnDrawGizmosSelected()
    {
        // Stopping/combat range (red)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);

        // Draw path if exists
        if (currentPath != null && pathfindingGrid != null)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < currentPath.Count; i++)
            {
                Vector3 worldPos = GridPointToWorld(currentPath[i]);
                Gizmos.DrawSphere(worldPos, 0.2f);

                if (i > 0)
                {
                    Vector3 prevPos = GridPointToWorld(currentPath[i - 1]);
                    Gizmos.DrawLine(prevPos, worldPos);
                }
            }

            // Draw current target node
            if (currentPathIndex < currentPath.Count)
            {
                Gizmos.color = Color.yellow;
                Vector3 targetPos = GridPointToWorld(currentPath[currentPathIndex]);
                Gizmos.DrawWireSphere(targetPos, 0.3f);
            }
        }

        // Draw grid for debugging
        if (pathfindingGrid != null && pathfindingGrid.nodes != null)
        {
            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    Vector3 worldPos = GridPointToWorld(new Point(x, y));
                    Node node = pathfindingGrid.nodes[x, y];

                    if (!node.walkable)
                    {
                        Gizmos.color = new Color(1, 0, 0, 0.2f); // Red for unwalkable
                        Gizmos.DrawCube(worldPos, Vector3.one * gridCellSize * 0.9f);
                    }
                }
            }
        }
    }


    public void SetMovementSpeed(float newSpeed) { movementSpeed = newSpeed; }
    public void SetStoppingDistance(float newStoppingDistance) { stoppingDistance = newStoppingDistance; }

    public float GetDistanceToPlayer()
    {
        if (player == null) return Mathf.Infinity;
        return Vector3.Distance(transform.position, player.position);
    }

    public bool IsMoving()
    {
        if (rb != null)
        {
            return rb.linearVelocity.magnitude > movementThreshold;
        }
        return Vector3.Distance(transform.position, lastPosition) > movementThreshold;
    }
}