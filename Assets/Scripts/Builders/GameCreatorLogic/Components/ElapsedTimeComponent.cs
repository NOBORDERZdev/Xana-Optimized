using UnityEngine;
using Models;
using Photon.Pun;
using System.Collections;
using UnityEngine.UI;

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
            if (isActivated && elapsedTimeComponentData.IsStart)
            {
                if (!IsAgainTouchable) return;

                IsAgainTouchable = false;
                BuilderEventManager.onComponentActivated?.Invoke(_componentType);
                PlayBehaviour();
            }
            if (isActivated && elapsedTimeComponentData.IsEnd)
            {
                BuilderEventManager.elapsedEndTime?.Invoke();
            }
        }
    }

    //private void OnCollisionStay(Collision collision)
    //{
    //    IsAgainTouchable = false;
    //}
    //private void OnCollisionExit(Collision collision)
    //{
    //    IsAgainTouchable = true;
    //}

    #region BehaviourControler
    private void StartComponent()
    {
        if (isActivated && elapsedTimeComponentData.IsStart)
        {
            TimeStats._timeStop?.Invoke(0, () => { TimeStats._timeStart?.Invoke(); });
            CancelInvoke("StopBehaviour");
            BuilderEventManager.elapsedEndTime += OnSubscribe;
        }
    }
    public void OnSubscribe()
    {
        Invoke("StopBehaviour", 5f);
    }

    private void StopComponent()
    {
        TimeStats._timeStop?.Invoke(0, () => { });
        BuilderEventManager.elapsedEndTime -= OnSubscribe;
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