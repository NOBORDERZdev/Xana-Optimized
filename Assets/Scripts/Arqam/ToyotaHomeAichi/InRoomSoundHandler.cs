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

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PhotonLocalPlayer" && other.GetComponent<PhotonView>().IsMine)
        {
            if (triggerType.Equals(TriggerType.RoomTrigger))
                playerInRoom?.Invoke(true, roomName);
            else if (triggerType.Equals(TriggerType.SoundTrigger))
                soundAction?.Invoke(true);
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

}
