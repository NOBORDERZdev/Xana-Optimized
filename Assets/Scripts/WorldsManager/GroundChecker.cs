using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{

    public float jumpVelocity;

    public Animator playerAnimator;
    public bool isGrounded = false;
    public bool IsGrounded()
    {
        float detectionRadius = 0.5f;
        int layerId = 0;
        int layerMask = 1 << layerId;
        Collider[] result = new Collider[1];
        return Physics.OverlapSphereNonAlloc(transform.position, detectionRadius, result, layerMask) > 0;

    }

    private void Update()
    {
        isGrounded = IsGrounded();
        playerAnimator.SetBool("IsGrounded", isGrounded);
    }
}
