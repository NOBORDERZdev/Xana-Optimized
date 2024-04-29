using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class ThaMeetingStatusUpdate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Hashtable hash = new Hashtable();
        if (hash.ContainsKey("MeetingStatus")){
            object status;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("MeetingStatus", out status))
            {
                ConstantsHolder.xanaConstants.toyotaMeetingStatus = (ConstantsHolder.MeetingStatus)(int)status;
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
