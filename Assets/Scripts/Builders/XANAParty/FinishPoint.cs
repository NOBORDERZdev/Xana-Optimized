using ExitGames.Client.Photon;
using Photon.Pun;
using PhysicsCharacterController;
using System.Collections.Generic;
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
        GameplayEntityLoader.instance.PositionResetButton.SetActive(false);
        GameplayEntityLoader.instance.PenguinPlayer.GetComponentInChildren<AnimatedController>().enabled = false;
        Animator penguinAnimator = GameplayEntityLoader.instance.PenguinPlayer.GetComponentInChildren<Animator>();
        penguinAnimator.SetBool("isGrounded", true);
        penguinAnimator.SetBool("isJump", false);
        penguinAnimator.SetBool("Win", true);
        FinishRaceCollider.enabled = false;
        BuilderEventManager.OnDisplayMessageCollisionEnter?.Invoke("You won the race", 3, true);
        triggerCollider.SetActive(true);
        GamificationComponentData gamificationTemp = GamificationComponentData.instance;
        gamificationTemp.TriggerRaceStatusUpdate();
        Hashtable _hash = new Hashtable();
        _hash.Add("IsReady", false);
        PhotonNetwork.LocalPlayer.SetCustomProperties(_hash);
    }

    internal void FinishRace()
    {
        if (!triggerCollider.activeInHierarchy)
        {
            BuilderEventManager.XANAPartyRaceFinish?.Invoke();
        }
    }
}