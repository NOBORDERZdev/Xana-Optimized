using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;

public class ThaMeetingStatusUpdate : MonoBehaviourPunCallbacks
{
    public enum MeetingStatus { End, Inprogress, HouseFull }
    [SerializeField]
    public MeetingStatus tms;

    private const string MeetingStatusPropertyName = "MeetingStatus";

    public void UpdateMeetingParams(int status)
    {
        //tms = (MeetingStatus)status;
        //this.GetComponent<PhotonView>().RPC(nameof(StartMeeting), RpcTarget.AllBuffered, status);
        StartMeeting(status);

        // Update the custom property for all players in the room
        Hashtable hash = new Hashtable();
        hash[MeetingStatusPropertyName] = status;
        PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);

        // Check if the meeting status property was updated
        if (propertiesThatChanged.ContainsKey(MeetingStatusPropertyName))
        {
            tms = (MeetingStatus)(int)propertiesThatChanged[MeetingStatusPropertyName];
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.LogError("New Player join room");
        // Send the current meeting status to the new player
        Hashtable hash = new Hashtable();
        hash[MeetingStatusPropertyName] = (int)tms;
        newPlayer.SetCustomProperties(hash);

        ConstantsHolder.xanaConstants.meetingStatus = (ConstantsHolder.MeetingStatus)((int)tms);
    }

    //[PunRPC]
    public void StartMeeting(int num) //, int ViewID
    {
        tms = (MeetingStatus)num;
    }

}
