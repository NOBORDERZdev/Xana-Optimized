using UnityEngine;
using Models;
using Photon.Pun;

public class RandomNumberComponent : ItemComponent
{
    float _minNumber = 0, _maxNumber = 0, GeneratedNumber = 0;

    [SerializeField]
    private RandomNumberComponentData randomNumberComponentData;
    private bool IsAgainTouchable = true;
    private bool isActivated = false;
    string RuntimeItemID = "";

    void GenerateNumber()
    {
        GeneratedNumber = (int)Random.Range(_minNumber, _maxNumber);
    }

    public void Init(RandomNumberComponentData randomNumberComponentData)
    {
        this.randomNumberComponentData = randomNumberComponentData;

        isActivated = true;
        RuntimeItemID = GetComponent<XanaItem>().itemData.RuntimeItemID;
        if (PhotonNetwork.IsMasterClient)
        {
            _minNumber = this.randomNumberComponentData.minNumber;
            _maxNumber = this.randomNumberComponentData.maxNumber;
            GenerateNumber();
            NetworkSyncManager.Instance.SyncPhotonView.RPC("SetRandomNumberComponent", RpcTarget.AllBufferedViaServer, RuntimeItemID, _minNumber, _maxNumber, GeneratedNumber);
        }
        else
        {
            var data = NetworkSyncManager.Instance.RandomNumberHist.Find(x => x.ItemID == RuntimeItemID);
            if (data != null)
            {
                _minNumber = data.MinNumber;
                _maxNumber = data.MaxNumber;
                GeneratedNumber = data.GeneratedNumber;
            }
        }
    }

    private void OnCollisionEnter(Collision _other)
    {
        if (_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            if (!IsAgainTouchable) return;

            IsAgainTouchable = false;

            BuilderEventManager.onComponentActivated?.Invoke(_componentType);
            PlayBehaviour();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        IsAgainTouchable = false;
    }

    //onCollsion Exit to ontrigger exit
    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag == "PhotonLocalPlayer" && other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            IsAgainTouchable = true;

            StopBehaviour();
        }
    }

    #region BehaviourControl
    private void StartComponent()
    {
        ReferencesForGamePlay.instance.m_34player.GetComponent<SoundEffects>().PlaySoundEffects(SoundEffects.Sounds.RandomNumber);

        BuilderEventManager.OnRandomCollisionEnter?.Invoke(GeneratedNumber);
        GenerateNumber();
    }
    private void StopComponent()
    {
        BuilderEventManager.OnRandomCollisionExit?.Invoke();
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
        _componentType = Constants.ItemComponentType.RandomNumberComponent;
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