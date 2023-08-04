using UnityEngine;
using Models;
using Photon.Pun;

public class TimerComponent : ItemComponent
{

    private bool isActivated = false;
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
                BuilderEventManager.OnTimerTriggerEnter?.Invoke("", timerComponentData.Timer + 1);
            }
            else if (isActivated && timerComponentData.IsEnd)
            {
                BuilderEventManager.OnTimerTriggerEnter?.Invoke("", 0);
            }
        }
    }
}