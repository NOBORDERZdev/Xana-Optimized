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
        if (XanaConstants.xanaConstants.Equals(JJMussuemEntry.Astro))
            worldName = "JJWorld_Astro";
        else if (XanaConstants.xanaConstants.Equals(JJMussuemEntry.Rental))
            worldName = "JJWorld_Rental";
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


        if (minutes >= 2)
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent(worldName + "_Stay_MoreThan2Mintue");
        }
        else
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent(worldName + "_Stay_LessThan2Mintue");
        }
        Debug.Log("<color=red>" + worldName + "_Stay_" + minutes.ToString() + "m_" + seconds.ToString() + "s</color>");

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
