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

public class FeedEventPrefab : MonoBehaviour
{
    public static string m_EnvName;
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

    [Header("WorldNameAndDescription")]
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
    bool isBannerLoaded =false; 
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
        UpdateUserCount();
    }

    void UpdateUserCount(string UserDetails)
    {
        //Debug.Log("Yes, TriggerData " + UserDetails);
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
                    joinedUserCount.text = allWorldData.player_count[i].count.ToString();

                    if (allWorldData.player_count[i].count >= 5)
                        joinedUserCount.transform.parent.gameObject.SetActive(true);
                    else if (PlayerPrefs.GetInt("ShowLiveUserCounter", 0) == 0)
                        joinedUserCount.transform.parent.gameObject.SetActive(false);

                    break;
                }

                joinedUserCount.text = "0";
            }
        }
    }
    void UpdateUserCount()
    {
        //Debug.Log("Yes, Init " + userAnalyticsHandler.userDataString);
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
                    //Debug.Log("Yes Matched : " + m_EnvironmentName);
                    Debug.Log("<color=green> Analytics -- Yes Matched : " + m_EnvironmentName + "</color>");
                    joinedUserCount.text = allWorldData.player_count[i].count.ToString();

                    if (allWorldData.player_count[i].count >= 5)
                        joinedUserCount.transform.parent.gameObject.SetActive(true);
                    else if (PlayerPrefs.GetInt("ShowLiveUserCounter", 0) == 0)
                        joinedUserCount.transform.parent.gameObject.SetActive(false);

                    break;
                }
                joinedUserCount.text = "0";
            }
        }
    }
    private void Update()//delete image after object out of screen
    {
        lastUpdateCallTime += Time.deltaTime;
        if (lastUpdateCallTime > 0.3f)//call every 0.4 sec
        {
            Vector3 mousePosNormal = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
            Vector3 mousePosNR = Camera.main.ScreenToViewportPoint(mousePosNormal);

            if (mousePosNR.y >= -0.1f && mousePosNR.y <= 1.1f)
            {
                isOnScreen = true;
            }
            else
            {
                isOnScreen = false;
            }

            lastUpdateCallTime = 0;
        }

        if (isVisible && isOnScreen && !string.IsNullOrEmpty(m_ThumbnailDownloadURL))//this is check if object is visible on camera then load feed or video one time
        {
            isVisible = false;
            //Debug.Log("Image download starting one time");
            DownloadAndLoadFeed();
            if (!string.IsNullOrEmpty(creatorName))
            {
                if (!creatorName.Equals("XANA"))
                    UpdateUserProfile();
            }
            //Debug.Log("2");
        }
        else if (isImageSuccessDownloadAndSave)
        {
            //Debug.Log("0");
            LoadFileAgain:
            if (isOnScreen && isNotLoaded)
            {
                //Debug.Log("01");
                if (!string.IsNullOrEmpty(m_ThumbnailDownloadURL))
                {
                    //Debug.Log("02"); 
                    if (AssetCache.Instance.HasFile(m_ThumbnailDownloadURL))
                    {
                        //Debug.Log("03");
                        isNotLoaded = false;
                        AssetCache.Instance.LoadSpriteIntoImage(worldIcon, m_ThumbnailDownloadURL, changeAspectRatio: true);
                    }
                }
                if (!string.IsNullOrEmpty(userAvatarURL))
                {
                    if (AssetCache.Instance.HasFile(userAvatarURL))
                    {
                        AssetCache.Instance.LoadSpriteIntoImage(userProfile, userAvatarURL, changeAspectRatio: true);
                    }
                }
            }
            else if (!isOnScreen && worldIcon.sprite && !isNotLoaded)
            {
                //Debug.Log("1");
                //realse from memory 
                isReleaseFromMemoryOrNot = true;
                isNotLoaded = true;
                //Debug.Log("remove from memory");
                AssetCache.Instance.RemoveFromMemory(m_ThumbnailDownloadURL, true);
                if (!string.IsNullOrEmpty(userAvatarURL))
                {
                    AssetCache.Instance.RemoveFromMemory(userAvatarURL, true);
                }
                worldIcon.sprite = null;
                worldIcon.sprite = dummyThumbnail;
                WorldManager.instance.ResourcesUnloadAssetFile();//UnloadUnusedAssets file call every 15 items.......

            }
            else if (isOnScreen && (worldIcon.sprite == null || worldIcon.sprite == dummyThumbnail))
            {
                //Debug.Log("here we are loading it again.");
                isNotLoaded = true;
                goto LoadFileAgain;
            }
        }
    }


    //private void LateUpdate()
    //{
    //    if(worldIcon.sprite==null)
    //    {
    //        isNotLoaded = true;
    //    }
    //}
    public void DownloadAndLoadFeed()
    {
        AssetCache.Instance.EnqueueOneResAndWait(m_ThumbnailDownloadURL, m_ThumbnailDownloadURL, (success) =>
        {
            if (success)
            {
                AssetCache.Instance.LoadSpriteIntoImage(worldIcon, m_ThumbnailDownloadURL, changeAspectRatio: true);
                isImageSuccessDownloadAndSave = true;
            }
        });
    }
    public void UpdateUserProfile()
    {
        if (!string.IsNullOrEmpty(userAvatarURL))
        {
            //Debug.Log("02"); 
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
    private void OnAnimatorIK(int layerIndex)
    {

    }

    private void OnDisable()
    {
        AssetCache.Instance.RemoveFromMemory(m_ThumbnailDownloadURL, true);
        if (!string.IsNullOrEmpty(userAvatarURL))
        {
            AssetCache.Instance.RemoveFromMemory(userAvatarURL, true);
        }
        worldIcon.sprite = null;
        worldIcon.sprite = dummyThumbnail;
        WorldManager.instance.ResourcesUnloadAssetFile();
        UserAnalyticsHandler.onChangeJoinUserStats -= UpdateUserCount;
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
        eviroment_Name.text = eviroment_Name.text.ToUpper();
        gameObject.GetComponent<Button>().interactable = true;
        UpdateWorldPanel();


        //CheckForDirectoryCreation(folderName);

        //StartCoroutine(DownloadTexture(folderName, l_imgUrl, (downloadedSucessfully, ImageRawData) =>
        //{
        //    if (downloadedSucessfully)
        //    {
        //        Texture2D texture = new Texture2D(1, 1);

        //        texture.LoadImage(ImageRawData);
        //        Sprite l_sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(1f, 1f));
        //        worldIcon.sprite = l_sprite;
        //        m_FadeImage.sprite = l_sprite;
        //        //BY Abdullah :Changing Xana Festival stage in Dubai to Xana Festival Stage 
        //        if (m_EnvironmentName.Contains("Dubai"))
        //        {
        //            eviroment_Name.text = "DUBAI FESTIVAL STAGE.";
        //            eviroment_Name.GetComponent<TextLocalization>().LocalizeTextText(eviroment_Name.text);
        //        }
        //        else
        //        {
        //            eviroment_Name.GetComponent<TextLocalization>().LocalizeTextText(m_EnvironmentName);
        //        }
        //        eviroment_Name.text = eviroment_Name.text.ToUpper();
        //        gameObject.GetComponent<Button>().interactable = true;
        //        UpdateWorldPanel();
        //    }
        //}));
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
            //if (!isBannerLoaded)
            //{
            //    StartCoroutine(DownloadAndLoadBanner());
            //}
        }
        else
        {
            
        }
        m_BannerSprite[1].sprite = m_FadeImage.sprite;
        m_BannerSprite[2].sprite = m_FadeImage.sprite;
    }
    //private int PreferdStringSize = 22;

    //int textWidth;

    //public void SetStringSize()
    //{

    //    textWidth = tempWorldName.Length;

    //    if (textWidth > PreferdStringSize)
    //    {

    //        tempWorldName = tempWorldName.Remove(PreferdStringSize);

    //        tempWorldName = string.Concat(tempWorldName, "...");


    //        m_WorldName.text = tempWorldName;

    //    }

    //}

    //string tempWorldName;

    public void OnClickPrefab()
    {
        //m_EnvDownloadLink = m_FileLink;
        ScrollController.transform.parent.GetComponent<ScrollActivity>().enabled = false;
        m_EnvName = m_EnvironmentName;
        XanaConstants.xanaConstants.builderMapID = int.Parse(idOfObject);
        XanaConstants.xanaConstants.IsMuseum = isMuseumScene;
        XanaConstants.xanaConstants.isBuilderScene = isBuilderScene;
        Launcher.sceneName = m_EnvName;
        ScrollController.verticalNormalizedPosition = 1f;
        //m_WorldDescriptionParser = m_WorldDescription;
        if (userProfile.sprite == null)
            UpdateUserProfile();
        //m_timestamp = uploadTimeStamp;
        loginPageManager.SetPanelToBottom();
        XanaConstants.xanaConstants.EnviornmentName = m_EnvironmentName;
        //XanaConstants.xanaConstants.museumDownloadLink = m_EnvDownloadLink;
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
        //tempWorldName = m_WorldName.text.ToString();
        XanaConstants.xanaConstants.MuseumID = idOfObject;
        //SetStringSize();

        //if (m_EnvName.Contains("GOZ : Animator Haruna Gouzu Gallery 2021"))
        //{

        //  m_WorldName.GetComponent<TextLocalization>().LocalizeTextText("GOZANIMATOR HARUNA ...");

        //}


        //m_SetPressedIndex = m_PressedIndex;
        //if (!isMuseumScene)
        //{
        //    if (m_EnvName.Contains("Crypto Ninja village"))
        //    {
        //        creator_Name.text = "Metaverse Ninja";
        //    }

        //}
        //else if (isMuseumScene)
        //{
        //    if (m_EnvName.Contains("THE RHETORIC STAR"))
        //    {
        //        creator_Name.text = "World Name";
        //        creator_Name.GetComponent<TextLocalization>().LocalizeTextText(creator_Name.text);
        //    }
        //}
        //else
        //{
        //    if (m_EnvironmentName.Contains("ROCK’N"))
        //    {
        //        PlayerPrefs.SetString("ScenetoLoad", "GekkoSan");
        //    }
        //    else if (m_EnvironmentName.Contains("Gouzu Gallarey"))
        //    {
        //        PlayerPrefs.SetString("ScenetoLoad", "GOZMuseum");
        //    }
        //    else if (m_EnvironmentName.Contains("Aurora Art"))
        //    {
        //        PlayerPrefs.SetString("ScenetoLoad", "Aurora");
        //    }
        //    else if (m_EnvironmentName.Contains("Hokusai"))
        //    {
        //        PlayerPrefs.SetString("ScenetoLoad", "Hokusai");
        //    }
        //    else if (m_EnvironmentName.Contains("Yukinori"))
        //    {
        //        PlayerPrefs.SetString("ScenetoLoad", "Yukinori");
        //    }
        //    else if (m_EnvironmentName.Contains("NFT Museum"))
        //    {
        //        PlayerPrefs.SetString("ScenetoLoad", "THE RHETORIC STAR");
        //    }
        //}

        // For Analitics & User Count
        UserAnalyticsHandler.onGetWorldId?.Invoke(int.Parse(idOfObject), entityType);
        UserAnalyticsHandler.onGetSingleWorldStats?.Invoke(int.Parse(idOfObject), entityType, visitCount);
    }

    //private void OnValidate()
    //{
    //    UpdeteUserCount();
    //}

    Sprite BannerSprite;
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
            isBannerLoaded= true;
        }

    }

}