using Photon.Pun;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;

public class VoiceManager : MonoBehaviourPunCallbacks
{
    public Recorder recorder;
    private byte currentGroup;
    public void SetVoiceGroup(byte newGroup)
    {
        byte oldGroup = currentGroup;
        currentGroup = newGroup;

        // Set the recorder's interest group to the new group
        recorder.InterestGroup = newGroup;

        // Change the groups: unsubscribe from the old group and subscribe to the new group
        if (PhotonVoiceNetwork.Instance.Client.InRoom)
            ChangeGroups(new byte[] { oldGroup }, new byte[] { newGroup });
        else
            Debug.Log("Not connected to Game Server. Cannot change groups.");
    }

    private void ChangeGroups(byte[] groupsToLeave, byte[] groupsToJoin)
    {
        if (PhotonVoiceNetwork.Instance.Client.InRoom)
        {
            PhotonVoiceNetwork.Instance.Client.OpChangeGroups(groupsToLeave, groupsToJoin);
        }
        else
        {
            Debug.Log("Operation ChangeGroups not allowed because the client is not connected to the Game Server.");
        }
    }
}
