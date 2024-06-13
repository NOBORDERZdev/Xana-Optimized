using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class StartPoint : MonoBehaviour
{
    public List<Transform> SpawnPoints;
    public GameObject triggerCollider;
    public bool isStartPoint;

    private void OnEnable()
    {
        if ( BuilderData.mapData != null && BuilderData.mapData.data.worldType != 1)
        {
            DisableCollider();
            return;
        }
        BuilderEventManager.XANAPartyRaceStart += DisableCollider;
        BuilderEventManager.XANAPartyWiatingForPlayer += EnableCollider;
    }

    private void OnDisable()
    {
        if (BuilderData.mapData != null && BuilderData.mapData.data.worldType != 1)
        {
            DisableCollider();
            return;
        }
        BuilderEventManager.XANAPartyRaceStart -= DisableCollider;
        BuilderEventManager.XANAPartyWiatingForPlayer -= EnableCollider;
    }

    void DisableCollider()
    {
         print("DisableCollider Call");
        //triggerCollider.SetActive(false);
        //gameObject.GetComponent<PhotonView>().RPC(nameof(StartGameRPC), RpcTarget.All);
        StartCoroutine(nameof(StartGame));
    }

    void EnableCollider()
    {
        triggerCollider.SetActive(true);
    }

   IEnumerator StartGame()
    {
        BuilderEventManager.OnDisplayMessageCollisionEnter?.Invoke("Ready?", 2, true);
        yield return new WaitForSeconds(2);
        BuilderEventManager.OnTimerCountDownTriggerEnter?.Invoke(3, true);
        yield return new WaitForSeconds(3);
        triggerCollider.SetActive(false);
    }
}