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
        URL_FiveElements,
        // Added Toyota Items
        THA_Home_Thumbnail,
        THA_Home_Thumbnail_PlayBtn,
        StayTime_THA,
        CL_NFT_THA_Stage,
        CL_NFT_THA_Factory,
        CL_NFT_THA_Consult,
        CL_NFT_THA_LandInfo,
        CL_NFT_THA_Architec,
        URL_Stage,
        URL_Factory,
        URL_Consult,
        URL_LandInfo,
        URL_Architec,
        SE_UU_Mobile_App_THA,

        // Added Xana Items
        App_Started,

        Login_Email_Success,
        Signup_Email_Completed,

        Login_Wallet_Success,
        Signup_Wallet_Completed,
        
        Join_World,
        StayTime,  
    }

    public static void SendFirebaseEvent(string eventName)
    {
        if (eventName.IsNullOrEmpty() || eventName.Substring(0) == "_") return;

        string prefix = "T_";

        if (APIBasepointManager.instance.IsXanaLive)
        {
            prefix = "L_"; environmentType = EnvironmentType.Live; 
        }
        eventName = prefix + eventName;
        Debug.Log("<color=red>FB Event: " + eventName + "</color>");

        Firebase.Analytics.FirebaseAnalytics.LogEvent(eventName);
    }
    public static void SendFirebaseEventForSummit(string eventName,string userId="")
    {
        if (eventName.IsNullOrEmpty() || eventName.Substring(0) == "_") return;

        string prefix = "TAU_";

        if (APIBasepointManager.instance.IsXanaLive)
        {
            prefix = "LA_"; environmentType = EnvironmentType.Live;
        }
        eventName = prefix + eventName;
        if (userId == "" || ConstantsHolder.xanaConstants.LoggedInAsGuest)
        {
            Debug.LogError("<color=red>FB Event: " + eventName + "</color>");
            Firebase.Analytics.FirebaseAnalytics.LogEvent(eventName);
        }
        else
        {
            Debug.LogError("<color=red>FB Event: " + eventName +" "+userId + "</color>");
            Firebase.Analytics.FirebaseAnalytics.LogEvent(eventName,userId,"");
        }
    }
    #endregion

}
