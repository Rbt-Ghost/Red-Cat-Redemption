using UnityEngine;
using System.Collections;

public class EnemyStats : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 50f;
    [SerializeField] private float currentHealth = 50f;

    [Header("Visual Effects")]
    [SerializeField] private SpriteRenderer enemySprite;
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private Color deathColor = new Color(0.3f, 0.3f, 0.3f, 1f); 
    [SerializeField] private float damageEffectDuration = 0.3f;

    private bool alive = true;
    private Color originalColor;
    private Coroutine damageEffectCoroutine;

    void Start()
    {
        // Find SpriteRenderer if it's not set
        if (enemySprite == null)
            enemySprite = GetComponent<SpriteRenderer>();

        // Save original color
        if (enemySprite != null)
            originalColor = enemySprite.color;

        currentHealth = maxHealth;
    }

    // Method to apply damage to the enemy
    public void TakeDamage(float damageAmount)
    {
        if (!alive) return;

        currentHealth -= damageAmount;
        Debug.Log("Enemy took " + damageAmount + " damage. Current health: " + currentHealth);

        // Trigger damage effect
        if (damageEffectCoroutine != null)
            StopCoroutine(damageEffectCoroutine);
        damageEffectCoroutine = StartCoroutine(DamageEffect());

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    // Method to handle enemy death
    void Die()
    {
        alive = false;
        Debug.Log("Enemy has died.");

        // Apply dead color 
        if (enemySprite != null)
            enemySprite.color = deathColor;

        // Disable collider to prevent further interactions
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = false;

        // Stop every damage effect if running
        if (damageEffectCoroutine != null)
        {
            StopCoroutine(damageEffectCoroutine);
            damageEffectCoroutine = null;
        }

        // Destroy the enemy after 2 seconds
        Destroy(gameObject, 2f);
    }

    // Damage effect
    IEnumerator DamageEffect()
    {
        if (enemySprite == null) yield break;

        // Change to damage color
        enemySprite.color = damageColor;

        // Wait for duration
        yield return new WaitForSeconds(damageEffectDuration);

        // Revert to original color (only if still alive)
        if (alive && enemySprite != null)
            enemySprite.color = originalColor;

        damageEffectCoroutine = null;
    }

    // Getter methods
    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public bool IsAlive()
    {
        return alive;
    }

    // Methods to set colors from outside
    public void SetDamageColor(Color newColor)
    {
        damageColor = newColor;
    }

    public void SetDeathColor(Color newColor)
    {
        deathColor = newColor;
    }

    public void SetDamageDuration(float duration)
    {
        damageEffectDuration = duration;
    }
}