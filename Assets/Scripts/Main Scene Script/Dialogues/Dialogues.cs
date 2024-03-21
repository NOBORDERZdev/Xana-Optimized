using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogues/Create New Dialgue", order = 0)]
public class Dialogues : ScriptableObject
{
    public string DialogueName;
    [TextArea]
    public string[] dialogues;
    [TextArea]
    public string[] japaneseDialogues;
}
