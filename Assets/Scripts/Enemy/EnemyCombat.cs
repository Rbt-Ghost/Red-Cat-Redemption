using UnityEngine;
using System.Collections;

public class EnemyCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private float range = 10f;
    [SerializeField] private float bulletDamage = 10f;
    [SerializeField] private float bulletRange = 15f;
    [SerializeField] private float bulletSpeed = 8f;
    [SerializeField] private float yOffset = 0.5f;

    [Header("Bullet Prefab")]
    [SerializeField] private GameObject enemyBulletPrefab;

    [Header("Shooting Point")]
    [SerializeField] private Transform shootingPoint;

    private Transform player;
    private bool isShooting = false;
    private Coroutine shootingCoroutine;
    [SerializeField] private EnemyStats enemyStats;

    [SerializeField]
    private AudioSource shootAudioSource;
    private float pitchRange;

    void Start()
    {
        // Find the player
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("Player not found! Make sure player has 'Player' tag.");
        }

        if (enemyBulletPrefab == null)
        {
            Debug.LogError("EnemyBullet prefab not assigned in EnemyCombat!");
        }

        if (shootingPoint == null)
        {
            shootingPoint = transform;
        }

        if (enemyStats == null)
        {
            enemyStats = GetComponent<EnemyStats>();
        }
    }

    void Update()
    {
        if (player == null || enemyBulletPrefab == null) return;

        // Calculate distance to player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Check if player is in range and start/stop shooting
        if (distanceToPlayer <= range && !isShooting && enemyStats.IsAlive())
        {
            StartShooting();
        }
        else if ((distanceToPlayer > range || !enemyStats.IsAlive()) && isShooting)
        {
            StopShooting();
        }
    }

    void StartShooting()
    {
        isShooting = true;
        shootingCoroutine = StartCoroutine(ShootingRoutine());
    }

    void StopShooting()
    {
        isShooting = false;
        if (shootingCoroutine != null)
        {
            StopCoroutine(shootingCoroutine);
            shootingCoroutine = null;
        }
    }

    IEnumerator ShootingRoutine()
    {
        while (isShooting && player != null && enemyStats.IsAlive())
        {
            // Shoot at player
            pitchRange = Random.Range(0.8f, 1.2f);
            shootAudioSource.pitch = pitchRange;
            shootAudioSource.Play();
            ShootAtPlayer();

            // Wait for random time between 2 and 8 seconds
            float randomDelay = Random.Range(2f, 8f);
            yield return new WaitForSeconds(randomDelay);
        }
    }

    void ShootAtPlayer()
    {
        PlayerStats playerStats = player.GetComponent<PlayerStats>();
         if (playerStats == null)
         {
             Debug.LogWarning("PlayerStats component not found on the player!");
            return;
        }
        
        if (player == null || enemyBulletPrefab == null || !enemyStats.IsAlive() || !playerStats.IsAlive()) return;

        // Calculate target position with Y offset
        Vector3 targetPosition = player.position + new Vector3(0f, yOffset, 0f);

        // Calculate exact direction towards target position
        Vector2 direction = (targetPosition - shootingPoint.position).normalized;

        // Instantiate bullet
        GameObject bullet = Instantiate(enemyBulletPrefab, shootingPoint.position, Quaternion.identity);
        EnemyBullet enemyBullet = bullet.GetComponent<EnemyBullet>();

        if (enemyBullet != null)
        {
            enemyBullet.Initialize(bulletDamage, bulletRange, direction);

            // Optional: Rotate bullet to face direction
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        else
        {
            Debug.LogWarning("EnemyBullet component not found on the bullet prefab!");
        }
    }

    // Gizmos to visualize the range in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    // Public methods to adjust settings
    public void SetRange(float newRange)
    {
        range = newRange;
    }

    public void SetBulletDamage(float newDamage)
    {
        bulletDamage = newDamage;
    }

    public void SetBulletRange(float newBulletRange)
    {
        bulletRange = newBulletRange;
    }

    public void SetBulletSpeed(float newSpeed)
    {
        bulletSpeed = newSpeed;
    }

    public void SetYOffset(float newYOffset)
    {
        yOffset = newYOffset;
    }
}