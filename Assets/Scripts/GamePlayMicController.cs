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
    [SerializeField]
    private string _socketId;
    private SocketManager _manager;
    private void OnDisable()
    {
        if (_manager != null)
        {
            _manager.Socket.Off();
            _manager.Close();
        }
    }
    private void Start()
    {

        _manager = new SocketManager(new Uri(ConstantsGod.API_BASEURL));
        _manager.Socket.On<ConnectResponse>(SocketIOEventTypes.Connect, OnConnected);
        _manager.Socket.On<CustomError>(SocketIOEventTypes.Error, OnError);
        _manager.Socket.On<CustomError>(SocketIOEventTypes.Disconnect, OnSocketDisconnect);
        UserMicEnableDisable(ConstantsHolder.xanaConstants.UserMicEnable);
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
    }
    private void OnConnected(ConnectResponse resp)
    {
        _socketId = resp.sid;
        _manager.Socket.On<string>("userMicControl", UserMicControl);
    }
    private void OnError(CustomError args)
    {
        Debug.Log("<color=blue> Mic Socket -- Connection Error  </color>" + args.message);
    }
    private void OnSocketDisconnect(CustomError args)
    {
        Debug.Log("<color=blue> Mic Socket -- Disconnect  </color>");
    }
    private void UserMicControl(string userMicStatus)
    {
        UserMicStatus micStatus = JsonConvert.DeserializeObject<UserMicStatus>(userMicStatus);
        //UserMicStatus?.Invoke(micStatus);
        if(micStatus.world_id.ToString() == ConstantsHolder.xanaConstants.MuseumID)
            UserMicEnableDisable(micStatus.micEnable);
    }
    private void UserMicEnableDisable(bool isEnable)
    {
        ConstantsHolder.xanaConstants.UserMicEnable = isEnable;
        if (isEnable)
        {
            if (PlayerPrefs.GetInt("micSound") == 0)
            {
                //ConstantsHolder.xanaConstants.StopMic();
                //XanaVoiceChat.instance.StopRecorder();
                XanaVoiceChat.instance.TurnOffMic();
                _micOff.interactable = true;
                _micOffPotrait.interactable = true;
                _settingOffButton.interactable = true;
                _settingOffButtonPotrait.interactable = true;
            }
            else
            {
                //ConstantsHolder.xanaConstants.PlayMic();
                //XanaVoiceChat.instance.EnableRecoder();
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
            //XanaVoiceChat.instance.StopRecorder();
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