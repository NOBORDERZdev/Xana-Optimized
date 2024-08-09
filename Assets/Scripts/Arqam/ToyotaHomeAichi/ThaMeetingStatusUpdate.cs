using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Text;
using System;

public class ThaMeetingStatusUpdate : MonoBehaviourPunCallbacks
{
    public enum MeetingStatus { End, Inprogress, HouseFull }
    [SerializeField]
    public MeetingStatus ThaMeetingStatus;

    //private const string MeetingStatusPropertyName = "MeetingStatus";
    private const int _roomID = 4;
    private int _playerCount;
    private PhotonView _pv;

    private void Start()
    {
        _pv = GetComponent<PhotonView>();
        if (PhotonNetwork.IsMasterClient)
        {
            ConstantsHolder.xanaConstants.meetingStatus = ConstantsHolder.MeetingStatus.End;
        }
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
        //BuilderEventManager.AfterPlayerInstantiated -= GetPlayerCount;
    }

    public void UpdateMeetingParams(int status)
    {
        this.GetComponent<PhotonView>().RPC(nameof(StartMeeting), RpcTarget.All, status);
        // Update the custom property for all players in the room
        //Hashtable hash = new Hashtable();
        //hash[MeetingStatusPropertyName] = status;
        //PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
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
        // if(FB_Notification_Initilizer.Instance.actorType == (ConstantsHolder.MeetingStatus)(num))
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
        if ((otherPlayer.ActorNumber == FB_Notification_Initilizer.Instance.userActorNum
            || otherPlayer.ActorNumber == FB_Notification_Initilizer.Instance.toyotaUserActorNum))
        {
            UpdateStatusOnLeft();
        }
    }

    private void UpdateStatusOnLeft()
    {
            int temp = FB_Notification_Initilizer.Instance.userInMeeting - 1;
            NFT_Holder_Manager.instance.meetingStatus.UpdateUserCounter(temp);
            Debug.LogError("3Left Room::" + temp);
            if (FB_Notification_Initilizer.Instance.userInMeeting <= 0)
            {
                NFT_Holder_Manager.instance.meetingStatus.UpdateMeetingParams((int)MeetingStatus.End);
                if (GameManager.currentLanguage.Contains("en") && !LocalizationManager.forceJapanese)
                {
                    NFT_Holder_Manager.instance.meetingTxtUpdate.UpdateMeetingTxt("Join Meeting Now!");
                }
                else if (GameManager.currentLanguage == "ja")
                {
                    NFT_Holder_Manager.instance.meetingTxtUpdate.UpdateMeetingTxt("会議室利用可");
                }
            }
    }

    private void NewPlayerSpawned()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (_pv != null)
            {
                Debug.LogError("Joined Room::" + FB_Notification_Initilizer.Instance.userInMeeting);
                _pv.RPC(nameof(StartMeeting), RpcTarget.All, (int)NFT_Holder_Manager.instance.meetingStatus.ThaMeetingStatus);
                _pv.RPC(nameof(SetMeetingCounter), RpcTarget.All, FB_Notification_Initilizer.Instance.userInMeeting);
            }
            else
            {
                if (NFT_Holder_Manager.instance && NFT_Holder_Manager.instance.meetingStatus)
                {
                    Debug.LogError("Joined Room::" + FB_Notification_Initilizer.Instance.userInMeeting);
                    NFT_Holder_Manager.instance.meetingStatus.GetComponent<PhotonView>().RPC(nameof(StartMeeting), RpcTarget.All, (int)NFT_Holder_Manager.instance.meetingStatus.ThaMeetingStatus);
                    NFT_Holder_Manager.instance.meetingStatus.GetComponent<PhotonView>().RPC(nameof(SetMeetingCounter),
                        RpcTarget.All, FB_Notification_Initilizer.Instance.userInMeeting);
                }
            }
        }
        if (_pv != null)
            _pv.RPC(nameof(UpdatePortal), RpcTarget.All);
        else
        {
            if (NFT_Holder_Manager.instance && NFT_Holder_Manager.instance.meetingStatus)
                NFT_Holder_Manager.instance.meetingStatus.GetComponent<PhotonView>().RPC(nameof(UpdatePortal), RpcTarget.All);
        }
    }

    [PunRPC]
    public void StartMeeting(int num)
    {
        ThaMeetingStatus = (MeetingStatus)num;
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
