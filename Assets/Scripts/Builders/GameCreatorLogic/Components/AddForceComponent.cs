using System.Collections;
using System.Collections.Generic;
using Models;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

//Rigidbody is required as force is added to the Rigidbody component
[RequireComponent(typeof(Rigidbody))]
public class AddForceComponent : ItemComponent
{

    //Reference to the component data class
    [SerializeField]
    private AddForceComponentData addForceComponentData;

    //Rigidbody of the component this script is attached to
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
        //ApplyAddForce();
    }

    public void ApplyAddForce()
    {
        //if (isActivated)
        //{
        rigidBody.isKinematic = false;
        //rigidBody.AddRelativeForce(addForceComponentData.forceDirection * addForceComponentData.forceAmountValue * forceMultiplier * Time.deltaTime, ForceMode.VelocityChange);

        rigidBody.AddForce(addForceComponentData.forceDirection * addForceComponentData.forceAmountValue * forceMultiplier * Time.deltaTime, ForceMode.VelocityChange);
        //isActivated = false;
        StartCoroutine(SetIsKinematiceTrue());
        //}
    }

    IEnumerator SetIsKinematiceTrue()
    {
        //wait so the applied force takes effect
        yield return new WaitForSeconds(1);

        while (rigidBody.velocity.magnitude > 0.0001f)
        {
            yield return null;
        }

        rigidBody.isKinematic = true;
    }


    private void OnCollisionEnter(Collision _other)
    {

        //}
        //private void OnTriggerEnter(Collider _other)
        //{

        if (_other.gameObject.CompareTag("Player") || (_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine))
        {
            ApplyAddForce();
        }
    }
}