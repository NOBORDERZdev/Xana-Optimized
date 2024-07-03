using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyTimerManager : MonoBehaviour
{
    public float timerDuration = 60f;
    private double startTime = -1;
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
        if (ConstantsHolder.xanaConstants.isJoinigXanaPartyGame)
        {
            ReferencesForGamePlay.instance.XANAPartyMatchingTimer.SetActive(false);
            return;
        }
        ReferencesForGamePlay.instance.XANAPartyMatchingTimer.SetActive(true);
        ReferencesForGamePlay.instance.XANAPartyMatchingTimer.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "60";
        if (PhotonNetwork.IsMasterClient && !ConstantsHolder.xanaConstants.isJoinigXanaPartyGame)
        {
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
            if (currentTime <= 0)
            {
                currentTime = 0;
                isTimerRunning = false;
                ReferencesForGamePlay.instance.isMatchingTimerFinished = true;
                ReferencesForGamePlay.instance.XANAPartyMatchingTimer.SetActive(false);
                // Handle timer end here
            }
            ReferencesForGamePlay.instance.XANAPartyMatchingTimer.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = currentTime.ToString("F0");
        }
    }

    [PunRPC]
    void StartTimer(double masterStartTime)
    {
        StartCoroutine(StartTimerDelay(masterStartTime));
    }
    IEnumerator StartTimerDelay(double masterStartTime)
    {
        yield return new WaitForSeconds(1);
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
}
