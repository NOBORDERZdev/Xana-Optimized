using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using TMPro;
using System.Collections;

public class PhotonTimer : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public enum TimerStatus { Idle, Running, Completed };
    public TimerStatus timerStatus;

    //[HideInInspector]
    public TMP_Text eventTimer;

    private int timerDuration = 60; // Duration of the timer in seconds
    public int timer = 0; // Current timer value
    //public bool isTimerRunning = false; // Flag to indicate if the timer is running

    private PhotonView pv;

    private Coroutine timerCoroutine;


    private void Start()
    {
        pv = GetComponent<PhotonView>();
        if (pv.IsMine)
            eventTimer.gameObject.SetActive(true);
        //InitializeTimer();
    }

    public void InitializeTimer()
    {
        timer = timerDuration;
        RefreshTimerUI();

        if (GetComponent<CharacterType>().playerType==CharacterType.PlayerType.Judge)
        {
            // Debug.Log("<color=red> Start Coroutine Here </color>");
            timerCoroutine = StartCoroutine(Timer());
            timerStatus = TimerStatus.Running;
        }
    }

    private void RefreshTimerUI()
    {
        string minutes = (timer / 60).ToString("00");
        string seconds = (timer % 60).ToString("00");
        eventTimer.text = $"{minutes}:{seconds}";
        if (timer >= 0)
            timer -= 1;
    }

    #region Coroutines

    private IEnumerator Timer()
    {

        if (timer <= -1)
        {
            timerCoroutine = null;
            eventTimer.text = "00:00";
            timerStatus = TimerStatus.Completed;
            if (pv.IsMine)
                GetComponent<CharacterType>().CallEventEnd();
        }
        else
        {
            RefreshTimer_S();
            yield return new WaitForSeconds(1f);
            timerCoroutine = StartCoroutine(Timer());
        }
    }

    public void RefreshTimer_S()
    {
        object[] package = new object[] { timer };
        //Debug.Log("<color=red> Raise Event call </color>");
        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.RefreshTimer,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true }
        );
    }

    public enum EventCodes : byte
    {
        NewPlayer,
        UpdatePlayers,
        ChangeStat,
        NewMatch,
        RefreshTimer
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code >= 200) return;
        //Debug.Log("<color=red> OnEventReceived </color>");
        EventCodes e = (EventCodes)photonEvent.Code;
        object[] o = (object[])photonEvent.CustomData;

        if (e.Equals(EventCodes.RefreshTimer))
            RefreshTimer_R(o);
    }

    private void RefreshTimer_R(object[] data)
    {
        timer = (int)data[0];
        RefreshTimerUI();
    }
    #endregion

}
