using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody))]
public class AvatarChangerComponent : ItemComponent
{
    public AvatarChangerComponentData componentData;

    public void InitAvatarChanger(AvatarChangerComponentData componentData)
    {
        this.componentData = componentData;
    }

    //OnCollisionEnter Convert OnTriggerEnter
    /*private void OnCollisionEnter(Collision collision)
    {
        if (*//*collision.gameObject.CompareTag("Player") || *//*(collision.gameObject.tag == "PhotonLocalPlayer" && collision.gameObject.GetComponent<PhotonView>().IsMine))
        {
            collision.gameObject.GetComponent<BuildingDetect>().OnAvatarChangerEnter(componentData.setTimer, componentData.avatarIndex);
            Destroy(this.gameObject);
        }
    }*/
    private void OnCollisionEnter(Collision _other)
    {
        //}
        //private void OnTriggerEnter(Collider _other)
        //{
        if (_other.gameObject.tag == "Player" || (_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine))
        {
            _other.gameObject.GetComponent<BuildingDetect>().OnAvatarChangerEnter(componentData.setTimer, componentData.avatarIndex);
            Destroy(this.gameObject);
        }
    }
}