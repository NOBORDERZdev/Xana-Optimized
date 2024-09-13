using Firebase.Crashlytics;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;

public class DeeplinkingDomeHandler : MonoBehaviour
{
    private bool FirstTimeopen = true;
    private string EventArguments;


    private void Awake()
    {
        Debug.LogError("------>> DeeplinkingDomeHandler ----- Awake");

        Application.deepLinkActivated += OpenEnvironmentDeeplink;
    }
    private void OnDestroy()
    {
        Application.deepLinkActivated -= OpenEnvironmentDeeplink;
    }
    private void Start()
    {
        Debug.LogError("------>> DeeplinkingDomeHandler ----- Start");

        string validateURL = Application.absoluteURL;

        if (PlayerPrefs.GetInt("PlayerDeepLinkOpened") == 0 && validateURL != "")
        {
            Debug.LogError("------>> DeeplinkingDomeHandler ---- "+ validateURL);

            if (validateURL.Contains("ENV"))
            {
                OpenEnvironmentDeeplink(Application.absoluteURL);
            }
            else if(PlayerPrefs.GetString("DeeplinkDome").Contains("ENV"))
            {
                OpenEnvironmentDeeplink(PlayerPrefs.GetString("DeeplinkDome"));
                PlayerPrefs.SetString("DeeplinkDome", "");
            }
        }

    }
    public void OpenEnvironmentDeeplink(string deeplinkUrl)
    {
        Debug.LogError("------>> DeeplinkingDomeHandler OpenEnvironmentDeeplink----- "+ deeplinkUrl);

        StartCoroutine(ValidateLoginthenDeeplink(deeplinkUrl));
    }
    IEnumerator ValidateLoginthenDeeplink(string deeplinkUrl)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    var intent = activity.Call<AndroidJavaObject>("getIntent");
                    intent.Call("removeExtra", "com.google.firebase.dynamiclinks.DYNAMIC_LINK_DATA");
                    intent.Call("removeExtra", "com.google.android.gms.appinvite.REFERRAL_BUNDLE");
                }
            }
        }
////#if UNITY_IOS
////        while ((!ConstantsHolder.loggedIn || !ConstantsHolder.isWalletLogin) &&
////            (PlayerPrefs.GetString("PlayerName") == "" && PlayerPrefs.GetInt("FirstTimeappOpen") == 0))
////            yield return new WaitForSeconds(0.5f);
////#endif
////#if UNITY_ANDROID
////        while ((!ConstantsHolder.loggedIn || !ConstantsHolder.isWalletLogin) &&
////          (PlayerPrefs.GetString("PlayerName") == ""))
////            yield return new WaitForSeconds(0.5f);
////#endif

        yield return new WaitForSeconds(1.5f);
#if UNITY_ANDROID

        string[] urlBreadDown = deeplinkUrl.Split("=");
        foreach (string word in urlBreadDown)
        {
            if (urlBreadDown[1] == word)
            {
                if (word.Contains("ENV"))
                {
                    EventArguments = word.Replace("ENV", "");
                    if (FirstTimeopen)
                    {
                        FirstTimeopen = false;
                        InvokeDeepLinkEnvironment(EventArguments);
                    }
                }
            }
        }
#endif
#if UNITY_IOS

        if (deeplinkUrl.Contains("ENV"))
        {
            int envIndex = deeplinkUrl.IndexOf("ENV");
            int ampersandIndex = deeplinkUrl.IndexOf("&");

            if (envIndex != -1 && ampersandIndex != -1)
            {
                string envSubstring = deeplinkUrl.Substring(envIndex + 3, ampersandIndex - envIndex - 3);
                if (FirstTimeopen)
                {
                    EventArguments = envSubstring;
                    FirstTimeopen = false;
                    InvokeDeepLinkEnvironment(EventArguments);
                }
            }
        }
#endif
        yield return new WaitForSeconds(1f);
    }
    public void InvokeDeepLinkEnvironment(string environmentIDf)
    {
        Debug.LogError("------>> DeeplinkingDomeHandler InvokeDeepLinkEnvironment----- " + environmentIDf);

       // if (EventArguments == "")
       //     return;

        StartCoroutine( TriggerSceneLoading(int.Parse(environmentIDf)));
    }


    IEnumerator TriggerSceneLoading(int DomeId)
    {
        Debug.LogError("------>> DeeplinkingDomeHandler TriggerSceneLoading ----- " + DomeId);

        yield return new WaitForSeconds(5f);

        Debug.LogError("------>> DeeplinkingDomeHandler TriggerSceneLoading active----- " + DomeId);
        while(LoadingHandler.Instance.loadingPanel.activeInHierarchy)
        {
            yield return new WaitForSeconds(1f);
        }
        Debug.LogError("------>> DeeplinkingDomeHandler TriggerSceneLoading Deactive----- " + DomeId);

        BuilderEventManager.LoadNewScene?.Invoke(DomeId, transform.position);
        LoadingHandler.Instance.EnterDome();
        yield return new WaitForSeconds(1f);
        ReferencesForGamePlay.instance.FullScreenMapStatus(false);
    }

}
