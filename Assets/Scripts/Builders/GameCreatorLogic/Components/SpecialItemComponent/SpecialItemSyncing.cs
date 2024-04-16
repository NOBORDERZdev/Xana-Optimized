using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.Demo.PunBasics;

public class SpecialItemSyncing : MonoBehaviourPun
{
    Shader defaultSkinShader, defaultClothShader;
    Shader newSkinShader, newClothShader;

    SkinnedMeshRenderer playerHair;
    SkinnedMeshRenderer playerBody;
    SkinnedMeshRenderer playerShirt;
    SkinnedMeshRenderer playerPants;
    SkinnedMeshRenderer playerShoes;
    GameObject playerObj;
    bool isInitialise = false;

    void OnEnable()
    {
        if (photonView.IsMine)
            return;

        if (!GamificationComponentData.instance.withMultiplayer)
        {
            gameObject.SetActive(false);
            return;
        }
        defaultSkinShader = GamificationComponentData.instance.skinShader;
        defaultClothShader = GamificationComponentData.instance.cloathShader;
        newSkinShader = GamificationComponentData.instance.superMarioShader;
        newClothShader = GamificationComponentData.instance.superMarioShader2;
        StartCoroutine(SyncingCoroutin());
    }

    private IEnumerator SyncingCoroutin()
    {
        yield return new WaitForSeconds(0.5f);
        playerObj = FindPlayerusingPhotonView(photonView);
        if (playerObj != null)
        {
            yield return new WaitForSeconds(0.5f);
            this.transform.SetParent(playerObj.transform);
            transform.localEulerAngles = Vector3.up * 180;
            transform.localPosition = Vector3.up * 0.824f;
            AvatarController ac = playerObj.GetComponent<AvatarController>();
            CharacterBodyParts charcterBodyParts = playerObj.GetComponent<CharacterBodyParts>();
            if (ac.wornHair)
                playerHair = ac.wornHair.GetComponent<SkinnedMeshRenderer>();
            if (ac.wornPant)
                playerPants = ac.wornPant.GetComponent<SkinnedMeshRenderer>();
            if (ac.wornShirt)
                playerShirt = ac.wornShirt.GetComponent<SkinnedMeshRenderer>();
            if (ac.wornShoes)
                playerShoes = ac.wornShoes.GetComponent<SkinnedMeshRenderer>();
            playerBody = charcterBodyParts.body;

            ApplySuperMarioEffect(true);
        }
    }

    void OnDisable()
    {
        if (photonView.IsMine)
            return;
        if (isInitialise)
            ApplySuperMarioEffect(false);
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

    private void ApplySuperMarioEffect(bool state)
    {
        if (playerObj == null && !state)
            return;

        if (playerHair)
            playerHair.material.shader = state ? newClothShader : defaultClothShader;
        if (playerBody)
        {
            playerBody.material.shader = state ? newSkinShader : defaultSkinShader;
            playerBody.material.SetColor("_Lips_Color", state ? new Color32(0, 0, 0, 0) : new Color32(255, 255, 255, 0));
            //if (state)
            //    playerBody.material.SetFloat("_Outer_Glow", 2);
        }
        if (playerShirt)
            playerShirt.material.shader = state ? newClothShader : defaultClothShader;
        if (playerPants)
            playerPants.material.shader = state ? newClothShader : defaultClothShader;
        if (playerShoes)
            playerShoes.material.shader = state ? newClothShader : defaultClothShader;

        isInitialise = state;
    }
}
