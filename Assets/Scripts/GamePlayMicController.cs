using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayMicController : MonoBehaviour
{
    public GameObject micOn;
    public GameObject micOff;
    public GameObject micOnPotrait;
    public GameObject micOffPotrait;
    public GameObject otherButton;
    public GameObject otherButtonPotrait;
    void Start()
    {
        // Disabling forcefully mute functionaility according to new requirement // Ahsan
        //if (WorldItemView.m_EnvName.Contains("Xana Festival") || WorldItemView.m_EnvName.Contains("NFTDuel Tournament") || WorldItemView.m_EnvName.Contains("BreakingDown Arena") /*|| WorldItemView.m_EnvName.Contains("XANA Summit")*/)
        //{
        //    if (ConstantsHolder.xanaConstants.mic == 1)
        //    {
        //        ConstantsHolder.xanaConstants.StopMic();
        //        XanaVoiceChat.instance.StopRecorder();
        //        XanaVoiceChat.instance.TurnOffMic();
        //        micOn.SetActive(false);
        //        micOnPotrait.SetActive(false);
        //        micOff.GetComponent<Button>().interactable = false;
        //        micOffPotrait.GetComponent<Button>().interactable = false;
        //        micOn.GetComponent<Button>().interactable = false;
        //        micOnPotrait.GetComponent<Button>().interactable = false;
        //        Debug.Log("Call MyBeachMute");
        //        otherButton.GetComponent<Button>().interactable = false;
        //        otherButtonPotrait.GetComponent<Button>().interactable = false;
        //    }
        //}
    }
}
