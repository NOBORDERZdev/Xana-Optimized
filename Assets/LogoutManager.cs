using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoutManager : MonoBehaviour
{
    public GameObject logoutPanel;
    public GameObject profileButton;

    private void Start()
    {
        if(SceneManager.GetSceneByName("Builder").isLoaded)
        {
            profileButton.SetActive(false);
        }
        else
        {
            profileButton.SetActive(true);
        }
    }

    public void OnClickLogoutButton()
    {
        UserLoginSignupManager.instance.LogoutAccount();
        PhotonNetwork.Disconnect();
        if (PlayerPrefs.GetInt("ShowLiveUserCounter") == 1)
        {
            Debug.Log("LogoutDone");
            logoutPanel.SetActive(false);
        }
        PlayerPrefs.SetInt("shownWelcome", 0);
        PlayerPrefs.SetString("UserNameAndPassword", "");
        GameManager.Instance.SpaceWorldManagerRef.worldSpaceHomeScreenRef.OnLogoutClearSpaceData();
        GlobalVeriableClass.callingScreen = "";
    }

    public void DeleteAccount()
    {
        UserLoginSignupManager.instance.DeleteAccount(() =>
        {
            Debug.Log("Account Deleted");
            PhotonNetwork.Disconnect();
        });
    }
}
