using UnityEngine;
using System.Collections;

public class StayTimeTracker : MonoBehaviour
{
    public string worldName;

    private float startTime;
    private bool isTrackingTime = false;

    //private bool isTimerRunning = false;
    //private float elapsedTime = 0.0f;

    IEnumerator Start()
    {
        StartTrackingTime();

        //isTimerRunning = true;
        //StartCoroutine(Countdown());

        yield return new WaitForSeconds(1f);

        if (worldName.Contains("Zone_Museum"))
            worldName = "1F_ZoneX_StayTime_"; 
        
        else if (worldName.Contains("Lobby"))
            worldName = "1F_Mainloby_StayTime_";

        else if (worldName.Contains("FiveElement"))
            worldName = "1F_FiveElement_StayTime_";

        else if (XanaConstants.xanaConstants.mussuemEntry.Equals(JJMussuemEntry.Astro) || XanaConstants.xanaConstants.mussuemEntry.Equals(JJMussuemEntry.Rental))
            worldName = "2F_AtomRental_StayTime_";
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

        //EndTimer();
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
        int seconds = Mathf.FloorToInt(stayTime % 60f);

        //if (minutes > 0)
        //{
        //    Debug.Log("<color=red>" + worldName + "Stay_" + minutes.ToString() + "m_" + seconds.ToString() + "s</color>");
        //    Firebase.Analytics.FirebaseAnalytics.LogEvent(worldName + "Stay_" + minutes.ToString() + "m_" + seconds.ToString() + "s");
        //}
        //else
        //{
        //    Debug.Log("<color=red>" + worldName + "Stay_" + seconds.ToString() + "s</color>");
        //    Firebase.Analytics.FirebaseAnalytics.LogEvent(worldName + "Stay_" + seconds.ToString() + "s");
        //}

        if (seconds > 0)
            minutes++;

        // For Testing 
        //minutes += 40;

        if (minutes > 30)
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent(worldName + minutes +"m_");
        }
        else
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent(worldName + +minutes + "m");
        }
        Debug.Log("<color=red>" + worldName + minutes + "m</color>");
    }


    //private IEnumerator Countdown()
    //{
    //    while (isTimerRunning)
    //    {
    //        elapsedTime += Time.deltaTime;
    //        yield return null; // Wait for the next frame
    //    } 
    //}

    //private void EndTimer()
    //{
    //    Debug.Log("Timer ended");

    //    isTimerRunning = false;
    //    int minutes = Mathf.FloorToInt(elapsedTime / 60f);
    //    int seconds = Mathf.FloorToInt(elapsedTime % 60f);

    //    if (minutes > 0)
    //    {
    //        Debug.Log("<color=red>" + worldName + "Stay_" + minutes.ToString() + "m_" + seconds.ToString() + "s</color>");
    //        Firebase.Analytics.FirebaseAnalytics.LogEvent(worldName + "Stay_" + minutes.ToString() + "m_" + seconds.ToString() + "s");
    //    }
    //    else
    //    {
    //        Debug.Log("<color=red>" + worldName + "Stay_" + seconds.ToString() + "s</color>");
    //        Firebase.Analytics.FirebaseAnalytics.LogEvent(worldName + "Stay_" + seconds.ToString() + "s");
    //    }
    //}


}
