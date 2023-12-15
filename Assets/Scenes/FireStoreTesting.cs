using Firebase.Firestore;
using Firebase.Extensions;
using UnityEngine;
using System.Collections.Generic;
using Firebase;
using System;
using Firebase.Analytics;
using System.Diagnostics.Tracing;

public class FireStoreTesting : MonoBehaviour
{
    FirebaseFirestore db;
    private DependencyStatus dependencyStatus;

    // Start is called before the first frame update
    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
                db = FirebaseFirestore.DefaultInstance;
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
        Debug.Log("Enabling data collection.");
        FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

        Debug.Log("Set user properties.");
        // Set the user's sign up method.
        AnalyticsLogin();
    }


    public void AnalyticsLogin()
    {
        // Log an event with no parameters.
        Debug.Log("Logging a login event." + FirebaseAnalytics.EventLogin);
        //FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLogin);
    }


    public void AddEvent()
    {
        TestFunc(EventName.XanaFestival);
        TestFunc(EventName.XanaLobby);
        TestFunc(EventName.BreakingDownArena);
    }

    private void TestFunc(EventName _EventName)
    {
        //object[] objects = new object[3];
        ReadFirebaseData(_EventName);
//        if (objects == null)
//        {
//            Debug.Log("No data found");
//            InsertData(_EventName);
//            return;
//        }
//        int eventCount = (int)objects[1];
//        int totalUsers = (int)objects[2];
//        DocumentReference docRef = db.Collection("Xana Users").Document(_EventName.ToString());
//        Dictionary<string, object> user = new Dictionary<string, object>
//{
//        { "EventName", objects[0]},
//        { "EventCount", eventCount++ },
//        { "TotalUsers", totalUsers++ },
//};

//        docRef.SetAsync(user).ContinueWithOnMainThread(task =>
//        {
//            Debug.Log("updated data to the " + _EventName.ToString() + " document in the Xana users collection.");
//        });
    }

    private void InsertData(EventName _eventName)
    {
        DocumentReference docRef = db.Collection("Xana Users").Document(_eventName.ToString());
        Dictionary<string, object> user = new Dictionary<string, object>
        {
        { "EventName", _eventName.ToString()},
        { "EventCount", 1 },
        { "TotalUsers", 1 },
        };

        docRef.SetAsync(user).ContinueWithOnMainThread(task =>
        {
            Debug.Log("Added data to the " + _eventName.ToString() + " document in the Xana Users collection.");
        });
    }

    private void ReadFirebaseData(EventName _eventName)
    {
        DocumentReference document = db.Collection("Xana Users").Document(_eventName.ToString());
        document.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                Debug.Log(String.Format("Document data for {0} document:", snapshot.Id));
                Dictionary<string, object> documentDictionary = snapshot.ToDictionary();
                Debug.Log(String.Format("EventName: {0}", documentDictionary["EventName"]));
                Debug.Log(String.Format("EventCount: {0}", documentDictionary["EventCount"]));
                Debug.Log(String.Format("TotalUsers: {0}", documentDictionary["TotalUsers"]));

                string eventName = documentDictionary["EventName"].ToString();
                Debug.Log("EventCount: " + eventName);
               
                int eventCount = int.Parse(documentDictionary["EventCount"].ToString());
                int totalUsers = int.Parse(documentDictionary["TotalUsers"].ToString());
                Debug.LogError("EventCount: " + eventCount);
                eventCount++;
                totalUsers++;
                Dictionary<string, object> user = new Dictionary<string, object>
                {
                    { "EventName", eventName},
                    { "EventCount", eventCount },
                    { "TotalUsers", totalUsers },
                };
                
                document.SetAsync(user).ContinueWithOnMainThread(task =>
                {
                    Debug.Log("Updated data to the " + _eventName.ToString() + " document in the Xana Users collection.");
                });
            }
            else
            {
                InsertData(_eventName);
                Debug.Log(String.Format("Document {0} does not exist!", snapshot.Id));
            }
        });
    }
}
public enum EventName
{
    XanaLobby, BreakingDownArena, XanaFestival
}