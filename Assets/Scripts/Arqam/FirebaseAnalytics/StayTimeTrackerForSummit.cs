using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Types;
using static GlobalConstants;

public class StayTimeTrackerForSummit : MonoBehaviour
{

    private float _startTime;
    public bool IsTrackingTime = false;
    public bool IsTrackingTimeForExteriorArea;
    public string SummitAreaName;
    public int DomeId;
    public int DomeWorldId;
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
        if (IsTrackingTime && ConstantsHolder.xanaConstants.EnviornmentName.Contains("XANA Summit"))
        {
            StopTrackingTime();
            CalculateAndLogStayTime();
        }
        else if (IsTrackingTime && ConstantsHolder.isFromXANASummit)
        {
            StopTrackingTime();
            CalculateAndLogStayTime();
        }
    }
    public void StartTrackingTime()
    {
        _startTime = Time.time;
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
        string worldName;
        if (IsTrackingTimeForExteriorArea)
            worldName = "_XS_" + SummitAreaName;
        else
            worldName ="_Dome_" + DomeId + "_World_" + DomeWorldId;
        if (minutes > 0)
            SendFirebaseEventForSummit("ST_" + (minutes) + worldName);
        else
            SendFirebaseEventForSummit("ST_1" + worldName);
    }
}
