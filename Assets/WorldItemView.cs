using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using SuperStar.Helpers;
using UnityEngine.Networking;
using Photon.Pun.Demo.PunBasics;

public class WorldItemView : MonoBehaviour
{
    [SerializeField] private DynamicScrollRect.DynamicScrollRect _dynamicScroll = null;
    public int Index;
    public Vector2 GridIndex { get; protected set; }
    public RectTransform RectTransform => transform as RectTransform;
    public void Activated()
    {
        gameObject.SetActive(true);
    }
    public void Deactivated()
    {
        gameObject.SetActive(false);
    }
    public void InitItem(int index, Vector2 gridPos, WorldItemDetail detail)
    {
       Index = index;
       GridIndex = gridPos;
       idOfObject =  detail.IdOfWorld;
       m_EnvironmentName = detail.EnvironmentName;
       m_WorldDescription = detail.WorldDescription;
       m_ThumbnailDownloadURL = detail.ThumbnailDownloadURL;
       creatorName = detail.CreatorName;
       createdAt = detail.CreatedAt;
       userLimit = detail.UserLimit;
       userAvatarURL = detail.UserAvatarURL;
       updatedAt = detail.UpdatedAt;
       entityType = detail.EntityType;
       m_BannerLink = detail.BannerLink;
       m_PressedIndex = detail.PressedIndex;
     //  worldTags = detail.WorldTags;
       Init();
    }

    public static string m_EnvName;
    public static string m_CreaName;
    [Header("WorldNameAndLinks")]
    public string idOfObject;
    public string m_EnvironmentName;
    public string m_WorldDescription;
    public string m_ThumbnailDownloadURL;
    public string creatorName;
    public string createdAt;
    public string userLimit;
    public string userAvatarURL;
    public string updatedAt = "00";
    public string entityType = "None";
    [Header("WorldNameAndDescription")]
    public TextMeshProUGUI eviroment_Name;
    public TextMeshProUGUI joinedUserCount;
    public string m_BannerLink;
    public Image worldIcon;
    public int m_PressedIndex;
    public bool isMuseumScene = false;
    public bool isBuilderScene = false;
    public bool isEnvirnomentScene = false;
    public bool isImageSuccessDownloadAndSave = false;
    public bool isReleaseFromMemoryOrNot = false;
    public bool isOnScreen;//check object is on screen or not
    public bool isVisible = false;
    bool isNotLoaded = true;
    public WorldItemPreviewTab worldItemPreview;
    UserAnalyticsHandler userAnalyticsHandler;
    bool isBannerLoaded = false;
    private void OnEnable()
    {
        //if (cnt > 0 && !isImageSuccessDownloadAndSave)
        //{
        //    isVisible = true;
        //}
        //cnt += 1;
        UserAnalyticsHandler.onChangeJoinUserStats += UpdateUserCount;
       // StartCoroutine(UpdateCoroutine());
        UpdateUserCount();



        ////////
        ///
    
        /////
    }
    private void OnDisable()
    {
        AssetCache.Instance.RemoveFromMemoryDelayCoroutine(m_ThumbnailDownloadURL, true);
        worldIcon.sprite = null;
        worldIcon.sprite = default;
        WorldManager.instance.ResourcesUnloadAssetFile();
        UserAnalyticsHandler.onChangeJoinUserStats -= UpdateUserCount;
       // StopAllCoroutines();
    }
    public void Init()
    {
        GetEventType(entityType);
        StartCoroutine(DownloadPrefabSprite());
        this.GetComponent<Button>().interactable = false;
        userAnalyticsHandler = APIBaseUrlChange.instance.GetComponent<UserAnalyticsHandler>();
        UpdateUserCount();
        if (m_EnvironmentName.Contains("XANA Lobby"))
        {
            if (!isBannerLoaded)
            {
                StartCoroutine(DownloadAndLoadBanner());
            }
        }
        if (!string.IsNullOrEmpty(m_ThumbnailDownloadURL))//this is check if object is visible on camera then load feed or video one time
        {
            StartCoroutine(DownloadAndLoadFeed());
        }
    }
    int cnt = 0;
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
                modifyEnityType = "USER";
            }
            for (int i = 0; i < allWorldData.player_count.Length; i++)
            {
                if (allWorldData.player_count[i].world_type == modifyEnityType && allWorldData.player_count[i].world_id.ToString() == idOfObject)
                {
                    Debug.Log("<color=green> Analytics -- Yes Matched : " + m_EnvironmentName + "</color>");
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

            for (int i = 0; i < allWorldData.player_count.Length; i++)
            {
                if (allWorldData.player_count[i].world_type == modifyEnityType && allWorldData.player_count[i].world_id.ToString() == idOfObject)
                {
                    Debug.Log("<color=green> Analytics -- Yes Matched : " + m_EnvironmentName + "</color>");
                    if (allWorldData.player_count[i].world_id == CheckServerForID()) // For Xana Lobby
                        joinedUserCount.text = (allWorldData.player_count[i].count + 5) + "";
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
        if (APIBaseUrlChange.instance.IsXanaLive)
            return 38; // Xana Lobby Id Mainnet
        else
            return 406; // Xana Lobby Id Testnet
    }
    //IEnumerator UpdateCoroutine()
    //{
    //    while (true)
    //    {
    //        yield return new WaitForSeconds(UnityEngine.Random.Range(0.4f, 0.7f));
    //        isOnScreen = true;
    //        if (isVisible && isOnScreen && !string.IsNullOrEmpty(m_ThumbnailDownloadURL))//this is check if object is visible on camera then load feed or video one time
    //        {
    //            isVisible = false;
    //            StartCoroutine(DownloadAndLoadFeed());
    //        }
    //        else if (isImageSuccessDownloadAndSave)
    //        {
    //        LoadFileAgain:
    //            if (isOnScreen && isNotLoaded)
    //            {
    //                if (!string.IsNullOrEmpty(m_ThumbnailDownloadURL))
    //                {
    //                    if (AssetCache.Instance.HasFile(m_ThumbnailDownloadURL))
    //                    {
    //                        isNotLoaded = false;
    //                        yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.5f));
    //                        AssetCache.Instance.LoadSpriteIntoImage(worldIcon, m_ThumbnailDownloadURL, changeAspectRatio: true);
    //                    }
    //                }
    //            }
    //            else if (!isOnScreen && worldIcon.sprite && !isNotLoaded)
    //            {
    //                //realse from memory 
    //                isReleaseFromMemoryOrNot = true;
    //                isNotLoaded = true;
    //                yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.5f));
    //                AssetCache.Instance.RemoveFromMemory(m_ThumbnailDownloadURL, true);
    //                worldIcon.sprite = default;
    //                WorldManager.instance.ResourcesUnloadAssetFile();//UnloadUnusedAssets file call every 15 items.......
    //            }
    //            else if (isOnScreen && (worldIcon.sprite == null || worldIcon.sprite == default))
    //            {
    //                isNotLoaded = true;
    //                goto LoadFileAgain;
    //            }

    //        }
    //    }
    //}
    public IEnumerator DownloadAndLoadFeed()
    {
        yield return null;
        AssetCache.Instance.EnqueueOneResAndWait(m_ThumbnailDownloadURL, m_ThumbnailDownloadURL, (success) =>
        {
            if (success)
            {
                AssetCache.Instance.LoadSpriteIntoImage(worldIcon, m_ThumbnailDownloadURL, changeAspectRatio: true);
                isImageSuccessDownloadAndSave = true;
            }
        });
    }
    void GetEventType(string entityType)
    {
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

    public IEnumerator DownloadPrefabSprite()
    {
        yield return new WaitForSeconds(0.1f);

        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(DownloadImage());
        }
        isVisible = true;
    }
    public IEnumerator DownloadImage()
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
    }
    public void OnClickPrefab()/// on button click
    {
        m_EnvName = m_EnvironmentName;
        m_CreaName = creatorName;
        XanaConstants.xanaConstants.builderMapID = int.Parse(idOfObject);
        XanaConstants.xanaConstants.IsMuseum = isMuseumScene;
        XanaConstants.xanaConstants.isBuilderScene = isBuilderScene;
        Launcher.sceneName = m_EnvName;
 
        worldItemPreview.Init(m_EnvironmentName.Contains("XANA Lobby") ? XanaWorldBanner: worldIcon.sprite,
            m_EnvironmentName, m_WorldDescription, creatorName, createdAt, updatedAt, isBuilderScene, userAvatarURL);

        XanaConstants.xanaConstants.EnviornmentName = m_EnvironmentName;
        XanaConstants.xanaConstants.buttonClicked = this.gameObject;
        if (isMuseumScene)
            WorldItemPreviewTab.m_MuseumIsClicked = true;
        if (m_EnvironmentName == "Xana Festival")
        {
            XanaConstants.xanaConstants.userLimit = (Convert.ToInt32(userLimit) /*- 1*/).ToString();
        }
        else
        {
            XanaConstants.xanaConstants.userLimit = userLimit;
        }
        XanaConstants.xanaConstants.MuseumID = idOfObject;
        worldItemPreview.CallAnalytics(idOfObject, entityType);
    }
    Sprite XanaWorldBanner;
    IEnumerator DownloadAndLoadBanner()
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(m_BannerLink);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log("<color = red>" + www.error + "</color>");
        }
        else
        {
            Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2());
            XanaWorldBanner = sprite;
            isBannerLoaded = true;
        }
    }
}