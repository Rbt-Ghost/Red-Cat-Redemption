using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    private float damage;
    private float range;
    private float direction; // 1 for right, -1 for left
    private Vector3 startPosition;

    [SerializeField]
    private float bulletSpeed = 10f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D not found on Bullet!");
        }

        // No gravity
        rb.gravityScale = 0f;

        // Store the starting position to calculate distance traveled
        startPosition = transform.position;
    }

    public void Initialize(float bulletDamage, float bulletRange, float bulletDirection)
    {
        damage = bulletDamage;
        range = bulletRange;
        direction = bulletDirection;

        // Set the bullet's velocity based on direction
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(direction * bulletSpeed, 0f);
        }
    }

    void Update()
    {
        // Check if bullet has traveled beyond its range
        float distanceTraveled = Mathf.Abs(transform.position.x - startPosition.x);

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
        // Apply damage to enemy
        else if (collision.CompareTag("Enemy"))
        {
            EnemyStats enemyStats = collision.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                enemyStats.TakeDamage(damage);
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
        // Apply damage to enemy
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyStats enemyStats = collision.gameObject.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                enemyStats.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}