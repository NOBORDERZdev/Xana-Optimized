using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialoguesManagerVoice : MonoBehaviour
{
    #region Singleton
    public static DialoguesManagerVoice Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    #endregion

    [Header("Dialogues")]
    public VoiceDialouge dialogues;
    public AudioSource audioSource;
    public int num;



    private void Start()
    {
        //if(VoiceTrigger.instance)
        //{
        //    dialogues = VoiceTrigger.instance.characterDialogue;
        //    num = int.Parse(VoiceTrigger.instance.Chracter.name);
        //    print("My number for chracter" + num);
        //}
    }

    public void StartDialogue(VoiceTrigger instance)
    {

        num = 0;   
        dialogues = instance.characterDialogue;
        Debug.LogError(instance.Chracter.name);
        num = int.Parse(instance.Chracter.name);
        print("My number for chracter" + num);
        SoundToPlay(num);
    }

    private void SoundToPlay( int num)
    {
        audioSource.clip = dialogues.Voicedialogues[num];
        audioSource.Play();
    }
}


