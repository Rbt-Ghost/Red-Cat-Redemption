using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Npc : MonoBehaviour
{
    public Image npcPortrait;
    private SpriteRenderer npcSpriteRenderer;

    public GameObject dialogBox;
    public Text dialogText;
    public Text npcName;
    public string _name = "";
    public string[] dialogLines;
    private int currentLine;

    public GameObject continueButton;
    public float wordSpeed;
    private bool playerIsClose;

    void Start()
    {
        npcSpriteRenderer = GetComponent<SpriteRenderer>();

        npcName.text = _name;

        if (npcPortrait != null && npcSpriteRenderer != null)
            npcPortrait.sprite = npcSpriteRenderer.sprite;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F) && playerIsClose)
        {
            if (dialogBox.activeInHierarchy)
            {
                resetText();
            }
            else
            {
                dialogBox.SetActive(true);
                StartCoroutine(Typing());
            }
        }

        if (dialogText.text == dialogLines[currentLine])
        {
            continueButton.SetActive(true);
        }
        else
        {
            continueButton.SetActive(false);
        }
    }

    public void resetText()
    {
        dialogText.text = "";
        currentLine = 0;
        dialogBox.SetActive(false);
    }

    IEnumerator Typing()
    {
        foreach(char letter in dialogLines[currentLine].ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }
    }

    public void NextLine()
    {
        continueButton.SetActive(false);

        if (currentLine < dialogLines.Length - 1)
        {
            currentLine++;
            dialogText.text = "";
            StartCoroutine(Typing());
        }
        else
        {
            resetText();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = false;
            resetText();
        }
    }
}
