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
        //    Debug.LogError(item.skyId+" "+item.shaderName);
        //}
    }
}

[System.Serializable]
public class SkyBoxItem
{
    public int skyId;
    public string skyName;
    public string shaderName;
    //public Material skyMaterial;
    public DirectionalLightData directionalLightData;
    public VolumeProfile ppVolumeProfile;
}

[System.Serializable]
public class DirectionalLightData
{
    public Vector3 directionLightRot = new Vector3(50, -30);
    public float lightIntensity = 1f;
    public Color directionLightColor;
    public float directionLightShadowStrength;
    public float character_directionLightIntensity;
    //public LensFlareData lensFlareData;
    public DirectionalLightData()
    {
        directionLightRot = new Vector3(50, -30);
        lightIntensity = 1f;
        directionLightColor = new Color(0.7843137f, 0.9294118f, 1.0f);
        directionLightShadowStrength = .5f;
        //lensFlareData = new LensFlareData();
    }
}

[System.Serializable]
public class LensFlareData
{
    public LensFlareDataSRP falreData;
    public float flareScale = 0.6f;
    public float flareIntensity = 1f;
}

