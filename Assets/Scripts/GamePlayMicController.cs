using BestHTTP.SocketIO3;
using BestHTTP.SocketIO3.Events;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayMicController : MonoBehaviour
{
    public SocketManager Manager;
    string address;
    public string socketId;
    public GameObject micOn;
    public GameObject micOff;
    public GameObject micOnPotrait;
    public GameObject micOffPotrait;
    public GameObject SettingOnButton;
    public GameObject SettingOffButton;
    public GameObject SettingOnButtonPotrait;
    public GameObject SettingOffButtonPotrait;

    public Action<UserMicStatus> UserMicStatus;
    void Start()
    {

        Manager = new SocketManager(new Uri(ConstantsGod.API_BASEURL));
        Manager.Socket.On<ConnectResponse>(SocketIOEventTypes.Connect, OnConnected);
        Manager.Socket.On<CustomError>(SocketIOEventTypes.Error, OnError);
        Manager.Socket.On<CustomError>(SocketIOEventTypes.Disconnect, OnSocketDisconnect);
        //Disabling forcefully mute functionaility according to new requirement // Ahsan
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
        UserMicEnableDisable(ConstantsHolder.xanaConstants.UserMicEnable);
    }
    void OnConnected(ConnectResponse resp)
    {
        Manager.Socket.On<string>("userMicControl", UserMicControl);
    }
    void OnError(CustomError args)
    {
        Debug.Log("<color=blue> Mic Socket -- Connection Error  </color>" + args.message);
    }
    void OnSocketDisconnect(CustomError args)
    {
        Debug.Log("<color=blue> Mic Socket -- Disconnect  </color>");
    }
    void UserMicControl(string userMicStatus)
    {
        UserMicStatus micStatus = JsonConvert.DeserializeObject<UserMicStatus>(userMicStatus);
        //UserMicStatus?.Invoke(micStatus);
        UserMicEnableDisable(micStatus.micEnable);
    }
    void UserMicEnableDisable(bool isEnable)
    {
        if (isEnable)
        {
            if (PlayerPrefs.GetInt("micSound") == 0)
            {
                //micOff.SetActive(true);
                //micOffPotrait.SetActive(true);
                ConstantsHolder.xanaConstants.StopMic();
                XanaVoiceChat.instance.StopRecorder();
                XanaVoiceChat.instance.TurnOffMic();
                micOff.GetComponent<Button>().interactable = true;
                micOffPotrait.GetComponent<Button>().interactable = true;
                SettingOffButton.GetComponent<Button>().interactable = true;
                SettingOffButtonPotrait.GetComponent<Button>().interactable = true;
            }
            else
            {
                ConstantsHolder.xanaConstants.PlayMic();
                XanaVoiceChat.instance.EnableRecoder();
                XanaVoiceChat.instance.TurnOnMic();
                micOn.GetComponent<Button>().interactable = true;
                micOnPotrait.GetComponent<Button>().interactable = true;
                SettingOnButton.GetComponent<Button>().interactable = true;
                SettingOnButtonPotrait.GetComponent<Button>().interactable = true;
            }
            //micOn.SetActive(true);
            //micOnPotrait.SetActive(true);
            Debug.Log("Call MyBeachMute");
        }
        else
        {
            //ConstantsHolder.xanaConstants.StopMic();
            XanaVoiceChat.instance.StopRecorder();
            XanaVoiceChat.instance.TurnOffMic();
            if(PlayerPrefs.GetInt("micSound") == 0)
            {
                //micOn.SetActive(false);
                //micOnPotrait.SetActive(false);
                micOff.GetComponent<Button>().interactable = false;
                micOffPotrait.GetComponent<Button>().interactable = false;
                //Debug.Log("Call MyBeachMute");
                SettingOffButton.GetComponent<Button>().interactable = false;
                SettingOffButtonPotrait.GetComponent<Button>().interactable = false;
            }
            else
            {
                micOn.GetComponent<Button>().interactable = false;
                micOnPotrait.GetComponent<Button>().interactable = false;
                SettingOnButton.GetComponent<Button>().interactable = false;
                SettingOnButtonPotrait.GetComponent<Button>().interactable = false;

            }
        }
    }
}
[System.Serializable]
public class UserMicStatus
{
    public int world_id;
    public bool micEnable;
}