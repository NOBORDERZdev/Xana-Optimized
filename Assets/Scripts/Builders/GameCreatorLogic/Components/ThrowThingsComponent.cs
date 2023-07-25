using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;
using Photon.Pun;

public class ThrowThingsComponent : MonoBehaviour
{
    ThrowThingsComponentData throwThingsComponentData;

    public void Init(ThrowThingsComponentData throwThingsComponentData)
    {
        this.throwThingsComponentData = throwThingsComponentData;
    }

    private void OnCollisionEnter(Collision _other)
    {
        if (_other.gameObject.CompareTag("Player") || (_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine))
        {
            BuilderEventManager.OnThrowThingsComponentCollisionEnter?.Invoke();
            GamificationComponentData.instance.playerControllerNew.Ninja_Throw(false, 1);
            Destroy(gameObject);
        }
    }
}