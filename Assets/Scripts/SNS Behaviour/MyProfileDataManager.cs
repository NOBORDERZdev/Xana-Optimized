using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SuperStar.Helpers;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System;
using UnityEngine.UI.Extensions;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Mail;
using AdvancedInputFieldPlugin;
using System.Linq;
using System.Text;
using System.Web;
using System.Threading.Tasks;

public class MyProfileDataManager : MonoBehaviour
{
    public static MyProfileDataManager Instance;

    string defaultUrl = "https://";
    public GetUserDetailData myProfileData = new GetUserDetailData();


    public List<AllFeedByUserIdRow> allMyFeedImageRootDataList = new List<AllFeedByUserIdRow>();//image feed list
    List<FeedResponseRow> allMyTextPostFeedImageRootDataList = new List<FeedResponseRow>();//text feed list

    public List<AllFeedByUserIdRow> allMyFeedVideoRootDataList = new List<AllFeedByUserIdRow>();//video feed list
    AllTextPostByUserIdRoot currentPageAllTextPostWithUserIdRoot = new AllTextPostByUserIdRoot();

    private AllUserWithFeedRow FeedRawData;

    [Space]
    [Header("Screen References")]
    public GameObject myProfileScreen;
    public GameObject editProfileScreen;
    [SerializeField] GameObject pickImageOptionScreen;
    public GameObject OtherPlayerdataObj;
    [SerializeField] GameObject settingsButton;
    [SerializeField] GameObject otherProfileScreenBackButton;
    public Action otherProfileScreenBackButtonAction;

    [Space]
    [Header("Profile Screen Refresh Object")]
    [SerializeField] GameObject mainFullScreenContainer;
    [SerializeField] GameObject mainProfileDetailPart;
     //public GameObject userPostPart;
    [SerializeField] GameObject bioDetailPart;
    [SerializeField] GameObject bioTxtParent;

    [Space]
    [Header("Player info References")]
    //public TextMeshProUGUI topHaderUserNameText;
    public Image profileImage;
    public TextMeshProUGUI totalPostText;
    [SerializeField] TextMeshProUGUI totalFollowerText;
    [SerializeField] TextMeshProUGUI totalFollowingText;
    [Space]
    [SerializeField] TextMeshProUGUI playerNameText;
    [SerializeField] TextMeshProUGUI displayName;
    [SerializeField] TextMeshProUGUI textUserBio;
    [SerializeField] GameObject _alignment_space; // use this b/w bio and Tags in Profile Screen
    [SerializeField] TextMeshProUGUI websiteText;

    [SerializeField] GameObject seeMoreBioButton;
    [SerializeField] GameObject seeMoreButtonTextObj;
    [SerializeField] GameObject seeLessButtonTextObj;

    [Space]
    [Header("Photo, Movie, NFT Button Panel Tab panel Reference")]
    string CurrentSection;
    [SerializeField] ParentHeightResetScript parentHeightResetScript;
    [SerializeField] SelectionItemScript selectionItemScript1;
    [SerializeField] SelectionItemScript selectionItemScript2;

    [Space]
    [Header("Player Uploaded Item References")]
    [SerializeField] Transform mainPostContainer;
    public Transform allPhotoContainer;
    public Transform allMovieContainer;
    public GameObject photoPrefab;
    [SerializeField] GameObject NFTImagePrefab;

    [Space]
    [SerializeField] GameObject tabPrivateObject;
    [SerializeField] GameObject tabPublicObject;

    [Space]
    [SerializeField] Sprite defultProfileImage;

    [Space]
    [Header("Edit Profile Reference")]
    [SerializeField] Image editProfileImage;
    [SerializeField] AdvancedInputField editProfileNameAdvanceInputfield;
    [SerializeField] AdvancedInputField editProfileUniqueNameAdvanceInputfield;
    [SerializeField] AdvancedInputField editProfileBioInputfield;
    [SerializeField] GameObject websiteErrorObj;
    [SerializeField] GameObject nameErrorMessageObj;
    [SerializeField] GameObject uniqueNameErrorMessageObj;
    [SerializeField] Button editProfileDoneButton;
    public bool isEditProfileNameAlreadyExists;

    [Space]
    [SerializeField] GameObject editProfileBioScreen;
    [SerializeField] AdvancedInputField bioEditAdvanceInputField;

    [Space]
    [Header("Tags in Edit Profile")]
    [SerializeField] GameObject tags_row;
    [SerializeField] GameObject tags_row_obj;
    [SerializeField] Transform tags_row_parent;
    [SerializeField] List<string> availableTagsAtServer;
    public List<string> userSelectedTags;
    [SerializeField] GameObject dropDownBtn;


    [Space]
    [Header("For API Pagination")]
    [SerializeField] ScrollRectFasterEx profileMainScrollRectFasterEx;
    private bool NFTShowingOnneBool;

    [Header("NFT Data Holder Scriptable Object")]
    [SerializeField] OwnedNFTContainer _OwnedNFTDataObj;
    int tempOPCount = 0;
    bool tempLogout = false;
    bool isSetTempSpriteAfterUpdateAvatar = false;
    string lastTopUserText;
    string tempBioOnly10LineStr = "";
    [SerializeField] List<int> loadedMyPostAndVideoId = new List<int>();
    [SerializeField] List<int> loadedMyPostAndVideoIdInFeedPage = new List<int>();
    int generatedTagCount = 0; // generated Tag Counter 
    int availableTagsCount = 0;
    [SerializeField] int checkEditNameUpdated = 0;
    [SerializeField] int checkEditInfoUpdated = 0;
    string website = "";
    string job = "";
    string bio = "";
    string gender = "";
    string username = "";
    string uniqueUsername = "";
    string[] tempTags;
    bool isUrl = false;
    Coroutine webValidCo;
    Coroutine editProfileErrorCo;
    GameObject currentEditProfileErrorMessgaeObj;
    [SerializeField] string setImageAvatarTempPath = "";
    [SerializeField] string setImageAvatarTempFilename = "";
    private WebCamTexture webCamTexture;
    Coroutine followingCo;
    Coroutine followeCo;
    [SerializeField] GetUserDetailRoot tempMyProfileDataRoot = new GetUserDetailRoot();
    bool profileMakedFlag = false;
    public string permissionCheck = "";
    public string TestingJasonForTags;
    UserLoginSignupManager userLoginSignupManager;
    SNS_APIManager apiManager;
    ProfileUIHandler profileUIHandler;
    FeedUIController feedUIController;
    public static event Action<BackButtonHandler.screenTabs> OnScreenTabStateChange;
    //HomeScoketHandler socketController;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        NFTShowingOnneBool = false;
    }

    private void OnEnable()
    {
        if (profileMainScrollRectFasterEx.GetComponent<Mask>().enabled)
        {
            profileMainScrollRectFasterEx.GetComponent<Mask>().enabled = false;
        }
        settingsButton.SetActive(true);
        otherProfileScreenBackButton.SetActive(false);
    }

    private void OnDisable()
    {
        settingsButton.SetActive(false);
        otherProfileScreenBackButton.SetActive(true);
    }

    private void Start()
    {
        ClearDummyData();//clear dummy data.......
        userLoginSignupManager = UserLoginSignupManager.instance;
        apiManager= SNS_APIManager.Instance;
        profileUIHandler = ProfileUIHandler.instance;
        feedUIController = FeedUIController.Instance;
        //socketController = HomeScoketHandler.instance;
        string saveDir = Path.Combine(Application.persistentDataPath, "UserProfilePic");
        if (!Directory.Exists(saveDir))
        {
            Directory.CreateDirectory(saveDir);
        }

        if (GlobalVeriableClass.callingScreen == "Profile")
        {
            myProfileScreen.SetActive(true);
        }
        RequestGetUserDetails();
    }

    #region Profile screen methods
    //this method is used to clear the dummy data.......
    public void ClearDummyData()
    {
        playerNameText.text = "";
        displayName.text = "";
       // jobText.gameObject.SetActive(false);
        textUserBio.text = "";
        websiteText.text = "";
        _alignment_space.SetActive(false);
        websiteText.gameObject.SetActive(false);
        profileImage.sprite = defultProfileImage;
    }

    //this method is used to clear my profile data after logout.......
    public void ClearAndResetAfterLogout()
    {
        loadedMyPostAndVideoId.Clear();  //amit-19-3-2022 onlogout clear feed id list 
        loadedMyPostAndVideoIdInFeedPage.Clear();  //Riken 
        ClearDummyData();
        tempLogout = true;
        MyProfileSceenShow(false);
        userLoginSignupManager.userRoleScriptScriptableObj.userNftRoleSlist.Clear();
    }

    //this method is used to Profile Tab Button Click.......
    public void ProfileTabButtonClick()
    {
        if (apiManager == null)
        {
            apiManager = SNS_APIManager.Instance;
        }
        apiManager.RequestGetUserDetails("myProfile");//Get My Profile data       
        MyProfileSceenShow(true);//active myprofile screen
    }

    //this method is used to my profile screen show or not.......
    public void MyProfileSceenShow(bool isShow)
    {
        myProfileScreen.SetActive(isShow);
    }

    //this method is used to setup data after get api response.......
    public void SetupData(GetUserDetailData myData, string callingFrom)
    {
        myProfileData = myData;
        //Debug.Log(callingFrom);
        if (callingFrom == "EditProfileAvatar")
        {
            EditProfileDoneButtonSetUp(true);//setup edit profile done button.......
            if (!isEditProfileNameAlreadyExists)
            {
                editProfileScreen.SetActive(false);
            }
            isEditProfileNameAlreadyExists = false;
            //Debug.Log("Profile Update Success and delete file");
            if (File.Exists(setImageAvatarTempPath))
            {
                File.Delete(setImageAvatarTempPath);

                UpdateAavtarUrlOfAllMyFeed();
            }
            if (AssetCache.Instance.HasFile(setImageAvatarTempFilename))
            {
                //Debug.Log("IOS update Profile Pic Delete");
                AssetCache.Instance.DeleteAsset(setImageAvatarTempFilename);
            }
            setImageAvatarTempPath = "";
            setImageAvatarTempFilename = "";

            LoadDataMyProfile();//set data
        }
        else
        {
            LoadDataMyProfile();//set data
            apiManager.RequestGetFeedsByUserId(apiManager.userId, 1, 40, "MyProfile");
        }
    }

   
    //this method is used to set temp profile image after update profile image.......
    public void AfterUpdateAvatarSetTempSprite()
    {
        if (profileImage.sprite != editProfileImage.sprite)
        {
            isSetTempSpriteAfterUpdateAvatar = true;
            profileImage.sprite = editProfileImage.sprite;
        }
    }

    //this method is used to Update Avatar after update All Feed Avatar url.......
    void UpdateAavtarUrlOfAllMyFeed()
    {
        //New text post based feed implementation
        FeedData[] userPostItems = mainPostContainer.GetComponentsInChildren<FeedData>();
        //Debug.Log("UpdateAavtarUrlOfAllMyFeed Length:" + userPostItems.Length);
        for (int i = 0; i < userPostItems.Length; i++)
        {
            //Debug.Log("ID:" + userPostItems[i].userData.Id);
            userPostItems[i]._data.user.avatar = myProfileData.avatar;
        }
    }

  
    //this method is used to my profile data set.......
    public void LoadDataMyProfile()
    {
        if (profileUIHandler)
        {
            profileUIHandler.followerBtn.interactable = true;
            profileUIHandler.followingBtn.interactable = true;
        }
        playerNameText.text = myProfileData.name;
        displayName.text = "@"+myProfileData.userProfile.username;
        lastTopUserText = myProfileData.name;

        totalFollowerText.text = myProfileData.followerCount.ToString();
        totalFollowingText.text = myProfileData.followingCount.ToString();
        UpdateUserTags();

        websiteText.gameObject.SetActive(false);
        if (myProfileData.userProfile != null)
        {
          //  jobText.gameObject.SetActive(false);
            if (!string.IsNullOrEmpty(myProfileData.userProfile.bio))
            {
                textUserBio.text = SNS_APIManager.DecodedString(myProfileData.userProfile.bio);
                if (textUserBio.text == " ")
                    _alignment_space.SetActive(false);
                else
                    _alignment_space.SetActive(true);
                SetupBioPart(textUserBio.text);//check and show only 10 line.......
            }
            else
            {
                seeMoreBioButton.SetActive(false);
                _alignment_space.SetActive(false);
            }
        }
        else
        {
            seeMoreBioButton.SetActive(false);
            textUserBio.text = TextLocalization.GetLocaliseTextByKey("You have no bio yet.");
        }

        if (!isSetTempSpriteAfterUpdateAvatar)//if temp avatar set is true then do not add default profile image.......
        {
            profileImage.sprite = defultProfileImage;
        }
        isSetTempSpriteAfterUpdateAvatar = false;
        UpdateProfilePic();
        mainProfileDetailPart.GetComponent<VerticalLayoutGroup>().spacing = 0.01f;
        StartCoroutine(WaitToRefreshProfileScreen());
    }


    public void UpdateProfilePic()
    {
        if (!string.IsNullOrEmpty(myProfileData.avatar))
        {
            Debug.Log("My profile Avatar :-" + myProfileData.avatar);
            bool isUrlContainsHttpAndHttps = apiManager.CheckUrlDropboxOrNot(myProfileData.avatar);
            if (isUrlContainsHttpAndHttps)
            {
                AssetCache.Instance.EnqueueOneResAndWait(myProfileData.avatar, myProfileData.avatar, (success) =>
                {
                    if (success)
                    {
                        AssetCache.Instance.LoadSpriteIntoImage(profileImage, myProfileData.avatar, changeAspectRatio: true);
                        ConstantsHolder.xanaConstants.userProfileLink = myProfileData.avatar;
                    }
                });
            }
            else
            {
                GetImageFromAWS(myProfileData.avatar, profileImage);
                ConstantsHolder.xanaConstants.userProfileLink = myProfileData.avatar;
            }
        }
    }



    public void UpdateUserTags()
    {
       if (profileUIHandler)
       {
            if (myProfileData.tags != null && myProfileData.tags.Length > 0)
            {
                profileUIHandler.UserTagsParent.transform.parent.gameObject.SetActive(true);
                profileUIHandler.UserTagsParent.GetComponent<HorizontalLayoutGroup>().spacing = 18.01f;

                while (profileUIHandler.UserTagsParent.transform.childCount < myProfileData.tags.Length)
                {
                    GameObject _tagobject = Instantiate(profileUIHandler.TagPrefab, profileUIHandler.UserTagsParent.transform);
                    _tagobject.name = "TagPrefab" + profileUIHandler.UserTagsParent.transform.childCount;
                }

                for (int i = 0; i < profileUIHandler.UserTagsParent.transform.childCount; i++)
                {
                    if (i < myProfileData.tags.Length)
                    {
                        profileUIHandler.UserTagsParent.transform.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text = myProfileData.tags[i];
                    }
                    else
                    {
                        Destroy(profileUIHandler.UserTagsParent.transform.GetChild(i).gameObject);
                    }
                }
            }
            else
            {
                profileUIHandler.UserTagsParent.transform.parent.gameObject.SetActive(false);
            }
       }
    }

    public string ReplaceNonCharacters(string aString, char replacement)
    {
        var sb = new StringBuilder(aString.Length);
        for (var i = 0; i < aString.Length; i++)
        {
            if (char.IsSurrogatePair(aString, i))
            {
                int c = char.ConvertToUtf32(aString, i);
                i++;
                if (IsCharacter(c))
                    sb.Append(char.ConvertFromUtf32(c));
                else
                    sb.Append(replacement);
            }
            else
            {
                char c = aString[i];
                if (IsCharacter(c))
                    sb.Append(c);
                else
                    sb.Append(replacement);
            }
        }
        return sb.ToString();
    }

    public bool IsCharacter(int point)
    {
        return point < 0xFDD0 || // everything below here is fine
            point > 0xFDEF &&    // exclude the 0xFFD0...0xFDEF non-characters
            (point & 0xfffE) != 0xFFFE; // exclude all other non-characters
    }

    //this method is used to Refresh my profile main content size fitter.......
    public IEnumerator WaitToRefreshProfileScreen()
    {
        Debug.Log("Enter in Content Size Filter Section");

        var components = new List<ContentSizeFitter>
        {
            textUserBio.GetComponent<ContentSizeFitter>(),
            bioTxtParent.GetComponent<ContentSizeFitter>(),
            bioDetailPart.GetComponent<ContentSizeFitter>(),
            mainProfileDetailPart.GetComponent<ContentSizeFitter>(),
            mainFullScreenContainer.GetComponent<ContentSizeFitter>()
        };

        foreach (var component in components)
        {
            component.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
            yield return new WaitForSeconds(0.01f);
            component.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
    }

   
    //this method is used to setup bio text.......
    public void SetupBioPart(string bioText)
    {
        string[] bioLines = bioText.Split('\n');
        if (bioLines.Length > 10)
        {
            tempBioOnly10LineStr = string.Join("\n", bioLines.Take(10));
            textUserBio.text = tempBioOnly10LineStr;
            SeeMoreLessBioTextSetup(true);
            seeMoreBioButton.SetActive(true);
        }
        else
        {
            seeMoreBioButton.SetActive(false);
        }
    }

    //this method is used to Bio SeeMore or Less button click.......
    public void OnClickBioSeeMoreLessButton()
    {
        if (seeMoreButtonTextObj.activeSelf)
        {
            textUserBio.text = SNS_APIManager.DecodedString(myProfileData.userProfile.bio);
            SeeMoreLessBioTextSetup(false);
        }
        else
        {
            textUserBio.text = tempBioOnly10LineStr;
            SeeMoreLessBioTextSetup(true);
        }
        ResetMainScrollDefaultTopPos();
        StartCoroutine(WaitToRefreshProfileScreen());
    }

    //this method is used to see more and see less bio text setup.......
    void SeeMoreLessBioTextSetup(bool isSeeMore)
    {
        seeMoreButtonTextObj.SetActive(isSeeMore);
        seeLessButtonTextObj.SetActive(!isSeeMore);
    }

    //this method is used to reset to main scroll default position to top.......
    public void ResetMainScrollDefaultTopPos()
    {
        profileMainScrollRectFasterEx.verticalNormalizedPosition = 1;
    }
   
    //this method is used to load All my feed and setup.......
    public void AllFeedWithUserId(int pageNumb, Transform Feedparent = null, bool IsNew = false)
    {
        mainFullScreenContainer.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        //New Text post based feed implimentation
        currentPageAllTextPostWithUserIdRoot = apiManager.allTextPostWithUserIdRoot;
        bool IsMyProfileFeed = false;
       
        int rowsCount = currentPageAllTextPostWithUserIdRoot.data.rows.Count;
        for (int i = 0; i <= rowsCount; i++)
        {
            if (i < rowsCount)
            {
                var currentRow = currentPageAllTextPostWithUserIdRoot.data.rows[i];
                if (loadedMyPostAndVideoId.Contains(currentRow.id))
                {
                    var child = allPhotoContainer.transform.GetChild(i);
                    var feedData = child.GetComponent<FeedData>();
                    if (feedData)
                    {
                        feedData.SetFeedPrefab(currentRow, false);
                        child.name = "User Feed Post old one " + i;
                    }
                }
                else if ((!loadedMyPostAndVideoId.Contains(currentRow.id) && Feedparent == null) ||
                         (!loadedMyPostAndVideoIdInFeedPage.Contains(currentRow.id) && Feedparent != null))
                {
                    Transform parent = Feedparent == null && !string.IsNullOrEmpty(currentRow.text_post) ? allPhotoContainer : null;
                    GameObject userTagPostObject = Instantiate(photoPrefab, parent);
                    userTagPostObject.AddComponent<LayoutElement>();
                    userTagPostObject.name = "User Feed Post 2.0 " + i;
                    if (IsNew)
                    {
                        userTagPostObject.transform.SetAsFirstSibling();
                    }
                    FeedData userPostItem = userTagPostObject.GetComponent<FeedData>();
                    userPostItem.SetFeedPrefab(currentRow, false);
                    userPostItem.isProfileScene = true;
                    if (!allMyTextPostFeedImageRootDataList.Contains(currentRow))
                    {
                        if (IsNew)
                        {
                            allMyTextPostFeedImageRootDataList.Insert(0, currentRow);
                        }
                        else
                        {
                            allMyTextPostFeedImageRootDataList.Add(currentRow);
                        }
                    }
                    if (pageNumb == 1 && i == 0)
                    {
                        userTagPostObject.transform.SetAsFirstSibling();
                        allMyTextPostFeedImageRootDataList.Insert(0, currentRow);
                    }
                    else
                    {
                        userTagPostObject.transform.SetSiblingIndex(i);
                        allMyTextPostFeedImageRootDataList.Add(currentRow);
                    }
                }
            }
            if (allPhotoContainer != null)
            {
                allPhotoContainer.GetComponent<VerticalLayoutGroup>().spacing = 5.01f;
            }
        }

        int childCount = allPhotoContainer.childCount;

        if (childCount > rowsCount)
        {
            for (int i = rowsCount; i < childCount; i++)
            {
                Destroy(allPhotoContainer.GetChild(i).gameObject);
            }
        }
        Debug.Log("Pagenmub bar");
        if (allMyTextPostFeedImageRootDataList.Count >= 2 && 
            allMyTextPostFeedImageRootDataList[0].id == allMyTextPostFeedImageRootDataList[1].id)
        {
            allMyTextPostFeedImageRootDataList.RemoveAt(0);
        }
        //allPhotoContainer.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        StartCoroutine(DisableFadderWithDelay());
    }

    IEnumerator DisableFadderWithDelay()
    {
        yield return new WaitForSeconds(0.4f);

        parentHeightResetScript.SetParentheight(allPhotoContainer.GetComponent<RectTransform>().sizeDelta);
        yield return new WaitForSeconds(0.1f);
        //Debug.Log(allPhotoContainer.GetComponent<RectTransform>().sizeDelta);
        //Debug.Log(allPhotoContainer.transform.childCount);
        //allPhotoContainer.GetComponent<ContentSizeFitter>().enabled = true;
        //Debug.Log(allPhotoContainer.GetComponent<RectTransform>().sizeDelta);
        feedUIController.ShowLoader(false);
        //allPhotoContainer.gameObject.SetActive(false);
        //allPhotoContainer.gameObject.SetActive(true);
    }



    //this method is used to other player profile back button.......
    public void OnClickOtherPalyerProfileBackButton()
    {
        feedUIController.feedUiScreen.SetActive(true);
    }

    //this method is used to follow user button click.......
    public void OnClickFollowUserButton()
    {
        apiManager.RequestFollowAUser(FeedRawData.id.ToString(), "MyProfile");
    }

    public void UpdateBackButtonOnClickListener()
    {
        otherProfileScreenBackButton.GetComponent<Button>().onClick.RemoveAllListeners();
        otherProfileScreenBackButton.GetComponent<Button>().onClick.AddListener(() => otherProfileScreenBackButtonAction?.Invoke());
    }

    public void UpdateBackButtonAction(Action _action)
    {
        otherProfileScreenBackButtonAction = _action;
    }

    #endregion


    #region Edit Profile Methods.......
    //this method is used to edit profile button click
    public void OnClickEditProfileButton()
    {
        EditProfileDoneButtonSetUp(true);//setup edit profile done button.......
        editProfileScreen.SetActive(true);
        SetupEditProfileScreen();
        OnScreenTabStateChange?.Invoke(BackButtonHandler.screenTabs.EditProfile);
    }

    void SetupEditProfileScreen()
    {
        editProfileImage.sprite = profileImage.sprite;
        editProfileNameAdvanceInputfield.Text = playerNameText.text;
        GetAvailableTagsFromServer();
        if (myProfileData.userProfile != null)
        {
            editProfileBioInputfield.Text = SNS_APIManager.DecodedString(myProfileData.userProfile.bio);
            if (string.IsNullOrWhiteSpace(editProfileBioInputfield.Text))
            {
                editProfileBioInputfield.Text = "";
            }
            if (myProfileData.userProfile.username == "null" || myProfileData.userProfile.username == "Null")
                editProfileUniqueNameAdvanceInputfield.Text = "";
            else
                editProfileUniqueNameAdvanceInputfield.Text = myProfileData.userProfile.username;
            editProfileBioInputfield.transform.parent.GetComponent<InputFieldHightResetScript>().OnValueChangeAfterResetHeight();
            // Convert Array into List
            userSelectedTags = new List<string>(myProfileData.tags);
        }
    }

    #region Display Tags in Edit Profile Screen
    void GetAvailableTagsFromServer()
    {
        if (availableTagsAtServer == null || availableTagsAtServer.Count == 0)
        {
            availableTagsAtServer = new List<string>();
            StartCoroutine(CallAPI_ForTags());
        }
        else
        {
            // Api Called Already so display tags
            // Update List if user has made some unsave change and reopen edit profile screen
            userSelectedTags = new List<string>(myProfileData.tags);
            Highlight_UserSelectedTag();
        }
    }
    IEnumerator CallAPI_ForTags()
    {
        string api = ConstantsGod.API_BASEURL + ConstantsGod.availableTags;

        using (UnityWebRequest www = UnityWebRequest.Get(api))
        {
            www.SendWebRequest();

            while (!www.isDone)
            {
                yield return null;
            }
                

            if (www.result != UnityWebRequest.Result.ConnectionError && www.result != UnityWebRequest.Result.ProtocolError)
            {
                ConvertJsonStringIntoList(www.downloadHandler.text);
            }
            www.Dispose();
        }
    }
    void ConvertJsonStringIntoList(string availableTags)
    {
        TagDetails tags = JsonUtility.FromJson<TagDetails>(availableTags);
        Debug.Log("Available tags :" + tags.data.count);

        // Store All tags in list
        availableTagsAtServer.AddRange(tags.data.rows.Select(row => row.tagName));

        availableTagsCount = availableTagsAtServer.Count;
        DisplayTags();
    }

   

    void DisplayTags()
    {
        // Each row contains 4 tags: Calculate total rows
        int totalRows = (availableTagsCount + 3) / 4;
        // Generate Rows
        for (int i = 0; i < totalRows; i++)
        {
            GenerateRow_Tags();
        }
    }
    void GenerateRow_Tags()
    {
        GameObject tempRow = Instantiate(tags_row, tags_row_parent);

        // Generate Tags for Each Row
        int tagPerRow = CalculateTagsPerRow(generatedTagCount);
        for (int k = 0; k < tagPerRow && availableTagsCount > generatedTagCount; k++)
        {
            GameObject tempTag = Instantiate(tags_row_obj, tempRow.transform);
            string currentTag = availableTagsAtServer[generatedTagCount];
            tempTag.GetComponent<TagPrefabInfo>().tagName.text = currentTag;

            if (userSelectedTags.Contains(currentTag))
                tempTag.GetComponent<TagPrefabInfo>().Select_UnselectTags();

            generatedTagCount++;
        }

        tempRow.GetComponent<UnityEngine.UI.HorizontalLayoutGroup>().spacing = 18.01f;
    }
    int CalculateTagsPerRow(int startInd)
    {
        int counter = 0;
        for (int i = 0; i < 4; i++)
        {
            if (availableTagsAtServer.Count - 1 < startInd + i)
                break;

            counter += availableTagsAtServer[startInd + i].Length;
        }

        if (counter > 30) // Each row can has Max 35 characters
            return 3;
        else
            return 4;
    }

    void Highlight_UserSelectedTag()
    {
        var tagPrefabInfos = tags_row_parent.GetComponentsInChildren<TagPrefabInfo>();
        foreach (var tagInfo in tagPrefabInfos)
        {
            if (userSelectedTags.Contains(tagInfo.tagName.text))
            {
                tagInfo.Select_UnselectTags();
            }
        }
    }


    public void EnableTagScroller(ScrollRect scrollerObj)
    {
        if (dropDownBtn.transform.localScale.y == 1)
        {
            dropDownBtn.transform.localScale = new Vector3(1, -1, 1);
            scrollerObj.enabled = true;
        }
        else
        {
            dropDownBtn.transform.localScale = Vector3.one;
            scrollerObj.enabled = false;
            scrollerObj.content.localPosition = Vector3.zero;
        }
    }


    public void TestCase()
    {
        generatedTagCount = 0; // generated TagCounter 
        availableTagsCount = 0;
        ConvertJsonStringIntoList(TestingJasonForTags);
    }

    #endregion

    //this method used to Edit profile Back Button click.......
    public void OnClickEditProfileBackButton()
    {
        ProfilePostPartShow();

        if (File.Exists(setImageAvatarTempPath))
        {
            File.Delete(setImageAvatarTempPath);
        }
        setImageAvatarTempPath = "";
        setImageAvatarTempFilename = "";
    }

    //this method is used to set edit profile done button interactable active or disable.......
    public void EditProfileDoneButtonSetUp(bool isActive)
    {
        editProfileDoneButton.interactable = isActive;
    }

   

    public void OnClickEditProfileDoneButton()
    {
        ProfilePostPartShow(true);

        checkEditNameUpdated = 0;
        checkEditInfoUpdated = 0;

        username = playerNameText.text;
        job = "";
        gender = "";
        website = "";
        bio = "";

        if (myProfileData.userProfile != null)
        {
            //job = myProfileData.userProfile.job;
            job = SNS_APIManager.DecodedString(myProfileData.userProfile.job);
            website = myProfileData.userProfile.website;
            bio = SNS_APIManager.DecodedString(myProfileData.userProfile.bio);
            gender = myProfileData.userProfile.gender;
            uniqueUsername = myProfileData.userProfile.username;
        }
        EditProfileDoneButtonSetUp(false);//setup edit profile done button.......
        EditProfileInfoCheckAndAPICalling();
    }

    void EditProfileInfoCheckAndAPICalling()
    {
        string tempStr;
        string keytoLocalize;
        if (!string.IsNullOrEmpty(editProfileNameAdvanceInputfield.Text) && editProfileNameAdvanceInputfield.Text != playerNameText.text)
        {
            tempStr = editProfileNameAdvanceInputfield.Text.Trim();
            username = tempStr;
            checkEditNameUpdated = 1;
            GameManager.Instance.UpdatePlayerName(username);
        }
        else if (string.IsNullOrEmpty(editProfileNameAdvanceInputfield.Text))
        {
            ShowEditProfileNameErrorMessage("Display name can't be empty");
            return;
        }

        if (!string.IsNullOrEmpty(editProfileUniqueNameAdvanceInputfield.Text) && editProfileUniqueNameAdvanceInputfield.Text != uniqueUsername
            && (uniqueUsername != "null" || uniqueUsername != "Null"))
        {
             if (editProfileUniqueNameAdvanceInputfield.Text.Length < 5 || editProfileUniqueNameAdvanceInputfield.Text.Length > 15)
            {
                keytoLocalize = TextLocalization.GetLocaliseTextByKey("The username must be between 5 and 15 characters.");
                ShowEditProfileUniqueNameErrorMessage(keytoLocalize);
                return;
            }
            else if (!editProfileUniqueNameAdvanceInputfield.Text.Any(c => char.IsDigit(c) || c == '_'))
            {
                keytoLocalize = TextLocalization.GetLocaliseTextByKey("The username must not include Space. Alphabet, Numbers, or Underscore allowed.");
                ShowEditProfileUniqueNameErrorMessage(keytoLocalize);
                return;

            }
            
            tempStr = editProfileUniqueNameAdvanceInputfield.Text.Trim();
            tempStr = tempStr.Replace("@", "");
            uniqueUsername = tempStr;
            checkEditInfoUpdated = 1;
        }
        else if (string.IsNullOrEmpty(editProfileUniqueNameAdvanceInputfield.Text))
        {
            ShowEditProfileUniqueNameErrorMessage("Username can't be empty");
            return;
        }

        if (editProfileBioInputfield.Text != bio)
        {
            tempStr = editProfileBioInputfield.Text.Trim();
            bio = tempStr;
            checkEditInfoUpdated = 1;
        }
        else if (string.IsNullOrEmpty(editProfileBioInputfield.Text))
        {
            bio = "";
        }

        tempTags = userSelectedTags.ToArray();
        if (!new HashSet<string>(tempTags).SetEquals(tempMyProfileDataRoot.data.tags))
        {
            checkEditInfoUpdated = 1;
        }

        if (checkEditNameUpdated == 1 && !string.IsNullOrEmpty(username))
        {
            isEditProfileNameAlreadyExists = false;
            apiManager.RequestSetName(username);
        }

        if (checkEditInfoUpdated == 1)
        {
            string countryName = System.Globalization.RegionInfo.CurrentRegion.EnglishName;
            apiManager.RequestUpdateUserProfile(uniqueUsername ?? "", gender ?? "Male", SNS_APIManager.EncodedString(job ?? ""),
                countryName ?? "", website ?? "", SNS_APIManager.EncodedString(bio ?? ""), tempTags);
        }

        if (string.IsNullOrEmpty(setImageAvatarTempPath))
        {
            if (checkEditNameUpdated == 1 || checkEditInfoUpdated == 1)
            {
                StartCoroutine(WaitEditProfileGetUserDetails(false));
            }
            else
            {
                editProfileScreen.SetActive(false);
                EditProfileDoneButtonSetUp(true);
            }
        }
        else
        {
            StartCoroutine(WaitEditProfileGetUserDetails(true));
        }

    }

    public IEnumerator IERequestForWebSiteValidation(string url)
    {
        WWWForm form = new WWWForm();
        form.AddField("url", url);
        Debug.Log("Web URL:" + url);
        using (UnityWebRequest www = UnityWebRequest.Post((ConstantsGod.API_BASEURL + ConstantsGod.r_url_WebsiteValidation), form))
        {
            www.SendWebRequest();
            while (!www.isDone)
            {
                yield return null;
            }

            //feedUIController.ShowLoader(false);

            if (www.result==UnityWebRequest.Result.ConnectionError || www.result==UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
                EditProfileErrorMessageShow(websiteErrorObj);
                Debug.Log("Invalid WebSite");
            }
            else
            {
                string data = www.downloadHandler.text;
                WebSiteValidRoot webSiteValidRoot = JsonConvert.DeserializeObject<WebSiteValidRoot>(data);
                if (webSiteValidRoot.success)
                {
                    if (isUrl)
                    {
                        Uri myUri = new Uri(website);
                        //website = myUri.Host;
                    }
                    Debug.Log("final result Web Str:" + website);

                    EditProfileInfoCheckAndAPICalling();
                    Debug.Log("Valid WebSite:");
                }
                else
                {
                    EditProfileErrorMessageShow(websiteErrorObj);
                    Debug.Log("Invalid WebSite");
                }
            }
            www.Dispose();
        }
    }

    //this method is used to show web site error message.......
    void EditProfileErrorMessageShow(GameObject currentOBJ)
    {
        if (editProfileErrorCo != null)
        {
            StopCoroutine(editProfileErrorCo);
            currentEditProfileErrorMessgaeObj.SetActive(false);
        }
        currentOBJ.SetActive(true);
        currentEditProfileErrorMessgaeObj = currentOBJ;
        editProfileErrorCo = StartCoroutine(WaitUntilErrorAnimationFinished());
    }

    //this coroutine is used to show and wait until finish error message animation.......
    IEnumerator WaitUntilErrorAnimationFinished()
    {
        yield return new WaitForSeconds(2f);
        EditProfileDoneButtonSetUp(true);//setup edit profile done button.......
        currentEditProfileErrorMessgaeObj.SetActive(false);
    }

    //this method is used to show edit profile name error message.......
    public void ShowEditProfileNameErrorMessage(string msg)
    {
        if (msg == "Username already exists")
        {
            isEditProfileNameAlreadyExists = true;
        }
        nameErrorMessageObj.GetComponent<Text>().text = TextLocalization.GetLocaliseTextByKey(msg);
        EditProfileErrorMessageShow(nameErrorMessageObj);
    }
    public void ShowEditProfileUniqueNameErrorMessage(string msg)
    {
        uniqueNameErrorMessageObj.GetComponent<Text>().text = TextLocalization.GetLocaliseTextByKey(msg);
        EditProfileErrorMessageShow(uniqueNameErrorMessageObj);
    }

    //this coroutine is used check and GetUserDetails or UploadAvatar api call.......
    IEnumerator WaitEditProfileGetUserDetails(bool isProfileUpdate)
    {
        feedUIController.ShowLoader(true);
        yield return new WaitForSeconds(1f);
        if (!isProfileUpdate)
        {
            apiManager.RequestGetUserDetails("EditProfileAvatar");//Get My Profile data 
        }
        else
        {
            Debug.Log("=========Uploading profile pic :" + setImageAvatarTempPath);
            //Debug.Log("=========Uploading profile pic Temp name :" + setImageAvatarTempFilename);
            AWSHandler.Instance.PostAvatarObject(setImageAvatarTempPath, setImageAvatarTempFilename, "EditProfileAvatar");//upload avatar image on AWS.
        }
    }

    //this method is used to Bio Button click.......
    public void OnEditBioButtonClick()
    {
        editProfileBioScreen.SetActive(true);
        bioEditAdvanceInputField.Text = editProfileBioInputfield.Text;
    }

    //this method is used to edit bio Back Button click.......
    public void OnEditBioBackButtonClick()
    {
        if (editProfileErrorCo != null)
        {
            StopCoroutine(editProfileErrorCo);
            currentEditProfileErrorMessgaeObj.SetActive(false);
        }
        editProfileBioScreen.SetActive(false);
    }

    //this method is used to edit bio Done Button click.......
    public void OnEditBioDoneButtonClick()
    {
        if (editProfileErrorCo != null)
        {
            StopCoroutine(editProfileErrorCo);
            currentEditProfileErrorMessgaeObj.SetActive(false);
        }
        editProfileBioScreen.SetActive(false);

        string resultString = Regex.Replace(bioEditAdvanceInputField.Text.ToString(), @"^\s*$[\r\n]*", string.Empty, RegexOptions.Multiline);

        editProfileBioInputfield.Text = resultString;
        editProfileBioInputfield.transform.parent.GetComponent<InputFieldHightResetScript>().OnValueChangeAfterResetHeight();
    }

    //this method is used to change Profile Button click.......
    public void OnClickChangeProfilePicButton()
    {
        mainFullScreenContainer.SetActive(false);//fo disable profile screen post part.......
        pickImageOptionScreen.SetActive(true);
    }

    //this method is used to show profile post main part.......
    public void ProfilePostPartShow(bool _resetProfileScreen = false)
    {
        if (!mainFullScreenContainer.activeSelf)
            mainFullScreenContainer.SetActive(true);//fo disable profile screen post part.......
        if (_resetProfileScreen && allPhotoContainer.childCount > 0)
        {
            loadedMyPostAndVideoId.Clear();
            for (int i = 0; i < allPhotoContainer.childCount; i++)
            {
                Destroy(allPhotoContainer.GetChild(i).gameObject);
            }
            Invoke(nameof(ProfileTabButtonClick), 5);
        }
        else
        {
             Invoke(nameof(ProfileTabButtonClick), 5);
        }
    }


    //this method is used to pick group avatar from gellery for group avatar.
    public void OnPickImageFromGellery(int maxSize)
    {
#if UNITY_IOS
        if (permissionCheck == "false")
        {
            string url = MyNativeBindings.GetSettingsURL();
            Debug.Log("the settings url is:" + url);
            Application.OpenURL(url);
        }
        else
        {
            iOSCameraPermission.VerifyPermission(gameObject.name, "SampleCallback");
        }
          setImageAvatarTempPath = "";
        setImageAvatarTempFilename = "";
        //setGroupFromCamera = false;

        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {
                if (pickImageOptionScreen.activeSelf)//false meadia option screen.
                {
                    pickImageOptionScreen.SetActive(false);
                }

                // Create Texture from selected image
                Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize, false);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }

                //setGroupTempAvatarTexture = texture;

               Debug.Log("OnPickGroupAvatarFromGellery path: " + path);

                //string[] pathArry = path.Split('/');

                //string fileName = pathArry[pathArry.Length - 1];
                string fileName = Path.GetFileName(path);
               Debug.Log("OnPickGroupAvatarFromGellery FileName: " + fileName);

                string[] fileNameArray = fileName.Split('.');
                string str = DateTime.Now.Day + "_" + DateTime.Now.Month + "_" + DateTime.Now.Year + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + ".";
                fileName = fileNameArray[0] + str + fileNameArray[1];

                setImageAvatarTempPath = Path.Combine(Application.persistentDataPath, "XanaChat", fileName); ;
                setImageAvatarTempFilename = fileName;

                Crop(texture, setImageAvatarTempPath);

                //editProfileImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
            }
        });
        Debug.Log("Permission result: " + permission);
       
#elif UNITY_ANDROID
        setImageAvatarTempPath = "";
        setImageAvatarTempFilename = "";
        //setGroupFromCamera = false;

        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {
                if (pickImageOptionScreen.activeSelf)//false meadia option screen.
                {
                    pickImageOptionScreen.SetActive(false);
                }

                // Create Texture from selected image
                Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize, false);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }

                //setGroupTempAvatarTexture = texture;

                Debug.Log("OnPickGroupAvatarFromGellery path: " + path);

                //string[] pathArry = path.Split('/');

                //string fileName = pathArry[pathArry.Length - 1];
                string fileName = Path.GetFileName(path);
                Debug.Log("OnPickGroupAvatarFromGellery FileName: " + fileName);

                string[] fileNameArray = fileName.Split('.');
                string str = DateTime.Now.Day + "_" + DateTime.Now.Month + "_" + DateTime.Now.Year + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + ".";
                fileName = fileNameArray[0] + str + fileNameArray[1];

                setImageAvatarTempPath = Path.Combine(Application.persistentDataPath, "UserProfilePic", fileName); ;
                setImageAvatarTempFilename = fileName;

                Crop(texture, setImageAvatarTempPath);

            }
        });

        if (permission != NativeGallery.Permission.Granted)
        {
            using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject currentActivityObject = unityClass.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                string packageName = currentActivityObject.Call<string>("getPackageName");

                using (var uriClass = new AndroidJavaClass("android.net.Uri"))
                using (AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("fromParts", "package", packageName, null))
                using (var intentObject = new AndroidJavaObject("android.content.Intent", "android.settings.APPLICATION_DETAILS_SETTINGS", uriObject))
                {
                    intentObject.Call<AndroidJavaObject>("addCategory", "android.intent.category.DEFAULT");
                    intentObject.Call<AndroidJavaObject>("setFlags", 0x10000000);
                    currentActivityObject.Call("startActivity", intentObject);
                }
            }
        }
        Debug.Log("Permission result: " + permission);
#endif
    }

    //this method is used to take picture from camera for group avatar.
    public void OnPickImageFromCamera(int maxSize)
    {
#if UNITY_IOS
        if (permissionCheck == "false")
        {
            string url = MyNativeBindings.GetSettingsURL();
            Debug.Log("the settings url is:" + url);
            Application.OpenURL(url);
        }
        else
        {
            iOSCameraPermission.VerifyPermission(gameObject.name, "SampleCallback");
        }
         setImageAvatarTempPath = "";
        setImageAvatarTempFilename = "";
        //setGroupFromCamera = false;
        NativeCamera.Permission permission = NativeCamera.TakePicture((path) =>
        {
            if (path != null)
            {
                if (pickImageOptionScreen.activeSelf)//false meadia option screen.
                {
                    pickImageOptionScreen.SetActive(false);
                }
                // Create a Texture2D from the captured image
                Texture2D texture = NativeCamera.LoadImageAtPath(path, maxSize, false);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }

                //setGroupTempAvatarTexture = texture;

               Debug.Log("OnGroupAvatarTakePicture Camera ImagePath : " + path);

                //string[] pathArry = path.Split('/');

                //string fileName = pathArry[pathArry.Length - 1];
                string fileName = Path.GetFileName(path);
               Debug.Log("Camera filename : " + fileName);

                string[] fileNameArray = fileName.Split('.');
                string str = DateTime.Now.Day + "_" + DateTime.Now.Month + "_" + DateTime.Now.Year + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + ".";
                fileName = fileNameArray[0] + str + fileNameArray[1];

                string filePath = Path.Combine(Application.persistentDataPath, "XanaChat", fileName);

               Debug.Log("Camera filePath:" + filePath + "    :filename:" + fileName + "   :texture width:" + texture.width + " :height:" + texture.height);

                setImageAvatarTempPath = filePath;
                setImageAvatarTempFilename = fileName;
                //setGroupFromCamera = true;

                Crop(texture, setImageAvatarTempPath);
                //editProfileImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
            }
        }, maxSize);
         
#elif UNITY_ANDROID
        setImageAvatarTempPath = "";
        setImageAvatarTempFilename = "";

        //StartCoroutine(OpenWebCamPhotoCamera());
        //return;
        //setGroupFromCamera = false;
        NativeCamera.Permission permission = NativeCamera.TakePicture((path) =>
        {
            if (path != null)
            {
                if (pickImageOptionScreen.activeSelf)//false meadia option screen.
                {
                    pickImageOptionScreen.SetActive(false);
                }
                // Create a Texture2D from the captured image
                Texture2D texture = NativeCamera.LoadImageAtPath(path, maxSize, false);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }

                //setGroupTempAvatarTexture = texture;

                Debug.Log("OnGroupAvatarTakePicture Camera ImagePath : " + path);

                //string[] pathArry = path.Split('/');

                //string fileName = pathArry[pathArry.Length - 1];
                string fileName = Path.GetFileName(path);
                Debug.Log("Camera filename : " + fileName);

                string[] fileNameArray = fileName.Split('.');
                string str = DateTime.Now.Day + "_" + DateTime.Now.Month + "_" + DateTime.Now.Year + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + ".";
                fileName = fileNameArray[0] + str + fileNameArray[1];

                string filePath = Path.Combine(Application.persistentDataPath, "UserProfilePic", fileName);

                Debug.Log("Camera filePath:" + filePath + "    :filename:" + fileName + "   :texture width:" + texture.width + " :height:" + texture.height);

                setImageAvatarTempPath = filePath;
                setImageAvatarTempFilename = fileName;
                //setGroupFromCamera = true;

                Crop(texture, setImageAvatarTempPath);
                //editProfileImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
            }
        }, maxSize);

        if (permission != NativeCamera.Permission.Granted)
        {
            using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject currentActivityObject = unityClass.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                string packageName = currentActivityObject.Call<string>("getPackageName");

                using (var uriClass = new AndroidJavaClass("android.net.Uri"))
                using (AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("fromParts", "package", packageName, null))
                using (var intentObject = new AndroidJavaObject("android.content.Intent", "android.settings.APPLICATION_DETAILS_SETTINGS", uriObject))
                {
                    intentObject.Call<AndroidJavaObject>("addCategory", "android.intent.category.DEFAULT");
                    intentObject.Call<AndroidJavaObject>("setFlags", 0x10000000);
                    currentActivityObject.Call("startActivity", intentObject);
                }
            }
        }
        Debug.Log("Permission result: " + permission);
#endif
    }


    public void OnClickTakePhotoFromWebCam()
    {
        StartCoroutine(TakePhotoFromWebCam());
    }
    IEnumerator TakePhotoFromWebCam()  // Start this Coroutine on some button click
    {
        // NOTE - you almost certainly have to do this here:

        yield return new WaitForEndOfFrame();

        if (pickImageOptionScreen.activeSelf)//false meadia option screen.
        {
            pickImageOptionScreen.SetActive(false);
        }

        // it's a rare case where the Unity doco is pretty clear,
        // http://docs.unity3d.com/ScriptReference/WaitForEndOfFrame.html
        // be sure to scroll down to the SECOND long example on that doco page 
        Texture2D photo = new Texture2D(webCamTexture.width, webCamTexture.height);
        photo.SetPixels(webCamTexture.GetPixels());
        photo.Apply();

        string fileName = "CapturePhoto";
        Debug.Log("Camera filename : " + fileName);

        string str = DateTime.Now.Day + "_" + DateTime.Now.Month + "_" + DateTime.Now.Year + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + ".";
        fileName = fileName + str + ".png";

        string filePath = Path.Combine(Application.persistentDataPath, "UserProfilePic", fileName);

        Debug.Log("Camera filePath:" + filePath + "    :filename:" + fileName);

        setImageAvatarTempPath = filePath;
        setImageAvatarTempFilename = fileName;

        webCamTexture.Stop();
        //webcamScreen.SetActive(false);
        Crop(photo, setImageAvatarTempPath);
    }
    #endregion


    #region Get Image From AWS
    public void GetImageFromAWS(string key, Image mainImage)
    {
        //Debug.Log("My Profile GetImageFromAWS key:" + key);
        if (AssetCache.Instance.HasFile(key))
        {
            AssetCache.Instance.LoadSpriteIntoImage(mainImage, key, changeAspectRatio: true);
            return;
        }
        else
        {
            AssetCache.Instance.EnqueueOneResAndWait(key, (ConstantsGod.r_AWSImageKitBaseUrl + key), (success) =>
            {
                if (success)
                {
                    AssetCache.Instance.LoadSpriteIntoImage(mainImage, key, changeAspectRatio: true);
                }
            });
        }
    }

   

    public bool CheckUrlDropboxOrNot(string url)
    {
        string[] splitArray = url.Split(':');
        if (splitArray.Length > 0)
        {
            if (splitArray[0] == "https" || splitArray[0] == "http")
            {
                return true;
            }
        }
        return false;
    }

    public string GetWebDomainFromUrl(string url)
    {
        string[] splitArray = url.Split(':');
        if (splitArray.Length > 0)
        {
            if (splitArray[0] == "https" || splitArray[0] == "http")
            {
                return splitArray[1];
            }
        }
        return url;
    }
    #endregion

    #region Profile Image Crop.......
    //crop mate .......
    public void Crop(Texture2D LoadedTexture, string path)
    {
        // If image cropper is already open, do nothing
        if (ImageCropper.Instance.IsOpen)
            return;

        StartCoroutine(SetImageCropper(LoadedTexture, path));

        //Invoke("ProfilePostPartShow", 1f);
    }

    private IEnumerator SetImageCropper(Texture2D screenshot, string path)
    {
        yield return new WaitForEndOfFrame();

        bool ovalSelection = true;
        bool autoZoom = true;

        float minAspectRatio = 1, maxAspectRatio = 1;

        ImageCropper.Instance.Show(screenshot, (bool result, Texture originalImage, Texture2D croppedImage) =>
        {
            // If screenshot was cropped successfully
            if (result)
            {
                Sprite s = Sprite.Create(croppedImage, new Rect(0, 0, croppedImage.width, croppedImage.height), new Vector2(0, 0), 1);
                editProfileImage.sprite = s;

                try
                {
                    byte[] bytes = croppedImage.EncodeToPNG();
                    File.WriteAllBytes(path, bytes);
                    Debug.Log("File SAVE");
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }
            else
            {
                //Debug.Log("--------Image not cropped");
                setImageAvatarTempPath = "";
                //croppedImageHolder.enabled = false;
                //croppedImageSize.enabled = false;
            }
            // Destroy the screenshot as we no longer need it in this case
            Destroy(screenshot);
            Resources.UnloadUnusedAssets();
            //Caching.ClearCache();
            //GC.Collect();
            Invoke("ProfilePostPartShow", 0.5f);
        },
        settings: new ImageCropper.Settings()
        {
            ovalSelection = ovalSelection,
            autoZoomEnabled = autoZoom,
            imageBackground = Color.clear, // transparent background
            selectionMinAspectRatio = minAspectRatio,
            selectionMaxAspectRatio = maxAspectRatio,
            markTextureNonReadable = false
        },
        croppedImageResizePolicy: (ref int width, ref int height) =>
        {
            // uncomment lines below to save cropped image at half resolution
            //width /= 2;
            //height /= 2;
        });
    }
    #endregion

    #region Profile Follower and Following list Screen Methods
    //this method is used to profile follower button click.......
    public void OnClickFollowerButton()
    {
        //feedUIController.ProfileFollowerFollowingScreenSetup(0, topHaderUserNameText.text);
        feedUIController.OnClickProfileFollowerButton();
        //if (apiManager.profileAllFollowerRoot.data.rows.Count != myProfileData.followerCount)
        //{
        //    //feedUIController.ProfileFollowerFollowingListClear();

        //    //feedUIController.ShowLoader(true);
        //    feedUIController.isProfileFollowerDataLoaded = false;
        //    apiManager.RequestGetAllFollowersFromProfile(myProfileData.id.ToString(), 1, 50);

        //    //if (followingCo != null)
        //    //{
        //    //    StopCoroutine(followingCo);
        //    //}
        //    //followingCo = StartCoroutine(WaitToCallFollowing());
        //}
    }

    IEnumerator WaitToCallFollowing()
    {
        yield return new WaitForSeconds(1f);
        feedUIController.isProfileFollowingDataLoaded = false;
        apiManager.RequestGetAllFollowingFromProfile(myProfileData.id.ToString(), 1, 50);
    }

    //this method is used to profile Following button click.......
    public void OnClickFollowingButtton()
    {
        //feedUIController.ProfileFollowerFollowingScreenSetup(1, topHaderUserNameText.text);
        feedUIController.OnClickProfileFollowingButton();
        //if (apiManager.profileAllFollowingRoot.data.rows.Count != myProfileData.followingCount)
        //{
        //    //feedUIController.ProfileFollowerFollowingListClear();

        //    //feedUIController.ShowLoader(true);
        //    feedUIController.isProfileFollowingDataLoaded = false;
        //    apiManager.RequestGetAllFollowingFromProfile(myProfileData.id.ToString(), 1, 50);

        //    //if (followeCo != null)
        //    //{
        //    //    StopCoroutine(followeCo);
        //    //}
        //    //followeCo = StartCoroutine(WaitToFollower());
        //}
    }

    IEnumerator WaitToFollower()
    {
        yield return new WaitForSeconds(1f);
        feedUIController.isProfileFollowerDataLoaded = false;
        apiManager.RequestGetAllFollowersFromProfile(myProfileData.id.ToString(), 1, 50);
    }
    #endregion

    #region my profile Data API
    public void RequestGetUserDetails()
    {
        //Commented in order to make profile 2.0 work after ahsan removed old feedui object from scene ----- UMER
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(IERequestGetUserDetails());
        }
    }
    public IEnumerator IERequestGetUserDetails()
    {
        WWWForm form = new WWWForm();
        using (UnityWebRequest www = UnityWebRequest.Get((ConstantsGod.API_BASEURL + ConstantsGod.r_url_GetUserDetails)))
        {
            www.SetRequestHeader("Authorization", apiManager.userAuthorizeToken);

            www.SendWebRequest();
            while(!www.isDone)
            {
                yield return null;
            }

            if (www.result==UnityWebRequest.Result.ConnectionError || www.result==UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("IERequestGetUserDetails error:" + www.error);
            }
            else
            {
                string data = www.downloadHandler.text;
               // Debug.Log("IERequestGetUserDetails Loaded Completed data:" + data);
                tempMyProfileDataRoot = JsonUtility.FromJson<GetUserDetailRoot>(data);
                myProfileData = tempMyProfileDataRoot.data;
                OnlyLoadDataMyProfile();//set data                
            }
            www.Dispose();
        }
    }
    public void OnlyLoadDataMyProfile()
    {
        totalFollowerText.text = myProfileData.followerCount.ToString();
        totalFollowingText.text = myProfileData.followingCount.ToString();
        if (string.IsNullOrEmpty(tempMyProfileDataRoot.data.avatar) && !profileMakedFlag)
        {
            profileMakedFlag = true;
            ProfilePictureManager.instance.MakeProfilePicture(tempMyProfileDataRoot.data.name);
        }
        else if (ConstantsHolder.xanaConstants.userProfileLink != tempMyProfileDataRoot.data.avatar)
        {
            UpdateProfilePic();
        }
        //if (socketController == null)
        //{
        //    socketController = HomeScoketHandler.instance;
        //}
        //socketController.ConnectSNSSockets(apiManager.userId);

    }
    #endregion

    #region Permission Methods
    public void RequestPermission()
    {
        if (UniAndroidPermission.IsPermitted(AndroidPermission.CAMERA))
        {
            Debug.Log("CAMERA is already permitted!!");
            return;
        }
        UniAndroidPermission.RequestPermission(AndroidPermission.CAMERA, OnAllow, OnDeny, OnDenyAndNeverAskAgain);
    }

    private void OnAllow()
    {
        Debug.Log("CAMERA is permitted NOW!!");
    }

    private void OnDeny()
    {
        Debug.Log("CAMERA is NOT permitted...");
    }

    private void OnDenyAndNeverAskAgain()
    {
        Debug.Log("CAMERA is NOT permitted and checked never ask again option");

        using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (AndroidJavaObject currentActivityObject = unityClass.GetStatic<AndroidJavaObject>("currentActivity"))
        {
            string packageName = currentActivityObject.Call<string>("getPackageName");

            using (var uriClass = new AndroidJavaClass("android.net.Uri"))
            using (AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("fromParts", "package", packageName, null))
            using (var intentObject = new AndroidJavaObject("android.content.Intent", "android.settings.APPLICATION_DETAILS_SETTINGS", uriObject))
            {
                intentObject.Call<AndroidJavaObject>("addCategory", "android.intent.category.DEFAULT");
                intentObject.Call<AndroidJavaObject>("setFlags", 0x10000000);
                currentActivityObject.Call("startActivity", intentObject);
            }
        }
    }

   
    private void SampleCallback(string permissionWasGranted)
    {
        Debug.Log("Callback.permissionWasGranted = " + permissionWasGranted);

        if (permissionWasGranted == "true")
        {
            // You can now use the device camera.
        }
        else
        {
            permissionCheck = permissionWasGranted;

            // permission denied, no access should be visible, when activated when requested permission
            return;

            // You cannot use the device camera.  You may want to display a message to the user
            // about changing the camera permission in the Settings app.
            // You may want to re-enable the button to display the Settings message again.
        }
    }
    #endregion
}

[System.Serializable]
public class TagDetails
{
    public bool success;
    public data data;
    public string msg;
}