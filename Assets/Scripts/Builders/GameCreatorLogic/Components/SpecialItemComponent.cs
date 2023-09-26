using Models;
using Photon.Pun;
using UnityEngine;

public class SpecialItemComponent : ItemComponent
{
    SpecialItemComponentData specialItemComponentData;
    string RuntimeItemID = "";

    public void Init(SpecialItemComponentData componentData)
    {
        this.specialItemComponentData = componentData;
        RuntimeItemID = this.GetComponent<XanaItem>().itemData.RuntimeItemID;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "PhotonLocalPlayer" && other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            //TimeStats._intensityChangerStop?.Invoke();
            //TimeStats._blindComponentStop?.Invoke();
            //// Ninja Component Stops
            //PlayerControllerNew pc = GamificationComponentData.instance.playerControllerNew;
            //pc.NinjaComponentTimerStart(0);
            //BuilderEventManager.OnNinjaMotionComponentCollisionEnter?.Invoke(0);
            //// Ninja Component Stops

            //GamificationComponentData.instance.buildingDetect.StoppingCoroutine();

            //this.gameObject.SetActive(false);
            BuilderEventManager.onComponentActivated?.Invoke(_componentType);
            PlayBehaviour();
            if(GamificationComponentData.instance.withMultiplayer)
                GamificationComponentData.instance.photonView.RPC("GetObject", RpcTarget.All, RuntimeItemID, Constants.ItemComponentType.none);
            else
                GamificationComponentData.instance.GetObject(RuntimeItemID, Constants.ItemComponentType.none);

        }
    }

    #region BehaviourControl

    private void StartComponent()
    {
        GamificationComponentData.instance.buildingDetect.
                    SpecialItemPowerUp(specialItemComponentData.setTimer, specialItemComponentData.playerSpeed, specialItemComponentData.playerHeight);

    }
    private void StopComponent()
    {
        // this method will never Call because this object is Destroy when the player touch to it.
        GamificationComponentData.instance.buildingDetect.StopSpecialItemComponent();
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
        _componentType = Constants.ItemComponentType.SpecialItemComponent;
    }

    #endregion

}