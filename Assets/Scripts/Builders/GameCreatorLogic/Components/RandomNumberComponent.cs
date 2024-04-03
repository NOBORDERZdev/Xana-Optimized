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
        _minNumber = this.randomNumberComponentData.minNumber;
        _maxNumber = this.randomNumberComponentData.maxNumber;
        GenerateNumber();
    }

    private void CollisionEnter()
    {
        if (!IsAgainTouchable) return;

        IsAgainTouchable = false;

        BuilderEventManager.onComponentActivated?.Invoke(_componentType);
        PlayBehaviour();
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
        ReferrencesForDynamicMuseum.instance.m_34player.GetComponent<SoundEffects>().PlaySoundEffects(SoundEffects.Sounds.RandomNumber);

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
        CollisionEnter();
    }

    #endregion
}