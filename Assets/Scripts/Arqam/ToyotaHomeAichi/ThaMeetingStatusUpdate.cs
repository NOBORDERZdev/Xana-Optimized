using UnityEngine;

public class ThaMeetingStatusUpdate : MonoBehaviour
{
    public enum MeetingStatus { End, Inprogress, HouseFull }
    public MeetingStatus tms = MeetingStatus.End;


    public void UpdateMeetingParams(int status)
    {
        tms = (MeetingStatus)status;
    }


}
