using UnityEngine;
using TMPro;

public class dialogueZone : MonoBehaviour
{
    public TextAsset dialogueFile;
    public GameObject dialogueUI;
    private TextMeshProUGUI nameText;
    private TextMeshProUGUI dialogueText;

    private string[] lines;
    private int currentLine = 0;
    private Player playerInZone = null;
    private bool isDialogueActive = false;

    void Start()
    {
        dialogueUI.SetActive(false);
        Transform namePanel = dialogueUI.transform.Find("Character Name");
        if (namePanel != null)
        {
            nameText = namePanel.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            Debug.LogError("Cannot find Character Name panel.");
        }

        Transform dialoguePanel = dialogueUI.transform.Find("Dialogue Text");
        if (dialoguePanel != null)
        {
            dialogueText = dialoguePanel.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            Debug.LogError("Cannot find Dialogue Text panel");
        }
        if (dialogueFile != null)
        {
            lines = dialogueFile.text.Split('\n');
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = other.GetComponent<Player>();
            Debug.Log("Player entered dialogue zone.");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited dialogue zone.");
            playerInZone = null;
        }
    }

    void Update()
    {
        if (playerInZone && !isDialogueActive && Input.GetKeyDown(KeyCode.F))
        {
            StartDialogue();
        }
        else if (isDialogueActive)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                ShowNextLine();
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                EndDialogue();
            }
        }
    }

    void StartDialogue()
    {
        currentLine = 0;
        isDialogueActive = true;
        if (playerInZone != null)
            playerInZone.canMove = false;
        dialogueUI.SetActive(true);
        ShowLine();
    }

    void ShowLine()
    {
        if (currentLine < lines.Length)
        {
            string line = lines[currentLine].Trim();
            int closeBracket = line.IndexOf(']');
            if (closeBracket > 0)
            {
                string characterName = line.Substring(1, closeBracket - 1);
                string dialogue = line.Substring(closeBracket + 1).Trim();
                nameText.text = characterName;
                dialogueText.text = dialogue;
            }
        }
        else
        {
            EndDialogue();
        }
    }

    void ShowNextLine()
    {
        currentLine++;
        ShowLine();
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        if (playerInZone != null)
            playerInZone.canMove = true;
        dialogueUI.SetActive(false);
    }
}
