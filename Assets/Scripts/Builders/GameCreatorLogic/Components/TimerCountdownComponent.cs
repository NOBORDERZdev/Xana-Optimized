using UnityEngine;
using Models;
using Photon.Pun;

public class TimerCountdownComponent : ItemComponent
{

    public TimerCountdownComponentData timerCountdownComponentData;
    public int timerLimit, i, defaultValue;
    string RuntimeItemID = "";

    public void Init(TimerCountdownComponentData timerCountdownComponentData)
    {
        this.timerCountdownComponentData = timerCountdownComponentData;
        defaultValue = (int)timerCountdownComponentData.setTimer - 1;
        RuntimeItemID = GetComponent<XanaItem>().itemData.RuntimeItemID;
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "PhotonLocalPlayer" && other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            BuilderEventManager.onComponentActivated?.Invoke(_componentType);
            PlayBehaviour();
        }
    }

    #region BehaviourControl
    private void StartComponent()
    {
        BuilderEventManager.OnTimerCountDownTriggerEnter?.Invoke(defaultValue, true);
    }
    private void StopComponent()
    {
        BuilderEventManager.OnTimerCountDownTriggerEnter?.Invoke(0, false);
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
        _componentType = Constants.ItemComponentType.TimerCountdownComponent;
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