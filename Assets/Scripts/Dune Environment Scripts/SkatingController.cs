using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkatingController : MonoBehaviour
{
    private Animator animator;
    private PhysicMaterial frictionPhysics;
    private PhysicMaterial maxFrictionPhysics;
    private PhysicMaterial slippyPhysics;
    private CapsuleCollider _capsuleCollider;
    private Vector3 colliderCenter;
    private float colliderRadius;
    private float colliderHeight;

    void OnEnable()
    {
        Init();
    }

    public void Init()
    {
        animator = ReferencesForGamePlay.instance.m_34player.GetComponent<Animator>();
        animator.updateMode = AnimatorUpdateMode.AnimatePhysics;

        // prevents the collider from slipping on ramps
        maxFrictionPhysics = new PhysicMaterial();
        maxFrictionPhysics.name = "_maxFrictionPhysics";
        maxFrictionPhysics.staticFriction = 1f;
        maxFrictionPhysics.dynamicFriction = 1f;
        maxFrictionPhysics.frictionCombine = PhysicMaterialCombine.Maximum;


        // capsule collider info
        _capsuleCollider = GetComponent<CapsuleCollider>();

        // save your collider preferences 
        colliderCenter = _capsuleCollider.center;
        colliderRadius = _capsuleCollider.radius;
        colliderHeight = _capsuleCollider.height;

        _capsuleCollider.center = new Vector3(0,  0.75f, 0);
        _capsuleCollider.radius = 0.29f;
        _capsuleCollider.height = 1.5f;

        // set the friction
        _capsuleCollider.material = maxFrictionPhysics;

    }
    private void OnDisable()
    {
        _capsuleCollider.center = colliderCenter;
        _capsuleCollider.radius = colliderRadius;
        _capsuleCollider.height = colliderHeight;
        _capsuleCollider.material = null;

    }
}
