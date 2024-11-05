using System.Collections;
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
    //Dictionary<int, Material> skyItems;
    //Dictionary<int, Color> directionLightColors;

    [ColorUsage(true, true)]
    public Color defaultSkyboxColor;

    [SerializeField]
    internal SkyBoxItem defaultSkyBoxData;
    public SkyBoxesData skyBoxesData;
    public int index;
    public Light directionLight, characterDirectionLight;
    public Volume ppVolume;
    public LensFlareComponentSRP sceneLensFlare;

    void Start()
    {
        instance = this;
    }


    public IEnumerator DownloadSituatioChangerSkyboxes()
    {
        //Added this because of the blinking issue due to the download process when the player triggers the Situation Changer or Blind component.

        AsyncOperationHandle NoMoonSky;
        
        //bool noMoonSky = false;
        //NoMoonSky = AddressableDownloader.Instance.MemoryManager.GetReferenceIfExist("NoMoonSky", ref noMoonSky);
        //if (!noMoonSky)
            NoMoonSky = Addressables.LoadAssetAsync<Material>("NoMoonSky");
        while (!NoMoonSky.IsDone)
        {
            yield return null;
        }
        if (NoMoonSky.Status == AsyncOperationStatus.Succeeded)
            AddressableDownloader.bundleAsyncOperationHandle.Add(NoMoonSky);
            //AddressableDownloader.Instance.MemoryManager.AddToReferenceList(NoMoonSky, "NoMoonSky");
    }

    public IEnumerator DownloadBlindComponentSkyboxes()
    {
        AsyncOperationHandle darkblindSky;
        //bool darkblindSkyflag = false;
        //darkblindSky = AddressableDownloader.Instance.MemoryManager.GetReferenceIfExist("BlindSky", ref darkblindSkyflag);
        //if (!darkblindSkyflag)
            darkblindSky = Addressables.LoadAssetAsync<Material>("BlindSky");
        while (!darkblindSky.IsDone)
        {
            yield return null;
        }
        if (darkblindSky.Status == AsyncOperationStatus.Succeeded)
            AddressableDownloader.bundleAsyncOperationHandle.Add(darkblindSky);
            //AddressableDownloader.Instance.MemoryManager.AddToReferenceList(darkblindSky, "BlindSky");
    }


    //void CreateDictionaryFromScriptable()
    //{
    //    skyItems = new Dictionary<int, Material>();
    //    directionLightColors = new Dictionary<int, Color>();
    //    count = skyItems.Count;
    //    for (int i = 0; i < count; i++)
    //    {
    //        //skyItems.Add(skyBoxesData.skyBoxes[i].skyId, skyBoxesData.skyBoxes[i].skyMaterial);
    //        directionLightColors.Add(skyBoxesData.skyBoxes[i].skyId, skyBoxesData.skyBoxes[i].directionalLightData.directionLightColor);
    //    }
    //}
    int indexx = 0;
    string skyboxMatKey = "";
    public void ChangeSkyBox(int skyID)
    {
        builderMapDownload.reflectionProbe.gameObject.SetActive(false);
        builderMapDownload.reflectionProbe.enabled = true;

        if (skyID != -1)
        {
            bool skyBoxExist = skyBoxesData.skyBoxes.Exists(x => x.skyId == skyID);
            if (skyBoxExist)
            {

                indexx = skyBoxesData.skyBoxes.FindIndex(x => x.skyId == skyID);
                skyboxMatKey = skyBoxesData.skyBoxes[indexx].skyName.Replace(" ", "");
                AsyncOperationHandle loadSkyBox;
                //bool flag = false;
                //loadSkyBox = AddressableDownloader.Instance.MemoryManager.GetReferenceIfExist(skyboxMatKey, ref flag);
                //if (!flag)
                    loadSkyBox = Addressables.LoadAssetAsync<Material>(skyboxMatKey);
                //Debug.LogError("indexx == " + indexx);

                loadSkyBox.Completed += LoadSkyBox_Completed;
            }
            else
                BuilderEventManager.ApplySkyoxSettings?.Invoke();
            //RenderSettings.skybox = skyBoxesData.skyBoxes[indexx].skyMaterial;
        }
        if (skyID == -1)
        {
            Color topColor, middleColor, bottomColor;
            if (ColorUtility.TryParseHtmlString("#" + builderMapDownload.levelData.skyProperties.skyColorTop, out topColor) && ColorUtility.TryParseHtmlString("#" + builderMapDownload.levelData.skyProperties.skyColorMiddle, out middleColor) && ColorUtility.TryParseHtmlString("#" + builderMapDownload.levelData.skyProperties.skyColorBottom, out bottomColor))
                SetSkyColor(topColor, middleColor, bottomColor);
            ppVolume.profile = defaultSkyBoxData.ppVolumeProfile;
            directionLight.color = defaultSkyBoxData.directionalLightData.directionLightColor;
            DirectionLightColorChange(-1);

            //DynamicGI.UpdateEnvironment();
        }

    }
    private void LoadSkyBox_Completed(AsyncOperationHandle obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
            AddressableDownloader.bundleAsyncOperationHandle.Add(obj);
            //AddressableDownloader.Instance.MemoryManager.AddToReferenceList(obj, skyboxMatKey);
        Material _mat = obj.Result as Material;
        //Debug.LogError("indexx == " + indexx);
        _mat.shader = Shader.Find(skyBoxesData.skyBoxes[indexx].shaderName);
        RenderSettings.skybox = _mat;
        directionLight.color = skyBoxesData.skyBoxes[indexx].directionalLightData.directionLightColor;
        ppVolume.profile = skyBoxesData.skyBoxes[indexx].ppVolumeProfile;
        DirectionLightColorChange(indexx);
        //DynamicGI.UpdateEnvironment();
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
        //LensFlareData lensFlareData = new LensFlareData();
        if (skyID == -1)
        {
            directionLight.intensity = defaultSkyBoxData.directionalLightData.lightIntensity;
            directionLight.shadowStrength = defaultSkyBoxData.directionalLightData.directionLightShadowStrength;
           // lensFlareData = defaultSkyBoxData.directionalLightData.lensFlareData;
        }
        else
        {
            directionLight.intensity = skyBoxesData.skyBoxes[skyID].directionalLightData.lightIntensity;
            directionLight.shadowStrength = skyBoxesData.skyBoxes[skyID].directionalLightData.directionLightShadowStrength;
           // lensFlareData = skyBoxesData.skyBoxes[skyID].directionalLightData.lensFlareData;
        }


        //if (lensFlareData.falreData != null)
        //{
        //    sceneLensFlare.lensFlareData = lensFlareData.falreData;
        //    sceneLensFlare.scale = lensFlareData.flareScale;
        //    sceneLensFlare.intensity = lensFlareData.flareIntensity;
        //}
        //else
        //{
        //    sceneLensFlare.lensFlareData = null;
        //    sceneLensFlare.scale = 1;
        //    sceneLensFlare.intensity = 1;
        //}
        builderMapDownload.reflectionProbe.gameObject.SetActive(true);
        DynamicGI.UpdateEnvironment();
    }
}