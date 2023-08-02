using UnityEngine;
using Models;
using Photon.Pun;

public class RandomNumberComponent : ItemComponent
{
    float _minNumber = 0, _maxNumber = 0, GeneratedNumber = 0;

    [SerializeField]
    private RandomNumberComponentData randomNumberComponentData;
    private bool isActivated = false;

    // Start is called before the first frame update
    void Start()
    {
        _minNumber = randomNumberComponentData.minNumber;
        _maxNumber = randomNumberComponentData.maxNumber;
        GenerateNumber();
    }

    void GenerateNumber()
    {
        GeneratedNumber = (int)Random.Range(_minNumber, _maxNumber);
    }

    public void Init(RandomNumberComponentData randomNumberComponentData)
    {
        this.randomNumberComponentData = randomNumberComponentData;

        isActivated = true;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "PhotonLocalPlayer" && other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            BuilderEventManager.OnRandomCollisionEnter?.Invoke(GeneratedNumber);
            GenerateNumber();
        }
    }

    //onCollsion Exit to ontrigger exit
    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag == "PhotonLocalPlayer" && other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            BuilderEventManager.OnRandomCollisionExit?.Invoke();
        }
    }
}