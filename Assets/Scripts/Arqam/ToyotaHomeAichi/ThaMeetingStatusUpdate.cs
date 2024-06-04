using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine.Networking;
using Unity.VisualScripting;

public class ThaMeetingStatusUpdate : MonoBehaviourPunCallbacks
{
    public enum MeetingStatus { End, Inprogress, HouseFull }
    [SerializeField]
    public MeetingStatus tms;

    private const int roomID = 4;
    private int playerCount;
    private PhotonView pv;

    private void Start()
    {
        pv = GetComponent<PhotonView>();
        if (PhotonNetwork.IsMasterClient)
        {
            CheckAndUpdateMeetingStatus();
            ConstantsHolder.xanaConstants.meetingStatus = ConstantsHolder.MeetingStatus.End;
        }
    }
    private void CheckAndUpdateMeetingStatus()
    {

    }

    public void UpdateMeetingParams(int status)
    {
        this.GetComponent<PhotonView>().RPC(nameof(StartMeeting), RpcTarget.All, status);
    }

    public void UpdateUserCounter(int countPram)
    {
        this.GetComponent<PhotonView>().RPC(nameof(SetMeetingCounter), RpcTarget.All, countPram);
    }

    public void GetActorNum(int num, int actorType)
    {
        this.GetComponent<PhotonView>().RPC(nameof(SetActorNum), RpcTarget.All, num, actorType);
    }

    [PunRPC]
    private void SetMeetingCounter(int count)
    {
        FB_Notification_Initilizer.Instance.userInMeeting = count;
    }

    [PunRPC]
    private void SetActorNum(int actorNum, int actorTypeIndex)
    {
        if (actorTypeIndex == 0)
            FB_Notification_Initilizer.Instance.userActorNum = actorNum;
        else if (actorTypeIndex == 1)
            FB_Notification_Initilizer.Instance.toyotaUserActorNum = actorNum;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        NewPlayerSpawned();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if(otherPlayer.ActorNumber == FB_Notification_Initilizer.Instance.userActorNum 
            || otherPlayer.ActorNumber == FB_Notification_Initilizer.Instance.toyotaUserActorNum)
        {
            int temp = FB_Notification_Initilizer.Instance.userInMeeting - 1;
            NFT_Holder_Manager.instance.meetingStatus.UpdateUserCounter(temp);
            if (FB_Notification_Initilizer.Instance.userInMeeting <= 0)
            {
                NFT_Holder_Manager.instance.meetingStatus.UpdateMeetingParams((int)MeetingStatus.End);
                NFT_Holder_Manager.instance.meetingTxtUpdate.UpdateMeetingTxt("Join Meeting Now!");
            }
        }
    }

    private void NewPlayerSpawned()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (pv != null)
            {
                pv.RPC(nameof(StartMeeting), RpcTarget.All, (int)NFT_Holder_Manager.instance.meetingStatus.tms);
                pv.RPC(nameof(SetMeetingCounter), RpcTarget.All, FB_Notification_Initilizer.Instance.userInMeeting);
            }
            else
            {
                if (NFT_Holder_Manager.instance && NFT_Holder_Manager.instance.meetingStatus)
                {
                    NFT_Holder_Manager.instance.meetingStatus.GetComponent<PhotonView>().RPC(nameof(StartMeeting), RpcTarget.All, (int)NFT_Holder_Manager.instance.meetingStatus.tms);
                    NFT_Holder_Manager.instance.meetingStatus.GetComponent<PhotonView>().RPC(nameof(SetMeetingCounter),
                        RpcTarget.All, FB_Notification_Initilizer.Instance.userInMeeting);
                }
            }
        }
        
        if (pv != null)
            pv.RPC(nameof(UpdatePortal), RpcTarget.All);
        else
        {
            if (NFT_Holder_Manager.instance && NFT_Holder_Manager.instance.meetingStatus)
                NFT_Holder_Manager.instance.meetingStatus.GetComponent<PhotonView>().RPC(nameof(UpdatePortal), RpcTarget.All);
        }
    }

    [PunRPC]
    public void StartMeeting(int num)
    {
        tms = (MeetingStatus)num;
        ConstantsHolder.xanaConstants.meetingStatus = (ConstantsHolder.MeetingStatus)(num);
    }

    [PunRPC]
    public void UpdatePortal()
    {
        if (NFT_Holder_Manager.instance && NFT_Holder_Manager.instance.meetingTxtUpdate != null)
            NFT_Holder_Manager.instance.meetingTxtUpdate.WrapObjectOnOff();
    }
    public class MeetinRoomProperties
    {
        public int id { get; set; }
        public int worldId { get; set; }
        public string email { get; set; }
        public bool isJoined { get; set; }
        public bool isCompanyMember { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
    }

    public class MeetingRoomStatusResponse
    {
        public bool success { get; set; }
        public List<MeetinRoomProperties> data { get; set; }
        public string msg { get; set; }
    }
}
