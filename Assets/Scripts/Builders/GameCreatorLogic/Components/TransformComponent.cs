using System.Collections;
using DG.Tweening;
using Models;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class TransformComponent : ItemComponent, IInRoomCallbacks
{
    public bool rotateObject, ScaleObject, TransObject;
    float timeSpent =0;
    string ItemID;
    private void Update()
    {
        if(rotateObject)
        {
            if (PhotonNetwork.IsMasterClient) {
                 NetworkSyncManager.instance.TransformComponentrotation[ItemID] =   transform.rotation ;
                 NetworkSyncManager.instance.TransformComponentTime[ItemID] = timeSpent;
            }
            else
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation,(Quaternion) NetworkSyncManager.instance.TransformComponentrotation[ItemID], this.m_Angle * (1.0f / PhotonNetwork.SerializationRate)); ;
                timeSpent = NetworkSyncManager.instance.TransformComponentTime[ItemID]  ;
            }
        }
        if (ScaleObject)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                NetworkSyncManager.instance.TransformComponentScale[ItemID] = transform.localScale;
                NetworkSyncManager.instance.TransformComponentTime[ItemID] = timeSpent;
            }
            else
            {
                transform.localScale = (Vector3)NetworkSyncManager.instance.TransformComponentScale[ItemID];
                timeSpent = NetworkSyncManager.instance.TransformComponentTime[ItemID];
            }

        }
        if (TransObject)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                NetworkSyncManager.instance.TransformComponentPos[ItemID] = transform.position;
                NetworkSyncManager.instance.TransformComponentTime[ItemID] = timeSpent;
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, (Vector3)NetworkSyncManager.instance.TransformComponentPos[ItemID], this.m_Distance * (1.0f / PhotonNetwork.SerializationRate)); ;
                timeSpent = NetworkSyncManager.instance.TransformComponentTime[ItemID];
            }

        }

    }

    private void Start()
    {
        NetworkSyncManager.instance.OnDeserilized += Sync;
    }
    void Sync()
    { if(rotateObject)
            this.m_Angle = Quaternion.Angle(transform.rotation,(Quaternion) NetworkSyncManager.instance.TransformComponentrotation[ItemID]);
      if(TransObject)
            this.m_Distance = Vector3.Distance(transform.position, (Vector3)NetworkSyncManager.instance.TransformComponentPos[ItemID]);
    }
    public void increasTime()
    {
        timeSpent++;

        timeSpent %= toFroComponentData != null ? (toFroComponentData.timeToAnimate * 2) : rotateComponentData != null ? (rotateComponentData.timeToAnimate * 2) : (scalerComponentData.timeToAnimate * 2);//(rotateComponentData.timeToAnimate*2); 
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
    RotateComponentData rotateComponentData;

    public void InitRotate(RotateComponentData rotateComponentData,string itemid)
    {
        this.rotateComponentData = rotateComponentData;
       // StartCoroutine(rotateModule());
       ItemID = itemid;
        NetworkSyncManager.instance.TransformComponentrotation.TryAdd(itemid, transform.rotation);
        NetworkSyncManager.instance.TransformComponentTime.TryAdd(itemid, timeSpent);
        if (PhotonNetwork.IsMasterClient)
        {

            RotateFromAtoB();
        //    InvokeRepeating(nameof(increasTime), 1, 99999);
        }
        rotateObject = true;
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
      
        transform.DORotate(rotateComponentData.maxValue, rotateComponentData.timeToAnimate - (timeSpent% rotateComponentData.timeToAnimate)).SetEase(AnimationCurveValueConvertor(rotateComponentData.animationCurveIndex)).OnComplete(RotateFromBtoA);
        
    }
    private void RotateFromBtoA()
    {
        transform.DORotate(rotateComponentData.defaultValue, rotateComponentData.timeToAnimate - (timeSpent % rotateComponentData.timeToAnimate)).SetEase(AnimationCurveValueConvertor(rotateComponentData.animationCurveIndex)).OnComplete(RotateFromAtoB);
    }


    #endregion


    #region ToAndFro Module
    ToFroComponentData toFroComponentData;
    public Ease toFroEaseType;
    public void InitToFro(ToFroComponentData toFroComponentData, string itemid)
    {
        this.toFroComponentData = toFroComponentData;
        NetworkSyncManager.instance.TransformComponentPos.TryAdd(itemid, transform.position);
        NetworkSyncManager.instance.TransformComponentTime.TryAdd(itemid, timeSpent);
        if (PhotonNetwork.IsMasterClient)
        {
            MoveFromAtoB();
           // InvokeRepeating(nameof(increasTime), 1, 99999);
        }
        ItemID = itemid;
       
        TransObject = true;
        
     //   StartCoroutine(toFroModule());
    }
    private void MoveFromAtoB()//Better than loop call Functions
    {
        transform.DOMove(toFroComponentData.maxValue, toFroComponentData.timeToAnimate - (timeSpent % toFroComponentData.timeToAnimate)).SetEase(AnimationCurveValueConvertor(toFroComponentData.animationCurveIndex)).OnComplete(MoveFromBtoA) ;
    }
    private void MoveFromBtoA()
    {
        transform.DOMove(toFroComponentData.defaultValue, toFroComponentData.timeToAnimate - (timeSpent % toFroComponentData.timeToAnimate)).SetEase(AnimationCurveValueConvertor(toFroComponentData.animationCurveIndex)).OnComplete(MoveFromAtoB);
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

    ScalerComponentData scalerComponentData;
    public Ease scalerEaseType;
    private float m_Angle;
    private float m_Distance;

    public void InitScale(ScalerComponentData scalerComponentData, string itemid)
    {
        this.scalerComponentData = scalerComponentData;
        NetworkSyncManager.instance.TransformComponentTime.TryAdd(itemid, timeSpent);
        NetworkSyncManager.instance.TransformComponentScale.TryAdd(itemid, transform.localScale);
        // StartCoroutine(ScalingObject());
        if (PhotonNetwork.IsMasterClient)
        {
            ScaleFormAtoB();
          //  InvokeRepeating(nameof(increasTime), 1, 99999);
        }
        ItemID = itemid;
    
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
        transform.DOScale(scalerComponentData.maxScaleValue, scalerComponentData.timeToAnimate - (timeSpent % scalerComponentData.timeToAnimate)).SetEase(AnimationCurveValueConvertor(scalerComponentData.animationCurveIndex)).OnComplete(ScaleFormBtoA);
    } private void ScaleFormBtoA()
    {
        transform.DOScale(scalerComponentData.defaultScaleValue, scalerComponentData.timeToAnimate - (timeSpent % scalerComponentData.timeToAnimate)).SetEase(AnimationCurveValueConvertor(scalerComponentData.animationCurveIndex)).OnComplete(ScaleFormAtoB);
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
       if(newMasterClient==PhotonNetwork.LocalPlayer)
        {


        }
    }

    #endregion
}