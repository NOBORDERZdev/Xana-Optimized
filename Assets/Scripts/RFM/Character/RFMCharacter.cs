using Photon.Pun;
using Photon.Voice.PUN;
using RFM.Managers;
using System;
using UnityEngine;

public class RFMCharacter : MonoBehaviour
{
    public PhotonView photonView;
    public PhotonVoiceView voiceView;
    public RFMPlayerClass RFMPlayer;
    public bool isHunter;
    //public static Action gameStartAction;


    private void OnEnable()
    {
        RFM.EventsManager.onGameStart += GameStart;
    }

    private void OnDisable()
    {
        RFM.EventsManager.onGameStart -= GameStart;
    }

    //void Start()
    //{
    //    gameStartAction += GameStart;
    //}

    public void GameStart()
    {
        Debug.Log($"RFM {photonView.Owner.NickName} + player is hunter: { photonView.Owner.CustomProperties["isHunter"]}");
        isHunter = bool.Parse(photonView.Owner.CustomProperties["isHunter"].ToString());

        if (RFMManager.Instance.isPlayerHunter)
        {
            voiceView.SpeakerInUse.gameObject.SetActive(isHunter);
        }
        else
        {
            voiceView.SpeakerInUse.gameObject.SetActive(!isHunter);
        }
    }
}
