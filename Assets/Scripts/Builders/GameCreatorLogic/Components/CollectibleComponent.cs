using UnityEngine;
using Models;
using Photon.Pun;

public class CollectibleComponent : ItemComponent
{

    private bool activateComponent = true;
    string RuntimeItemID = "";

    public void Init(CollectibleComponentData collectibleComponentData)
    {
        activateComponent = true;
        RuntimeItemID = GetComponent<BuilderItem>().itemData.RuntimeItemID;
    }

    private void OnCollisionEnter(Collision _other)
    {
        if (_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            BuilderEventManager.onComponentActivated?.Invoke(_componentType);
            PlayBehaviour();
            if (GamificationComponentData.instance.withMultiplayer)
                GamificationComponentData.instance.photonView.RPC("GetObject", RpcTarget.All, RuntimeItemID, Constants.ItemComponentType.none);
            else GamificationComponentData.instance.GetObjectwithoutRPC(RuntimeItemID, Constants.ItemComponentType.none);
        }
    }

    #region BehaviourControl
    public void StartComponent()
    {
        ReferrencesForGameplay.instance.m_34player.GetComponent<SoundEffects>().PlaySoundEffects(SoundEffects.Sounds.Collectible);
        //this.gameObject.SetActive(false);
        //Toast.Show(ConstantsHolder.collectibleMsg);
    }

    private void StopComponent()
    {


    }

    public override void StopBehaviour()
    {
        if(isPlaying)
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
        _componentType = Constants.ItemComponentType.CollectibleComponent;
    }

    #endregion
}