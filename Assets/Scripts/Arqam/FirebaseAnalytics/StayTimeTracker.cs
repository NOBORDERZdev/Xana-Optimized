using UnityEngine;
using System.Collections;
using static GlobalConstants;

public class StayTimeTracker : MonoBehaviour
{
    public string worldName;

    private float startTime;
    private bool isTrackingTime = false;

    public bool forXanaDefaultAnalytics = false;

    //private bool isTimerRunning = false;
    //private float elapsedTime = 0.0f;

    IEnumerator Start()
    {
        StartTrackingTime();

        //isTimerRunning = true;
        //StartCoroutine(Countdown());

        yield return new WaitForSeconds(1f);

        // There are two Different Analytics for Xana and Client
        if (!forXanaDefaultAnalytics)
        {
            if (worldName.Contains("Zone_Museum"))
                worldName = FirebaseTrigger.StayTime_ZoneX.ToString();

            else if (worldName.Contains("Lobby"))
                worldName = FirebaseTrigger.StayTime_MainLobby.ToString();

            else if (worldName.Contains("FiveElement"))
                worldName = FirebaseTrigger.StayTime_FiveElement.ToString();

            else if (ConstantsHolder.xanaConstants.mussuemEntry.Equals(JJMussuemEntry.Astro) || ConstantsHolder.xanaConstants.mussuemEntry.Equals(JJMussuemEntry.Rental))
                worldName = FirebaseTrigger.StayTime_AtomRental.ToString();

            else if (worldName.Contains("D_Infinity_Labo"))
                worldName = FirebaseTrigger.StayTime_THA.ToString();
        }
        else
        {
            // Using For Xana Default Analyticsq
            worldName = FirebaseTrigger.StayTime.ToString();
        }
    }

    private void StartTrackingTime()
    {
        startTime = Time.time;
        isTrackingTime = true;
    }

    private void OnDisable()
    {
        if (isTrackingTime)
        {
            StopTrackingTime();
            CalculateAndLogStayTime();
        }
    }


    private void StopTrackingTime()
    {
        isTrackingTime = false;
    }

    private void CalculateAndLogStayTime()
    {
        float stayTime = Time.time - startTime;
        startTime = Mathf.Abs(startTime);
        int minutes = Mathf.FloorToInt(stayTime / 60f);

        if (forXanaDefaultAnalytics)
        {
            // Less than 1 minute
            // More than 1 but less than 10
            // More than 10 minutes
            if (minutes < 1) 
                SendFirebaseEvent(worldName+ "Lessthan_1");
            else if (minutes >= 1 && minutes < 10)
                SendFirebaseEvent(worldName + "Morethan_1_Lessthan_10");
            else if (minutes >= 10 )
                SendFirebaseEvent(worldName + "Morethan_10");
        }
        else
        {
            if (minutes < 3)
                SendFirebaseEvent(worldName + "_" + (minutes + 1));
            else
            {
                SendFirebaseEvent(worldName + "_1");
                SendFirebaseEvent(worldName + "_2");
                SendFirebaseEvent(worldName + "_3");
            }
            if (minutes >= 3) SendFirebaseEvent(worldName + "_5");
            if (minutes >= 5) SendFirebaseEvent(worldName + "_10");
            if (minutes >= 10) SendFirebaseEvent(worldName + "_20");
            if (minutes >= 20) SendFirebaseEvent(worldName + "_30");
            if (minutes >= 30) SendFirebaseEvent(worldName + "_30+");
        }
    }

}
