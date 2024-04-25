using UnityEngine;
using TMPro;
public class ThaMeetingTxtUpdate : MonoBehaviour
{
    public TextMeshProUGUI tmp;

    // Start is called before the first frame update
    void Start()
    {
        tmp.text = "Meeting Room";

        InterRoomCommunication.OnUserJoinInterviewRoom += HandleUserJoinInterviewRoom;
        InterRoomCommunication.OnUserLeaveInterviewRoom += HandleUserLeaveInterviewRoom;
    }

    private void OnDestroy()
    {
        InterRoomCommunication.OnUserJoinInterviewRoom -= HandleUserJoinInterviewRoom;
        InterRoomCommunication.OnUserLeaveInterviewRoom -= HandleUserLeaveInterviewRoom;
    }

    private void HandleUserJoinInterviewRoom(int userId)
    {
        // Handle user joining interview room
    }

    private void HandleUserLeaveInterviewRoom(int userId)
    {
        Debug.LogError("UserId: " + userId);
        // Handle user leaving interview room
        UpdateMeetingTxt("Meeting Room", Color.blue);
    }

    public void UpdateMeetingTxt(string data, Color txtColor = default)
    {
        tmp.text = data;
        tmp.color = txtColor;
    }
 
}
