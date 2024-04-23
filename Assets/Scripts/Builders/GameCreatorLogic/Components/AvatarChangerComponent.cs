using UnityEngine;
using Models;
using Photon.Pun;

//[RequireComponent(typeof(Rigidbody))]
public class AvatarChangerComponent : ItemComponent
{
    public AvatarChangerComponentData componentData;
    string RuntimeItemID = "";

    public void InitAvatarChanger(AvatarChangerComponentData componentData)
    {
        this.componentData = componentData;
        RuntimeItemID = this.GetComponent<XanaItem>().itemData.RuntimeItemID;
    }

    private void OnCollisionEnter(Collision _other)
    {
        if (_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            if (componentData.avatarIndex == 1)//Hunter Selected
                Toast.Show("Coming Soon, We will update The Hunter Appearance");
            else
            {
                BuilderEventManager.onComponentActivated?.Invoke(_componentType);
                PlayBehaviour();
            }
            if (GamificationComponentData.instance.withMultiplayer)
                GamificationComponentData.instance.photonView.RPC("GetObject", RpcTarget.All, RuntimeItemID, Constants.ItemComponentType.none);
            else GamificationComponentData.instance.GetObjectwithoutRPC(RuntimeItemID, Constants.ItemComponentType.none);
        }
    }

    #region BehaviourControl

    private void StartComponent()
    {
        //GamificationComponentData.instance.buildingDetect.StopSpecialItemComponent();
        //GamificationComponentData.instance.playerControllerNew.NinjaComponentTimerStart(0);
        if (componentData.avatarIndex > 1)
        {
            GamificationComponentData.instance.buildingDetect.OnAvatarChangerEnter(componentData.setTimer, componentData.avatarIndex, this.gameObject);
        }
    }
    private void StopComponent()
    {
        // this method will never Call because this object is Destroy when the player touch to it.
        BuilderEventManager.StopAvatarChangeComponent?.Invoke(true);
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
        _componentType = Constants.ItemComponentType.AvatarChangerComponent;
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