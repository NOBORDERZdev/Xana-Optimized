using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Firebase.DynamicLinks;
using Firebase.Crashlytics;
using Photon.Pun.Demo.PunBasics;
using UnityEditor;

public class DynamicEventManager : Singleton<DynamicEventManager>
{
    #region Variables
    private string EnvironmentURl = "/world/get-world-custom-data/";
    private string EventArguments;
    bool FirstTimeopen = true;
    #endregion

    #region Unity Functions
    private void Awake()
    {
        XanaEventDetails.eventDetails = new XanaEventDetails();
        XanaEventDetails.eventDetails.DataIsInitialized = false;
        Application.deepLinkActivated += OpenEnvironmentDeeplink;
    }
    private void OnDestroy()
    {
        Application.deepLinkActivated -= OpenEnvironmentDeeplink;
    }
    private void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                Firebase.FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;
                Crashlytics.ReportUncaughtExceptionsAsFatal = true;
                Firebase.FirebaseApp.LogLevel = Firebase.LogLevel.Debug;
                ConstantsHolder.xanaConstants.isFirebaseInit = true;
                string validateURL = Application.absoluteURL;
                if (PlayerPrefs.GetInt("PlayerDeepLinkOpened") == 0 && validateURL != "")
                {
                   if(validateURL.Contains("ENV"))
                   {
                        OpenEnvironmentDeeplink(Application.absoluteURL);
                   }
                }

            }
            else
            {
                UnityEngine.Debug.Log(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        });
    }
    public void OpenEnvironmentDeeplink(string deeplinkUrl)
    {
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
#if UNITY_IOS
        while ((!ConstantsHolder.loggedIn || !ConstantsHolder.isWalletLogin) &&
            (PlayerPrefs.GetString("PlayerName") == "" && PlayerPrefs.GetInt("FirstTimeappOpen") == 0))
            yield return new WaitForSeconds(0.5f);
#endif
#if UNITY_ANDROID
        while ((!ConstantsHolder.loggedIn || !ConstantsHolder.isWalletLogin) &&
          (PlayerPrefs.GetString("PlayerName") == ""))
            yield return new WaitForSeconds(0.5f);
#endif


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
        if (EventArguments == "")
            return;

        StartCoroutine(HitGetEnvironmentJson(ConstantsGod.API_BASEURL + EnvironmentURl + environmentIDf, environmentIDf));
    }

    IEnumerator HitGetEnvironmentJson(string url, string envId)
    {
        //Debug.LogError(url + " ----- Environment Jump ---- " + envId);

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            yield return request.SendWebRequest();
            EnvironmentDetails environmentDetails = JsonUtility.FromJson<EnvironmentDetails>(request.downloadHandler.text);
            ConstantsHolder.xanaConstants.MuseumID = envId;

            if (!request.isHttpError && !request.isNetworkError)
            {
                if (request.error == null)
                {
                    if (environmentDetails.success == true)
                    {
                        ConstantsHolder.xanaConstants.MuseumID = envId;
                        yield return new WaitForSeconds(4f);
                        Screen.orientation = ScreenOrientation.LandscapeLeft;
                        PlayerPrefs.SetInt("PlayerDeepLinkOpened", 1);
                        bool isBuilderScene = false;
                        bool isMuseumScene = false;

                        if (environmentDetails.data.entityType == WorldType.MUSEUM.ToString())
                            isMuseumScene = true;
                        else if (environmentDetails.data.entityType == WorldType.USER_WORLD.ToString())
                        {
                            isBuilderScene = true;
                            isMuseumScene = true;
                        }
                        if (name == "Xana Festival")
                            ConstantsHolder.userLimit = 10;
                        else
                            ConstantsHolder.userLimit = 10;

                        ConstantsHolder.xanaConstants.builderMapID = int.Parse(envId);
                        ConstantsHolder.xanaConstants.IsMuseum = isMuseumScene;
                        ConstantsHolder.xanaConstants.isBuilderScene = isBuilderScene;
                        if (ConstantsHolder.xanaConstants.EnviornmentName.Contains("RooftopParty") || ConstantsHolder.xanaConstants.EnviornmentName.Contains("XanaParty"))
                        {
                            ConstantsHolder.xanaConstants.isXanaPartyWorld = true;
                        }
                        else
                        {
                            ConstantsHolder.xanaConstants.isXanaPartyWorld = false;
                        }
                        WorldItemView.m_EnvName = environmentDetails.data.name;

                        if (isBuilderScene)
                            WorldManager.instance.JoinBuilderWorld();
                        else
                            WorldManager.instance.JoinEvent();

                    }
                }
            }
            else
            {
       // Debug.LogError(" ----- Environment Jump ---- " + request.error);

                if (request.Equals(UnityWebRequest.Result.ConnectionError))
                {
                    yield return StartCoroutine(HitGetEnvironmentJson(url, envId));
                }
                else
                {
                    if (request.error != null)
                    {
                        if (environmentDetails.success == false)
                        {
                            yield return StartCoroutine(HitGetEnvironmentJson(url, envId));
                        }
                    }
                }
            }
            request.Dispose();
        }
    }
    #endregion
}
#if UNITY_EDITOR

[CustomEditor(typeof(DynamicEventManager))]
public class EditorTestDeeplinking : Editor
{
    public int EnvironmentID = default;

    public override void OnInspectorGUI()
    {
        EnvironmentID = EditorGUILayout.IntField("EnvironmentID", EnvironmentID);

        if (GUILayout.Button("Enter World"))
        {
            if (EnvironmentID > 0)
            {
                Debug.LogError(" ----- Environment Jump ---- " + EnvironmentID);
                DynamicEventManager.Instance.InvokeDeepLinkEnvironment("" + EnvironmentID);
            }
        }
    }
}
#endif