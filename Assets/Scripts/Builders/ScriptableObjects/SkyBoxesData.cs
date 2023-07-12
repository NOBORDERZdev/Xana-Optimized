using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "ScriptableObjects/Skyboxes")]
public class SkyBoxesData : ScriptableObject
{
    public List<SkyBoxItem> skyBoxes;

    internal void Assign()
    {
        //foreach (SkyBoxItem item in skyBoxes)
        //{
        //    item.directionalLightData.lightIntensity = item.lightIntensity;
        //    item.directionalLightData.directionLightColor = item.directionLightColor;
        //    item.directionalLightData.directionLightShadowStrength = item.directionLightShadowStrength;
        //    item.directionalLightData.character_directionLightIntensity = item.character_directionLightIntensity;
        //}
    }
}

[System.Serializable]
public class SkyBoxItem
{
    public int skyId;
    public string skyName;
    //public Sprite icon;
    public Material skyMaterial;
    //public float lightIntensity = 1f;
    //public Color directionLightColor;
    //public float directionLightShadowStrength;
    //public float character_directionLightIntensity;
    public DirectionalLightData directionalLightData;
    public VolumeProfile ppVolumeProfile;
}

[System.Serializable]
public class DirectionalLightData
{
    public float lightIntensity = 1f;
    public Color directionLightColor;
    public float directionLightShadowStrength;
    public float character_directionLightIntensity;
    public LensFlareData lensFlareData;
}

[System.Serializable]
public class LensFlareData
{
    public LensFlareDataSRP falreData;
    public float flareScale = 0.6f;
}
