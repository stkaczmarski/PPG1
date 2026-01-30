using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    [Header("UI")]
    public GameObject dialogueBox;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    [Header("Options")]
    public Transform optionsContainer;
    public GameObject optionButtonPrefab;

    private List<DialogueNode> currentConversation;
    public bool isDialogueActive = false;

    private ShopInteractable currentShop;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        dialogueBox.SetActive(false);
    }

    public void StartDialogue(List<DialogueNode> conversation, ShopInteractable shop = null)
    {
        currentConversation = conversation;
        currentShop = shop;
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
            if (option.actionType == DialogueAction.OpenShop)
            {
                btn.onClick.AddListener(() => OpenShopAction());
            }
            else if(option.actionType == DialogueAction.GiveItem)
            {
                ItemData item = option.itemToGive;
                int nextIndex = option.nextNodeIndex;
                btn.onClick.AddListener(() => GiveItemAction(item, nextIndex));
            }
            else
            {
                int targetIndex = option.nextNodeIndex;
                btn.onClick.AddListener(() => ShowNode(targetIndex));
            }
        }

        if (node.options.Length == 0)
        {
            GameObject btnObj = Instantiate(optionButtonPrefab, optionsContainer);
            btnObj.GetComponentInChildren<TextMeshProUGUI>().text = "Koniec";
            Button btn = btnObj.GetComponent<Button>();
            btn.onClick.AddListener(() => EndDialogue());
        }
    }

    void OpenShopAction()
    {
        ShopInteractable shopToOpen = currentShop;
        EndDialogue();

        if (shopToOpen != null)
        {
            if (ShopManager.Instance != null)
            {
                ShopManager.Instance.OpenShop(shopToOpen);
            }
        }
    }

    void GiveItemAction(ItemData item, int nextNodeIndex)
    {
        if (item != null)
        {
            bool added = InventoryManager.Instance.AddItem(item);

            if (added)
            {
                Debug.Log("Otrzymano przedmiot: " + item.itemName);
            }
            else
            {
                Debug.Log("Brak miejsca w ekwipunku");
            }
        }
        else
        {
            Debug.LogWarning("Nie przypisano ItemData");
        }
        ShowNode(nextNodeIndex);
    }

    public void EndDialogue()
    {
        isDialogueActive = false;
        dialogueBox.SetActive(false);
        currentShop = null;
    }
}