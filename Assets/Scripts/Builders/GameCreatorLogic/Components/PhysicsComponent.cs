using UnityEngine;
using Models;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsComponent : ItemComponent
{
    PhysicsComponentData _physicsComponentData;
    Rigidbody _assetRigidBody;

    new void Awake()
    {
        _assetRigidBody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _assetRigidBody.isKinematic = false;
        _assetRigidBody.useGravity = true;
        _assetRigidBody.mass = _physicsComponentData.PhysicsComponentMassValue;
        _assetRigidBody.useGravity = _physicsComponentData.PhysicsComponentUseGravity;

        // Freeze position
        if (_physicsComponentData.PhysicsComponentFreezePosX)
            _assetRigidBody.constraints |= RigidbodyConstraints.FreezePositionX;
        if (_physicsComponentData.PhysicsComponentFreezePosY)
            _assetRigidBody.constraints |= RigidbodyConstraints.FreezePositionY;
        if (_physicsComponentData.PhysicsComponentFreezePosZ)
            _assetRigidBody.constraints |= RigidbodyConstraints.FreezePositionZ;

        // Freeze rotation
        if (_physicsComponentData.PhysicsComponentFreezeRotX)
            _assetRigidBody.constraints |= RigidbodyConstraints.FreezeRotationX;
        if (_physicsComponentData.PhysicsComponentFreezeRotY)
            _assetRigidBody.constraints |= RigidbodyConstraints.FreezeRotationY;
        if (_physicsComponentData.PhysicsComponentFreezeRotZ)
            _assetRigidBody.constraints |= RigidbodyConstraints.FreezeRotationZ;

    }

    public void Init(PhysicsComponentData physicsComponentData)
    {
        this._physicsComponentData = physicsComponentData;
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
        _componentType = Constants.ItemComponentType.PhysicsComponent;
    }

    public override void CollisionEnterBehaviour()
    {
    }

    #endregion
}