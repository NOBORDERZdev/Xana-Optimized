using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyTimerManager : MonoBehaviour
{
    public float timerDuration = 60f;
    private double startTime;
    public bool isTimerRunning = false;
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
            startTime = PhotonNetwork.Time;
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
            // Update your UI or other game elements with currentTime
        }
    }

    [PunRPC]
    void StartTimer(double masterStartTime)
    {
        startTime = masterStartTime;
        isTimerRunning = true;
    }
}
