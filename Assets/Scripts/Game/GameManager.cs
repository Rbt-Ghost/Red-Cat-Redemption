using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Required for Legacy UI

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Panels")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject victoryPanel;

    [Header("Victory UI Text")]
    [SerializeField] private Text victoryStatsText;

    [Header("Scene Settings")]
    [SerializeField] private string homeScreenSceneName = "HomeScreen_Scene";

    private PlayerStats playerStats;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        playerStats = FindObjectOfType<PlayerStats>();

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
    }

    public void TriggerGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void TriggerVictory()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
            Time.timeScale = 0f;

            UpdateVictoryText();
        }
    }

    void UpdateVictoryText()
    {
        if (victoryStatsText == null || playerStats == null) return;

        // 1. Calculate raw HP lost (e.g., 90 - 70 = 20 damage)
        float hpLost = playerStats.GetMaxHealth() - playerStats.GetCurrentHealth();

        // 2. Convert HP to "Lives" (20 / 10 = 2 lives)
        // We use Mathf.Ceil or Round to ensure we don't get decimals like 2.5 lives
        float livesLost = hpLost / 10f;

        // 3. Update the text
        // Result: "It took you only 2/9 lives"
        victoryStatsText.text = "It took you only " + livesLost + "/9 lives";
    }

    public void PlayAgain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToHomeScreen()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(homeScreenSceneName);
    }
}