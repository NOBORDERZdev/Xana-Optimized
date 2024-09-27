using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using SuperStar.Helpers;
using UnityEngine.Networking;
using Photon.Pun.Demo.PunBasics;
using EnhancedUI.EnhancedScroller;

public class WorldItemView : EnhancedScrollerCellView
{
    /*[SerializeField] private DynamicScrollRect.DynamicScrollRect _dynamicScroll = null;*/
    /*public int Index;
    public Vector2 GridIndex { get; protected set; }*/
    public RectTransform RectTransform => transform as RectTransform;
    public void Activated()
    {
        gameObject.SetActive(true);
    }
    public void Deactivated()
    {
        gameObject.SetActive(false);
    }
    public void InitItem(int index, Vector2 gridPos, WorldItemDetail detail, int _loopcount = 0)
    {
        if (PreviewLogo)
            PreviewLogo.gameObject.SetActive(true);
        /*Index = index;
        GridIndex = gridPos;*/
        idOfObject = detail.IdOfWorld;
        m_EnvironmentName = detail.EnvironmentName;
        m_WorldDescription = detail.WorldDescription;
        //Changed image quality due to poor text quality
        if (m_EnvironmentName == "ZONE-X")
            detail.ThumbnailDownloadURL = detail.ThumbnailDownloadURL.Replace("?width=640&height=360", "?width=960&height=540");
        m_ThumbnailDownloadURL = detail.ThumbnailDownloadURL;
        //creatorName = detail.Creator_Name;
        createdAt = detail.CreatedAt;
        userLimit = detail.UserLimit;
        userAvatarURL = detail.UserAvatarURL;
        updatedAt = detail.UpdatedAt;
        entityType = detail.EntityType;
        m_BannerLink = detail.BannerLink;
        m_PressedIndex = detail.PressedIndex;
        ThumbnailDownloadURLHigh = detail.ThumbnailDownloadURLHigh;
        worldTags = detail.WorldTags;
        Creator_Name = detail.Creator_Name;
        CreatorAvatarURL = detail.CreatorAvatarURL;
        CreatorDescription = detail.CreatorDescription;
        worldVisitCount = detail.WorldVisitCount;
        isFavourite = detail.isFavourite;
        if (creatorNameText)
            creatorNameText.text = Creator_Name;
        Init(index, _loopcount);
    }


    public void InitItem(WorldItemDetail detail, int _loopcount = 0)
    {
        if (PreviewLogo)
            PreviewLogo.gameObject.SetActive(true);
        idOfObject = detail.IdOfWorld;
        m_EnvironmentName = detail.EnvironmentName;
        m_WorldDescription = detail.WorldDescription;
        m_ThumbnailDownloadURL = detail.ThumbnailDownloadURL;
        //creatorName = detail.CreatorName;
        createdAt = detail.CreatedAt;
        userLimit = detail.UserLimit;
        userAvatarURL = detail.UserAvatarURL;
        updatedAt = detail.UpdatedAt;
        entityType = detail.EntityType;
        m_BannerLink = detail.BannerLink;
        m_PressedIndex = detail.PressedIndex;
        ThumbnailDownloadURLHigh = detail.ThumbnailDownloadURLHigh;
        worldTags = detail.WorldTags;
        Creator_Name = detail.Creator_Name;
        CreatorAvatarURL = detail.CreatorAvatarURL;
        CreatorDescription = SNS_APIManager.DecodedString(detail.CreatorDescription);
        worldVisitCount = detail.WorldVisitCount;
        UserMicEnable=detail.UserMicEnable;
        isFavourite = detail.isFavourite;
        if (creatorNameText)
            creatorNameText.text = Creator_Name;
        Init(0, _loopcount);
    }


    public static string m_EnvName;
    public static string m_CreaName;
    [Header("WorldNameAndLinks")]
    public string idOfObject;
    public string m_EnvironmentName;
    public string m_WorldDescription;
    public string m_ThumbnailDownloadURL, ThumbnailDownloadURLHigh;
    //public string creatorName;
    public string createdAt;
    public string userLimit;
    public string userAvatarURL;
    public string updatedAt = "00";
    public string entityType = "None";
    [Header("WorldNameAndDescription")]
    public TextMeshProUGUI eviroment_Name;
    public TextMeshProUGUI joinedUserCount;
    public TextMeshProUGUI creatorNameText;
    public string m_BannerLink;
    public Image worldIcon;
    public int m_PressedIndex;
    public bool isMuseumScene = false;
    public bool isBuilderScene = false;
    public bool isEnvirnomentScene = false;
    public bool isImageSuccessDownloadAndSave = false;
    public bool isReleaseFromMemoryOrNot = false;
    public bool isOnScreen;
    public bool isVisible = false;
    bool isNotLoaded = true;
    public Transform PreviewLogo;

    [Header("Tags and Category")]
    public string[] worldTags;
    public string Creator_Name;
    public string CreatorAvatarURL;
    public string CreatorDescription;
    public string worldVisitCount;
    public bool UserMicEnable;
    public bool isFavourite;
    public WorldDescriptionPopupPreview worldItemPreview;
    UserAnalyticsHandler userAnalyticsHandler;
    bool isBannerLoaded = false;
    //private void OnEnable()
    //{
    //    //UpdateUserCount();
    //    //if (!string.IsNullOrEmpty(m_ThumbnailDownloadURL))
    //    //{
    //    //    LoadImagesFromRemote();
    //    //}
    //    //UserAnalyticsHandler.onChangeJoinUserStats += UpdateUserCount;
    //}
    //private void OnDisable()
    //{
    //    if (!m_EnvironmentName.Contains("XANA Lobby"))
    //    {
    //        AssetCache.Instance.RemoveFromMemoryDelayCoroutine(m_ThumbnailDownloadURL, true);
    //        worldIcon.sprite = null;
    //        worldIcon.sprite = default;
    //    }
    //    //UserAnalyticsHandler.onChangeJoinUserStats -= UpdateUserCount;
    //}
    private void OnEnable()
    {
        if(worldIcon.sprite==null)
        {
            LoadImagesFromRemote();
            worldItemPreview = WorldManager.instance.worldItemPreviewTabRef;
        }
    }

    public void Init(int worlditemcount, int _loopcount)
    {
        GetEventType(entityType);
        //if (!m_EnvironmentName.Contains("XANA Lobby"))
        //    this.GetComponent<Button>().interactable = false;
        userAnalyticsHandler = APIBasepointManager.instance.GetComponent<UserAnalyticsHandler>();
        //UpdateUserCount();
        SetWorldName();
        worldItemPreview = WorldManager.instance.worldItemPreviewTabRef;
        LoadImagesFromRemote(worlditemcount, _loopcount);
    }


    void LoadImagesFromRemote(int worlditemcount = 0, int _loopcount = 0)
    {
        //if (m_EnvironmentName.Contains("XANA Lobby"))
        //{
        //    if (!isBannerLoaded)
        //    {
        //        if (gameObject.activeInHierarchy)
        //            StartCoroutine(DownloadAndLoadBanner());
        //    }
        //}
        if (!string.IsNullOrEmpty(m_ThumbnailDownloadURL))//this is check if object is visible on camera then load feed or video one time
        {
            if (gameObject.activeInHierarchy)
                StartCoroutine(DownloadAndLoadFeed(worlditemcount, _loopcount));
        }
    }
    void UpdateUserCount()
    {
        joinedUserCount.text = "0";
        if (userAnalyticsHandler == null)
        {
            return;
        }
        else if (string.IsNullOrEmpty(userAnalyticsHandler.userDataString))
        {
            return;
        }
        AllWorldData allWorldData = JsonConvert.DeserializeObject<AllWorldData>(userAnalyticsHandler.userDataString);
        if (allWorldData != null && allWorldData.player_count.Length > 0)
        {
            string modifyEnityType = entityType;
            if (modifyEnityType.Contains("_"))
            {
                modifyEnityType = "USER";
            }
            if (PlayerPrefs.GetInt("ShowLiveUserCounter").Equals(1))
                joinedUserCount.transform.parent.gameObject.SetActive(true);
            else
                joinedUserCount.transform.parent.gameObject.SetActive(false);
            for (int i = 0; i < allWorldData.player_count.Length; i++)
            {
                if (allWorldData.player_count[i].world_type == modifyEnityType && allWorldData.player_count[i].world_id.ToString() == idOfObject)
                {
                    //Debug.Log("<color=green> Analytics -- Yes Matched : " + m_EnvironmentName + "</color>");
                    if (allWorldData.player_count[i].world_id == CheckServerForID()) // For Xana Lobby
                        joinedUserCount.text = allWorldData.player_count[i].count + 5 + "";
                    else
                        joinedUserCount.text = allWorldData.player_count[i].count.ToString();

                    if (allWorldData.player_count[i].count > 5)
                        joinedUserCount.transform.parent.gameObject.SetActive(true);
                    if (m_EnvironmentName.Contains("XANA Lobby") && allWorldData.player_count[i].count > 0)
                        joinedUserCount.transform.parent.gameObject.SetActive(true);
                    break;
                }
                if (CheckServerForID().ToString() == idOfObject)
                    joinedUserCount.text = "5";
                else
                    joinedUserCount.text = "0";
            }
        }
    }
    void UpdateUserCount(string UserDetails)
    {
        joinedUserCount.text = "0";
        if (string.IsNullOrEmpty(UserDetails))
        {
            return;
        }
        AllWorldData allWorldData = JsonConvert.DeserializeObject<AllWorldData>(UserDetails);
        if (allWorldData != null && allWorldData.player_count.Length > 0)
        {
            string modifyEnityType = entityType;
            if (modifyEnityType.Contains("_"))
            {
                //modifyEnityType = modifyEnityType.Split("_").First();
                modifyEnityType = "USER";
            }
            for (int i = 0; i < allWorldData.player_count.Length; i++)
            {
                if (allWorldData.player_count[i].world_type == modifyEnityType && allWorldData.player_count[i].world_id.ToString() == idOfObject)
                {
                    //Debug.Log("<color=green> Analytics -- Yes Matched : " + m_EnvironmentName + "</color>");
                    if (allWorldData.player_count[i].world_id == CheckServerForID())
                    { // For Xana Lobby
                        joinedUserCount.text = (allWorldData.player_count[i].count + 5) + "";
                    }
                    else
                        joinedUserCount.text = allWorldData.player_count[i].count.ToString();

                    if (allWorldData.player_count[i].count > 5)
                        joinedUserCount.transform.parent.gameObject.SetActive(true);
                    else if (PlayerPrefs.GetInt("ShowLiveUserCounter", 0) == 0)
                        joinedUserCount.transform.parent.gameObject.SetActive(false);

                    break;
                }
                if (CheckServerForID().ToString() == idOfObject)
                {
                    joinedUserCount.text = "5";
                }
                else
                    joinedUserCount.text = "0";
            }
        }
    }
    int CheckServerForID()
    {
        if (APIBasepointManager.instance.IsXanaLive)
            return 38; // Xana Lobby Id Mainnet
        else
            return 406; // Xana Lobby Id Testnet
    }
    public IEnumerator DownloadAndLoadFeed(int worlditemcount, int _loopcount)
    {
        if (AssetCache.Instance.HasFile(m_ThumbnailDownloadURL))
        {
            AssetCache.Instance.LoadSpriteIntoImage(worldIcon, m_ThumbnailDownloadURL, changeAspectRatio: true);
            if (PreviewLogo)
                PreviewLogo.gameObject.SetActive(false);
        }
        else
        {
            AssetCache.Instance.EnqueueOneResAndWait(m_ThumbnailDownloadURL, m_ThumbnailDownloadURL, (success) =>
            {
                if (success)
                {
                    AssetCache.Instance.LoadSpriteIntoImage(worldIcon, m_ThumbnailDownloadURL, changeAspectRatio: true);
                    isImageSuccessDownloadAndSave = true;
                    if (PreviewLogo)
                        PreviewLogo.gameObject.SetActive(false);

                }
            });
        }
        if (worlditemcount >= 26 || worlditemcount == _loopcount - 1)
        {
            /*LoadingHandler.Instance.SearchLoadingCanvas.SetActive(false);*/
            LoadingHandler.Instance.worldLoadingScreen.SetActive(false);
        }
        yield return null;
    }
    void GetEventType(string entityType)
    {
        isBuilderScene = false;
        isMuseumScene = false;
        isEnvirnomentScene = false;

        if (entityType == WorldType.MUSEUM.ToString())
        {
            isMuseumScene = true;
        }
        else if (entityType == WorldType.ENVIRONMENT.ToString())
        {
            isEnvirnomentScene = true;
        }
        else if (entityType == WorldType.USER_WORLD.ToString())
        {
            isBuilderScene = true;
            isMuseumScene = true;
        }
    }

    void SetWorldName()
    {
        if (eviroment_Name != null && joinedUserCount != null)
        {
            if (m_EnvironmentName.Contains("Dubai"))
            {
                eviroment_Name.text = "DUBAI FESTIVAL STAGE.";
                eviroment_Name.GetComponent<TextLocalization>().LocalizeTextText(eviroment_Name.text);
            }
            else
            {
                eviroment_Name.GetComponent<TextLocalization>().LocalizeTextText(m_EnvironmentName);
            }
            eviroment_Name.text = eviroment_Name.text;
            if (!string.IsNullOrEmpty(worldVisitCount))
                joinedUserCount.text = worldVisitCount + " visits";
            gameObject.GetComponent<Button>().interactable = true;
            isVisible = true;
        }
        //StartCoroutine(DownloadImage());
    }
    /*public IEnumerator DownloadImage()
    {
        if (m_EnvironmentName.Contains("Dubai"))
        {
            eviroment_Name.text = "DUBAI FESTIVAL STAGE.";
            eviroment_Name.GetComponent<TextLocalization>().LocalizeTextText(eviroment_Name.text);
        }
        else
        {
            eviroment_Name.GetComponent<TextLocalization>().LocalizeTextText(m_EnvironmentName);
        }
        eviroment_Name.text = eviroment_Name.text;
        gameObject.GetComponent<Button>().interactable = true;
        yield return null;
    }*/
    public void OnClickPrefab()
    {
        m_EnvName = m_EnvironmentName;
        m_CreaName = Creator_Name;
        ConstantsHolder.xanaConstants.builderMapID = int.Parse(idOfObject);
        ConstantsHolder.xanaConstants.IsMuseum = isMuseumScene;
        ConstantsHolder.xanaConstants.isBuilderScene = isBuilderScene;

        if (m_EnvName.Contains("RooftopParty") || m_EnvName.Contains("XanaParty"))
        {
            ConstantsHolder.xanaConstants.isXanaPartyWorld = true;
        }
        else
        {
            ConstantsHolder.xanaConstants.isXanaPartyWorld = false;
            ConstantsHolder.isSoftBankGame = false;

        }
        //if (m_EnvironmentName.Contains("XANA Lobby"))
        //{
        //    worldItemPreview.Init(XanaWorldBanner,
        //   m_EnvironmentName, m_WorldDescription, creatorName, createdAt, updatedAt, isBuilderScene, userAvatarURL, "", worldTags,
        //   entityType, Creator_Name, CreatorDescription, CreatorAvatarURL,isFavourite);
        //}
        //else
        //{
        worldItemPreview.Init(this.gameObject, worldIcon.sprite,
    m_EnvironmentName, m_WorldDescription, Creator_Name, createdAt, updatedAt, isBuilderScene, userAvatarURL, m_ThumbnailDownloadURL, worldTags,
    entityType, Creator_Name, CreatorDescription, CreatorAvatarURL, UserMicEnable, isFavourite, idOfObject);
        //}

        ConstantsHolder.xanaConstants.EnviornmentName = m_EnvironmentName;
        ConstantsHolder.xanaConstants.buttonClicked = this.gameObject;
        if (isMuseumScene)
            WorldDescriptionPopupPreview.m_MuseumIsClicked = true;
        if (m_EnvironmentName == "Xana Festival")
        {
            ConstantsHolder.userLimit = int.Parse(userLimit);
        }
        else if (m_EnvironmentName == "XANA_DUNE")
        {
            ConstantsHolder.userLimit = 1;
        }
        else
        {
            ConstantsHolder.userLimit = int.Parse(userLimit);
        }
        if (m_EnvironmentName == "ZONE-X")
            GlobalConstants.SendFirebaseEvent(GlobalConstants.FirebaseTrigger.Home_Thumbnail.ToString());
        else if (m_EnvironmentName == "D +  Infinity Labo")
            GlobalConstants.SendFirebaseEvent(GlobalConstants.FirebaseTrigger.THA_Home_Thumbnail.ToString());

        ConstantsHolder.xanaConstants.MuseumID = idOfObject;
        worldItemPreview.CallAnalytics(idOfObject, entityType);
    }
    Sprite XanaWorldBanner;
    IEnumerator DownloadAndLoadBanner()
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(m_BannerLink);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            //Debug.Log("<color = red>" + www.error + "</color>");
        }
        else
        {
            Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2());
            XanaWorldBanner = sprite;
            isBannerLoaded = true;
        }
        www.Dispose();
    }
}