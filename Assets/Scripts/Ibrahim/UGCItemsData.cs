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
    public List<HairsEyeData> hairTypes;
    public List<HairsEyeData> eyeColor;

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
    public HairsEyeData GetHairData(string name)
    {
        foreach (HairsEyeData item in hairTypes)
        {
            if (item.typeName.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                return item;
            }
        }
        return null;
    }
    public HairsEyeData GetEyeData(string name)
    {
        foreach (HairsEyeData item in eyeColor)
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
    public class HairsEyeData
    {
        public string typeName;
        public string keyValue;
    }
}
[Serializable]
public class UGCItemData
{
    public string hair_color;
    public string skin_color;
    public string lips_color;
    public string gender;
    public string _hairItemData;
    public string _eyeItemData;
    public int faceItemData;
    public int lipItemData;
    public int noseItemData;
    public ControllerType type;
    //public UGCItemData()
    //{
    //    faceItemData = new DataContain();
    //    lipItemData = new DataContain();
    //    noseItemData = new DataContain();
    //    _hairItemData = new HairEyeData();
    //    _eyeItemData = new HairEyeData();
    //}
}
//[Serializable]
//public class DataContain
//{
//    public string typeName;
//    public int index;
//    public int value;
//}
//[Serializable]
//public class HairEyeData
//{
//    public string typeName;
//    public string keyValue;
//}