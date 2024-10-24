using UnityEngine;
using System.Collections;
using static GlobalConstants;
using System;
using PMY;

public class StayTimeTracker : MonoBehaviour
{
    public string worldName;
    public DateTime sceneEnterTime;
    [Space(5)]
    [SerializeField] private float _timeRemaining = 60f; // Time in seconds
    private float _timerUpdateInterval = 1f; // Update interval in seconds
    private float _timeSinceLastUpdate = 0f; // Time passed since last UI update
    private bool IsTrackingTime = false;

    //private float startTime;
    //private bool isTrackingTime = false;


    IEnumerator Start()
    {
        //StartTrackingTime();

        //isTimerRunning = true;
        //StartCoroutine(Countdown());

        yield return new WaitForSeconds(1f);

        if (worldName.Contains("Zone_Museum"))
            worldName = FirebaseTrigger.StayTime_ZoneX.ToString();

        else if (worldName.Contains("Lobby"))
            worldName = FirebaseTrigger.StayTime_MainLobby.ToString();

        else if (worldName.Contains("FiveElement"))
            worldName = FirebaseTrigger.StayTime_FiveElement.ToString();

        else if (XanaConstants.xanaConstants.mussuemEntry.Equals(JJMussuemEntry.Astro) || XanaConstants.xanaConstants.mussuemEntry.Equals(JJMussuemEntry.Rental))
            worldName = FirebaseTrigger.StayTime_AtomRental.ToString();

        else if (worldName.Contains("PMY ACADEMY"))
            worldName = FirebaseTrigger.StayTime_PMYLobby.ToString();
        else if (worldName.Contains("PMYGallery"))
            worldName = FirebaseTrigger.StayTime_PMYGallery.ToString();
        else if (worldName.Contains("PMYRoomA"))
        {
            if (XanaConstants.xanaConstants.assetLoadType.Equals(XanaConstants.AssetLoadType.ByAddressable))
                BuilderEventManager.AfterWorldOffcialWorldsInatantiated += UpdateWorldName;
            else if (XanaConstants.xanaConstants.assetLoadType.Equals(XanaConstants.AssetLoadType.ByBuild))
                UpdateWorldName();
        }

        IsTrackingTime = true;
    }

    private void UpdateWorldName()
    {
        BuilderEventManager.AfterWorldOffcialWorldsInatantiated -= UpdateWorldName;
        switch (PMY_Nft_Manager.Instance.PMY_RoomId)
        {
            case 8:
                worldName = FirebaseTrigger.StayTime_CRoom1.ToString();
                break;
            case 9:
                worldName = FirebaseTrigger.StayTime_CRoom2.ToString();
                break;
            case 10:
                worldName = FirebaseTrigger.StayTime_CRoom3.ToString();
                break;
            case 11:
                worldName = FirebaseTrigger.StayTime_CRoom4.ToString();
                break;
            case 12:
                worldName = FirebaseTrigger.StayTime_CRoom5.ToString();
                break;
            case 13:
                worldName = FirebaseTrigger.StayTime_CRoom6.ToString();
                break;
        }
    }

    //private void StartTrackingTime()
    //{
    //    sceneEnterTime = DateTime.Now;
    //    //startTime = Time.time;
    //    isTrackingTime = true;
    //}

    //private void OnDisable()
    //{
    //    if (isTrackingTime)
    //    {
    //        StopTrackingTime();
    //        CalculateAndLogStayTime();
    //    }
    //}


    //private void StopTrackingTime()
    //{
    //    isTrackingTime = false;
    //}

    //private void CalculateAndLogStayTime()
    //{
    //    TimeSpan stayTime = DateTime.Now - sceneEnterTime;
    //    double minutes = stayTime.TotalMinutes;
    //    int min = (int)minutes;
    //    //float stayTime = Time.time - startTime;
    //    //startTime = Mathf.Abs(startTime);
    //    //int minutes = Mathf.FloorToInt(stayTime / 60f);

    //    if (min < 3)
    //        SendFirebaseEvent(worldName + "_" + (min + 1));
    //    else
    //    {
    //        SendFirebaseEvent(worldName + "_1");
    //        SendFirebaseEvent(worldName + "_2");
    //        SendFirebaseEvent(worldName + "_3");
    //    }
    //    if (min >= 3) SendFirebaseEvent(worldName + "_5");
    //    if (min >= 5) SendFirebaseEvent(worldName + "_10");
    //    if (min >= 10) SendFirebaseEvent(worldName + "_20");
    //    if (min >= 20) SendFirebaseEvent(worldName + "_30");
    //    if (min >= 30) SendFirebaseEvent(worldName + "_30+");
    //}

    private void FixedUpdate()
    {
        if (IsTrackingTime)
        {
            // Accumulate time since last update
            _timeSinceLastUpdate += Time.fixedDeltaTime;

            // Check if it's time to update the timer
            if (_timeSinceLastUpdate >= _timerUpdateInterval)
            {
                // Update the remaining time and reset the counter
                _timeRemaining -= _timerUpdateInterval;
                _timeSinceLastUpdate = 0f;

                // Check if the timer has finished
                if (_timeRemaining <= 0)
                {
                    SendFirebaseEvent(worldName + "_1");
                    _timeRemaining = 60;
                }
            }
        }
    }

}
