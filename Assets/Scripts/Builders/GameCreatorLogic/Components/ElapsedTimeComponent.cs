using UnityEngine;
using Models;
using Photon.Pun;

public class ElapsedTimeComponent : ItemComponent
{
    private bool isActivated = false;
    private bool IsAgainTouchable = true;
    [SerializeField]
    private ElapsedTimeComponentData elapsedTimeComponentData;

    public void Init(ElapsedTimeComponentData elapsedTimeComponentData)
    {
        this.elapsedTimeComponentData = elapsedTimeComponentData;
        isActivated = true;
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

    #region BehaviourControler
    private void StartComponent()
    {
        if (isActivated && elapsedTimeComponentData.IsStart)
        {
            TimeStats._timeStop?.Invoke(0, () => { TimeStats._timeStart?.Invoke(); });
        }
        if (isActivated && elapsedTimeComponentData.IsEnd)
        {
            TimeStats._timeStop?.Invoke(5, () => { });
        }
    }
    private void StopComponent()
    {
        TimeStats._timeStop?.Invoke(0, () => { });
    }

    public override void PlayBehaviour()
    {
        isPlaying = true;
        StartComponent();
    }
    public override void StopBehaviour()
    {
        if (isPlaying)
        {
            isPlaying = false;
            StopComponent();
        }
    }

    public override void ResumeBehaviour()
    {
        PlayBehaviour();
    }

    public override void ToggleBehaviour()
    {
        isPlaying = !isPlaying;

        if (isPlaying)
            PlayBehaviour();
        else
            StopBehaviour();
    }
    public override void AssignItemComponentType()
    {
        _componentType = Constants.ItemComponentType.ElapsedTimeComponent;
    }

    #endregion
}