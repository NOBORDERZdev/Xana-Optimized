using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyTimerManager : MonoBehaviourPunCallbacks
{
    public float timerDuration = 60f;
    [HideInInspector]
    public double startTime = -1;
    public bool isTimerRunning = false;

    private void Awake()
    {
        if (!GetComponent<PhotonView>().IsMine)
        {
            GetComponent<PartyTimerManager>().enabled = false;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        if (ConstantsHolder.xanaConstants.isJoinigXanaPartyGame || ConstantsHolder.xanaConstants.isBuilderGame)
        {
            ReferencesForGamePlay.instance.XANAPartyWaitingPanel.SetActive(false);
            return;
        }
        if(LocalizationManager.forceJapanese || GameManager.currentLanguage == "ja")
            ReferencesForGamePlay.instance.XANAPartyWaitingPanel.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "他のプレイヤーを待っています... " + "60s";
        else
            ReferencesForGamePlay.instance.XANAPartyWaitingPanel.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Waiting for other players... " + "60s";

        XANAPartyManager.Instance.GameIndex = 0;

        if (PhotonNetwork.IsMasterClient && !ConstantsHolder.xanaConstants.isJoinigXanaPartyGame)
        {
            XANAPartyManager.Instance.GetComponent<PenpenzLpManager>().RaceID = 0;
            if (startTime <= -1)
            {
                startTime = PhotonNetwork.Time;
            }
            GetComponent<PhotonView>().RPC(nameof(StartTimer), RpcTarget.AllBuffered, startTime);
        }
    }

    void Update()
    {
        if (isTimerRunning && !ConstantsHolder.xanaConstants.isJoinigXanaPartyGame)
        {
            double elapsedTime = PhotonNetwork.Time - startTime;
            float currentTime = (float)(timerDuration - elapsedTime);
            if (currentTime <= 0 && !ReferencesForGamePlay.instance.isMatchingTimerFinished)
            {
               
                currentTime = 0;
                isTimerRunning = false;
                ReferencesForGamePlay.instance.isMatchingTimerFinished = true;
                StartCoroutine(GameplayEntityLoader.instance.PenguinPlayer.GetComponent<XANAPartyMulitplayer>().ShowLobbyCounter());
            }
            if (LocalizationManager.forceJapanese || GameManager.currentLanguage == "ja")
                ReferencesForGamePlay.instance.XANAPartyWaitingPanel.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "他のプレイヤーを待っています... " + currentTime.ToString("F0") + "s";
            else
                ReferencesForGamePlay.instance.XANAPartyWaitingPanel.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Waiting for other players... " + currentTime.ToString("F0") + "s";
        }
    }

    [PunRPC]
    public void StartTimer(double masterStartTime)
    {
        StartCoroutine(StartTimerDelay(masterStartTime));
    }
    IEnumerator StartTimerDelay(double masterStartTime)
    {
        while (GameplayEntityLoader.instance == null || GameplayEntityLoader.instance.PenguinPlayer == null)
        {
            yield return new WaitForSeconds(0.5f);
        }
        PartyTimerManager ref_PartyTimerManager = GameplayEntityLoader.instance.PenguinPlayer.GetComponent<PartyTimerManager>();
        if (!ref_PartyTimerManager.isTimerRunning)
        {
            if (ref_PartyTimerManager.startTime <= -1)
            {
                ref_PartyTimerManager.startTime = masterStartTime;
            }
            ref_PartyTimerManager.isTimerRunning = true;
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (startTime <= -1)
            {
                startTime = PhotonNetwork.Time;
            }
            GetComponent<PhotonView>().RPC(nameof(StartTimer), RpcTarget.AllBuffered, startTime);
        }
    }

}
