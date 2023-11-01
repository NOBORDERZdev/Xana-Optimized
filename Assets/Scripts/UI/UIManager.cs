using AdvancedInputFieldPlugin;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public GameObject LoginRegisterScreen, SignUpScreen, HomePage, Canvas;
    public GameObject _SplashScreen;
    public Transform SecondSliderScrollView;

    [Header("Footer Reference")]
    public GameObject _footerCan;
    public GameObject faceMorphPanel;
    [Space(5)]
    [Header("New World Layout References")]
    public Transform SearchHomeHolder;
    public Transform SearchWorldHolder,
        SearchWorldScreenHolder,
        SearchViewAllWorldScreenHolder,
        AvatarWindowHolder,
        HomeWorldTabsHolder,
        WorldWorldTabsHolder,
        WorldScrollerHolder,
        LobbyTabHolder,
        AdvanceSearchInputField,
        CategoryHolderWorldView;

    int stateofDevice = 0;
    private void Awake()
    {
        Instance = this;
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
            StartCoroutine(IsSplashEnable(false, 4f));
        }
        else
        {
            StartCoroutine(IsSplashEnable(false, 0f));
            StartCoroutine(LoadingHandler.Instance.ShowLoadingForCharacterUpdation(4));
        }
        stateofDevice = PlayerPrefs.GetInt("DeviceSizeStateXana");
        ContentScreenSpaceSet(S1Dimension[stateofDevice]);
    }
    public IEnumerator IsSplashEnable(bool _state, float _time)
    {
        SavaCharacterProperties.NeedToShowSplash = 2;
        yield return new WaitForSeconds(_time);
        _SplashScreen.SetActive(_state);
        ShowFooter(!_state);
    }
    public List<float> S1Dimension, S2Dimension = new List<float>();
    APIURL ViewAllWorldName = default;
    public int PreviousScreen,ScreenSpaceIndex = 0;
    public void SwitchToScreen(int Screen)
    {
        switch(Screen)
        {
            case 0:
                {
                    PreviousScreen = 0;
                    StartCoroutine(ActivateCategoryWindow(true));
                    SearchViewAllWorldScreenHolder.gameObject.SetActive(false);
                    SearchWorldScreenHolder.gameObject.SetActive(false);
                    SearchHomeHolder.gameObject.SetActive(true);
                    SearchWorldHolder.gameObject.SetActive(false);
                    AvatarWindowHolder.gameObject.SetActive(true);
                    LobbyTabHolder.gameObject.SetActive(LobbyTabHolder.GetComponent<LobbyWorldViewFlagHandler>().ActivityInApp());
                    HomeWorldTabsHolder.gameObject.SetActive(false);
                    WorldWorldTabsHolder.gameObject.SetActive(false);
                    WorldManager.instance.WorldPageStateHandler(false);
                    WorldManager.instance.WorldScrollReset();
                    WorldManager.instance.AllWorldTabReference.AvatarWindowSizeControl(false);
                    SecondSliderScrollView.GetComponent<Mask>().enabled = false;
                    WorldScrollerHolder.gameObject.SetActive(false);
                    WorldManager.instance.transform.GetComponent<MainMenuWorldManager>().ResetCategoryScrollers();
                    break;
                }
            case 1:
                {
                    PreviousScreen = 1;
                    StartCoroutine(ActivateCategoryWindow(true));
                    SearchViewAllWorldScreenHolder.gameObject.SetActive(false);
                    SearchWorldScreenHolder.gameObject.SetActive(false);
                    SearchHomeHolder.gameObject.SetActive(false);
                    SearchWorldHolder.gameObject.SetActive(true);
                    AvatarWindowHolder.gameObject.SetActive(false);
                    LobbyTabHolder.gameObject.SetActive(false);
                    HomeWorldTabsHolder.gameObject.SetActive(false);
                    WorldWorldTabsHolder.gameObject.SetActive(false);
                    WorldManager.instance.WorldPageStateHandler(false);
                    WorldManager.instance.WorldScrollReset();
                    WorldManager.instance.AllWorldTabReference.AvatarWindowSizeControl(true);
                    SecondSliderScrollView.GetComponent<Mask>().enabled = true;
                    WorldScrollerHolder.gameObject.SetActive(false);
                    WorldManager.instance.transform.GetComponent<MainMenuWorldManager>().ResetCategoryScrollers();
                    break;
                }
            case 2:
                {
                    StartCoroutine(ActivateCategoryWindow(false));
                    AdvanceSearchInputField.GetComponent<AdvancedInputField>().Clear();
                    SearchWorldScreenHolder.gameObject.SetActive(true);
                    SearchHomeHolder.gameObject.SetActive(false);
                    SearchWorldHolder.gameObject.SetActive(false);
                    SearchViewAllWorldScreenHolder.gameObject.SetActive(false);
                    AvatarWindowHolder.gameObject.SetActive(false);
                    LobbyTabHolder.gameObject.SetActive(false);
                    HomeWorldTabsHolder.gameObject.SetActive(false);
                    WorldWorldTabsHolder.gameObject.SetActive(false);
                    WorldScrollerHolder.gameObject.SetActive(true);
                    WorldManager.instance.WorldPageStateHandler(true);
                    WorldManager.instance.WorldScrollReset();
                    SecondSliderScrollView.GetComponent<Mask>().enabled = true;
                    break;
                }
            case 3:
                {
                    StartCoroutine(ActivateCategoryWindow(false));
                    SearchViewAllWorldScreenHolder.gameObject.SetActive(true);
                    SearchWorldScreenHolder.gameObject.SetActive(false);
                    SearchHomeHolder.gameObject.SetActive(false);
                    SearchWorldHolder.gameObject.SetActive(false);
                    AvatarWindowHolder.gameObject.SetActive(false);
                    LobbyTabHolder.gameObject.SetActive(false);
                    HomeWorldTabsHolder.gameObject.SetActive(false);
                    WorldWorldTabsHolder.gameObject.SetActive(false);
                    WorldScrollerHolder.gameObject.SetActive(true);
                    WorldManager.instance.WorldPageStateHandler(true);
                    WorldManager.instance.WorldScrollReset();
                    SecondSliderScrollView.GetComponent<Mask>().enabled = true;
                    WorldManager.instance.ChangeWorld(ViewAllWorldName);
                    break;
                }
        }
        switch (ScreenSpaceIndex)
        {
            case 0: //// H S0
                {
                    if (Screen == 1)
                    {
                        ContentScreenSpaceSet(-S1Dimension[stateofDevice]);
                        ContentScreenSpaceSet(S2Dimension[stateofDevice]);
                        ScreenSpaceIndex = 1;
                    }
                    else if (Screen == 2 || Screen == 3)
                    {
                        ContentScreenSpaceSet(-S1Dimension[stateofDevice]);
                        ScreenSpaceIndex = 2;
                    }
                    break;
                }
            case 1: //// W S1
                {
                    if (Screen == 0)
                    {
                        ContentScreenSpaceSet(-S2Dimension[stateofDevice]);
                        ContentScreenSpaceSet(S1Dimension[stateofDevice]);
                        ScreenSpaceIndex = 0;
                    }
                    else if (Screen == 2 || Screen == 3)
                    {
                        ContentScreenSpaceSet(-S2Dimension[stateofDevice]);
                        ScreenSpaceIndex = 2;
                    }
                    break;
                }
            case 2: //// V S S3  S4
                {
                    if (Screen == 0)
                    {
                        ContentScreenSpaceSet(S1Dimension[stateofDevice]);
                        ScreenSpaceIndex = 0;
                    }
                    else if (Screen == 1)
                    {
                        ContentScreenSpaceSet(S2Dimension[stateofDevice]);
                        ScreenSpaceIndex = 1;
                    }
                    break;
                }
        }
    }
    void ContentScreenSpaceSet(float height)
    {
        Vector2 contentsize = WorldManager.instance.AllWorldTabReference.transform.GetComponent<RectTransform>().offsetMin;
        WorldManager.instance.AllWorldTabReference.transform.GetComponent<RectTransform>().offsetMin =
            new Vector2(contentsize.x, contentsize.y + height);

    }
    public void SetWorldToDisplay(APIURL worldCheck)
    {
        ViewAllWorldName = worldCheck;
        SwitchToScreen(3);
    }
    public IEnumerator ActivateCategoryWindow(bool _state)
    {
        yield return new WaitForSeconds(0.7f);
        CategoryHolderWorldView.gameObject.SetActive(_state);

    }
}