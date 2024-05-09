using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
using System.Collections;

public class InRoomSoundHandler : MonoBehaviour
{
    public enum TriggerType { RoomTrigger, SoundTrigger }
    public TriggerType triggerType;
    [Space(5)]
    [SerializeField] string roomName = "";
    public static UnityAction<bool, string> playerInRoom;
    public static UnityAction<bool> soundAction;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PhotonLocalPlayer" && other.GetComponent<PhotonView>().IsMine)
        {
            if (triggerType.Equals(TriggerType.RoomTrigger))
                playerInRoom?.Invoke(true, roomName);
            else if (triggerType.Equals(TriggerType.SoundTrigger))
                soundAction?.Invoke(true);
            if (roomName.Equals("Home"))
                TextUpdate();
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "PhotonLocalPlayer" && other.GetComponent<PhotonView>().IsMine)
        {
            if (triggerType.Equals(TriggerType.RoomTrigger))
                playerInRoom?.Invoke(false, roomName);
            else if (triggerType.Equals(TriggerType.SoundTrigger))
                soundAction?.Invoke(false);
        }
    }
    void TextUpdate()
    {
        if (NFT_Holder_Manager.instance.meetingStatus.tms.Equals(ThaMeetingStatusUpdate.MeetingStatus.Inprogress))
        {
            NFT_Holder_Manager.instance.meetingTxtUpdate.UpdateMeetingTxt("Waiting For Interviewer");
            Debug.Log("Join Meeting Now!");
        }
        else if (NFT_Holder_Manager.instance.meetingStatus.tms.Equals(ThaMeetingStatusUpdate.MeetingStatus.End))
        {
            NFT_Holder_Manager.instance.meetingTxtUpdate.UpdateMeetingTxt("Join Meeting Now!");
            Debug.Log("Meeting Ended");
        }
        else if (NFT_Holder_Manager.instance.meetingStatus.tms.Equals(ThaMeetingStatusUpdate.MeetingStatus.HouseFull))
        {
            NFT_Holder_Manager.instance.meetingTxtUpdate.UpdateMeetingTxt("Meeting Is In Progress");
            Debug.Log("House Full");
        }
    }

}
