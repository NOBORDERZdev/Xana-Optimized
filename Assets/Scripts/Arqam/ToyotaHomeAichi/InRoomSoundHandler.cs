using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

public class InRoomSoundHandler : MonoBehaviour
{
    [SerializeField] string roomName = "";
    public static UnityAction<bool, string> playerInRoom;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "PhotonLocalPlayer" && other.GetComponent<PhotonView>().IsMine)
            playerInRoom?.Invoke(true, roomName);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "PhotonLocalPlayer")
            playerInRoom?.Invoke(false, roomName);       
    }

}
