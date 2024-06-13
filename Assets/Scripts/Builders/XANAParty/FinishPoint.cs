using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FinishPoint : MonoBehaviour
{
    public List<Transform> SpawnPoints;
    public GameObject triggerCollider;
    public Collider FinishRaceCollider;

    private void OnEnable()
    {
        DisableCollider();
        if (BuilderData.mapData.data.worldType != 1)
        {
            FinishRaceCollider.enabled = false;
            return;
        }
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
        BuilderEventManager.XANAPartyRaceFinish -= EnableCollider;
    }

    void DisableCollider()
    {
        triggerCollider.SetActive(false);
    }

    void EnableCollider()
    {
        FinishRaceCollider.enabled = false;
        BuilderEventManager.OnDisplayMessageCollisionEnter?.Invoke("You won the race", 3, true);
        triggerCollider.SetActive(true);
        GamificationComponentData gamificationTemp = GamificationComponentData.instance;
        gamificationTemp.TriggerRaceStatusUpdate();
    }

    internal void FinishRace()
    {
        if (!triggerCollider.activeInHierarchy)
        {
            BuilderEventManager.XANAPartyRaceFinish?.Invoke();
        }
    }
}