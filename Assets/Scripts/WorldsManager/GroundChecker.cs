using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{

    public float jumpVelocity;

    public Animator playerAnimator;
    public bool isGrounded = false;
    bool isBuilderScene;
    string[] animatorParams = { "IsGrounded", "BlendNY", "BlendNX", "throw", "throwfalse", "throwing", "isNinjaMotion", "idleNinjaAnimation3", "idleNinjaAnimation2", "NinjaJump", "NinjaThrow", "canDoubleJump" };

    PhotonAnimatorView animatorView;
    private void Start()
    {
        isBuilderScene = ConstantsHolder.xanaConstants.isBuilderScene;
        if (isBuilderScene)
            SyncAnimatorParams();
    }

    public bool IsGrounded()
    {
        float detectionRadius = 0.5f;
        int layerId = 0;
        int layerMask = 1 << layerId;
        Collider[] result = new Collider[1];
        return Physics.OverlapSphereNonAlloc(transform.position, detectionRadius, result, layerMask) > 0;

    }

    //private void Update()
    //{
    //    if (isBuilderScene)
    //        return;
    //    isGrounded = IsGrounded();
    //    playerAnimator.SetBool("IsGrounded", isGrounded);
    //}

    void SyncAnimatorParams()
    {
        animatorView = GetComponent<PhotonAnimatorView>();
        animatorView.SetParameterSynchronized(animatorParams[0], PhotonAnimatorView.ParameterType.Bool, PhotonAnimatorView.SynchronizeType.Continuous);

        if (GamificationComponentData.instance.withMultiplayer)
        {
            for (int i = 1; i < animatorParams.Length; i++)
            {
                var parameterType = (i == 1 || i == 2) ? PhotonAnimatorView.ParameterType.Float : PhotonAnimatorView.ParameterType.Bool;
                animatorView.SetParameterSynchronized(animatorParams[i], parameterType, PhotonAnimatorView.SynchronizeType.Continuous);
            }
        }
    }
}
