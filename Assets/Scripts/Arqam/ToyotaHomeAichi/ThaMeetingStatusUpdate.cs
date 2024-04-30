using UnityEngine;
using Photon.Pun;
using static ThaMeetingStatusUpdate;

public class ThaMeetingStatusUpdate : MonoBehaviour
{
    public enum MeetingStatus { End, Inprogress, HouseFull }
    public MeetingStatus tms = MeetingStatus.End;


    public void UpdateMeetingParams(int status)
    {
        //tms = (MeetingStatus)status;
        this.GetComponent<PhotonView>().RPC(nameof(StartMeeting), RpcTarget.AllBuffered, status);
    }


    [PunRPC]
    public void StartMeeting(int num) //, int ViewID
    {
        tms = (MeetingStatus)num;
    }

}
