using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine.Networking;
using System.Collections;
using Unity.VisualScripting;

public class ThaMeetingStatusUpdate : MonoBehaviourPunCallbacks
{
    public enum MeetingStatus { End, Inprogress, HouseFull }
    [SerializeField]
    public MeetingStatus tms;

    //private const string MeetingStatusPropertyName = "MeetingStatus";
    private const int roomID = 4;
    private int playerCount;
    private PhotonView pv;

    private void Start()
    {
        BuilderEventManager.AfterPlayerInstantiated += GetPlayerCount;
        pv = GetComponent<PhotonView>();
    }
    private void OnDisable()
    {
        BuilderEventManager.AfterPlayerInstantiated -= GetPlayerCount;
    }

    public void UpdateMeetingParams(int status)
    {
        this.GetComponent<PhotonView>().RPC(nameof(StartMeeting), RpcTarget.All, status);

        // Update the custom property for all players in the room
        //Hashtable hash = new Hashtable();
        //hash[MeetingStatusPropertyName] = status;
        //PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
    }

    //public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    //{
    //    base.OnRoomPropertiesUpdate(propertiesThatChanged);
    //    Debug.LogError("RoomProperty Update");
    //    // Check if the meeting status property was updated
    //    if (propertiesThatChanged.ContainsKey(MeetingStatusPropertyName))
    //        tms = (MeetingStatus)(int)propertiesThatChanged[MeetingStatusPropertyName];
    //}

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        NewPlayerSpawned();
    }


    private void NewPlayerSpawned()
    {
        Debug.LogError("New Player join room:::");
        //// Check if the meeting status property was updated
        //if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(MeetingStatusPropertyName))
        //{
        //    int parameterValue = (int)PhotonNetwork.CurrentRoom.CustomProperties[MeetingStatusPropertyName];
        //    Debug.LogError("New Player join room:::" + parameterValue);
        if (pv != null)
            pv.RPC(nameof(StartMeeting), RpcTarget.All, playerCount);
        else
        {
            NFT_Holder_Manager.instance.meetingStatus.GetComponent<PhotonView>().RPC(nameof(StartMeeting), RpcTarget.All, playerCount);
            Debug.LogError("PhotonViewNotExist: ");
        }
        //}
        //else
        //    Debug.LogError("Property not exist::");

        if (pv != null)
            pv.RPC(nameof(UpdatePortal), RpcTarget.All);
        else
        {
            NFT_Holder_Manager.instance.meetingStatus.GetComponent<PhotonView>().RPC(nameof(UpdatePortal), RpcTarget.All);
            Debug.LogError("PhotonViewNotExist: ");
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

    private void GetPlayerCount()        // call in start when player join toyota world
    {
        CheckUsersCount(false);
    }
    public void RemoteCheckUserCount()   // call when user exist from meeting
    {
        if (pv != null)
            pv.RPC(nameof(CheckUsersCount), RpcTarget.All, true);
        else
        {
            NFT_Holder_Manager.instance.meetingStatus.GetComponent<PhotonView>().RPC(nameof(CheckUsersCount), RpcTarget.All, true);
            Debug.LogError("PhotonViewNotExist: ");
        }
    }

    [PunRPC]
    private async void CheckUsersCount(bool existFromMeeting)
    {
        StringBuilder ApiURL = new StringBuilder();
        ApiURL.Append(ConstantsGod.API_BASEURL + ConstantsGod.getmeetingroomcount + roomID);
        Debug.LogError("API URL is : " + ApiURL.ToString());
        using (UnityWebRequest request = UnityWebRequest.Get(ApiURL.ToString()))
        {
            request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            await request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log("Error is" + request.error);
            }
            else
            {
                StringBuilder data = new StringBuilder();
                data.Append(request.downloadHandler.text);
                MeetingRoomStatusResponse meetingRoomStatusResponse = JsonConvert.DeserializeObject<MeetingRoomStatusResponse>(data.ToString());

                if (!existFromMeeting)
                {
                    playerCount = meetingRoomStatusResponse.data.Count;
                    Debug.LogError("playerCount: " + playerCount);
                }
                else if (existFromMeeting)
                {
                    //playerCount = playerCount >= 1 ? 2 : playerCount;
                    if (playerCount >= 1)
                    {
                        playerCount = 2;
                        Debug.LogError("Enter");
                    }
                    Debug.LogError("playerCount: " + playerCount + ":" + existFromMeeting);
                }
                
                NFT_Holder_Manager.instance.meetingStatus.tms = (MeetingStatus)(playerCount);
                ConstantsHolder.xanaConstants.meetingStatus = (ConstantsHolder.MeetingStatus)(playerCount);
            }
            TextUpdate();
        }

        void TextUpdate()
        {
            if (NFT_Holder_Manager.instance.meetingTxtUpdate == null) return;

            if (NFT_Holder_Manager.instance.meetingStatus.tms.Equals(MeetingStatus.Inprogress))
                NFT_Holder_Manager.instance.meetingTxtUpdate.UpdateMeetingTxt("Waiting For Interviewer");
            else if (NFT_Holder_Manager.instance.meetingStatus.tms.Equals(MeetingStatus.End))
                NFT_Holder_Manager.instance.meetingTxtUpdate.UpdateMeetingTxt("Join Meeting Now!");
            else if (NFT_Holder_Manager.instance.meetingStatus.tms.Equals(MeetingStatus.HouseFull))
                NFT_Holder_Manager.instance.meetingTxtUpdate.UpdateMeetingTxt("Meeting Is In Progress");
        }
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
