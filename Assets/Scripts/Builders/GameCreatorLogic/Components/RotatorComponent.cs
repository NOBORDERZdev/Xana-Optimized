using System.Collections;
using UnityEngine;
using Models;
using Photon.Pun;

public class RotatorComponent : ItemComponent
{
    RotatorComponentData _rotatorComponentData;
    bool _startComponent;
    Vector3 _currentRotation;
    string _itemID;
    float _mAngle;

    public void Init(RotatorComponentData rotatorComponentData, string itemid)
    {
        InitRotate(rotatorComponentData);
        _itemID = itemid;
        NetworkSyncManager.Instance.RotatorComponent.TryAdd(itemid, Vector3.zero);
        NetworkSyncManager.Instance.OnDeserilized += Sync;
    }

    public void InitRotate(RotatorComponentData rotatorComponentData)
    {
        this._rotatorComponentData = rotatorComponentData;

        PlayBehaviour();
    }
    void Sync()
    {
        object component;
        if (NetworkSyncManager.Instance.RotatorComponent.TryGetValue(_itemID, out component))
        {
            this._mAngle = Quaternion.Angle(transform.rotation, Quaternion.Euler((Vector3)component));
        }

    }


    private void Update() //Provide better performance than infinite corutine
    {
        if (_startComponent)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                _currentRotation = gameObject.transform.rotation.eulerAngles;
                _currentRotation += new Vector3(0f, _rotatorComponentData.speed * Time.deltaTime, 0f);
                gameObject.transform.rotation = Quaternion.Euler(_currentRotation);
                NetworkSyncManager.Instance.RotatorComponent[_itemID] = _currentRotation;
                //  Debug.LogError("Setting data" + (Vector3)NetworkSyncManager.instance.rotatorComponent[itemID]);

            }
            else
            {
                object obj;
                if (NetworkSyncManager.Instance.RotatorComponent.TryGetValue(_itemID, out obj))
                {
                    _currentRotation = (Vector3)obj;
                    //     Debug.LogError("Rotation  " + currentRotation);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(this._currentRotation), this._mAngle * (1.0f / PhotonNetwork.SerializationRate));
                }

            }
        }

    }

    #region BehaviourControl
    private void StartComponent()
    {
        StopComponent();

        _startComponent = true;
    }
    private void StopComponent()
    {
        _startComponent = false;
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
        _componentType = Constants.ItemComponentType.RotatorComponent;
    }

    public override void CollisionExitBehaviour()
    {
        //throw new System.NotImplementedException();
    }

    public override void CollisionEnterBehaviour()
    {
        //throw new System.NotImplementedException();
    }

    #endregion
}