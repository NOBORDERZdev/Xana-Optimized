using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "LightingData", menuName = "ScriptableObjects/LightingSettings", order = 1)]
public class EnvironmentProperties : ScriptableObject
{
    [Header("Light Settings")]
    [Space]
    public AmbientMode AmbientMode;
    public Color32 AmbientLight;

    [Header("Settings for Tri Ambient Mode")]
    [Space]
    public Color32 AmbientSkyColor ;
    public Color32 AmbientEquatorColor ;
    public Color32 AmbientGroundColor ;

    [Header("SkyBox for Environment")]
    [Space]
    public Material SkyBoxMaterial;
    

    public void ApplyLightSettings()
    {
        if (SkyBoxMaterial)
        {
            RenderSettings.skybox = SkyBoxMaterial;
        }
        //else
        //{
        //    Debug.LogWarning("Environment SkyBox Not Found using default skybox");
        //    RenderSettings.skybox = new Material(Shader.Find("Standard"));

        //}

        RenderSettings.ambientMode = AmbientMode;
        if (AmbientMode == AmbientMode.Trilight)
        {
            RenderSettings.ambientSkyColor = AmbientSkyColor;
            RenderSettings.ambientEquatorColor =  AmbientEquatorColor;
            RenderSettings.ambientGroundColor = AmbientGroundColor;
        }
        else
        {
            if (AmbientLight == Color.black)
            {
                AmbientLight = new Color32(224, 224, 224, 0);
            }
            RenderSettings.ambientLight = AmbientLight;
        }
    }

}
