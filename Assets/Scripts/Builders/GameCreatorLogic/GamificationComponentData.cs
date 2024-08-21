using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Models;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Rendering;

public class GamificationComponentData : MonoBehaviourPunCallbacks
{
    public static GamificationComponentData instance;

    //References for Gamification Components
    public BuildingDetect buildingDetect;
    public Volume postProcessVol;
    public RuntimeAnimatorController cameraBlurEffect;
    internal GameObject specialItemParticleEffect;
    public Material hologramMaterial;
    public Shader superMarioShader;
    public Shader superMarioShader2;
    public Shader skinShader;
    public Shader cloathShader;
    internal PlayerController playerControllerNew;
    internal AvatarController avatarController;
    internal CharacterBodyParts charcterBodyParts;
    internal IKMuseum ikMuseum;
    public Texture defaultSkyTex;

    public Vector3 spawnPointPosition;
    public GameObject raycast;
    public GameObject throwAimPrefab;
    public Material lineMaterial;
    public GameObject handBall;

    public GameObject helpParentReference;
    public GameObject worldSpaceCanvas;

    public AudioSource audioSource;

    internal Vector3 Ninja_Throw_InitPosY;
    internal Vector3 Ninja_Throw_InitPosX;
    internal bool worldCameraEnable;

    //Orientation Changer
    public CanvasGroup landscapeCanvas;
    public CanvasGroup potraitCanvas;

    internal IComponentBehaviour activeComponent;

    internal List<WarpFunctionComponent> warpComponentList = new List<WarpFunctionComponent>();

    public static Action WarpComponentLocationUpdate;

    //public List<GameObject> AvatarChangerModels;
    public List<string> AvatarChangerModelNames;

    internal bool isNight;
    internal bool isBlindToogle;
    internal bool isAvatarChanger;
    internal bool isBlindfoldedFootPrinting;
    internal int previousSkyID;

    internal List<XanaItem> xanaItems = new List<XanaItem>();
    internal List<XanaItem> multiplayerComponentsxanaItems = new List<XanaItem>();
    internal List<GameObject> multiplayerComponentsObject = new List<GameObject>();
    public List<string> multiplayerComponentsName = new List<string>();

    //AI Generated Skybox
    public Material aiSkyMaterial;
    public VolumeProfile aiPPVolumeProfile;
    public LensFlareDataSRP lensFlareDataSRP;
    internal bool isSkyLoaded;

    //Gamification components with multipler
    public bool withMultiplayer = false;
    internal GameObject DoorKeyObject;
    internal int doorKeyCount = 0;

    //Font
    public TMPro.TMP_FontAsset orbitronFont;
    internal TMPro.TMP_FontAsset hiraginoFont;
    //public TMPro.TMP_FontAsset arialFont;

    //Name canvas
    internal Canvas nameCanvas;
    public PlayerCanvas playerCanvas;
    internal bool isBuilderWorldPlayerSetup;
    public RuntimeAnimatorController idleAnimation;
    internal bool ZoomControl;

    //platformLayers
    public LayerMask platformLayers;

    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;

    internal List<ItemData> MultiplayerComponentData = new List<ItemData>();
    public MultiplayerComponent MultiplayerComponent;

    bool isPotrait = false;
    private void Awake()
    {
        instance = this;
    }

    public new void OnEnable()
    {
        BuilderEventManager.ReSpawnPlayer += PlayerSpawnBlindfoldedDisplay;
        //ChangeOrientation
        BuilderEventManager.BuilderSceneOrientationChange += OrientationChange;
        //OnSelfiActive
        BuilderEventManager.UIToggle += UICanvasToggle;
        BuilderEventManager.RPCcallwhenPlayerJoin += GetRPC;

        OrientationChange(false);
        warpComponentList.Clear();
        WarpComponentLocationUpdate += UpdateWarpFunctionData;

        //reset ignore layer collision on scene load
        Physics.IgnoreLayerCollision(9, 22, false);
        ZoomControl = true;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        BuilderEventManager.ReSpawnPlayer -= PlayerSpawnBlindfoldedDisplay;
        BuilderEventManager.BuilderSceneOrientationChange -= OrientationChange;
        BuilderEventManager.UIToggle -= UICanvasToggle;
        BuilderEventManager.RPCcallwhenPlayerJoin -= GetRPC;

        WarpComponentLocationUpdate -= UpdateWarpFunctionData;

    }

    public void PlayerSpawnBlindfoldedDisplay()
    {
        StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(0.2f);
        playerControllerNew.transform.position = spawnPointPosition;
        yield return new WaitForSeconds(0.3f);
        Physics.IgnoreLayerCollision(9, 22, false);

        Debug.Log("Blindfolded spawned");
    }

    public float MapValue(float oldValue, float oldMin, float oldMax, float newMin, float newMax)
    {
        return (oldValue - oldMin) * (newMax - newMin) / (oldMax - oldMin) + newMin;
    }

    public TextureFormat GetTextureFormat()
    {
#if UNITY_ANDROID || UNITY_IOS
        return TextureFormat.ASTC_8x8;
#elif UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
        return TextureFormat.ETC2_RGBA8Crunched;
#elif UNITY_WEBGL
        return TextureFormat.ETC2_RGBA8Crunched;
#endif
    }

    #region OrientationChange
    void OrientationChange(bool orientation)
    {
        StartCoroutine(ChangeOrientation(orientation));
    }

    IEnumerator ChangeOrientation(bool orientation)
    {
        isPotrait = orientation;
        DisableUICanvas();
        yield return new WaitForSeconds(0.1f);
        if (isPotrait)
        {
            potraitCanvas.DOFade(1, 0.5f);
            potraitCanvas.blocksRaycasts = true;
            potraitCanvas.interactable = true;
        }
        else
        {
            landscapeCanvas.DOFade(1, 0.5f);
            landscapeCanvas.blocksRaycasts = true;
            landscapeCanvas.interactable = true;
        }

        yield return new WaitForSeconds(0.5f);

        BuilderEventManager.PositionUpdateOnOrientationChange?.Invoke();
    }

    void UICanvasToggle(bool state)
    {
        if (state)
        {
            DisableUICanvas();
        }
        else
            StartCoroutine(ChangeOrientation(isPotrait));
    }

    void DisableUICanvas()
    {
        landscapeCanvas.DOKill();
        landscapeCanvas.alpha = 0;
        landscapeCanvas.blocksRaycasts = false;
        landscapeCanvas.interactable = false;
        potraitCanvas.DOKill();
        potraitCanvas.alpha = 0;
        potraitCanvas.blocksRaycasts = false;
        potraitCanvas.interactable = false;
    }
    #endregion

    #region WarpComponent location update
    void UpdateWarpFunctionData()
    {
        foreach (WarpFunctionComponent warpFunctionComponent1 in warpComponentList)
        {
            foreach (WarpFunctionComponent warpFunctionComponent2 in warpComponentList)
            {
                if (warpFunctionComponent1 == warpFunctionComponent2)
                    continue;

                WarpFunctionComponentData data1 = warpFunctionComponent1.warpFunctionComponentData;
                WarpFunctionComponentData data2 = warpFunctionComponent2.warpFunctionComponentData;

                if (data1.isWarpPortalStart)
                {
                    string startKey = data1.warpPortalStartKeyValue;

                    if (startKey == data2.warpPortalEndKeyValue && startKey != "")
                    {
                        Vector3 endPoint = warpFunctionComponent2.transform.position;
                        endPoint.y += warpFunctionComponent2.GetComponent<XanaItem>().m_renderer.bounds.size.y;
                        UpdateEndPortalLocations(data1.warpPortalDataEndPoint, startKey, endPoint);
                        Vector3 startPoint = warpFunctionComponent1.transform.position;
                        startPoint.y += warpFunctionComponent1.GetComponent<XanaItem>().m_renderer.bounds.size.y;
                        UpdateStartPortalLocations(data2.warpPortalDataStartPoint, startKey, startPoint);
                    }
                }
                else
                {
                    string endKey = data1.warpPortalEndKeyValue;
                    if (endKey == data2.warpPortalStartKeyValue && endKey != "")
                    {
                        Vector3 endPoint = warpFunctionComponent1.transform.position;
                        endPoint.y += warpFunctionComponent1.GetComponent<XanaItem>().m_renderer.bounds.size.y;
                        UpdateEndPortalLocations(data2.warpPortalDataEndPoint, endKey, endPoint);
                        Vector3 startPoint = warpFunctionComponent2.transform.position;
                        startPoint.y += warpFunctionComponent2.GetComponent<XanaItem>().m_renderer.bounds.size.y;
                        UpdateStartPortalLocations(data1.warpPortalDataStartPoint, endKey, startPoint);
                    }
                }
            }
        }
    }

    void UpdateStartPortalLocations(List<PortalSystemStartPoint> endPoints, string key, Vector3 location)
    {
        PortalSystemStartPoint portalSystemEndPoint = endPoints.Find(x => x.indexPortalStartKey == key);
        if (portalSystemEndPoint != null)
            portalSystemEndPoint.portalStartLocation = location;
    }

    void UpdateEndPortalLocations(List<PortalSystemEndPoint> endPoints, string key, Vector3 location)
    {
        PortalSystemEndPoint portalSystemEndPoint = endPoints.Find(x => x.indexPortalEndKey == key);
        if (portalSystemEndPoint != null)
            portalSystemEndPoint.portalEndLocation = location;
    }
    #endregion

    #region Components for multiplayer
    //All components for multiplayer
    [PunRPC]
    public void GetObject(string RuntimeItemID, Constants.ItemComponentType componentType)
    {
        if (!withMultiplayer)
            return;
        //store rpc data in roomoption
        if (PhotonNetwork.IsMasterClient && (componentType != Constants.ItemComponentType.AudioComponent))
            SetRoomData(RuntimeItemID, componentType);

        GetItemFromList(RuntimeItemID, componentType);
    }

    public void GetObjectwithoutRPC(string RuntimeItemID, Constants.ItemComponentType componentType)
    {
        GetItemFromList(RuntimeItemID, componentType);
    }

    void GetItemFromList(string RuntimeItemID, Constants.ItemComponentType componentType)
    {
        var item = xanaItems.FirstOrDefault(x => x.itemData.RuntimeItemID == RuntimeItemID);
        if (componentType == Constants.ItemComponentType.none)
        {
            item.gameObject.SetActive(false);
            return;
        }

        RestrictionComponents(componentType);

        if (item != null)
        {
            var component = item.GetComponent(componentType.ToString()) as ItemComponent;
            if (component != null)
            {
                component.PlayBehaviour();
            }
        }
    }

    void RestrictionComponents(Constants.ItemComponentType componentType)
    {
        if (componentType == Constants.ItemComponentType.BlindComponent || componentType == Constants.ItemComponentType.SituationChangerComponent)
            return;
        BuilderEventManager.onComponentActivated?.Invoke(componentType);
    }

    internal void SetRoomData(string RuntimeItemID, Constants.ItemComponentType componentType)
    {

        GamificationComponentRPC gamificationComponentRPC = new GamificationComponentRPC();
        gamificationComponentRPC.RuntimeItemID = RuntimeItemID;
        gamificationComponentRPC.componentType = componentType.ToString();

        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();

        GamificationComponentRPCs componentRPCs = new GamificationComponentRPCs();

        // Populate your GamificationComponentRPC list here

        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gamificationComponentRPCs", out object gamificationComponentRPCsObj))
        {
            componentRPCs = JsonUtility.FromJson<GamificationComponentRPCs>(gamificationComponentRPCsObj.ToString());
        }

        componentRPCs.rpcList.Add(gamificationComponentRPC);
        string json = JsonUtility.ToJson(componentRPCs);
        customProperties["gamificationComponentRPCs"] = json;
        PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
    }

    void GetRPC()
    {
        if (!withMultiplayer)
            return;

        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gamificationComponentRPCs", out object gamificationComponentRPCsObj))
        {
            GamificationComponentRPCs componentRPCs = JsonUtility.FromJson<GamificationComponentRPCs>(gamificationComponentRPCsObj.ToString());

            if (componentRPCs.rpcList != null)
            {
                foreach (GamificationComponentRPC gamificationComponentRPC in componentRPCs.rpcList)
                {
                    Constants.ItemComponentType componentType = (Constants.ItemComponentType)Enum.Parse(typeof(Constants.ItemComponentType), gamificationComponentRPC.componentType);

                    GetObjectwithoutRPC(gamificationComponentRPC.RuntimeItemID, componentType);
                }
            }
        }
    }
    MultiplayerComponentDatas multiplayerComponentdatas = new MultiplayerComponentDatas();
    internal void SetMultiplayerComponentData(MultiplayerComponentData multiplayerComponentData)
    {
        var hash = new ExitGames.Client.Photon.Hashtable();

        // Multiplayer component data list
        if (multiplayerComponentdatas.multiplayerComponents.Count == 0 && PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gamificationMultiplayerComponentDatas", out object multiplayerComponentdatasObj))
        {
            multiplayerComponentdatas = JsonUtility.FromJson<MultiplayerComponentDatas>(multiplayerComponentdatasObj.ToString());
        }

        multiplayerComponentdatas.multiplayerComponents.Add(multiplayerComponentData);
        string json = JsonUtility.ToJson(multiplayerComponentdatas);
        //Debug.LogError(json);
        hash.Add("gamificationMultiplayerComponentDatas", json);
        PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
    }

    public void MasterClientSwitched(Player newMasterClient)
    {
        //if (!withMultiplayer)
        //    return;

        //if (PhotonNetwork.LocalPlayer == newMasterClient)
        //{
        //    foreach (XanaItem xanaItem in multiplayerComponentsxanaItems)
        //    {
        //        if (!xanaItem.itemData.addForceComponentData.isActive || !xanaItem.itemData.translateComponentData.avatarTriggerToggle)
        //            xanaItem.SetData(xanaItem.itemData);
        //    }
        //}
    }
    #endregion
}

[Serializable]
public class GamificationComponentRPC
{
    public string RuntimeItemID = "";
    public string componentType = "";
}
[Serializable]
public class GamificationComponentRPCs
{
    public List<GamificationComponentRPC> rpcList = new List<GamificationComponentRPC>();
}

[Serializable]
public class UTCTimeCounterValue
{
    public string UTCTime = "";
    public float CounterValue = 0;
}