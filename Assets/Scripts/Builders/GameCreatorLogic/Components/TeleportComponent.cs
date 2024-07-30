using System.Collections;
using Models;
using Photon.Pun;
using UnityEngine;

public class TeleportComponent : ItemComponent
{
    private TeleportComponentData _teleportComponentData;

    //private const int turnOffTime = 5;
    private Coroutine _timer;
    PhotonView _pvIsMine;


    bool _collideWithComponent;
    public void Init(TeleportComponentData teleportComponentData)
    {
        this._teleportComponentData = teleportComponentData;
    }

    //IEnumerator TimerCountDown()
    //{
    //    yield return new WaitForSeconds(turnOffTime);
    //    StopBehaviour();
    //}

    private void OnCollisionEnter(Collision _other)
    {
        if (_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            //BuilderEventManager.LoadSceneByName?.Invoke(_teleportComponentData.spaceID, new Vector3(0,0,0));
            StartComponent();
        }
    }


    #region BehaviourControl
    private void StartComponent()
    {
        if (_collideWithComponent)
            return;
        _collideWithComponent = true;
        Invoke(nameof(CollideWithComponet), 0.5f);
        BuilderEventManager.onComponentActivated?.Invoke(_componentType);
        //EventManager.ComponentActivated(_componentType);

        //CanvasComponenetsManager._instance.displayMessageTitle.text = Constants.TeleportMessage(teleportComponentData.spaceID);
        //CanvasComponenetsManager._instance.displayMessageParentReference.SetActive(true);

        //if(timer != null) StopCoroutine(timer);
        //timer = StartCoroutine(TimerCountDown());

        Debug.Log("Teleport Component : " + _teleportComponentData.spaceID);
    }

    void CollideWithComponet()
    {
        _collideWithComponent = false;
    }

    private void StopComponent()
    {
        //CanvasComponenetsManager._instance.displayMessageParentReference.SetActive(false);
        //CanvasComponenetsManager._instance.displayMessageTitle.text = "";
        //if (timer != null) StopCoroutine(timer);
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
    public override void CollisionExitBehaviour()
    {
        StopBehaviour();
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
        _componentType = Constants.ItemComponentType.TeleportComponent;
    }

    public override void CollisionEnterBehaviour()
    {
        //throw new System.NotImplementedException();
    }

    #endregion
}