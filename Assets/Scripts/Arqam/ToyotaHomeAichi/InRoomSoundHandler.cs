using UnityEngine;
using UnityEngine.Events;

public class InRoomSoundHandler : MonoBehaviour
{
    [SerializeField] string roomName = "";
    public static UnityAction<bool, string> playerInRoom;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "PhotonLocalPlayer")
            playerInRoom?.Invoke(true, roomName);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "PhotonLocalPlayer")
            playerInRoom?.Invoke(false, roomName);       
    }

}
