using UnityEngine;
using Models;
using Photon.Pun;

public class ThrowThingsComponent : ItemComponent
{
    ThrowThingsComponentData throwThingsComponentData;
    string RuntimeItemID = "";

    public void Init(ThrowThingsComponentData throwThingsComponentData)
    {
        this.throwThingsComponentData = throwThingsComponentData;
        RuntimeItemID = this.GetComponent<XanaItem>().itemData.RuntimeItemID;
    }

    private void OnCollisionEnter(Collision _other)
    {
        if (_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine)
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
        GamificationComponentData.instance.playerControllerNew.Ninja_Throw(false, 1);
        BuilderEventManager.OnThrowThingsComponentCollisionEnter?.Invoke();
    }
    private void StopComponent()
    {
        // This Stop Cannot Work because the Item is Destroyed when player touch it
        GamificationComponentData.instance.playerControllerNew.ThrowThingsEnded();
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
        _componentType = Constants.ItemComponentType.ThrowThingsComponent;
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