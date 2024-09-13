using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Types;
using static GlobalConstants;

public class StayTimeTrackerForSummit : MonoBehaviour
{
    [SerializeField] private float _timeRemaining = 60f; // Time in seconds
    private float _timerUpdateInterval = 1f; // Update interval in seconds
    private float _timeSinceLastUpdate = 0f; // Time passed since last UI update

    private float _startTime;
    public bool IsTrackingTime = false;
    public bool IsTrackingTimeForExteriorArea;
    public string SummitAreaName;
    public int DomeId;
    public int DomeWorldId;
    public bool IsBuilderWorld = false;
    public enum SummitAreaTrigger
    {
        Central_Area,
        Entertainment_Area,
        Business_Area,
        Game_Area,
        Web3_Area
    }
    private void Start()
    {
        SummitAreaName = SummitAreaTrigger.Central_Area.ToString();
    }
    private void OnDisable()
    {
        //if (IsTrackingTime && ConstantsHolder.xanaConstants.EnviornmentName.Contains("XANA Summit"))
        //{
        //    StopTrackingTime();
        //    CalculateAndLogStayTime();
        //}
        //else if (IsTrackingTime && ConstantsHolder.isFromXANASummit)
        //{
        //    StopTrackingTime();
        //    CalculateAndLogStayTime();
        //}
    }
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
                    SendFirebaseEventForSummit("ST_1" + SetEventName());
                    _timeRemaining = 60;
                }
            }
        }
    }
    private string SetEventName()
    {
        string worldName;
        if (IsTrackingTimeForExteriorArea)
            worldName = "_XS_" + SummitAreaName;
        else
        {
            if (IsBuilderWorld)
                worldName = "_Dome_" + DomeId + "_BW_" + DomeWorldId;
            else
                worldName = "_Dome_" + DomeId + "_XW_" + DomeWorldId;
        }
        return worldName;
    }
    public void StartTrackingTime()
    {
        SendFirebaseEventForSummit("ST_1" + SetEventName());
        //_startTime = Time.time;
        _timeRemaining = 60;
        IsTrackingTime = true;
    }
    public void StopTrackingTime()
    {
        IsTrackingTime = false;
    }
    public void CalculateAndLogStayTime()
    {
        float stayTime = Time.time - _startTime;
        _startTime = Mathf.Abs(_startTime);
        int minutes = Mathf.FloorToInt(stayTime / 60f);
        
        if (minutes > 0)
            SendFirebaseEventForSummit("ST_" + (minutes) + SetEventName());
        else
            SendFirebaseEventForSummit("ST_1" + SetEventName());
    }

}
