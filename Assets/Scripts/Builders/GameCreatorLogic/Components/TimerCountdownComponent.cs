using UnityEngine;
using Models;
using Photon.Pun;

public class TimerCountdownComponent : ItemComponent
{

    public TimerCountdownComponentData timerCountdownComponentData;
    public int timerLimit, i, defaultValue;

    public void Init(TimerCountdownComponentData timerCountdownComponentData)
    {
        this.timerCountdownComponentData = timerCountdownComponentData;
        defaultValue = (int)timerCountdownComponentData.setTimer - 1;

    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "PhotonLocalPlayer" && other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            i = defaultValue;
            BuilderEventManager.OnTimerCountDownTriggerEnter?.Invoke(i, true);
        }
    }
}