using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
public class LoadingSavingAvatar : MonoBehaviour
{
    //public Image LoadingImg;
    public GameObject tabBG;
    private bool Once;
    private float time;
    public float TotalTimer;
    public TextMeshProUGUI versionText;
    public static string version = "";

    // Start is called before the first frame update
    void Start()
    {
        Once = false;
        versionText.text = "Ver." + Application.version;
        if (XanaConstants.xanaConstants.screenType==XanaConstants.ScreenType.TabScreen)
        {
            tabBG.SetActive(true);
        }
    }

    public void CallWelcome()
    {
        if (UserRegisterationManager.instance != null)
            UserRegisterationManager.instance.ShowWelcomeScreen();
    }

    public IEnumerator getVersion()
    {

        UnityWebRequest uwr = UnityWebRequest.Get(ConstantsGod.API_BASEURL + ConstantsGod.GetVersion);

        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            try
            {
                VersionDetails bean = JsonUtility.FromJson<VersionDetails>(uwr.downloadHandler.text.ToString().Trim());
                if (bean.success)
                {
                    version = bean.data.name;
                }
            }
            catch
            {

            }
        }
    }
    [System.Serializable]
    public class VersionData
    {
        public int id;
        public string name;
        public DateTime createdAt;
        public DateTime updatedAt;
    }
    [System.Serializable]
    public class VersionDetails
    {
        public bool success;
        public VersionData data;
        public string msg;
    }

}
