using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

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

    public void StartDialogue(string name, Sprite portrait, string[] dialogueLines)
    {
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
        dialogText.text = "";
        dialogBox.SetActive(false);
    }
}
