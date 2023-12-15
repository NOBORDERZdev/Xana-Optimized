using Firebase.Firestore;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;
using Firebase;
using Photon.Voice.Unity.Demos.DemoVoiceUI;

public class GlobalConstants
{
    public static EnvironmentType environmentType = EnvironmentType.Test;
    public static FirebaseFirestore db;
    public enum EnvironmentType
    {
        Test,
        Live
    }

    #region Firebase
    public enum FirebaseTrigger
    {
        Home_Thumbnail,
        Home_Thumbnail_PlayBtn,
        WP_MainLobby_A_ZoneX,
        WP_MainLobby_B_FiveElement,
        WP_MainLobby_C_AtomMuseum,
        WP_MainLobby_D_RentalSpace,
        WP_Infoboard_Atom,
        WP_Infoboard_Rental,
        WP_EachRoom_Atom,
        WP_EachRoom_Rental,
        StayTime_MainLobby,
        StayTime_ZoneX,
        StayTime_FiveElement,
        StayTime_AtomRental,
        CL_NFT_AtomRoom,
        CL_NFT_AtomRental,
        URL_AtomRoom,
        URL_AtomRental,
        // Added New Items
        CL_IMG_ZoneX,
        CL_IMG_FiveElements, // Done
        URL_ZoneX,
        URL_FiveElements
    }

    public static void SendFirebaseEvent(string eventName)
    {
        if (eventName.IsNullOrEmpty() || eventName.Substring(0) == "_") return;

        string prefix = "T_";

        if (APIBaseUrlChange.instance.IsXanaLive)
        {
            prefix = "L_"; environmentType = EnvironmentType.Live;
        }
        eventName = prefix + eventName;
        Debug.Log("<color=red>" + eventName + "</color>");

        Firebase.Analytics.FirebaseAnalytics.LogEvent(eventName);
        ReadFirebaseData(eventName);
    }

    private static void ReadFirebaseData(string _eventName)
    {
        DocumentReference document = db.Collection(_eventName).Document(System.DateTime.Now.ToString());
        document.SetAsync(new Dictionary<string, object>
        {
            { "EventName", _eventName },
            { "Time", System.DateTime.Now },
            { "UserName", XanaConstants.xanaConstants.userId },
        }).ContinueWithOnMainThread(task =>
        {
            Debug.Log("Added data to the " + _eventName.ToString() + " document in the Xana Users collection.");
        });
        




        //DocumentReference document = db.Collection(XanaConstants.xanaConstants.EnviornmentName).Document(_eventName);
        //document.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        //{
        //    DocumentSnapshot snapshot = task.Result;
        //    if (snapshot.Exists)
        //    {
        //        if (snapshot.ContainsField("User_" + XanaConstants.xanaConstants.userName + "_" + XanaConstants.xanaConstants.userId))
        //        {
        //            // Debug.Log(String.Format("Document data for {0} document:", snapshot.Id));
        //            Dictionary<string, object> documentDictionary = snapshot.ToDictionary();
        //            //Debug.Log(String.Format("EventName: {0}", documentDictionary["EventName"]));
        //            //Debug.Log(String.Format("EventCount: {0}", documentDictionary["EventCount"]));
        //            //Debug.Log(String.Format("TotalUsers: {0}", documentDictionary["TotalUsers"]));

        //            //string eventName = documentDictionary["EventName"].ToString();
        //            Debug.Log("EventCount: " + _eventName + "---" + "User_" + XanaConstants.xanaConstants.userName + "_" + XanaConstants.xanaConstants.userId);
        //            int totalEventCount = int.Parse(documentDictionary["Total_Event_Count"].ToString());
        //            int eventCount = int.Parse(documentDictionary["User_" + XanaConstants.xanaConstants.userName + "_" + XanaConstants.xanaConstants.userId].ToString());
        //            //int totalUsers = int.Parse(documentDictionary["TotalUsers"].ToString());
        //            Debug.LogError("EventCount: " + eventCount);
        //            eventCount++;
        //            totalEventCount++;
        //            //totalUsers++;
        //            Dictionary<string, object> user = new Dictionary<string, object>
        //        {
        //            { "User_"+XanaConstants.xanaConstants.userName+"_"+XanaConstants.xanaConstants.userId, eventCount},
        //            { "Total_Event_Count",totalEventCount},
        //            //{ "EventCount", eventCount },
        //            //{ "TotalUsers", totalUsers },
        //        };
        //            document.UpdateAsync(user).ContinueWithOnMainThread(task =>
        //            {
        //                Debug.Log("Updated data to the " + _eventName.ToString() + " document in the Xana Users collection.");
        //            });

        //        }
        //        else
        //        {
        //            Dictionary<string, object> user = new Dictionary<string, object>
        //        {
        //            { "Total_Event_Count",1},
        //            { "User_"+XanaConstants.xanaConstants.userName+"_"+XanaConstants.xanaConstants.userId, 1},
        //            //{ "EventCount", eventCount },
        //            //{ "TotalUsers", totalUsers },
        //        };

        //            document.UpdateAsync(user).ContinueWithOnMainThread(task =>
        //            {
        //                Debug.Log("Updated data to the " + _eventName.ToString() + " document in the Xana Users collection.");
        //            });
        //        }



        //    }
        //    else
        //    {
        //        Dictionary<string, object> user = new Dictionary<string, object>
        //        {
        //            { "Total_Event_Count",1},
        //            { "User_"+XanaConstants.xanaConstants.userName+"_"+XanaConstants.xanaConstants.userId, 1},
        //            //{ "EventCount", eventCount },
        //            //{ "TotalUsers", totalUsers },
        //        };

        //        document.UpdateAsync(user).ContinueWithOnMainThread(task =>
        //        {
        //            Debug.Log("Added data to the " + _eventName.ToString() + " document in the Xana Users collection.");
        //        });
        //        //Debug.Log(String.Format("Document {0} does not exist!", snapshot.Id));
        //    }
        //});
    }


    #endregion

}
