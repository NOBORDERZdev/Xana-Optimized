using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StoreManager;

public class CharacterHandler : MonoBehaviour
{
    public static CharacterHandler instance;
    public AvatarGender activePlayerGender;
    public AvatarData maleAvatarData;
    public AvatarData femaleAvatarData;

    public GameObject playerNameCanvas;
    public GameObject playerPostCanvas;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        if(SaveCharacterProperties.instance && !string.IsNullOrEmpty(SaveCharacterProperties.instance.SaveItemList.gender))
            ActivateAvatarByGender(SaveCharacterProperties.instance.SaveItemList.gender);
    }

    public void ActivateAvatarByGender(string gender)
    {
        switch (gender)
        {
            case "Male":
                maleAvatarData.avatar_parent.gameObject.SetActive(true);
                femaleAvatarData.avatar_parent.gameObject.SetActive(false);
                UpdateAvatarRefrences(maleAvatarData);
                break;
            case "Female":
                maleAvatarData.avatar_parent.gameObject.SetActive(false);
                femaleAvatarData.avatar_parent.gameObject.SetActive(true);
                UpdateAvatarRefrences(femaleAvatarData);
                break;
        }
    }

    private void UpdateAvatarRefrences(AvatarData _avatarData)
    {
        activePlayerGender = _avatarData.avatar_Gender;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.mainCharacter = _avatarData.avatar_parent;
            GameManager.Instance.m_ChHead = _avatarData.avatar_face.gameObject;
            GameManager.Instance.m_CharacterAnimator = _avatarData.avatar_animator;
            GameManager.Instance.avatarController = _avatarData.avatar_parent.GetComponent<AvatarController>();
            GameManager.Instance.characterBodyParts = _avatarData.avatar_parent.GetComponent<CharacterBodyParts>();
            GameManager.Instance.m_CharacterAnimator.SetBool("Action", true);
            GameManager.Instance.ActorManager.Init();

        }
        if (SaveCharacterProperties.instance != null)
        {
            SaveCharacterProperties.instance.charcterBodyParts = GameManager.Instance.characterBodyParts;
            SaveCharacterProperties.instance.characterController = GameManager.Instance.avatarController;
        }
        
        if (playerNameCanvas && playerPostCanvas)
        {
            UpdateNameAndPostTarget(_avatarData.avatar_parent);   // Update the target of the name and post canvas to the active player
        }
    }


    private void UpdateNameAndPostTarget(GameObject activePlayer)
    {
        playerNameCanvas.GetComponent<FollowUser>().targ = activePlayer.transform;
        playerNameCanvas.GetComponent<FollowUser>().newplayerTransform = activePlayer.transform;

        playerPostCanvas.GetComponent<LookAtCamera>()._playerTransform = activePlayer.transform;
        playerPostCanvas.GetComponent<LookAtCamera>().newplayerTransform = activePlayer.transform;

        if (!playerNameCanvas.activeInHierarchy)
            playerNameCanvas.SetActive(true);
        if (!playerPostCanvas.activeInHierarchy)
            playerPostCanvas.SetActive(true);
    }

    public AvatarData GetActiveAvatarData()
    {
        if (activePlayerGender == AvatarGender.Male)
        {
            return maleAvatarData;
        }
        else
        {
            return femaleAvatarData;
        }
    }


    [Serializable]
    public class AvatarData
    {
        public AvatarGender avatar_Gender;
        public GameObject avatar_parent;
        public SkinnedMeshRenderer avatar_body;
        public SkinnedMeshRenderer avatar_face;
        public Animator avatar_animator;
        public Texture DShirt_Texture, DPent_Texture, DShoe_Texture, DEye_texture, DFace_Texture, DSkin_Texture;
    }
}
