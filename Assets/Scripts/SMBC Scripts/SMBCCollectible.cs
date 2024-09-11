using Photon.Pun;
using UnityEngine;

public class SMBCCollectible : MonoBehaviour
{
    public SMBCCollectibleType CollectibleType;
    bool _IsTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PhotonLocalPlayer" && other.gameObject.GetComponent<PhotonView>().IsMine && !_IsTriggered)
        {
            _IsTriggered = !_IsTriggered;
            CollectibleProgress(CollectibleType);
            Destroy(gameObject, 0.05f);
        }
    }

    void CollectibleProgress(SMBCCollectibleType collectibleType)
    {
        switch (collectibleType)
        {
            case SMBCCollectibleType.DoorKey:
                SMBCManager.Instance.AddKey();
                break;
            case SMBCCollectibleType.RocketPart:
                SMBCManager.Instance.AddRocketPart();
                break;
            case SMBCCollectibleType.Axe:
                SMBCManager.Instance.AddRocketPart();
                break;
        }
    }
}
