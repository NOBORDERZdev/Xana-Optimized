using System;
using System.IO;
using System.Linq;
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
    /// <summary>
    [SerializeField] private DynamicScrollRect.DynamicScrollRect _dynamicScroll = null;
    [SerializeField] private TextMeshProUGUI _text = null;
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
       // _text.SetText(Index.ToString());
        GridIndex = gridPos;
      idOfObject =  detail.IdOfWorld;
        m_EnvironmentName= detail.EnvironmentName;
        m_WorldDescription = detail.WorldDescription;
         m_ThumbnailDownloadURL= detail.ThumbnailDownloadURL;
        creatorName= detail.CreatorName;
       createdAt= detail.CreatedAt;
       userLimit= detail.UserLimit;
       userAvatarURL= detail.UserAvatarURL;
       updatedAt= detail.UpdatedAt;
       entityType= detail.EntityType;
       m_BannerLink= detail.BannerLink;
       m_PressedIndex= detail.PressedIndex;
        worldTags = detail.WorldTags;
        Init();
}
    /// </summary>





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
    [Header("Tags and Category")]
    public GameObject tagScroller;
    public Transform tagsParent;
    public GameObject tagsPrefab;
    public string[] worldTags;
    public bool tagsInstantiated;
    [Header("WorldNameAndDescription")]
    public GameObject descriptionPanelParent;
    public TextMeshProUGUI m_WorldName;
    public Text m_WorldNameTH;
    public TextMeshProUGUI m_WorldDescriptionTxt;
    public TextMeshProUGUI eviroment_Name;
    public Text creatorNameText;
    public Text createdAtText;
    public Text updatedAtText;
    public Text visitCount;
    public TextMeshProUGUI joinedUserCount;
    public string m_BannerLink;
    public Image[] m_BannerSprite;
    public Sprite dummyThumbnail;
    [Header("Images")]
    public Image m_FadeImage;
    public Image worldIcon;
    public Image userProfile;
    public ScrollRect ScrollController;
    public GameObject XanaProfile;
    public Button m_JoinEventBtn;
    public int m_PressedIndex;
    public bool isMuseumScene = false;
    public bool isBuilderScene = false;
    public bool isEnvirnomentScene = false;
    [Space]
    public bool isImageSuccessDownloadAndSave = false;
    public bool isReleaseFromMemoryOrNot = false;
    public bool isOnScreen;//check object is on screen or not
    public bool isVisible = false;
    bool isNotLoaded = true;
    public LoginPageManager loginPageManager;
    UserAnalyticsHandler userAnalyticsHandler;
    bool isBannerLoaded = false;
    public void Init()
    {
        GetEventType(entityType);
        StartCoroutine(DownloadPrefabSprite());
        this.GetComponent<Button>().interactable = false;

        creatorNameText.text = creatorName;
        createdAtText.text = " : " + createdAt.Substring(0, 10);
        updatedAtText.text = " : " + updatedAt.Substring(0, 10);
        if (!string.IsNullOrEmpty(creatorName))
        {
            if (creatorName.Equals("XANA"))
            {
                XanaProfile.SetActive(true);
                userProfile.transform.parent.gameObject.SetActive(false);
            }
        }

        userAnalyticsHandler = APIBaseUrlChange.instance.GetComponent<UserAnalyticsHandler>();
        UpdateUserCount();
        if (m_EnvironmentName.Contains("XANA Lobby"))
        {
            if (!isBannerLoaded)
            {
                StartCoroutine(DownloadAndLoadBanner());
            }
        }
    }
    int cnt = 0;
    private void OnEnable()
    {
        if (cnt > 0 && !isImageSuccessDownloadAndSave)
        {
            isVisible = true;
        }
        cnt += 1;

        UserAnalyticsHandler.onChangeJoinUserStats += UpdateUserCount;
        StartCoroutine(UpdateCoroutine());
        UpdateUserCount();
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
    IEnumerator UpdateCoroutine()
    {
        while (true)
        {
            //   Debug.LogError("Running " + eviroment_Name.text);
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.4f, 0.7f));

            Vector3 mousePosNR = Camera.main.ScreenToViewportPoint(
                new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z));

            isOnScreen = mousePosNR.y >= -3f && mousePosNR.y <= 3f ? true : false;

            if (isVisible && isOnScreen && !string.IsNullOrEmpty(m_ThumbnailDownloadURL))//this is check if object is visible on camera then load feed or video one time
            {
                isVisible = false;
                StartCoroutine(DownloadAndLoadFeed());
                if (!string.IsNullOrEmpty(creatorName) && userProfile.gameObject.activeInHierarchy)
                {
                    if (!creatorName.Equals("XANA"))
                        StartCoroutine(UpdateUserProfile());
                }
            }
            else if (isImageSuccessDownloadAndSave)
            {
            LoadFileAgain:
                if (isOnScreen && isNotLoaded)
                {
                    if (!string.IsNullOrEmpty(m_ThumbnailDownloadURL))
                    {
                        if (AssetCache.Instance.HasFile(m_ThumbnailDownloadURL))
                        {
                            isNotLoaded = false;
                            yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.5f));
                            AssetCache.Instance.LoadSpriteIntoImage(worldIcon, m_ThumbnailDownloadURL, changeAspectRatio: true);
                        }
                    }
                    if (!string.IsNullOrEmpty(userAvatarURL) && userProfile.gameObject.activeInHierarchy)
                    {
                        if (AssetCache.Instance.HasFile(userAvatarURL))
                        {
                            yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.5f));
                            AssetCache.Instance.LoadSpriteIntoImage(userProfile, userAvatarURL, changeAspectRatio: true);
                        }
                    }
                }
                else if (!isOnScreen && worldIcon.sprite && !isNotLoaded)
                {
                    //realse from memory 
                    isReleaseFromMemoryOrNot = true;
                    isNotLoaded = true;
                    yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.5f));
                    AssetCache.Instance.RemoveFromMemory(m_ThumbnailDownloadURL, true);
                    worldIcon.sprite = null;
                    worldIcon.sprite = dummyThumbnail;
                    WorldManager.instance.ResourcesUnloadAssetFile();//UnloadUnusedAssets file call every 15 items.......
                }
                else if (isOnScreen && (worldIcon.sprite == null || worldIcon.sprite == dummyThumbnail))
                {
                    //  Debug.LogError("here we are loading it again.");
                    isNotLoaded = true;
                    goto LoadFileAgain;
                }

            }
        }
    }
    public IEnumerator DownloadAndLoadFeed()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.6f));
        AssetCache.Instance.EnqueueOneResAndWait(m_ThumbnailDownloadURL, m_ThumbnailDownloadURL, (success) =>
        {
            if (success)
            {
                AssetCache.Instance.LoadSpriteIntoImage(worldIcon, m_ThumbnailDownloadURL, changeAspectRatio: true);
                isImageSuccessDownloadAndSave = true;
            }
        });
    }
    IEnumerator UpdateUserProfile()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.6f));

        if (!string.IsNullOrEmpty(userAvatarURL))
        {
            if (AssetCache.Instance.HasFile(userAvatarURL))
            {
                AssetCache.Instance.LoadSpriteIntoImage(userProfile, userAvatarURL, changeAspectRatio: true);
            }
            else
            {
                AssetCache.Instance.EnqueueOneResAndWait(userAvatarURL, userAvatarURL, (success) =>
                {
                    if (success)
                    {
                        AssetCache.Instance.LoadSpriteIntoImage(userProfile, userAvatarURL, changeAspectRatio: true);
                    }
                });
            }
        }

    }
    private void OnDisable()
    {
        AssetCache.Instance.RemoveFromMemory(m_ThumbnailDownloadURL, true);
        worldIcon.sprite = null;
        worldIcon.sprite = dummyThumbnail;
        WorldManager.instance.ResourcesUnloadAssetFile();
        UserAnalyticsHandler.onChangeJoinUserStats -= UpdateUserCount;
        StopAllCoroutines();
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
            StartCoroutine(DownloadImage(m_ThumbnailDownloadURL));
        }

        if (isBuilderScene)
            m_JoinEventBtn.onClick.AddListener(() => WorldManager.instance.JoinBuilderWorld());
        else
            m_JoinEventBtn.onClick.AddListener(() => WorldManager.instance.JoinEvent());

        isVisible = true;
    }
    public IEnumerator DownloadImage(string l_imgUrl)
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
        UpdateWorldPanel();

        yield return null;
    }
    public void UpdateWorldPanel()
    {
        if (!m_EnvironmentName.Contains("XANA Lobby"))
        {
            m_BannerSprite[0].sprite = m_FadeImage.sprite;
        }

        m_BannerSprite[1].sprite = m_FadeImage.sprite;
        if (m_BannerSprite.Length > 2)
            m_BannerSprite[2].sprite = m_FadeImage.sprite;
    }

    public void OnClickPrefab()
    {
        ScrollController.transform.parent.GetComponent<ScrollActivity>().enabled = false;
        m_EnvName = m_EnvironmentName;
        m_CreaName = creatorName;
        XanaConstants.xanaConstants.builderMapID = int.Parse(idOfObject);
        XanaConstants.xanaConstants.IsMuseum = isMuseumScene;
        XanaConstants.xanaConstants.isBuilderScene = isBuilderScene;
        Launcher.sceneName = m_EnvName;
        ScrollController.verticalNormalizedPosition = 1f;
        if (userProfile.sprite == null)
            StartCoroutine(UpdateUserProfile());
        if (!tagsInstantiated)
            InstantiateWorldtags();

        loginPageManager.SetPanelToBottom();
        XanaConstants.xanaConstants.EnviornmentName = m_EnvironmentName;
        XanaConstants.xanaConstants.buttonClicked = this.gameObject;
        if (isMuseumScene)
            LoginPageManager.m_MuseumIsClicked = true;


        m_WorldName.GetComponent<TextLocalization>().LocalizeTextText(m_EnvironmentName);
        m_WorldNameTH.GetComponent<TextLocalization>().LocalizeTextText(m_EnvName);
        m_WorldDescriptionTxt.GetComponent<TextLocalization>().LocalizeTextText(m_WorldDescription);
        if (m_EnvironmentName == "Xana Festival")
        {
            XanaConstants.xanaConstants.userLimit = (Convert.ToInt32(userLimit) /*- 1*/).ToString();
        }
        else
        {
            XanaConstants.xanaConstants.userLimit = userLimit;
        }
        XanaConstants.xanaConstants.MuseumID = idOfObject;
        // For Analitics & User Count
        UserAnalyticsHandler.onGetWorldId?.Invoke(int.Parse(idOfObject), entityType);
        UserAnalyticsHandler.onGetSingleWorldStats?.Invoke(int.Parse(idOfObject), entityType, visitCount);
    }


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
            m_BannerSprite[0].sprite = sprite;
            isBannerLoaded = true;
        }

    }


    void InstantiateWorldtags()
    {
        if (worldTags.Length > 0)
            tagScroller.SetActive(true);
        else
            return;

        if (tagsParent.transform.childCount > 0)
        {
            foreach (Transform t in tagsParent)
                Destroy(t.gameObject);
        }

        for (int i = 0; i < worldTags.Length; i++)
        {
            GameObject temp = Instantiate(tagsPrefab, tagsParent);
            temp.GetComponent<TagPrefabInfo>().tagName.text = worldTags[i];
            temp.GetComponent<TagPrefabInfo>().tagNameHighlighter.text = worldTags[i];
            temp.GetComponent<TagPrefabInfo>().descriptionPanel = descriptionPanelParent;
        }
        tagsInstantiated = true;
    }
}
