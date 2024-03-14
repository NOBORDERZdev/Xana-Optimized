using UnityEngine;
using UnityEngine.Events;

public class InRoomSoundHandler : MonoBehaviour
{
    public static UnityAction<bool> playerInRoom;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "PhotonLocalPlayer")
            playerInRoom?.Invoke(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "PhotonLocalPlayer")
            playerInRoom?.Invoke(false);       
    }

}
