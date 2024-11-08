using UnityEngine;

public class GlobalConstants
{
    public static EnvironmentType environmentType = EnvironmentType.Test;
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
    }
    #endregion

}
