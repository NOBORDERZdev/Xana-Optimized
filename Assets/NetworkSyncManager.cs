using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NetworkSyncManager : MonoBehaviour, IPunObservable
{
   
    public static NetworkSyncManager instance;
    public PhotonView view;

    public Dictionary<string,object> rotatorComponent = new Dictionary<string, object>();
    public Dictionary<string, object> TransformComponentrotation = new Dictionary<string, object>();
    public Dictionary<string, object> TransformComponentScale = new Dictionary<string, object>();
    public Dictionary<string, object> TransformComponentPos = new Dictionary<string, object>();
    public Dictionary<string, int> TransformComponentTime = new Dictionary<string, int>();
    public Dictionary<string, object> TranslateComponentpos = new Dictionary<string, object>();
    

    public Action<string,int, int, int> OnRandomNumberSet;
    public List<RandomNumberComponentsData> RandomNumberHist = new List<RandomNumberComponentsData>();
    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
        
    [PunRPC]
    public void SetRandomNumberComponent(string itemID, int minNumber, int maxnumber, int generatedNumber)
    {
        OnRandomNumberSet?.Invoke(itemID, minNumber, maxnumber, generatedNumber);
        RandomNumberHist.Add(new RandomNumberComponentsData() { GeneratedNumber = generatedNumber , ItemID = itemID,MinNumber = minNumber,MaxNumber = maxnumber });
    }




    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
           stream.SendNext( rotatorComponent);  stream.SendNext(TransformComponentrotation);  stream.SendNext(TransformComponentScale);  stream.SendNext(TransformComponentPos);  stream.SendNext(TransformComponentTime);  stream.SendNext(TranslateComponentpos);
        
        }else if(!PhotonNetwork.IsMasterClient)
        {
         
            rotatorComponent = (Dictionary<string,object>)stream.ReceiveNext();
            TransformComponentrotation = (Dictionary<string, object>)stream.ReceiveNext();
            TransformComponentScale = (Dictionary<string, object>)stream.ReceiveNext();
            TransformComponentPos = (Dictionary<string, object>)stream.ReceiveNext();
            TransformComponentTime = (Dictionary<string, int>)stream.ReceiveNext();
            TranslateComponentpos = (Dictionary<string, object>)stream.ReceiveNext();
            /*    rotatorComponent =  JsonUtility.FromJson < Dictionary<string, Vector3>>((string)stream.ReceiveNext());
                var dat = (string)stream.ReceiveNext();
                Debug.LogError("DataReceived" + dat);
                TransformComponentrotation = JsonUtility.FromJson<Dictionary<string, Quaternion>>(dat);
                TransformComponentScale = JsonUtility.FromJson<Dictionary<string, Vector3>>((string)stream.ReceiveNext());
                TransformComponentPos = JsonUtility.FromJson<Dictionary<string, Vector3>>((string)stream.ReceiveNext());
                TransformComponentTime = JsonUtility.FromJson<Dictionary<string, int>>((string)stream.ReceiveNext());
                TranslateComponentpos = JsonUtility.FromJson<Dictionary<string, Vector3>>((string)stream.ReceiveNext());*/

        }
    }



}

public class RandomNumberComponentsData
{
    public string ItemID;
    public int MinNumber;
    public int MaxNumber;
    public int GeneratedNumber;

}
