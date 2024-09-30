using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
public class MeetingRoomStatusData : MonoBehaviour
{
    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject meetingObj = Resources.Load("ThaMeetingObj") as GameObject;
            meetingObj = PhotonNetwork.InstantiateRoomObject(meetingObj.name, new Vector3(0f, 0f, 0f), Quaternion.identity);
            NFT_Holder_Manager.instance.meetingStatus = meetingObj.GetComponent<ThaMeetingStatusUpdate>();
            //Debug.LogError("Instantiate Meeting Object");
        }
        else if (NFT_Holder_Manager.instance.meetingStatus == null)
            NFT_Holder_Manager.instance.meetingStatus = FindObjectOfType<ThaMeetingStatusUpdate>();

        NFT_Holder_Manager.instance.SetChatRefrence();

        // send Space_Entry_UniqueUsers_Mobile_App
        GlobalConstants.SendFirebaseEvent(GlobalConstants.FirebaseTrigger.SE_UU_Mobile_App_THA.ToString());
    }
}
