using UnityEngine;

public class Npc : MonoBehaviour
{
    public string npcName;
    private SpriteRenderer spriteRenderer;
    public string[] dialogLines;

    private bool playerIsClose;

    void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Check Keyboard 'F' OR Mobile Interact Button
        bool interactTriggered = Input.GetKeyDown(KeyCode.F);

        if (MobileInputManager.Instance != null && MobileInputManager.Instance.IsInteracting())
        {
            interactTriggered = true;
        }

        if (interactTriggered && playerIsClose && !DialogueManager.Instance.IsDialogueActive())
        {
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
            // Removed redundant check
            playerIsClose = false;
        }
    }
}