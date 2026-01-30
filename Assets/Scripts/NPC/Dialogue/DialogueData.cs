using UnityEngine;

[System.Serializable]
public class DialogueOption
{
    public string text;
    public int nextNodeIndex;
}

[System.Serializable]
public class DialogueNode
{
    public string npcName;
    [TextArea(3, 10)]
    public string dialogueText;
    public DialogueOption[] options;
}