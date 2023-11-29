using System;
using Photon.Pun;
using UnityEngine;

public class RPCCalls : MonoBehaviourPun
{
    public static RPCCalls Instance;
    

    private static Action _callback;
    private static Action<string, int> _callbackStrInt;
    // private Dictionary<string, Action> _dict = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // public void SetRPC(Action callback)
    // {
    //     _dict.Add(nameof(callback), callback);
    //     // _callback = callback;
    // }
    
    public void CallRPC (Action callback, RpcTarget target)
    {
        _callback = callback;
        Debug.LogError("RFM CallRPC() " + _callback);

        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(GenericRPC), target);
        }
        
        // if (_dict.ContainsKey(methodName))
        // {
        //     photonView.RPC(nameof(GenericRPC), target, methodName);
        // }
    }
    
    public void CallRPC (Action<string, int> callback, RpcTarget target, string strParam, int intParam)
    {
        _callbackStrInt = callback;
        Debug.LogError("RFM CallRPC() " + _callbackStrInt + 
                       " with string parameter: " + strParam + " and int parameter: " + intParam);

        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(GenericRPCStringInt), target, strParam, intParam);
        }
        
        // if (_dict.ContainsKey(methodName))
        // {
        //     photonView.RPC(nameof(GenericRPC), target, methodName);
        // }
    }
    
    [PunRPC]
    private void GenericRPC(/*string methodName*/)
    {
        Debug.LogError("RFM GenericRPC()" + _callback);
        // _dict.TryGetValue(methodName, out var callback);
        _callback?.Invoke();
    }
    
    [PunRPC]
    private void GenericRPCStringInt (string stringParam, int intParam)
    {
        Debug.LogError("RFM GenericRPCStringInt() " + _callbackStrInt + 
                       " with string parameter: " + stringParam + " and int parameter: " + intParam);
        _callbackStrInt?.Invoke(stringParam, intParam);
    }
}
