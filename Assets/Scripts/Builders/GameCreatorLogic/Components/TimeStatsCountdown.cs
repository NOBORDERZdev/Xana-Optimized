using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeStatsCountdown : MonoBehaviour
{
    public static Action<int> _timeStart;
    public static Action _timeStop;

    private bool coroutineIsRunning;

    public Sprite[] countdownSprites;
    public bool IsTimeCountDownActive;
    public Image BGImage, _mainImage;
    [SerializeField]
    private int i;

    IEnumerator countTimer;
    private void OnEnable()
    {
        _timeStart += StartTimer;
        _timeStop += StopTimer;
    }
    private void OnDisable()
    {
        _timeStart -= StartTimer;
        _timeStop -= StopTimer;
    }

    private void Start()
    {
        //_mainImage.enabled = false;
        //BGImage.enabled = false;
        countTimer = countdownTimer();
    }


    public void StartTimer(int j)
    {
        if (!IsTimeCountDownActive)
        {
            i = j;
            IsTimeCountDownActive = true;
            //_mainImage.enabled = true;
            //BGImage.enabled = true;
            coroutineIsRunning = true;
            //countTimer = countdownTimer();
            //StartCoroutine(countTimer);

            //TimeStats.canRun = false;
            BuilderEventManager.OnTimerCountDownTriggerEnter?.Invoke(i, true);
        }
    }
    public void StopTimer()
    {
        IsTimeCountDownActive = false;
        StopCoroutine(countTimer);
        coroutineIsRunning = false;
        BuilderEventManager.OnTimerCountDownTriggerEnter?.Invoke(i, false);
    }

    IEnumerator countdownTimer()
    {
        //_mainImage.sprite = countdownSprites[i];
        //BuilderEventManager.OnTimerCountDownTriggerEnter?.Invoke(i, true);
        yield return new WaitForSeconds(1f);
        i--;
        if (i >= 0)
        {
            countTimer = countdownTimer();
            StartCoroutine(countTimer);
        }
        else
        {
            _timeStop?.Invoke();
            //_mainImage.enabled = false;
            //BGImage.enabled = false;
            yield break;
        }
    }
}
