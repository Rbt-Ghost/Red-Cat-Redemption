using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionNpc : MonoBehaviour
{
    public string npcName;
    private SpriteRenderer spriteRenderer;

    [Header("Dialogue Content")]
    public string[] firstTimeLines = { "Hello traveler!", "I haven't seen you around here before." };
    public string[] missionLines = { "Ready for the task?", "Let's go on the mission!" };

    [Header("Scene Transition")]
    public string missionSceneName = "Mission_Scene";

    private bool hasMetPlayer = false;
    private bool playerIsClose;
    private bool waitingForMissionEnd = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Check if player presses F and dialogue isn't already running
        if (Input.GetKeyDown(KeyCode.F) && playerIsClose && !DialogueManager.Instance.IsDialogueActive())
        {
            if (!hasMetPlayer)
            {
                // First interaction
                DialogueManager.Instance.StartDialogue(npcName, spriteRenderer.sprite, firstTimeLines);
                hasMetPlayer = true;
            }
            else
            {
                // Second interaction (Mission)
                DialogueManager.Instance.StartDialogue(npcName, spriteRenderer.sprite, missionLines);
                waitingForMissionEnd = true;
            }
        }

        // Check if the mission dialogue just finished
        if (waitingForMissionEnd && !DialogueManager.Instance.IsDialogueActive())
        {
            LoadMissionScene();
        }
    }

    void LoadMissionScene()
    {
        SceneManager.LoadScene(missionSceneName);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerIsClose = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerIsClose = false;
    }
}