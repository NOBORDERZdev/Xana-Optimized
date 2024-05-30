using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using System.Security.Cryptography;
using UnityEngine;

public class VoiceManager : MonoBehaviourPunCallbacks
{
    public Recorder recorder;
    //public Speaker speaker;
    private byte currentGroup;

    void Start()
    {
        BuilderEventManager.AfterPlayerInstantiated += UpdateVoiceGroup;
    }
    private void OnDisable()
    {
        BuilderEventManager.AfterPlayerInstantiated -= UpdateVoiceGroup;
    }

    private void UpdateVoiceGroup()
    {
        if (recorder == null /*|| speaker == null*/)
        {
            Debug.LogError("Recorder or Speaker component is missing.");
            return;
        }

        if (PhotonNetwork.InRoom)
        {
            SetVoiceGroup(1); // Default group for all users initially
        }
    }

    public void SetVoiceGroup(byte newGroup)
    {
        byte oldGroup = currentGroup;
        currentGroup = newGroup;

        // Set the recorder's interest group to the new group
        recorder.InterestGroup = newGroup;

        // Change the groups: unsubscribe from the old group and subscribe to the new group
        ChangeGroups(new byte[] { oldGroup }, new byte[] { newGroup });
    }

    private void ChangeGroups(byte[] groupsToLeave, byte[] groupsToJoin)
    {
        PhotonVoiceNetwork.Instance.Client.OpChangeGroups(groupsToLeave, groupsToJoin);
    }

    //[PunRPC]
    //public void ChangePlayerVoiceGroup(int playerID, byte group)
    //{
    //    if (PhotonNetwork.LocalPlayer.ActorNumber == playerID)
    //    {
    //        SetVoiceGroup(group);
    //    }
    //}

    //// Method to call from the game logic to change voice groups for specific players
    //public void ChangeVoiceGroupForPlayer(Player player, byte newGroup)
    //{
    //    photonView.RPC("ChangePlayerVoiceGroup", player, player.ActorNumber, newGroup);
    //}

    //public override void OnJoinedRoom()
    //{
    //    base.OnJoinedRoom();
    //    SetVoiceGroup(0); // Set default group when joined room
    //}


}
