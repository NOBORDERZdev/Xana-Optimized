using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class ThaMeetingStatusUpdate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Hashtable hash = new Hashtable();
        if (hash.ContainsKey("MeetingStatus")){
            if (ArrowManager.Instance.GetComponent<PhotonView>().Controller.CustomProperties.TryGetValue("MeetingStatus", out object num))
                ConstantsHolder.xanaConstants.toyotaMeetingStatus = (ConstantsHolder.MeetingStatus)num;
        }
    }

    public void UpdateMeetingStatus()
    {
        Hashtable hash = new Hashtable();
        hash.Add("MeetingStatus", (int)ConstantsHolder.xanaConstants.toyotaMeetingStatus);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }


}
