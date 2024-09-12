using Photon.Pun;
using UnityEngine;

public class SMBCPlanetInfo : MonoBehaviour
{
    public string PlanetName;
    private bool _alreadyTriggered = false;

    private void Start()
    {
        if (SMBCManager.Instance.CheckRocketPartCollectOrNot(PlanetName))
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PhotonLocalPlayer" && other.gameObject.GetComponent<PhotonView>().IsMine && !_alreadyTriggered)
        {
            _alreadyTriggered = !_alreadyTriggered;
            SMBCManager.Instance.CurrentPlanetName = PlanetName;
        }
    }
}
