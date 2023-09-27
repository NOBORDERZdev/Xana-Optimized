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

            GamificationComponentData.instance.buildingDetect.StopSpecialItemComponent();
            GamificationComponentData.instance.playerControllerNew.NinjaComponentTimerStart(0);
            if (componentData.avatarIndex == 1)//Hunter Selected
            {
                Toast.Show("Coming Soon, We will update The Hunter Appearance");
            }
            else if (componentData.avatarIndex > 1)
            {
                GamificationComponentData.instance.buildingDetect.OnAvatarChangerEnter(componentData.setTimer, componentData.avatarIndex, this.gameObject);
            }
            this.gameObject.SetActive(false);

        }
    }
}