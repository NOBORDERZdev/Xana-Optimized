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
    public List<ItemData> eyeShapeTypes;

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
    public ItemData GetEyeShapeData(string name)
    {
        foreach (ItemData item in eyeShapeTypes)
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
    public Color hair_color;
    public string skin_color;
    public Color lips_color;
    public string gender;
    public string _hairItemData;
    public string _eyeItemData;
    public int faceItemData;
    public int lipItemData;
    public int noseItemData;
    public int eyeShapeItemData;
    public bool CharactertypeAi;

    public Texture default_male_face_color, default_male_body_color, default_female_face_color, default_female_body_color;
    public Color default_male_lips_color, default_female_lips_color;
}