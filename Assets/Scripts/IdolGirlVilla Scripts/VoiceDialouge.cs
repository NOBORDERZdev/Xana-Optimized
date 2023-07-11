using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueForVoice", menuName = "VoiceDialogues/Create New Dialgue for SOund", order = 1)]
public class VoiceDialouge : ScriptableObject
{
    public string DialogueName;
    public AudioClip[] Voicedialogues;
}
