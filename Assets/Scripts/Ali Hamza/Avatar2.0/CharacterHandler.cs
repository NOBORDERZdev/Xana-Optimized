using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHandler : MonoBehaviour
{
    //public static CharacterHandler instance;

    public AvatarData maleAvatarData;
    public AvatarData femaleAvatarData;
    public AvatarData avatarData;
    private void Awake()
    {
        //instance = this;
    }

    private void OnEnable()
    {
        GameManager.Instance.avatarGender = avatarData.avatar_Gender;
        GameManager.Instance.mainCharacter = avatarData.avatar_parent;
        GameManager.Instance.m_ChHead = avatarData.avatar_face.gameObject;
        GameManager.Instance.m_CharacterAnimator = avatarData.avatar_animator;
        GameManager.Instance.avatarController = avatarData.avatar_parent.GetComponent<AvatarController>();
        GameManager.Instance.characterBodyParts = avatarData.avatar_parent.GetComponent<CharacterBodyParts>();
        //GameManager.Instance.characterBodyParts.body= avatarData.avatar_body;
        //GameManager.Instance.characterBodyParts.head = avatarData.avatar_face;

        GameManager.Instance.m_CharacterAnimator.SetBool("Action", true);
        GameManager.Instance.ActorManager.Init();
    }


    public void ActivateAvatarByGender(string gender)
    {
        switch (gender)
        {
            case "Male":
                GameManager.Instance.mainCharacter = maleAvatarData.avatar_parent;
                GameManager.Instance.m_ChHead = maleAvatarData.avatar_face.gameObject;
                GameManager.Instance.m_CharacterAnimator = maleAvatarData.avatar_animator;
                //GameManager.Instance.characterBodyParts.body = maleAvatarData.avatar_body;
                //GameManager.Instance.characterBodyParts.head = maleAvatarData.avatar_face;
                break;
            case "Female":
                GameManager.Instance.mainCharacter = femaleAvatarData.avatar_parent;
                GameManager.Instance.m_ChHead = femaleAvatarData.avatar_face.gameObject;
                GameManager.Instance.m_CharacterAnimator = femaleAvatarData.avatar_animator;
                //GameManager.Instance.characterBodyParts.body = femaleAvatarData.avatar_body;
                // GameManager.Instance.characterBodyParts.head = femaleAvatarData.avatar_face;
                break;
        }
        maleAvatarData.avatar_parent.SetActive(false);
        femaleAvatarData.avatar_parent.SetActive(false);
        GameManager.Instance.mainCharacter.SetActive(true);
        GameManager.Instance.avatarController = GameManager.Instance.mainCharacter.GetComponent<AvatarController>();
        GameManager.Instance.characterBodyParts = GameManager.Instance.mainCharacter.GetComponent<CharacterBodyParts>();


        // GameManager.Instance.characterBodyParts.characterHeadMat = GameManager.Instance.characterBodyParts.head.materials[2];
        // GameManager.Instance.characterBodyParts.characterBodyMat = GameManager.Instance.characterBodyParts.body.materials[0];

        NFTBoxerEyeData.instance.bodyParts = GameManager.Instance.mainCharacter.GetComponent<CharacterBodyParts>();
        GameManager.Instance.m_CharacterAnimator.SetBool("Action", true);

        //StopCoroutine(GameManager.Instance.mainCharacter.GetComponent<Actor>().StartBehaviour());
        //StopCoroutine(GameManager.Instance.mainCharacter.GetComponent<Actor>().StartActorBehaviour());
        GameManager.Instance.ActorManager.Init();
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
