using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;

public class CountdownManager : MonoBehaviourPunCallbacks
{
    #region Variables
    public bool startCountdown = false;
    public float remainingTIme = 60;
    TakeDamage _takeDamage;
    public Coroutine countdownCoroutine;
    TextMeshProUGUI timeText;
    Image timeFillImg;
    byte eventCodeNumber = 2;
    #endregion

    #region Unity Funcs

    void Awake()
    {
        timeText = FightingModule.GameManager.instance._timerText;
        timeFillImg = FightingModule.GameManager.instance.roundTimeImg;
    }
    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    private void Start()
    {
        _takeDamage = GetComponent<TakeDamage>();
    }
    #endregion

    #region User Funcs
    [PunRPC]
    public void SettingUp()
    {
        print("SettingUP");
        remainingTIme = 600;
        StartCoroutine(SentCurrentTime());
        countdownCoroutine = StartCoroutine(countdown());
    }
    /// <summary>
    /// 
    /// </summary>

    private void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == eventCodeNumber)
        {
            object[] data = (object[])photonEvent.CustomData;
            float newRecievedRemainingTime = (float)data[0];
            //Here use the New Sync Remaining Time
            remainingTIme = newRecievedRemainingTime;
        }
    }

    IEnumerator SentCurrentTime() // Call this Function from MasterCleint when the New Player Enters
    {
        yield return null; // just 1 second delay if the new player is loading new scene, so that when he loads the scene then rasie this event
        object[] data = new object[] { remainingTIme };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(eventCodeNumber, data, raiseEventOptions, SendOptions.SendReliable);
    }

    ////Remarks/
    ///Update The Time Locally for each player, and when the new Player enters then just sent remaining to new players
    ///

    public void stopCountDown()
    {
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null;
        }
    }

    IEnumerator countdown()
    {
        while (remainingTIme > 0)
        {
            yield return new WaitForSeconds(1f);
            remainingTIme--;
            timeText.text = remainingTIme.ToString("00");
            remainingTIme = Mathf.Clamp(remainingTIme, 0f, Mathf.Infinity);
            timeFillImg.fillAmount = remainingTIme / 60;
        }
        _takeDamage.OnTimeOver();
    }
    #endregion
}
