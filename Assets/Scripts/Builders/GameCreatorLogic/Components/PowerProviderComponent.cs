using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;
using Photon.Pun;

public class PowerProviderComponent : ItemComponent
{
    public PowerProviderComponentData componentData;
    string RuntimeItemID = "";

    public void InitPowerProvider(PowerProviderComponentData componentData)
    {
        this.componentData = componentData;
        RuntimeItemID = this.GetComponent<XanaItem>().itemData.RuntimeItemID;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PhotonLocalPlayer" && other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            BuilderEventManager.onComponentActivated?.Invoke(_componentType);
            PlayBehaviour();
            GamificationComponentData.instance.photonView.RPC("GetObject", RpcTarget.All, RuntimeItemID, Constants.ItemComponentType.none);
        }
    }

    #region BehaviourControl
    private void StartComponent()
    {
        GamificationComponentData.instance.buildingDetect.OnPowerProviderEnter(componentData.setTimer, componentData.playerSpeed, componentData.playerHeight);
    }
    private void StopComponent()
    {

    }

    public override void StopBehaviour()
    {
        isPlaying = false;
        StopComponent();
    }

    public override void PlayBehaviour()
    {
        isPlaying = true;
        StartComponent();
    }

    public override void ToggleBehaviour()
    {
        isPlaying = !isPlaying;

        if (isPlaying)
            PlayBehaviour();
        else
            StopBehaviour();
    }
    public override void ResumeBehaviour()
    {
        PlayBehaviour();
    }

    public override void AssignItemComponentType()
    {
        _componentType = Constants.ItemComponentType.PowerProviderComponent;
    }

    #endregion
}
