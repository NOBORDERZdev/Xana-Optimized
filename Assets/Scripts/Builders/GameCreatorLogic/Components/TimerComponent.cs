using UnityEngine;
using Models;
using Photon.Pun;

public class TimerComponent : ItemComponent
{

    private bool isActivated = false;
    private TimerComponentData timerComponentData;
    string RuntimeItemID = "";

    public void Init(TimerComponentData timerComponentData)
    {
        this.timerComponentData = timerComponentData;

        isActivated = true;

        RuntimeItemID = GetComponent<XanaItem>().itemData.RuntimeItemID;
    }


    private void OnCollisionEnter(Collision _other)
    {
        if (_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            BuilderEventManager.onComponentActivated?.Invoke(_componentType);
            PlayBehaviour();
        }
    }

    #region BehaviourControl

    private void StartComponent()
    {
        if (isActivated && timerComponentData.IsStart)
        {
            BuilderEventManager.OnTimerTriggerEnter?.Invoke("", timerComponentData.Timer + 1);
        }
        else if (isActivated && timerComponentData.IsEnd)
        {
            BuilderEventManager.OnTimerTriggerEnter?.Invoke("", 0);
        }
    }
    private void StopComponent()
    {
        BuilderEventManager.OnTimerTriggerEnter?.Invoke("", 0);
    }

    public override void StopBehaviour()
    {
        isPlaying = false;
        StopComponent();
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

    #endregion
}