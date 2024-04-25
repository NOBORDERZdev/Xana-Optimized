using UnityEngine;
using Photon.Pun;
using System;

public class InterRoomCommunication : MonoBehaviour
{
    public static InterRoomCommunication obj;
    public static Action<int> OnUserJoinInterviewRoom; // Event for user joining interview room
    public static Action<int> OnUserLeaveInterviewRoom; // Event for user leaving interview room

    private void Awake()
    {
        if (obj == null)
            obj = this;
        if (obj != null)
            Destroy(this.gameObject);
    }

    public void BroadcastUserJoinInterviewRoom(int userId)
    {
        ArrowManager.Instance.UserJoinInterviewRoom(userId);
    }

    public void BroadcastUserLeaveInterviewRoom(int userId)
    {
        ArrowManager.Instance.UserLeaveInterviewRoom(userId);
    }


}
