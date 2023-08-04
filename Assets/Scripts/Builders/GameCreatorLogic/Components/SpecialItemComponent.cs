using Models;
using Photon.Pun;
using UnityEngine;

public class SpecialItemComponent : ItemComponent
{
    SpecialItemComponentData specialItemComponentData;

    public void Init(SpecialItemComponentData componentData)
    {
        this.specialItemComponentData = componentData;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "PhotonLocalPlayer" && other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            TimeStats._intensityChangerStop?.Invoke();
            // Ninja Component Stops
            PlayerControllerNew pc = GamificationComponentData.instance.playerControllerNew;
            pc.NinjaComponentTimerStart(0);
            BuilderEventManager.OnNinjaMotionComponentCollisionEnter?.Invoke(0);
            // Ninja Component Stops

            GamificationComponentData.instance.buildingDetect.StoppingCoroutine();
            GamificationComponentData.instance.buildingDetect.
                SpecialItemPowerUp(specialItemComponentData.setTimer, specialItemComponentData.playerSpeed, specialItemComponentData.playerHeight);
            this.gameObject.SetActive(false);
        }
    }
}