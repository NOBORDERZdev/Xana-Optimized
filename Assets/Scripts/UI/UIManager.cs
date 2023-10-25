using AdvancedInputFieldPlugin;
using System.Collections;
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
    }
    public IEnumerator IsSplashEnable(bool _state, float _time)
    {
        SavaCharacterProperties.NeedToShowSplash = 2;
        yield return new WaitForSeconds(_time);
        _SplashScreen.SetActive(_state);
        ShowFooter(!_state);
    }
    APIURL ViewAllWorldName = default;
    public int PreviousScreen;
    public void SwitchToScreen(int Screen)
    {
        switch(Screen)
        {
            case 0:
                {
                    PreviousScreen = 0;
                    CategoryHolderWorldView.gameObject.SetActive(true);
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
                    SecondSliderScrollView.GetComponent<Mask>().enabled = false;
                    WorldScrollerHolder.gameObject.SetActive(false);
                    break;
                }
            case 1:
                {
                    PreviousScreen = 1;
                    CategoryHolderWorldView.gameObject.SetActive(true);
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
                    SecondSliderScrollView.GetComponent<Mask>().enabled = true;
                    WorldScrollerHolder.gameObject.SetActive(false);
                    break;
                }
            case 2:
                {
                    CategoryHolderWorldView.gameObject.SetActive(false);
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
                    CategoryHolderWorldView.gameObject.SetActive(false);
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
    }
    public void SetWorldToDisplay(APIURL worldCheck)
    {
        ViewAllWorldName = worldCheck;
        SwitchToScreen(3);
    }
}