using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ScriptableObjects/UGCItemsData")]
public class UGCItemsData : ScriptableObject
{
    public List<ItemData> faceTypes;
    public List<ItemData> lipTypes;
    public List<ItemData> noseTypes;
    public List<HairsData> hairTypes;

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
    public HairsData GetHairData(string name)
    {
        foreach (HairsData item in hairTypes)
        {
            if (item.typeName.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                return item;
            }
        }
        return null;
    }
    [System.Serializable]
    public class ItemData
    {
        public string typeName;
        public int index;
        public int value;
    }
    [System.Serializable]
    public class HairsData
    {
        public string typeName;
        public string keyValue;
    }
}
