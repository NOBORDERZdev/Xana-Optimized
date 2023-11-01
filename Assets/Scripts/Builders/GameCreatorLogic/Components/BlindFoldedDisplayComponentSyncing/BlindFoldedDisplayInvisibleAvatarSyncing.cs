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

    Material defaultHairMat;
    Material defaultBodyMat;
    Material defaultShirtMat;
    Material defaultPantsMat;
    Material defaultShoesMat;
    Material defaultFreeCamConsoleMat;
    Material[] defaultHeadMaterials;

    GameObject playerObj;

    void OnEnable()
    {
        if (photonView.IsMine)
            return;

        hologramMaterial = GamificationComponentData.instance.hologramMaterial;
        playerObj = FindPlayerusingPhotonView(photonView);
        if (playerObj != null)
        {
            this.transform.SetParent(playerObj.transform);
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
            defaultHeadMaterials = new Material[playerHead.sharedMesh.subMeshCount];
            for (int i = 0; i < playerHead.materials.Length; i++)
            {
                defaultHeadMaterials[i] = playerHead.materials[i];
            }

            defaultBodyMat = playerBody.material;
            defaultPantsMat = playerPants.material;
            defaultShirtMat = playerShirt.material;
            defaultHairMat = playerHair.material;
            defaultShoesMat = playerShoes.material;

            defaultFreeCamConsoleMat = playerFreeCamConsole.material;
            AvatarInvisibilityApply();
        }
    }

    void OnDisable()
    {
        StopAvatarInvisibility();
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

    void AvatarInvisibilityApply()
    {
        playerHair.material = hologramMaterial;
        playerBody.material = hologramMaterial;
        playerShirt.material = hologramMaterial;
        playerPants.material = hologramMaterial;
        playerShoes.material = hologramMaterial;
        Material[] newMaterials = new Material[playerHead.sharedMesh.subMeshCount];

        // Assign the new material to all submeshes
        for (int i = 0; i < newMaterials.Length; i++)
        {
            newMaterials[i] = hologramMaterial;
        }

        // Apply the new materials to the SkinnedMeshRenderer
        playerHead.materials = newMaterials;

        playerFreeCamConsole.material = hologramMaterial;
        playerFreeCamConsoleOther.material = hologramMaterial;
    }

    void StopAvatarInvisibility()
    {
        if (photonView.IsMine)
            return;
        playerHair.material = defaultHairMat;
        playerBody.material = defaultBodyMat;
        playerShirt.material = defaultShirtMat;
        playerPants.material = defaultPantsMat;
        playerShoes.material = defaultShoesMat;
        playerHead.sharedMaterials = defaultHeadMaterials;
        playerFreeCamConsole.material = defaultFreeCamConsoleMat;
        playerFreeCamConsoleOther.material = defaultFreeCamConsoleMat;
    }
}
