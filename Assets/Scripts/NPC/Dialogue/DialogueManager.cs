using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    [Header("UI Components")]
    public GameObject dialogueBox;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    [Header("Options")]
    public Transform optionsContainer;
    public GameObject optionButtonPrefab;

    private List<DialogueNode> currentConversation;
    public bool isDialogueActive = false;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        dialogueBox.SetActive(false);
    }

    public void StartDialogue(List<DialogueNode> conversation)
    {
        currentConversation = conversation;
        isDialogueActive = true;
        dialogueBox.SetActive(true);

        ShowNode(0);
    }

    public void ShowNode(int index)
    {
        if (index < 0 || index >= currentConversation.Count)
        {
            EndDialogue();
            return;
        }

        DialogueNode node = currentConversation[index];

        nameText.text = node.npcName;
        dialogueText.text = node.dialogueText;

        foreach (Transform child in optionsContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (DialogueOption option in node.options)
        {
            GameObject btnObj = Instantiate(optionButtonPrefab, optionsContainer);
            btnObj.GetComponentInChildren<TextMeshProUGUI>().text = option.text;

            Button btn = btnObj.GetComponent<Button>();
            int targetIndex = option.nextNodeIndex;
            btn.onClick.AddListener(() => ShowNode(targetIndex));
        }

        if (node.options.Length == 0)
        {
            GameObject btnObj = Instantiate(optionButtonPrefab, optionsContainer);
            btnObj.GetComponentInChildren<TextMeshProUGUI>().text = "Koniec";
            Button btn = btnObj.GetComponent<Button>();
            btn.onClick.AddListener(() => EndDialogue());
        }
    }

    public void EndDialogue()
    {
        isDialogueActive = false;
        dialogueBox.SetActive(false);
    }
}