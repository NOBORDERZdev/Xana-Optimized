using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnMouseDownScriptVoice : MonoBehaviour
{
    //public VoiceDialouge dialogues;
    public VoiceTrigger voiceTrigger; 
    //public GameObject actionButton;


    private void Start()
    {
        
    }
    private void OnMouseUp()
    {
        DialoguesManagerVoice.Instance.StartDialogue(voiceTrigger);
        voiceTrigger.actionButtonObject.SetActive(false);
        //actionButton.SetActive(false);
    }
}
