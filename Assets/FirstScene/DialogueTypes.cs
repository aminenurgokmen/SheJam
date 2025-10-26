using UnityEngine;

public enum DialogueSide { Left, Right }

[System.Serializable]
public class DialogueEntry
{
    public string speaker;                 // "Ruh", "Şaman"
    public DialogueSide side = DialogueSide.Left; // Hangi kutu?
    [TextArea(2, 5)] public string text;   // Replik
}
