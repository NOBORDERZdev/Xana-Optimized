using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;

[CreateAssetMenu(menuName = "ScriptableObjects/Lightppdata")]
public class LightPPScriptableData : ScriptableObject
{
    public List<LightPPData> lightData = new List<LightPPData>();

    public LightPPData GetLightData(int id)
    {       
        foreach (LightPPData item in lightData)
        {
            if (item.uniqueID == id)
            {
                return item;
            }
        }
        return null;
    }
}
