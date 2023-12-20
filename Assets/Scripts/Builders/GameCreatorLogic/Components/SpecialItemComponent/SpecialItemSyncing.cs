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
            AvatarController avatarController = playerObj.GetComponent<AvatarController>();
            CharcterBodyParts charcterBodyParts = playerObj.GetComponent<CharcterBodyParts>();
            playerHair = avatarController.wornHair.GetComponent<SkinnedMeshRenderer>();
            playerPants = avatarController.wornPant.GetComponent<SkinnedMeshRenderer>();
            playerShirt = avatarController.wornShirt.GetComponent<SkinnedMeshRenderer>();
            playerShoes = avatarController.wornShose.GetComponent<SkinnedMeshRenderer>();
            playerBody = charcterBodyParts.Body;
            ApplySuperMarioEffect();
        }
    }

    void OnDisable()
    {
        if (photonView.IsMine)
            return;
        if (isInitialise)
            ApplyDefaultEffect();
    }

    GameObject FindPlayerusingPhotonView(PhotonView pv)
    {
        Player player = pv.Owner;
        foreach (GameObject playerObject in Launcher.instance.playerobjects)
        {
            PhotonView _photonView = playerObject.GetComponent<PhotonView>();
            if (_photonView.Owner == player && _photonView.GetComponent<AvatarController>())
            {
                return playerObject;
            }
        }
        return null;
    }

    private void ApplySuperMarioEffect()
    {
        playerHair.material.shader = newClothShader;
        playerBody.material.shader = newSkinShader;
        playerBody.material.SetFloat("_Outer_Glow", 2);
        playerShirt.material.shader = newClothShader;
        playerPants.material.shader = newClothShader;
        playerShoes.material.shader = newClothShader;
        isInitialise = true;
    }

    private void ApplyDefaultEffect()
    {
        if (playerObj == null)
            return;

        playerHair.material.shader = defaultClothShader;
        playerBody.material.shader = defaultSkinShader;
        playerShirt.material.shader = defaultClothShader;
        playerPants.material.shader = defaultClothShader;
        playerShoes.material.shader = defaultClothShader;
    }
}
