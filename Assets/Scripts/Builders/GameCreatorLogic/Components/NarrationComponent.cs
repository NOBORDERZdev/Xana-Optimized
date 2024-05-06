using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;
using Photon.Pun;

public class NarrationComponent : ItemComponent
{
    [SerializeField]
    private NarrationComponentData narrationComponentData;
    public static IEnumerator currentCoroutine;
    string RuntimeItemID = "";

    private bool IsAgainTouchable = true;
    public bool isCoroutineRunning = false;
    int i = 0;

    public void Init(NarrationComponentData narrationComponentData)
    {
        this.narrationComponentData = narrationComponentData;
        RuntimeItemID = GetComponent<XanaItem>().itemData.RuntimeItemID;
    }

    private void OnCollisionEnter(Collision _other)
    {
        if (_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            if (!IsAgainTouchable) return;

            IsAgainTouchable = false;

            BuilderEventManager.onComponentActivated?.Invoke(_componentType);
            PlayBehaviour();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        IsAgainTouchable = false;
    }
    private void OnCollisionExit(Collision collision)
    {
        IsAgainTouchable = true;
    }

    #region BehaviourControl

    private void StartComponent()
    {
        ReferencesForGamePlay.instance.m_34player.GetComponent<SoundEffects>().PlaySoundEffects(SoundEffects.Sounds.Narration);

        string msg = "";
        if (narrationComponentData.narrationsData.Length == 0)
            msg = "Narration data is empty";
        else
            msg = narrationComponentData.narrationsData;

        if (narrationComponentData.onStoryNarration)
        {
            BuilderEventManager.OnNarrationCollisionEnter?.Invoke(msg, true, narrationComponentData.onCloseNarration);
        }
        else if (narrationComponentData.onTriggerNarration)
        {
            BuilderEventManager.OnNarrationCollisionEnter?.Invoke(msg, false, narrationComponentData.onCloseNarration);
        }
    }
    private void StopComponent()
    {
        BuilderEventManager.OnNarrationCollisionExit?.Invoke();
    }

    public override void StopBehaviour()
    {
        if (isPlaying)
        {
            isPlaying = false;
            StopComponent();
        }
    }

    public override void PlayBehaviour()
    {
        isPlaying = true;
        StartComponent();
    }

    public override void ToggleBehaviour()
    {
        isPlaying = !isPlaying;

        if (isPlaying)
            PlayBehaviour();
        else
            StopBehaviour();
    }
    public override void ResumeBehaviour()
    {
        PlayBehaviour();
    }

    public override void AssignItemComponentType()
    {
        _componentType = Constants.ItemComponentType.NarrationComponent;
    }

    public override void CollisionExitBehaviour()
    {
        //throw new System.NotImplementedException();
    }

    public override void CollisionEnterBehaviour()
    {
        //CollisionEnter();
    }

    #endregion
}