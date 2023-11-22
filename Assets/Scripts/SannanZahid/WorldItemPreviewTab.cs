using SuperStar.Helpers;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldItemPreviewTab : MonoBehaviour
{
    public TextMeshProUGUI WorldNameTxt;
    public TextMeshProUGUI WorldDescriptionTxt;
    public Text CreatorNameTxt;
    public Text CreatedAtTxt;
    public Text UpdatedAtTxt;
    public Text VisitCountTxt;
    [Header("Images")]
    public Image FadeImg;
    public Image WorldIconImg;
    public Image UserProfileImg;
    public ScrollRect ScrollControllerRef;
    public GameObject XanaProfile;
    public Button JoinEventBtn;
    public Image[] BannerImgSprite;
    bool _isBuilderScene = default;
    public static bool m_WorldIsClicked = false;
    public static bool m_MuseumIsClicked = false;
    public static bool m_isSignUpPassed = false;
    public GameObject m_WorldPlayPanel;
    public ScrollActivity scrollActivity;
    string ThumbnailDownloadURL="";
    public Transform LobbyLogoContaionr,XanaAvatarIcon,NoAvatarIcon,AvatarIcon;

    [Header("Tags and Category")]
    public GameObject tagScroller;
    public Transform tagsParent;
    public GameObject tagsPrefab;
    public GameObject worldDetailPage;
    public string[] m_WorldTags;
    public bool tagsInstantiated;
    public Transform PreviewLogo;

    public void Init(Sprite worldImg,string worldName, string worldDescription, string creatorName,
        string createdAt, string updatedAt, bool isBuilderSceneF, string userAvatarURL,string ThumbnailDownloadURLHigh,string[] worldTags)
    {
        PreviewLogo.gameObject.SetActive(true);
        WorldIconImg.sprite = null;
        if (!ThumbnailDownloadURL.Equals(""))
        {
            AssetCache.Instance.RemoveFromMemoryDelayCoroutine(ThumbnailDownloadURL, true);
        }
        JoinEventBtn.onClick.RemoveAllListeners();
       
        scrollActivity.enabled = false;
        ScrollControllerRef.verticalNormalizedPosition = 1f;
        WorldNameTxt.GetComponent<TextLocalization>().LocalizeTextText(worldName);
        WorldDescriptionTxt.GetComponent<TextLocalization>().LocalizeTextText(worldDescription);
        CreatorNameTxt.text = creatorName;
        CreatedAtTxt.text = createdAt.Substring(0, 10);
        UpdatedAtTxt.text = updatedAt.Substring(0, 10);
        if (ThumbnailDownloadURLHigh == "")
        {
            PreviewLogo.gameObject.SetActive(false);
            WorldIconImg.sprite = worldImg;
        }
        else
        {
            ThumbnailDownloadURL = ThumbnailDownloadURLHigh;
            StartCoroutine(DownloadAndSetImage(ThumbnailDownloadURLHigh, WorldIconImg));
        }
        if(worldTags!=null && worldTags.Length>0)
        {
            m_WorldTags = worldTags;
            InstantiateWorldtags();
        }
        else
        {
            tagScroller.SetActive(false);
        }
        m_WorldPlayPanel.SetActive(true);
        m_WorldPlayPanel.GetComponent<OnPanel>().rectInterpolate = true;
        m_MuseumIsClicked = false;
        UIManager.Instance.ShowFooter(false);
        GameManager.Instance.WorldBool = true;
        m_WorldIsClicked = true;
        m_isSignUpPassed = true;
        _isBuilderScene = isBuilderSceneF;
        if (_isBuilderScene)
            JoinEventBtn.onClick.AddListener(() => WorldManager.instance.JoinBuilderWorld());
        else
            JoinEventBtn.onClick.AddListener(() => WorldManager.instance.JoinEvent());
        SetPanelToBottom();
        AvatarIcon.GetChild(0).GetComponent<Image>().sprite = NoAvatarIcon.GetComponent<Image>().sprite;
        if (string.IsNullOrEmpty(userAvatarURL))
        {
            NoAvatarIcon.gameObject.SetActive(true);
            XanaAvatarIcon.gameObject.SetActive(false);
            AvatarIcon.gameObject.SetActive(false);
        }
        else if(!string.IsNullOrEmpty(creatorName) && creatorName.ToLower().Contains("xana"))
        {
            NoAvatarIcon.gameObject.SetActive(false);
            XanaAvatarIcon.gameObject.SetActive(true);
            AvatarIcon.gameObject.SetActive(false);
        }
        else
        {
            NoAvatarIcon.gameObject.SetActive(false);
            XanaAvatarIcon.gameObject.SetActive(false);
            AvatarIcon.gameObject.SetActive(true);
            StartCoroutine(DownloadAndSetImage(userAvatarURL, UserProfileImg));
        }
    }
    public void CallAnalytics(string idOfObject,string entityType)
    {
        UserAnalyticsHandler.onGetWorldId?.Invoke(int.Parse(idOfObject), entityType);
        UserAnalyticsHandler.onGetSingleWorldStats?.Invoke(int.Parse(idOfObject), entityType, VisitCountTxt);
    }
    public void SetPanelToBottom()
    {
        if (scrollActivity.gameObject.activeInHierarchy)
        {
            scrollActivity.BottomToTop();
            CheckWorld();
        }
    }
    public void CheckWorld()
    {
        UIManager.Instance.HomePage.SetActive(true);
        FadeImg.sprite = WorldIconImg.sprite;
        UpdateWorldPanel();
        string EnvironmentName = WorldNameTxt.text; 
        if (EnvironmentName == "TACHIBANA SHINNNOSUKE METAVERSE MEETUP" || EnvironmentName == "DJ Event")
        {
            EnvironmentName = "DJ Event";
            if (!PremiumUsersDetails.Instance.CheckSpecificItem(EnvironmentName, false))
            {
                if (EnvironmentName == "DJ Event")
                {
                    PremiumUsersDetails.Instance.PremiumUserUIDJEvent.SetActive(true);
                }
                return;
            }
        }
        else if (EnvironmentName == " Astroboy x Tottori Metaverse Museum")
        {
            if (!PremiumUsersDetails.Instance.CheckSpecificItem(EnvironmentName, true))
            {
                return;
            }
        }
        else if (!_isBuilderScene)
        {
            if (!PremiumUsersDetails.Instance.CheckSpecificItem(EnvironmentName))
            {
                return;
            }
        }
        UIManager.Instance.ShowFooter(false);
        m_MuseumIsClicked = false;
        GameManager.Instance.WorldBool = true;
    }
    public void UpdateWorldPanel()
    {
        if (!WorldNameTxt.text.Contains("XANA Lobby"))
        {
            BannerImgSprite[0].sprite = FadeImg.sprite;
            LobbyLogoContaionr.gameObject.SetActive(false);
        }
        else
        {
            LobbyLogoContaionr.gameObject.SetActive(true);

        }

        BannerImgSprite[1].sprite = FadeImg.sprite;
        if (BannerImgSprite.Length > 2)
            BannerImgSprite[2].sprite = FadeImg.sprite;
    }
    IEnumerator DownloadAndSetImage(string downloadURL,Image imageHolder)
    {
        yield return null;
        if (!string.IsNullOrEmpty(downloadURL))
        {
            if (AssetCache.Instance.HasFile(downloadURL))
            {
                AssetCache.Instance.LoadSpriteIntoImage(imageHolder, downloadURL, changeAspectRatio: true);
                PreviewLogo.gameObject.SetActive(false);
            }
            else
            {
                AssetCache.Instance.EnqueueOneResAndWait(downloadURL, downloadURL, (success) =>
                {
                    if (success)
                    {
                        AssetCache.Instance.LoadSpriteIntoImage(imageHolder, downloadURL, changeAspectRatio: true);
                        if (imageHolder.Equals(WorldIconImg))
                        {
                            PreviewLogo.gameObject.SetActive(false);
                        }
                    }
                });
            }
          
        }
    }


    void InstantiateWorldtags()
    {
        if (m_WorldTags.Length > 0)
            tagScroller.SetActive(true);
        else
            return;

        if (tagsParent.transform.childCount > 0)
        {
            foreach (Transform t in tagsParent)
                Destroy(t.gameObject);
        }

        for (int i = 0; i < m_WorldTags.Length; i++)
        {
            GameObject temp = Instantiate(tagsPrefab, tagsParent);
            temp.GetComponent<TagPrefabInfo>().tagName.text = m_WorldTags[i];
            temp.GetComponent<TagPrefabInfo>().tagNameHighlighter.text = m_WorldTags[i];
            temp.GetComponent<TagPrefabInfo>().descriptionPanel = worldDetailPage;
        }
        tagsInstantiated = true;
    }
}