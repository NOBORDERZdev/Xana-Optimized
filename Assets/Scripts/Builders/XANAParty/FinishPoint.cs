using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishPoint : MonoBehaviour
{
    public List<Transform> SpawnPoints;
    public string StartPointItemID;
    public GameObject triggerCollider;
    public Collider FinishRaceCollider;
    StartPoint _startPoint;

    private void OnEnable()
    {
        DisableCollider();
        if (BuilderData.mapData.data.worldType != 1)
        {
            FinishRaceCollider.enabled = false;
            return;
        }
        BuilderEventManager.AfterWorldInstantiated += FindStartPoint;
        BuilderEventManager.XANAPartyRaceFinish += EnableCollider;
    }

    private void OnDisable()
    {
        DisableCollider();
        if (BuilderData.mapData != null && BuilderData.mapData.data.worldType != 1)
        {
            FinishRaceCollider.enabled = false;
            return;
        }
        BuilderEventManager.AfterWorldInstantiated -= FindStartPoint;
        BuilderEventManager.XANAPartyRaceFinish -= EnableCollider;
    }

    void DisableCollider()
    {
        triggerCollider.SetActive(false);
    }

    void EnableCollider()
    {
        if (BuilderData.StartPointID == StartPointItemID)
        {
            FinishRaceCollider.enabled = false;
            BuilderEventManager.OnDisplayMessageCollisionEnter?.Invoke("You won the race", 3, true);
            StartCoroutine(triggerBackToLobby());
            triggerCollider.SetActive(true);
        }
    }

    IEnumerator triggerBackToLobby()
    {
        GameObject tempPenguin = GameplayEntityLoader.instance.PenguinPlayer;
        if (tempPenguin.GetComponent<PhotonView>().IsMine)
        {
            yield return new WaitForSeconds(3.5f);
            GameplayEntityLoader.instance.PenguinPlayer.GetComponent<XANAPartyMulitplayer>().BackToLobby();
        }
        else
        {
            yield return null;
        }
      
    }

    void FindStartPoint()
    {
        _startPoint = BuilderData.StartFinishPoints
        .Find(item => item.ItemID == StartPointItemID)
        ?.SpawnObject.GetComponent<StartPoint>();
    }

    internal void FinishRace()
    {
        if (!triggerCollider.activeInHierarchy)
        {
            BuilderEventManager.XANAPartyRaceFinish?.Invoke();
        }
    }
}