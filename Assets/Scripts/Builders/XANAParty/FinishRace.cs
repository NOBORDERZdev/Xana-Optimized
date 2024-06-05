using Photon.Pun;
using UnityEngine;

public class FinishRace : MonoBehaviour
{
    [SerializeField]
    FinishPoint _finishPoint;

    private void OnCollisionEnter(Collision _other)
    {
        if (_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            _finishPoint.FinishRace();
        }
    }
}
