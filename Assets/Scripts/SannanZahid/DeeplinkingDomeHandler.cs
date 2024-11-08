using System.Collections;
using UnityEngine;

public class DeeplinkingDomeHandler : MonoBehaviour
{
//    private bool _firstTimeopen = true;
//    private string _eventArguments;

//    private void Awake()
//    {
//        Application.deepLinkActivated += OpenEnvironmentDeeplink;
//    }
//    private void OnDestroy()
//    {
//        Application.deepLinkActivated -= OpenEnvironmentDeeplink;
//    }
//    private void Start()
//    {
//        string validateURL = Application.absoluteURL;

//        if (PlayerPrefs.GetInt("PlayerDeepLinkOpened") == 0 && validateURL != "")
//        {
//            if (validateURL.Contains("ENV"))
//            {
//                OpenEnvironmentDeeplink(Application.absoluteURL);
//            }
//            else if(PlayerPrefs.GetString("DeeplinkDome").Contains("ENV"))
//            {
//                OpenEnvironmentDeeplink(PlayerPrefs.GetString("DeeplinkDome"));
//                PlayerPrefs.SetString("DeeplinkDome", "");
//            }
//        }
//    }
//    public void OpenEnvironmentDeeplink(string deeplinkUrl)
//    {
//        StartCoroutine(ValidateLoginthenDeeplink(deeplinkUrl));
//    }
//    IEnumerator ValidateLoginthenDeeplink(string deeplinkUrl)
//    {
//        if (Application.platform == RuntimePlatform.Android)
//        {
//            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
//            {
//                using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
//                {
//                    var intent = activity.Call<AndroidJavaObject>("getIntent");
//                    intent.Call("removeExtra", "com.google.firebase.dynamiclinks.DYNAMIC_LINK_DATA");
//                    intent.Call("removeExtra", "com.google.android.gms.appinvite.REFERRAL_BUNDLE");
//                }
//            }
//        }

//        yield return new WaitForSeconds(1.5f);
//#if UNITY_ANDROID

//        string[] urlBreadDown = deeplinkUrl.Split("=");
//        foreach (string word in urlBreadDown)
//        {
//            if (urlBreadDown[1] == word)
//            {
//                if (word.Contains("ENV"))
//                {
//                    _eventArguments = word.Replace("ENV", "");
//                    if (_firstTimeopen)
//                    {
//                        _firstTimeopen = false;
//                        InvokeDeepLinkEnvironment(_eventArguments);
//                    }
//                }
//            }
//        }
//#endif
//#if UNITY_IOS

//        if (deeplinkUrl.Contains("ENV"))
//        {
//            int envIndex = deeplinkUrl.IndexOf("ENV");
//            int ampersandIndex = deeplinkUrl.IndexOf("&");

//            if (envIndex != -1 && ampersandIndex != -1)
//            {
//                string envSubstring = deeplinkUrl.Substring(envIndex + 3, ampersandIndex - envIndex - 3);
//                if (_firstTimeopen)
//                {
//                    _eventArguments = envSubstring;
//                    _firstTimeopen = false;
//                    InvokeDeepLinkEnvironment(_eventArguments);
//                }
//            }
//        }
//#endif
//        yield return new WaitForSeconds(1f);
//    }
//    public void InvokeDeepLinkEnvironment(string environmentIDf)
//    {
//        StartCoroutine( TriggerSceneLoading(int.Parse(environmentIDf)));
//    }
//    IEnumerator TriggerSceneLoading(int DomeId)
//    {
//        yield return new WaitForSeconds(5f);

//        while(LoadingHandler.Instance.loadingPanel.activeInHierarchy)
//        {
//            yield return new WaitForSeconds(1f);
//        }
//        SaveCharacterProperties.NeedToShowSplash = 2;
//        BuilderEventManager.LoadNewScene?.Invoke(DomeId, transform.position);
//        LoadingHandler.Instance.EnterDome();
//        yield return new WaitForSeconds(1f);
//        ReferencesForGamePlay.instance.FullScreenMapStatus(false);
//    }
}