using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class BlindFoldedDisplayFootPrintAvatarSyncing : MonoBehaviourPun
{
    SkinnedMeshRenderer playerHair;
    SkinnedMeshRenderer playerBody;
    SkinnedMeshRenderer playerShirt;
    SkinnedMeshRenderer playerPants;
    SkinnedMeshRenderer playerShoes;
    SkinnedMeshRenderer playerHead;
    MeshRenderer playerFreeCamConsole;
    MeshRenderer playerFreeCamConsoleOther;

    GameObject playerObj;

    void OnEnable()
    {
        if (photonView.IsMine)
            return;
        StartCoroutine(SyncingCoroutin());
    }

    private IEnumerator SyncingCoroutin()
    {
        yield return new WaitForSeconds(0.5f);
        playerObj = FindPlayerusingPhotonView(photonView);
        if (playerObj != null)
        {
            yield return new WaitForSeconds(0.5f);
            AvatarController avatarController = playerObj.GetComponent<AvatarController>();
            CharcterBodyParts charcterBodyParts = playerObj.GetComponent<CharcterBodyParts>();
            IKMuseum iKMuseum = playerObj.GetComponent<IKMuseum>();
            playerHair = avatarController.wornHair.GetComponent<SkinnedMeshRenderer>();
            playerPants = avatarController.wornPant.GetComponent<SkinnedMeshRenderer>();
            playerShirt = avatarController.wornShirt.GetComponent<SkinnedMeshRenderer>();
            playerShoes = avatarController.wornShose.GetComponent<SkinnedMeshRenderer>();
            playerBody = charcterBodyParts.Body;
            playerHead = charcterBodyParts.Head.GetComponent<SkinnedMeshRenderer>();
            playerFreeCamConsole = iKMuseum.ConsoleObj.GetComponent<MeshRenderer>();
            playerFreeCamConsoleOther = iKMuseum.m_ConsoleObjOther.GetComponent<MeshRenderer>();

            this.transform.SetParent(playerShoes.transform);
            this.transform.localPosition = Vector3.up * 0.0207f;
            transform.localEulerAngles = Vector3.zero;
            RingbufferFootSteps ringbufferFootStep = gameObject.GetComponentInChildren<RingbufferFootSteps>();
            //for (int i = 0; i < ringbufferFootSteps.Length; i++)
            //{
            ringbufferFootStep.enabled = true;
            ringbufferFootStep.transform.GetChild(0).gameObject.SetActive(true);
            AvatarFootPrintVisible(false);
        }
    }

    void OnDisable()
    {
        if (photonView.IsMine)
            return;
        AvatarFootPrintVisible(true);
    }

    GameObject FindPlayerusingPhotonView(PhotonView pv)
    {
        Player player = pv.Owner;
        PhotonView[] photonViews = GameObject.FindObjectsOfType<PhotonView>();
        foreach (PhotonView photonView in photonViews)
        {
            if (photonView.Owner == player && photonView.GetComponent<AvatarController>())
            {
                return photonView.gameObject;
            }
        }
        return null;
    }

    void AvatarFootPrintVisible(bool state)
    {
        playerHair.enabled = state;
        playerBody.enabled = state;
        playerShirt.enabled = state;
        playerPants.enabled = state;
        playerShoes.enabled = state;
        playerHead.enabled = state;
        playerFreeCamConsole.enabled = state;
        playerFreeCamConsoleOther.enabled = state;
    }
}
