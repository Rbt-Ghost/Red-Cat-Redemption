using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth = 100f;

    [Header("Visual Effects")]
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private Color deathColor = new Color(0.3f, 0.3f, 0.3f, 1f);
    [SerializeField] private float damageEffectDuration = 0.3f;

    private bool alive = true;
    private Color originalColor;
    private Coroutine damageEffectCoroutine;

    void Start()
    {
        if (playerSprite == null) playerSprite = GetComponent<SpriteRenderer>();
        if (playerSprite != null) originalColor = playerSprite.color;
    }

    void Update()
    {
        // Keep your input logic if needed
        // GetInput(); 
    }

    public void TakeDamage(float damageAmount)
    {
        if (!alive) return;

        currentHealth -= damageAmount;

        if (damageEffectCoroutine != null) StopCoroutine(damageEffectCoroutine);
        damageEffectCoroutine = StartCoroutine(DamageEffect());

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    void Die()
    {
        alive = false;
        Debug.Log("Player has died.");

        if (playerSprite != null) playerSprite.color = deathColor;

        // --- Call Game Over ---
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TriggerGameOver();
        }
        else
        {
            Debug.LogError("GameManager not found in scene!");
        }
    }

    public void Respawn()
    {
        alive = true;
        currentHealth = maxHealth;
        transform.position = Vector3.zero;
        if (playerSprite != null) playerSprite.color = originalColor;
    }

    public void Heal(float healAmount)
    {
        if (!alive) return;
        currentHealth += healAmount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
    }

    IEnumerator DamageEffect()
    {
        if (playerSprite == null) yield break;
        playerSprite.color = damageColor;
        yield return new WaitForSeconds(damageEffectDuration);
        playerSprite.color = originalColor;
        damageEffectCoroutine = null;
    }

    public float GetCurrentHealth() { return currentHealth; }
    public float GetMaxHealth() { return maxHealth; }
    public bool IsAlive() { return alive; }
}