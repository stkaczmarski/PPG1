using UnityEngine;

public enum DialogueAction
{
    None,
    OpenShop,
    GiveItem
}

[System.Serializable]
public class DialogueOption
{
    public string text;
    public int nextNodeIndex;
    public DialogueAction actionType;
    public ItemData itemToGive;
}

[System.Serializable]
public class DialogueNode
{
    public string npcName;
    [TextArea(3, 10)]
    public string dialogueText;
    public DialogueOption[] options;
}