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
        // Find SpriteRenderer if it's not set
        if (playerSprite == null)
            playerSprite = GetComponent<SpriteRenderer>();
        
        // Save original color
        if (playerSprite != null)
            originalColor = playerSprite.color;
    }

    void Update()
    {
        GetInput();
    }
    //Testing input for damage and healing
    void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TakeDamage(10f);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            Heal(10f);
        }
        if( Input.GetKeyDown(KeyCode.R) && !alive)
        {
            Respawn();
        }
    }
    // Method to apply damage to the player
    public void TakeDamage(float damageAmount)
    {
        if (!alive) return;
        
        currentHealth -= damageAmount;
        Debug.Log("Player took " + damageAmount + " damage. Current health: " + currentHealth);
        
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
    // Method to handle player death
    void Die()
    {
        alive = false;
        Debug.Log("Player has died.");
        
        // Apply dead color
        if (playerSprite != null)
            playerSprite.color = deathColor;
        
        // Stop every damage effect if running
        if (damageEffectCoroutine != null)
        {
            StopCoroutine(damageEffectCoroutine);
            damageEffectCoroutine = null;
        }
    }
    // Method to respawn the player
    public void Respawn()
    {
        alive = true;
        currentHealth = maxHealth;
        Debug.Log("Player has respawned.");
        transform.position = Vector3.zero;
        
        // Reset the original color
        if (playerSprite != null)
            playerSprite.color = originalColor;
    }
    // Method to heal the player
    public void Heal(float healAmount)
    {
        if (!alive)
        {
            Debug.Log("Cannot heal. Player is dead.");
            return;
        }

        currentHealth += healAmount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        Debug.Log("Player healed " + healAmount + ". Current health: " + currentHealth);
    }

    // Damage effect
    IEnumerator DamageEffect()
    {
        if (playerSprite == null) yield break;
        
        // Change to red
        playerSprite.color = damageColor;
        
        // Wait for duration
        yield return new WaitForSeconds(damageEffectDuration);
        
        // Revert to original color
        playerSprite.color = originalColor;
        
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
    
    // Metode pentru setat culorile din exterior
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