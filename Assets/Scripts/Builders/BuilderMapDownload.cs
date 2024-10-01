using MD_Plugin;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class BuilderMapDownload : MonoBehaviour
{

    public const string prefabPrefix = "pf";
    private float progressPlusValue = 100;
    public Transform builderAssetsParent;
    public GameObject terrainPlane;
    public MeshRenderer realisticPlanRenderer;
    public GameObject waterPlane;

    public SkyBoxesData skyBoxData;
    public RealisticTerrainMaterials realisticTerrainMaterials;
    public LightPPScriptableData lightAndPPData;
    public Color skyBoxColor;
    public Light directionalLight;
    public Light characterLight;

    public Volume postProcessVol;
    public VolumeProfile defaultPostProcessVolProfile;
    public MeshCombiner meshCombiner;

    //Reflection Probe
    public ReflectionProbe reflectionProbe;
    private byte[] deformationData;

    #region PRIVATE_VAR
    private ServerData serverData;
    internal LevelData levelData;
    private AISkyboxItem aiSkyboxItem;
    #endregion

    #region XANA PARTY WORLD
    [Header("Dynamic Object Prefab")]
    public GameObject MultiplayerComponent;

    public GameObject XANAPartyLoading;
    #endregion
    //internal string response;

    #region UNITY_METHOD
    private void OnEnable()
    {
        BuilderEventManager.OnBuilderDataFetch += OnBuilderDataFetch;
        BuilderEventManager.ApplySkyoxSettings += SetSkyProperties;
        BuilderEventManager.AfterPlayerInstantiated += SetPlayerProperties;
        BuilderEventManager.AfterWorldInstantiated += XanaSetItemData;

        if (ConstantsHolder.XanaPartyMaxPlayers == 1)
            GamificationComponentData.instance.SinglePlayer = true;
    }

    private void OnDisable()
    {
        BuilderEventManager.OnBuilderDataFetch -= OnBuilderDataFetch;
        BuilderEventManager.ApplySkyoxSettings -= SetSkyProperties;
        BuilderEventManager.AfterPlayerInstantiated -= SetPlayerProperties;
        BuilderEventManager.AfterWorldInstantiated -= XanaSetItemData;
        BuilderData.spawnPoint.Clear();
        BuilderData.StartFinishPoints.Clear();

        if (GamificationComponentData.instance.aiSkyMaterial != null)
            Destroy(GamificationComponentData.instance.aiSkyMaterial.mainTexture); // AR changes
        RenderSettings.skybox = null;
    }

    private void Start()
    {
        BuilderEventManager.OnBuilderDataFetch?.Invoke(ConstantsHolder.xanaConstants.builderMapID, ConstantsGod.AUTH_TOKEN);
        GamificationComponentData.instance.isSkyLoaded = false;

        if(ConstantsHolder.xanaConstants.isXanaPartyWorld)
        {
            XANAPartyLoading.SetActive(true);
        }
    }


    #endregion

    #region PRIVATE_METHODS

    void OnBuilderDataFetch(int id, string token)
    {
        StartCoroutine(FetchUserMapFromServer(id, token));
    }

    //the api is set we just have to get the map
    IEnumerator FetchUserMapFromServer(int _mapId, string userToken)
    {
        string _url = ConstantsGod.API_BASEURL + ConstantsGod.BUILDERGETSINGLEWORLDBYID + _mapId;
        ////Debug.LogError("URL ==> " + _url);
        using (UnityWebRequest www = UnityWebRequest.Get(_url))
        {
            ////Debug.LogError("UserToken ==> " + userToken);
            www.SetRequestHeader("Authorization", userToken);
            www.SendWebRequest();
            while (!www.isDone)
            {
                yield return null;
            }
            if ((www.result == UnityWebRequest.Result.ConnectionError) || (www.result == UnityWebRequest.Result.ProtocolError))
            {
                //response = www.downloadHandler.text;
            }
            else
            {
                //response = www.downloadHandler.text;
                serverData = JsonUtility.FromJson<ServerData>(www.downloadHandler.text);
                BuilderData.mapData = serverData;
                StartCoroutine(PopulateLevel());
            }
        }
    }

    IEnumerator DownloadLevelDataJson(string _url, Action<string> OnSuccess, Action<string> OnError)
    {
        string path = Application.persistentDataPath + "/_LevelData.nbz";
        WebClient client = new WebClient();
        client.DownloadFile(_url, path);
        using (FileStream fs = new FileStream(path, FileMode.Open))
        using (ZipArchive zip = new ZipArchive(fs))
        {
            var entry = zip.Entries.First();
            using (StreamReader sr = new StreamReader(entry.Open()))
            {
                try
                {
                    string compressData = sr.ReadToEnd();
                    string deCompressData = DecompressString(compressData);
                    OnSuccess.Invoke(deCompressData);
                    // response = deCompressData;
                }
                catch (Exception e)
                {
                }

            }
        }
        client.Dispose();
        File.Delete(path);
        yield return null;
    }

    IEnumerator PopulateLevel()
    {
        if (string.IsNullOrEmpty(serverData.data.map_json_link))
        {
            levelData = serverData.data.json;
        }
        else
        {
            yield return StartCoroutine(DownloadLevelDataJson(serverData.data.map_json_link, (sucess) =>
            {
                levelData = GetDecompressJson(sucess);
                BuilderData.mapData.data.json = levelData;
            },
            (onfalse) =>
            {
                //Debug.Log("Failed to load json....");
            }));
        }

        if (BuilderAssetDownloader.isPostLoading)
        {
            BuilderEventManager.AfterMapDataDownloaded?.Invoke();
        }
        else
        {
            StartCoroutine(DownloadAssetsData(() =>
            {
                LoadAddressableSceneAfterDownload();
            }));
        }

        if (!ConstantsHolder.xanaConstants.isXanaPartyWorld)
        {
            XANAPartyLoading.SetActive(false);
        }

        GamificationComponentData.instance.previousSkyID = levelData.skyProperties.skyId;
        if (levelData.skyProperties.skyId != -1)
        {
            bool skyBoxExist = skyBoxData.skyBoxes.Exists(x => x.skyId == levelData.skyProperties.skyId);
            if (!skyBoxExist)
            {
                aiSkyboxItem = levelData.skyProperties.aISkyboxItem;
                yield return StartCoroutine(AISkyTextureDownload());
            }
        }

        if (!string.IsNullOrEmpty(levelData.terrainProperties.meshDeformationPath))
            yield return StartCoroutine(LoadMeshDeformationFile(levelData.terrainProperties.meshDeformationPath, GetTerrainDeformation));
        if (!string.IsNullOrEmpty(levelData.terrainProperties.texturePath))
            yield return StartCoroutine(SetTerrainTexture(levelData.terrainProperties.texturePath));
        if (!string.IsNullOrEmpty(levelData.terrainProperties.waterTexturePath))
            yield return StartCoroutine(SetWaterTexture(levelData.terrainProperties.waterTexturePath));

        SetPlaneScaleAndPosition(levelData.terrainProperties.planeScale, levelData.terrainProperties.planePos);

        if (levelData.uploadProperties != null)
            BuilderEventManager.UploadPropertiesData?.Invoke(levelData.uploadProperties);

        if (levelData.audioPropertiesBGM != null)
            BuilderEventManager.BGMDownloader?.Invoke(levelData.audioPropertiesBGM);

        if (serverData.data.worldType == 1)
        {
            GamificationComponentData.instance.withMultiplayer = true;
            ConstantsHolder.xanaConstants.isXanaPartyWorld = true;
        }

        if (GamificationComponentData.instance.withMultiplayer && levelData.otherItems.Count > 0)
        {
            yield return StartCoroutine(DownloadAddressableGamificationObject());
            yield return StartCoroutine(GemificationObjectLoadWait(1f));
        }

        //Debug.LogError("Map is downloaed");

    }

    public IEnumerator GemificationObjectLoadWait(float waitTime)
    {
        DefaultPool pool = PhotonNetwork.PrefabPool as DefaultPool;
        //Debug.Log("GemificationObject assets count====" + GamificationComponentData.instance.multiplayerComponentsObject.Count);
        yield return new WaitForSeconds(waitTime);
        if (pool != null && GamificationComponentData.instance.multiplayerComponentsObject != null)
        {
            foreach (GameObject prefab in GamificationComponentData.instance.multiplayerComponentsObject)
            {
                //If a key already exists in the Pool Resource Cache dictionary, then remove it due to assigning a new game object because it gets null ref when the user exits and re-enters the any builder world.
                if (pool.ResourceCache.ContainsKey(prefab.name))
                    pool.ResourceCache.Remove(prefab.name);
                pool.ResourceCache.Add(prefab.name, prefab);
            }
        }
    }

    public IEnumerator DownloadAddressableGamificationObject()
    {
        GamificationComponentData.instance.multiplayerComponentsObject.Clear();
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            foreach (string key in GamificationComponentData.instance.multiplayerComponentsName)
            {
                AsyncOperationHandle loadOp;
                //bool flag = false;
                //AsyncOperationHandle loadOp = AddressableDownloader.Instance.MemoryManager.GetReferenceIfExist(key, ref flag);
                //if (!flag)
                //{
                if (key != "Hiragino-Sans")
                    loadOp = Addressables.LoadAssetAsync<GameObject>(key);
                else
                    loadOp = Addressables.LoadAssetAsync<TMPro.TMP_FontAsset>(key);
                //}
                while (!loadOp.IsDone)
                    yield return null;
                if (loadOp.Status == AsyncOperationStatus.Failed)
                {
                }
                else if (loadOp.Status == AsyncOperationStatus.Succeeded)
                {
                    AddressableDownloader.bundleAsyncOperationHandle.Add(loadOp);
                    //Debug.Log("Gamification Loaded" + loadOp.Result);
                    if (key != "Hiragino-Sans")
                    {
                        GameObject assets = loadOp.Result as GameObject;
                        GamificationComponentData.instance.multiplayerComponentsObject.Add(assets);
                        if (key == "SpecialItemParticleFlame")
                            GamificationComponentData.instance.specialItemParticleEffect = assets;
                        //Debug.Log("GemificationObject=== " + assets.name);
                    }
                    else
                        GamificationComponentData.instance.hiraginoFont = loadOp.Result as TMPro.TMP_FontAsset;

                    //AddressableDownloader.Instance.MemoryManager.AddToReferenceList(loadOp, key);
                }
            }
        }
    }


    public IEnumerator DownloadAssetsData(Action CallBack)
    {
        GamificationComponentData.instance.xanaItems.Clear();
        GamificationComponentData.instance.MultiplayerComponentstoSet.Clear();
        int count = levelData.otherItems.Count;
        progressPlusValue = 0.6f / count;
        LoadingHandler.Instance.UpdateLoadingStatusText("Downloading Assets...");

        for (int i = 0; i < count; i++)
        {

            string key = prefabPrefix + levelData.otherItems[i].ItemID + "_XANA";
            bool flag = false;

            AsyncOperationHandle _async = AddressableDownloader.Instance.MemoryManager.GetReferenceIfExist(key, ref flag);
            if (!flag)
                _async = Addressables.LoadAssetAsync<GameObject>(key);


            while (!_async.IsDone)
            {
                yield return null;
            }
            if (_async.Status == AsyncOperationStatus.Succeeded)
            {
                GetObject(_async, levelData.otherItems[i]);
                AddressableDownloader.Instance.MemoryManager.AddToReferenceList(_async, prefabPrefix + levelData.otherItems[i].ItemID + "_XANA");
            }
            //if (ConstantsHolder.xanaConstants.isFromXanaLobby)
            //{
            //    LoadingHandler.Instance.UpdateLoadingSliderForJJ(i * progressPlusValue + .2f, .1f);
            //}
            //else
            //    LoadingHandler.Instance.UpdateLoadingSlider(i * progressPlusValue + .2f);

            // Addressables.Release(_async);
        }
        CallBack();
    }

    //Set Hierarchy same as builder
    private void SetObjectHirarchy()
    {
        foreach (XanaItem xanaItem in GamificationComponentData.instance.xanaItems)
        {
            if (!xanaItem.itemData.ParentID.Equals(""))
            {
                string parentId = xanaItem.itemData.ParentID;
                XanaItem parentItem = GamificationComponentData.instance.xanaItems.Find(x => x.itemData.RuntimeItemID == parentId);
                if (parentItem != null)
                {
                    xanaItem.transform.SetParent(parentItem.transform);
                }
            }
        }
    }

    public string DecompressString(string compressedText)
    {
        byte[] gZipBuffer = Convert.FromBase64String(compressedText);
        using (var memoryStream = new MemoryStream())
        {
            int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
            memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);
            var buffer = new byte[dataLength];
            memoryStream.Position = 0;
            using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
            {
                gZipStream.Read(buffer, 0, buffer.Length);
            }

            // response = Encoding.UTF8.GetString(buffer);
            return Encoding.UTF8.GetString(buffer);
        }
    }

    LevelData GetDecompressJson(string LevelDataJson)
    {
        LevelData levelData = new LevelData();
        levelData = JsonUtility.FromJson<LevelData>(LevelDataJson);
        return levelData;
    }

    public IEnumerator LoadMeshDeformationFile(string path, Action<byte[]> callback)
    {
        UnityWebRequest www = UnityWebRequest.Get(path);
        www.SendWebRequest();
        while (!www.isDone)
        {
            yield return null;
        }


        if (www.result != UnityWebRequest.Result.Success)
        {
            //Debug.Log(www.error);
        }
        else
        {
            byte[] results = www.downloadHandler.data;
            deformationData = results;
            callback?.Invoke(results);
        }

        www.Dispose();
    }

    public void GetTerrainDeformation(byte[] meshDeformation)
    {
        var deformedMeshData = System.Text.Encoding.UTF8.GetString(meshDeformation);
        if (deformedMeshData.Length <= 10) return;
        terrainPlane.GetComponent<MeshFilter>().mesh.vertices = DeserializeVector3Array(deformedMeshData);
        terrainPlane.GetComponent<MD_MeshColliderRefresher>().MeshCollider_UpdateMeshCollider();
    }
    public static Vector3[] DeserializeVector3Array(string vertexData)
    {
        string[] vectors = vertexData.Split('|');
        Vector3[] result = new Vector3[vectors.Length];
        for (int i = 0; i < vectors.Length; i++)
        {
            string[] values = vectors[i].Split(' ');
            result[i] = new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
        }
        return result;
    }



    IEnumerator SetTerrainTexture(string textureUrl)
    {
        MeshRenderer meshRenderer = terrainPlane.GetComponent<MeshRenderer>();

        if (meshRenderer != null)
        {
            //Debug.Log(textureUrl);
            yield return StartCoroutine(GetTexture(textureUrl, (Texture tex) =>
            {
                meshRenderer.material.SetTexture("_MainTex", tex);
            }));
        }

        if (levelData.terrainProperties.realisticMatIndex != -1)
        {
            yield return StartCoroutine(SetRealisticTerrain(meshRenderer));
        }
    }

    IEnumerator SetRealisticTerrain(MeshRenderer meshRenderer)
    {
        bool realisticTerrainExist = realisticTerrainMaterials.terrainMaterials.Exists(x => x.id == levelData.terrainProperties.realisticMatIndex);
        if (realisticTerrainExist)
        {
            AsyncOperationHandle loadRealisticMaterial;
            RealisticMaterialData realisticMaterialData = realisticTerrainMaterials.terrainMaterials.Find(x => x.id == levelData.terrainProperties.realisticMatIndex);
            string loadRealisticMatKey = realisticMaterialData.name.Replace(" ", "");

            bool flag = false;
            loadRealisticMaterial = AddressableDownloader.Instance.MemoryManager.GetReferenceIfExist(loadRealisticMatKey, ref flag);

            if (!flag)
            {
                loadRealisticMaterial = Addressables.LoadAssetAsync<Material>(loadRealisticMatKey);
            }

            // Wait for the load operation to complete
            while (!loadRealisticMaterial.IsDone)
            {
                yield return null;
            }

            if (loadRealisticMaterial.Status == AsyncOperationStatus.None || loadRealisticMaterial.Status == AsyncOperationStatus.Failed || loadRealisticMaterial.Result == null)
            {
                // Log the failure
                Debug.LogError("Loading material failed or returned null. Retrying...");

                // Release the handle if the material is null or failed
                if (loadRealisticMaterial.IsValid())
                {
                    Addressables.Release(loadRealisticMaterial);
                }

                // Retry loading the material again
                loadRealisticMaterial = Addressables.LoadAssetAsync<Material>(loadRealisticMatKey);

                while (!loadRealisticMaterial.IsDone)
                {
                    yield return null;
                }
            }

            if (loadRealisticMaterial.Status == AsyncOperationStatus.Succeeded && loadRealisticMaterial.Result != null)
            {
                // Add the loaded material to the reference list
                AddressableDownloader.Instance.MemoryManager.AddToReferenceList(loadRealisticMaterial, loadRealisticMatKey);

                Material _mat = loadRealisticMaterial.Result as Material;
                _mat.shader = Shader.Find(realisticMaterialData.shaderName);

                meshRenderer.enabled = false;
                realisticPlanRenderer.material = _mat;

                if (deformationData.Length > 0)
                {
                    var deformedMeshData = Encoding.UTF8.GetString(deformationData);
                    if (deformedMeshData.Length >= 10)
                    {
                        realisticPlanRenderer.GetComponent<MeshFilter>().mesh.vertices = DeserializeVector3Array(deformedMeshData);
                    }
                }

                realisticPlanRenderer.gameObject.SetActive(true);
            }
            else
            {
                // Log if loading failed after retry
                Debug.LogError("Failed to load the material even after retry.");
            }
        }
    }


    IEnumerator SetWaterTexture(string textureUrl)
    {
        MeshRenderer meshRenderer = waterPlane.GetComponent<MeshRenderer>();

        if (meshRenderer != null)
        {
            yield return StartCoroutine(GetTexture(textureUrl, (Texture tex) =>
            {
                meshRenderer.material.SetTexture("_MainTex", tex);
            }));
        }
    }

    void SetPlaneScaleAndPosition(Vector3 scale, Vector3 pos)
    {
        ////Debug.Log(scale + "  " + pos);
        terrainPlane.transform.localScale = scale;
        terrainPlane.transform.position = pos + new Vector3(0, -0.001f, 0);
    }

    void SetSkyProperties()
    {
        StartCoroutine(SetSkyPropertiesDelay());
    }
    Bloom bloom;
    WhiteBalance whiteBalance;
    ColorAdjustments colorAdjustments;
    IEnumerator SetSkyPropertiesDelay()
    {
        reflectionProbe.gameObject.SetActive(false);
        reflectionProbe.enabled = true;
        SkyProperties skyProperties = levelData.skyProperties;
        LensFlareData lensFlareData = new LensFlareData();
        Camera.main.clearFlags = CameraClearFlags.Skybox;
        if (skyProperties.skyId != -1)
        {

            bool skyBoxExist = skyBoxData.skyBoxes.Exists(x => x.skyId == skyProperties.skyId);
            if (skyBoxExist)
            {
                AsyncOperationHandle loadSkyBox;

                SkyBoxItem skyBoxItem = skyBoxData.skyBoxes.Find(x => x.skyId == skyProperties.skyId);
                string skyboxMatKey = skyBoxItem.skyName.Replace(" ", "");
                //bool flag = false;
                //loadSkyBox = AddressableDownloader.Instance.MemoryManager.GetReferenceIfExist(skyboxMatKey, ref flag);
                //if (!flag)
                loadSkyBox = Addressables.LoadAssetAsync<Material>(skyboxMatKey);
                while (!loadSkyBox.IsDone)
                {
                    yield return null;
                }
                if (loadSkyBox.Status == AsyncOperationStatus.None)
                {
                    ////Debug.LogError(" ---------- NONE ------------ SKY BOXX");
                }
                else if (loadSkyBox.Status == AsyncOperationStatus.Failed)
                {
                    ////Debug.LogError(" ----------- FAILED ----------- SKY BOXX");
                }
                else if (loadSkyBox.Status == AsyncOperationStatus.Succeeded)
                {
                    // //Debug.LogError(" ---------- Success ------------ SKY BOXX");
                    //AddressableDownloader.Instance.MemoryManager.AddToReferenceList(loadSkyBox, skyboxMatKey);
                    AddressableDownloader.bundleAsyncOperationHandle.Add(loadSkyBox);
                    Material _mat = loadSkyBox.Result as Material;
                    _mat.shader = Shader.Find(skyBoxItem.shaderName);
                    if (skyProperties.skyId == 32 && _mat.GetTexture("_Tex") == null)
                    {
                        //Set texture when getting null from addressable
                        _mat.SetTexture("_Tex", GamificationComponentData.instance.defaultSkyTex);
                    }
                    RenderSettings.skybox = _mat;
                    directionalLight.transform.rotation = Quaternion.Euler(skyBoxItem.directionalLightData.directionLightRot);
                    directionalLight.intensity = skyBoxItem.directionalLightData.lightIntensity;
                    characterLight.intensity = skyBoxItem.directionalLightData.character_directionLightIntensity;
                    directionalLight.shadowStrength = skyBoxItem.directionalLightData.directionLightShadowStrength;
                    directionalLight.color = skyBoxItem.directionalLightData.directionLightColor;
                    SetPostProcessProperties(skyBoxItem.ppVolumeProfile);
                    lensFlareData = skyBoxItem.directionalLightData.lensFlareData;
                }
            }
            else
            {
                AISkyboxItem skyBoxItem = aiSkyboxItem;

                ////Debug.LogError(JsonUtility.ToJson(skyBoxItem));
                if (skyBoxItem.texture == null)
                {
                    //Remove texture downloading code
                    yield return StartCoroutine(AISkyTextureDownload());
                }
                GamificationComponentData.instance.aiSkyMaterial.mainTexture = skyBoxItem.texture;
                RenderSettings.skybox = GamificationComponentData.instance.aiSkyMaterial;
                directionalLight.intensity = skyBoxItem.lightPPData.directionalLightData.lightIntensity;
                characterLight.intensity = skyBoxItem.lightPPData.directionalLightData.character_directionLightIntensity;
                directionalLight.shadowStrength = skyBoxItem.lightPPData.directionalLightData.directionLightShadowStrength;
                directionalLight.color = skyBoxItem.lightPPData.directionalLightData.directionLightColor;
                directionalLight.transform.rotation = Quaternion.Euler(skyBoxItem.lightPPData.directionalLightData.directionLightRot);
                //set pp for AI generated skybox
                GamificationComponentData.instance.aiPPVolumeProfile.TryGet(out bloom);
                GamificationComponentData.instance.aiPPVolumeProfile.TryGet(out whiteBalance);
                GamificationComponentData.instance.aiPPVolumeProfile.TryGet(out colorAdjustments);
                UpdateDirectionLightAndPPData(skyBoxItem);
                skyBoxItem.lightPPData.directionalLightData.lensFlareData.falreData = GamificationComponentData.instance.lensFlareDataSRP;
                lensFlareData = skyBoxItem.lightPPData.directionalLightData.lensFlareData;
            }
        }
        else
        {
            Color topColor, middleColor, bottomColor;
            ColorUtility.TryParseHtmlString("#" + skyProperties.skyColorTop, out topColor);
            ColorUtility.TryParseHtmlString("#" + skyProperties.skyColorMiddle, out middleColor);
            ColorUtility.TryParseHtmlString("#" + skyProperties.skyColorBottom, out bottomColor);

            Material mat = new Material(Shader.Find("GradientSkybox/Linear/Three Color"));
            mat.name = "Gradient Skybox";
            mat.SetColor("_TopColor", topColor);
            mat.SetColor("_MiddleColor", middleColor);
            mat.SetColor("_BottomColor", bottomColor);
            SetPostProcessProperties(defaultPostProcessVolProfile);
            RenderSettings.skybox = mat;
            directionalLight.transform.rotation = Quaternion.Euler(SituationChangerSkyboxScript.instance.defaultSkyBoxData.directionalLightData.directionLightRot);
            directionalLight.color = skyBoxColor;
            directionalLight.intensity = SituationChangerSkyboxScript.instance.defaultSkyBoxData.directionalLightData.lightIntensity;
            directionalLight.shadowStrength = SituationChangerSkyboxScript.instance.defaultSkyBoxData.directionalLightData.directionLightShadowStrength;
            characterLight.intensity = SituationChangerSkyboxScript.instance.defaultSkyBoxData.directionalLightData.character_directionLightIntensity;
            lensFlareData = SituationChangerSkyboxScript.instance.defaultSkyBoxData.directionalLightData.lensFlareData;
        }

        reflectionProbe.gameObject.SetActive(true);
        if (lensFlareData != null)
            SetLensFlareData(lensFlareData.falreData, lensFlareData.flareScale, lensFlareData.flareIntensity);
        else
            SetLensFlareData(null, 1, 1);
        GamificationComponentData.instance.isSkyLoaded = true;
        directionalLight.gameObject.SetActive(true);
        if (TimeStats.playerCanvas)
            RenderSettings.ambientLight = TimeStats.playerCanvas.oldAmbientColorBlind;
        DynamicGI.UpdateEnvironment();
    }

    private void UpdateDirectionLightAndPPData(AISkyboxItem currentItemData)
    {
        if (bloom)
        {
            bloom.active = currentItemData.lightPPData.ppData.PPBloomData.isBloomActive;
            bloom.intensity.value = currentItemData.lightPPData.ppData.PPBloomData.intensity;
            bloom.scatter.value = currentItemData.lightPPData.ppData.PPBloomData.scatter;
            bloom.threshold.value = currentItemData.lightPPData.ppData.PPBloomData.threshold;
        }

        if (whiteBalance)
        {
            whiteBalance.active = currentItemData.lightPPData.ppData.PPWhiteBalData.isWhiteBalActive;
            whiteBalance.temperature.value = currentItemData.lightPPData.ppData.PPWhiteBalData.temperature;
            whiteBalance.tint.value = currentItemData.lightPPData.ppData.PPWhiteBalData.tint;
        }

        if (colorAdjustments)
        {
            colorAdjustments.active = currentItemData.lightPPData.ppData.PPColorAdjData.isColorAdjActive;
            colorAdjustments.contrast.value = currentItemData.lightPPData.ppData.PPColorAdjData.contrast;
            colorAdjustments.postExposure.value = currentItemData.lightPPData.ppData.PPColorAdjData.exposure;
            colorAdjustments.saturation.value = currentItemData.lightPPData.ppData.PPColorAdjData.saturation;
        }

        SetPostProcessProperties(GamificationComponentData.instance.aiPPVolumeProfile);
    }

    private void LoadSkyBox_Completed(AsyncOperationHandle<Material> obj)
    {
        //Debug.Log(obj.Result.shader.name + "-----" + obj.Status);
        RenderSettings.skybox = obj.Result;
        DynamicGI.UpdateEnvironment();
    }

    void SetLensFlareData(LensFlareDataSRP lensFlareData, float lensFlareScale, float lensFlareIntensity)
    {
        LensFlareComponentSRP lensFlareComponent = directionalLight.gameObject.GetComponent<LensFlareComponentSRP>();

        if (lensFlareComponent == null)
        {
            lensFlareComponent = directionalLight.gameObject.AddComponent<LensFlareComponentSRP>();
            lensFlareComponent.occlusionRadius = 0.35f;
        }
        if (lensFlareData != null)
        {
            lensFlareComponent.lensFlareData = lensFlareData;
            lensFlareComponent.intensity = lensFlareIntensity;
            lensFlareComponent.scale = lensFlareScale;
        }
        else
        {
            lensFlareComponent.lensFlareData = null;
            lensFlareComponent.intensity = 1;
            lensFlareComponent.scale = 1;
        }
    }

    void SetPlayerProperties()
    {
        BuilderEventManager.ApplyPlayerProperties?.Invoke(levelData.playerProperties.jumpMultiplier, levelData.playerProperties.speedMultiplier);
        //Invoke(nameof(XanaSetItemData), 1.5f);
    }



    void XanaSetItemData()
    {
        StartCoroutine(XanaSetItemDataCO());
    }

    IEnumerator XanaSetItemDataCO()
    {
        if (levelData.otherItems.Count > 0)
        {
            if (!GamificationComponentData.instance.withMultiplayer)
            {
                yield return StartCoroutine(DownloadAddressableGamificationObject());
                yield return StartCoroutine(GemificationObjectLoadWait(1f));
            }

            while (GamificationComponentData.instance.MultiplayerComponentstoSet.Count != GamificationComponentData.instance.MultiplayerComponentData.Count)
            {
                yield return new WaitForSeconds(5f);
            }
            foreach (XanaItem xanaItem in GamificationComponentData.instance.xanaItems)
            {
                xanaItem.SetData(xanaItem.itemData);
            }

            GamificationComponentData.WarpComponentLocationUpdate?.Invoke();
            //Set Hierarchy same as builder
            SetObjectHirarchy();

            BuilderEventManager.CombineMeshes?.Invoke();
        }

        //PlayerSetup();

        Hashtable _hash = new Hashtable();
        _hash.Add("IsReady", true);
        PhotonNetwork.LocalPlayer.SetCustomProperties(_hash);

        PlayerSetup();

        //call for Execute all rpcs of this room
        BuilderEventManager.RPCcallwhenPlayerJoin?.Invoke();
        BuilderEventManager.BGMStart?.Invoke();
        BuilderEventManager.UploadPropertiesInit?.Invoke();
        reflectionProbe.enabled = true;
        if (levelData.skyProperties.skyId != -1)
        {
            GameplayEntityLoader.instance.environmentCameraRender.clearFlags = CameraClearFlags.Skybox;
            GameplayEntityLoader.instance.firstPersonCamera.clearFlags = CameraClearFlags.Skybox;
        }
    }

    internal void PlayerSetup()
    {
        if (!GamificationComponentData.instance.buildingDetect)
            return;

        CapsuleCollider capsuleCollider_34 = GamificationComponentData.instance.buildingDetect.GetComponent<CapsuleCollider>();
        capsuleCollider_34.enabled = true;
        capsuleCollider_34.isTrigger = false;
        CharacterController mainPlayerCharacterController = GamificationComponentData.instance.playerControllerNew.GetComponent<CharacterController>();
        mainPlayerCharacterController.center = Vector3.up * 0.9f;
        mainPlayerCharacterController.height = 1.65f;
        mainPlayerCharacterController.radius = 0.2f;
        mainPlayerCharacterController.stepOffset = .25f;
        CapsuleCollider mainPlayerCollider = GamificationComponentData.instance.playerControllerNew.GetComponent<CapsuleCollider>();
        mainPlayerCollider.center = Vector3.up * 0.5f;

        //CapsuleCollider playerCollider = GamificationComponentData.instance.charcterBodyParts.GetComponent<CapsuleCollider>();
        capsuleCollider_34.height = 1.5f;
        capsuleCollider_34.center = Vector3.up * 0.68f;
        capsuleCollider_34.radius = .3f;
        CharacterController playerCharacterController = GamificationComponentData.instance.charcterBodyParts.GetComponent<CharacterController>();
        playerCharacterController.height = capsuleCollider_34.height;
        playerCharacterController.center = capsuleCollider_34.center;
        GamificationComponentData.instance.isBuilderWorldPlayerSetup = true;
        //GamificationComponentData.instance.playerControllerNew.transform.localPosition += Vector3.up;
    }

    public void UpdateScene()
    {
        DynamicGI.UpdateEnvironment();
    }
    void SetPostProcessProperties(VolumeProfile _postProcessVol)
    {
        postProcessVol.profile = _postProcessVol;
        Vignette vignette;
        postProcessVol.profile.TryGet(out vignette);

        if (vignette && GamificationComponentData.instance.buildingDetect)
        {
            GamificationComponentData.instance.buildingDetect.defaultIntensityvalue = (float)vignette.intensity;
            GamificationComponentData.instance.buildingDetect.defaultSmootnesshvalue = (float)vignette.smoothness;
        }
    }


    private void GetObject(AsyncOperationHandle obj, ItemData _itemData)
    {
        switch (obj.Status)
        {
            case AsyncOperationStatus.Succeeded:
                GameObject tempObject = obj.Result as GameObject;
                CreateENV(tempObject, _itemData);
                break;

            case AsyncOperationStatus.Failed:
                //Debug.Log("Download error");
                break;

            default:
                break;
        }

    }

    private void CreateENV(GameObject objectTobeInstantiate, ItemData _itemData)
    {
        GameObject newObj = Instantiate(objectTobeInstantiate, _itemData.Position, _itemData.Rotation, builderAssetsParent);
        newObj.SetActive(true);
        XanaItem xanaItem = newObj.GetComponent<XanaItem>();
        xanaItem.itemData = _itemData;
        newObj.transform.localScale = _itemData.Scale;
        if (_itemData.ItemID.Contains("SFP") && serverData.data.worldType == 1)
        {
            StartFinishPointData startFinishPlatform = new StartFinishPointData();
            startFinishPlatform.ItemID = _itemData.ItemID;
            startFinishPlatform.SpawnObject = newObj;
            startFinishPlatform.IsStartPoint = startFinishPlatform.SpawnObject.GetComponent<StartPoint>() != null ? true : false;
            BuilderData.StartFinishPoints.Add(startFinishPlatform);
        }
        else if (_itemData.ItemID.Contains("SPW") || _itemData.spawnComponent)
        {
            SpawnPointData spawnPointData = new SpawnPointData();
            spawnPointData.spawnObject = newObj;
            spawnPointData.IsActive = _itemData.spawnerComponentData.IsActive;
            BuilderData.spawnPoint.Add(spawnPointData);
        }

        if (IsMultiplayerComponent(_itemData) && GamificationComponentData.instance.withMultiplayer)
        {
            newObj.SetActive(false);

            GamificationComponentData.instance.MultiplayerComponentData.Add(_itemData);
            var multiplayerObject = Instantiate(MultiplayerComponent, _itemData.Position, _itemData.Rotation);
            MultiplayerComponentData multiplayerComponentData = new();
            multiplayerComponentData.RuntimeItemID = _itemData.RuntimeItemID;
            multiplayerComponentData.viewID = 0;
            GamificationComponentData.instance.SetMultiplayerComponentData(multiplayerComponentData);

            return;
        }

        //meshCombinerRef.HandleRendererEvent(xanaItem.itemGFXHandler._renderers, _itemData);
        //foreach (Transform childTransform in newObj.GetComponentsInChildren<Transform>())
        //{
        //    childTransform.tag = "Item";
        //}

        //Add game object into XanaItems List for Hirarchy
        //if (!GamificationComponentData.instance.xanaItems.Exists(x => x == xanaItem))
        GamificationComponentData.instance.xanaItems.Add(xanaItem);
        if (!_itemData.isVisible)
            newObj.SetActive(false);
    }

    bool IsMultiplayerComponent(ItemData itemData)
    {
        if (itemData.rotatorComponentData.IsActive || itemData.addForceComponentData.isActive || itemData.toFroComponentData.IsActive || itemData.translateComponentData.IsActive || itemData.scalerComponentData.IsActive || itemData.rotateComponentData.IsActive)
        {
            return true;
        }
        else return false;
    }
    #endregion

    #region COROUTINE

    IEnumerator AISkyTextureDownload()
    {
        string textureURL = aiSkyboxItem.textureURL;
        if (textureURL.Contains("https://cdn.xana.net/xanaprod/Defaults/"))
        {
            textureURL = textureURL.Replace("https://cdn.xana.net/xanaprod/Defaults/", "https://aydvewoyxq.cloudimg.io/" + (APIBasepointManager.instance.IsXanaLive ? "_xanaprod_" : "_apitestxana_") + "/xanaprod/Defaults/");
            textureURL += "?width=4096&height=2048";
        }
        var texture = new Texture2D(256, 128, GamificationComponentData.instance.GetTextureFormat(), false);
        var imagineImageRequest = UnityWebRequest.Get(textureURL);
        imagineImageRequest.SendWebRequest();

        while (!imagineImageRequest.isDone)
            yield return null;

        if (imagineImageRequest.result != UnityWebRequest.Result.Success)
        {
            imagineImageRequest.Dispose();
        }
        else
        {
            var image = imagineImageRequest.downloadHandler.data;
            texture.LoadImage(image);
            texture.Compress(true);
            aiSkyboxItem.texture = texture;
            imagineImageRequest.Dispose();
        }
    }

    IEnumerator GetTexture(string url, Action<Texture> DownloadedTexture)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        request.SendWebRequest();
        while (!request.isDone)
            yield return null;
        if ((request.result == UnityWebRequest.Result.ConnectionError) || (request.result == UnityWebRequest.Result.ProtocolError))
            Debug.Log(request.error);
        else
        {
            DownloadedTexture(((DownloadHandlerTexture)request.downloadHandler).texture);
        }

        request.Dispose();
    }

    void LoadAddressableSceneAfterDownload()
    {
        if ((SceneManager.sceneCount > 1 || ConstantsHolder.isFromXANASummit) && !ConstantsHolder.xanaConstants.isXanaPartyWorld)
        {
            Photon.Pun.Demo.PunBasics.MutiplayerController.instance.Connect(ConstantsHolder.xanaConstants.EnviornmentName);
            return;
        }
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
        //if (ConstantsHolder.xanaConstants.isFromXanaLobby)
        //{
        //    LoadingHandler.Instance.UpdateLoadingSliderForJJ(UnityEngine.Random.Range(.8f, .9f), 0.1f);
        //}
        if (!ConstantsHolder.xanaConstants.isFromXanaLobby)
        {
            // LoadingHandler.Instance.UpdateLoadingSlider(.8f);
            LoadingHandler.Instance.UpdateLoadingStatusText("Getting World Ready....");
        }
    }

    #endregion

    [System.Serializable]
    public class DataTemp
    {
        public GameObject _object;
        public ItemData _itemData;
    }
}

[Serializable]
public class ServerData
{
    public bool success;
    public Data data = new Data();
    public string msg;
}

[Serializable]
public class Data
{
    public int id;
    public string name;
    public string thumbnail;
    public string terrainTexture;
    public LevelData json;
    public string status;
    public string description;
    public string map_json_link;
    public User user;

    public int worldType;
    // Count Variable Added by WaqasAhmad
    // Same Class used in Analytics Script
    public string count;
}

[Serializable]
public class UserMap
{
    public int id;
    public string name;
    public string thumbnail;
    public string terrainTexture;
    public LevelData json = new LevelData();
    public string status;
    public string description;
    public bool isDeleted;
    public int createdBy;
    public string createdAt;
    public string updatedAt;
    public User user = new User();
}

[Serializable]
public class User
{
    public int id;
    public string name;
    public string email;
    public string avatar;
    public string phoneNumber;
    public string coins;
}

[Serializable]
public class LevelData
{
    public string mapCode;
    public string mapName;
    public List<ItemData> groundItems;  //ground items
    public List<ItemData> otherItems;   //other items
    public SkyProperties skyProperties;
    public PlayerProperties playerProperties;
    public TerrainProperties terrainProperties;
    public UploadProperties uploadProperties;
    public AudioPropertiesBGM audioPropertiesBGM = new AudioPropertiesBGM();
}

//User Map Data Model - 
[Serializable]
public class UserMapData
{
    public string mapName;
    public string mapSpritePath;
    public string timestamp;
    public string mapCode;

    public UserMapData()
    {
        mapName = mapSpritePath = timestamp = mapCode = null;
    }
}

//User Maps
[Serializable]
public class UserMaps
{
    public List<UserMapData> userMapList;
}

[Serializable]
public class AudioPropertiesBGM
{
    #region Data Variables
    public DataAudioBGM dataAudioBGM = new DataAudioBGM();
    #endregion
}

[Serializable]
public class DataAudioBGM
{
    #region Data Variables
    public string pathAudioBGM;
    public bool audioLoopBGM;
    public bool enableDisableBGM;
    public float audioVolume = .1f;
    #endregion
}

[Serializable]
public class SkyProperties
{
    #region Data Variables
    //public string skyColor;
    public string skyColorTop;
    public string skyColorMiddle;
    public string skyColorBottom;
    public int skyId;
    public AISkyboxItem aISkyboxItem = new AISkyboxItem();
    #endregion

    public SkyProperties()
    {
        //this.skyColor = ColorUtility.ToHtmlStringRGBA(Color.white);  //init default color as white
        if (skyColorTop == null)
            this.skyColorTop = ColorUtility.ToHtmlStringRGBA(Color.blue);  //init default color as white
        else
        {
            Color color;
            ColorUtility.TryParseHtmlString("#" + "", out color);
            this.skyColorTop = ColorUtility.ToHtmlStringRGBA(color);
            //Debug.Log("skyColorTop : " + skyColorTop);
        }
        if (skyColorMiddle == null)
        {
            Color color;
            this.skyColorMiddle = ColorUtility.ToHtmlStringRGBA(Color.blue + Color.white);
        }
        else
        {
            Color color;
            ColorUtility.TryParseHtmlString("#" + "", out color);
            this.skyColorMiddle = ColorUtility.ToHtmlStringRGBA(color);
            //Debug.Log("skyColorMiddle : " + skyColorMiddle);
        }
        if (skyColorBottom == null)
            this.skyColorBottom = ColorUtility.ToHtmlStringRGBA(Color.white);  //init default color as white
        else
        {
            Color color;
            ColorUtility.TryParseHtmlString("#" + "", out color);
            this.skyColorBottom = ColorUtility.ToHtmlStringRGBA(color);
            //Debug.Log("skyColorBottom : " + skyColorBottom);
        }

        aISkyboxItem = new AISkyboxItem();
    }
}

#region AI Skybox

[Serializable]
public class AISkyboxItem
{
    public string obfuscatedID;
    public string textureURL;
    public Texture2D texture;
    public string skyName;
    //public DirectionalLightData directionalLightData;
    public LightPPData lightPPData;
    public AISkyboxItem()
    {
        lightPPData = new LightPPData();
    }
}

[Serializable]
public class LightPPData
{
    public int uniqueID;
    public string name;
    public DirectionalLightData directionalLightData;
    public PostProcessData ppData;
    public LightPPData()
    {
        directionalLightData = new DirectionalLightData();
        ppData = new PostProcessData();
    }
}
[Serializable]
public class PostProcessData
{
    public PPBloomData PPBloomData;
    public PPWhiteBalData PPWhiteBalData;
    public PPColorAdjData PPColorAdjData;

    public PostProcessData()
    {
        PPBloomData = new PPBloomData();
        PPWhiteBalData = new PPWhiteBalData();
        PPColorAdjData = new PPColorAdjData();
    }
}
[Serializable]
public class PPBloomData
{
    public bool isBloomActive;
    public float threshold, intensity, scatter;
    public PPBloomData()
    {
        isBloomActive = true;
        threshold = 1.5f;
        intensity = 1f;
        scatter = 0.7f;
    }
}
[Serializable]
public class PPWhiteBalData
{
    public bool isWhiteBalActive;
    public float temperature, tint;
    public PPWhiteBalData()
    {
        isWhiteBalActive = true;
        temperature = -5f;
        tint = 0;
    }
}
[Serializable]
public class PPColorAdjData
{
    public bool isColorAdjActive;
    public float exposure, contrast, saturation;

    public PPColorAdjData()
    {
        isColorAdjActive = true;
        exposure = .3f;
        contrast = 20f;
        saturation = 2f;
    }
}
#endregion

[Serializable]
public class PlayerProperties
{
    #region Data Variables
    public float speedMultiplier;
    public float jumpMultiplier;
    #endregion

    public PlayerProperties()
    {
        this.speedMultiplier = 1f;
        this.jumpMultiplier = 1f;
    }
}

[Serializable]
public class TerrainProperties
{
    #region Data Variables
    public string materialName;
    public string texturePath;
    public string waterTexturePath;
    public string meshDeformationPath;
    public Vector3[] deformedPlane;
    public Vector3 planeScale;
    public Vector3 planePos;
    public int upMovesAllowed;
    public int downMovesAllowed;
    public int leftMovesAllowed;
    public int rightMovesAllowed;
    public int planeHeightLimit;
    public int terrainTextureSelected;
    public Texture terrainTexture;
    public int realisticMatIndex;
    #endregion

    public TerrainProperties()
    {
        materialName = "";
        texturePath = "";
        waterTexturePath = "";
        meshDeformationPath = "";
        planeScale = new UnityEngine.Vector3(0f, 1f, 0f);
        planePos = new UnityEngine.Vector3(0.0f, 0f, 0.0f);
        upMovesAllowed = downMovesAllowed = leftMovesAllowed = rightMovesAllowed = 2;
        planeHeightLimit = 5;
        terrainTextureSelected = 0;
        realisticMatIndex = -1;
        terrainTexture = null;
    }
}

[Serializable]
public class SpawnComponentData
{
    public bool IsActive;
}

[Serializable]
public class MultiplayerComponentData
{
    public string RuntimeItemID = "";
    public int viewID = 0;
}
[Serializable]
public class MultiplayerComponentDatas
{
    public List<MultiplayerComponentData> multiplayerComponents = new List<MultiplayerComponentData>();
}

[Serializable]
public class RealisticMaterialData
{
    public int id;
    public string name;
    //public Material material;
    public string shaderName;
    //public Sprite thumbnail;
}
public enum MediaTypeBuilder
{
    YouTube, LocalVideo, LocalImage
}
[Serializable]
public class UploadProperties
{
    #region Data Variables
    public List<UploadData> uploadData;
    #endregion
}

[Serializable]
public class UploadData
{
    #region Data Variables
    public string uploadMediaId;
    public Vector3 position;
    public Quaternion rotation;
    // public Vector3 scale;
    public float scale;
    public string url;
    public string localFileName;
    public int index;
    public MediaTypeBuilder mediaType;
    public bool isLivestream;
    public bool addFrame;
    public bool isRepeat;

    #endregion

    public UploadData()
    {
        url = "";
        mediaType = MediaTypeBuilder.YouTube;
    }
}