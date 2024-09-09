using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoutManager : MonoBehaviour
{
    public void OnClickLogoutButton()
    {
        UserLoginSignupManager.instance.LogoutAccount();
        PhotonNetwork.Disconnect();
        if (PlayerPrefs.GetInt("ShowLiveUserCounter") == 1)
        {
            Debug.Log("LogoutDone");
        }
        PlayerPrefs.SetInt("shownWelcome", 0);
        PlayerPrefs.SetString("UserNameAndPassword", "");
        GameManager.Instance.SpaceWorldManagerRef.worldSpaceHomeScreenRef.OnLogoutClearSpaceData();
        GlobalVeriableClass.callingScreen = "";
    }
}
