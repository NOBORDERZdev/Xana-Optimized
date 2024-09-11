using Photon.Pun;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.Rendering;

public class VoiceManager : MonoBehaviourPunCallbacks
{
    public Recorder recorder;
    private byte currentGroup;
    private void Start()
    {
        BuilderEventManager.AfterPlayerInstantiated += AssignReference;
    }
    private void OnDisable()
    {
        BuilderEventManager.AfterPlayerInstantiated -= AssignReference;
    }
    private void AssignReference()
    {
        NFT_Holder_Manager.instance.voiceManager = this;
        recorder = GameObject.FindObjectOfType<Recorder>();
    }
    public void SetVoiceGroup(byte newGroup)
    {
        byte oldGroup = currentGroup;
        currentGroup = newGroup;

        // Set the recorder's interest group to the new group
        recorder.InterestGroup = newGroup;

        // Change the groups: unsubscribe from the old group and subscribe to the new group
        if (PunVoiceClient.Instance.Client.InRoom)
            ChangeGroups(new byte[] { oldGroup }, new byte[] { newGroup });
        else
            Debug.LogError("Not connected to Game Server. Cannot change groups.");
    }

    private void ChangeGroups(byte[] groupsToLeave, byte[] groupsToJoin)
    {
        if (PunVoiceClient.Instance.Client.InRoom)
        {
            PunVoiceClient.Instance.Client.OpChangeGroups(groupsToLeave, groupsToJoin);
        }
        else
        {
            Debug.LogError("Operation ChangeGroups not allowed because the client is not connected to the Game Server.");
        }
    }
}
