using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NPC;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;

public class NpcBehaviourSelector : MonoBehaviourPunCallbacks
{
    [SerializeField] NpcMovementController movementController;
    [SerializeField] NpcJump npcJump;
    [SerializeField] NpcSelfie npcSelfie;
    [SerializeField] NpcFreeCam npcFreeCam;
    [SerializeField] NpcEmotes npcEmotes;
    [Space(5)]
    [SerializeField] TMP_Text nameTxt;

    [HideInInspector]
    public bool isPerformingAction = false;
    [HideInInspector]
    public Coroutine ActionCoroutine = null;

    private Coroutine emoteCoroutine;
    private bool isNewlySpwaned = true;
    private int maxNpcBehaviourAction = 6;
    private System.Action<int> MasterclientChanged;
    private void OnEnable()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(PerformAction());
            this.GetComponent<PhotonView>().RPC("SetNameRPC", RpcTarget.AllBuffered, this.GetComponent<PhotonView>().ViewID, nameTxt.text);
        }
        MasterclientChanged += OnMasterChange;
    }

    private void OnDisable()
    {
        MasterclientChanged -= OnMasterChange;
    }

    void OnMasterChange(int viewID)
    {
        StartCoroutine(PerformAction());
    }

    public IEnumerator PerformAction()
    {
        if (!isPerformingAction)
        {
            isPerformingAction = true;
            int rand;
            if (isNewlySpwaned)
            {
                rand = 0;
                isNewlySpwaned = false;
            }
            else
                rand = Random.Range(1, maxNpcBehaviourAction);


            switch (rand)
            {
                case 0:                     // for wandering ai
                    movementController.Wander();
                    npcSelfie.ForceFullyDisableSelfie();

                    StopEmotes();
                    break;
                case 1:                   // for jump
                    npcJump.AiJump();

                    StopEmotes();
                    break;
                case 2:                    // for selfie control
                    npcSelfie.SelfieAction();

                    StopEmotes();
                    break;
                case 3:                  // for freeCam mode
                    npcFreeCam.PerformFreeCam();

                    StopEmotes();
                    break;
                case 4:                   // for emotes display
                    if (emoteCoroutine != null)
                        StopCoroutine(emoteCoroutine);
                    emoteCoroutine = StartCoroutine(npcEmotes.PlayEmote());
                    npcSelfie.ForceFullyDisableSelfie();
                    break;
                default:                // for default case
                    movementController.Wander();
                    npcSelfie.ForceFullyDisableSelfie();

                    StopEmotes();
                    break;
            }
        }
        yield return null;
    }

    private void StopEmotes()
    {
        if (emoteCoroutine != null)
            StopCoroutine(emoteCoroutine);

        npcEmotes.ForceFullyStopEmote();
    }

    public void SetAiName(string name)
    {
        nameTxt.text = name;
    }

    [Photon.Pun.PunRPC]
    void SetNameRPC(int viewID, string name)
    {
        Debug.LogError("Set name rpc called :- " + viewID + "- - - " + name);

        if (this.GetComponent<PhotonView>().ViewID == viewID)
            nameTxt.text = name;
    }

    public void OnPlayerEnteredRoom(Player newPlayer)
    {
        throw new System.NotImplementedException();
    }

    public void OnPlayerLeftRoom(Player otherPlayer)
    {
        throw new System.NotImplementedException();
    }

    public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        throw new System.NotImplementedException();
    }

    public void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        throw new System.NotImplementedException();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        //base.OnMasterClientSwitched(newMasterClient);
        Debug.LogError("OnMasterClientSwitched called");
       // MasterclientChanged?.Invoke(newMasterClient.ActorNumber);
    }
}
