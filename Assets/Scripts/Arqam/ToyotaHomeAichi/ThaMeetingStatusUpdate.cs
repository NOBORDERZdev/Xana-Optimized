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
    void Start()
    {
        Hashtable hash = new Hashtable();
        if (hash.ContainsKey("MeetingStatus")){
            if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("MeetingStatus"))
            {
                ConstantsHolder.xanaConstants.toyotaMeetingStatus = (MeetingStatus)(int)PhotonNetwork.LocalPlayer.CustomProperties["MeetingStatus"];
                Debug.LogError("Num:: " + (int)PhotonNetwork.LocalPlayer.CustomProperties["MeetingStatus"]);
            }
        }
        else
            Debug.LogError("Meeting Status Not Exist");
    }

    public void UpdateMeetingStatus()
    {
        Hashtable hash = new Hashtable();
        hash["MeetingStatus"] = (int)ConstantsHolder.xanaConstants.toyotaMeetingStatus;
        PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
        Debug.LogError("Num:: " + (int)ConstantsHolder.xanaConstants.toyotaMeetingStatus);
    }


}
