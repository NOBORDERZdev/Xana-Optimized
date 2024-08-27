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
    [SerializeField]
    private Button _micOff, _micOffPotrait, _settingOnButton, _settingOffButton, _settingOnButtonPotrait, _settingOffButtonPotrait;
    private string address;
    private SocketManager Manager;
    public string socketId;
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
        if(micStatus.world_id.ToString() == ConstantsHolder.xanaConstants.MuseumID)
            UserMicEnableDisable(micStatus.micEnable);
    }
    void UserMicEnableDisable(bool isEnable)
    {
        ConstantsHolder.xanaConstants.UserMicEnable = isEnable;
        if (isEnable)
        {
            if (PlayerPrefs.GetInt("micSound") == 0)
            {
                //ConstantsHolder.xanaConstants.StopMic();
                XanaVoiceChat.instance.StopRecorder();
                XanaVoiceChat.instance.TurnOffMic();
                _micOff.interactable = true;
                _micOffPotrait.interactable = true;
                _settingOffButton.interactable = true;
                _settingOffButtonPotrait.interactable = true;
            }
            else
            {
                //ConstantsHolder.xanaConstants.PlayMic();
                XanaVoiceChat.instance.EnableRecoder();
                XanaVoiceChat.instance.TurnOnMic();
                _settingOnButton.gameObject.SetActive(true);
                _settingOnButtonPotrait.gameObject.SetActive(true);
                _settingOffButton.gameObject.SetActive(false);
                _settingOffButtonPotrait.gameObject.SetActive(false);
                _micOff.interactable = true;
                _micOffPotrait.interactable = true;
                _settingOffButton.interactable = true;
                _settingOffButtonPotrait.interactable = true;
            }
            //Debug.Log("Call MyBeachMute");
        }
        else
        {
            //ConstantsHolder.xanaConstants.StopMic();
            XanaVoiceChat.instance.StopRecorder();
            XanaVoiceChat.instance.TurnOffMic();
            _settingOnButton.gameObject.SetActive(false);
            _settingOnButtonPotrait.gameObject.SetActive(false);
            _settingOffButton.gameObject.SetActive(true);
            _settingOffButtonPotrait.gameObject.SetActive(true);
            _micOff.interactable = false;
            _micOffPotrait.interactable = false;
            _settingOffButton.interactable = false;
            _settingOffButtonPotrait.interactable = false;
        }
    }
}
[System.Serializable]
public class UserMicStatus
{
    public int world_id;
    public bool micEnable;
}