using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private Text healthText;
    [SerializeField]
    private PlayerStats playerStats;
    [SerializeField]
    private Image fill;
    [SerializeField]
    private float changeSpeed = 15.0f;
    
    // Target fill amount based on current health
    private float targetFillAmount = 1.0f;

    void Start()
    {
       // Find PlayerStats if not assigned
        if (playerStats == null)
        {
            playerStats = FindFirstObjectByType<PlayerStats>();
            Debug.Log("Not found");
        }
        
        if (fill == null)
        {
            fill = GetComponent<Image>();
        }
        
    // Initialize healthbar with default values
        UpdateHealthBar();
    }

    void Update()
    {
        // Compute target fill amount and update text
        if (playerStats != null)
        {
            targetFillAmount = playerStats.GetCurrentHealth() / playerStats.GetMaxHealth();
            healthText.text = $"{playerStats.GetCurrentHealth()} / {playerStats.GetMaxHealth()}";
        }
        
        // Fill amount interpolation for smooth transition
        if (fill != null)
        {
            fill.fillAmount = Mathf.Lerp(fill.fillAmount, targetFillAmount, changeSpeed * Time.deltaTime);
        }
    }
    
    // Public method to force update health bar (e.g., on health change)
    public void UpdateHealthBar()
    {
        if (playerStats != null && fill != null)
        {
            targetFillAmount = playerStats.GetCurrentHealth() / playerStats.GetMaxHealth();
            healthText.text = $"{playerStats.GetCurrentHealth()} / {playerStats.GetMaxHealth()}";
            fill.fillAmount = targetFillAmount;// Immediate update without animation
        }
    }
}