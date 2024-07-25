using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummitAnalyticsTrigger : MonoBehaviour
{
    private StayTimeTrackerForSummit _stayTimeTrackerForSummit;
    private string LastTriggeredArea;
    private void Start()
    {
        _stayTimeTrackerForSummit = GameplayEntityLoader.instance._stayTimeTrackerForSummit;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.parent.name == LastTriggeredArea)
            return;
        LastTriggeredArea = other.gameObject.transform.parent.name;
        if (other.gameObject.transform.parent.name == StayTimeTrackerForSummit.SummitAreaTrigger.Central_Area.ToString())
            CallEvents(StayTimeTrackerForSummit.SummitAreaTrigger.Central_Area.ToString());
        else if (other.gameObject.transform.parent.name == StayTimeTrackerForSummit.SummitAreaTrigger.Entertainment_Area.ToString())
            CallEvents(StayTimeTrackerForSummit.SummitAreaTrigger.Entertainment_Area.ToString());
        else if (other.gameObject.transform.parent.name == StayTimeTrackerForSummit.SummitAreaTrigger.Business_Area.ToString())
            CallEvents(StayTimeTrackerForSummit.SummitAreaTrigger.Business_Area.ToString());
        else if (other.gameObject.transform.parent.name == StayTimeTrackerForSummit.SummitAreaTrigger.Game_Area.ToString())
            CallEvents(StayTimeTrackerForSummit.SummitAreaTrigger.Game_Area.ToString());
        else if (other.gameObject.transform.parent.name == StayTimeTrackerForSummit.SummitAreaTrigger.Web3_Area.ToString())
            CallEvents(StayTimeTrackerForSummit.SummitAreaTrigger.Web3_Area.ToString());
        Invoke("RemoveLastTriggeredArea", 2f);

    }
    private void CallEvents(string areaName)
    {
        if (_stayTimeTrackerForSummit != null)
        {
            if (_stayTimeTrackerForSummit.isTrackingTimeForExteriorArea)
            {
                _stayTimeTrackerForSummit.StopTrackingTime();
                _stayTimeTrackerForSummit.CalculateAndLogStayTime();
                _stayTimeTrackerForSummit.isTrackingTimeForExteriorArea = false;
            }
            else
            {
                _stayTimeTrackerForSummit.SummitAreaName = areaName;
                string eventName = "XS_TV_" + _stayTimeTrackerForSummit.SummitAreaName;
                GlobalConstants.SendFirebaseEventForSummit(eventName);
                _stayTimeTrackerForSummit.isTrackingTimeForExteriorArea = true;
                _stayTimeTrackerForSummit.StartTrackingTime();
            }
        }
    }
    private void RemoveLastTriggeredArea()
    {
        LastTriggeredArea = "";
    }
}
