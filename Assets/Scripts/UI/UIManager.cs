using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun.Demo.PunBasics;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public GameObject LoginRegisterScreen, SignUpScreen, HomePage, WorldPage, Canvas, HotSection;
    public GameObject _SplashScreen;

    [Header("Footer Reference")]
    public GameObject _footerCan;
    public GameObject faceMorphPanel;
    [Space(5)]
    [Header("Home Section")]
    public GridLayoutGroup HotHomeSection;
    public GridLayoutGroup NewHomeSection;
    [Space(5)]
    [Header("Worlds Section")]
    public GridLayoutGroup HotWorldSection;
    public GridLayoutGroup NewWorldSection;
    [Space(5)]
    [Header("Find World Section")]
    public GridLayoutGroup FindScetion;
    [Space(5)]
    [Header("New World Layout References")]
    public Transform SearchHomeHolder;
    public Transform SearchWorldHolder, 
        AvatarWindowHolder,
        HomeWorldTabsHolder, 
        WorldWorldTabsHolder, 
        WorldScrollerHolder,
        LobbyTabHolder;


    private void Awake()
    {
        Instance = this;
    }
    public void AvaterButtonCustomPushed()
    {
        LoginPageManager.m_WorldIsClicked = false;
        LoginPageManager.m_MuseumIsClicked = false;
    }
    public void IsWorldClicked()
    {
        if(LoginPageManager.m_WorldIsClicked || LoginPageManager.m_MuseumIsClicked || UserRegisterationManager.instance.LoggedIn)
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
        if (XanaConstants.xanaConstants.screenType == XanaConstants.ScreenType.TabScreen)
        {
            HotHomeSection.constraintCount = 4;
            NewHomeSection.constraintCount = 4;
        }
    }
    public IEnumerator IsSplashEnable(bool _state, float _time)
    {
        SavaCharacterProperties.NeedToShowSplash = 2;
        yield return new WaitForSeconds(_time);
        _SplashScreen.SetActive(_state);
        ShowFooter(!_state);
    }
    public void SwitchToScreen(int Screen)
    {
        switch(Screen)
        {
            case 0:
                {
                    SearchHomeHolder.gameObject.SetActive(true);
                    SearchWorldHolder.gameObject.SetActive(false);
                    AvatarWindowHolder.gameObject.SetActive(true);
                    LobbyTabHolder.gameObject.SetActive(true);
                    HomeWorldTabsHolder.gameObject.SetActive(true);
                    WorldWorldTabsHolder.gameObject.SetActive(false);
                    break;
                }
            case 1:
                {
                    SearchHomeHolder.gameObject.SetActive(false);
                    SearchWorldHolder.gameObject.SetActive(true);
                    AvatarWindowHolder.gameObject.SetActive(false);
                    LobbyTabHolder.gameObject.SetActive(false);
                    HomeWorldTabsHolder.gameObject.SetActive(false);
                    WorldWorldTabsHolder.gameObject.SetActive(true);
                    break;
                }
        }
    }
}