using UnityEngine;
using Models;
using Photon.Pun;

public class NinjaComponent : ItemComponent
{
    NinjaComponentData ninjaComponentData;
    string RuntimeItemID = "";
    PlayerController pc;

    public void Init(NinjaComponentData ninjaComponentData)
    {
        this.ninjaComponentData = ninjaComponentData;
        RuntimeItemID = this.GetComponent<XanaItem>().itemData.RuntimeItemID;

        pc = GamificationComponentData.instance.playerControllerNew;
    }

    private void OnCollisionEnter(Collision _other)
    {
        if (_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            //return;
            BuilderEventManager.StopAvatarChangeComponent?.Invoke(true);

            // Special Item Component Stops
            //GamificationComponentData.instance.buildingDetect.StopSpecialItemComponent();
            // Special Item Component Stops

            //PlayerController pc = GamificationComponentData.instance.playerControllerNew;
            //pc.Ninja_Throw(true);
            //pc.NinjaComponentTimerStart(ninjaComponentData.setTimerNinjaEffect);
            //BuilderEventManager.OnNinjaMotionComponentCollisionEnter?.Invoke(ninjaComponentData.setTimerNinjaEffect);

            //pc.movementSpeed = ninjaComponentData.ninjaSpeedVar;
            //pc.sprintSpeed = ninjaComponentData.ninjaSpeedVar;
            //pc.jumpHeight = 5;
            //Destroy(this.gameObject);
            BuilderEventManager.onComponentActivated?.Invoke(_componentType);
            PlayBehaviour();
            if (GamificationComponentData.instance.withMultiplayer)
                GamificationComponentData.instance.photonView.RPC("GetObject", RpcTarget.All, RuntimeItemID, Constants.ItemComponentType.none);
            else GamificationComponentData.instance.GetObjectwithoutRPC(RuntimeItemID, Constants.ItemComponentType.none);
        }
    }

    #region BehaviourControl

    private void StartComponent()
    {
        ReferrencesForGameplay.instance.m_34player.GetComponent<SoundEffects>().PlaySoundEffects(SoundEffects.Sounds.Ninja);

        pc.Ninja_Throw(true);
        pc.NinjaComponentTimerStart(ninjaComponentData.setTimerNinjaEffect);
        pc.movementSpeed = ninjaComponentData.ninjaSpeedVar;
        pc.sprintSpeed = ninjaComponentData.ninjaSpeedVar;
        pc.jumpHeight = 5;
    }
    private void StopComponent()
    {
        // this method will never Call because this object is Destroy when the player touch to it.
        pc.NinjaComponentTimerStart(0);
    }

    public override void StopBehaviour()
    {
        if (isPlaying)
        {
            isPlaying = false;
            StopComponent();
        }
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
        _componentType = Constants.ItemComponentType.NinjaComponent;
    }

    #endregion
}