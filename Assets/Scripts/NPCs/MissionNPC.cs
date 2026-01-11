using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionNpc : MonoBehaviour
{
    public string npcName;
    private SpriteRenderer spriteRenderer;

    [Header("Persistence")]
    [Tooltip("Give each NPC a unique ID so they don't share progress")]
    public string npcID = "QuestGiver_01";

    [Header("Dialogue Content")]
    public string[] firstTimeLines = { "Hello! I haven't seen you around here before." };
    public string[] missionLines = { "Ready for the task?", "Let's go on the mission!" };

    [Header("Scene Settings")]
    public string missionSceneName = "MissionScene";

    private bool hasMetPlayer = false;
    private bool playerIsClose;
    private bool isTalkingAboutMission = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Load the saved state. 0 = False, 1 = True.
        hasMetPlayer = PlayerPrefs.GetInt(npcID + "_Met", 0) == 1;
    }

    void Update()
    {
        // 1. Interaction Logic
        // Check Keyboard 'F' OR Mobile Interact Button
        bool interactTriggered = Input.GetKeyDown(KeyCode.F);

        if (MobileInputManager.Instance != null && MobileInputManager.Instance.IsInteracting())
        {
            interactTriggered = true;
        }

        if (interactTriggered && playerIsClose && !DialogueManager.Instance.IsDialogueActive())
        {
            if (!hasMetPlayer)
            {
                DialogueManager.Instance.StartDialogue(npcName, spriteRenderer.sprite, firstTimeLines);

                // Save immediately so it's remembered even if the game crashes
                hasMetPlayer = true;
                PlayerPrefs.SetInt(npcID + "_Met", 1);
                PlayerPrefs.Save();
            }
            else
            {
                DialogueManager.Instance.StartDialogue(npcName, spriteRenderer.sprite, missionLines);
                isTalkingAboutMission = true;
            }
        }

        // 2. Scene Transition Logic
        if (isTalkingAboutMission && !DialogueManager.Instance.IsDialogueActive())
        {
            SceneManager.LoadScene(missionSceneName);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerIsClose = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerIsClose = false;
    }

    // Optional: Call this to 'forget' the player (useful for testing)
    [ContextMenu("Reset NPC Memory")]
    public void ResetMemory()
    {
        PlayerPrefs.DeleteKey(npcID + "_Met");
        hasMetPlayer = false;
        Debug.Log(npcName + " has forgotten the player.");
    }
}