using UnityEngine;
using System;

public class BackButtonHandler : MonoBehaviour
{

    public screenTabs _screenTabs;
    public static BackButtonHandler instance;
    public GameObject exitPanel;

    public enum screenTabs
    {
        Hometab,
        Othertabs,
        Avatar,
        Post,
        Gameplay
    }

    private void OnEnable()
    {
        HomeFooterHandler.OnScreenTabStateChange += setScreenTabEnum;
        UIHandler.OnScreenTabStateChange += setScreenTabEnum;
        InventoryManager.OnScreenTabStateChange += setScreenTabEnum;
    }

    private void OnDisable()
    {
        HomeFooterHandler.OnScreenTabStateChange -= setScreenTabEnum;
        UIHandler.OnScreenTabStateChange -= setScreenTabEnum;
        InventoryManager.OnScreenTabStateChange -= setScreenTabEnum;
    }

    private void Awake()
    {
        if (instance != null)
            instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleBackButtonPressed();
        }
    }

    private void HandleBackButtonPressed()
    {
        switch (_screenTabs)
        {
            case screenTabs.Hometab:
                PromptQuitGame();
                break;
            case screenTabs.Othertabs:
                GoToHomeScreen();
                break;
            case screenTabs.Avatar:
                ExitFromAvatarTab();
                break;
            case screenTabs.Post:
                ExitFromPostTab();
                break;
            case screenTabs.Gameplay:
                ExitFromGamePlay();
                break;
        }
    }

    private void ExitFromGamePlay()
    {
        Debug.Log("Exit from gameplay");
    }

    private void ExitFromAvatarTab()
    {
        InventoryManager.instance.OnClickBackButton();
    }

    private void GoToHomeScreen()
    {
        GameManager.Instance.bottomTabManagerInstance.OnClickHomeButton();
    }

    private void ExitFromPostTab()
    {
        GameManager.Instance.UiManager.ResetPlayerToLastPostPosted();
        GameManager.Instance.UiManager.SwitchToPostScreen(false);

    }

    private void PromptQuitGame()
    {
        exitPanel.SetActive(true);
    }

    public void ConfirmQuitDialog()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }


    public void setScreenTabEnum(screenTabs screenTabs)
    {
        _screenTabs = screenTabs;
    }

}
