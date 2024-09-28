using System.Collections;
using Models;
using Photon.Pun;
using UnityEngine;

//[RequireComponent(typeof(Rigidbody))]
public class AddForceComponent : ItemComponent
{
    private AddForceComponentData _addForceComponentData;
    Rigidbody _rigidBody;
    Rigidbody _rigidBodyPlayer;
    ContactPoint[] _contactPoints;
    CharacterController _characterControllerNew;

    string _runtimeItemID = "";

    //Checks if the force be applied or not
    bool _isActivated = false;

    [Range(0, 2)] float _drag = 0.3f;
    [Range(0, 1)] float _angDrag = 0.2f;
    float _forceMultiplier = .4f;
    float _playerEndVelocity = 40;

    bool collideWithComponent;

    public void Init(AddForceComponentData addForceComponentData)
    {
        _rigidBody = GetComponent<Rigidbody>();
        _rigidBody.isKinematic = true;
        _rigidBody.useGravity = true;
        _rigidBody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        this._addForceComponentData = addForceComponentData;
        _isActivated = addForceComponentData.isActive;

        _runtimeItemID = GetComponent<XanaItem>().itemData.RuntimeItemID;
    }

    public void ApplyAddForce()
    {
        if (GamificationComponentData.instance == null || _addForceComponentData == null || _rigidBodyPlayer == null)
            return;

        if (!_addForceComponentData.forceApplyOnAvatar)
        {
            if (_addForceComponentData.forceApplyOnFixedDirection)
            {
                _rigidBody.isKinematic = false;
                _rigidBody.drag = _drag;
                _rigidBody.angularDrag = _angDrag;
                _rigidBody.AddForce(_addForceComponentData.forceDirection * _addForceComponentData.forceAmountValue * _forceMultiplier, ForceMode.Impulse);
                StartCoroutine(SetIsKinematiceTrue());
            }
            else
            {
                //AddRigidBody();
                _rigidBody.isKinematic = false;
                _rigidBody.drag = _drag;
                _rigidBody.angularDrag = _angDrag;
                var addForce = GamificationComponentData.instance.MapValue(_addForceComponentData.forceAmountValue, 0, 100, 1, 10) * (Vector3.up * _addForceComponentData.fixedForceonYAxisValue + _rigidBodyPlayer.velocity);
                if (_contactPoints != null && _contactPoints.Length > 0)
                {
                    foreach (ContactPoint contact in _contactPoints)
                    {
                        _rigidBody.AddForceAtPosition(addForce, contact.point, ForceMode.Impulse);
                    }
                }
                //RemoveRigidBody();
            }
        }
        else
        {
            if (ConstantsHolder.xanaConstants.isXanaPartyWorld)
            {
                var addForce = GamificationComponentData.instance.MapValue(_addForceComponentData.forceAmountValue, 0, 100, 30, 100);
                var direction = Quaternion.AngleAxis(GamificationComponentData.instance.PlayerRigidBody.gameObject.transform.rotation.y, Vector3.up) * _addForceComponentData.forceDirection;
                if (direction.y == 0) direction.y = 0.5f;
                GamificationComponentData.instance.PlayerRigidBody.AddForce(addForce * direction, ForceMode.Impulse);
            }
            Debug.Log("Coming soon");
            //AddRigidBody();
            //_rigidBodyPlayer.isKinematic = false;
            //_characterControllerNew.enabled = false;
            //var addForce = GamificationComponentData.instance.MapValue(_addForceComponentData.forceAmountValue, 0, 100, 500, 1200);
            //var direction = Quaternion.AngleAxis(_rigidBodyPlayer.gameObject.transform.rotation.y, Vector3.up) * _addForceComponentData.forceDirection;
            //if (direction.y == 0) direction.y = 0.5f;
            //_rigidBodyPlayer.AddForce(addForce * direction, ForceMode.Impulse);
            //StartCoroutine(SetIsKinematiceTrue());
        }

    }

    IEnumerator SetIsKinematiceTrue()
    {
        //wait so the applied force takes effect
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        while ((!_addForceComponentData.forceApplyOnAvatar && _rigidBody.velocity.magnitude > 0.0001f) || (_addForceComponentData.forceApplyOnAvatar && ((_rigidBodyPlayer != null && (_rigidBodyPlayer.velocity.sqrMagnitude > _playerEndVelocity)) || !GamificationComponentData.instance.playerControllerNew._IsGrounded)))
        {
            yield return new WaitForFixedUpdate();
        }
        //_rigidBody.isKinematic = true;
        if (_rigidBodyPlayer && !ConstantsHolder.isSoftBankGame)
            _rigidBodyPlayer.isKinematic = true;
        if (_characterControllerNew)
            _characterControllerNew.enabled = true;
        //RemoveRigidBody();
    }

    //void AddRigidBody()
    //{
    //    _rigidBodyPlayer = GamificationComponentData.instance.playerControllerNew.gameObject.AddComponent<Rigidbody>();
    //    _rigidBodyPlayer.mass = 60;
    //    _rigidBodyPlayer.isKinematic = true;
    //    _rigidBodyPlayer.useGravity = true;
    //    _rigidBodyPlayer.constraints = RigidbodyConstraints.FreezeRotation;
    //}

    //void RemoveRigidBody()
    //{
    //    if (_rigidBodyPlayer)
    //        Destroy(_rigidBodyPlayer);
    //}


    private void OnCollisionEnter(Collision _other)
    {
        if (_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            _rigidBodyPlayer = _other.gameObject.GetComponent<Rigidbody>();
            if (!ConstantsHolder.xanaConstants.isXanaPartyWorld && _characterControllerNew == null)
                _characterControllerNew = ReferencesForGamePlay.instance.MainPlayerParent.GetComponent<CharacterController>();
            ReferencesForGamePlay.instance.m_34player.GetComponent<SoundEffects>().PlaySoundEffects(SoundEffects.Sounds.AddForce);
            if (GamificationComponentData.instance.withMultiplayer && !_addForceComponentData.forceApplyOnAvatar)
                GamificationComponentData.instance.photonView.RPC("GetObject", RpcTarget.All, _runtimeItemID, _componentType);
            else
                GamificationComponentData.instance.GetObjectwithoutRPC(_runtimeItemID, _componentType);
        }
    }

    #region BehaviourControl
    private void StartComponent()
    {
        //if (collideWithComponent)
        //    return;
        //collideWithComponent = true;
        //Invoke(nameof(CollideWithComponet), 0.5f);
        ApplyAddForce();
    }

    void CollideWithComponet()
    {
        collideWithComponent = false;
    }

    private void StopComponent()
    {
        //rigidBody.isKinematic = false;
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
        _componentType = Constants.ItemComponentType.AddForceComponent;
    }

    public override void CollisionExitBehaviour()
    {
        //throw new System.NotImplementedException();
    }

    public override void CollisionEnterBehaviour()
    {
        //CollisionEnter();
    }

    #endregion
}