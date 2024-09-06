using DG.Tweening;
using Models;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class TransformComponent : ItemComponent, IInRoomCallbacks
{
    public bool RotateObject;
    public bool ScaleObject;
    public bool TransObject;
    float _timeSpent = 0;
    string _itemID;
    float _elapsed = 0f;

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private void Update()
    {
        _elapsed += Time.deltaTime;
        if (_elapsed >= 1f)
        {
            _elapsed = _elapsed % 1f;
            _timeSpent++;
            _timeSpent %= _toFroComponentData != null ? (_toFroComponentData.timeToAnimate * 2) : _rotateComponentData != null ? (_rotateComponentData.timeToAnimate * 2) : (_scalerComponentData.timeToAnimate * 2);//(rotateComponentData.timeToAnimate*2); 
        }

        object component;
        if (RotateObject)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                NetworkSyncManager.Instance.TransformComponentrotation[_itemID] = transform.rotation;
                NetworkSyncManager.Instance.TransformComponentTime[_itemID] = _timeSpent;
            }
            else
            {
                if (NetworkSyncManager.Instance.TransformComponentrotation.TryGetValue(_itemID, out component))
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, (Quaternion)NetworkSyncManager.Instance.TransformComponentrotation[_itemID], this._mAngle * (1.0f / PhotonNetwork.SerializationRate));
                }
                if (NetworkSyncManager.Instance.TransformComponentTime.TryGetValue(_itemID, out var obj))
                {
                    _timeSpent = obj;
                }
            }
        }
        if (ScaleObject)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                NetworkSyncManager.Instance.TransformComponentScale[_itemID] = transform.localScale;
                NetworkSyncManager.Instance.TransformComponentTime[_itemID] = _timeSpent;
            }
            else
            {
                if (NetworkSyncManager.Instance.TransformComponentScale.TryGetValue(_itemID, out component))
                {
                    transform.localScale = (Vector3)component;
                }
                if (NetworkSyncManager.Instance.TransformComponentTime.TryGetValue(_itemID, out var obj))
                {
                    _timeSpent = obj;
                }
            }

        }
        if (TransObject)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                NetworkSyncManager.Instance.TransformComponentPos[_itemID] = transform.position;
                NetworkSyncManager.Instance.TransformComponentTime[_itemID] = _timeSpent;
            }
            else
            {
                if (NetworkSyncManager.Instance.TransformComponentPos.TryGetValue(_itemID, out component))
                {
                    transform.position = Vector3.MoveTowards(transform.position, (Vector3)component, this._mDistance * (1.0f / PhotonNetwork.SerializationRate)); ;
                }
                if (NetworkSyncManager.Instance.TransformComponentTime.TryGetValue(_itemID, out var obj))
                {
                    _timeSpent = obj;
                }
            }

        }

    }

    private void Start()
    {
        NetworkSyncManager.Instance.OnDeserilized += Sync;
    }
    void Sync()
    {
        object component;


        if (RotateObject)
        {
            if (NetworkSyncManager.Instance.TransformComponentrotation.TryGetValue(_itemID, out component))
            {
                this._mAngle = Quaternion.Angle(transform.rotation, (Quaternion)component);
            }
        }
        if (TransObject)
        {
            if (NetworkSyncManager.Instance.TransformComponentPos.TryGetValue(_itemID, out component))
            {
                this._mDistance = Vector3.Distance(transform.position, (Vector3)component);
            }
        }
    }
    public void increasTime()
    {
        _timeSpent++;

        _timeSpent %= _toFroComponentData != null ? (_toFroComponentData.timeToAnimate * 2) : _rotateComponentData != null ? (_rotateComponentData.timeToAnimate * 2) : (_scalerComponentData.timeToAnimate * 2);//(rotateComponentData.timeToAnimate*2); 
    }
    Ease AnimationCurveValueConvertor(int index)
    {
        switch (index)
        {
            case 0:
                return Ease.Linear;
                Debug.Log("Linear");
                break;
            case 1:
                return Ease.InOutBounce;
                Debug.Log("Spring");
                break;
            case 2:
                return Ease.InSine;
                Debug.Log("easeIn");
                break;
            case 3:
                return Ease.OutSine;
                Debug.Log("easeOut");
                break;
            case 4:
                return Ease.InOutSine;
                Debug.Log("easeInOut");
                break;
            case 5:
                return Ease.InOutBack;
            case 6:
                return Ease.InOutCirc;
            case 7:
                return Ease.InOutExpo;
            case 8:
                return Ease.OutBounce;
            case 9:
                return Ease.OutQuint;

            default:
                return Ease.Linear;
                break;
        }
    }

    #region Rotate Module
    RotateComponentData _rotateComponentData;

    public void InitRotate(RotateComponentData rotateComponentData, string itemid)
    {
        this._rotateComponentData = rotateComponentData;
        // StartCoroutine(rotateModule());
        _itemID = itemid;
        NetworkSyncManager.Instance.TransformComponentrotation.TryAdd(itemid, transform.rotation);
        NetworkSyncManager.Instance.TransformComponentTime.TryAdd(itemid, _timeSpent);
        if (PhotonNetwork.IsMasterClient)
        {

            RotateFromAtoB();
            //    InvokeRepeating(nameof(increasTime), 1, 99999);
        }
        RotateObject = true;
    }

    /* IEnumerator rotateModule()
     {
         //StartComponent();
         while (true)
         {

             yield return transform.DORotate(rotateComponentData.maxValue, rotateComponentData.timeToAnimate).SetEase(AnimationCurveValueConvertor(rotateComponentData.animationCurveIndex)).WaitForCompletion();
             yield return transform.DORotate(rotateComponentData.defaultValue, rotateComponentData.timeToAnimate).SetEase(AnimationCurveValueConvertor(rotateComponentData.animationCurveIndex)).WaitForCompletion();
         }
     }*/

    private void RotateFromAtoB()
    {

        transform.DORotate(_rotateComponentData.maxValue, _rotateComponentData.timeToAnimate - (_timeSpent % _rotateComponentData.timeToAnimate)).SetEase(AnimationCurveValueConvertor(_rotateComponentData.animationCurveIndex)).OnComplete(RotateFromBtoA);

    }
    private void RotateFromBtoA()
    {
        transform.DORotate(_rotateComponentData.defaultValue, _rotateComponentData.timeToAnimate - (_timeSpent % _rotateComponentData.timeToAnimate)).SetEase(AnimationCurveValueConvertor(_rotateComponentData.animationCurveIndex)).OnComplete(RotateFromAtoB);
    }


    #endregion


    #region ToAndFro Module
    public Ease ToFroEaseType;
    ToFroComponentData _toFroComponentData;
    public void InitToFro(ToFroComponentData toFroComponentData, string itemid)
    {
        this._toFroComponentData = toFroComponentData;
        NetworkSyncManager.Instance.TransformComponentPos.TryAdd(itemid, transform.position);
        NetworkSyncManager.Instance.TransformComponentTime.TryAdd(itemid, _timeSpent);
        if (PhotonNetwork.IsMasterClient)
        {
            MoveFromAtoB();
            // InvokeRepeating(nameof(increasTime), 1, 99999);
        }
        _itemID = itemid;

        TransObject = true;

        //   StartCoroutine(toFroModule());
    }
    private void MoveFromAtoB()//Better than loop call Functions
    {
        transform.DOMove(_toFroComponentData.maxValue, _toFroComponentData.timeToAnimate - (_timeSpent % _toFroComponentData.timeToAnimate)).SetEase(AnimationCurveValueConvertor(_toFroComponentData.animationCurveIndex)).OnComplete(MoveFromBtoA);
    }
    private void MoveFromBtoA()
    {
        transform.DOMove(_toFroComponentData.defaultValue, _toFroComponentData.timeToAnimate - (_timeSpent % _toFroComponentData.timeToAnimate)).SetEase(AnimationCurveValueConvertor(_toFroComponentData.animationCurveIndex)).OnComplete(MoveFromAtoB);
    }


    /*IEnumerator toFroModule()
    {
        //StartComponent();
        while (true)
        {

            yield return transform.DOMove(toFroComponentData.maxValue, toFroComponentData.timeToAnimate).SetEase(AnimationCurveValueConvertor(toFroComponentData.animationCurveIndex)).WaitForCompletion();
            yield return transform.DOMove(toFroComponentData.defaultValue, toFroComponentData.timeToAnimate).SetEase(AnimationCurveValueConvertor(toFroComponentData.animationCurveIndex)).WaitForCompletion();
        }
    }*/

    #endregion


    #region Scale Module

    public Ease ScalerEaseType;
    ScalerComponentData _scalerComponentData;
    private float _mAngle;
    private float _mDistance;

    public void InitScale(ScalerComponentData scalerComponentData, string itemid)
    {
        this._scalerComponentData = scalerComponentData;
        NetworkSyncManager.Instance.TransformComponentTime.TryAdd(itemid, _timeSpent);
        NetworkSyncManager.Instance.TransformComponentScale.TryAdd(itemid, transform.localScale);
        // StartCoroutine(ScalingObject());
        if (PhotonNetwork.IsMasterClient)
        {
            ScaleFormAtoB();
            //  InvokeRepeating(nameof(increasTime), 1, 99999);
        }
        _itemID = itemid;

        ScaleObject = true;
    }

    /*IEnumerator ScalingObject()
    {
        //StartComponent();
        while (true)
        {
            yield return transform.DOScale(scalerComponentData.maxScaleValue, scalerComponentData.timeToAnimate).SetEase(AnimationCurveValueConvertor(scalerComponentData.animationCurveIndex)).WaitForCompletion();
            yield return transform.DOScale(scalerComponentData.defaultScaleValue, scalerComponentData.timeToAnimate).SetEase(AnimationCurveValueConvertor(scalerComponentData.animationCurveIndex)).WaitForCompletion();
        }
    }*/

    private void ScaleFormAtoB()
    {
        transform.DOScale(_scalerComponentData.maxScaleValue, _scalerComponentData.timeToAnimate - (_timeSpent % _scalerComponentData.timeToAnimate)).SetEase(AnimationCurveValueConvertor(_scalerComponentData.animationCurveIndex)).OnComplete(ScaleFormBtoA);
    }
    private void ScaleFormBtoA()
    {
        transform.DOScale(_scalerComponentData.defaultScaleValue, _scalerComponentData.timeToAnimate - (_timeSpent % _scalerComponentData.timeToAnimate)).SetEase(AnimationCurveValueConvertor(_scalerComponentData.animationCurveIndex)).OnComplete(ScaleFormAtoB);
    }


    #endregion

    #region BehaviourControl
    private void StartComponent()
    {

    }
    private void StopComponent()
    {
        /* StopCoroutine(rotateModule());
         StopCoroutine(toFroModule());
         StopCoroutine(ScalingObject());*/
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
        _componentType = Constants.ItemComponentType.TransformComponent;
    }

    public override void CollisionExitBehaviour()
    {
        //throw new System.NotImplementedException();
    }

    public override void CollisionEnterBehaviour()
    {
        //throw new System.NotImplementedException();
    }

    public void OnPlayerEnteredRoom(Player newPlayer)
    {

    }

    public void OnPlayerLeftRoom(Player otherPlayer)
    {

    }

    public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {

    }

    public void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {

    }

    public void OnMasterClientSwitched(Player newMasterClient)
    {
        if (newMasterClient == PhotonNetwork.LocalPlayer)
        {
            if (RotateObject)
            {
                if (_timeSpent > _rotateComponentData.timeToAnimate)
                {
                    RotateFromBtoA();
                }
                else
                {
                    RotateFromAtoB();
                }
            }
            if (TransObject)
            {
                if (_timeSpent > _toFroComponentData.timeToAnimate)
                {
                    MoveFromBtoA();
                }
                else
                {
                    MoveFromAtoB();
                }

            }
            if (ScaleObject)
            {
                if (_timeSpent > _scalerComponentData.timeToAnimate)
                {
                    ScaleFormBtoA();
                }
                else
                {
                    ScaleFormAtoB();
                }
            }

        }
    }

    #endregion
}