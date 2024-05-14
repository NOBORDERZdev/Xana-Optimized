using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;
using Photon.Pun.Demo.PunBasics;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine.Networking;

public class ThaMeetingStatusUpdate : MonoBehaviourPunCallbacks
{
    public enum MeetingStatus { End, Inprogress, HouseFull }
    [SerializeField]
    public MeetingStatus tms;

    private const string MeetingStatusPropertyName = "MeetingStatus";
    public int roomID = 4;
    public int playerCount;
    //private void OnEnable()
    //{
    //    MutiplayerController.instance.playerJoined += NewPlayerSpawned;
    //}
    //private void OnDisable()
    //{
    //    MutiplayerController.instance.playerJoined -= NewPlayerSpawned;
    //}
    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            CheckUsersCount();
        }
    }
    public void UpdateMeetingParams(int status)
    {
        this.GetComponent<PhotonView>().RPC(nameof(StartMeeting), RpcTarget.AllBuffered, status);

        // Update the custom property for all players in the room
        Hashtable hash = new Hashtable();
        hash[MeetingStatusPropertyName] = status;
        PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);
        Debug.LogError("RoomProperty Update");
        // Check if the meeting status property was updated
        if (propertiesThatChanged.ContainsKey(MeetingStatusPropertyName))
        {
            tms = (MeetingStatus)(int)propertiesThatChanged[MeetingStatusPropertyName];
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        NewPlayerSpawned();
    }

    private void NewPlayerSpawned()
    {
        Debug.LogError("New Player join room:::");

        //if (ConstantsHolder.xanaConstants.meetingStatus != ConstantsHolder.MeetingStatus.End) return;

        // Check if the meeting status property was updated
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(MeetingStatusPropertyName))
        {
            int parameterValue = (int)PhotonNetwork.CurrentRoom.CustomProperties[MeetingStatusPropertyName];
            Debug.LogError("New Player join room:::" + parameterValue);
            this.GetComponent<PhotonView>().RPC(nameof(StartMeeting), RpcTarget.AllBuffered, parameterValue);
        }
        else
            Debug.LogError("Property not exist::");
    }

    [PunRPC]
    public void StartMeeting(int num)
    {
        tms = (MeetingStatus)num;
        ConstantsHolder.xanaConstants.meetingStatus = (ConstantsHolder.MeetingStatus)(num);
    }
    public async void CheckUsersCount()
    {
        StringBuilder ApiURL = new StringBuilder();
        ApiURL.Append(ConstantsGod.API_BASEURL + ConstantsGod.getmeetingroomcount + roomID);
        Debug.Log("API URL is : " + ApiURL.ToString());
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
                playerCount = meetingRoomStatusResponse.data.Count;
                Debug.Log("player count is :: " + playerCount);
                if (playerCount == 0)
                {
                    NFT_Holder_Manager.instance.meetingStatus.tms = ThaMeetingStatusUpdate.MeetingStatus.End;
                }
                else if (playerCount == 1)
                {
                    NFT_Holder_Manager.instance.meetingStatus.tms = ThaMeetingStatusUpdate.MeetingStatus.Inprogress;
                }
                else if (playerCount == 2)
                {
                    NFT_Holder_Manager.instance.meetingStatus.tms = ThaMeetingStatusUpdate.MeetingStatus.HouseFull;
                }
            }
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
