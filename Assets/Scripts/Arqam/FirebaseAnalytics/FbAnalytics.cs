using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;
using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;

public class FbAnalytics : MonoBehaviour
{
    DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
    protected bool firebaseInitialized = false;
    public bool IsLogEnabled = true;

    private void Start()
    {
        StartCoroutine(AssignDelay());
    }

    IEnumerator AssignDelay()
    {
        yield return new WaitForSeconds(0.1f);
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError(
                    "Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    void InitializeFirebase()
    {
        DebugLog("Enabling data collection.");
        FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

        DebugLog("Set user properties.");
        // Set the user's sign up method.
       
        firebaseInitialized = true;
        AnalyticsLogin();
    }

    public void DebugLog(string s)
    {
        if (IsLogEnabled)
            print(s);
    }

    /// <summary>
    /// any event detail of Analytics will pass through here
    /// </summary>
    /// <param name="info"></param>
    public void LogEvent(string info)
    {
        FirebaseAnalytics.LogEvent(info);
    }

    /// <summary>
    /// log with int parameter
    /// </summary>
    /// <param name="info"></param>
    /// <param name="parameter"></param>
    public void LogEvent(string info, string paramenterName, int parameterValue)
    {
        FirebaseAnalytics.LogEvent(info, paramenterName, parameterValue);
    }
//    public void 
    public void AnalyticsLogin()
    {
        // Log an event with no parameters.
        DebugLog("Logging a login event." + FirebaseAnalytics.EventLogin);
        FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLogin);
    }

    public void AnalyticsProgress()
    {
        // Log an event with a float.
        DebugLog("Logging a progress event.");
        FirebaseAnalytics.LogEvent("progress", "percent", 0.4f);
    }

    public void AnalyticsScore()
    {
        // Log an event with an int parameter.
        DebugLog("Logging a post-score event.");
        FirebaseAnalytics.LogEvent(
            FirebaseAnalytics.EventPostScore,
            FirebaseAnalytics.ParameterScore,
            42);
    }

    public void AnalyticsGroupJoin()
    {
        // Log an event with a string parameter.
        DebugLog("Logging a group join event.");
        FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventJoinGroup, FirebaseAnalytics.ParameterGroupId,
            "spoon_welders");
    }

    public void AnalyticsLevelUp()
    {
        // Log an event with multiple parameters.
        DebugLog("Logging a level up event.");
        FirebaseAnalytics.LogEvent(
            FirebaseAnalytics.EventLevelUp,
            new Parameter(FirebaseAnalytics.ParameterLevel, 5),
            new Parameter(FirebaseAnalytics.ParameterCharacter, "mrspoon"),
            new Parameter("hit_accuracy", 3.14f));
    }

    // Reset analytics data for this app instance.
    public void ResetAnalyticsData()
    {
        DebugLog("Reset analytics data.");
        FirebaseAnalytics.ResetAnalyticsData();
    }

  
}