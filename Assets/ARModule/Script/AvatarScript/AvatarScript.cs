using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Animations.Rigging;
using UnityEngine.SceneManagement;

public class AvatarScript : MonoBehaviour
{
    public int avatarIndexId = 0;

    public GameObject avatarHips;

    [Header("Avatar Animation References")]
    public AnimationClip[] currentAvatarAnimationClip;

    protected Animator animator;
    private GameObject avatarObj = null;

    public RuntimeAnimatorController ranim;

    protected AnimatorOverrideController animatorOverrideController;

    public AnimationClipOverrides clipOverrides;

    public UnityEngine.Avatar defaultAvatar;

    [Header("For Animation Rig For Action Scene")]
    public bool r_isActionScene = false;
    public GameObject headAnimObj;
    public GameObject chestAnimObj;
    public GameObject mainRootRotationAnimObj;
    public GameObject mainRootPositionAnimObj;
    public RigBuilder rigBuilder;

    public void Start()
    {

        if (this.transform.childCount > 0)
        {
            avatarObj = this.transform.GetChild(0).gameObject;

            if (avatarObj.GetComponent<Animator>() == null)
            {
                //If Animator Component Not Found
                avatarObj.AddComponent<Animator>();
                animator = avatarObj.GetComponent<Animator>();
                animator.avatar = defaultAvatar;
                animator.applyRootMotion = false;

                animator.runtimeAnimatorController = ranim;

                animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);

                animator.runtimeAnimatorController = animatorOverrideController;

                clipOverrides = new AnimationClipOverrides(animatorOverrideController.overridesCount);
                animatorOverrideController.GetOverrides(clipOverrides);

                ChangeAnimationClip();
                if (r_isActionScene)
                {
                    Transform rootRotationObj = avatarObj.transform.Find("Hips");
                    avatarHips = rootRotationObj.gameObject;
                    if (rootRotationObj != null)
                    {
                        mainRootRotationAnimObj.GetComponent<MultiRotationConstraint>().data.constrainedObject = rootRotationObj;
                        mainRootPositionAnimObj.GetComponent<MultiPositionConstraint>().data.constrainedObject = rootRotationObj;

                        Transform hadeObj = rootRotationObj.GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).transform;
                        //hadeObj.rotation = Quaternion.Euler(Vector3.zero);
                        //hadeObj.transform.parent.rotation = Quaternion.Euler(Vector3.zero);
                        headAnimObj.GetComponent<MultiRotationConstraint>().data.constrainedObject = hadeObj;

                        chestAnimObj.GetComponent<MultiRotationConstraint>().data.constrainedObject = rootRotationObj.GetChild(2).GetChild(0).transform;
                    }

                    rigBuilder.enabled = true;
                    if(SceneManager.GetActiveScene().name == "ARModuleRealityScene")
                    {
                        animator.SetBool("Idle", true);
                        StartCoroutine(WaitToResetRigBuilder());
                    }
                    //this.GetComponent<Animator>().enabled=true;
                    ARFaceModuleManager.Instance.m_CanRotateCharacter = false;

                    //ARFacePoseTrackingManager.Instance.m_SkinnedMeshRenderer = avatarObj.transform.GetChild(2).GetChild(0).GetComponent<SkinnedMeshRenderer>();
                }
            }
            else
            {
                //If Animator Component Found
                animator = avatarObj.GetComponent<Animator>();

                animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);

                animator.runtimeAnimatorController = animatorOverrideController;

                clipOverrides = new AnimationClipOverrides(animatorOverrideController.overridesCount);
                animatorOverrideController.GetOverrides(clipOverrides);
            }
        }

        if (avatarHips != null)
        {
            startPos = avatarHips.transform.position;
        }
    }

    IEnumerator WaitToResetRigBuilder()
    {
        rigBuilder.enabled = false;
        yield return new WaitForSeconds(0.1f);
        rigBuilder.enabled = true;
        animator.SetBool("Idle", false);
    }

    public void ChangeAnimationClip()
    {
        if(avatarIndexId >= currentAvatarAnimationClip.Length)
        {
            avatarIndexId = 0;
        }
        //Debug.LogError("Index:" + avatarIndexId + "   :count:" + currentAvatarAnimationClip.Length);

        animator.runtimeAnimatorController = ranim;

        clipOverrides["IdleAnim"] = currentAvatarAnimationClip[avatarIndexId];
        animatorOverrideController.ApplyOverrides(clipOverrides);
    }

    Vector3 startPos;
    public void SetDefaultAvatarHipsPos()
    {
        if (avatarHips != null)
        {
            avatarHips.transform.position = startPos;
        }
    }
}

public class AnimationClipOverrides : List<KeyValuePair<AnimationClip, AnimationClip>>
{
    public AnimationClipOverrides(int capacity) : base(capacity) { }

    public AnimationClip this[string name]
    {
        get { return this.Find(x => x.Key.name.Equals(name)).Value; }
        set
        {
            int index = this.FindIndex(x => x.Key.name.Equals(name));
            if (index != -1)
                this[index] = new KeyValuePair<AnimationClip, AnimationClip>(this[index].Key, value);
        }
    }
}