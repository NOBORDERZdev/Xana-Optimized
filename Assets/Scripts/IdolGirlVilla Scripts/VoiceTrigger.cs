using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoiceTrigger : MonoBehaviour
{
    //public static VoiceTrigger instance;
    public GameObject actionButtonObject, Chracter;
    public VoiceDialouge characterDialogue;


    public bool startTalking;

    //private void Awake()
    //{
    //    if (instance == null)
    //        instance = this;
    //}
    private void Start()
    {
        //if (DialoguesManagerVoice.Instance.dialogues == null)
        //    DialoguesManagerVoice.Instance.dialogues = new VoiceDialouge();

        //actionButtonObject.GetComponent<OnMouseDownScriptVoice>().dialogues = characterDialogue;
        //actionButtonObject.GetComponent<OnMouseDownScriptVoice>().actionButton = actionButtonObject;

        //actionButton = actionButtonObject.GetComponent<Button>();
        //actionButton.onClick.AddListener(() => DialoguesManager.Instance.StartDialogue(characterDialogue));
    }

    public void ChangeInteractableButton(bool state)
    {
        actionButtonObject.SetActive(state);
    }

    //public void OnTriggerEnter(Collider other)
    //{
    //    actionButtonObject.SetActive(true);
    //}


    //public void OnTriggerExit(Collider other)
    //{
    //    actionButtonObject.SetActive(false);
    //}
}
