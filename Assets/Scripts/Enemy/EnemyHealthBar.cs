using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField]
    private EnemyStats enemyStats;
    [SerializeField]
    private Image fill;
    [SerializeField]
    private float changeSpeed = 15.0f;

    // Target fill amount based on current health
    private float targetFillAmount = 1.0f;

    void Start()
    {
        // Find EnemyStats if not assigned
        if (enemyStats == null)
        {
            enemyStats = GetComponentInParent<EnemyStats>();
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
        // Compute target fill amount
        if (enemyStats != null)
        {
            targetFillAmount = enemyStats.GetCurrentHealth() / enemyStats.GetMaxHealth();
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
        if (enemyStats != null && fill != null)
        {
            targetFillAmount = enemyStats.GetCurrentHealth() / enemyStats.GetMaxHealth();
            fill.fillAmount = targetFillAmount; // Immediate update without animation
        }
    }
}