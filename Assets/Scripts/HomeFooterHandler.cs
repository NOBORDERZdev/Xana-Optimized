using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class HomeFooterHandler : MonoBehaviour
{
    public List<Image> allButtonIcon = new List<Image>();
    public List<Sprite> allButtonUnSelected = new List<Sprite>();
    public List<Sprite> allButtonSelected = new List<Sprite>();
    public List<TextMeshProUGUI> AllTitleText = new List<TextMeshProUGUI>();
    public Color sellectedColor = new Color();
    public Color unSellectedColor = new Color();
    public Color intractableFalseColor = new Color();
    public Color DisableButtonColor = new Color();
    public Color ActiveButtonColor = new Color();

    //public int gameManager.defaultSelection = 0;
    public bool WaitToLoadAvatarData = false;
    public CanvasGroup canvasGroup;
    public GameObject postingBtn;
    public Image PostButton;
    public GameObject chatMessageUnReadCountObj;
    public TextMeshProUGUI chatMessageUnReadCountText;
    AdditiveScenesLoader additiveScenesManager;
    GameManager gameManager;
    HomeScoketHandler socketController;

    public static event Action<BackButtonHandler.screenTabs> OnScreenTabStateChange;

    private bool notLoadedAgain = false;
    private void Awake()
    {
        gameManager = GameManager.Instance;
        socketController = HomeScoketHandler.instance;
        if (gameManager.defaultSelection == 3)
        {
            if (GlobalVeriableClass.callingScreen == "Profile")
            {
                gameManager.defaultSelection = 4;
            }
            else
            {
                GlobalVeriableClass.callingScreen = "Feed";
            }
        }


    }
    void Start()
    {
        if (gameManager.UiManager != null)
        {
            gameManager.defaultSelection = 0;
        }
        if (gameManager.UiManager != null && gameManager.defaultSelection == 0)
        {
            CheckLoginOrNotForFooterButton();
        }
        if (additiveScenesManager == null)
        {
            additiveScenesManager = gameManager.additiveScenesManager;
        }
        if (ConstantsHolder.xanaConstants.CurrentSceneName == "Addressable" && !ConstantsHolder.xanaConstants.isFromXanaLobby)
        {
            ConstantsHolder.xanaConstants.CurrentSceneName = "";
            GameManager.Instance.defaultSelection = 10;
            if (ConstantsHolder.xanaConstants.isFromHomeTab && !ConstantsHolder.xanaConstants.OpenSpaceScreen_fromSummit)
            {
                Invoke(nameof(OnClickHomeButton), 0);
                ConstantsHolder.xanaConstants.isFromHomeTab = false;

            }
            else
            {
                MainSceneEventHandler.OnBackRefAssign?.Invoke();
                notLoadedAgain = true;
                if (PlayerPrefs.GetInt("PlayerDeepLinkOpened") == 1)
                {
                    Debug.LogError("going here");
                    PlayerPrefs.SetInt("PlayerDeepLinkOpened", 0);
                }
                else if(!ConstantsHolder.xanaConstants.OpenSpaceScreen_fromSummit)
                {
                    Invoke(nameof(OnClickHomeWorldButton), 0f);
                }
                   
            }
        }
        else
        {
            gameManager.UiManager._footerCan.transform.GetChild(0).GetComponent<HomeFooterHandler>().OnSelectedClick(0);
        }

        if (PlayerPrefs.GetInt("IsLoggedIn") == 0)
        {
            Image buttonImage = allButtonIcon[2].transform.GetComponent<Image>();
            buttonImage.color = new Color(0.8f, 0.8f, 0.8f, 1f);
            allButtonIcon[3].transform.GetComponent<Image>().color = buttonImage.color;
        }
     }


    private void OnEnable()
    {
        MainSceneEventHandler.OnSucessFullLogin += CheckLoginOrNotForFooterButton;
    }

    private void OnDisable()
    {
        MainSceneEventHandler.OnSucessFullLogin -= CheckLoginOrNotForFooterButton;
    }

    public void OnSelectedClick(int index)
    {
        //if (PlayerPrefs.GetInt("IsLoggedIn") == 0)
        //{

        //    // allButtonIcon[2].transform.parent.GetComponent<Button>().interactable = false;
        //    allButtonIcon[4].transform.parent.GetComponent<Button>().interactable = false;
        //    //PostButton.transform.GetComponent<Button>().interactable = false;
        //    //  allButtonIcon[4].transform.GetChild(0).GetComponent<Image>().color = Color.gray;
        //}


        for (int i = 0; i < allButtonIcon.Count; i++)
        {
            //if (i == 2 || i == 3)
            //{
            //    break;
            //}
            if (i == index)
            {
                allButtonIcon[i].sprite = allButtonSelected[i];
                AllTitleText[i].color = ActiveButtonColor;
                gameManager.defaultSelection = index;
                //if (i == 2)
                //{
                //    allButtonIcon[i].transform.GetChild(0).GetComponent<Image>().color = ActiveButtonColor;
                //}
            }
            else
            {
                allButtonIcon[i].transform.GetComponent<Image>().color = unSellectedColor;
                AllTitleText[i].color = unSellectedColor;
                allButtonIcon[i].sprite = allButtonUnSelected[i];
                //if (i == 2)
                //{
                //    allButtonIcon[i].transform.GetChild(0).GetComponent<Image>().color = Color.black;
                //}
            }
        }
        //PostButton.transform.GetComponent<Button>().interactable = true;

    }
    public void CheckLoginOrNotForFooterButton()
    {
        if (PlayerPrefs.GetInt("IsLoggedIn") == 0)
        {
            allButtonIcon[2].transform.parent.GetComponent<Button>().interactable = false;
            allButtonIcon[2].transform.GetComponent<Image>().color = DisableButtonColor;
            allButtonIcon[3].transform.parent.GetComponent<Button>().interactable = false;
            allButtonIcon[3].transform.GetComponent<Image>().color = DisableButtonColor;
            if (postingBtn != null)
            {
                postingBtn.transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.295f);
            }
        }
        else
        {
            if (postingBtn != null)
            {
                postingBtn.transform.GetComponent<Button>().interactable = true;
                postingBtn.transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);

            }
            allButtonIcon[2].transform.parent.GetComponent<Button>().interactable = true;
            allButtonIcon[2].transform.GetComponent<Image>().color = unSellectedColor;
            allButtonIcon[3].transform.parent.GetComponent<Button>().interactable = true;
            allButtonIcon[3].transform.GetComponent<Image>().color = unSellectedColor;
            allButtonIcon[4].transform.parent.GetComponent<Button>().interactable = true;
            allButtonIcon[4].transform.GetComponent<Image>().color = unSellectedColor;
        }
        if (CommonAPIManager.Instance != null && PlayerPrefs.GetInt("IsLoggedIn") != 0)//For Get All Chat UnRead Message Count.......
        {
            CommonAPIManager.Instance.RequestGetAllChatUnReadMessagesCount();
        }
    }

    public void HomeSceneFooterSNSButtonIntrectableTrueFalse()
    {
        for (int i = 2; i < allButtonIcon.Count; i++)
        {
            if (PlayerPrefs.GetInt("IsLoggedIn") == 0)
            {
                allButtonIcon[i].color = new Color(intractableFalseColor.r, intractableFalseColor.g, intractableFalseColor.b, 0.5f);
                if (i == 2)
                {
                    allButtonIcon[i].transform.GetChild(0).GetComponent<Image>().color = DisableButtonColor /*Color.gray*/;
                }
                AllTitleText[i].color = unSellectedColor;

                allButtonIcon[i].transform.parent.GetComponent<Button>().interactable = false;
            }
            else
            {
                ////if (i == 2 || i == 3){
                ////    break;
                ////}
                //allButtonIcon[i].color = unSellectedColor;
                ////if (i == 2)
                ////{
                ////    allButtonIcon[i].transform.GetChild(0).GetComponent<Image>().color = Color.black;
                ////}
                //if (AllTitleText.Count> i && AllTitleText[i] != null) 
                //{
                //    AllTitleText[i].color = ActiveButtonColor;
                //}
                //allButtonIcon[i].transform.parent.GetComponent<Button>().interactable = true;

            }
        }

        if (CommonAPIManager.Instance != null && PlayerPrefs.GetInt("IsLoggedIn") != 0)//For Get All Chat UnRead Message Count.......
        {
            CommonAPIManager.Instance.RequestGetAllChatUnReadMessagesCount();
        }
    }

    Coroutine waitToLoadAvatarDataCo;
    IEnumerator waitToAvatarDataLoad()
    {
        yield return new WaitForSeconds(2f);
        WaitToLoadAvatarData = true;
    }
    public void OnClickHomeButton()
    {
        if (!(GlobalVeriableClass.callingScreen == "Home"))
        {
            gameManager.UiManager._footerCan.transform.GetChild(0).GetComponent<HomeFooterHandler>().OnSelectedClick(0);
            if (FeedUIController.Instance)
            {
                FeedUIController.Instance.bottomTabManager.OnSelectedClick(0);
            }
            GlobalVeriableClass.callingScreen = "Home";
            if (/*gameManager.defaultSelection != 0*/ true)
            {
                gameManager.FriendsHomeManager.GetComponent<FriendHomeManager>().EnableFriendsView(true);
                gameManager.defaultSelection = 0;
                if (additiveScenesManager != null && additiveScenesManager.SNSmodule)
                {
                    additiveScenesManager.SNSmodule.SetActive(false);
                    //additiveScenesManager.SNSMessage.SetActive(false);
                }
                //  gameManager.mainCharacter.GetComponent<AvatarControllerHome>().UpdateState(false);
                gameManager.ActorManager._cinemaCam.SetActive(false);
                if (gameManager.UiManager != null)
                {
                    CheckLoginOrNotForFooterButton();
                    gameManager.UiManager.HomeWorldScreen.SetActive(false);
                    gameManager.UiManager.HomePage.SetActive(true);
                    gameManager.UiManager._footerCan.GetComponent<CanvasGroup>().alpha = 1; // hiding home footer
                    gameManager.UiManager._footerCan.GetComponent<CanvasGroup>().interactable = true;
                    gameManager.UiManager._footerCan.GetComponent<CanvasGroup>().blocksRaycasts = true;
                    gameManager.UiManager.Canvas.SetActive(true);

                    gameManager.UiManager._footerCan.GetComponent<CanvasGroup>().alpha = 1;
                    gameManager.UiManager._footerCan.GetComponent<CanvasGroup>().interactable = true;
                    gameManager.UiManager._footerCan.GetComponent<CanvasGroup>().blocksRaycasts = true;

                    if (FeedUIController.Instance)
                    {
                        FeedUIController.Instance.footerCan.GetComponent<CanvasGroup>().alpha = 0;
                        FeedUIController.Instance.footerCan.GetComponent<CanvasGroup>().interactable = false;
                        FeedUIController.Instance.footerCan.GetComponent<CanvasGroup>().blocksRaycasts = false;
                    }
                }
            }
            gameManager.ActorManager.IdlePlayerAvatorForPostMenu(false);
            gameManager.HomeCamera.GetComponent<HomeCameraController>().CenterAlignCam();
            ConstantsHolder.xanaConstants.IsProfileVisit = false;
            DisableSubScreen();
        }
        gameManager.HomeCameraInputHandler(true);
        //GlobalVeriableClass.callingScreen = "";
        OnScreenTabStateChange?.Invoke(BackButtonHandler.screenTabs.Hometab);
       QuestDataHandler.Instance.OpenAndCloseQuestPanel(false);
    }
    public void OnClickHomeButtonIdleAvatar()
    {
        gameManager.ActorManager.IdlePlayerAvatorForMenu(false);
        OnClickHomeButton();
    }
    public void OnClickHomeWorldButton()
    {
        if (ConstantsHolder.IsXSummitApp)
        {
            if (ConstantsHolder.xanaConstants.isFromTottoriWorld)
                return;
            MainSceneEventHandler.OpenLandingScene?.Invoke();
            return;
        }

        gameManager.HomeCameraInputHandler(false);

        GlobalVeriableClass.callingScreen = "";
        Debug.Log("Home button onclick");

        if (gameManager.defaultSelection != 1)
        {
            //socketController.DisscountSNSSockets();

            gameManager.ActorManager._cinemaCam.SetActive(false);
            gameManager.defaultSelection = 1;
            //  gameManager.mainCharacter.GetComponent<AvatarControllerHome>().UpdateState(true);
            gameManager.UiManager._footerCan.transform.GetChild(0).GetComponent<HomeFooterHandler>().OnSelectedClick(1);
            if (FeedUIController.Instance)
            {
                FeedUIController.Instance.bottomTabManager.OnSelectedClick(1);
            }
            if (additiveScenesManager != null && additiveScenesManager.SNSmodule)
            {
                additiveScenesManager.SNSmodule.SetActive(false);
                // additiveScenesManager.SNSMessage.SetActive(false);
            }
            ////---->>>Sannan   if (gameManager.UiManager != null)
            //   {
            //     gameManager.UiManager._footerCan.transform.GetChild(0).GetComponent<HomeFooterHandler>().defaultSelection = 0;
            //     gameManager.UiManager._footerCan.transform.GetChild(0).GetComponent<HomeFooterHandler>().OnSelectedClick(0);
            // }
            if (gameManager.UiManager != null)
            {
                CheckLoginOrNotForFooterButton();
                gameManager.UiManager._footerCan.GetComponent<CanvasGroup>().alpha = 1; // hiding home footer
                gameManager.UiManager._footerCan.GetComponent<CanvasGroup>().interactable = true;
                gameManager.UiManager._footerCan.GetComponent<CanvasGroup>().blocksRaycasts = true;
                gameManager.UiManager.Canvas.SetActive(true);
                gameManager.UiManager.HomeWorldScreen.SetActive(true);
                gameManager.UiManager.HomePage.SetActive(false);
                gameManager.UiManager.SwitchToScreen(0);
                if (FeedUIController.Instance)
                {
                    FeedUIController.Instance.footerCan.GetComponent<CanvasGroup>().alpha = 0;
                    FeedUIController.Instance.footerCan.GetComponent<CanvasGroup>().interactable = false;
                    FeedUIController.Instance.footerCan.GetComponent<CanvasGroup>().blocksRaycasts = false;
                }

            }
            ConstantsHolder.xanaConstants.IsProfileVisit = false;
            if (notLoadedAgain == false || ConstantsHolder.xanaConstants.hasWorldTransitionedInternally)
            {
                WorldManager.LoadHomeScreenWorlds?.Invoke();
                ConstantsHolder.xanaConstants.hasWorldTransitionedInternally = false;
            }
            //FlexibleRect.OnAdjustSize?.Invoke(false);
            DisableSubScreen();
            //WorldManager.instance.ChangeWorld(APIURL.Hot);
            //WorldManager.instance.AllWorldTabReference.ScrollEnableDisable(0);
        }

        if (SearchWorldUIController.IsSearchBarActive)
        {
            SearchWorldUIController.IsSearchBarActive = false;
            WorldManager.instance.worldSearchManager.ClearInputField();
        }

        OnScreenTabStateChange?.Invoke(BackButtonHandler.screenTabs.Othertabs);
        QuestDataHandler.Instance.OpenAndCloseQuestPanel(false);
    }

    /*public void OnClickNewWorldButton()
    {
        //if (!gameManager.UiManager.WorldPage.activeSelf)
        {
            Debug.Log("World button onclick");
            if (gameManager.defaultSelection != 1)
            {
               // gameManager.mainCharacter.GetComponent<AvatarControllerHome>().UpdateState(true);
                OnSelectedClick(1);
                if (FindObjectOfType<AdditiveScenesLoader>() != null)
                {
                    FindObjectOfType<AdditiveScenesLoader>().SNSmodule.SetActive(false);
                    FindObjectOfType<AdditiveScenesLoader>().SNSMessage.SetActive(false);
                }
                if (gameManager.UiManager != null)
                {
                    gameManager.defaultSelection = 1;
                    gameManager.UiManager._footerCan.transform.GetChild(0).GetComponent<HomeFooterHandler>().OnSelectedClick(1);
                }
               // gameManager.UiManager.Canvas.SetActive(true);
                gameManager.UiManager.SwitchToScreen(1);
                WorldManager.instance.ChangeWorld(APIURL.Hot);
                WorldManager.instance.AllWorldTabReference.ScrollEnableDisable(0);
            }
        }
    }*/

    public void SetProfileButton()
    {
        allButtonIcon[4].transform.parent.GetComponent<Button>().interactable = true;
        allButtonIcon[4].transform.GetComponent<Image>().color = unSellectedColor;
    }
    public void OnClickAvatarButton()
    {
        if (gameManager.defaultSelection != 0)
        {
            // gameManager.mainCharacter.GetComponent<AvatarControllerHome>().UpdateState(true);
            gameManager.UiManager._footerCan.transform.GetChild(0).GetComponent<HomeFooterHandler>().OnSelectedClick(1);
            if (FeedUIController.Instance)
            {
                FeedUIController.Instance.bottomTabManager.OnSelectedClick(1);
            }
            if (additiveScenesManager != null)
            {
                additiveScenesManager.SNSmodule.SetActive(false);
                // additiveScenesManager.SNSMessage.SetActive(false);
            }
            if (gameManager.UiManager != null)
            {
                gameManager.defaultSelection = 0;
                gameManager.UiManager._footerCan.transform.GetChild(0).GetComponent<HomeFooterHandler>().OnSelectedClick(0);
                if (FeedUIController.Instance)
                {
                    FeedUIController.Instance.bottomTabManager.OnSelectedClick(0);
                }
            }

            // gameManager.UiManager.Canvas.SetActive(true);
        }
        gameManager.BottomAvatarBtnPressed();
    }
    //this method is used to Explore button click.......
    public void OnClickWorldButton()
    {
        if (!UserPassManager.Instance.CheckSpecificItem("WorldButton"))
        {
            print("Please Upgrade to Premium account");
            return;
        }
        else
        {
            print("Horayyy you have Access");
        }


        GlobalVeriableClass.callingScreen = "";

        if (gameManager.defaultSelection != 1)
        {
            gameManager.ActorManager._cinemaCam.SetActive(false);
            // gameManager.mainCharacter.GetComponent<AvatarControllerHome>().UpdateState(true);
            gameManager.UiManager._footerCan.transform.GetChild(0).GetComponent<HomeFooterHandler>().OnSelectedClick(1);
            if (FeedUIController.Instance)
            {
                FeedUIController.Instance.bottomTabManager.OnSelectedClick(1);
            }
            if (additiveScenesManager != null)
            {
                //if (SNS_MessageController.Instance != null)
                //{
                //    SNS_MessageController.Instance.isChatDetailsScreenDeactive = true;
                //}
                // additiveScenesManager.SNSMessage.SetActive(true);
                additiveScenesManager.SNSmodule.SetActive(false);
                gameManager.defaultSelection = 1;
                //SNS_MessageController.Instance.footerCan.GetComponent<HomeFooterHandler>().OnSelectedClick(1);
            }
            else
            {
                Initiate.Fade("SNSMessageModuleScene", Color.black, 1.0f, true);
            }

            if (gameManager.UiManager.Canvas.activeSelf)
            {
                gameManager.UiManager._footerCan.GetComponent<CanvasGroup>().alpha = 1; // hiding home footer
                gameManager.UiManager._footerCan.GetComponent<CanvasGroup>().interactable = true;
                gameManager.UiManager._footerCan.GetComponent<CanvasGroup>().blocksRaycasts = true;
                gameManager.UiManager.Canvas.SetActive(true);
                // gameManager.UiManager.Canvas.SetActive(false);
                Invoke("ClearUnloadAssetData", 0.2f);
            }
        }
    }
    /*public void OnclickEventButton()
    {
        Debug.Log("OnclickEventButton");
        UserPassManager.Instance.OpenComingSoonPopUp();
    }*/

    //this method is used to create button click.......
    public void OnClickCreateButton()
    {
        Debug.Log("Create button onclick");

        if (gameManager.defaultSelection != 5)
        {
            gameManager.ActorManager._cinemaCam.SetActive(false);
            //OnSelectedClick(5);
            gameManager.defaultSelection = 5;
            gameManager.ActorManager.IdlePlayerAvatorForMenu(true);
            if (ConstantsHolder.xanaConstants.r_MainSceneAvatar != null)
            {
                Destroy(ConstantsHolder.xanaConstants.r_MainSceneAvatar);
                ConstantsHolder.xanaConstants.r_MainSceneAvatar = null;
            }
            GameObject MainSceneAvatar = Instantiate(gameManager.mainCharacter);
            Transform rootRotationObj = MainSceneAvatar.transform.Find("mixamorig:Hips");
            if (rootRotationObj != null)
            {
                Transform hadeObj = rootRotationObj.GetChild(2).GetChild(0).GetChild(0).GetChild(1).transform;
                hadeObj.localRotation = Quaternion.Euler(Vector3.zero);
                hadeObj.transform.GetChild(0).transform.localRotation = Quaternion.Euler(Vector3.zero);
            }
            DontDestroyOnLoad(MainSceneAvatar);
            MainSceneAvatar.SetActive(false);
            //MainSceneAvatar.transform.parent.transform.eulerAngles= new Vector3(0,180,0);
            Initiate.Fade("ARModuleRoomScene", Color.black, 1.0f, true);
            ConstantsHolder.xanaConstants.r_MainSceneAvatar = MainSceneAvatar;

        }
    }

    //this method is used to feed button click.......
    public void OnClickFeedButton()
    {
        //if (!UserPassManager.Instance.CheckSpecificItem("sns_feed"))
        //{
        //    print("Please Upgrade to Premium account");
        //    return;
        //}
        //else
        //{
        //    print("Horayyy you have Access");
        //}
        gameManager.HomeCameraInputHandler(false);

        if (gameManager.defaultSelection != 2)
        {
            //socketController.DisscountSNSSockets();
            gameManager.ActorManager._cinemaCam.SetActive(false);
            // gameManager.mainCharacter.GetComponent<AvatarControllerHome>().UpdateState(true);
            // LoaderShow(true);
            gameManager.defaultSelection = 2;
            // gameManager.ActorManager.IdlePlayerAvatorForMenu(true);
            GlobalVeriableClass.callingScreen = "Feed";
            // gameManager.m_MainCamera.gameObject.SetActive(true);
            if (additiveScenesManager != null)
            {
                additiveScenesManager.SNSmodule.SetActive(true);
                //additiveScenesManager.SNSMessage.SetActive(false);
                gameManager.defaultSelection = 2;
                FeedUIController.Instance.feedUiScreen.SetActive(true);
                gameManager.UiManager._footerCan.transform.GetChild(0).GetComponent<HomeFooterHandler>().OnSelectedClick(2);
                FeedUIController.Instance.footerCan.GetComponent<HomeFooterHandler>().OnSelectedClick(2);
                FeedUIController.Instance.footerCan.GetComponent<HomeFooterHandler>().HomeSceneFooterSNSButtonIntrectableTrueFalse();
                gameManager.UiManager.HomeWorldScreen.SetActive(false);
                gameManager.UiManager._footerCan.GetComponent<CanvasGroup>().alpha = 0;
                gameManager.UiManager._footerCan.GetComponent<CanvasGroup>().interactable = false;
                gameManager.UiManager._footerCan.GetComponent<CanvasGroup>().blocksRaycasts = false;

                FeedUIController.Instance.footerCan.GetComponent<CanvasGroup>().alpha = 1;
                FeedUIController.Instance.footerCan.GetComponent<CanvasGroup>().interactable = true;
                FeedUIController.Instance.footerCan.GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
            else
            {
                if (SceneManager.GetActiveScene().name != "SNSFeedModuleScene")
                {
                    Initiate.Fade("SNSFeedModuleScene", Color.black, 1.0f, true);
                }

            }
            if (MyProfileDataManager.Instance.myProfileScreen.activeSelf)
            {
                //FeedUIController.Instance.FadeInOutScreenShow();//show fade in out.......
                FeedUIController.Instance.ResetAllFeedScreen(true);
                MyProfileDataManager.Instance.MyProfileSceenShow(false);//false my profile screen
            }
            //else
            //{
            //    SNS_APIManager.Instance.RequestGetUserDetails("myProfile");
            //}
            ConstantsHolder.xanaConstants.IsProfileVisit = false;
            if (FeedUIController.Instance != null)
            {
                FeedUIController.Instance.SetAddFriendScreen(false);
                FeedUIController.Instance.feedUiScreen.SetActive(true);
                FeedUIController.Instance.profileFollowerFollowingListScreen.SetActive(false);
                // OLD FEED UI
                ////if (FeedUIController.Instance.feedUiScreen.activeSelf)
                ////{
                ////    FeedUIController.Instance.SetUpFeedTabDefaultTop();//set default scroll top.......
                ////}
                // End Old Feed UI
            }
            gameManager.UiManager.HomeWorldScreen.SetActive(false);
            if (gameManager.UiManager.Canvas.activeSelf)
            {
                // gameManager.UiManager.Canvas.SetActive(false);
                Invoke("ClearUnloadAssetData", 0.2f);
            }


            //home page thumnbail images destroy
            WorldManager.instance.ClearHomePageData();
            DisableSubScreen();
        }
        if (MyProfileDataManager.Instance)
        {
            MyProfileDataManager.Instance.UpdateBackButtonAction(OnClickFeedButton);
        }
        OnScreenTabStateChange?.Invoke(BackButtonHandler.screenTabs.Othertabs);
        QuestDataHandler.Instance.OpenAndCloseQuestPanel(false);
    }

    public void OnClickAddFriends()
    {
        //if (!UserPassManager.Instance.CheckSpecificItem("AdFriends"))
        //{
        //    print("Please Upgrade to Premium account");
        //    return;
        //}
        //else
        //{
        //    print("Horayyy you have Access");
        //}
        // gameManager.ActorManager.IdlePlayerAvatorForMenu(true);
        gameManager.HomeCameraInputHandler(false);

        if (!ConstantsHolder.loggedIn) // Show login page for not sign in
        {
            //show popup here to login for adding friends
            //UserRegisterationManager.instance.OpenUIPanal(17);
            return;
        }
        gameManager.ActorManager._cinemaCam.SetActive(false);
        if (gameManager.defaultSelection != 3)
        {

            if (additiveScenesManager != null)
            {
                additiveScenesManager.SNSmodule.SetActive(true);
                gameManager.defaultSelection = 3;
                gameManager.defaultSelection = 3;
                gameManager.UiManager._footerCan.transform.GetChild(0).GetComponent<HomeFooterHandler>().OnSelectedClick(3);
                FeedUIController.Instance.footerCan.GetComponent<HomeFooterHandler>().OnSelectedClick(3);
                GlobalVeriableClass.callingScreen = "Feed";
                // additiveScenesManager.SNSMessage.SetActive(false);
            }
            else
            {
                if (SceneManager.GetActiveScene().name != "SNSFeedModuleScene")
                {
                    Initiate.Fade("SNSFeedModuleScene", Color.black, 1.0f, true);
                }
            }
            //below camera line was Commented before but i uncommented it in order to make profile 2.0 work ------- UMER
            gameManager.m_MainCamera.gameObject.SetActive(true);
            FeedUIController.Instance.SetAddFriendScreen(true);
            SNS_APIManager.Instance.SetHotFriend();
            FeedUIController.Instance.findFriendInputFieldAdvanced.Text = "";
            FeedUIController.Instance.findFriendScreen.gameObject.SetActive(false);
            //Commented in order to make profile 2.0 work after ahsan removed old feedui object from scene ----- UMER
            FeedUIController.Instance.OnClickHotFrnd();
            FeedUIController.Instance.ResetAllFeedScreen(true);
            FeedUIController.Instance.footerCan.GetComponent<HomeFooterHandler>().HomeSceneFooterSNSButtonIntrectableTrueFalse();


            //Invoke(nameof(InvokeDisableFeed),1f);
            //if (MyProfileDataManager.Instance.myProfileScreen.activeSelf)
            //{
            //    //FeedUIController.Instance.FadeInOutScreenShow();//show fade in out.......
            //    FeedUIController.Instance.ResetAllFeedScreen(true);
            //    MyProfileDataManager.Instance.MyProfileSceenShow(false);//false my profile screen
            //}
            //else
            //{
            //    SNS_APIManager.Instance.RequestGetUserDetails("myProfile");
            //}

            //if (FeedUIController.Instance != null)
            //{
            //    if (FeedUIController.Instance.feedUiScreen.activeSelf)
            //    {
            //        FeedUIController.Instance.SetUpFeedTabDefaultTop();//set default scroll top.......
            //    }
            //}
            gameManager.UiManager.HomeWorldScreen.SetActive(false);
            gameManager.UiManager._footerCan.GetComponent<CanvasGroup>().alpha = 0;
            gameManager.UiManager._footerCan.GetComponent<CanvasGroup>().interactable = false;
            gameManager.UiManager._footerCan.GetComponent<CanvasGroup>().blocksRaycasts = false;

            FeedUIController.Instance.footerCan.GetComponent<CanvasGroup>().alpha = 1;
            FeedUIController.Instance.footerCan.GetComponent<CanvasGroup>().interactable = true;
            FeedUIController.Instance.footerCan.GetComponent<CanvasGroup>().blocksRaycasts = true;
            if (gameManager.UiManager.Canvas.activeSelf)
            {
                // gameManager.UiManager.Canvas.SetActive(false);
                Invoke("ClearUnloadAssetData", 0.2f);
            }

            DisableSubScreen();
        }
        if (MyProfileDataManager.Instance)
        {
            MyProfileDataManager.Instance.MyProfileSceenShow(false);
            MyProfileDataManager.Instance.OtherPlayerdataObj.SetActive(true);
            FeedUIController.Instance.AddFriendPanel.SetActive(true);
            MyProfileDataManager.Instance.gameObject.SetActive(false);
        }
        else
        {
            FeedUIController.Instance.AddFriendPanel.SetActive(true);
            OtherPlayerProfileData.Instance.myPlayerdataObj.GetComponent<MyProfileDataManager>().myProfileScreen.SetActive(false);
            OtherPlayerProfileData.Instance.myPlayerdataObj.gameObject.SetActive(false);
        }
        FeedUIController.Instance.feedUiScreen.SetActive(false);
        if (MyProfileDataManager.Instance)
        {
            MyProfileDataManager.Instance.UpdateBackButtonAction(OnClickAddFriends);
        }
        OnScreenTabStateChange?.Invoke(BackButtonHandler.screenTabs.Othertabs);
        QuestDataHandler.Instance.OpenAndCloseQuestPanel(false);
    }

    void DisableSubScreen()
    {
        if (FeedUIController.Instance != null)
        {
            FeedUIController.Instance.SNSSettingController.settingScreen.SetActive(false);
            FeedUIController.Instance.SNSSettingController.myAccountScreen.SetActive(false);
            if (FeedUIController.Instance.BestFriendFull.activeInHierarchy || FeedUIController.Instance.ConfirmUnfollowPanel.activeInHierarchy)
            {
                FeedUIController.Instance.BestFriendFull.SetActive(false);
                FeedUIController.Instance.ConfirmUnfollowPanel.SetActive(false);
            }
        }
    }


    //void InvokeDisableFeed(){ 
    //    FeedUIController.Instance.feedUiScreen.SetActive(false);
    //}

    //this method is used to Profile button click.......
    public void OnClickProfileButton()
    {
        if (PlayerPrefs.GetInt("IsLoggedIn") != 0)
        {
            gameManager.HomeCameraInputHandler(false);

            if (/*gameManager.defaultSelection != 4*/ true)
            {
                // gameManager.mainCharacter.GetComponent<AvatarControllerHome>().UpdateState(true);
                //---->>>Sannan OnSelectedClick(4);
                if (GlobalVeriableClass.callingScreen == "Profile")
                    return;


                if (FeedUIController.Instance)
                {
                    FeedUIController.Instance.feedUiScreen.SetActive(false);
                }

                if (ProfileUIHandler.instance)
                {
                    // Reset Scroller position 
                    Transform contantObj = ProfileUIHandler.instance.mainscrollControllerRef.m_ScrollRect.content.transform;
                    Vector2 tempPos = contantObj.position;
                    tempPos.y = 0f;
                    contantObj.position = tempPos;
                }

                gameManager.defaultSelection = 4;
                GlobalVeriableClass.callingScreen = "Profile";
                gameManager.ActorManager._cinemaCam.SetActive(true);
                // LoaderShow(true);
                //gameManager.ActorManager.IdlePlayerAvatorForMenu(true);

                if (additiveScenesManager != null)
                {
                    additiveScenesManager.SNSmodule.SetActive(true);
                    // additiveScenesManager.SNSMessage.SetActive(false);
                    gameManager.defaultSelection = 4;
                    gameManager.UiManager._footerCan.transform.GetChild(0).GetComponent<HomeFooterHandler>().OnSelectedClick(4);
                    FeedUIController.Instance.footerCan.GetComponent<HomeFooterHandler>().OnSelectedClick(4);
                }
                else
                {
                    if (SceneManager.GetActiveScene().name != "SNSFeedModuleScene")
                    {
                        Initiate.Fade("SNSFeedModuleScene", Color.black, 1.0f, true);
                    }
                }
                //Commented in order to make profile 2.0 work after ahsan removed old feedui object from scene ----- UMER
                //if (!MyProfileDataManager.Instance.myProfileScreen.activeSelf)
                //{
                //MyProfileDataManager.Instance.ProfileTabButtonClick();
                //FeedUIController.Instance.ResetAllFeedScreen(false);
                //}
                if (MyProfileDataManager.Instance)
                {
                    MyProfileDataManager.Instance.ProfileTabButtonClick();
                    FeedUIController.Instance.ResetAllFeedScreen(false);
                    FeedUIController.Instance.AddFriendPanel.SetActive(false);
                    FeedUIController.Instance.ShowLoader(true);
                }
                if (gameManager.UiManager.Canvas.activeSelf)
                {
                    gameManager.UiManager._footerCan.GetComponent<CanvasGroup>().alpha = 0; // hiding home footer
                    gameManager.UiManager._footerCan.GetComponent<CanvasGroup>().interactable = false;
                    gameManager.UiManager._footerCan.GetComponent<CanvasGroup>().blocksRaycasts = false;
                    gameManager.UiManager.Canvas.SetActive(false);

                    gameManager.UiManager.HomeWorldScreen.SetActive(false);
                    FeedUIController.Instance.footerCan.GetComponent<HomeFooterHandler>().HomeSceneFooterSNSButtonIntrectableTrueFalse();
                    FeedUIController.Instance.footerCan.GetComponent<CanvasGroup>().alpha = 1;
                    FeedUIController.Instance.footerCan.GetComponent<CanvasGroup>().interactable = true;
                    FeedUIController.Instance.footerCan.GetComponent<CanvasGroup>().blocksRaycasts = true;
                    Invoke("ClearUnloadAssetData", 0.2f);
                }
                //gameManager.ActorManager.IdlePlayerAvatorForPostMenu(true);
                if (OtherPlayerProfileData.Instance)
                {
                    OtherPlayerProfileData.Instance.myPlayerdataObj.SetActive(true);
                    MyProfileDataManager.Instance.ResetMainScrollDefaultTopPos();
                }
                if (MyProfileDataManager.Instance)
                {
                    MyProfileDataManager.Instance.OtherPlayerdataObj.SetActive(false);
                }
                ConstantsHolder.xanaConstants.SnsProfileID = SNS_APIManager.Instance.userId;
                ConstantsHolder.xanaConstants.IsProfileVisit = true;
                ConstantsHolder.xanaConstants.IsOtherProfileVisit = false;
                ProfileUIHandler.instance.SwitchBetweenUserAndOtherProfileUI(true);
                ProfileUIHandler.instance.SetMainScrollRefs();
                ProfileUIHandler.instance.SetUserAvatarClothing(gameManager.mainCharacter.GetComponent<AvatarController>()._PCharacterData);
                ProfileUIHandler.instance.editProfileBtn.SetActive(true);
                ProfileUIHandler.instance.followProfileBtn.SetActive(false);
                DisableSubScreen();
            }

            //home page thumnbail images destroy
            WorldManager.instance.ClearHomePageData();
            gameManager.FriendsHomeManager.GetComponent<FriendHomeManager>().EnableFriendsView(false);
            if (MyProfileDataManager.Instance)
            {
                MyProfileDataManager.Instance.UpdateBackButtonAction(OnClickProfileButton);
            }
            OnScreenTabStateChange?.Invoke(BackButtonHandler.screenTabs.Othertabs);
            QuestDataHandler.Instance.OpenAndCloseQuestPanel(false);
        }
        else {
            if (ConstantsHolder.xanaConstants.SwitchXanaToXSummit)
            {
                    Screen.orientation = ScreenOrientation.LandscapeLeft;
                    UserLoginSignupManager.instance.LoginRegisterScreen.SetActive(true);
            }
            else
            {
                UserLoginSignupManager.instance.LoginRegisterScreen.SetActive(true);
            }
            
        }
    }
    public void InitProfileData()
    {
        gameManager.HomeCameraInputHandler(false);
        if (FeedUIController.Instance)
        {
            FeedUIController.Instance.feedUiScreen.SetActive(false);
        }

        if (ProfileUIHandler.instance)
        {
            // Reset Scroller position 
            Transform contantObj = ProfileUIHandler.instance.mainscrollControllerRef.m_ScrollRect.content.transform;
            Vector2 tempPos = contantObj.position;
            tempPos.y = 0f;
            contantObj.position = tempPos;
        }
        gameManager.defaultSelection = 4;
        //GlobalVeriableClass.callingScreen = "Profile";
        gameManager.ActorManager._cinemaCam.SetActive(true);

        if (additiveScenesManager != null)
        {
            additiveScenesManager.SNSmodule.SetActive(true);
            // additiveScenesManager.SNSMessage.SetActive(false);
            gameManager.defaultSelection = 4;
            FeedUIController.Instance.footerCan.GetComponent<HomeFooterHandler>().OnSelectedClick(4);
        }
        else
        {
            if (SceneManager.GetActiveScene().name != "SNSFeedModuleScene")
            {
                Initiate.Fade("SNSFeedModuleScene", Color.black, 1.0f, true);
            }
        }

        if (gameManager.UiManager.Canvas.activeSelf)
        {
            gameManager.UiManager._footerCan.GetComponent<CanvasGroup>().alpha = 0; // hiding home footer
            gameManager.UiManager._footerCan.GetComponent<CanvasGroup>().interactable = false;
            gameManager.UiManager._footerCan.GetComponent<CanvasGroup>().blocksRaycasts = false;
            gameManager.UiManager.Canvas.SetActive(false);

            gameManager.UiManager.HomeWorldScreen.SetActive(false);
            FeedUIController.Instance.footerCan.GetComponent<HomeFooterHandler>().HomeSceneFooterSNSButtonIntrectableTrueFalse();
            FeedUIController.Instance.footerCan.GetComponent<CanvasGroup>().alpha = 1;
            FeedUIController.Instance.footerCan.GetComponent<CanvasGroup>().interactable = true;
            FeedUIController.Instance.footerCan.GetComponent<CanvasGroup>().blocksRaycasts = true;
            Invoke("ClearUnloadAssetData", 0.2f);
        }

        DisableSubScreen();
    }
    public void ShopButtonClicked()
    {
        if (!GameManager.Instance.isAllSceneLoaded)
            return;

        if (additiveScenesManager != null && additiveScenesManager.SNSmodule)
        {
            additiveScenesManager.SNSmodule.SetActive(false);
            // additiveScenesManager.SNSMessage.SetActive(false);
            // FeedUIController.Instance.footerCan.GetComponent<HomeFooterHandler>().gameManager.defaultSelection = 4;
            //  FeedUIController.Instance.footerCan.GetComponent<HomeFooterHandler>().OnSelectedClick(4);
        }
        // gameManager.ActorManager.IdlePlayerAvatorForMenu(true);
        //  gameManager.userAnimationPostFeature.GetComponent<UserPostFeature>().ActivatePostButtbleHome(false);
        // gameManager.mainCharacter.GetComponent<AvatarControllerHome>().UpdateState(true);
        gameManager.UiManager.HomeWorldScreen.SetActive(false);
        ConstantsHolder.xanaConstants.isStoreActive = true;
        InventoryManager.upateAssetOnGenderChanged?.Invoke();

        OnScreenTabStateChange?.Invoke(BackButtonHandler.screenTabs.Avatar);
    }
    public void SetDefaultButtonSelection(int index)
    {
        switch (index)
        {
            case 3:
                OnSelectedClick(3);
                gameManager.defaultSelection = 3;
                GlobalVeriableClass.callingScreen = "Feed";
                break;
            case 4:
                OnSelectedClick(4);
                gameManager.defaultSelection = 4;
                GlobalVeriableClass.callingScreen = "Profile";
                break;
            default:
                break;
        }
    }
    public void MessageUnReadCountSetUp(int messageUnReadCount)
    {
        if (messageUnReadCount <= 0)
        {
            chatMessageUnReadCountObj.SetActive(false);
        }
        else
        {
            chatMessageUnReadCountText.text = messageUnReadCount.ToString();
            chatMessageUnReadCountObj.SetActive(true);
        }
    }

    void ClearUnloadAssetData()
    {
        //Resources.UnloadUnusedAssets();
    }

    public void createBackFromSns()
    {
        ConstantsHolder.xanaConstants.isBackfromSns = true;
    }


    public void ComingSoon()
    {
        SNSNotificationHandler.Instance.ShowNotificationMsg("Coming soon");
    }
}