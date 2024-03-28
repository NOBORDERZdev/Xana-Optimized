using System.Collections;
using Models;
using Photon.Pun;
using UnityEngine;

//[RequireComponent(typeof(Rigidbody))]
public class AddForceComponent : ItemComponent
{
    private AddForceComponentData addForceComponentData;
    Rigidbody rigidBody;

    string RuntimeItemID = "";

    //Checks if the force be applied or not
    bool isActivated = false;

    int forceMultiplier = 20;

    public void Init(AddForceComponentData addForceComponentData)
    {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.isKinematic = true;
        rigidBody.useGravity = true;
        this.addForceComponentData = addForceComponentData;
        isActivated = addForceComponentData.isActive;

        RuntimeItemID = GetComponent<XanaItem>().itemData.RuntimeItemID;
    }

    public void ApplyAddForce()
    {
        rigidBody.isKinematic = false;
        rigidBody.AddForce(addForceComponentData.forceDirection * addForceComponentData.forceAmountValue * forceMultiplier * Time.deltaTime, ForceMode.VelocityChange);
        StartCoroutine(SetIsKinematiceTrue());
    }

    IEnumerator SetIsKinematiceTrue()
    {
        yield return new WaitForSeconds(1);
        while (rigidBody.velocity.magnitude > 0.0001f)
        {
            yield return null;
        }
        rigidBody.isKinematic = true;
    }


    private void OnCollisionEnter(Collision _other)
    {
        if (_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            if (GamificationComponentData.instance.withMultiplayer)
                GamificationComponentData.instance.photonView.RPC("GetObject", RpcTarget.All, RuntimeItemID, _componentType);
            else GamificationComponentData.instance.GetObjectwithoutRPC(RuntimeItemID, _componentType);
        }
    }

    #region BehaviourControl
    private void StartComponent()
    {
        ApplyAddForce();
        ReferrencesForDynamicMuseum.instance.m_34player.GetComponent<SoundEffects>().PlaySoundEffects(SoundEffects.Sounds.AddForce);

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

    #endregion
}