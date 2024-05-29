using Photon.Pun;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;

public class VoiceManager : MonoBehaviour
{
    public Recorder recorder;
    public byte value = 0;

    void Start()
    {
        if (recorder == null)
        {
            Debug.LogError("Recorder or Speaker component is missing.");
            return;
        }

        //if (PhotonNetwork.IsConnectedAndReady)
        //{
        byte group = (byte)(1 % 2);
        SetVoiceGroup(group); // Default group for all users initially
        //}
    }

    public void SetVoiceGroup(byte group)
    {
        value = group;
        Debug.LogError("Set Voice Group: " + group);
        recorder.InterestGroup = group; // Set the group to which this player will broadcast their voice
        PhotonVoiceNetwork.Instance.Client.OpChangeGroups(null, new byte[] { group });
        //PhotonVoiceNetwork.Instance.Client.ChangeGroups(null, new byte[] { group }); // Subscribe to the group to receive others' voices
    }


    //public void ChangePlayerVoiceGroup(byte group)
    //{
    //        SetVoiceGroup(group);
    //}

    //// Method to call from the game logic to change voice groups for specific players
    //public void ChangeVoiceGroupForPlayer(Player player, byte newGroup)
    //{
    //    photonView.RPC("ChangePlayerVoiceGroup", player, player.ActorNumber, newGroup);
    //}


}
