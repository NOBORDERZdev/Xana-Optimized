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
using static GlobalConstants;
public class FeedEventPrefab : MonoBehaviour
{
    public static string m_EnvName;
    public static string m_CreaName;
    //public static string m_EnvDownloadLink;
    //public static string m_timestamp;
    [Header("WorldNameAndLinks")]
    public string idOfObject;
    public string m_EnvironmentName;
    public string m_WorldDescription;
    public string m_ThumbnailDownloadURL;
    public string creatorName;
    public string createdAt;
    public string userLimit;
    public string userAvatarURL;
    //public string m_FileLink;
    //public string uploadTimeStamp;
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

    //public bool isMuseum;

    [Space]
    public bool isImageSuccessDownloadAndSave = false;
    public bool isReleaseFromMemoryOrNot = false;
    public bool isOnScreen;//check object is on screen or not

    public bool isVisible = false;
    float lastUpdateCallTime;

    bool isClearAfterMemory = false;
    bool isNotLoaded = true;
    public LoginPageManager loginPageManager;
    UserAnalyticsHandler userAnalyticsHandler;
    bool isBannerLoaded = false;
    private void Awake()
    {
        loginPageManager = GetComponent<LoginPageManager>();
    }

    public void Init()
    {
        GetEventType(entityType);
        Invoke("DownloadPrefabSprite", 0.1f);
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

        userAnalyticsHandler = APIBasepointManager.instance.GetComponent<UserAnalyticsHandler>();
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
                //modifyEnityType = modifyEnityType.Split("_").First();
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
                if(CheckServerForID().ToString()== idOfObject)
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
                //modifyEnityType = modifyEnityType.Split("_").First();
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
        if (APIBasepointManager.instance.IsXanaLive)
            return 38; // Xana Lobby Id Mainnet
        else
            return 406; // Xana Lobby Id Testnet
    }
   // WaitForSeconds UpdateTime = new WaitForSeconds(0.5f);
    IEnumerator UpdateCoroutine()
    {
        while(true)
        {
         //   Debug.LogError("Running " + eviroment_Name.text);
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.4f, 0.7f));

       // Vector3 mousePosNormal = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
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
                        yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f,0.5f));
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
               // WorldManager.instance.ResourcesUnloadAssetFile();//UnloadUnusedAssets file call every 15 items.......
            }
            else if (isOnScreen && (worldIcon.sprite == null || worldIcon.sprite == dummyThumbnail))
            {
              //  Debug.LogError("here we are loading it again.");
                isNotLoaded = true;
                goto LoadFileAgain;
            }

        }
        }
        //StartCoroutine(UpdateCoroutine());
    }

   // WaitForSeconds DownloadTime = new WaitForSeconds(0.5f);

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
                        //isImageSuccessDownloadAndSave = true;
                    }
                });
            }
        }

    }
    private void OnDisable()
    {

        AssetCache.Instance.RemoveFromMemory(m_ThumbnailDownloadURL, true);
        //if (!string.IsNullOrEmpty(userAvatarURL))
        //{
        //    AssetCache.Instance.RemoveFromMemory(userAvatarURL, true);
        //}
        worldIcon.sprite = null;
        worldIcon.sprite = dummyThumbnail;
        //WorldManager.instance.ResourcesUnloadAssetFile();
        UserAnalyticsHandler.onChangeJoinUserStats -= UpdateUserCount;
        //StopCoroutine(UpdateCoroutine());
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

    public void DownloadPrefabSprite()
    {
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

    public void ReloadAfterEnable()
    {
        if (!string.IsNullOrEmpty(m_ThumbnailDownloadURL))
            StartCoroutine(DownloadImage(m_ThumbnailDownloadURL));

    }

    public void CheckForDirectoryCreation(string folderName)
    {
        if (System.IO.Directory.Exists(Application.persistentDataPath + "/MainMenuData"))
        {
            if (!System.IO.Directory.Exists(Application.persistentDataPath + "/MainMenuData/" + folderName))
            {
                System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/MainMenuData/" + folderName);
            }
            if (!System.IO.Directory.Exists(Application.persistentDataPath + "/MainMenuData/" + folderName + "/" + idOfObject))
            {
                System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/MainMenuData/" + folderName + "/" + idOfObject);
            }
        }
        else
        {
            System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/MainMenuData");
            if (!System.IO.Directory.Exists(Application.persistentDataPath + "/MainMenuData/" + folderName))
            {
                System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/MainMenuData/" + folderName);
            }
            if (!System.IO.Directory.Exists(Application.persistentDataPath + "/MainMenuData/" + folderName + "/" + idOfObject))
            {
                System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/MainMenuData/" + folderName + "/" + idOfObject);
            }
        }
    }

    string folderName;
    public IEnumerator DownloadImage(string l_imgUrl)
    {
        if (isBuilderScene)
        {
            folderName = "BuilderData";
        }
        else if (isMuseumScene)
        {
            folderName = "MuseumData";
        }
        else
        {
            folderName = "EnvData";
        }

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


    IEnumerator DownloadTexture(string folderName, string l_imgUrl, Action<bool, byte[]> callBack)
    {
        if (string.IsNullOrEmpty(updatedAt))
            updatedAt = "00";
        string nameOfFile = updatedAt.GetHashCode().ToString();

        if (File.Exists(Application.persistentDataPath + "/MainMenuData/" + folderName + "/" + idOfObject + "/" + nameOfFile + ".txt"))
        {
            byte[] data = File.ReadAllBytes(Application.persistentDataPath + "/MainMenuData/" + folderName + "/" + idOfObject + "/" + "thumbnail.jpg");
            callBack(true, data);
        }
        else
        {
            DeleteOldData(Application.persistentDataPath + "/MainMenuData/" + folderName + "/" + idOfObject);

            using (UnityWebRequest www = UnityWebRequest.Get(l_imgUrl))
            {
                var operation = www.SendWebRequest();
                while (!operation.isDone)
                {
                    yield return null;
                }
                if (www.isHttpError || www.isNetworkError)
                {
                    callBack(false, null);
                    Debug.Log("Network Error");
                }
                else
                {
                    byte[] data = www.downloadHandler.data;
                    callBack(true, data);
                    File.WriteAllBytesAsync(Application.persistentDataPath + "/MainMenuData/" + folderName + "/" + idOfObject + "/" + "thumbnail.jpg", data, System.Threading.CancellationToken.None);
                    File.WriteAllTextAsync(Application.persistentDataPath + "/MainMenuData/" + folderName + "/" + idOfObject + "/" + nameOfFile + ".txt", updatedAt);
                }
                www.Dispose();
            }
        }
        yield return new WaitForEndOfFrame();
    }

    void DeleteOldData(string filePath)
    {
        try
        {
            foreach (string s in Directory.GetFiles(filePath))
            {
                File.Delete(s);
            }

        }
        catch (FileNotFoundException e)
        {
            Debug.Log("<color = red>" + e.Message + "</color>");
        }
    }

    public void UpdateWorldPanel()
    {
        if (!m_EnvironmentName.Contains("XANA Lobby"))
        {
            m_BannerSprite[0].sprite = m_FadeImage.sprite;
        }

        m_BannerSprite[1].sprite = m_FadeImage.sprite;
        if(m_BannerSprite.Length>2)
        m_BannerSprite[2].sprite = m_FadeImage.sprite;
    }

    public void OnClickPrefab()
    {
        //m_EnvDownloadLink = m_FileLink;
        ScrollController.transform.parent.GetComponent<ScrollActivity>().enabled = false;
        m_EnvName = m_EnvironmentName;
        m_CreaName = creatorName;
        ConstantsHolder.xanaConstants.builderMapID = int.Parse(idOfObject);
        ConstantsHolder.xanaConstants.IsMuseum = isMuseumScene;
        ConstantsHolder.xanaConstants.isBuilderScene = isBuilderScene;
        ScrollController.verticalNormalizedPosition = 1f;
        //m_WorldDescriptionParser = m_WorldDescription;
        if (userProfile.sprite == null)
            StartCoroutine( UpdateUserProfile());
        //m_timestamp = uploadTimeStamp;

        if (!tagsInstantiated)
            InstantiateWorldtags();

        loginPageManager.SetPanelToBottom();
        ConstantsHolder.xanaConstants.EnviornmentName = m_EnvironmentName;
        //ConstantsHolder.xanaConstants.museumDownloadLink = m_EnvDownloadLink;
        ConstantsHolder.xanaConstants.buttonClicked = this.gameObject;
        if (isMuseumScene)
            LoginPageManager.m_MuseumIsClicked = true;


        m_WorldName.GetComponent<TextLocalization>().LocalizeTextText(m_EnvironmentName);
        m_WorldNameTH.GetComponent<TextLocalization>().LocalizeTextText(m_EnvName);
        m_WorldDescriptionTxt.GetComponent<TextLocalization>().LocalizeTextText(m_WorldDescription);
        if (m_EnvironmentName == "Xana Festival")
        {
            ConstantsHolder.userLimit = (Convert.ToInt32(userLimit));
        }
        else
        {
            ConstantsHolder.userLimit = int.Parse(userLimit);
        }
        //tempWorldName = m_WorldName.text.ToString();
        ConstantsHolder.xanaConstants.MuseumID = idOfObject;
        //SetStringSize();

        // For Analitics & User Count
        UserAnalyticsHandler.onGetWorldId?.Invoke(int.Parse(idOfObject), entityType);
        //UserAnalyticsHandler.onGetSingleWorldStats?.Invoke(int.Parse(idOfObject), entityType, visitCount); // Due to Flow change this API in not in use

        if (m_EnvironmentName == "ZONE-X")
            SendFirebaseEvent(FirebaseTrigger.Home_Thumbnail.ToString());
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
            if (!worldTags[i].IsNullOrEmpty())
            {
                GameObject temp = Instantiate(tagsPrefab, tagsParent);
                temp.GetComponent<TagPrefabInfo>().tagName.text = worldTags[i];
                temp.GetComponent<TagPrefabInfo>().tagNameHighlighter.text = worldTags[i];
                //temp.GetComponent<TagPrefabInfo>().descriptionPanel = descriptionPanelParent;
            }
        }
        tagsInstantiated = true;
    }

}