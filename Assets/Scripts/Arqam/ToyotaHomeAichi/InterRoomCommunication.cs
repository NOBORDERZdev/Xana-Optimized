using UnityEngine;
using Photon.Pun;
using System;

public class InterRoomCommunication : MonoBehaviour
{
    public static event Action<int> OnUserJoinInterviewRoom; // Event for user joining interview room
    public static event Action<int> OnUserLeaveInterviewRoom; // Event for user leaving interview room


    public void BroadcastUserJoinInterviewRoom(int userId)
    {
        PhotonView.Get(this).RPC("RPC_UserJoinInterviewRoom", RpcTarget.Others, userId);
    }

    public void BroadcastUserLeaveInterviewRoom(int userId)
    {
        PhotonView.Get(this).RPC("RPC_UserLeaveInterviewRoom", RpcTarget.Others, userId);
    }

    [PunRPC]
    private void RPC_UserJoinInterviewRoom(int userId)
    {
        OnUserJoinInterviewRoom?.Invoke(userId);
    }

    [PunRPC]
    private void RPC_UserLeaveInterviewRoom(int userId)
    {
        OnUserLeaveInterviewRoom?.Invoke(userId);
    }


}
