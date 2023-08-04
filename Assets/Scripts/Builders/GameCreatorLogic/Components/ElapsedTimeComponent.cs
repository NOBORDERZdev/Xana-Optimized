using UnityEngine;
using Models;
using Photon.Pun;

public class ElapsedTimeComponent : ItemComponent
{
    [Tooltip("Set Total Time")]
    [SerializeField]
    private float m_TotalTime;
    private bool isActivated = false;
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
                TimeStats._timeStop?.Invoke(0, () => { TimeStats._timeStart?.Invoke(); });
            }
            if (isActivated && elapsedTimeComponentData.IsEnd)
            {
                TimeStats._timeStop?.Invoke(5, () => { });
            }
        }
    }
}