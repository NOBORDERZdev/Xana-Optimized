using UnityEngine;
using System.Collections;

public class StayTimeTracker : MonoBehaviour
{
    public string worldName;

    private float startTime;
    private bool isTrackingTime = false;

    IEnumerator Start()
    {
        StartTrackingTime();
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

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Escape))
    //        Application.Quit();
    //}
    //private void OnApplicationQuit()
    //{
    //    StopTrackingTime();
    //    CalculateAndLogStayTime();
    //}

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
        int seconds = Mathf.FloorToInt(stayTime % 60f);

        if (minutes > 0)
        {
            Debug.Log("<color=red>" + worldName + "Stay_" + minutes.ToString() + "m:" + seconds.ToString() + "s</color>");
            Firebase.Analytics.FirebaseAnalytics.LogEvent(worldName + "Stay_" + minutes.ToString() + "m:" + seconds.ToString() + "s");
        }
        else
        {
            Debug.Log("<color=red>" + worldName + "Stay_" + seconds.ToString() + "s</color>");
            Firebase.Analytics.FirebaseAnalytics.LogEvent(worldName + "Stay_"+ seconds.ToString() + "s");
        }


    }



}
