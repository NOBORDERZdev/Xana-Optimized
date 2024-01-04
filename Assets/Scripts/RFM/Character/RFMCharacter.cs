using Photon.Pun;
using Photon.Voice.PUN;
using RFM.Managers;
using System;
using UnityEngine;

public class RFMCharacter : MonoBehaviour
{
    public PhotonView photonView;
    public PhotonVoiceView voiceView;
    //public RFMPlayerClass RFMPlayer;
    public bool isHunter;
    public GameObject rearViewCamera;
    //public static Action gameStartAction;


    private void OnEnable()
    {
        RFM.EventsManager.onTakePositionTimeStart += OnTakePositionStart;
        RFM.EventsManager.onGameStart += GameStart;
        //if (photonView.IsMine)
            //RFM.EventsManager.OnShowRearViewMirror(true);
    }

    private void OnDisable()
    {
        RFM.EventsManager.onTakePositionTimeStart -= OnTakePositionStart;
        RFM.EventsManager.onGameStart -= GameStart;
        //if (photonView.IsMine)
            //RFM.EventsManager.OnShowRearViewMirror(false);
    }

    void Start()
    {
        voiceView.SpeakerInUse.GetComponent<AudioSource>().spatialBlend = 0;
        if (!photonView.IsMine)
        {
            Destroy(rearViewCamera);
        }
    }

    private void OnTakePositionStart()
    {
        isHunter = bool.Parse(photonView.Owner.CustomProperties["isHunter"].ToString());

        if (isHunter)
        {
            GetComponent<RFM.Character.PlayerRunner>().enabled = false;
            GetComponent<RFM.Character.PlayerHunter>().enabled = true;
        }
        else
        {
            GetComponent<RFM.Character.PlayerHunter>().enabled = false;
            GetComponent<RFM.Character.PlayerRunner>().enabled = true;
        }
    }




    public void GameStart()
    {
        Debug.Log($"RFM {photonView.Owner.NickName} + player is hunter: { photonView.Owner.CustomProperties["isHunter"]}");
        //isHunter = bool.Parse(photonView.Owner.CustomProperties["isHunter"].ToString());

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
