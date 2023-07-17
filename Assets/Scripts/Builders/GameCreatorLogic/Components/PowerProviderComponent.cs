using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody))]
public class PowerProviderComponent : ItemComponent
{
    public PowerProviderComponentData componentData;

    public void InitPowerProvider(PowerProviderComponentData componentData)
    {
        this.componentData = componentData;
    }
    private void Start()
    {
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    //onCollsion Enter to ontrigger enter
    /*private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Power Provider Trigger: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Player") || (collision.gameObject.tag == "PhotonLocalPlayer" && collision.gameObject.GetComponent<PhotonView>().IsMine))
        {
            collision.gameObject.GetComponent<BuildingDetect>().OnPowerProviderEnter(componentData.setTimer, componentData.playerSpeed, componentData.playerHeight);
            Destroy(this.gameObject);
        }
    }*/
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Power Provider Trigger: " + other.gameObject.name);
        if (/*other.gameObject.CompareTag("Player") || */(other.gameObject.tag == "PhotonLocalPlayer" && other.gameObject.GetComponent<PhotonView>().IsMine))
        {
            GamificationComponentData.instance.buildingDetect.OnPowerProviderEnter(componentData.setTimer, componentData.playerSpeed, componentData.playerHeight);
            Destroy(this.gameObject);
        }
    }
}
