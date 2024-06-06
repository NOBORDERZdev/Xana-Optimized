using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class StartPoint : MonoBehaviour
{
    public List<Transform> SpawnPoints;
    public string FinishPointItemID;
    public GameObject triggerCollider;
    FinishPoint _finishPoint;

    private void OnEnable()
    {
        if (BuilderData.mapData.data.worldType != 1)
        {
            DisableCollider();
            return;
        }
        BuilderEventManager.AfterWorldInstantiated += FindFinishPoint;
        BuilderEventManager.XANAPartyRaceStart += DisableCollider;
        BuilderEventManager.XANAPartyWiatingForPlayer += EnableCollider;
    }

    private void OnDisable()
    {
        if (BuilderData.mapData.data.worldType != 1)
        {
            DisableCollider();
            return;
        }
        BuilderEventManager.AfterWorldInstantiated -= FindFinishPoint;
        BuilderEventManager.XANAPartyRaceStart -= DisableCollider;
        BuilderEventManager.XANAPartyWiatingForPlayer -= EnableCollider;
    }

    void DisableCollider()
    {
         print("DisableCollider Call");
        //triggerCollider.SetActive(false);
        gameObject.GetComponent<PhotonView>().RPC(nameof(StartGameRPC), RpcTarget.All);
    }


    void EnableCollider()
    {
        triggerCollider.SetActive(true);
    }

    void FindFinishPoint()
    {
        _finishPoint = BuilderData.StartFinishPoints
        .Find(item => item.ItemID == FinishPointItemID)
        ?.SpawnObject.GetComponent<FinishPoint>();
    }

    [PunRPC]
    void StartGameRPC()
    {
        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        print("Start GAME Call");
        BuilderEventManager.OnTimerCountDownTriggerEnter?.Invoke(3, true);
        yield return new WaitForSeconds(3);
        BuilderEventManager.OnDisplayMessageCollisionEnter?.Invoke("Race Start...", 2, true);
        yield return new WaitForSeconds(2);
        triggerCollider.SetActive(false);
    }
}