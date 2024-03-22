using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MicControllerButtons : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject otherButton;
    public GameObject OnStateUI;
    public GameObject OffStateUI;
    public GameObject micOnButtonGameplay;
    public GameObject micOffButtonGameplay;


    private void Start()
    {
        if (WorldItem.m_EnvName.Contains("Xana Festival") || WorldItem.m_EnvName.Contains("NFTDuel Tournament"))
        {
            if (OnStateUI)
                OnStateUI.SetActive(false);

            if (OffStateUI)
            {
                OffStateUI.SetActive(true);
                OffStateUI.GetComponent<Button>().interactable = false;
            }
        }

    }
    private void OnEnable()
    {

        if (XanaConstantsHolder.xanaConstants.mic == 1)
        {
            if (this.gameObject.name == "OffButton")
            {
                otherButton.SetActive(true);
                this.gameObject.SetActive(false);
            }
            else if (this.gameObject.name == "OnButton")
            {
                this.gameObject.SetActive(true);
                otherButton.SetActive(false);
            }
        }
        else
        {
            if (this.gameObject.name == "OffButton")
            {
                this.gameObject.SetActive(true);
                otherButton.SetActive(false);
            }
            else if (this.gameObject.name == "OnButton")
            {
                otherButton.SetActive(true);
                this.gameObject.SetActive(false);
            }
        }
        //XanaVoiceChat.instance.UpdateMicButton();
    }

    public void ClickMicMain()
    {
        if (XanaConstantsHolder.xanaConstants.mic == 1)
        {
            XanaConstantsHolder.xanaConstants.StopMic();
            XanaVoiceChat.instance.TurnOffMic();
            micOnButtonGameplay.SetActive(false);
            micOffButtonGameplay.SetActive(true);
        }
        else
        {
            XanaConstantsHolder.xanaConstants.PlayMic();
            XanaVoiceChat.instance.TurnOnMic();
            micOnButtonGameplay.SetActive(true);
            micOffButtonGameplay.SetActive(false);
        }
        OnEnable();


    }
}
