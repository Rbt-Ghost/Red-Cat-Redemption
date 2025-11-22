using UnityEngine;

public class Npc : MonoBehaviour
{
    public string npcName;
    public SpriteRenderer spriteRenderer;
    public string[] dialogLines;

    private bool playerIsClose;

    void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && playerIsClose && !DialogueManager.Instance.IsDialogueActive())
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
            if (other.CompareTag("Player"))
                playerIsClose = false;
        }
    }
}
