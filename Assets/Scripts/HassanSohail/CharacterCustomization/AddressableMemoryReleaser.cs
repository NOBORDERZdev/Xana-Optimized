using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
public class AddressableMemoryReleaser : MonoBehaviour
{
    [SerializeField]
    List<MemoryObject> memoryObjects = new List<MemoryObject>();
    public void AddToReferenceList(AsyncOperationHandle Obj)
    {
        MemoryObject item = new(Obj);
        memoryObjects.Add(item);
    }
    public void RemoveAllAddressables()
    {
        foreach (MemoryObject objj in memoryObjects)
            Addressables.ReleaseInstance(objj.Obj);
        memoryObjects.Clear();
        GC.Collect();
    }
}
[Serializable]
public class MemoryObject
{
    public AsyncOperationHandle Obj;
    public MemoryObject (AsyncOperationHandle handler)
    {
        this.Obj = handler;
    }
}