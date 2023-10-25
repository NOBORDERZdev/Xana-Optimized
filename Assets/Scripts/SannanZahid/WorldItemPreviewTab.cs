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

    [Header("PMY: ClassRom Items")]
    public GameObject enterClassCodePanel;
    public Text classCodeInputField;
    public TextMeshProUGUI wrongCodeText;
    public void PMY_CodeEnter()
    {
        // Check Enter code is Ok or Not
        if (IsClassCodeValid((classCodeInputField.text)))
        {
            // Yes Class Available, Create Room for that Class
            Debug.Log("<color=green> PMY -- Class Available  </color>");
            XanaConstants.xanaConstants.pmy_isClassAvailable = true;
            XanaConstants.xanaConstants.pmy_joinedClassCode = int.Parse(classCodeInputField.text);
            WorldManager.instance.PlayWorld();
        }
        else
        {
            Debug.Log("<color=red> PMY -- Class Not Available  </color>");
            XanaConstants.xanaConstants.pmy_isClassAvailable = false;
            wrongCodeText.gameObject.SetActive(true);
            Invoke(nameof(PMY_CloseWrongCode), 2f);
        }
    }
    void PMY_CloseWrongCode()
    {
        wrongCodeText.gameObject.SetActive(false);
    }
    bool IsClassCodeValid(string classCodeInputField)
    {
        if (string.IsNullOrEmpty("classCodeInputField")) return false;
        classCodeInputField.Replace(" ", "");

        try { return XanaConstants.xanaConstants.pmy_ClassCode.Contains(int.Parse(classCodeInputField)); }
        catch { return false; }

    }




    public void Init(Sprite worldImg,string worldName, string worldDescription, string creatorName,
        string createdAt, string updatedAt, bool isBuilderSceneF, string userAvatarURL)
    {
        JoinEventBtn.onClick.RemoveAllListeners();
        if (_isBuilderScene)
            JoinEventBtn.onClick.AddListener(() => WorldManager.instance.JoinBuilderWorld());
        else
            JoinEventBtn.onClick.AddListener(() => WorldManager.instance.JoinEvent());
        scrollActivity.enabled = false;
        ScrollControllerRef.verticalNormalizedPosition = 1f;
        WorldNameTxt.GetComponent<TextLocalization>().LocalizeTextText(worldName);
        WorldDescriptionTxt.GetComponent<TextLocalization>().LocalizeTextText(worldDescription);
        CreatorNameTxt.text = creatorName;
        CreatedAtTxt.text = createdAt.Substring(0, 10);
        UpdatedAtTxt.text = updatedAt.Substring(0, 10);
        WorldIconImg.sprite = worldImg;
        m_WorldPlayPanel.SetActive(true);
        m_WorldPlayPanel.GetComponent<OnPanel>().rectInterpolate = true;
        m_MuseumIsClicked = false;
        UIManager.Instance.ShowFooter(false);
        GameManager.Instance.WorldBool = true;
        m_WorldIsClicked = true;
        m_isSignUpPassed = true;
        _isBuilderScene = isBuilderSceneF;
        SetPanelToBottom();
        if (!string.IsNullOrEmpty(creatorName))
        {
            if (creatorName.Equals("XANA"))
            {
                XanaProfile.SetActive(true);
                UserProfileImg.transform.parent.gameObject.SetActive(false);
            }
        }
        StartCoroutine(UpdateUserProfile(userAvatarURL));
    }
    // For Analitics & User Count
    public void CallAnalytics(string idOfObject,string entityType)
    {
        UserAnalyticsHandler.onGetWorldId?.Invoke(int.Parse(idOfObject), entityType);
        UserAnalyticsHandler.onGetSingleWorldStats?.Invoke(int.Parse(idOfObject), entityType, VisitCountTxt);
    }
    IEnumerator UpdateUserProfile(string userAvatarURL)
    {
        yield return new WaitForSeconds(Random.Range(0.1f, 0.6f));

        if (!string.IsNullOrEmpty(userAvatarURL))
        {
            if (AssetCache.Instance.HasFile(userAvatarURL))
            {
                AssetCache.Instance.LoadSpriteIntoImage(UserProfileImg, userAvatarURL, changeAspectRatio: true);
            }
            else
            {
                AssetCache.Instance.EnqueueOneResAndWait(userAvatarURL, userAvatarURL, (success) =>
                {
                    if (success)
                    {
                        AssetCache.Instance.LoadSpriteIntoImage(UserProfileImg, userAvatarURL, changeAspectRatio: true);
                    }
                });
            }
        }

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
        }

        BannerImgSprite[1].sprite = FadeImg.sprite;
        if (BannerImgSprite.Length > 2)
            BannerImgSprite[2].sprite = FadeImg.sprite;
    }
}