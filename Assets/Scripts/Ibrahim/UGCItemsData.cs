using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ScriptableObjects/UGCItemsData")]
public class UGCItemsData : ScriptableObject
{
    [System.Serializable]
    public class ItemData
    {
        public string typeName;
        public int index;
        public int value;
    }
    public List<ItemData> faceTypes;
    public List<ItemData> lipTypes;
    public List<ItemData> noseTypes;

    public ItemData GetFaceData(string name)
    {
        foreach (ItemData item in faceTypes)
        {
            if (item.typeName.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                return item;
            }
        }
        return null;
    }
    public ItemData GetNoseData(string name)
    {
        foreach (ItemData item in noseTypes)
        {
            if (item.typeName.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                return item;
            }
        }
        return null;
    }
    public ItemData GetlipData(string name)
    {
        foreach (ItemData item in lipTypes)
        {
            if (item.typeName.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                return item;
            }
        }
        return null;
    }
}
