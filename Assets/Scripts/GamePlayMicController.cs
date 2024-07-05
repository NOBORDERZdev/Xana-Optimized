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
        if (/*WorldItemView.m_EnvName.Contains("Xana Festival") || WorldItemView.m_EnvName.Contains("NFTDuel Tournament") || WorldItemView.m_EnvName.Contains("BreakingDown Arena")*/ false)
        {
            if (ConstantsHolder.xanaConstants.mic == 1)
            {
                ConstantsHolder.xanaConstants.StopMic();
                XanaVoiceChat.instance.TurnOffMic();
                micOn.SetActive(false);
                micOnPotrait.SetActive(false);
                micOff.GetComponent<Button>().interactable = false;
                micOffPotrait.GetComponent<Button>().interactable = false;
                micOn.GetComponent<Button>().interactable = false;
                micOnPotrait.GetComponent<Button>().interactable = false;
                Debug.Log("Call MyBeachMute");
                otherButton.GetComponent<Button>().interactable = false;
                otherButtonPotrait.GetComponent<Button>().interactable = false;
            }
        }
    }
}
