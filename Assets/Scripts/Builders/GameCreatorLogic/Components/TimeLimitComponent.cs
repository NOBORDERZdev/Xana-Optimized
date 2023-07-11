using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Models;
using Photon.Pun;

//[RequireComponent(typeof(Rigidbody))]
public class TimeLimitComponent : ItemComponent
{

    // public static Action TimerCompleted;


    [Tooltip("Set Total Time")]
    [SerializeField]
    private float m_TotalTime;

    [SerializeField]
    private bool isTimerStarted = false;

    public bool IsTimeLimitActive;

    private Text m_TimeCounterText;

    public string TimeLimitSTR;

    private const string m_TimerUIName = "TimeLimitText";

    private const int m_EndTime = 0;

    private TimeSpan m_SecondsInToTimeFormate;

    private const string m_TimeFormate = @"mm\:ss";

    [SerializeField]
    private bool isActivated = false;
    [SerializeField]
    private TimeLimitComponentData timeLimitComponentData;

    public Action StartTimerEvent;
    public Action EndTimerEvent;

    BoxCollider boxCollider;

    bool timerUploaded = false;
    private bool coroutineIsRunning = false;

    private void OnEnable()
    {
        StartTimerEvent += StartTimer;
        EndTimerEvent += StopTimer;
    }
    private void OnDisable()
    {
        StartTimerEvent -= StartTimer;
        EndTimerEvent -= StopTimer;
    }



    private void Start()
    {
        timerUploaded = false;
        m_TotalTime = timeLimitComponentData.TimeLimitt;
    }

    public void StartTimer()
    {
        Debug.Log("Start Timer Time Limit");
        isTimerStarted = true;
        StartCoroutine(Timer());
        if (!IsTimeLimitActive)
        {
            IsTimeLimitActive = true;

            //TimeStats.canRun = false;
            BuilderEventManager.OnTimerLimitTriggerEnter?.Invoke(timeLimitComponentData.PurposeHeading, m_TotalTime);
        }
    }


    public void StopTimer()
    {
        isTimerStarted = false;
        coroutineIsRunning = false;
        IsTimeLimitActive = false;
        Start();
        //m_TimeCounterText.color = Color.black;
        //m_TimeCounterText.gameObject.SetActive(false);
    }

    IEnumerator Timer()
    {
        if (isActivated)
        {
            if (!isTimerStarted || coroutineIsRunning) { yield break; }
            coroutineIsRunning = true;
            while (!m_TotalTime.Equals(m_EndTime) && isTimerStarted)
            {
                m_TotalTime -= Time.deltaTime;
                m_TotalTime = (Mathf.Clamp(m_TotalTime, 0, Mathf.Infinity));
                m_SecondsInToTimeFormate = TimeSpan.FromSeconds(m_TotalTime);
                yield return null;
            }
            StopTimer();
        }
    }


    public void Init(TimeLimitComponentData timeLimitComponentData)
    {
        //Debug.Log(JsonUtility.ToJson(timeLimitComponentData));

        this.timeLimitComponentData = timeLimitComponentData;

        isActivated = true;
    }


    private void OnCollisionEnter(Collision _other)
    {
        //}
        //private void OnTriggerEnter(Collider _other)
        //{
        Debug.Log("Timer Limit Trigger: " + _other.gameObject.name);
        if (isActivated)
        {
            if (_other.gameObject.CompareTag("Player") || (_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine))
            {
                StartTimerEvent?.Invoke();
            }
        }
    }
}