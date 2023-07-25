using UnityEngine;
using Models;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody))]
public class AvatarChangerComponent : ItemComponent
{
    public AvatarChangerComponentData componentData;

    public void InitAvatarChanger(AvatarChangerComponentData componentData)
    {
        this.componentData = componentData;
    }

    private void OnCollisionEnter(Collision _other)
    {
        if (_other.gameObject.tag == "Player" || (_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine))
        {

            if (componentData.avatarIndex > 0)
            {
                GamificationComponentData.instance.buildingDetect.OnAvatarChangerEnter(componentData.setTimer, componentData.avatarIndex);
                this.gameObject.SetActive(false);
            }

            GamificationComponentData.instance.playerControllerNew.NinjaComponentTimerStart(0);
        }
    }
}