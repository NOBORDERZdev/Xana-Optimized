using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Types;
using System.Threading.Tasks;
using UnityEngine.Networking;

public class EmailEntryController : MonoBehaviour
{
    public string WorldIdTestnet;
    public string WorldIdMainnet;
    public string WorldId;
    public EmailEntryTPPHandler WarpPointsAndAuthMailHandler;
    private bool alreadyTriggered;

    private void OnEnable()
    {
        WarpPointsAndAuthMailHandler = GetComponentInParent<EmailEntryTPPHandler>();

        if (APIBasepointManager.instance.IsXanaLive)
            WorldId = WorldIdMainnet;
        else
            WorldId = WorldIdTestnet;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (PhotonNetwork.InRoom)
        {
            if (other.tag == "PhotonLocalPlayer" && other.GetComponent<PhotonView>().IsMine && !alreadyTriggered)
            {
                alreadyTriggered = true;
                if (WarpPointsAndAuthMailHandler.IsEmailVerificationReq)
                {
                    GamePlayUIHandler.inst.SummitCXOEmailAuthUIHandle.SetAuthEmailUIState(true);
                    GamePlayUIHandler.inst.SummitCXOEmailAuthUIHandle.AuthEmailAfterVerification += VerifyUserEnteredEmail;
                }
                else
                {
                    TriggerSceneLoading(WorldId);
                    DisableCollider();
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (PhotonNetwork.InRoom)
        {
            if (other.tag == "PhotonLocalPlayer" && other.GetComponent<PhotonView>().IsMine && alreadyTriggered)
            {
                alreadyTriggered = false;
                GamePlayUIHandler.inst.SummitCXOEmailAuthUIHandle.AuthEmailAfterVerification -= VerifyUserEnteredEmail;
            }
        }
    }

    void VerifyUserEnteredEmail(string _userEnteredEmail)
    {

        if (WarpPointsAndAuthMailHandler.AuthEmailData.data != null && WarpPointsAndAuthMailHandler.AuthEmailData.data.Count > 0)
        {
            if (WarpPointsAndAuthMailHandler.AuthEmailData.data.Contains(_userEnteredEmail))
            {
                TriggerSceneLoading(WorldId);
                DisableCollider();
            }
            else
            {
                GamePlayUIHandler.inst.SummitCXOEmailAuthUIHandle.PlayErrorMsgAnim();
            }
        }
        else
        {
            Debug.LogError("No Auth Email Data Found Against Dome ID: " + ConstantsHolder.domeId);
        }
    }

    void TriggerSceneLoading(string WorldId)
    {
        GamePlayUIHandler.inst.SummitCXOEmailAuthUIHandle.SetAuthEmailUIState(false);
        if (ConstantsHolder.MultiSectionPhoton)
        {
            ConstantsHolder.DiasableMultiPartPhoton = true;
        }
        BuilderEventManager.LoadSceneByName?.Invoke(WorldId, transform.GetChild(1).transform.position);
    }

    async void DisableCollider()
    {
        await Task.Delay(2000);
        alreadyTriggered = false;
    }

}
