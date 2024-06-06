using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using Models;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsComponent : ItemComponent
{
    PhysicsComponentData physicsComponentData;
    Rigidbody rigidBody;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        rigidBody.isKinematic = false;

        rigidBody.mass = physicsComponentData.physicsMassValue;
        rigidBody.useGravity = physicsComponentData.physicsUseGravity;

        // Freeze position
        if (physicsComponentData.physicsFreezePosX)
            rigidBody.constraints |= RigidbodyConstraints.FreezePositionX;
        if (physicsComponentData.physicsFreezePosY)
            rigidBody.constraints |= RigidbodyConstraints.FreezePositionY;
        if (physicsComponentData.physicsFreezePosZ)
            rigidBody.constraints |= RigidbodyConstraints.FreezePositionZ;

        // Freeze rotation
        if (physicsComponentData.physicsFreezeRotX)
            rigidBody.constraints |= RigidbodyConstraints.FreezeRotationX;
        if (physicsComponentData.physicsFreezeRotY)
            rigidBody.constraints |= RigidbodyConstraints.FreezeRotationY;
        if (physicsComponentData.physicsFreezeRotZ)
            rigidBody.constraints |= RigidbodyConstraints.FreezeRotationZ;

    }

    public void Init(PhysicsComponentData physicsComponentData)
    {
        this.physicsComponentData = physicsComponentData;
    }



    #region BehaviourControl
    private void StartComponent()
    {

    }
    private void StopComponent()
    {


    }

    public override void StopBehaviour()
    {
        isPlaying = false;
        StopComponent();
    }

    public override void PlayBehaviour()
    {
        isPlaying = true;
        StartComponent();
    }
    public override void CollisionExitBehaviour()
    {

    }
    public override void ToggleBehaviour()
    {
        isPlaying = !isPlaying;

        if (isPlaying)
            PlayBehaviour();
        else
            StopBehaviour();
    }
    public override void ResumeBehaviour()
    {
        PlayBehaviour();
    }

    public override void AssignItemComponentType()
    {
        _componentType = Constants.ItemComponentType.Physics;
    }

    public override void CollisionEnterBehaviour()
    {
    }

    #endregion
}
