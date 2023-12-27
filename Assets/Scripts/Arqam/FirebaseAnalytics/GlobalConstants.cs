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

        URL_ZoneX,
        URL_FiveElements,
        URL_AtomRoom,
        URL_AtomRental,

        CL_IMG_ZoneX,
        CL_IMG_FiveElements, // Done


        //Events For PMY Module
        Home_Thumbnail_PMY,
        Home_Thumbnail_PlayBtn_PMY,

        StayTime_PMYLobby,
        StayTime_CRoom1,
        StayTime_CRoom2,
        StayTime_PMYGallery,

        Corporate_Room,
        Gallery,

        CL_NFT_PMYLobby,
        CL_NFT_CRoom1,
        CL_NFT_CRoom2,
        CL_NFT_Gallery,

        URL_PMYLobby,
        URL_CRoom1,
        URL_CRoom2,
        URL_Gallery
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
        Debug.Log("<color=red>FbEvents:" + eventName + "</color>");

        Firebase.Analytics.FirebaseAnalytics.LogEvent(eventName);
    }
    #endregion

}
