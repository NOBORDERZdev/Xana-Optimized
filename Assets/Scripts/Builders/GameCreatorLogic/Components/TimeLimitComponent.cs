using UnityEngine;
using Models;
using Photon.Pun;

public class TimeLimitComponent : ItemComponent
{
    [SerializeField]
    private bool isActivated = false;
    [SerializeField]
    private TimeLimitComponentData timeLimitComponentData;
    string RuntimeItemID = "";

    public void Init(TimeLimitComponentData timeLimitComponentData)
    {
        this.timeLimitComponentData = timeLimitComponentData;

        isActivated = true;
        RuntimeItemID = GetComponent<XanaItem>().itemData.RuntimeItemID;
    }


    private void OnCollisionEnter(Collision _other)
    {
        if (isActivated)
        {
            if (_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine)
            {
                BuilderEventManager.onComponentActivated?.Invoke(_componentType);
                PlayBehaviour();
            }
        }
    }

    #region BehaviourControl
    private void StartComponent()
    {
        BuilderEventManager.OnTimerLimitTriggerEnter?.Invoke(timeLimitComponentData.PurposeHeading, timeLimitComponentData.TimeLimitt + 1);
    }
    private void StopComponent()
    {
        BuilderEventManager.OnTimerLimitTriggerEnter?.Invoke(timeLimitComponentData.PurposeHeading, 0);
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
        _componentType = Constants.ItemComponentType.TimeLimitComponent;
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