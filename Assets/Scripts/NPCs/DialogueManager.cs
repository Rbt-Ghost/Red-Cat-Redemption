using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    private bool isDialogActive = false;

    [Header("UI")]
    public GameObject dialogBox;
    public Text dialogText;
    public Text npcName;
    public Image npcPortrait;
    public GameObject continueButton;

    [Header("Typing")]
    public float wordSpeed = 0.02f;

    private string[] lines;
    private int index;
    private bool isTyping;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        dialogBox.SetActive(false);
    }

    void Update()
    {
        // Do nothing if the dialogue is not active
        if (!isDialogActive)
            return;

        // Player presses F to continue
        if (Input.GetKeyDown(KeyCode.F))
        {
            NextLine();
        }
    }

    public void StartDialogue(string name, Sprite portrait, string[] dialogueLines)
    {
        isDialogActive = true;

        npcName.text = name;
        npcPortrait.sprite = portrait;
        lines = dialogueLines;
        index = 0;

        dialogBox.SetActive(true);
        dialogText.text = "";
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        continueButton.SetActive(false);

        foreach (char letter in lines[index].ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }

        isTyping = false;
        continueButton.SetActive(true);
    }

    public void NextLine()
    {
        if (isTyping) return;

        continueButton.SetActive(false);

        if (index < lines.Length - 1)
        {
            index++;
            dialogText.text = "";
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }

    public void EndDialogue()
    {
        //isDialogActive = false;

        dialogText.text = "";
        dialogBox.SetActive(false);

        StartCoroutine(EndDialogueCooldown());
    }

    private IEnumerator EndDialogueCooldown()
    {
        // Wait for the end of the current frame
        // This ensures the NPC script finishes its Update loop 
        // thinking the dialog is STILL active, preventing the restart.
        yield return new WaitForEndOfFrame();

        isDialogActive = false;
    }

    public bool IsDialogueActive()
    {
        return isDialogActive;
    }
}
