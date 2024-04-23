using UnityEngine;
using Models;
using Photon.Pun;
using System.Collections;

public class TimerComponent : ItemComponent
{
    private bool isActivated = false;
    private bool IsAgainTouchable = true;
    private TimerComponentData timerComponentData;

    public void Init(TimerComponentData timerComponentData)
    {
        this.timerComponentData = timerComponentData;

        isActivated = true;
    }


    private void OnCollisionEnter(Collision _other)
    {
        if (_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            if (isActivated && timerComponentData.IsStart)
            {
                if (!IsAgainTouchable) return;

                IsAgainTouchable = false;
                BuilderEventManager.onComponentActivated?.Invoke(_componentType);
                PlayBehaviour();
            }
            if (isActivated && timerComponentData.IsEnd)
            {
                BuilderEventManager.OnTimerLimitEnd?.Invoke();
            }
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
        if (isActivated && timerComponentData.IsStart)
        {
            BuilderEventManager.OnTimerTriggerEnter?.Invoke("", timerComponentData.Timer + 1);
            CancelInvoke("StopBehaviour");
            BuilderEventManager.OnTimerLimitEnd += OnSubscribe;
        }
    }
    private void StopComponent()
    {
        BuilderEventManager.OnTimerTriggerEnter?.Invoke("", 0);
        BuilderEventManager.OnTimerLimitEnd -= OnSubscribe;
    }

    public void OnSubscribe()
    {
        Invoke("StopBehaviour", 5f);
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
        _componentType = Constants.ItemComponentType.TimerComponent;
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