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
        playerStats = Object.FindFirstObjectByType<PlayerStats>();

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
    }

    void Update()
    {
        /*
        // --- INSTANT WIN CHEAT ---
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Instant Win Triggered!");
            TriggerVictory();
        }
        */
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

        // Calculate lives lost (Damage / 10)
        float hpLost = playerStats.GetMaxHealth() - playerStats.GetCurrentHealth();
        float livesLost = hpLost / 10f;

        // Update the text
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