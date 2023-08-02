using System.Collections;
using Models;
using Photon.Pun;
using UnityEngine;

//[RequireComponent(typeof(Rigidbody))]
public class AddForceComponent : ItemComponent
{
    private AddForceComponentData addForceComponentData;
    Rigidbody rigidBody;

    //Checks if the force be applied or not
    bool isActivated = false;

    int forceMultiplier = 20;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.isKinematic = true;
    }

    public void Init(AddForceComponentData addForceComponentData)
    {
        this.addForceComponentData = addForceComponentData;
        isActivated = addForceComponentData.isActive;
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
            ApplyAddForce();
        }
    }
}