using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteract : MonoBehaviour
{
    public bool player_detection;

    public List<DialogueNode> myConversation;

    void Update()
    {
        if (player_detection && Input.GetKeyDown(KeyCode.E) && !DialogueManager.instance.isDialogueActive)
        {
            Debug.Log("Interakcja: Start dialogu");
            DialogueManager.instance.StartDialogue(myConversation);
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