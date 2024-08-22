using ExitGames.Client.Photon;
using Photon.Pun;
using PhysicsCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

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
        if (GameplayEntityLoader.instance)
        {
            GameplayEntityLoader.instance.PenguinPlayer.GetComponent<PhotonView>().RPC("RPC_AddWinnerId", RpcTarget.AllBuffered, int.Parse(ConstantsHolder.userId));
            GameplayEntityLoader.instance.PenguinPlayer.GetComponent<PhotonView>().RPC("UpdateStatusOnRaceFinish", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.ActorNumber, true);
        }
        GameplayEntityLoader.instance.PositionResetButton.SetActive(false);
        GameplayEntityLoader.instance.PenguinPlayer.GetComponentInChildren<AnimatedController>().enabled = false;
        Animator penguinAnimator = GameplayEntityLoader.instance.PenguinPlayer.GetComponentInChildren<Animator>();
        penguinAnimator.SetBool("isGrounded", true);
        penguinAnimator.SetBool("isJump", false);
        //penguinAnimator.SetBool("Win", true);
        StartCoroutine(DelayedWinAnimation(penguinAnimator, 0.01f));
        FinishRaceCollider.enabled = false;
        if (LocalizationManager.forceJapanese || GameManager.currentLanguage == "ja")
            BuilderEventManager.OnDisplayMessageCollisionEnter?.Invoke("レースに勝利しました", 3, true);
        else
            BuilderEventManager.OnDisplayMessageCollisionEnter?.Invoke("YOU WON THE RACE", 3, true);
        triggerCollider.SetActive(true);
        GamificationComponentData gamificationTemp = GamificationComponentData.instance;
        gamificationTemp.TriggerRaceStatusUpdate();
        Hashtable _hash = new Hashtable();
        _hash.Add("IsReady", false);
        PhotonNetwork.LocalPlayer.SetCustomProperties(_hash);
    }

    IEnumerator DelayedWinAnimation(Animator animator, float delay)
    {
        while (!animator.GetBool("isGrounded"))
        {
            yield return new WaitForSeconds(delay); // Wait for the next frame
        }
        animator.SetBool("Win", true);
    }

    internal void FinishRace()
    {
        if (!triggerCollider.activeInHierarchy)
        {
            BuilderEventManager.XANAPartyRaceFinish?.Invoke();
        }
    }
}