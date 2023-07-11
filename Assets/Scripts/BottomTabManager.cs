using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class BottomTabManager : MonoBehaviour
{
    public List<Image> allButtonIcon = new List<Image>();
    public List<Sprite> allButtonUnSelected = new List<Sprite>();
    public List<Sprite> allButtonSelected = new List<Sprite>();
    public List<Text> AllTitleText = new List<Text>();
    public Color sellectedColor = new Color();
    public Color unSellectedColor = new Color();
    public Color intractableFalseColor = new Color();
    public int defaultSelection = 0;
    // public GameObject loaderObj;
    public bool WaitToLoadAvatarData = false;
    public CanvasGroup canvasGroup;

    public GameObject chatMessageUnReadCountObj;
    public TextMeshProUGUI chatMessageUnReadCountText;

    private void Awake()
    {
        if (defaultSelection == 3)//default feed then set feed
        {
            if (GlobalVeriableClass.callingScreen == "Profile")//profile
            {
                defaultSelection = 4;
            }
            else
            {
                GlobalVeriableClass.callingScreen = "Feed";
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance._footerCan.transform.GetChild(0).GetComponent<BottomTabManager>().defaultSelection = 0;
        }
        OnSelectedClick(defaultSelection);

        if (UIManager.Instance != null && defaultSelection == 0)
        {
            CheckLoginOrNotForFooterButton();
        }
    }

    public void OnSelectedClick(int index)
    {
        for (int i = 0; i < allButtonIcon.Count; i++)
        {
            if (i == index)
            {
                //allButtonIcon[i].color = sellectedColor;
                allButtonIcon[i].sprite = allButtonSelected[i];
                //AllTitleText[i].color = sellectedColor;
                defaultSelection = index;
                if (i == 2)
                {
                    allButtonIcon[i].transform.GetChild(0).GetComponent<Image>().color = Color.white;
                }
            }
            else
            {
                //allButtonIcon[i].color = unSellectedColor;
                allButtonIcon[i].sprite = allButtonUnSelected[i];
                //AllTitleText[i].color = unSellectedColor;
                if (i == 2)
                {
                    allButtonIcon[i].transform.GetChild(0).GetComponent<Image>().color = Color.black;
                }
            }
        }
    }

    //this method is used to check is user login as guest then disable All SNS footer button.......
    public void CheckLoginOrNotForFooterButton()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance._footerCan.transform.GetChild(0).GetComponent<BottomTabManager>().HomeSceneFooterSNSButtonIntrectableTrueFalse();
        }
    }

    public void HomeSceneFooterSNSButtonIntrectableTrueFalse()
    {
        //Debug.LogError("ParentName:" + this.transform.parent.gameObject.name);
        for (int i = 2; i < allButtonIcon.Count; i++)
        {
            //  if (string.IsNullOrEmpty(PlayerPrefs.GetString("LoginToken")) || string.IsNullOrEmpty(PlayerPrefs.GetString("UserName")))
            if (PlayerPrefs.GetInt("IsLoggedIn") == 0)
            {
                allButtonIcon[i].color = new Color(intractableFalseColor.r, intractableFalseColor.g, intractableFalseColor.b, 0.5f);
                if (i == 2)
                {
                    allButtonIcon[i].transform.GetChild(0).GetComponent<Image>().color = Color.gray;
                }
                AllTitleText[i].color = intractableFalseColor;

                allButtonIcon[i].transform.parent.GetComponent<Button>().interactable = false;
            }
            else
            {
                //allButtonIcon[i].color = unSellectedColor;
                allButtonIcon[i].color = Color.white;
                if (i == 2)
                {
                    allButtonIcon[i].transform.GetChild(0).GetComponent<Image>().color = Color.black;
                }
                AllTitleText[i].color = unSellectedColor;
                allButtonIcon[i].transform.parent.GetComponent<Button>().interactable = true;
            }
        }

        if (CommonAPIManager.Instance != null && PlayerPrefs.GetInt("IsLoggedIn") != 0)//For Get All Chat UnRead Message Count.......
        {
            CommonAPIManager.Instance.RequestGetAllChatUnReadMessagesCount();
        }
        /*if (this.gameObject.activeInHierarchy)
        {
            if (waitToLoadAvatarDataCo != null)
            {
                StopCoroutine(waitToLoadAvatarDataCo);
            }
            waitToLoadAvatarDataCo = StartCoroutine(waitToAvatarDataLoad());
        }*/
    }

    Coroutine waitToLoadAvatarDataCo;
    IEnumerator waitToAvatarDataLoad()
    {
        yield return new WaitForSeconds(2f);
        WaitToLoadAvatarData = true;
    }

    //this method is used to Home button click.......
    public void OnClickHomeButton()
    {
        GlobalVeriableClass.callingScreen = "";
        Debug.Log("Home button onclick");
        // LoaderShow(false);
        if (defaultSelection != 0)
        {
            OnSelectedClick(0);
            //Initiate.Fade("Main", Color.black, 1.0f);
            if (FindObjectOfType<AdditiveScenesManager>() != null)
            {
                FindObjectOfType<AdditiveScenesManager>().SNSmodule.SetActive(false);
                FindObjectOfType<AdditiveScenesManager>().SNSMessage.SetActive(false);
            }
            if (UIManager.Instance != null)
            {
                UIManager.Instance._footerCan.transform.GetChild(0).GetComponent<BottomTabManager>().defaultSelection = 0;
                UIManager.Instance._footerCan.transform.GetChild(0).GetComponent<BottomTabManager>().OnSelectedClick(0);
            }

            UIManager.Instance.Canvas.SetActive(true);
            UIManager.Instance.HotSection.SetActive(true);
            UIManager.Instance.WorldPage.SetActive(false);

            // EventList.instance.ReLoadImageAfterEnable();
            WorldManager.instance.OpenXANAWorldPage();
        }
    }

    public void OnClickNewWorldButton()
    {
        if (!UIManager.Instance.WorldPage.activeSelf)
        {
            Debug.Log("World button onclick");
            if (defaultSelection != 1)
            {
                OnSelectedClick(1);
                //Initiate.Fade("Main", Color.black, 1.0f);
                if (FindObjectOfType<AdditiveScenesManager>() != null)
                {
                    FindObjectOfType<AdditiveScenesManager>().SNSmodule.SetActive(false);
                    FindObjectOfType<AdditiveScenesManager>().SNSMessage.SetActive(false);
                }
                if (UIManager.Instance != null)
                {
                    UIManager.Instance._footerCan.transform.GetChild(0).GetComponent<BottomTabManager>().defaultSelection = 1;
                    UIManager.Instance._footerCan.transform.GetChild(0).GetComponent<BottomTabManager>().OnSelectedClick(1);
                }

                UIManager.Instance.Canvas.SetActive(true);
                UIManager.Instance.WorldPage.SetActive(true);
                UIManager.Instance.HotSection.SetActive(false);

                //EventList.instance.ReLoadImageAfterEnable();
            }
            //  GameManager.Instance.BottomAvatarBtnPressed();
            WorldManager.instance.OpenAllWorldPage();
        }
        if (XanaConstants.xanaConstants.screenType == XanaConstants.ScreenType.TabScreen)
        {
            //Home Tab
            //HotHomeSection.constraintCount = 4;
            //HotHomeSection.cellSize = new Vector2(320, 320);
            //NewHomeSection.constraintCount = 4;
            //NewHomeSection.cellSize = new Vector2(320, 320);
            //World Tab
            //UIManager.Instance.HotWorldSection.transform.parent.GetComponent<gridsetup>().columns = 4;
            UIManager.Instance.HotWorldSection.constraintCount = 4;
            //UIManager.Instance.HotWorldSection.cellSize = new Vector2(320, 320);
            //UIManager.Instance.HotWorldSection.padding.left = 12;
            //UIManager.Instance.HotWorldSection.padding.right = 12;
            //UIManager.Instance.HotWorldSection.spacing = new Vector2(0,0);

            //UIManager.Instance.HotWorldSection.cellSize = new Vector2(320, 320);
            //UIManager.Instance.NewWorldSection.transform.parent.GetComponent<gridsetup>().columns = 4;
            UIManager.Instance.NewWorldSection.constraintCount = 4;
            //UIManager.Instance.NewWorldSection.cellSize = new Vector2(320,320);
            //UIManager.Instance.NewWorldSection.padding.left = 12;
           // UIManager.Instance.NewWorldSection.padding.right = 12;
           // UIManager.Instance.NewWorldSection.spacing = new Vector2(0, 0);
            //UIManager.Instance.NewWorldSection.cellSize = new Vector2(320, 320);
        }
    }


    public void OnClickAvatarButton()
    {
        if (defaultSelection != 0)
        {
            OnSelectedClick(1);
            //Initiate.Fade("Main", Color.black, 1.0f);
            if (FindObjectOfType<AdditiveScenesManager>() != null)
            {
                FindObjectOfType<AdditiveScenesManager>().SNSmodule.SetActive(false);
                FindObjectOfType<AdditiveScenesManager>().SNSMessage.SetActive(false);
            }
            if (UIManager.Instance != null)
            {
                UIManager.Instance._footerCan.transform.GetChild(0).GetComponent<BottomTabManager>().defaultSelection = 0;
                UIManager.Instance._footerCan.transform.GetChild(0).GetComponent<BottomTabManager>().OnSelectedClick(0);
            }

            UIManager.Instance.Canvas.SetActive(true);
            //EventList.instance.ReLoadImageAfterEnable();
        }
        GameManager.Instance.BottomAvatarBtnPressed();
    }
    //this method is used to Explore button click.......
    public void OnClickWorldButton()
    {
        if (!PremiumUsersDetails.Instance.CheckSpecificItem("sns_message"))
        {
            //PremiumUsersDetails.Instance.PremiumUserUI.SetActive(true);
            print("Please Upgrade to Premium account");
            return;
        }
        else
        {
            print("Horayyy you have Access");
        }

        //if (XanaConstants.xanaConstants.r_isSNSComingSoonActive)
        //{
        //    if (SNSNotificationManager.Instance != null)
        //    {
        //        SNSNotificationManager.Instance.ShowNotificationMsg("This features is coming soon");//this method is used to show SNS notification.......
        //        //SNSComingSoonManager.Instance.snsComingSoonScreen.SetActive(true);
        //        print("sns features coming soon.......");
        //        return;
        //    }
        //}

        GlobalVeriableClass.callingScreen = "";
        // LoaderShow(true);
        //Debug.Log("World button onclick");

        if (defaultSelection != 1)
        {
            OnSelectedClick(1);
            //defaultSelection = 1;
            //Initiate.Fade("SNSMessageModuleScene", Color.black, 1.0f, true);

            if (FindObjectOfType<AdditiveScenesManager>() != null)
            {
                if (MessageController.Instance != null)
                {
                    MessageController.Instance.isChatDetailsScreenDeactive = true;
                }
                FindObjectOfType<AdditiveScenesManager>().SNSMessage.SetActive(true);
                FindObjectOfType<AdditiveScenesManager>().SNSmodule.SetActive(false);
                MessageController.Instance.footerCan.GetComponent<BottomTabManager>().defaultSelection = 1;
                MessageController.Instance.footerCan.GetComponent<BottomTabManager>().OnSelectedClick(1);
            }
            else
            {
                Initiate.Fade("SNSMessageModuleScene", Color.black, 1.0f, true);
            }

            if (UIManager.Instance.Canvas.activeSelf)
            {
                UIManager.Instance.Canvas.SetActive(false);
                Invoke("ClearUnloadAssetData", 0.2f);
            }
        }
    }

    //this method is used to Event button click.......
    public void OnclickEventButton()
    {
        Debug.Log("OnclickEventButton");
        PremiumUsersDetails.Instance.OpenComingSoonPopUp();
    }

    //this method is used to create button click.......
    public void OnClickCreateButton()
    {
        Debug.Log("Create button onclick");

        /*if (!WaitToLoadAvatarData)
        {
            return;
        }*/

        if (defaultSelection != 5)
        {
            //OnSelectedClick(5);
            defaultSelection = 5;
            if (XanaConstants.xanaConstants.r_MainSceneAvatar != null)
            {
                Destroy(XanaConstants.xanaConstants.r_MainSceneAvatar);
                XanaConstants.xanaConstants.r_MainSceneAvatar = null;
            }
            GameObject MainSceneAvatar = Instantiate(GameManager.Instance.mainCharacter);
            /*if (MainSceneAvatar.GetComponent<Animator>() != null)
            {
                Destroy(MainSceneAvatar.GetComponent<Animator>());
            }*/

            Transform rootRotationObj = MainSceneAvatar.transform.Find("mixamorig:Hips");
            if (rootRotationObj != null)
            {
                Transform hadeObj = rootRotationObj.GetChild(2).GetChild(0).GetChild(0).GetChild(1).transform;
                hadeObj.localRotation = Quaternion.Euler(Vector3.zero);
                hadeObj.transform.GetChild(0).transform.localRotation = Quaternion.Euler(Vector3.zero);
            }

            DontDestroyOnLoad(MainSceneAvatar);
            MainSceneAvatar.SetActive(false);
            //Initiate.Fade("ARModuleActionScene", Color.black, 1.0f, true);
            Initiate.Fade("ARModuleRoomScene", Color.black, 1.0f, true);
            XanaConstants.xanaConstants.r_MainSceneAvatar = MainSceneAvatar;
            //FeedUIController.Instance.OnClickCreateFeedPickImageOrVideo();
        }
    }

    //this method is used to feed button click.......
    public void OnClickFeedButton()
    {
        if (!PremiumUsersDetails.Instance.CheckSpecificItem("sns_feed"))
        {
            //PremiumUsersDetails.Instance.PremiumUserUI.SetActive(true);
            print("Please Upgrade to Premium account");
            return;
        }
        else
        {
            print("Horayyy you have Access");
        }

        //if (XanaConstants.xanaConstants.r_isSNSComingSoonActive)
        //{
        //    if (SNSNotificationManager.Instance != null)
        //    {
        //        SNSNotificationManager.Instance.ShowNotificationMsg("This features is coming soon");//this method is used to show SNS notification.......
        //        //SNSComingSoonManager.Instance.snsComingSoonScreen.SetActive(true);
        //        print("sns features coming soon.......");
        //        return;
        //    }
        //}

        if (defaultSelection != 3)
        {
            // LoaderShow(true);
            OnSelectedClick(3);
            defaultSelection = 3;
            GlobalVeriableClass.callingScreen = "Feed";

            if (FindObjectOfType<AdditiveScenesManager>() != null)
            {
                FindObjectOfType<AdditiveScenesManager>().SNSmodule.SetActive(true);
                FindObjectOfType<AdditiveScenesManager>().SNSMessage.SetActive(false);
                FeedUIController.Instance.footerCan.GetComponent<BottomTabManager>().defaultSelection = 3;
                FeedUIController.Instance.footerCan.GetComponent<BottomTabManager>().OnSelectedClick(3);
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
            else
            {
                APIManager.Instance.RequestGetUserDetails("myProfile");
            }

            if (FeedUIController.Instance != null)
            {
                if (FeedUIController.Instance.feedUiScreen.activeSelf)
                {
                    FeedUIController.Instance.SetUpFeedTabDefaultTop();//set default scroll top.......
                }
            }

            /*if (SceneManager.GetActiveScene().name != "SNSFeedModuleScene")
            {
                // Initiate.Fade("SNSFeedModuleScene", Color.black, 1.0f, true);
            }
            else
            {
                if (MyProfileDataManager.Instance.myProfileScreen.activeSelf)
                {
                    FeedUIController.Instance.FadeInOutScreenShow();//show fade in out.......
                    FeedUIController.Instance.ResetAllFeedScreen(true);
                    StartCoroutine(MyProfileDataManager.Instance.MyProfileSceenShow(false));//false my profile screen
                    //  FindObjectOfType<AdditiveScenesManager>().SNSmodule.SetActive(true);
                    // FindObjectOfType<AdditiveScenesManager>().SNSMessage.SetActive(false);
                }
            }*/

            if (UIManager.Instance.Canvas.activeSelf)
            {
                UIManager.Instance.Canvas.SetActive(false);
                Invoke("ClearUnloadAssetData", 0.2f);
            }
        }
    }

    //this method is used to Profile button click.......
    public void OnClickProfileButton()
    {
        if (defaultSelection != 4)
        {
            OnSelectedClick(4);
            defaultSelection = 4;
            GlobalVeriableClass.callingScreen = "Profile";

            // LoaderShow(true);

            if (FindObjectOfType<AdditiveScenesManager>() != null)
            {
                FindObjectOfType<AdditiveScenesManager>().SNSmodule.SetActive(true);
                FindObjectOfType<AdditiveScenesManager>().SNSMessage.SetActive(false);
                FeedUIController.Instance.footerCan.GetComponent<BottomTabManager>().defaultSelection = 4;
                FeedUIController.Instance.footerCan.GetComponent<BottomTabManager>().OnSelectedClick(4);
            }
            else
            {
                if (SceneManager.GetActiveScene().name != "SNSFeedModuleScene")
                {
                    Initiate.Fade("SNSFeedModuleScene", Color.black, 1.0f, true);
                }
            }

            if (!MyProfileDataManager.Instance.myProfileScreen.activeSelf)
            {
                //FeedUIController.Instance.FadeInOutScreenShow();//show fade in out.......
                //FeedUIController.Instance.ShowLoader(true);
                MyProfileDataManager.Instance.ProfileTabButtonClick();
                FeedUIController.Instance.ResetAllFeedScreen(false);
            }

            /*if (SceneManager.GetActiveScene().name != "SNSFeedModuleScene")
            {
                //Initiate.Fade("SNSFeedModuleScene", Color.black, 1.0f, true);
            }
            else
            {
                if (!MyProfileDataManager.Instance.myProfileScreen.activeSelf)
                {
                    FeedUIController.Instance.FadeInOutScreenShow();//show fade in out.......
                    MyProfileDataManager.Instance.ProfileTabButtonClick();
                    FeedUIController.Instance.ResetAllFeedScreen(false);
                }
            }*/

            //Debug.Log("Profile button onclick");

            if (UIManager.Instance.Canvas.activeSelf)
            {
                UIManager.Instance.Canvas.SetActive(false);
                Invoke("ClearUnloadAssetData", 0.2f);
            }
        }
    }


    public void SetDefaultButtonSelection(int index)
    {
        switch (index)
        {
            case 3:
                OnSelectedClick(3);
                defaultSelection = 3;
                GlobalVeriableClass.callingScreen = "Feed";
                break;
            case 4:
                OnSelectedClick(4);
                defaultSelection = 4;
                GlobalVeriableClass.callingScreen = "Profile";
                break;
            default:
                break;
        }
    }

    //public void LoaderShow(bool isActive)
    //{
    //    Debug.Log("Loooooooooooooooooooooooooooooooder   Activated");
    //    loaderObj.SetActive(isActive);
    //}  

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
        Resources.UnloadUnusedAssets();
        //Caching.ClearCache();
        //GC.Collect();
    }
}