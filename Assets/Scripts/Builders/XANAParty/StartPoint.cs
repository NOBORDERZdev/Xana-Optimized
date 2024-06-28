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
        if (PhotonNetwork.IsMasterClient)
        {
            NetworkSyncManager.instance.view.RPC(nameof(StartGameRPC), RpcTarget.All);
        }
    }

    void EnableCollider()
    {
        triggerCollider.SetActive(true);
    }

    [PunRPC]
    public void StartGameRPC()
    {
        StartCoroutine(nameof(StartGame));
    }

   IEnumerator StartGame()
    {
        BuilderEventManager.OnDisplayMessageCollisionEnter?.Invoke("Ready?", 2, true);
        yield return new WaitForSeconds(2);
        BuilderEventManager.OnTimerCountDownTriggerEnter?.Invoke(3, true);
        yield return new WaitForSeconds(4);
        triggerCollider.SetActive(false);
    }
}