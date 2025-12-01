using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float aggroRange = 15f;
    [SerializeField] private float movementSpeed = 3f;
    [SerializeField] private float stoppingDistance = 8f;
    [SerializeField]
    private float movementThreshold = 0.01f; //Used for detecting movement.
    [Header("References")]
    [SerializeField] private EnemyStats enemyStats;
    [SerializeField] private EnemyCombat enemyCombat;

    [SerializeField] private Animator enemyAnimator;
    private Transform player;
    private Rigidbody2D rb;
    private bool isAggro = false;
    [SerializeField]
    private AudioSource footstepAudioSource; // Audio source for footstep sounds
    private float pitchRange;

    private Vector3 lastPosition; //To track last position for movement detection
    void Awake()
    {
        lastPosition = transform.position;
    }
    void Start()
    {
        // Find the player
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("Player not found! Make sure player has 'Player' tag.");
        }

        // Get components
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

        // Calculate distance to player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Check if player is in aggro range
        if (distanceToPlayer <= aggroRange)
        {
            isAggro = true;
            MoveTowardsPlayer();
        }
        else
        {
            isAggro = false;
            StopMovement();
        }
        enemyAnimator.SetBool("isMoving", IsMoving());
        lastPosition = transform.position;
    }

    void MoveTowardsPlayer()
    {
        if (player == null || rb == null) return;

        // Calculate distance to player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Stop moving if within stopping distance (combat range)
        if (distanceToPlayer <= stoppingDistance)
        {
            StopMovement();
            return;
        }

        // Calculate direction towards player
        Vector2 direction = (player.position - transform.position).normalized;

        // Move only horizontally and vertically align with player
        Vector2 movement = new Vector2(direction.x, 0f);

        // Apply movement
        rb.linearVelocity = new Vector2(movement.x * movementSpeed, rb.linearVelocity.y);

        // Optional: Align Y position with player (smoothly)
        float targetY = player.position.y+0.5f;
        float currentY = transform.position.y;

        // If significantly misaligned on Y axis, move vertically
        if (Mathf.Abs(targetY - currentY) > 0.5f)
        {
            float yDirection = Mathf.Sign(targetY - currentY);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, yDirection * movementSpeed);
        }
        else
        {
            // Maintain current Y position
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        }
    }

    void StopMovement()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    // Gizmos to visualize the ranges in the editor
    void OnDrawGizmosSelected()
    {
        // Aggro range (green)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, aggroRange);

        // Stopping/combat range (red)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);
    }

    // Public methods to adjust settings
    public void SetAggroRange(float newAggroRange)
    {
        aggroRange = newAggroRange;
    }

    public void SetMovementSpeed(float newSpeed)
    {
        movementSpeed = newSpeed;
    }

    public void SetStoppingDistance(float newStoppingDistance)
    {
        stoppingDistance = newStoppingDistance;
    }

    public bool IsAggro()
    {
        return isAggro;
    }

    public float GetDistanceToPlayer()
    {
        if (player == null) return Mathf.Infinity;
        return Vector3.Distance(transform.position, player.position);
    }
     public bool IsMoving()
    {
        //Calculate movement based on Rigidbody2D velocity for more accurate detection
        if (rb != null)
        {
            return rb.linearVelocity.magnitude > movementThreshold;
        }
        // Fallback if there is no rigidbody
        return Vector3.Distance(transform.position, lastPosition) > movementThreshold;
    }
}