using UnityEngine;
using Models;
using Photon.Pun;

public class ThrowThingsComponent : MonoBehaviour
{
    ThrowThingsComponentData throwThingsComponentData;

    public void Init(ThrowThingsComponentData throwThingsComponentData)
    {
        this.throwThingsComponentData = throwThingsComponentData;
    }

    private void OnCollisionEnter(Collision _other)
    {
        if (_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            BuilderEventManager.StopAvatarChangeComponent?.Invoke(true);
            GamificationComponentData.instance.buildingDetect.StopSpecialItemComponent();

            GamificationComponentData.instance.playerControllerNew.Ninja_Throw(false, 1);
            BuilderEventManager.OnThrowThingsComponentCollisionEnter?.Invoke();
            Destroy(gameObject);
        }
    }
}