using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ConstantsHolder;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class ThaMeetingStatusUpdate : MonoBehaviour
{
    // Start is called before the first frame update
    //void Start()
    //{
    //    Hashtable hash = new Hashtable();
    //    if (hash.ContainsKey("MeetingStatus")){
    //        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("MeetingStatus"))
    //        {
    //            ConstantsHolder.xanaConstants.toyotaMeetingStatus = (MeetingStatus)(int)PhotonNetwork.LocalPlayer.CustomProperties["MeetingStatus"];
    //            Debug.LogError("Num:: " + (int)PhotonNetwork.LocalPlayer.CustomProperties["MeetingStatus"]);
    //        }
    //    }
    //    else
    //        Debug.LogError("Meeting Status Not Exist");
    //}

    //public void UpdateMeetingStatus()
    //{
    //    Hashtable hash = new Hashtable();
    //    hash["MeetingStatus"] = (int)ConstantsHolder.xanaConstants.toyotaMeetingStatus;
    //    PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
    //    Debug.LogError("Num:: " + (int)ConstantsHolder.xanaConstants.toyotaMeetingStatus);
    //}
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("MeetingStatus"))
        {
            object status = PhotonNetwork.LocalPlayer.CustomProperties["MeetingStatus"];
            if (status is int)
            {
                ConstantsHolder.xanaConstants.toyotaMeetingStatus = (MeetingStatus)(int)status;
                Debug.LogError("Meeting Status:: " + (int)status); 
            }
        }
        else
        {
            Debug.LogError("Meeting Status Not Exist");
        }
    }

    public void UpdateMeetingStatus()
    {
        Hashtable properties = new Hashtable
        {
            ["MeetingStatus"] = (int)ConstantsHolder.xanaConstants.toyotaMeetingStatus
        };

        if (PhotonNetwork.CurrentRoom != null)
        {
            PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
            Debug.LogError("Meeting Status Updated: " + (int)ConstantsHolder.xanaConstants.toyotaMeetingStatus);
        }
        else
        {
            Debug.LogError("Attempted to set custom properties on a null room.");
        }
    }

}
