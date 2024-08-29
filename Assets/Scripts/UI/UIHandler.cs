using AdvancedInputFieldPlugin;
using System.Collections;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIHandler : MonoBehaviour
{
    public GameObject LoginRegisterScreen, SignUpScreen, HomePage, Canvas,HomeWorldScreen;
     public CanvasGroup Loadinghandler_CanvasRef;
    public GameObject _SplashScreen;
    public GameObject SplashScreenXana;
    public GameObject SplashScreenSummit;

    public Transform _postScreen,_postCamera, _postScreenBG;
    public bool IsSplashActive = true;
    /*public Transform SecondSliderScrollView;*/

    [Header("Footer Reference")]
    public GameObject _footerCan;
    public GameObject faceMorphPanel;
    //[Space(5)]
    //[Header("New World Layout References")]
    //public Transform SearchHomeHolder;
    public Transform /*SearchWorldHolder,*/
        SearchWorldScreenHolder;
        //AvatarWindowHolder,
        
        //WorldWorldTabsHolder, 
        /*,*/
        //LobbyTabHolder,
        /*AdvanceSearchInputField*/

    //public GameObject worldHolder;
    public GameObject searchWorldHolder;


    public bool isAvatarSelectionBtnClicked = false;
    [SerializeField] Color postButtonColor;

    public static event Action<BackButtonHandler.screenTabs> OnScreenTabStateChange;

    private void Awake()
    {
        Canvas.GetComponent<CanvasGroup>().alpha = 0;
        Canvas.GetComponent<CanvasGroup>().blocksRaycasts = false;
        Canvas.GetComponent<CanvasGroup>().interactable = false;
        _footerCan.GetComponent<CanvasGroup>().alpha = 0.0f;
        _footerCan.GetComponent<CanvasGroup>().interactable = false;
        _footerCan.GetComponent<CanvasGroup>().blocksRaycasts = false;
        _SplashScreen.SetActive(false);
        _SplashScreen.SetActive(true);
    }
    bool a =false;


    public void AvatarSelectionBtnClicked()
    {
        if (!GameManager.Instance.isAllSceneLoaded)
            return;

        if (!isAvatarSelectionBtnClicked)
            isAvatarSelectionBtnClicked = true;
        GameManager.Instance.HomeCameraInputHandler(false);

    }

    public void SwitchToPostScreen(bool flag)
    {

        if (!GameManager.Instance.isAllSceneLoaded)
            return;

        if ( (PlayerPrefs.GetInt("IsLoggedIn") == 0))
        {
           // SNSNotificationHandler.Instance.ShowNotificationMsg("Need To Login");
        }
        else
        {
            CharacterHandler.instance.playerNameCanvas.SetActive(!flag);
            GameManager.Instance.HomeCameraInputHandler(!flag);

            _postScreen.gameObject.SetActive(flag);
            _postScreenBG.gameObject.SetActive(flag);
            HomePage.gameObject.SetActive(!flag);
            _postCamera.gameObject.SetActive(flag);
            ShowFooter(!flag);
            GameManager.Instance.ActorManager.IdlePlayerAvatorForPostMenu(flag);
            GameManager.Instance.userAnimationPostFeature.GetComponent<UserPostFeature>().ActivatePostButtbleHome(!flag);
            GameManager.Instance.userAnimationPostFeature.postButton.interactable = false;
            GameManager.Instance.userAnimationPostFeature.postButtonText.color = postButtonColor;
        }

        OnScreenTabStateChange?.Invoke(BackButtonHandler.screenTabs.Post);
    }
    public void ResetPlayerToLastPostPosted()
    {
        GameManager.Instance.userAnimationPostFeature.transform.GetComponent<UserPostFeature>().SetLastPostToPlayer();
        GameManager.Instance.HomeCamera.GetComponent<HomeCameraController>().CenterAlignCam();
        OnScreenTabStateChange?.Invoke(BackButtonHandler.screenTabs.Hometab);
    }
    public void AvaterButtonCustomPushed()
    {
        WorldDescriptionPopupPreview.m_WorldIsClicked = false;
        WorldDescriptionPopupPreview.m_MuseumIsClicked = false;
    }
    public void IsWorldClicked()
    {
        if(WorldDescriptionPopupPreview.m_WorldIsClicked || WorldDescriptionPopupPreview.m_MuseumIsClicked || ConstantsHolder.loggedIn)
            WorldManager.instance.PlayWorld();
    }
    public void ShowFooter(bool _state)
    {
        _footerCan.SetActive(_state);
    }
    private void Start()
    {
        if (!ConstantsHolder.xanaConstants.SwitchXanaToXSummit)
        {
            SplashScreenXana.SetActive(true);
            SplashScreenSummit.SetActive(false);
            if (Screen.orientation == ScreenOrientation.LandscapeRight || Screen.orientation == ScreenOrientation.LandscapeLeft)
            {
                Screen.orientation = ScreenOrientation.Portrait;
            }

        }
        else 
        {
            SplashScreenXana.SetActive(false);
            SplashScreenSummit.SetActive(true);
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }
        if (SaveCharacterProperties.NeedToShowSplash == 1)
        {
            if (PlayerPrefs.HasKey("TermsConditionAgreement"))
            {
                IsSplashActive = false;
                StartCoroutine(IsSplashEnable(false, 3f));
                if (!ConstantsHolder.xanaConstants.SwitchXanaToXSummit)
                {
                    if (Screen.orientation == ScreenOrientation.LandscapeRight || Screen.orientation == ScreenOrientation.LandscapeLeft)
                    {
                        Screen.orientation = ScreenOrientation.Portrait;
                    }

                }
                else
                {
                    Screen.orientation = ScreenOrientation.LandscapeLeft;
                }
            }
         }
        else
        {
            StartCoroutine(IsSplashEnable(false, 0f));
            //StartCoroutine(LoadingHandler.Instance.ShowLoadingForCharacterUpdation(5));
        }
    }
    public IEnumerator IsSplashEnable(bool _state, float _time)
    {
        SaveCharacterProperties.NeedToShowSplash = 2;
        Canvas.GetComponent<CanvasGroup>().alpha = 0;
        LoadingHandler.Instance.worldLoadingScreen.GetComponent<CanvasGroup>().alpha = 0.0f;
        _footerCan.GetComponent<CanvasGroup>().alpha = 0.0f;
         Canvas.GetComponent<CanvasGroup>().interactable =false;
        Canvas.GetComponent<CanvasGroup>().blocksRaycasts =false;
        _footerCan.GetComponent<CanvasGroup>().interactable=false;
        _footerCan.GetComponent<CanvasGroup>().blocksRaycasts=false;
        yield return new WaitForSeconds(_time);
        _SplashScreen.SetActive(_state);
        Canvas.GetComponent<CanvasGroup>().alpha = 1.0f;
        Canvas.GetComponent<CanvasGroup>().interactable =true;
        Canvas.GetComponent<CanvasGroup>().blocksRaycasts =true;
        _footerCan.GetComponent<CanvasGroup>().interactable=true;
        _footerCan.GetComponent<CanvasGroup>().blocksRaycasts=true;
        LoadingHandler.Instance.worldLoadingScreen.GetComponent<CanvasGroup>().alpha = 1.0f;
        _footerCan.GetComponent<CanvasGroup>().alpha = 1.0f;
        if(Loadinghandler_CanvasRef != null)
            Loadinghandler_CanvasRef.alpha = 1.0f;
        ShowFooter(!_state);
        if (!_state)
        {
            SplashMemoryFree();
        }
    }

    public void SplashMemoryFree()
    {
        StartCoroutine(ReleaseSplashFromMemory());
    }

    IEnumerator ReleaseSplashFromMemory()
    {
        yield return new WaitForSeconds(2);
        Destroy(_SplashScreen);
    }


    public int PreviousScreen;
    public void SwitchToScreen(int Screen)
    {
        switch(Screen)
        {
            case 0:
                {
                    PreviousScreen = 0;
                    SearchWorldScreenHolder.gameObject.SetActive(false);
                    //SearchHomeHolder.gameObject.SetActive(true);
                    //SearchWorldHolder.gameObject.SetActive(false);
                    //AvatarWindowHolder.gameObject.SetActive(false);
                    /*LobbyTabHolder.gameObject.SetActive(LobbyTabHolder.GetComponent<LobbyWorldViewFlagHandler>().ActivityInApp());*/
                  //  HomeWorldTabsHolder.gameObject.SetActive(true);
                    //WorldWorldTabsHolder.gameObject.SetActive(false);
                    //WorldManager.instance.WorldPageStateHandler(false);
                    //WorldManager.instance.WorldScrollReset();
                    //worldHolder.SetActive(true);
                    searchWorldHolder.SetActive(false);
                    /*SecondSliderScrollView.GetComponent<Mask>().enabled = false;*/
                    break;
                }
            case 1:
                {
                    PreviousScreen = 1;
                    SearchWorldScreenHolder.gameObject.SetActive(false);
                    //SearchHomeHolder.gameObject.SetActive(false);
                    //SearchWorldHolder.gameObject.SetActive(true);
                    //AvatarWindowHolder.gameObject.SetActive(false);
                    /*LobbyTabHolder.gameObject.SetActive(false);*/
                    //  HomeWorldTabsHolder.gameObject.SetActive(false);
                    //WorldWorldTabsHolder.gameObject.SetActive(true);
                    //WorldManager.instance.WorldPageStateHandler(true);
                    //WorldManager.instance.WorldScrollReset();
                    /*SecondSliderScrollView.GetComponent<Mask>().enabled = true;*/
                    break;
                }
            case 2:
                {
                    //AdvanceSearchInputField.GetComponent<AdvancedInputField>().Clear();
                    WorldManager.instance.previousSearchKey = "";
                    WorldManager.instance.SearchKey = "";
                    SearchWorldScreenHolder.gameObject.SetActive(true);
                    //SearchHomeHolder.gameObject.SetActive(false);
                    //SearchWorldHolder.gameObject.SetActive(false);
                    //AvatarWindowHolder.gameObject.SetActive(false);
                    /*LobbyTabHolder.gameObject.SetActive(false);*/
                    //worldHolder.SetActive(false);
                    WorldManager.instance.worldSearchManager.searchWorldInput.Clear();
                    searchWorldHolder.SetActive(true);
                  //  HomeWorldTabsHolder.gameObject.SetActive(false);
                    //WorldWorldTabsHolder.gameObject.SetActive(false);
                    //WorldManager.instance.WorldPageStateHandler(true);
                    WorldManager.instance.WorldScrollReset();
                    WorldManager.instance.WorldLoadingText(APIURL.Temp);
                   /* SecondSliderScrollView.GetComponent<Mask>().enabled = true;*/
                    ShowFooter(true);
                    break;
                }
        }
    }
}