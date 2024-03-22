using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class BlindFoldedDisplayInvisibleAvatarSyncing : MonoBehaviourPun
{
    Material hologramMaterial;
    SkinnedMeshRenderer playerHair;
    SkinnedMeshRenderer playerBody;
    SkinnedMeshRenderer playerShirt;
    SkinnedMeshRenderer playerPants;
    SkinnedMeshRenderer playerShoes;
    SkinnedMeshRenderer playerHead;
    MeshRenderer playerFreeCamConsole;
    MeshRenderer playerFreeCamConsoleOther;

    Material[] defaultHairMat;
    Material defaultBodyMat;
    Material defaultShirtMat;
    Material defaultPantsMat;
    Material defaultShoesMat;
    Material defaultFreeCamConsoleMat;
    Material[] defaultHeadMaterials;

    GameObject playerObj;
    bool isInitialise;

    void OnEnable()
    {
        if (photonView.IsMine)
            return;

        hologramMaterial = GamificationComponentData.instance.hologramMaterial;
        if (GamificationComponentData.instance.withMultiplayer)
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
            AvatarSetupController ac = playerObj.GetComponent<AvatarSetupController>();
            AvatarBodyParts charcterBodyParts = playerObj.GetComponent<AvatarBodyParts>();
            IKMuseum iKMuseum = playerObj.GetComponent<IKMuseum>();

            if (ac.wornHair)
                playerHair = ac.wornHair.GetComponent<SkinnedMeshRenderer>();
            if (ac.wornPant)
                playerPants = ac.wornPant.GetComponent<SkinnedMeshRenderer>();
            if (ac.wornShirt)
                playerShirt = ac.wornShirt.GetComponent<SkinnedMeshRenderer>();
            if (ac.wornShose)
                playerShoes = ac.wornShose.GetComponent<SkinnedMeshRenderer>();
            playerBody = charcterBodyParts.body;
            playerHead = charcterBodyParts.head;
            playerFreeCamConsole = iKMuseum.ConsoleObj.GetComponent<MeshRenderer>();
            playerFreeCamConsoleOther = iKMuseum.m_ConsoleObjOther.GetComponent<MeshRenderer>();
            defaultHeadMaterials = new Material[playerHead.sharedMesh.subMeshCount];
            for (int i = 0; i < playerHead.materials.Length; i++)
            {
                defaultHeadMaterials[i] = playerHead.materials[i];
            }
            if (playerBody)
                defaultBodyMat = playerBody.material;
            if (playerPants)
                defaultPantsMat = playerPants.material;
            if (playerShirt)
                defaultShirtMat = playerShirt.material;
            if (playerHair)
                defaultHairMat = playerHair.sharedMaterials;
            if (playerShoes)
                defaultShoesMat = playerShoes.material;

            defaultFreeCamConsoleMat = playerFreeCamConsole.material;
            AvatarInvisibilityApply();
        }
    }

    void OnDisable()
    {
        if (GamificationComponentData.instance.withMultiplayer)
        {
            if (isInitialise)
                StopAvatarInvisibility();
        }
    }

    GameObject FindPlayerusingPhotonView(PhotonView pv)
    {
        Player player = pv.Owner;
        PhotonView[] photonViews = GameObject.FindObjectsOfType<PhotonView>();
        foreach (PhotonView photonView in photonViews)
        {
            if (photonView.Owner == player && photonView.GetComponent<AvatarSetupController>())
            {
                return photonView.gameObject;
            }
        }
        return null;
    }

    void AvatarInvisibilityApply()
    {
        AvatarInvisibility(false);
        isInitialise = true;
    }

    private void AvatarInvisibility(bool state)
    {
        if (playerHair)
        {
            Material[] hairMats = new Material[playerHair.sharedMaterials.Length];
            for (int i = 0; i < hairMats.Length; i++)
            {
                hairMats[i] = hologramMaterial;
            }
            playerHair.sharedMaterials = state ? defaultHairMat : hairMats;
        }
        if (playerBody)
            playerBody.material = state ? defaultBodyMat : hologramMaterial;
        if (playerShirt)
            playerShirt.material = state ? defaultShirtMat : hologramMaterial;
        if (playerPants)
            playerPants.material = state ? defaultPantsMat : hologramMaterial;
        if (playerShoes)
            playerShoes.material = state ? defaultShoesMat : hologramMaterial;

        Material[] newMaterials = new Material[playerHead.sharedMesh.subMeshCount];

        // Assign the new material to all submeshes
        for (int i = 0; i < newMaterials.Length; i++)
        {
            newMaterials[i] = hologramMaterial;
        }

        // Apply the new materials to the SkinnedMeshRenderer
        playerHead.sharedMaterials = state ? defaultHeadMaterials : newMaterials;

        playerFreeCamConsole.material = state ? defaultFreeCamConsoleMat : hologramMaterial;
        playerFreeCamConsoleOther.material = state ? defaultFreeCamConsoleMat : hologramMaterial;
    }

    void StopAvatarInvisibility()
    {
        if (photonView.IsMine)
            return;
        AvatarInvisibility(true);
    }
}
