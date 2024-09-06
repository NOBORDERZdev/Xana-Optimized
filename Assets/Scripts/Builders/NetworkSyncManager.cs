using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NetworkSyncManager : MonoBehaviour, IPunObservable
{
    public static NetworkSyncManager Instance;
    public PhotonView SyncPhotonView;

    public Dictionary<string, object> RotatorComponent = new Dictionary<string, object>();
    public Dictionary<string, object> TransformComponentrotation = new Dictionary<string, object>();
    public Dictionary<string, object> TransformComponentScale = new Dictionary<string, object>();
    public Dictionary<string, object> TransformComponentPos = new Dictionary<string, object>();
    public Dictionary<string, float> TransformComponentTime = new Dictionary<string, float>();
    public Dictionary<string, object> TranslateComponentpos = new Dictionary<string, object>();

    public Action OnDeserilized;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        // PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        // PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(RotatorComponent);
            stream.SendNext(TransformComponentrotation);
            stream.SendNext(TransformComponentScale);
            stream.SendNext(TransformComponentPos);
            stream.SendNext(TransformComponentTime);
            stream.SendNext(TranslateComponentpos);
        }
        else
        {
            RotatorComponent = (Dictionary<string, object>)stream.ReceiveNext();
            TransformComponentrotation = (Dictionary<string, object>)stream.ReceiveNext();
            TransformComponentScale = (Dictionary<string, object>)stream.ReceiveNext();
            TransformComponentPos = (Dictionary<string, object>)stream.ReceiveNext();
            TransformComponentTime = (Dictionary<string, float>)stream.ReceiveNext();
            TranslateComponentpos = (Dictionary<string, object>)stream.ReceiveNext();

            OnDeserilized?.Invoke();
        }
    }
}