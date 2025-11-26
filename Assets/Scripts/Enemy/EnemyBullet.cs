using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyBullet : MonoBehaviour
{
    private float damage;
    private float range;
    private Vector3 startPosition;

    [SerializeField]
    private float bulletSpeed = 10f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D not found on EnemyBullet!");
        }

        // No gravity
        rb.gravityScale = 0f;

        // Store the starting position to calculate distance traveled
        startPosition = transform.position;
    }

    public void Initialize(float bulletDamage, float bulletRange, Vector2 direction)
    {
        damage = bulletDamage;
        range = bulletRange;

        // Set the bullet's velocity based on direction
        if (rb != null)
        {
            rb.linearVelocity = direction.normalized * bulletSpeed;
        }
    }

    void Update()
    {
        // Check if bullet has traveled beyond its range
        float distanceTraveled = Vector3.Distance(transform.position, startPosition);

        if (distanceTraveled >= range)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Destroy bullet only if it hits a wall with non-trigger collider
        if (collision.CompareTag("Wall") && !collision.isTrigger)
        {
            Destroy(gameObject);
        }
        // Apply damage to player
        else if (collision.CompareTag("Player"))
        {
            PlayerStats playerStats = collision.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Destroy bullet if it hits a wall with non-trigger collider
        if (collision.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
        // Apply damage to player
        else if (collision.gameObject.CompareTag("Player"))
        {
            PlayerStats playerStats = collision.gameObject.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}