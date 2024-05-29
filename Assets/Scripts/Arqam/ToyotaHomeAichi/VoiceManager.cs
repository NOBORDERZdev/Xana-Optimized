using Photon.Pun;
using Photon.Voice.Unity;
using UnityEngine;

public class VoiceManager : MonoBehaviour
{
    public Recorder recorder;

    void Start()
    {
        if (recorder == null)
        {
            Debug.LogError("Recorder or Speaker component is missing.");
            return;
        }

        if (PhotonNetwork.IsConnectedAndReady)
        {
            SetVoiceGroup(0); // Default group for all users initially
        }
    }

    public void SetVoiceGroup(byte group)
    {
        recorder.InterestGroup = group; // Set the group to which this player will broadcast their voice
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
