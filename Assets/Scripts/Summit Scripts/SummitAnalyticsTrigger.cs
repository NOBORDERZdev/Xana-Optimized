using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummitAnalyticsTrigger : MonoBehaviour
{
    private StayTimeTrackerForSummit _stayTimeTrackerForSummit;
    private string _lastTriggeredArea;
    private void Start()
    {
        if (GameplayEntityLoader.instance!=null)
            _stayTimeTrackerForSummit = GameplayEntityLoader.instance.StayTimeTrackerForSummit;
    }
    private void OnTriggerEnter(Collider other)
    {
        if ( other.gameObject.transform.parent==null || _lastTriggeredArea.IsNullOrEmpty() && other.gameObject.transform.parent.name == _lastTriggeredArea)
            return;
        _lastTriggeredArea = other.gameObject.transform.parent.name;
        if (Enum.TryParse(_lastTriggeredArea, out StayTimeTrackerForSummit.SummitAreaTrigger areaTrigger))
        {
            CallEvents(areaTrigger.ToString());
        }

        Invoke("RemoveLastTriggeredArea", 2f);

    }
    private void CallEvents(string areaName)
    {
        if (_stayTimeTrackerForSummit != null)
        {
            if (_stayTimeTrackerForSummit.IsTrackingTimeForExteriorArea)
            {
                _stayTimeTrackerForSummit.StopTrackingTime();
                _stayTimeTrackerForSummit.CalculateAndLogStayTime();
                _stayTimeTrackerForSummit.IsTrackingTimeForExteriorArea = false;
            }
            else
            {
                _stayTimeTrackerForSummit.SummitAreaName = areaName;
                string eventName = "XS_TV_" + _stayTimeTrackerForSummit.SummitAreaName;
                GlobalConstants.SendFirebaseEventForSummit(eventName);
                _stayTimeTrackerForSummit.IsTrackingTimeForExteriorArea = true;
                _stayTimeTrackerForSummit.StartTrackingTime();
            }
        }
    }
    private void RemoveLastTriggeredArea()
    {
        _lastTriggeredArea = "";
    }
}
