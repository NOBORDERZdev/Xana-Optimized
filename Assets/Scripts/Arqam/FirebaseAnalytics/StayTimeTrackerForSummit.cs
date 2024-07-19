using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Types;
using static GlobalConstants;

public class StayTimeTrackerForSummit : MonoBehaviour
{
    private float startTime;
    public bool isTrackingTime = false;
    public int DomeId;
    public int DomeWorldId;
    public SummitAreaTrigger SummitArea;
    public enum SummitAreaTrigger
    { 
        Middle,
        Sand,
        Water,
        FestivalStage,
        ChimmnyTown,
        SpaceX
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!isTrackingTime)
        {
            StartTrackingTime();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (isTrackingTime) 
        {
            StopTrackingTime();
            CalculateAndLogStayTime();
        }
    }
    public void StartTrackingTime()
    {
        startTime = Time.time;
        isTrackingTime = true;
    }
    private void OnDisable()
    {
        if (isTrackingTime && ConstantsHolder.isFromXANASummit)
        {
            StopTrackingTime();
            CalculateAndLogStayTime();
        }
    }
    public void StopTrackingTime()
    {
        isTrackingTime = false;
    }
    public void CalculateAndLogStayTime()
    {
        float stayTime = Time.time - startTime;
        startTime = Mathf.Abs(startTime);
        int minutes = Mathf.FloorToInt(stayTime / 60f);
        string worldName ="_Dome_" + DomeId + "_World_" + DomeWorldId;
        if (minutes < 3)
            SendFirebaseEventForSummit("ST_" + (minutes + 1) + worldName);
        else
        {
            SendFirebaseEventForSummit("ST_1" + worldName);
            SendFirebaseEventForSummit("ST_2" + worldName);
            SendFirebaseEventForSummit("ST_3" + worldName);
        }
        if (minutes >= 3) SendFirebaseEventForSummit("ST_5"+ worldName);
        if (minutes >= 5) SendFirebaseEventForSummit("ST_10"+ worldName);
        if (minutes >= 10) SendFirebaseEventForSummit("ST_20" + worldName);
        if (minutes >= 20) SendFirebaseEventForSummit("ST_30" + worldName);
        if (minutes >= 30) SendFirebaseEventForSummit("ST_30+" + worldName);
    }
}
