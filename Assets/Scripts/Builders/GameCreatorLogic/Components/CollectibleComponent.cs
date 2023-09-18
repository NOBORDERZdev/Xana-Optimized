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
        RuntimeItemID = GetComponent<XanaItem>().itemData.RuntimeItemID;
    }

    private void OnCollisionEnter(Collision _other)
    {
        if (_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            GamificationComponentData.instance.photonView.RPC("GetObject", RpcTarget.AllBuffered, RuntimeItemID, _componentType);
        }
    }

    #region BehaviourControl
    public void StartComponent()
    {
        this.gameObject.SetActive(false);
        //Toast.Show(XanaConstants.collectibleMsg);
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
        _componentType = Constants.ItemComponentType.CollectibleComponent;
    }

    #endregion
}