using DG.Tweening;
using Photon.Voice.Unity.Demos;
using SuperStar.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class WorldDescriptionPopupPreview : MonoBehaviour
{
    public string worldId;
    public GameObject thumbnailPrefabRef;
    public TextMeshProUGUI WorldNameTxt;
    public TextMeshProUGUI WorldDescriptionTxt;
    public TextMeshProUGUI CreatorNameTxt;
    public Text CreatedAtTxt;
    public Text UpdatedAtTxt;
    public Text VisitCountTxt;
    [Header("Images")]
    public RectTransform WorldImageHolderRef;
    public Image WorldIconImg;
    public Image UserProfileImg;
    public GameObject WorldDetailContentrRef;
    public Button JoinEventBtn;
    public GameObject followingWorld;
    public GameObject followingWorldHighlight;
    public GameObject followWorldLoader;
    bool _isBuilderScene = default;
    public static bool m_WorldIsClicked = false;
    public static bool m_MuseumIsClicked = false;
    public static bool m_isSignUpPassed = false;
    public GameObject m_WorldPlayPanel;
    public ScrollActivity scrollActivity;
    public Transform AvatarIcon;
    public Transform XanaAvatarIcon;
    public Sprite NoAvatarIcon;
    public TextMeshProUGUI CreatorDescriptionTxt;
    public GameObject creatorPanel, sepLineSmallView, sepLineLargeView;
    public RectTransform worldImageMask;

    [Header("Tags and Category")]
    public GameObject tagScroller;
    public Transform tagsParent;
    public GameObject tagsPrefab;
    public GameObject worldDetailPage;
    public string[] m_WorldTags;
    public bool tagsInstantiated;
    public Transform PreviewLogo;
    public GameObject backButton;
    public bool UserMicEnable;
    public static Action<bool> OndescriptionPanelSwipUp;
    private void OnEnable()
    {
        OndescriptionPanelSwipUp += UpdateDescirptionPanelItem;
    }

    private void OnDisable()
    {
        OndescriptionPanelSwipUp -= UpdateDescirptionPanelItem;
    }

    void UpdateDescirptionPanelItem(bool isFullOpen)
    {
        if (isFullOpen)
        {
            WorldImageHolderRef.sizeDelta = new Vector2(WorldImageHolderRef.sizeDelta.x, 487f);
            SetTopAndBottomPositions(-15f, 20f);
            SetSeparatorLineProp(false);
            backButton.SetActive(true);
        }
        else
        {
            WorldImageHolderRef.sizeDelta = new Vector2(WorldImageHolderRef.sizeDelta.x, 495f);
            SetTopAndBottomPositions(0f, 0f);
            SetSeparatorLineProp(true);
            backButton.SetActive(false);
        }
    }

    void SetTopAndBottomPositions(float top, float bottom)
    {
        // Calculate the new height of the RectTransform
        float newHeight = Mathf.Abs(top - bottom);

        // Set the new height of the RectTransform
        worldImageMask.sizeDelta = new Vector2(worldImageMask.sizeDelta.x, newHeight);

        // Calculate the new Y position of the RectTransform
        float newYPosition = (top + bottom) / 2f;

        // Update the Y position of the RectTransform
        worldImageMask.localPosition = new Vector3(worldImageMask.localPosition.x, newYPosition, worldImageMask.localPosition.z);
    }

    public void SetSeparatorLineProp(bool _switchLine)
    {
        sepLineSmallView.SetActive(_switchLine);
        sepLineLargeView.SetActive(!_switchLine);
    }

    // Function to set the top properties of a RectTransform
    //void SetParentBodyTop(RectTransform rectTransform, float top)
    //{
    //    //// Get the current anchored position
    //    //Vector2 anchoredPosition = rectTransform.anchoredPosition;

    //    //// Set the top parameter
    //    //anchoredPosition.y = top;

    //    //// Assign the new anchored position
    //    //rectTransform.anchoredPosition = anchoredPosition;
    //}

    public void Init(GameObject thumbnailObjRef, Sprite worldImg, string worldName, string worldDescription, string creatorName,
        string createdAt, string updatedAt, bool isBuilderSceneF, string userAvatarURL, string ThumbnailDownloadURLHigh, string[] worldTags,
        string entityType, string creator_Name, string creator_Description, string creatorAvatar, bool userMicEnable, bool isFavourite, string _worldId)
    {
        WorldDetailContentrRef.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        worldId = _worldId;
        if (thumbnailObjRef)
            thumbnailPrefabRef = thumbnailObjRef;

        PreviewLogo.gameObject.SetActive(true);
        WorldIconImg.sprite = null;
        //if (!ThumbnailDownloadURL.Equals(""))
        //{
        //    AssetCache.Instance.RemoveFromMemoryDelayCoroutine(ThumbnailDownloadURL, true);
        //}
        JoinEventBtn.onClick.RemoveAllListeners();

        scrollActivity.enabled = false;
        //ScrollControllerRef.verticalNormalizedPosition = 1f;
        WorldNameTxt.GetComponent<TextLocalization>().LocalizeTextText(worldName);
        WorldDescriptionTxt.GetComponent<TextLocalization>().LocalizeTextText(worldDescription);
        CreatorNameTxt.text = creatorName;
        CreatorNameTxt.GetComponent<TextLocalization>().LocalizeTextText(creatorName);
        CreatedAtTxt.text = createdAt.Substring(0, 10);
        UpdatedAtTxt.text = updatedAt.Substring(0, 10);

        PreviewLogo.gameObject.SetActive(false);
        WorldIconImg.sprite = worldImg;

        //if (ThumbnailDownloadURLHigh == "")
        //{
        //    PreviewLogo.gameObject.SetActive(false);
        //    WorldIconImg.sprite = worldImg;
        //}
        //else
        //{
        //    ThumbnailDownloadURL = ThumbnailDownloadURLHigh;
        //    StartCoroutine(DownloadAndSetImage(ThumbnailDownloadURLHigh, WorldIconImg));
        //}
        if (worldTags != null && worldTags.Length > 0)
        {
            m_WorldTags = worldTags;
            InstantiateWorldtags();
        }
        else
        {
            tagScroller.transform.parent.gameObject.SetActive(false);
        }
        //UpdateDescirptionPanelItem(false);
        m_WorldPlayPanel.SetActive(true);
        m_WorldPlayPanel.GetComponent<OnPanel>().rectInterpolate = true;
        m_MuseumIsClicked = false;
        GameManager.Instance.UiManager.ShowFooter(false);
        GameManager.Instance.WorldBool = true;
        m_WorldIsClicked = true;
        m_isSignUpPassed = true;
        _isBuilderScene = isBuilderSceneF;
        if (_isBuilderScene)
            JoinEventBtn.onClick.AddListener(() => WorldManager.instance.JoinBuilderWorld());
        else
            JoinEventBtn.onClick.AddListener(() => WorldManager.instance.JoinEvent());
        SetPanelToBottom();
        AvatarIcon.GetChild(0).GetComponent<Image>().sprite = NoAvatarIcon;
        /*if (entityType == WorldType.USER_WORLD.ToString() && (creator_Name != null || creator_Description != null || creatorAvatar != null))
        {*/
        CreatorNameTxt.text = creator_Name;
        CreatorNameTxt.GetComponent<TextLocalization>().LocalizeTextText(creator_Name);
        CreatorDescriptionTxt.GetComponent<TextLocalization>().LocalizeTextText(creator_Description);
        AvatarIcon.GetChild(0).GetComponent<Image>().sprite = NoAvatarIcon;
        //if (string.IsNullOrEmpty(userAvatarURL))
        //{
            //NoAvatarIcon.gameObject.SetActive(true);
            //XanaAvatarIcon.gameObject.SetActive(false);
            //AvatarIcon.gameObject.SetActive(true);
        //}
        if (!string.IsNullOrEmpty(creatorName) && creatorName.ToLower().Contains("xana"))
        {
            //NoAvatarIcon.gameObject.SetActive(false);
            XanaAvatarIcon.gameObject.SetActive(true);
            AvatarIcon.gameObject.SetActive(false);
        }
        else if (!string.IsNullOrEmpty(userAvatarURL))
        {
            //NoAvatarIcon.gameObject.SetActive(false);
            XanaAvatarIcon.gameObject.SetActive(false);
            AvatarIcon.gameObject.SetActive(true);
            StartCoroutine(DownloadAndSetImage(userAvatarURL, UserProfileImg));
        }
        creatorPanel.SetActive(true);
        /*}
        else
        {
            creatorPanel.SetActive(false);
        }*/
        UserMicEnable = userMicEnable;
        ConstantsHolder.xanaConstants.UserMicEnable = UserMicEnable;
        CheckWorldFav(isFavourite);
        WorldDescriptionTxt.transform.parent.GetComponent<ParentHeightAdjuster>().SetParentHeight();
        //CreatorDescriptionTxt.transform.parent.GetComponent<ParentHeightAdjuster>().SetParentHeight();
        //WorldDetailContentrRef.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    public void CallAnalytics(string idOfObject, string entityType)
    {
        UserAnalyticsHandler.onGetWorldId?.Invoke(int.Parse(idOfObject), entityType);
        //UserAnalyticsHandler.onGetSingleWorldStats?.Invoke(int.Parse(idOfObject), entityType, VisitCountTxt); // Due to Flow change this API in not in use
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
        //  GameManager.Instance.UiManager.HomePage.SetActive(true);
        //FadeImg.sprite = WorldIconImg.sprite;
        //UpdateWorldPanel();
        string EnvironmentName = WorldNameTxt.text;
        if (EnvironmentName == "TACHIBANA SHINNNOSUKE METAVERSE MEETUP" || EnvironmentName == "DJ Event")
        {
            EnvironmentName = "DJ Event";
            if (!UserPassManager.Instance.CheckSpecificItem(EnvironmentName, false))
            {
                if (EnvironmentName == "DJ Event")
                {
                    UserPassManager.Instance.PremiumUserUIDJEvent.SetActive(true);
                }
                return;
            }
        }
        else if (EnvironmentName == " Astroboy x Tottori Metaverse Museum")
        {
            if (!UserPassManager.Instance.CheckSpecificItem(EnvironmentName, true))
            {
                return;
            }
        }
        else if (!_isBuilderScene)
        {
            if (!UserPassManager.Instance.CheckSpecificItem(EnvironmentName))
            {
                return;
            }
        }
        GameManager.Instance.UiManager.ShowFooter(false);
        m_MuseumIsClicked = false;
        GameManager.Instance.WorldBool = true;
    }

    public void UpdateWorldPanel()
    {
        //BannerImgSprite[0].sprite = FadeImg.sprite;
        //BannerImgSprite[1].sprite = FadeImg.sprite;
        //if (BannerImgSprite.Length > 2)
        //    BannerImgSprite[2].sprite = FadeImg.sprite;
    }

    IEnumerator DownloadAndSetImage(string downloadURL, Image imageHolder)
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
            tagScroller.transform.parent.gameObject.SetActive(true);
        else
            return;

        if (tagsParent.transform.childCount > 0)
        {
            foreach (Transform t in tagsParent)
                Destroy(t.gameObject);
        }

        for (int i = 0; i < m_WorldTags.Length; i++)
        {
            if (!m_WorldTags[i].IsNullOrEmpty())
            {
                GameObject temp = Instantiate(tagsPrefab, tagsParent);
                temp.GetComponent<TagPrefabInfo>().tagName.text = m_WorldTags[i];
                temp.GetComponent<TagPrefabInfo>().tagName.text = temp.GetComponent<TagPrefabInfo>().tagName.text.ToUpper();
                //temp.GetComponent<TagPrefabInfo>().tagNameHighlighter.text = m_WorldTags[i];
                temp.GetComponent<TagPrefabInfo>().descriptionPanel = worldDetailPage;
                //if (i == 0)
                //{
                //    temp.GetComponent<Image>().color = new Color(0.1607843f, 0.1607843f, 0.1882353f);
                //    temp.GetComponent<TagPrefabInfo>().tagName.color = Color.white;
                //}
            }
        }
        tagsInstantiated = true;
    }

    void CheckWorldFav(bool isFavourite)
    {
        if (isFavourite)
        {
            followingWorldHighlight.SetActive(true);
            followingWorld.SetActive(false);
        }
        else
        {
            followingWorldHighlight.SetActive(false);
            followingWorld.SetActive(true);
        }
        followingWorld.GetComponent<Button>().interactable = true;
        followingWorldHighlight.GetComponent<Button>().interactable = true;
    }

    //Both below functions are called from inspector on world fav buttons
    public void FavoriteWorldBtnClicked()
    {
        if (!UserPassManager.Instance.CheckSpecificItem("Favorite Worlds"))
        {
            print("Please Upgrade to Premium account");
            return;
        }
        else
        {
            print("Horayyy you have Access");
        }
        followingWorld.GetComponent<Button>().interactable = false;
        followWorldLoader.SetActive(true);
        string apiUrl = ConstantsGod.API_BASEURL + ConstantsGod.FOLLOWWORLD + worldId;
        Debug.LogError(apiUrl);
        StartCoroutine(FollowWorldAPI(apiUrl, worldId, (isSucess) =>
        {
            if (isSucess)
            {
                followingWorldHighlight.SetActive(true);
                followingWorldHighlight.GetComponent<Button>().interactable = true;
                followingWorld.SetActive(false);
                followWorldLoader.SetActive(false);
                if (thumbnailPrefabRef)
                    thumbnailPrefabRef.GetComponent<WorldItemView>().isFavourite = true;
                //Reloading following space
                WorldManager.instance.changeFollowState = true;
                WorldManager.ReloadFollowingSpace?.Invoke();
            }
            else
            {
                followingWorld.GetComponent<Button>().interactable = true;
                followWorldLoader.SetActive(false);
            }
        }));
    }

    public void HighlightFavoriteBtnClicked()
    {
        followingWorldHighlight.GetComponent<Button>().interactable = false;
        followWorldLoader.SetActive(true);
        string apiUrl = ConstantsGod.API_BASEURL + ConstantsGod.FOLLOWWORLD + worldId;
        StartCoroutine(FollowWorldAPI(apiUrl, worldId, (isSucess) =>
        {
            if (isSucess)
            {
                followingWorld.SetActive(true);
                followingWorld.GetComponent<Button>().interactable = true;
                followingWorldHighlight.SetActive(false);
                followWorldLoader.SetActive(false);
                if (thumbnailPrefabRef)
                {
                    thumbnailPrefabRef.GetComponent<WorldItemView>().isFavourite = false;

                }
                //Reloading following space
                WorldManager.instance.changeFollowState = true;
                WorldManager.ReloadFollowingSpace?.Invoke();

            }
            else
            {
                followingWorldHighlight.GetComponent<Button>().interactable = true;
                followWorldLoader.SetActive(false);
            }
        }));
    }

    IEnumerator FollowWorldAPI(string APIurl, string worldId, Action<bool> CallBack)
    {
        yield return new WaitForEndOfFrame();
        //WWWForm wWWForm = new WWWForm();
        //wWWForm.AddField("worldId", worldId);
        Dictionary<string, int> data = new Dictionary<string, int>();
        data.Add("worldId", int.Parse(worldId));
        string jsonData = JsonUtility.ToJson(data);
        using (UnityWebRequest www = UnityWebRequest.Put(APIurl, jsonData))
        {
            www.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            www.SetRequestHeader("Content-Type", "application/json");
            www.SendWebRequest();
            while (!www.isDone)
                yield return null;
            if ((www.result == UnityWebRequest.Result.ConnectionError) || (www.result == UnityWebRequest.Result.ProtocolError))
            {
                //Debug.LogError("following world error :- " + www.downloadHandler.text);
                CallBack(false);
            }
            else
            {
                // Debug.LogError("following world :- "+www.downloadHandler.text);
                CallBack(true);
            }
            www.Dispose();
        }
    }
}