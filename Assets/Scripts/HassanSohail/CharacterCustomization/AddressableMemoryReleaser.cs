using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Nethereum.Signer.Crypto;
using UnityEngine.InputSystem;

public class AddressableMemoryReleaser : MonoBehaviour
{
    [SerializeField]
    List<MemoryObject> memoryObjects = new List<MemoryObject>();
    public void AddToReferenceList(AsyncOperationHandle Obj,string refKey)
    {
        foreach(MemoryObject _mObj in memoryObjects)
        {
            if (_mObj.Key.Equals(refKey))
            {
                return;
            }
        }
        MemoryObject item = new(refKey, Obj);
        memoryObjects.Add(item);
        /*if (memoryObjects.Count > 0)
        {
            if (memoryObjects.First(x => x.Key.Equals(refKey)) == null)
            {
                MemoryObject item = new(refKey, Obj);
                memoryObjects.Add(item);
            }
        }
        else
        {
            MemoryObject item = new(refKey, Obj);
            memoryObjects.Add(item);
        }*/
    }
    public AsyncOperationHandle GetReferenceIfExist(string refKey,ref bool flag)
    {
        foreach (MemoryObject _mObj in memoryObjects)
        {
            if (_mObj.Key.Equals(refKey))
            {
                flag = true;
                return _mObj.HandlerObj;
            }
        }
        /*
        if (memoryObjects.Count > 0)
        {
            Debug.LogError(memoryObjects.Count);
            MemoryObject _mObj = memoryObjects.First(x => x.Key.Equals( refKey));
            if (_mObj != null)
            {
               // Obj = _mObj.HandlerObj;
               flag = true;
                return _mObj.HandlerObj;
            }
        }*/
        return default;
    }
    public void RemoveAllAddressables()
    {
        foreach (MemoryObject objj in memoryObjects)
        {
            if(objj.HandlerObj.IsValid())
            {
                Addressables.ReleaseInstance(objj.HandlerObj);
            }
        }
        memoryObjects.Clear();
        GC.Collect();
        AssetBundle.UnloadAllAssetBundles(false);
        Resources.UnloadUnusedAssets();
    }
    public void RemoveAddressable(string key)         // Added by Ali Hamza to release specific object based on key
    {
        for (int i = memoryObjects.Count-1; i >=0 ; i--)
        {
            if (memoryObjects[i].Key.Equals(key))
            {
                if (memoryObjects[i].HandlerObj.IsValid())
                {
                    Addressables.ReleaseInstance(memoryObjects[i].HandlerObj);
                }
                memoryObjects.Remove(memoryObjects[i]);
                break;
            }
        }
    }
}
[Serializable]
public class MemoryObject
{
    public string Key;
    public AsyncOperationHandle HandlerObj;
    public MemoryObject (string key,AsyncOperationHandle handler)
    {
        this.Key = key;
        this.HandlerObj = handler;
    }
}