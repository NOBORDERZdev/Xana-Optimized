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

    public string defaultUrl = "https://";

    public GetUserDetailData myProfileData = new GetUserDetailData();

    public List<AllFeedByUserIdRow> allMyFeedImageRootDataList = new List<AllFeedByUserIdRow>();//image feed list
    public List<AllFeedByUserIdRow> allMyFeedVideoRootDataList = new List<AllFeedByUserIdRow>();//video feed list
    public List<AllFeedByUserIdRow> allMyFeedInFeedPageRootDataList = new List<AllFeedByUserIdRow>();//video feed list

    public AllFeedByUserIdRoot currentPageAllFeedWithUserIdRoot = new AllFeedByUserIdRoot();

    private AllUserWithFeedRow FeedRawData;

    [Space]
    [Header("Screen References")]
    public GameObject myProfileScreen;
    public GameObject editProfileScreen;
    public GameObject pickImageOptionScreen;

    [Space]
    [Header("Profile Screen Refresh Object")]
    public GameObject mainFullScreenContainer;
    public GameObject mainProfileDetailPart;
    public GameObject userPostPart;
    public GameObject bioDetailPart;
    public GameObject bioTxtParent;

    [Space]
    [Header("Player info References")]
    public TextMeshProUGUI topHaderUserNameText;
    public Image profileImage;
    public TextMeshProUGUI totalPostText;
    public TextMeshProUGUI totalFollowerText;
    public TextMeshProUGUI totalFollowingText;
    [Space]
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI jobText;
    public TextMeshProUGUI textUserBio;
    public TextMeshProUGUI websiteText;

    public GameObject seeMoreBioButton;
    public GameObject seeMoreButtonTextObj;
    public GameObject seeLessButtonTextObj;

    [Space]
    [Header("Photo, Movie, NFT Button Panel Tab panel Reference")]
    public string CurrentSection;
    public ScrollRectGiftScreen tabScrollRectGiftScreen;
    public ParentHeightResetScript parentHeightResetScript;
    public SelectionItemScript selectionItemScript1;
    public SelectionItemScript selectionItemScript2;

    [Space]
    [Header("Follow Message Button References")]
    public Image followButtonImage;
    public Sprite followSprite, followingSprite;
    public TextMeshProUGUI followFollowingText;
    public Color followTextColor, FollowingTextColor;

    [Space]
    [Header("Player Uploaded Item References")]
    public Transform mainPostContainer;
    public Transform allPhotoContainer;
    public Transform allTagContainer;
    public Transform allMovieContainer;
    public Transform allOwnedNFTContainer;
    public GameObject photoPrefab;
    public GameObject photoPrefabInMyPostFeed;
    public GameObject NFTImagePrefab;

    [Header("post empty message reference")]
    public GameObject createYourFirstPostMsgObj;
    public GameObject emptyPhotoPostMsgObj;
    public GameObject emptyMoviePostMsgObj;
    public GameObject FooterCreateIcon;

    [Space]
    public GameObject tabPrivateObject;
    public GameObject tabPublicObject;

    [Space]
    public Sprite defultProfileImage;

    [Space]
    [Header("Edit Profile Reference")]
    public Image editProfileImage;
    //public TMP_InputField editProfileNameInputfield;
    //public InputField editProfileNameInputfield;
    public AdvancedInputField editProfileNameAdvanceInputfield;
    //public TMP_InputField editProfileJobInputfield;
    //public InputField editProfileJobInputfield;
    public AdvancedInputField editProfileJobAdvanceInputfield;
    //public TMP_InputField editProfileWebsiteInputfield;
    //public InputField editProfileWebsiteInputfield;
    public AdvancedInputField editProfileWebsiteAdvanceInputfield;
    public TMP_InputField editProfileBioInputfield;
    //public InputField editProfileBioInputfield;
    //public TMP_InputField editProfileGenderInputfield;
    public InputField editProfileGenderInputfield;
    public GameObject editProfilemainInfoPart;
    public GameObject websiteErrorObj;
    public GameObject nameErrorMessageObj;
    public Button editProfileDoneButton;
    bool isEditProfileNameAlreadyExists;

    [Space]
    public GameObject editProfileBioScreen;
    //public TMP_InputField bioEditInputField;
    //public InputField bioEditInputField;

    public AdvancedInputField bioEditAdvanceInputField;

    [Space]
    [Header("For API Pagination")]
    public ScrollRectFasterEx profileMainScrollRectFasterEx;
    public bool isFeedLoaded = false;
    public int profileFeedAPiCurrentPageIndex = 1;
    public float VerticalPosition;

    [Space]
    [Header("Premium UserRole Referense")]
    public UserRolesView userRolesView;
    private bool NFTShowingOnneBool;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        NFTShowingOnneBool = false;
    }

    int tempOPCount = 0;
    bool tempLogout = false;
    private void OnEnable()
    {
        if (tempOPCount == 0)
        {
            userRolesView.SetUpUserRole(ConstantsGod.UserPriorityRole, ConstantsGod.UserRoles);//this method is used to set user role.......
            tempOPCount++;
        }
        else
        {
            if (tempLogout)
            {
                tempLogout = false;
                StartCoroutine(WaitToRefreshProfileScreen());
            }
        }
    }

    private void Start()
    {
        ClearDummyData();//clear dummy data.......

        string saveDir = Path.Combine(Application.persistentDataPath, "XanaChat");
        if (!Directory.Exists(saveDir))
        {
            Directory.CreateDirectory(saveDir);
        }

        if (GlobalVeriableClass.callingScreen == "Profile")
        {
            //ProfileTabButtonClick();
            myProfileScreen.SetActive(true);
        }
        InvokeRepeating(nameof(RequestGetUserDetails), 0f, 2f);
        //string countryName = System.Globalization.RegionInfo.CurrentRegion.EnglishName;
        //Debug.LogError("Country Name:" + countryName + "    Name:"+ System.Globalization.RegionInfo.CurrentRegion.Name);
    }

    #region Profile screen methods
    //this method is used to clear the dummy data.......
    public void ClearDummyData()
    {
        playerNameText.text = "";
        topHaderUserNameText.text = "";
        topHaderUserNameText.GetComponent<LayoutElement>().enabled = false;
        jobText.text = "";
        jobText.gameObject.SetActive(false);
        textUserBio.text = "";
        websiteText.text = "";
        websiteText.gameObject.SetActive(false);
        profileImage.sprite = defultProfileImage;
    }

    //this method is used to clear my profile data after logout.......
    public void ClearAndResetAfterLogout()
    {
        userRolesView.ResetBadges();
        loadedMyPostAndVideoId.Clear();  //amit-19-3-2022 onlogout clear feed id list 
        loadedMyPostAndVideoIdInFeedPage.Clear();  //Riken 
        ClearDummyData();
        tempLogout = true;
        MyProfileSceenShow(false);
        UserRegisterationManager.instance.userRoleObj.userNftRoleSlist.Clear();
    }

    //this method is used to Profile Tab Button Click.......
    public void ProfileTabButtonClick()
    {
        APIManager.Instance.RequestGetUserDetails("myProfile");//Get My Profile data       
        MyProfileSceenShow(true);//active myprofile screen
    }

    //this method is used to my profile screen show or not.......
    public void MyProfileSceenShow(bool isShow)
    {
        myProfileScreen.SetActive(isShow);
        if (!isShow)
        {
            SetupEmptyMsgForPhotoTab(true);//check for empty message.......
        }
    }

    //this method is used to setup data after get api response.......
    public void SetupData(GetUserDetailData myData, string callingFrom)
    {
        myProfileData = myData;
        //Debug.LogError(callingFrom);
        if (callingFrom == "EditProfileAvatar")
        {
            FeedUIController.Instance.ShowLoader(false);
            EditProfileDoneButtonSetUp(true);//setup edit profile done button.......
            if (!isEditProfileNameAlreadyExists)
            {
                editProfileScreen.SetActive(false);
            }
            isEditProfileNameAlreadyExists = false;
            //Debug.LogError("Profile Update Success and delete file");
            if (File.Exists(setImageAvatarTempPath))
            {
                File.Delete(setImageAvatarTempPath);

                UpdateAavtarUrlOfAllMyFeed();
            }
            /*if (setGroupFromCamera)
            {
                if (File.Exists(setImageAvatarTempPath))
                {
                    File.Delete(setImageAvatarTempPath);
                }
            }*/
            if (AssetCache.Instance.HasFile(setImageAvatarTempFilename))
            {
                //Debug.LogError("IOS update Profile Pic Delete");
                AssetCache.Instance.DeleteAsset(setImageAvatarTempFilename);
            }
            //setGroupFromCamera = false;
            setImageAvatarTempPath = "";
            setImageAvatarTempFilename = "";

            LoadDataMyProfile();//set data
        }
        else
        {
            LoadDataMyProfile();//set data
            APIManager.Instance.RequestGetFeedsByUserId(APIManager.Instance.userId, 1, 40, "MyProfile");
        }
    }

    bool isSetTempSpriteAfterUpdateAvatar = false;
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
        UserPostItem[] userPostItems = mainPostContainer.GetComponentsInChildren<UserPostItem>();
        //Debug.LogError("UpdateAavtarUrlOfAllMyFeed Length:" + userPostItems.Length);
        for (int i = 0; i < userPostItems.Length; i++)
        {
            //Debug.LogError("ID:" + userPostItems[i].userData.Id);
            userPostItems[i].avtarUrl = myProfileData.avatar;
        }
    }

    string lastTopUserText;
    //this method is used to my profile data set.......
    public void LoadDataMyProfile()
    {
        userRolesView.SetUpUserRole(ConstantsGod.UserPriorityRole, ConstantsGod.UserRoles);//this method is used to set user role.......

        topHaderUserNameText.GetComponent<LayoutElement>().enabled = false;

        playerNameText.text = myProfileData.name;
        topHaderUserNameText.text = myProfileData.name;

        if (lastTopUserText != myProfileData.name)
        {
            topHaderUserNameText.GetComponent<ResetPrefferedWidthScript>().SetupObjectWidth();
        }
        lastTopUserText = myProfileData.name;

        totalFollowerText.text = myProfileData.followerCount.ToString();
        totalFollowingText.text = myProfileData.followingCount.ToString();
        totalPostText.text = myProfileData.feedCount.ToString();

        if (string.IsNullOrEmpty(myProfileData.userProfile.website))
        {
            websiteText.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Profile Website:" + myProfileData.userProfile.website);
            Uri uriResult;
            bool result = Uri.TryCreate(myProfileData.userProfile.website, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            if (result)
            {
                Debug.Log("Given URL is valid");
                Uri websiteHost = new Uri(myProfileData.userProfile.website);
                websiteText.text = websiteHost.Host.ToString();
            }
            else
            {
                Debug.Log("Given URL is Invalid");
                websiteText.text = myProfileData.userProfile.website.ToString();
            }
            websiteText.gameObject.SetActive(true);
        }

        if (myProfileData.userProfile != null)
        {
            if (!string.IsNullOrEmpty(myProfileData.userProfile.job))
            {
                jobText.text = APIManager.DecodedString(myProfileData.userProfile.job);
                jobText.gameObject.SetActive(true);
            }
            else
            {
                jobText.gameObject.SetActive(false);
            }

            if (!string.IsNullOrEmpty(myProfileData.userProfile.bio))
            {
                textUserBio.text = APIManager.DecodedString(myProfileData.userProfile.bio);
                SetupBioPart(textUserBio.text);//check and show only 10 line.......
            }
            else
            {
                //textUserBio.text = "You have no bio yet.";
                seeMoreBioButton.SetActive(false);
                textUserBio.text = TextLocalization.GetLocaliseTextByKey("You have no bio yet.");
            }
        }
        else
        {
            //textUserBio.text = "You have no bio yet.";
            seeMoreBioButton.SetActive(false);
            textUserBio.text = TextLocalization.GetLocaliseTextByKey("You have no bio yet.");
        }

        //Debug.LogError("isSetTempSpriteAfterUpdateAvatar:" + isSetTempSpriteAfterUpdateAvatar);
        if (!isSetTempSpriteAfterUpdateAvatar)//if temp avatar set is true then do not add default profile image.......
        {
            profileImage.sprite = defultProfileImage;
        }
        isSetTempSpriteAfterUpdateAvatar = false;

        if (!string.IsNullOrEmpty(myProfileData.avatar))
        {
            //Debug.LogError("My profile Avatar :-" + myProfileData.avatar);
            bool isUrlContainsHttpAndHttps = APIManager.Instance.CheckUrlDropboxOrNot(myProfileData.avatar);
            if (isUrlContainsHttpAndHttps)
            {
                AssetCache.Instance.EnqueueOneResAndWait(myProfileData.avatar, myProfileData.avatar, (success) =>
                {
                    if (success)
                    {
                        AssetCache.Instance.LoadSpriteIntoImage(profileImage, myProfileData.avatar, changeAspectRatio: true);
                    }
                });
            }
            else
            {
                GetImageFromAWS(myProfileData.avatar, profileImage);
            }
        }
        else
        {
            profileImage.sprite = defultProfileImage;
        }

        StartCoroutine(WaitToRefreshProfileScreen());
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
    IEnumerator WaitToRefreshProfileScreen()
    {
        Debug.Log("Enter in Content Size Filter Section");
        textUserBio.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        yield return new WaitForSeconds(0.01f);
        textUserBio.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        bioTxtParent.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        yield return new WaitForSeconds(0.01f);
        bioTxtParent.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        bioDetailPart.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        yield return new WaitForSeconds(0.01f);
        bioDetailPart.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        mainProfileDetailPart.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        yield return new WaitForSeconds(0.01f);
        mainProfileDetailPart.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        mainFullScreenContainer.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        yield return new WaitForSeconds(0.01f);
        mainFullScreenContainer.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    string tempBioOnly10LineStr = "";
    //this method is used to setup bio text.......
    public void SetupBioPart(string bioText)
    {
        int numLines = bioText.Split('\n').Length;
        //Debug.LogError("Bio Line Count:" + numLines);

        if (numLines > 10)
        {
            string[] bioLineSTR = bioText.Split('\n').Take(10).ToArray();
            //Debug.LogError("Result:" + bioLineSTR);

            tempBioOnly10LineStr = "";
            for (int i = 0; i < bioLineSTR.Length; i++)
            {
                tempBioOnly10LineStr += bioLineSTR[i] + "\n";
            }
            textUserBio.text = tempBioOnly10LineStr;

            SeeMoreLessBioTextSetup(true);
            seeMoreBioButton.SetActive(true);
        }
        else
        {
            //false see more button
            seeMoreBioButton.SetActive(false);
        }
    }

    //this method is used to Bio SeeMore or Less button click.......
    public void OnClickBioSeeMoreLessButton()
    {
        if (seeMoreButtonTextObj.activeSelf)
        {
            textUserBio.text = APIManager.DecodedString(myProfileData.userProfile.bio);
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

    //this method is used to MyProfile APi Pagination.......
    //public float lastVerticalNormalizedPosition = -1;
    public void ProfileAPiPagination()
    {
        Debug.Log("ProfileAPiPagination 000");
        if (myProfileScreen.activeSelf)
        {
            Debug.Log("ProfileAPiPagination 11");
            VerticalPosition = profileMainScrollRectFasterEx.verticalNormalizedPosition;
            //Debug.LogError("Profile y pos:" + profileMainScrollRectFasterEx.verticalEndPos + "  :verticalnormalize pos:"+ profileMainScrollRectFasterEx.verticalNormalizedPosition + "  :normalize:"+profileMainScrollRectFasterEx.normalizedPosition + "   :isLoaded:"+ isFeedLoaded);
            //if (profileMainScrollRectFasterEx.verticalEndPos <= 1 && isFeedLoaded)
            //if (profileMainScrollRectFasterEx.verticalNormalizedPosition <= 0 && lastVerticalNormalizedPosition != profileMainScrollRectFasterEx.verticalNormalizedPosition && isFeedLoaded)
            if (profileMainScrollRectFasterEx.verticalNormalizedPosition < 0.01f && isFeedLoaded)
            {
                //Debug.LogError("scrollRect pos :" + profileMainScrollRectFasterEx.verticalNormalizedPosition + " rows count:" + allFeedWithUserIdRoot.Data.Rows.Count + "   :pageIndex:" + (profileFeedAPiCurrentPageIndex+1));
                //lastVerticalNormalizedPosition = profileMainScrollRectFasterEx.verticalNormalizedPosition;
                if (currentPageAllFeedWithUserIdRoot.Data.Rows.Count > 0)
                {
                    isFeedLoaded = false;
                    //Debug.LogError("isDataLoad False");
                    APIManager.Instance.RequestGetFeedsByUserId(APIManager.Instance.userId, (profileFeedAPiCurrentPageIndex + 1), 10, "MyProfile");
                }
               // OnScrollNFT();
            }
        }
    }

    public List<int> loadedMyPostAndVideoId = new List<int>();
    public List<int> loadedMyPostAndVideoIdInFeedPage = new List<int>();
    //this method is used to load All my feed and setup.......
    public void AllFeedWithUserId(int pageNumb, Transform Feedparent = null, bool IsNew = false)
    {
        currentPageAllFeedWithUserIdRoot = APIManager.Instance.allFeedWithUserIdRoot;
        bool IsMyProfileFeed = false;
        FeedUIController.Instance.ShowLoader(false);

        if (FeedUIController.Instance.allFeedMessageTextList[2].gameObject.activeSelf)
        {
            if (currentPageAllFeedWithUserIdRoot.Data.Rows.Count == 0)
            {
                //FeedUIController.Instance.AllFeedScreenMessageTextActive(true, 2, TextLocalization.GetLocaliseTextByKey("no discover feed available"));
                FeedUIController.Instance.AllFeedScreenMessageTextActive(true, 2, TextLocalization.GetLocaliseTextByKey("There's nothing to show here."));
            }
            else
            {
                FeedUIController.Instance.AllFeedScreenMessageTextActive(false, 2, TextLocalization.GetLocaliseTextByKey(""));
            }
        }

        for (int i = 0; i < currentPageAllFeedWithUserIdRoot.Data.Rows.Count; i++)
        {
            Debug.Log("currentPageAllFeedWithUserIdRoot");
            if ((!loadedMyPostAndVideoId.Contains(currentPageAllFeedWithUserIdRoot.Data.Rows[i].Id) && Feedparent == null)
               || (!loadedMyPostAndVideoIdInFeedPage.Contains(currentPageAllFeedWithUserIdRoot.Data.Rows[i].Id) && Feedparent != null))
            {
                bool isVideo = false;

                Transform parent = allPhotoContainer;
                if (Feedparent == null)
                {
                    parent = allPhotoContainer;
                    if (!string.IsNullOrEmpty(currentPageAllFeedWithUserIdRoot.Data.Rows[i].Image))
                    {
                        parent = allPhotoContainer;
                    }
                    else if (!string.IsNullOrEmpty(currentPageAllFeedWithUserIdRoot.Data.Rows[i].Video))
                    {
                        isVideo = true;
                        parent = allMovieContainer;
                    }
                    IsMyProfileFeed = true;
                }
                else
                {
                    if (!string.IsNullOrEmpty(currentPageAllFeedWithUserIdRoot.Data.Rows[i].Video))
                    {
                        isVideo = true;
                    }
                }
                GameObject userTagPostObject;
                if (Feedparent == null)
                {
                    userTagPostObject = Instantiate(photoPrefab, parent);
                    Debug.Log("userTagPostObject is Instantiate in parent ");
                }
                else
                {
                    userTagPostObject = Instantiate(photoPrefabInMyPostFeed, Feedparent);
                    Debug.Log("userTagPostObject is Instantiate in FeedParent");
                }
                if (APIManager.Instance.allFeedWithUserIdRoot.Data.Rows.Count == 0)
                {
                    FeedUIController.Instance.AllFeedScreenMessageTextActive(true, 2, TextLocalization.GetLocaliseTextByKey("There's nothing to show here."));
                }
                else
                {
                    FeedUIController.Instance.AllFeedScreenMessageTextActive(false, 2, TextLocalization.GetLocaliseTextByKey(""));
                }
                if (IsNew)
                {
                    userTagPostObject.transform.SetAsFirstSibling();
                }
                Debug.Log("userTagPostObject" + userTagPostObject.name);
                UserPostItem userPostItem = userTagPostObject.GetComponent<UserPostItem>();
                userPostItem.userData = currentPageAllFeedWithUserIdRoot.Data.Rows[i];
                if (!allMyFeedInFeedPageRootDataList.Contains(currentPageAllFeedWithUserIdRoot.Data.Rows[i]))
                {
                    if (IsNew)
                    {
                        allMyFeedInFeedPageRootDataList.Insert(0, currentPageAllFeedWithUserIdRoot.Data.Rows[i]);
                    }
                    else
                    {
                        allMyFeedInFeedPageRootDataList.Add(currentPageAllFeedWithUserIdRoot.Data.Rows[i]);
                    }
                }
                FeedsByFollowingUser feedUserData = new FeedsByFollowingUser();
                feedUserData.Id = myProfileData.id;
                feedUserData.Name = myProfileData.name;
                feedUserData.Email = myProfileData.email;
                feedUserData.Avatar = myProfileData.avatar;
                userPostItem.feedUserData = feedUserData;
                userPostItem.avtarUrl = myProfileData.avatar;
                userPostItem.LoadFeed();

                if (Feedparent == null)
                {
                    loadedMyPostAndVideoId.Add(currentPageAllFeedWithUserIdRoot.Data.Rows[i].Id);
                }
                else
                {
                    if (IsNew)
                    {
                        loadedMyPostAndVideoIdInFeedPage.Insert(0, currentPageAllFeedWithUserIdRoot.Data.Rows[i].Id);
                    }
                    else
                    {
                        loadedMyPostAndVideoIdInFeedPage.Add(currentPageAllFeedWithUserIdRoot.Data.Rows[i].Id);
                    }
                }
                if (pageNumb == 1 && i == 0)
                {
                    Debug.LogError("Latest Profile pic set as top");
                    userTagPostObject.transform.SetAsFirstSibling();
                    //if (allMyFeedImageRootDataList.Any(x => x.Id != currentPageAllFeedWithUserIdRoot.Data.Rows[i].Id))
                    //{
                    if (!isVideo)//image
                    {
                        allMyFeedImageRootDataList.Insert(0, currentPageAllFeedWithUserIdRoot.Data.Rows[i]);
                    }
                    else
                    {
                        allMyFeedVideoRootDataList.Insert(0, currentPageAllFeedWithUserIdRoot.Data.Rows[i]);
                    }
                    // }
                }
                else
                {
                    Debug.LogError("Latest Profile pic set as top   5555");


                    //if (allMyFeedImageRootDataList.Any(x => x.Id != currentPageAllFeedWithUserIdRoot.Data.Rows[i].Id))
                    //{


                    if (!isVideo)//image
                    {
                        allMyFeedImageRootDataList.Add(currentPageAllFeedWithUserIdRoot.Data.Rows[i]);
                    }
                    else
                    {
                        allMyFeedVideoRootDataList.Add(currentPageAllFeedWithUserIdRoot.Data.Rows[i]);
                    }


                    // }
                }

            }
        }

        Debug.LogError("Pagenmub bar");
        if (pageNumb == 1)
        {
            Debug.LogError("Pagenmub");
            Invoke(nameof(RefreshHieght), 1f);
        }
        StartCoroutine(WaitToFeedLoadedUpdate(pageNumb, IsMyProfileFeed));
        if (allMyFeedImageRootDataList.Count >= 2)
        {
            Debug.LogError(allMyFeedImageRootDataList[0].Id + "    " + allMyFeedImageRootDataList[1].Id);
            if (allMyFeedImageRootDataList[0].Id == allMyFeedImageRootDataList[1].Id)
            {
                allMyFeedImageRootDataList.RemoveAt(0);
                Debug.LogError("Remove Same ID Post");
                allMyFeedInFeedPageRootDataList.RemoveAt(0);
            }
        }
    }

    public void RefreshHieght()
    {
        OnClickPhotoTabButtonMain(0);
    }
    IEnumerator WaitToFeedLoadedUpdate(int pageNum, bool IsMyProfileFeed)
    {
        yield return new WaitForSeconds(0.1f);
        //if (IsMyProfileFeed)
        //{
        //    userPostPart.GetComponent<ParentHeightResetScript>().GetAndCheckMaxHeightInAllTab();
        //}

        SetupEmptyMsgForPhotoTab(false);//check for empty message.......

        yield return new WaitForSeconds(1f);
        isFeedLoaded = true;
        if (pageNum > 1 && currentPageAllFeedWithUserIdRoot.Data.Rows.Count > 0)
        {
            profileFeedAPiCurrentPageIndex += 1;
        }
        //Debug.LogError("my profile AllFeedWithUserId:" + isFeedLoaded);
    }

    //this mehtod is used to load All Tab feed.......
    public IEnumerator AllTagFeed()
    {
        foreach (Transform item in allTagContainer)
        {
            Destroy(item.gameObject);
        }
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < APIManager.Instance.taggedFeedsByUserIdRoot.data.rows.Count; i++)
        {
            GameObject userPostObject = Instantiate(photoPrefab, allTagContainer);
            //Debug.LogError("tagdata" + APIManager.Instance.taggedFeedsByUserIdRoot.data.rows[i]);
            UserPostItem userPostItem = userPostObject.GetComponent<UserPostItem>();
            userPostItem.tagUserData = APIManager.Instance.taggedFeedsByUserIdRoot.data.rows[i];

            FeedsByFollowingUser feedUserData = new FeedsByFollowingUser();
            feedUserData.Id = FeedRawData.id;
            feedUserData.Name = FeedRawData.name;
            feedUserData.Email = FeedRawData.email;
            feedUserData.Avatar = FeedRawData.avatar;
            userPostItem.feedUserData = feedUserData;

            userPostItem.avtarUrl = FeedRawData.avatar;
            userPostItem.LoadFeed();
        }
    }

    public void OnSetUserUi(bool isFollow)
    {
        if (isFollow)
        {
            followButtonImage.sprite = followingSprite;
            //followFollowingText.text = "Following";
            followFollowingText.text = TextLocalization.GetLocaliseTextByKey("Following");
            followFollowingText.color = FollowingTextColor;
        }
        else
        {
            followButtonImage.sprite = followSprite;
            //followFollowingText.text = "Follow";
            followFollowingText.text = TextLocalization.GetLocaliseTextByKey("Follow");
            followFollowingText.color = followTextColor;
        }
    }

    public void PrivatePublicTabSetup(bool isFollow)
    {
        if (isFollow)
        {
            tabPrivateObject.SetActive(false);
            tabPublicObject.SetActive(true);
        }
        else
        {
            tabPrivateObject.SetActive(true);
            tabPublicObject.SetActive(false);
        }
    }

    //this method is used to check and setup ui for Empty photo tab message.......
    public void SetupEmptyMsgForPhotoTab(bool isReset)
    {
        //check for photo.......
        if (allPhotoContainer.childCount > 0 || isReset)
        {
            allPhotoContainer.gameObject.SetActive(true);
            emptyPhotoPostMsgObj.SetActive(false);
        }
        else
        {
            allPhotoContainer.gameObject.SetActive(false);
            emptyPhotoPostMsgObj.SetActive(true);
        }

        //check for movie.......
        if (allMovieContainer.childCount > 0 || isReset)
        {
            allMovieContainer.gameObject.SetActive(true);
            emptyMoviePostMsgObj.SetActive(false);
        }
        else
        {
            allMovieContainer.gameObject.SetActive(false);
            emptyMoviePostMsgObj.SetActive(true);
        }

        //check for create first message.......
        if (allPhotoContainer.childCount > 0 || allMovieContainer.childCount > 0 || isReset)
        {
            createYourFirstPostMsgObj.SetActive(false);
            FooterCreateIcon.GetComponent<Animator>().enabled = false;
            FooterCreateIcon.transform.GetChild(0).transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else
        {
            //createYourFirstPostMsgObj.SetActive(true);
            FooterCreateIcon.GetComponent<Animator>().enabled = true;
        }
    }

    //this method is used to check if my nft list is available the auto hide create first feed popup.......
    public void CheckAndDisableFirstFeedPopupForMyNFT(bool isMyNFTScreen)
    {
        //check for create first message.......
        if (isMyNFTScreen)
        {
            if (NftDataScript.Instance.ContentPanel.transform.childCount > 0)
            {
                createYourFirstPostMsgObj.SetActive(false);
                FooterCreateIcon.GetComponent<Animator>().enabled = false;
                FooterCreateIcon.transform.GetChild(0).transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
        }
        else
        {
            if (allPhotoContainer.childCount > 0 || allMovieContainer.childCount > 0)
            {
                createYourFirstPostMsgObj.SetActive(false);
                FooterCreateIcon.GetComponent<Animator>().enabled = false;
                FooterCreateIcon.transform.GetChild(0).transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
            else
            {
                //createYourFirstPostMsgObj.SetActive(true);
                FooterCreateIcon.GetComponent<Animator>().enabled = true;
            }
        }
    }

    //this method is used to Stop or play bottom create plus icon.......
    public void CreateFirstFeedPlusAnimStop(bool isDisableAnim)
    {
        if (myProfileScreen.activeSelf)
        {
            if (allPhotoContainer.childCount > 0 || allMovieContainer.childCount > 0 || isDisableAnim)
            {
                FooterCreateIcon.GetComponent<Animator>().enabled = false;
                FooterCreateIcon.transform.GetChild(0).transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
            else
            {
                FooterCreateIcon.GetComponent<Animator>().enabled = true;
            }
        }
    }

    //this method is used to other player profile back button.......
    public void OnClickOtherPalyerProfileBackButton()
    {
        FeedUIController.Instance.feedUiScreen.SetActive(true);
        FeedUIController.Instance.otherPlayerProfileScreen.SetActive(false);
    }

    //this method is used to follow user button click.......
    public void OnClickFollowUserButton()
    {
        APIManager.Instance.RequestFollowAUser(FeedRawData.id.ToString(), "MyProfile");
    }

    //this method is used to Create post Button Click.......
    public void OnClickCreatePostButton()
    {
        FeedUIController.Instance.OnClickCreateFeedPickImageOrVideo();
    }

    //this method is used to website button click.......
    public void OnClickWebsiteButtonClick()
    {
        string websiteUrl = "";
        if (CheckUrlDropboxOrNot(websiteText.text))
        {
            websiteUrl = websiteText.text;
        }
        else
        {
            websiteUrl = String.Concat(defaultUrl + websiteText.text);//https://www.xana.net/
        }

        Uri uriResult;
        bool result = Uri.TryCreate(myProfileData.userProfile.website, UriKind.Absolute, out uriResult)
                        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        if (result)
        {
            Debug.Log("Given URL is valid");
            websiteUrl = myProfileData.userProfile.website;
        }
        else
        {
            Debug.Log("Given URL is Invalid");
            websiteUrl = defaultUrl + myProfileData.userProfile.website;
        }
        //Debug.LogError("WebsiteURL:" + websiteUrl);
        Application.OpenURL(websiteUrl);
    }
    #endregion

    #region Photo, Movie, NFT Tab Methods.......
    //this method is used to Photo Tab button click.......
    public void OnClickPhotoTabButtonMain(int index)
    {
        NFTShowingOnneBool = false;
        selectionItemScript1.OnSelectedClick(index);
        TabCommonChange(index);

        NftDataScript.Instance.nftloading.SetActive(false);
        NftDataScript.Instance.ResetNftData();
        NftDataScript.Instance.NftLoadingPenal.SetActive(false);
    }
    public void OnClickPhotoTabButtonSub(int index)
    {
        selectionItemScript2.OnSelectedClick(index);
        TabCommonChange(index);

        NftDataScript.Instance.nftloading.SetActive(false);
        NftDataScript.Instance.ResetNftData();
        NftDataScript.Instance.NftLoadingPenal.SetActive(false);
    }

    //this method is used to Movie Tab button click.......
    public void OnClickMovieTabButtonMain(int index)
    {

        NFTShowingOnneBool = false;
        parentHeightResetScript.OnHeightReset(index);
        selectionItemScript1.OnSelectedClick(index);
        TabCommonChange(index);

        NftDataScript.Instance.nftloading.SetActive(false);
        NftDataScript.Instance.ResetNftData();
        NftDataScript.Instance.NftLoadingPenal.SetActive(false);
    }
    public void OnClickMovieTabButtonSub(int index)
    {
        selectionItemScript2.OnSelectedClick(index);
        TabCommonChange(index);
        NftDataScript.Instance.nftloading.SetActive(false);
        NftDataScript.Instance.ResetNftData();
        NftDataScript.Instance.NftLoadingPenal.SetActive(false);
    }

    public Coroutine ScrollNFTCoroutine;
    public bool NFTDataLoaded;
    public void OnScrollNFT()
    {
       // StartCoroutine(IEOnScrollNFT());
    }

    public IEnumerator IEOnScrollNFT()
    {
        /*
        if(UserRegisterationManager.instance._web3APIforWeb2._OwnedNFTDataObj.NFTlistdata.list.Count<=0)
        {
            UserRegisterationManager.instance._web3APIforWeb2.GetWeb2UserData(PlayerPrefs.GetString("publicID"), () =>
            {
                //if (CurrentSection.Equals("OwnedAssets"))
                //{
                displayNFTinUIAsync();
                //}
            });
        }
        else
        {
            displayNFTinUIAsync();
        }
        */
        yield return new WaitForSeconds(2);
        
    }
    //this method is used to NFT Tab button click.......
    public void OnClickNFTTabButtonMain(int index)
    {
        //  print("NFTShowingOnneBool bool " + NFTShowingOnneBool);
        //if(NFTShowingOnneBool)
        //{
        //    return;
        //}       

        //if(!NFTShowingOnneBool)
        //{
        //    NFTShowingOnneBool = true;
        // }  

        if (!PremiumUsersDetails.Instance.CheckSpecificItem("mynftbutton"))
        {
            print("Please Upgrade to Premium account");
            return;
        }
        else
        {
            print("Horayyy you have Access");
        }

        NftDataScript.Instance.NftLoadingPenal.SetActive(true);    
           NftDataScript.Instance.currentSelection();
        parentHeightResetScript.OnHeightReset(index);
       selectionItemScript1.OnSelectedClick(index);
        //if (NftDataScript.Instance.ContentPanel.transform.childCount == 0)
        //{
        //    OnScrollNFT();
        //}
        if (NftDataScript.Instance.ContentPanel.transform.childCount == 0)
        {
            displayNFTinUIAsync();
        }
        StartCoroutine(WaitToNFTTabHeight(index));
    }  
      
    public void OnClickNFTTabButtonSub(int index)
    {
        if (!PremiumUsersDetails.Instance.CheckSpecificItem("mynftbutton"))
        {
            print("Please Upgrade to Premium account");
            return;
        }
        else
        {
            print("Horayyy you have Access");
        }
        //  NftDataScript.Instance.currentSelection();
        selectionItemScript2.OnSelectedClick(index);
        NftDataScript.Instance.NftLoadingPenal.SetActive(true);
        NftDataScript.Instance.NoNftyet.SetActive(false);
        NftDataScript.Instance.NoNftyet.GetComponent<TMPro.TextMeshProUGUI>().text = string.Empty;
        NftDataScript.Instance.nftloading.SetActive(true);
     //   if (NftDataScript.Instance.ContentPanel.transform.childCount == 0)
      //  {
         //   OnScrollNFT();
     //   }
        if (NftDataScript.Instance.ContentPanel.transform.childCount == 0)
        {
            displayNFTinUIAsync();
        }  
       // StartCoroutine(WaitToNFTTabHeight(index));
    }

    IEnumerator WaitToNFTTabHeight(int index)
    {
        yield return new WaitForSeconds(0.001f);
        TabCommonChange(index);
    }

    void displayNFTinUIAsync()
    {
        //GameObject[] objsNFTs = NftDataScript.Instance.ContentPanel.transform.childCount;
        print("come to async showing NFT");

        //if (UserRegisterationManager.instance.userRoleObj.NFTsURL.Count < NftDataScript.Instance.ContentPanel.transform.childCount)
        //{
        //  print("come to 2222");
        if (UserRegisterationManager.instance._web3APIforWeb2._OwnedNFTDataObj.NFTlistdata.list.Count > 0)
        {
        print("come to async showing NFT in if "+ UserRegisterationManager.instance._web3APIforWeb2._OwnedNFTDataObj.NFTlistdata.list.Count);
            NftDataScript.Instance.NftLoadingPenal.SetActive(false);
            NftDataScript.Instance.NoNftyet.SetActive(false);
            NftDataScript.Instance.NoNftyet.GetComponent<TMPro.TextMeshProUGUI>().text = string.Empty;
            NftDataScript.Instance.nftloading.SetActive(false);
            print("userRoleObj.NFTsURL: " + UserRegisterationManager.instance._web3APIforWeb2._OwnedNFTDataObj.NFTsURL.Count);
            print("userRoleObj.NFTsURLList: " + UserRegisterationManager.instance._web3APIforWeb2._OwnedNFTDataObj.NFTlistdata.list.Count);  
            for (int i = 0; i < UserRegisterationManager.instance._web3APIforWeb2._OwnedNFTDataObj.NFTlistdata.list.Count; i++)
            {  
                     GameObject L_ItemBtnObj = Instantiate(NFTImagePrefab, NftDataScript.Instance.ContentPanel.transform);
                    Debug.Log("L_ItemBtnObj");
                     if (UserRegisterationManager.instance._web3APIforWeb2._OwnedNFTDataObj.NFTstype[i] == 4)
                    {
                         L_ItemBtnObj.gameObject.GetComponent<NFTtypeClass>().VideoIcon.SetActive(true);
                    }
                    int locali = i;
                     L_ItemBtnObj.gameObject.GetComponent<NFTtypeClass>()._indexNumber = i;
                     L_ItemBtnObj.gameObject.GetComponent<NFTtypeClass>().isVisible = true;
                    L_ItemBtnObj.gameObject.name= "image_NFT "+UserRegisterationManager.instance._web3APIforWeb2._OwnedNFTDataObj.NFTlistdata.list[locali].nftId.ToString(); 
             }
        }  
        else
        {
            print("No NFT Found");
            Debug.Log("call hua else data");
            NftDataScript.Instance.NftLoadingPenal.SetActive(true);
            NftDataScript.Instance.NoNftyet.SetActive(true);
            NftDataScript.Instance.NoNftyet.GetComponent<TMPro.TextMeshProUGUI>().text = TextLocalization.GetLocaliseTextByKey("NFT data not found");
            NftDataScript.Instance.nftloading.SetActive(false);
        }  
        // UserRegisterationManager.instance._web3APIforWeb2.OwnedNFTPageNumb++;
      //  Invoke(nameof(RefreshNFTScrollHeight), 1f);
    }
     public void RefreshNFTScrollHeight()
    {
        parentHeightResetScript.OnHeightReset(2);
    }
    IEnumerator DownloadImage(RawImage _rawImage, string MediaUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
            _rawImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
    }

    public static async Task<Texture2D> GetRemoteTexture(string url)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            // begin request:
            var asyncOp = www.SendWebRequest();

            // await until it's done: 
            while (asyncOp.isDone == false)
                await Task.Delay(1000 / 30);//30 hertz
                                            // read results:
            if (www.isNetworkError || www.isHttpError)
            // if( www.result!=UnityWebRequest.Result.Success )// for Unity >= 2020.1
            {
                // log error:
#if DEBUG
                Debug.Log($"{www.error}, URL:{www.url}");
#endif

                // nothing to return on error:
                return null;
            }
            else
            {
                // return valid results:
                return DownloadHandlerTexture.GetContent(www);
            }
        }
    }

    void TabCommonChange(int index)
    {
        // Debug.Log("<color=blue> Btn Index: " + index + "</color>");
        tabScrollRectGiftScreen.LerpToPage(index);
        parentHeightResetScript.OnHeightReset(index);
        if (index == 2)
        {
            CheckAndDisableFirstFeedPopupForMyNFT(true);//for Create First Feed Popup Auto hide if nft list available.......
        }
        else
        {
            CheckAndDisableFirstFeedPopupForMyNFT(false);//for Create First Feed Popup Auto hide if nft list available.......
        }
        switch (index)
        {
            case 0:
                CurrentSection = "Photo";
                break;
            case 1:
                CurrentSection = "Movie";
                break;
            case 2:
                CurrentSection = "OwnedAssets";
                break;
            default:
                break;
        }
    }
    #endregion

    #region Edit Profile Methods.......
    //this method is used to edit profile button click
    public void OnClickEditProfileButton()
    {
        EditProfileDoneButtonSetUp(true);//setup edit profile done button.......
        editProfileScreen.SetActive(true);
        SetupEditProfileScreen();
    }

    void SetupEditProfileScreen()
    {
        editProfileImage.sprite = profileImage.sprite;
        //editProfileNameInputfield.text = playerNameText.text;
        editProfileNameAdvanceInputfield.Text = playerNameText.text;
        if (myProfileData.userProfile != null)
        {
            //editProfileJobInputfield.text = myProfileData.userProfile.job;
            //editProfileJobAdvanceInputfield.Text = myProfileData.userProfile.job;
            editProfileJobAdvanceInputfield.Text = APIManager.DecodedString(myProfileData.userProfile.job);
            //editProfileWebsiteInputfield.text = myProfileData.userProfile.website;
            editProfileWebsiteAdvanceInputfield.Text = myProfileData.userProfile.website;
            editProfileBioInputfield.text = APIManager.DecodedString(myProfileData.userProfile.bio);
            editProfileGenderInputfield.text = myProfileData.userProfile.gender;

            editProfileBioInputfield.transform.parent.GetComponent<InputFieldHightResetScript>().OnValueChangeAfterResetHeight();
        }
    }

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

    public int checkEditNameUpdated = 0;
    public int checkEditInfoUpdated = 0;
    string website = "";
    string job = "";
    string bio = "";
    string gender = "";
    string username = "";

    public void OnClickEditProfileDoneButton()
    {
        ProfilePostPartShow();

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
            job = APIManager.DecodedString(myProfileData.userProfile.job);
            website = myProfileData.userProfile.website;
            bio = APIManager.DecodedString(myProfileData.userProfile.bio);
            gender = myProfileData.userProfile.gender;
        }

        EditProfileDoneButtonSetUp(false);//setup edit profile done button.......

        if (!CheckForWebSite())
        {
            EditProfileInfoCheckAndAPICalling();
        }
    }

    void EditProfileInfoCheckAndAPICalling()
    {
        /*checkEditNameUpdated = 0;
        checkEditInfoUpdated = 0;

        string username = playerNameText.text;
        job = "";
        gender = "";
        website = "";
        bio = "";

        if (myProfileData.userProfile != null)
        {
            job = myProfileData.userProfile.job;
            website = myProfileData.userProfile.website;
            bio = myProfileData.userProfile.bio;
            gender = myProfileData.userProfile.gender;
        }*/

        //if (!string.IsNullOrEmpty(editProfileNameInputfield.text))
        if (!string.IsNullOrEmpty(editProfileNameAdvanceInputfield.Text))
        {
            //if (editProfileNameInputfield.text != playerNameText.text)
            if (editProfileNameAdvanceInputfield.Text != playerNameText.text)
            {
                //string tempStr = editProfileNameInputfield.text;
                string tempStr = editProfileNameAdvanceInputfield.Text;
                if (tempStr.StartsWith(" "))
                {
                    tempStr = tempStr.TrimStart(' ');
                }
                if (tempStr.EndsWith(" "))
                {
                    tempStr = tempStr.TrimEnd(' ');
                }
                Debug.LogError("temp Name Str:" + tempStr);
                username = tempStr;
                checkEditNameUpdated = 1;
            }
        }
        else
        {
            Debug.LogError("Please enter username");
            ShowEditProfileNameErrorMessage("The name field should not be empty");
            return;
        }

        //if (editProfileJobInputfield.text != job)
        if (editProfileJobAdvanceInputfield.Text != job)
        {
            //string tempStr = editProfileJobInputfield.text;
            string tempStr = editProfileJobAdvanceInputfield.RichText;
            if (tempStr.StartsWith(" "))
            {
                tempStr = tempStr.TrimStart(' ');
            }
            Debug.LogError("temp Job Str:" + tempStr);
            job = tempStr;
            checkEditInfoUpdated = 1;
        }
        else
        {
            //if (string.IsNullOrEmpty(editProfileJobInputfield.text))
            if (string.IsNullOrEmpty(editProfileJobAdvanceInputfield.Text))
            {
                job = "";
            }
        }

        //if (editProfileWebsiteInputfield.text != website)
        /*if (editProfileWebsiteAdvanceInputfield.Text != website)
        {
            //string tempStr = editProfileWebsiteInputfield.text;
            string tempStr = editProfileWebsiteAdvanceInputfield.Text;
            if (tempStr.StartsWith(" "))
            {
                tempStr = tempStr.TrimStart(' ');
            }
            Debug.LogError("temp Web Str:" + tempStr);
            website = tempStr;
            checkEditInfoUpdated = 1;

            if (!string.IsNullOrEmpty(tempStr))
            {
                //FeedUIController.Instance.ShowLoader(true);
                //editProfileDoneButton.interactable = false;

                string webUrl = tempStr;
                bool isUrl = false;
                if (!CheckUrlDropboxOrNot(webUrl))
                {
                    webUrl = String.Concat(defaultUrl + tempStr);
                }
                else
                {
                    isUrl = true;
                }

                Debug.LogError("WebUrl:" + webUrl + "  :isUrl:" + isUrl);

                if (!IsReachableUri(webUrl) || webUrl.Contains("@"))
                {
                    Debug.LogError("Please enter valid web");
                    //FeedUIController.Instance.ShowLoader(false);
                    //websiteErrorObj.GetComponent<Animator>().SetBool("playAnim", true);
                    if (webErrorCo != null)
                    {
                        StopCoroutine(webErrorCo);
                        websiteErrorObj.SetActive(false);
                    }
                    websiteErrorObj.SetActive(true);
                    webErrorCo = StartCoroutine(WaitUntilErrorAnimationFinished(websiteErrorObj.GetComponent<Animator>()));                    
                    return;
                }
                else
                {                    
                    editProfileDoneButton.interactable = true;
                    if (isUrl)
                    {
                        Uri myUri = new Uri(tempStr);
                        website = myUri.Host;
                        Debug.LogError("temp Web Str111:" + website);
                    }
                }
            }

            //website = editProfileWebsiteInputfield.text;
            //checkEditInfoUpdated = 1;
        }
        else
        {
            //if (string.IsNullOrEmpty(editProfileWebsiteInputfield.text))
            if (string.IsNullOrEmpty(editProfileWebsiteAdvanceInputfield.Text))
            {
                website = "";
            }
        }*/

        if (editProfileBioInputfield.text != bio)
        {
            string tempStr = editProfileBioInputfield.text;
            if (tempStr.StartsWith(" "))
            {
                tempStr = tempStr.TrimStart(' ');
            }
            Debug.LogError("temp Bio Str:" + tempStr);
            bio = tempStr;
            checkEditInfoUpdated = 1;
        }
        else
        {
            if (string.IsNullOrEmpty(editProfileBioInputfield.text))
            {
                bio = "";
            }
        }

        if (!string.IsNullOrEmpty(editProfileGenderInputfield.text))
        {
            if (editProfileGenderInputfield.text != gender)
            {
                gender = editProfileGenderInputfield.text;
                checkEditInfoUpdated = 1;
            }
        }

        //editProfileScreen.SetActive(false);//Flase edit profile screen
        if (checkEditNameUpdated == 1)
        {
            if (string.IsNullOrEmpty(username))
            {
                username = "";
            }
            isEditProfileNameAlreadyExists = false;
            APIManager.Instance.RequestSetName(username);
        }

        if (checkEditInfoUpdated == 1)
        {
            string countryName = System.Globalization.RegionInfo.CurrentRegion.EnglishName;
            Debug.LogError("User Ingo Name:" + username + "   :job:" + job + "    :website:" + website + "    :bio:" + bio + "  :Gender:" + gender + "  :Country:" + countryName);

            if (string.IsNullOrEmpty(job))
            {
                job = "";
            }
            if (string.IsNullOrEmpty(website))
            {
                website = "";
            }
            if (string.IsNullOrEmpty(bio))
            {
                bio = "";
            }
            if (string.IsNullOrEmpty(gender))
            {
                gender = "Male";
                Debug.LogError("Default Gender:" + gender);
            }
            if (string.IsNullOrEmpty(countryName))
            {
                countryName = "";
            }

            APIManager.Instance.RequestUpdateUserProfile(gender, APIManager.EncodedString(job), countryName, website, APIManager.EncodedString(bio));
        }

        if (string.IsNullOrEmpty(setImageAvatarTempPath))
        {
            if (checkEditNameUpdated == 1 || checkEditInfoUpdated == 1)
            {
                Debug.LogError("EditProfileInfoCheckAndAPICalling Get User Details API Call");
                StartCoroutine(WaitEditProfileGetUserDetails(false));
            }
            else
            {
                editProfileScreen.SetActive(false);
                EditProfileDoneButtonSetUp(true);//setup edit profile done button.......
            }
        }
        else
        {
            StartCoroutine(WaitEditProfileGetUserDetails(true));
        }
    }

    //this method is return value with url is valid or not 
    public bool IsReachableUri(string url)
    {
        HttpWebRequest request;
        try
        {
            request = (HttpWebRequest)WebRequest.Create(url);
        }
        catch (Exception e)
        {
            Debug.LogError("isreachableUri ecception:" + e);
            return false;
        }
        request.Timeout = 2500;
        request.Method = "HEAD"; // As per Lasse's comment
        try
        {
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                return response.StatusCode == HttpStatusCode.OK;
            }
        }
        catch (WebException)
        {
            return false;
        }
    }

    public bool IsValidMail(string emailaddress)
    {
        try
        {
            MailAddress m = new MailAddress(emailaddress);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    bool isUrl = false;
    public bool CheckForWebSite()
    {
        if (editProfileWebsiteAdvanceInputfield.Text != website)
        {
            //string tempStr = editProfileWebsiteInputfield.text;
            string tempStr = editProfileWebsiteAdvanceInputfield.Text;
            if (tempStr.StartsWith(" "))
            {
                tempStr = tempStr.TrimStart(' ');
            }
            Debug.LogError("temp Web Str:" + tempStr);
            website = tempStr;
            checkEditInfoUpdated = 1;

            if (!string.IsNullOrEmpty(website))
            {
                isUrl = false;
                string webUrl = website;
                if (!CheckUrlDropboxOrNot(webUrl))
                {
                    // webUrl = String.Concat(defaultUrl + tempStr);
                }
                else
                {
                    isUrl = true;
                }

                FeedUIController.Instance.ShowLoader(true);
                RequestForWebSiteValidation(webUrl);
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            //if (string.IsNullOrEmpty(editProfileWebsiteInputfield.text))
            if (string.IsNullOrEmpty(editProfileWebsiteAdvanceInputfield.Text))
            {
                website = "";
            }
            return false;
        }
    }

    Coroutine webValidCo;
    public void RequestForWebSiteValidation(string url)
    {
        if (webValidCo != null)
        {
            StopCoroutine(webValidCo);
        }
        webValidCo = StartCoroutine(IERequestForWebSiteValidation(url));
    }
    public IEnumerator IERequestForWebSiteValidation(string url)
    {
        WWWForm form = new WWWForm();
        form.AddField("url", url);
        Debug.LogError("Web URL:" + url);
        using (UnityWebRequest www = UnityWebRequest.Post((ConstantsGod.API_BASEURL + ConstantsGod.r_url_WebsiteValidation), form))
        {
            yield return www.SendWebRequest();

            FeedUIController.Instance.ShowLoader(false);

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                EditProfileErrorMessageShow(websiteErrorObj);
                Debug.LogError("Invalid WebSite");
            }
            else
            {
                string data = www.downloadHandler.text;
                Debug.Log("Website Validation success data:" + data);
                WebSiteValidRoot webSiteValidRoot = JsonConvert.DeserializeObject<WebSiteValidRoot>(data);
                if (webSiteValidRoot.success)
                {
                    if (isUrl)
                    {
                        Uri myUri = new Uri(website);
                        //website = myUri.Host;
                    }
                    Debug.LogError("final result Web Str:" + website);

                    EditProfileInfoCheckAndAPICalling();
                    Debug.Log("Valid WebSite:");
                }
                else
                {
                    EditProfileErrorMessageShow(websiteErrorObj);
                    Debug.LogError("Invalid WebSite");
                }
            }
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
        //websiteErrorObj.SetActive(true);
        currentOBJ.SetActive(true);
        currentEditProfileErrorMessgaeObj = currentOBJ;
        editProfileErrorCo = StartCoroutine(WaitUntilErrorAnimationFinished());
    }

    Coroutine editProfileErrorCo;
    GameObject currentEditProfileErrorMessgaeObj;
    //this coroutine is used to show and wait until finish error message animation.......
    IEnumerator WaitUntilErrorAnimationFinished()
    {
        yield return new WaitForSeconds(0.5f);
        EditProfileDoneButtonSetUp(true);//setup edit profile done button.......
        yield return new WaitForSeconds(1f);
        //MyAnim.SetBool("playAnim", false);
        //FeedUIController.Instance.ShowLoader(false);
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

    //this coroutine is used check and GetUserDetails or UploadAvatar api call.......
    IEnumerator WaitEditProfileGetUserDetails(bool isProfileUpdate)
    {
        FeedUIController.Instance.ShowLoader(true);
        yield return new WaitForSeconds(1f);
        if (!isProfileUpdate)
        {
            APIManager.Instance.RequestGetUserDetails("EditProfileAvatar");//Get My Profile data 
        }
        else
        {
            Debug.LogError("Uploading profile pic :" + setImageAvatarTempPath);
            AWSHandler.Instance.PostAvatarObject(setImageAvatarTempPath, setImageAvatarTempFilename, "EditProfileAvatar");//upload avatar image on AWS.
        }
    }

    //this method is used to Bio Button click.......
    public void OnEditBioButtonClick()
    {
        editProfileBioScreen.SetActive(true);
        //bioEditInputField.text = editProfileBioInputfield.text;

        bioEditAdvanceInputField.Text = editProfileBioInputfield.text;

        //bioEditInputField.transform.parent.GetComponent<InputFieldHightResetScript>().OnValueChangeAndResetNormalInputField();
        //bioEditInputField.transform.parent.GetComponent<InputFieldHightResetScript>().OnValueChangeAfterResetHeight();
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

        //string resultString = Regex.Replace(bioEditInputField.text.ToString(), @"^\s*$[\r\n]*", string.Empty, RegexOptions.Multiline);
        string resultString = Regex.Replace(bioEditAdvanceInputField.Text.ToString(), @"^\s*$[\r\n]*", string.Empty, RegexOptions.Multiline);

        editProfileBioInputfield.text = resultString;
        //editProfileBioInputfield.text = bioEditInputField.text;
        editProfileBioInputfield.transform.parent.GetComponent<InputFieldHightResetScript>().OnValueChangeAfterResetHeight();
    }

    //this method is used to change Profile Button click.......
    public void OnClickChangeProfilePicButton()
    {
        mainFullScreenContainer.SetActive(false);//fo disable profile screen post part.......
        pickImageOptionScreen.SetActive(true);
    }

    //this method is used to show profile post main part.......
    public void ProfilePostPartShow()
    {
        if (!mainFullScreenContainer.activeSelf)
            mainFullScreenContainer.SetActive(true);//fo disable profile screen post part.......
    }

    public string setImageAvatarTempPath = "";
    public string setImageAvatarTempFilename = "";

    //public bool setGroupFromCamera = false;
    //[Space]
    //public Texture2D setGroupTempAvatarTexture;

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

                Debug.LogError("OnPickGroupAvatarFromGellery path: " + path);

                //string[] pathArry = path.Split('/');

                //string fileName = pathArry[pathArry.Length - 1];
                string fileName = Path.GetFileName(path);
                Debug.LogError("OnPickGroupAvatarFromGellery FileName: " + fileName);

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

                Debug.LogError("OnPickGroupAvatarFromGellery path: " + path);

                //string[] pathArry = path.Split('/');

                //string fileName = pathArry[pathArry.Length - 1];
                string fileName = Path.GetFileName(path);
                Debug.LogError("OnPickGroupAvatarFromGellery FileName: " + fileName);

                string[] fileNameArray = fileName.Split('.');
                string str = DateTime.Now.Day + "_" + DateTime.Now.Month + "_" + DateTime.Now.Year + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + ".";
                fileName = fileNameArray[0] + str + fileNameArray[1];

                setImageAvatarTempPath = Path.Combine(Application.persistentDataPath, "XanaChat", fileName); ;
                setImageAvatarTempFilename = fileName;

                Crop(texture, setImageAvatarTempPath);

                //editProfileImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
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
        //OnPickProfileImageFromGellery(maxSize);
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

                Debug.LogError("OnGroupAvatarTakePicture Camera ImagePath : " + path);

                //string[] pathArry = path.Split('/');

                //string fileName = pathArry[pathArry.Length - 1];
                string fileName = Path.GetFileName(path);
                Debug.LogError("Camera filename : " + fileName);

                string[] fileNameArray = fileName.Split('.');
                string str = DateTime.Now.Day + "_" + DateTime.Now.Month + "_" + DateTime.Now.Year + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + ".";
                fileName = fileNameArray[0] + str + fileNameArray[1];

                string filePath = Path.Combine(Application.persistentDataPath, "XanaChat", fileName);

                Debug.LogError("Camera filePath:" + filePath + "    :filename:" + fileName + "   :texture width:" + texture.width + " :height:" + texture.height);

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

                Debug.LogError("OnGroupAvatarTakePicture Camera ImagePath : " + path);

                //string[] pathArry = path.Split('/');

                //string fileName = pathArry[pathArry.Length - 1];
                string fileName = Path.GetFileName(path);
                Debug.LogError("Camera filename : " + fileName);

                string[] fileNameArray = fileName.Split('.');
                string str = DateTime.Now.Day + "_" + DateTime.Now.Month + "_" + DateTime.Now.Year + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + ".";
                fileName = fileNameArray[0] + str + fileNameArray[1];

                string filePath = Path.Combine(Application.persistentDataPath, "XanaChat", fileName);

                Debug.LogError("Camera filePath:" + filePath + "    :filename:" + fileName + "   :texture width:" + texture.width + " :height:" + texture.height);

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

    public AspectRatioFitter fit;

    public void StartWebCam()
    {
        WebCamDevice[] device = WebCamTexture.devices;

        if (device.Length == 0)
        {
            Debug.LogError("No camera detected");
            return;
        }
        for (int i = 0; i < device.Length; i++)
        {
            if (!device[i].isFrontFacing)
            {
                webCamTexture = new WebCamTexture(device[i].name, Screen.width, Screen.height);
            }
        }
        if (webCamTexture == null)
        {
            Debug.LogError("Enable to find back camera");
            return;
        }

        webCamTexture.Play();
        webcamRawImage.texture = webCamTexture;

        float ratio = (float)webCamTexture.width / (float)webCamTexture.height;
        fit.aspectRatio = ratio;

        int orient = -webCamTexture.videoRotationAngle;
        Debug.LogError("Ratio:" + ratio + " :Angle:" + orient);
        webcamRawImage.transform.localEulerAngles = new Vector3(0, 0, orient);
    }

    public GameObject webcamScreen;
    private WebCamTexture webCamTexture;
    public RawImage webcamRawImage;
    //public Renderer  webcampan;
    private IEnumerator OpenWebCamPhotoCamera()
    {
        webcamScreen.SetActive(true);
        // Request camera permission
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
            yield break;
        // Start the WebCamTexture

        StartWebCam();
        //webCamTexture = new WebCamTexture();
        //webcampan.material.mainTexture = webCamTexture;
        //webcamRawImage.texture = webCamTexture;
        //webCamTexture.Play();        
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
        Debug.LogError("Camera filename : " + fileName);

        string str = DateTime.Now.Day + "_" + DateTime.Now.Month + "_" + DateTime.Now.Year + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + ".";
        fileName = fileName + str + ".png";

        string filePath = Path.Combine(Application.persistentDataPath, "XanaChat", fileName);

        Debug.LogError("Camera filePath:" + filePath + "    :filename:" + fileName);

        setImageAvatarTempPath = filePath;
        setImageAvatarTempFilename = fileName;

        webCamTexture.Stop();
        webcamScreen.SetActive(false);
        Crop(photo, setImageAvatarTempPath);
    }
    #endregion

    #region Get Feed By User Id
    /*public void RequestGetFeedsByUserId(int userId, int pageNum, int pageSize)
    {
        FeedUIController.Instance.ShowLoader(true);
        StartCoroutine(IERequestGetFeedsByUserId(userId, pageNum, pageSize));
    }
    public IEnumerator IERequestGetFeedsByUserId(int userId, int pageNum, int pageSize)
    {
        WWWForm form = new WWWForm();

        using (UnityWebRequest www = UnityWebRequest.Get((APIManager.Instance.mainURL + APIManager.Instance.url_GetFeedsByUserId + "/" + userId + "/" + pageNum + "/" + pageSize)))
        {
            www.SetRequestHeader("Authorization", APIManager.Instance.userAuthorizeToken);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string data = www.downloadHandler.text;
                Debug.LogError("my profile feed data success:" + data);
                allFeedWithUserIdRoot = JsonConvert.DeserializeObject<AllFeedByUserIdRoot>(data);
                StartCoroutine(AllFeedWithUserId());
                // Debug.LogError("data" + allFeedWithUserIdRoot.Success);
            }
        }
    }*/
    #endregion

    #region Get Image From AWS
    public void GetImageFromAWS(string key, Image mainImage)
    {
        //Debug.LogError("My Profile GetImageFromAWS key:" + key);
        //GetExtentionType(key);
        if (AssetCache.Instance.HasFile(key))
        {
            //Debug.LogError("Image Available on Disk");
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

    /*public static ExtentionType currentExtention;
    public static ExtentionType GetExtentionType(string path)
    {
        if (string.IsNullOrEmpty(path))
            return (ExtentionType)0;

        string extension = Path.GetExtension(path);
        if (string.IsNullOrEmpty(extension))
            return (ExtentionType)0;

        if (extension[0] == '.')
        {
            if (extension.Length == 1)
                return (ExtentionType)0;

            extension = extension.Substring(1);
        }

        extension = extension.ToLowerInvariant();
        Debug.LogError("ExtentionType: " + extension);
        if (extension == "png" || extension == "jpg" || extension == "jpeg" || extension == "gif" || extension == "bmp" || extension == "tiff" || extension == "heic")
        {
            currentExtention = ExtentionType.Image;
            return ExtentionType.Image;
        }
        else if (extension == "mp4" || extension == "mov" || extension == "wav" || extension == "avi")
        {
            currentExtention = ExtentionType.Video;
            // Debug.LogError("vvvvvvvvvvvvvvvvvvvvvvvvvvvv");
            return ExtentionType.Video;
        }
        else if (extension == "mp3" || extension == "aac" || extension == "flac")
        {
            currentExtention = ExtentionType.Audio;
            return ExtentionType.Audio;
        }
        return (ExtentionType)0;
    }*/

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
                    Debug.LogError("File SAVE");
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
            else
            {
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
        FeedUIController.Instance.ProfileFollowerFollowingScreenSetup(0, topHaderUserNameText.text);

        if (APIManager.Instance.profileAllFollowerRoot.data.rows.Count != myProfileData.followerCount)
        {
            FeedUIController.Instance.ProfileFollowerFollowingListClear();

            //FeedUIController.Instance.ShowLoader(true);
            FeedUIController.Instance.isProfileFollowerDataLoaded = false;
            APIManager.Instance.RequestGetAllFollowersFromProfile(myProfileData.id.ToString(), 1, 50);

            if (followingCo != null)
            {
                StopCoroutine(followingCo);
            }
            followingCo = StartCoroutine(WaitToCallFollowing());
        }
    }

    Coroutine followingCo;
    IEnumerator WaitToCallFollowing()
    {
        yield return new WaitForSeconds(1f);
        FeedUIController.Instance.isProfileFollowingDataLoaded = false;
        APIManager.Instance.RequestGetAllFollowingFromProfile(myProfileData.id.ToString(), 1, 50);
    }

    //this method is used to profile Following button click.......
    public void OnClickFollowingButtton()
    {
        FeedUIController.Instance.ProfileFollowerFollowingScreenSetup(1, topHaderUserNameText.text);

        if (APIManager.Instance.profileAllFollowingRoot.data.rows.Count != myProfileData.followingCount)
        {
            FeedUIController.Instance.ProfileFollowerFollowingListClear();

            //FeedUIController.Instance.ShowLoader(true);
            FeedUIController.Instance.isProfileFollowingDataLoaded = false;
            APIManager.Instance.RequestGetAllFollowingFromProfile(myProfileData.id.ToString(), 1, 50);

            if (followeCo != null)
            {
                StopCoroutine(followeCo);
            }
            followeCo = StartCoroutine(WaitToFollower());
        }
    }

    Coroutine followeCo;
    IEnumerator WaitToFollower()
    {
        yield return new WaitForSeconds(1f);
        FeedUIController.Instance.isProfileFollowerDataLoaded = false;
        APIManager.Instance.RequestGetAllFollowersFromProfile(myProfileData.id.ToString(), 1, 50);
    }
    #endregion

    #region my profile Data API
    public GetUserDetailRoot tempMyProfileDataRoot = new GetUserDetailRoot();
    public void RequestGetUserDetails()
    {
        if (totalFollowerText.gameObject.activeInHierarchy)
        {
            StartCoroutine(IERequestGetUserDetails());
        }
    }
    public IEnumerator IERequestGetUserDetails()
    {
        WWWForm form = new WWWForm();

        //   form.AddField("name", setName_name);

        using (UnityWebRequest www = UnityWebRequest.Get((ConstantsGod.API_BASEURL + ConstantsGod.r_url_GetUserDetails)))
        {
            www.SetRequestHeader("Authorization", APIManager.Instance.userAuthorizeToken);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError("IERequestGetUserDetails error:" + www.error);
            }
            else
            {
                string data = www.downloadHandler.text;
                Debug.LogError("IERequestGetUserDetails Loaded Completed data:" + data);
                tempMyProfileDataRoot = JsonUtility.FromJson<GetUserDetailRoot>(data);
                myProfileData = tempMyProfileDataRoot.data;
                OnlyLoadDataMyProfile();//set data                
            }
        }
    }

    public void OnlyLoadDataMyProfile()
    {
        totalFollowerText.text = myProfileData.followerCount.ToString();
        totalFollowingText.text = myProfileData.followingCount.ToString();
        totalPostText.text = myProfileData.feedCount.ToString();
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

    public string permissionCheck = "";
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