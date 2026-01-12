using UnityEngine;

public class Npc : MonoBehaviour
{
    public string npcName;
    private SpriteRenderer spriteRenderer;
    public string[] dialogLines;

    [Header("UI")]
    [Tooltip("Drag the child GameObject with the 'F' icon or speech bubble here")]
    public GameObject interactionCue; // <--- ADD THIS

    private bool playerIsClose;

    void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        // Ensure it's hidden at start
        if (interactionCue != null) interactionCue.SetActive(false);
    }

    void Update()
    {
        // --- ADD VISUAL CUE LOGIC ---
        if (interactionCue != null)
        {
            // Show the cue only if player is close AND they aren't already talking
            bool shouldShowCue = playerIsClose && !DialogueManager.Instance.IsDialogueActive();

            // Only toggle if the state is different to save performance
            if (interactionCue.activeSelf != shouldShowCue)
            {
                interactionCue.SetActive(shouldShowCue);
            }
        }
        // ----------------------------

        // Check Keyboard 'F' OR Mobile Interact Button
        bool interactTriggered = Input.GetKeyDown(KeyCode.F);

        if (MobileInputManager.Instance != null && MobileInputManager.Instance.IsInteracting())
        {
            interactTriggered = true;
        }

        if (interactTriggered && playerIsClose && !DialogueManager.Instance.IsDialogueActive())
        {
            // Hide the cue immediately when interaction starts
            if (interactionCue != null) interactionCue.SetActive(false);

            DialogueManager.Instance.StartDialogue(
                npcName,
                spriteRenderer.sprite,
                dialogLines
            );
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerIsClose = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = false;
            // Ensure it hides when walking away
            if (interactionCue != null) interactionCue.SetActive(false);
        }
    }
}