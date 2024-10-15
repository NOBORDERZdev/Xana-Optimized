using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
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
    SkinnedMeshRenderer[] playerEyebrow;
    MeshRenderer playerFreeCamConsole;
    MeshRenderer playerFreeCamConsoleOther;

    GameObject playerObj;
    bool isInitialise = false;

    void OnEnable()
    {
        if (photonView.IsMine)
            return;
        if (GamificationComponentData.instance.withMultiplayer)
            StartCoroutine(SyncingCoroutin());
    }

    private IEnumerator SyncingCoroutin()
    {
        //Wait for MultiplayerComponent downloadfrom addressables
        do
        {
        yield return new WaitForSeconds(0.5f);
        }
        while (!GamificationComponentData.instance.IsMultiplayerComponentLoaded);

        playerObj = FindPlayerusingPhotonView(photonView);
        if (playerObj != null)
        {
            yield return new WaitForSeconds(0.5f);
            AvatarController ac = playerObj.GetComponent<AvatarController>();
            CharacterBodyParts charcterBodyParts = playerObj.GetComponent<CharacterBodyParts>();
            IKMuseum iKMuseum = playerObj.GetComponent<IKMuseum>();
            if (ac.wornHair)
                playerHair = ac.wornHair.GetComponent<SkinnedMeshRenderer>();
            if (ac.wornPant)
                playerPants = ac.wornPant.GetComponent<SkinnedMeshRenderer>();
            if (ac.wornShirt)
                playerShirt = ac.wornShirt.GetComponent<SkinnedMeshRenderer>();
            if (ac.wornShoes)
                playerShoes = ac.wornShoes.GetComponent<SkinnedMeshRenderer>();
            if (ac.wornEyebrow.Length > 0)
            {
                int index = 0;
                playerEyebrow = new SkinnedMeshRenderer[ac.wornEyebrow.Length];
                foreach (var eyeBrow in ac.wornEyebrow)
                {
                    playerEyebrow[index] = eyeBrow.GetComponent<SkinnedMeshRenderer>();
                    index++;
                }
            }
            playerBody = charcterBodyParts.body;
            playerHead = charcterBodyParts.head;
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
            isInitialise = true;
        }
    }

    void OnDisable()
    {
        if (photonView.IsMine)
            return;
        if (GamificationComponentData.instance.withMultiplayer)
        {
            if (isInitialise)
                AvatarFootPrintVisible(true);
        }
    }

    GameObject FindPlayerusingPhotonView(PhotonView pv)
    {
        Player player = pv.Owner;
        foreach (GameObject playerObject in MutiplayerController.instance.playerobjects)
        {
            PhotonView _photonView = playerObject.GetComponent<PhotonView>();
            if (_photonView.Owner == player && _photonView.GetComponent<AvatarController>())
            {
                return playerObject;
            }
        }
        return null;
    }

    void AvatarFootPrintVisible(bool state)
    {
        if (playerHair)
            playerHair.enabled = state;
        if (playerBody)
            playerBody.enabled = state;
        if (playerShirt)
            playerShirt.enabled = state;
        if (playerPants)
            playerPants.enabled = state;
        if (playerShoes)
            playerShoes.enabled = state;
        if (playerEyebrow.Length > 0)
        {
            foreach (var eyeBrow in playerEyebrow)
            {
                eyeBrow.enabled = state;
            }
        }
        playerHead.enabled = state;
        playerFreeCamConsole.enabled = state;
        playerFreeCamConsoleOther.enabled = state;
    }
}
