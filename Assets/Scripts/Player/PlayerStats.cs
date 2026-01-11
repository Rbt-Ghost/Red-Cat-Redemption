using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 90f; // Set to 90 as requested
    [SerializeField] private float currentHealth = 90f;

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
        if (playerSprite == null)
            playerSprite = GetComponent<SpriteRenderer>();

        if (playerSprite != null)
            originalColor = playerSprite.color;
    }

    void Update()
    {
        // Healing Debug (Optional, moved to H)
        //if (Input.GetKeyDown(KeyCode.H)) Heal(10f);

        // REMOVED 'P' KEY INPUT HERE TO PREVENT CONFLICT
    }

    public void TakeDamage(float damageAmount)
    {
        if (!alive) return;

        currentHealth -= damageAmount;

        if (damageEffectCoroutine != null)
            StopCoroutine(damageEffectCoroutine);
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

        if (playerSprite != null)
            playerSprite.color = deathColor;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.TriggerGameOver();
        }
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