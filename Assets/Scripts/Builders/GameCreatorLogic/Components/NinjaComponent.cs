using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;
using Photon.Pun;

public class NinjaComponent : MonoBehaviour
{
    NinjaComponentData ninjaComponentData;
    public void Init(NinjaComponentData ninjaComponentData)
    {
        this.ninjaComponentData = ninjaComponentData;
    }

    private void OnCollisionEnter(Collision _other)
    {
        if (/*_other.gameObject.CompareTag("Player") || */(_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine))
        {
            //return;
            // Special Item Component Stops
            GamificationComponentData.instance.buildingDetect.StopSpecialItemComponent();
            // Special Item Component Stops

            PlayerControllerNew pc = GamificationComponentData.instance.playerControllerNew;
            //pc.Ninja_Throw(true);
            //pc.NinjaComponentTimerStart(ninjaComponentData.setTimerNinjaEffect);
            BuilderEventManager.OnNinjaMotionComponentCollisionEnter?.Invoke(ninjaComponentData.setTimerNinjaEffect);

            pc.movementSpeed = ninjaComponentData.ninjaSpeedVar;
            pc.sprintSpeed = ninjaComponentData.ninjaSpeedVar;
            pc.jumpHeight = 5;
            Destroy(this.gameObject);
        }
    }
}
