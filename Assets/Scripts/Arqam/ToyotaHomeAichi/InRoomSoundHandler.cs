using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
public class InRoomSoundHandler : MonoBehaviour
{
    public enum TriggerType { RoomTrigger, SoundTrigger }
    public TriggerType triggerType;
    [Space(5)]
    [SerializeField] string roomName = "";
    public static UnityAction<bool, string> playerInRoom;
    public static UnityAction<bool> soundAction;
    private int _roomId = 4;
    public int PlayerCount;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PhotonLocalPlayer" && other.GetComponent<PhotonView>().IsMine)
        {
            if (triggerType.Equals(TriggerType.RoomTrigger))
                playerInRoom?.Invoke(true, roomName);
            else if (triggerType.Equals(TriggerType.SoundTrigger))
                soundAction?.Invoke(true);
            if (roomName == "Home" && triggerType.Equals(TriggerType.RoomTrigger))
            {
                TextUpdate();
            }
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
            if (roomName == "Home" && triggerType.Equals(TriggerType.RoomTrigger))
            {
                if (NFT_Holder_Manager.instance.meetingTxtUpdate != null)
                    NFT_Holder_Manager.instance.meetingTxtUpdate.MeetingRoomText.text = "";
            }
        }
    }
    void TextUpdate()
    {
        if (NFT_Holder_Manager.instance.meetingTxtUpdate == null) return;

        if (NFT_Holder_Manager.instance.meetingStatus.ThaMeetingStatus.Equals(ThaMeetingStatusUpdate.MeetingStatus.Inprogress))
        {
            if (GameManager.currentLanguage.Contains("en") && !LocalizationManager.forceJapanese)
            {
                NFT_Holder_Manager.instance.meetingTxtUpdate.UpdateMeetingTxt("Waiting For Interviewer");
            }
            else if (GameManager.currentLanguage == "ja")
            {
                NFT_Holder_Manager.instance.meetingTxtUpdate.UpdateMeetingTxt("面接官を待っています");
            }
            //Debug.LogError("Join Meeting Now!");
        }
        else if (NFT_Holder_Manager.instance.meetingStatus.ThaMeetingStatus.Equals(ThaMeetingStatusUpdate.MeetingStatus.End))
        {
            if (GameManager.currentLanguage.Contains("en") && !LocalizationManager.forceJapanese)
            {
                NFT_Holder_Manager.instance.meetingTxtUpdate.UpdateMeetingTxt("Join Meeting Now!");
            }
            else if (GameManager.currentLanguage == "ja")
            {
                NFT_Holder_Manager.instance.meetingTxtUpdate.UpdateMeetingTxt("今すぐミーティングに参加してください!");
            }
            //Debug.LogError("Meeting Ended");
        }
        else if (NFT_Holder_Manager.instance.meetingStatus.ThaMeetingStatus.Equals(ThaMeetingStatusUpdate.MeetingStatus.HouseFull))
        {
            if (GameManager.currentLanguage.Contains("en") && !LocalizationManager.forceJapanese)
            {
                NFT_Holder_Manager.instance.meetingTxtUpdate.UpdateMeetingTxt("Meeting Is In Progress");
            }
            else if (GameManager.currentLanguage == "ja")
            {
                NFT_Holder_Manager.instance.meetingTxtUpdate.UpdateMeetingTxt("会議が進行中です");
            }
            //Debug.LogError("House Full");
        }
    }


}
