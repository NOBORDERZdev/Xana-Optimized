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
            BuilderEventManager.onComponentActivated?.Invoke(_componentType);
            PlayBehaviour();
            if (GamificationComponentData.instance.withMultiplayer)
                GamificationComponentData.instance.photonView.RPC("GetObject", RpcTarget.All, RuntimeItemID, Constants.ItemComponentType.none);
            else
                GamificationComponentData.instance.GetObjectwithoutRPC(RuntimeItemID, Constants.ItemComponentType.none);
        }
    }

    #region BehaviourControl

    private void StartComponent()
    {
        ReferencesForGamePlay.instance.m_34player.GetComponent<SoundEffects>().PlaySoundEffects(SoundEffects.Sounds.SpecialItem);

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
        _componentType = Constants.ItemComponentType.SpecialItemComponent;
    }

    public override void CollisionExitBehaviour()
    {
        //throw new System.NotImplementedException();
    }

    public override void CollisionEnterBehaviour()
    {
        //CollisionEnter();
    }

    #endregion

}