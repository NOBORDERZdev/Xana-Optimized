using AdvancedInputFieldPlugin;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public GameObject LoginRegisterScreen, SignUpScreen, HomePage, Canvas;
    public GameObject _SplashScreen;
    public bool IsSplashActive = true;
    public Transform SecondSliderScrollView;

    [Header("Footer Reference")]
    public GameObject _footerCan;
    public GameObject faceMorphPanel;
    [Space(5)]
    [Header("New World Layout References")]
    public Transform SearchHomeHolder;
    public Transform SearchWorldHolder, 
        SearchWorldScreenHolder,
        AvatarWindowHolder,
        HomeWorldTabsHolder, 
        WorldWorldTabsHolder, 
        WorldScrollerHolder,
        LobbyTabHolder,
        AdvanceSearchInputField;
   

    private void Awake()
    {
        Instance = this;
        _SplashScreen.SetActive(false);
        _SplashScreen.SetActive(true);
    }
    public void AvaterButtonCustomPushed()
    {
        WorldItemPreviewTab.m_WorldIsClicked = false;
        WorldItemPreviewTab.m_MuseumIsClicked = false;
    }
    public void IsWorldClicked()
    {
        if(WorldItemPreviewTab.m_WorldIsClicked || WorldItemPreviewTab.m_MuseumIsClicked || UserRegisterationManager.instance.LoggedIn)
            WorldManager.instance.PlayWorld();
    }
    public void ShowFooter(bool _state)
    {
        _footerCan.SetActive(_state);
    }
    private void Start()
    {
        if (SavaCharacterProperties.NeedToShowSplash == 1)
        {
            if (PlayerPrefs.HasKey("TermsConditionAgreement"))
            {
                IsSplashActive = false;
                StartCoroutine(IsSplashEnable(false, 3f));
               
            }
           
        }
        else
        {

            StartCoroutine(IsSplashEnable(false, 0f));
            StartCoroutine(LoadingHandler.Instance.ShowLoadingForCharacterUpdation(4));
        }
    }
   
    public IEnumerator IsSplashEnable(bool _state, float _time)
    {
        SavaCharacterProperties.NeedToShowSplash = 2;
        Canvas.GetComponent<CanvasGroup>().alpha = 0;
        LoadingHandler.Instance.worldLoadingScreen.GetComponent<CanvasGroup>().alpha = 0.0f;
         yield return new WaitForSeconds(_time);
        _SplashScreen.SetActive(_state);
        Canvas.GetComponent<CanvasGroup>().alpha = 1.0f;
        LoadingHandler.Instance.worldLoadingScreen.GetComponent<CanvasGroup>().alpha = 1.0f;
        ShowFooter(!_state);
        WorldManager.instance.EventPrefabLobby.GetComponent<WorldItemView>().LoadRFMDirectly();
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
                    SearchHomeHolder.gameObject.SetActive(true);
                    SearchWorldHolder.gameObject.SetActive(false);
                    AvatarWindowHolder.gameObject.SetActive(true);
                    LobbyTabHolder.gameObject.SetActive(LobbyTabHolder.GetComponent<LobbyWorldViewFlagHandler>().ActivityInApp());
                    HomeWorldTabsHolder.gameObject.SetActive(true);
                    WorldWorldTabsHolder.gameObject.SetActive(false);
                    WorldManager.instance.WorldPageStateHandler(false);
                    WorldManager.instance.WorldScrollReset();
                    SecondSliderScrollView.GetComponent<Mask>().enabled = false;
                    break;
                }
            case 1:
                {
                    PreviousScreen = 1;
                    SearchWorldScreenHolder.gameObject.SetActive(false);
                    SearchHomeHolder.gameObject.SetActive(false);
                    SearchWorldHolder.gameObject.SetActive(true);
                    AvatarWindowHolder.gameObject.SetActive(false);
                    LobbyTabHolder.gameObject.SetActive(false);
                    HomeWorldTabsHolder.gameObject.SetActive(false);
                    WorldWorldTabsHolder.gameObject.SetActive(true);
                    WorldManager.instance.WorldPageStateHandler(true);
                    WorldManager.instance.WorldScrollReset();
                    SecondSliderScrollView.GetComponent<Mask>().enabled = true;
                    break;
                }
            case 2:
                {

                    AdvanceSearchInputField.GetComponent<AdvancedInputField>().Clear();
                    SearchWorldScreenHolder.gameObject.SetActive(true);
                    SearchHomeHolder.gameObject.SetActive(false);
                    SearchWorldHolder.gameObject.SetActive(false);
                    AvatarWindowHolder.gameObject.SetActive(false);
                    LobbyTabHolder.gameObject.SetActive(false);
                    HomeWorldTabsHolder.gameObject.SetActive(false);
                    WorldWorldTabsHolder.gameObject.SetActive(false);
                    WorldManager.instance.WorldPageStateHandler(true);
                    WorldManager.instance.WorldScrollReset();
                    SecondSliderScrollView.GetComponent<Mask>().enabled = true;
                    ShowFooter(true);
                    break;
                }
        }
    }
}