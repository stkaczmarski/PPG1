using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteract : MonoBehaviour
{
    public bool player_detection;

    public List<DialogueNode> myConversation;
    public ShopInteractable myShopScript;

    void Update()
    {
        if (player_detection && Input.GetKeyDown(KeyCode.E) && !DialogueManager.instance.isDialogueActive)
        {
            DialogueManager.instance.StartDialogue(myConversation, myShopScript);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player_detection = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player_detection = false;
        }
    }
}