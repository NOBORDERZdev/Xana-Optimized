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
    public Animator OpenGateAnimator;
    public bool IsRaceStarted = false;

    private void OnEnable()
    {
        if ( BuilderData.mapData != null && BuilderData.mapData.data.worldType != 1)
        {
            DisableCollider();
            return;
        }
        BuilderEventManager.XANAPartyRaceStart += DisableCollider;
        BuilderEventManager.XANAPartyWiatingForPlayer += EnableCollider;
        NetworkSyncManager.instance.startGame += StartG;
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
        NetworkSyncManager.instance.startGame -= StartG;
    }

    void DisableCollider()
    {
         print("DisableCollider Call");
        //triggerCollider.SetActive(false);
         if (PhotonNetwork.IsMasterClient)
          {
              NetworkSyncManager.instance.view.RPC("StartGameMainRPC", RpcTarget.All);
          }
        //StartCoroutine(StartGame());
    }

    private void StartG()
    {
        StartCoroutine(StartGame());
    }

    void EnableCollider()
    {
        triggerCollider.SetActive(true);
    }

    

   IEnumerator StartGame()
    {
        if(IsRaceStarted)
            yield break;
        IsRaceStarted = true;
        if(LocalizationManager.forceJapanese || GameManager.currentLanguage == "ja")
            BuilderEventManager.OnDisplayMessageCollisionEnter?.Invoke("レディー！", 1, true);
        else
            BuilderEventManager.OnDisplayMessageCollisionEnter?.Invoke("READY!", 1, true);
        //yield return new WaitForSeconds(2);
        //BuilderEventManager.OnTimerCountDownTriggerEnter?.Invoke(3, true);
        //yield return new WaitForSeconds(4);
        XANAPartyManager.Instance.GetComponent<PenpenzLpManager>().WinnerPlayerIds.Clear();
        XANAPartyManager.Instance.GetComponent<PenpenzLpManager>().RaceFinishTime.Clear();
        yield return new WaitForSeconds(1);
        ReferencesForGamePlay.instance.XANAPartyCounterPanel.SetActive(true);

        if (LocalizationManager.forceJapanese || GameManager.currentLanguage == "ja"){ 
            ReferencesForGamePlay.instance.XANAPartyCounterText.fontSize=50;
            ReferencesForGamePlay.instance.XANAPartyCounterText.text = "ゴー！".ToString();
        }
        else{ 
            ReferencesForGamePlay.instance.XANAPartyCounterText.text = "GO!".ToString();
        }

        yield return new WaitForSeconds(1);
        ReferencesForGamePlay.instance.XANAPartyCounterPanel.SetActive(false);
        triggerCollider.SetActive(false);
       GameplayEntityLoader.instance.PositionResetButton.SetActive(true);

        if (OpenGateAnimator != null)
            OpenGateAnimator.SetTrigger("OpenGate");
    }
}