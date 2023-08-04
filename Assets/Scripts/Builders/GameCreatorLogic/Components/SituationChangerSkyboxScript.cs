using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SituationChangerSkyboxScript : MonoBehaviour
{
    public static SituationChangerSkyboxScript instance;
    public BuilderMapDownload builderMapDownload;
    int count;
    Dictionary<int, Material> skyItems;
    Dictionary<int, Color> directionLightColors;

    [ColorUsage(true, true)]
    public Color defaultSkyboxColor;

    [SerializeField]
    internal SkyBoxItem defaultSkyBoxData;
    public SkyBoxesData skyBoxesData;
    public int index;
    public Light directionLight, characterDirectionLight;
    public Volume ppVolume;
    public LensFlareComponentSRP sceneLensFlare;
    private void Start()
    {
        instance = this;
        CreateDictionaryFromScriptable();
    }


    void CreateDictionaryFromScriptable()
    {
        skyItems = new Dictionary<int, Material>();
        directionLightColors = new Dictionary<int, Color>();
        count = skyItems.Count;
        for (int i = 0; i < count; i++)
        {
            //skyItems.Add(skyBoxesData.skyBoxes[i].skyId, skyBoxesData.skyBoxes[i].skyMaterial);
            directionLightColors.Add(skyBoxesData.skyBoxes[i].skyId, skyBoxesData.skyBoxes[i].directionalLightData.directionLightColor);
        }
    }

    public void ChangeSkyBox(int skyID)
    {
        Debug.Log("SKY BOXX" + skyID);

        int indexx = skyBoxesData.skyBoxes.FindIndex(x => x.skyId == skyID);

        if (skyID != -1)
        {
            string skyboxMatKey = skyBoxesData.skyBoxes[indexx].skyName.Replace(" ", "");
            AsyncOperationHandle<Material> loadSkyBox = Addressables.LoadAssetAsync<Material>(skyboxMatKey);
            loadSkyBox.Completed += LoadSkyBox_Completed;
            //RenderSettings.skybox = skyBoxesData.skyBoxes[indexx].skyMaterial;
            directionLight.color = skyBoxesData.skyBoxes[indexx].directionalLightData.directionLightColor;
            ppVolume.profile = skyBoxesData.skyBoxes[indexx].ppVolumeProfile;
            DirectionLightColorChange(indexx);

        }
        if (skyID == -1)
        {
            Color topColor, middleColor, bottomColor;
            if (ColorUtility.TryParseHtmlString("#" + builderMapDownload.levelData.skyProperties.skyColorTop, out topColor) && ColorUtility.TryParseHtmlString("#" + builderMapDownload.levelData.skyProperties.skyColorMiddle, out middleColor) && ColorUtility.TryParseHtmlString("#" + builderMapDownload.levelData.skyProperties.skyColorBottom, out bottomColor))
                SetSkyColor(topColor, middleColor, bottomColor);
            ppVolume.profile = defaultSkyBoxData.ppVolumeProfile;

            directionLight.intensity = 1f;
            directionLight.shadowStrength = .2f;
            characterDirectionLight.intensity = .15f;
        }
        DynamicGI.UpdateEnvironment();

    }
    private void LoadSkyBox_Completed(AsyncOperationHandle<Material> obj)
    {
        RenderSettings.skybox = obj.Result;
        DynamicGI.UpdateEnvironment();
        //throw new NotImplementedException();

    }
    void SetSkyColor(Color topColor, Color middleColor, Color bottomColor)
    {
        //cam.backgroundColor = color;
        Camera.main.clearFlags = CameraClearFlags.Skybox;
        Material mat = new Material(Shader.Find("GradientSkybox/Linear/Three Color"));
        mat.name = "Gradient Skybox";
        mat.SetColor("_TopColor", topColor);
        mat.SetColor("_MiddleColor", middleColor);
        mat.SetColor("_BottomColor", bottomColor);
        RenderSettings.skybox = mat;
        //DynamicGI.UpdateEnvironment();
    }



    void DirectionLightColorChange(int skyID)
    {


        LensFlareData lensFlareData = new LensFlareData();
        if (skyID == -1)
        {
            directionLight.intensity = 1;
            directionLight.shadowStrength = defaultSkyBoxData.directionalLightData.directionLightShadowStrength;
            lensFlareData = defaultSkyBoxData.directionalLightData.lensFlareData;
        }
        else
        {
            directionLight.intensity = skyBoxesData.skyBoxes[skyID].directionalLightData.lightIntensity;
            directionLight.shadowStrength = skyBoxesData.skyBoxes[skyID].directionalLightData.directionLightShadowStrength;
            lensFlareData = skyBoxesData.skyBoxes[skyID].directionalLightData.lensFlareData;
        }


        if (lensFlareData.falreData != null)
        {
            sceneLensFlare.lensFlareData = lensFlareData.falreData;
            sceneLensFlare.scale = lensFlareData.flareScale;
        }
        else
        {
            sceneLensFlare.lensFlareData = null;
            sceneLensFlare.scale = 1;
        }
        //DynamicGI.UpdateEnvironment();
    }
}