using UnityEngine;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class dialogueZone : MonoBehaviour
{
    public TextAsset dialogueFile;
    public GameObject dialogueUI;
    public GameObject choicePrefab;
    private TextMeshProUGUI nameText;
    private TextMeshProUGUI dialogueText;
    private GameObject choiceContainer;
    private GameObject namePanel;
    private GameObject dialoguePanel;
    private int unselectedIndex = 0;
    private int selectedIndex = 0;
    private int childCount = 0;

    private string[] lines;
    private int currentLine = 0;
    private List<string> choiceOptions = new List<string>();
    private string optionSelected = "";
    private Player playerInZone = null;
    private bool isDialogueActive = false;
    private bool dialogueFinished = false;

    void Start()
    {
        dialogueUI.SetActive(false);
        namePanel = dialogueUI.transform.Find("Character Name").gameObject;
        Transform nameTextPanel = namePanel.transform.Find("Name Content");
        if (nameTextPanel != null)
        {
            nameText = nameTextPanel.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            Debug.LogError("Cannot find Character Name panel.");
        }

        dialoguePanel = dialogueUI.transform.Find("Dialogue Text").gameObject;
        Transform dialogueTextPanel = dialoguePanel.transform.Find("Dialogue Content");
        if (dialogueTextPanel != null)
        {
            dialogueText = dialogueTextPanel.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            Debug.LogError("Cannot find Dialogue Text panel");
        }
        Transform choiceContainerPanel = dialogueUI.transform.Find("Choice Container");
        if (choiceContainerPanel != null)
        {
            choiceContainer = choiceContainerPanel.gameObject;
        }
        else
        {
            Debug.LogError("Cannot find Choice Container panel");
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
            dialogueFinished = false;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited dialogue zone.");
            foreach (Transform child in choiceContainer.transform)
            {
                Destroy(child.gameObject);
            }
            playerInZone = null;
        }
    }

    void Update()
    {
        if (playerInZone && !dialogueUI.activeSelf)
        {
            dialogueUI.SetActive(true);
            string dialogue = "Press F to talk.";
            GameObject choiceObj = Instantiate(choicePrefab, choiceContainer.transform, false);
            TextMeshProUGUI choiceTMP = choiceObj.GetComponentInChildren<TextMeshProUGUI>();
            if (choiceTMP != null)
            {
                choiceTMP.text = dialogue;
            }
            unselectedIndex = 0;
            selectedIndex = 0;
            childCount = 1;
            UpdateChoiceHighlight();
        }
        else if (!playerInZone && dialogueUI.activeSelf)
        {
            dialogueUI.SetActive(false);
            unselectedIndex = 0;
            selectedIndex = 0;
        }

        if (playerInZone && !isDialogueActive && !dialogueFinished && Input.GetKeyDown(KeyCode.F))
        {
            StartDialogue();
        }
        else if (isDialogueActive)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (optionSelected == "")
                {
                    optionSelected = choiceOptions.ElementAtOrDefault(selectedIndex) ?? "";
                }
                selectedIndex = 0;
                ShowNextLine();
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                selectedIndex = 0;
                EndDialogue();
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                selectedIndex--;
                UpdateChoiceHighlight();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                selectedIndex++;
                UpdateChoiceHighlight();
            }
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll > 0f)
            {
                selectedIndex--;
                UpdateChoiceHighlight();
            }
            else if (scroll < 0f)
            {
                selectedIndex++;
                UpdateChoiceHighlight();
            }
        }

        void StartDialogue()
        {
            currentLine = 0;
            isDialogueActive = true;
            if (playerInZone != null)
                playerInZone.canMove = false;
            namePanel.SetActive(true);
            dialoguePanel.SetActive(true);
            ShowNextLine();
        }

        IEnumerator DestroyChildrenAndShowLine()
        {
            foreach (Transform child in choiceContainer.transform)
            {
                Destroy(child.gameObject);
            }
            childCount = 0;
            yield return null;
            ShowLine();
        }

        void ShowLine()
        {

            if (currentLine < lines.Length)
            {
                string line = lines[currentLine].Trim();
                int colonPosition = line.IndexOf(':');
                string characterName = "";
                Debug.Log($"Choice option selected: {optionSelected}");
                while (optionSelected != "" && line.StartsWith("[=>") && !line.StartsWith($"[=>n{optionSelected}]"))
                {
                    currentLine++;
                    line = lines[currentLine].Trim();
                    colonPosition = line.IndexOf(':');
                }
                Debug.Log($"Processing line: {line}");

                if (optionSelected != "" && line.StartsWith($"[=>n{optionSelected}]"))
                {
                    int bracketClosePos = line.IndexOf(']');
                    colonPosition = line.IndexOf(':');
                    Debug.Log($"bracketClosePos: {bracketClosePos}, colonPosition: {colonPosition}");
                    characterName = "";
                    string dialogue = "";
                    if (colonPosition > 0)
                    {
                        characterName = line.Substring(bracketClosePos + 1, colonPosition - bracketClosePos - 1).Trim();
                        dialogue = line.Substring(colonPosition + 1).Trim();
                    }
                    else
                    {
                        dialogue = line.Substring(bracketClosePos + 1).Trim();
                    }
                    Debug.Log($"Character: {characterName}, Dialogue: {dialogue}");
                    nameText.text = characterName;
                    dialogueText.text = dialogue;
                    currentLine++;
                }
                else
                {
                    optionSelected = "";
                    choiceOptions.Clear();

                    if (colonPosition > 0)
                    {
                        characterName = line.Substring(0, colonPosition);
                    }
                    string dialogue = line.Substring(colonPosition + 1).Trim();
                    nameText.text = characterName;
                    dialogueText.text = dialogue;
                    currentLine++;

                    while (currentLine < lines.Length)
                    {
                        string choiceLine = lines[currentLine].Trim();
                        int bracketClosePos = choiceLine.IndexOf(']');
                        if (choiceLine.StartsWith("[") && !choiceLine.StartsWith("[=>") && bracketClosePos > 0)
                        {
                            string choiceOption = choiceLine.Substring(1, bracketClosePos - 1).Trim();
                            string choiceDialogue = choiceLine.Substring(bracketClosePos + 1).Trim();
                            choiceOptions.Add(choiceOption);
                            GameObject choiceObj = Instantiate(choicePrefab, choiceContainer.transform);
                            TextMeshProUGUI choiceTMP = choiceObj.GetComponentInChildren<TextMeshProUGUI>();
                            if (choiceTMP != null)
                            {
                                choiceTMP.text = choiceDialogue;
                            }
                            currentLine++;
                            childCount++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    unselectedIndex = 0;
                    selectedIndex = 0;
                    UpdateChoiceHighlight();
                }
            }
            else
            {
                EndDialogue();
            }
        }

        void ShowNextLine()
        {
            //ShowLine();
            StartCoroutine(DestroyChildrenAndShowLine());
        }

        void EndDialogue()
        {
            nameText.text = "";
            dialogueText.text = "";
            optionSelected = "";

            foreach (Transform child in choiceContainer.transform)
            {
                Destroy(child.gameObject);
            }

            isDialogueActive = false;
            dialogueFinished = true;
            namePanel.SetActive(false);
            dialoguePanel.SetActive(false);
            if (playerInZone != null)
                playerInZone.canMove = true;
        }

        void UpdateChoiceHighlight()
        {
            Debug.Log($"Choice Container has {childCount} children.");
            if (childCount <= 0) return;

            if (unselectedIndex < 0)
            {
                unselectedIndex = (unselectedIndex + childCount) % childCount;
            }
            else if (unselectedIndex >= childCount)
            {
                unselectedIndex = unselectedIndex % childCount;
            }

            if (selectedIndex < 0)
            {
                selectedIndex = (selectedIndex + childCount) % childCount;
            }
            else if (selectedIndex >= childCount)
            {
                selectedIndex = selectedIndex % childCount;
            }
            Transform unselected = choiceContainer.transform.GetChild(unselectedIndex);
            Transform selected = choiceContainer.transform.GetChild(selectedIndex);

            if (unselected == null || selected == null)
            {
                Debug.LogError("Choice index out of range.");
                return;
            }
            Debug.Log($"unselected: {unselected.GetComponentInChildren<TextMeshProUGUI>().text}");
            Debug.Log($"selected: {selected.GetComponentInChildren<TextMeshProUGUI>().text}");

            unselected.GetComponent<UnityEngine.UI.Image>().color = Color.white;
            selected.GetComponent<UnityEngine.UI.Image>().color = Color.yellow;

            Debug.Log($"Unselected Index: {unselectedIndex}, Selected Index: {selectedIndex}");
            unselectedIndex = selectedIndex;
        }

    }

}
