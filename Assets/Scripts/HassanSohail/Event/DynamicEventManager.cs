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
    bool Debugging = true;
    bool isOnAppLunached;
    string DebugDeepLink = "https://unitytesting.page.link/?link=https://www.xana.net/Join%3D3755&apn=com.nbi.xana&isi=6642649722&ibi=com.wujie.xsummit";
    static DynamicEventManager Instance;
    #endregion

    #region Unity Functions

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
        SaveCharacterProperties.NeedToShowSplash = 1;
        print("Set need to show 1 in dynamic event manager");
        XanaEventDetails.eventDetails = new XanaEventDetails();
        XanaEventDetails.eventDetails.DataIsInitialized = false;
        // Application.deepLinkActivated += OpenEnvironmentDeeplink;
    }

    private void OnDestroy()
    {
        // Application.deepLinkActivated -= OpenEnvironmentDeeplink;
    }

    private void Start()
    {

        Debug.Log("DynamicEventManager: Start called");
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                Debug.Log("Firebase dependencies are available");
                Firebase.FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;
                Crashlytics.ReportUncaughtExceptionsAsFatal = true;
                Firebase.FirebaseApp.LogLevel = Firebase.LogLevel.Debug;
                ConstantsHolder.xanaConstants.isFirebaseInit = true;
                string validateURL = Application.absoluteURL;
                if (PlayerPrefs.GetInt("PlayerDeepLinkOpened") == 0 && validateURL != "")
                {
                    Debug.Log("PlayerDeepLinkOpened is 0 and URL is not empty");
                    if (validateURL.Contains("ENV"))
                    {
                        Debug.Log("Detected ENV in URL");
                        ConstantsHolder.xanaConstants.isSummitDeepLink = true;
                        ConstantsHolder.xanaConstants.isJoiningXANADeeplink = false;
                        //OpenEnvironmentDeeplink(Application.absoluteURL);
                    }
                    else if (validateURL.Contains("Join"))
                    {
                        Debug.Log("Detected Join in URL");
                        ConstantsHolder.xanaConstants.isSummitDeepLink = false;
                        ConstantsHolder.xanaConstants.isJoiningXANADeeplink = true;
                        PlayerPrefs.SetInt("FirstTimeappOpen", 0);

                        XANADeeplink(Application.absoluteURL);
                    }
                }
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
            }
        });

    }

    public void XANADeeplink(string deeplinkUrl)
    {
        Debug.Log($"XANADeeplink called with URL: {deeplinkUrl}");
        StartCoroutine(ValidateLoginthenDeeplink(deeplinkUrl));
    }

    IEnumerator ValidateLoginthenDeeplink(string deeplinkUrl)
    {
        // Decode the URL to handle URL-encoded characters
        string decodedUrl = UnityWebRequest.UnEscapeURL(deeplinkUrl);
        Debug.Log($"Decoded URL: {decodedUrl}");

        if (Application.platform == RuntimePlatform.Android)
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    var intent = activity.Call<AndroidJavaObject>("getIntent");
                    intent.Call("removeExtra", "com.google.firebase.dynamiclinks.DYNAMIC_LINK_DATA");
                    intent.Call("removeExtra", "com.google.android.gms.appinvite.REFERRAL_BUNDLE");
                    Debug.Log("Removed extras from Android intent");
                }
            }
        }

#if UNITY_IOS
    while ((((!ConstantsHolder.loggedIn || (!ConstantsHolder.xanaConstants.LoggedInAsGuest)) &&
           (PlayerPrefs.GetString("PlayerName") == "")) && PlayerPrefs.GetInt("FirstTimeappOpen") == 0))
    {
        Debug.Log("Waiting for login on IOS : loggedIn : " + ConstantsHolder.loggedIn + " :  LoggedInAsGuest " +
        ConstantsHolder.xanaConstants.LoggedInAsGuest + " : PlayerName  " + PlayerPrefs.GetString("PlayerName") + " : FirstTimeappOpen : " + PlayerPrefs.GetInt("FirstTimeappOpen"));
        yield return new WaitForSeconds(0.5f);
    }
#endif

#if UNITY_ANDROID
        while ((((!ConstantsHolder.loggedIn || (!ConstantsHolder.xanaConstants.LoggedInAsGuest)) &&
            (PlayerPrefs.GetString("PlayerName") == ""))))
        {
            yield return new WaitForSeconds(0.5f);
        }
#endif

        yield return new WaitForSeconds(1.5f);

#if UNITY_ANDROID
        // Extract the number after "Join=" and before "&"
        string joinKey = "Join=";
        int startIndex = decodedUrl.IndexOf(joinKey) + joinKey.Length;
        int endIndex = decodedUrl.IndexOf("&", startIndex);

        if (startIndex != -1)
        {
            if (endIndex != -1)
            {
                EventArguments = decodedUrl.Substring(startIndex, endIndex - startIndex);
            }
            else
            {
                EventArguments = decodedUrl.Substring(startIndex);
            }
            Debug.Log($"EventArguments set to: {EventArguments}");
            if (FirstTimeopen)
            {
                FirstTimeopen = false;
                Debug.Log($"Invoking deep link environment with arguments: {EventArguments}");
                InvokeDeepLinkEnvironment(EventArguments);
            }
        }
#endif

#if UNITY_IOS
    if (decodedUrl.Contains("Join"))
    {
        int envIndex = decodedUrl.IndexOf("Join");
        int ampersandIndex = decodedUrl.IndexOf("&", envIndex);

        if (envIndex != -1)
        {
            string envSubstring;
            if (ampersandIndex != -1)
            {
                envSubstring = decodedUrl.Substring(envIndex + 4, ampersandIndex - envIndex - 4);
            }
            else
            {
                envSubstring = decodedUrl.Substring(envIndex + 4);
            }
            envSubstring = envSubstring.Replace("=", "");
            if (FirstTimeopen)
            {
                EventArguments = envSubstring;
                FirstTimeopen = false;
                Debug.Log($"Invoking deep link environment with arguments: {EventArguments}");
                InvokeDeepLinkEnvironment(EventArguments);
            }
        }
    }
#endif

        yield return new WaitForSeconds(1f);
    }


    public void InvokeDeepLinkEnvironment(string environmentIDf)
    {
       // Debug.Log($"InvokeDeepLinkEnvironment called with ID: {environmentIDf}");
        if (EventArguments == "")
        {
         //   Debug.LogWarning("EventArguments is empty, returning");
            return;
        }

        StartCoroutine(HitGetEnvironmentJson(ConstantsGod.API_BASEURL + EnvironmentURl + environmentIDf, environmentIDf));
    }

    IEnumerator HitGetEnvironmentJson(string url, string envId)
    {
        //Debug.Log($"HitGetEnvironmentJson called with URL: {url} and envId: {envId}");

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            yield return request.SendWebRequest();
            EnvironmentDetails environmentDetails = JsonUtility.FromJson<EnvironmentDetails>(request.downloadHandler.text);
            Debug.Log("World Data : "+ request.downloadHandler.text);
            ConstantsHolder.xanaConstants.MuseumID = envId;
            LoadingHandler.Instance.ShowLoading();
            LoadingHandler.Instance.UpdateLoadingSlider(0);
            LoadingHandler.Instance.UpdateLoadingStatusText("Loading World");
            Screen.orientation = ScreenOrientation.LandscapeLeft;

            if (!request.isHttpError && !request.isNetworkError)
            { 
                if (request.error == null)
                {
                    if (environmentDetails.success == true)
                    {
                       
                        Debug.Log("Environment details successfully retrieved");
                        ConstantsHolder.xanaConstants.MuseumID = envId;
                        yield return new WaitForSeconds(4f);
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
                        LoadingHandler.Instance.ShowLoading();
                        LoadingHandler.Instance.UpdateLoadingSlider(0);
                        LoadingHandler.Instance.UpdateLoadingStatusText("Loading World");
                        Screen.orientation = ScreenOrientation.LandscapeLeft;
                        WorldItemView.m_EnvName = environmentDetails.data.name;
                        SaveCharacterProperties.NeedToShowSplash = 2;
                        PlayerPrefs.SetInt("FirstTimeappOpen", 1);
                        if (isBuilderScene)
                            WorldManager.instance.JoinBuilderWorld();
                        else
                            WorldManager.instance.JoinEvent();

                    }
                }
            }
            else
            {
                Debug.LogError($"Error retrieving environment details: {request.error}");

                if (request.Equals(UnityWebRequest.Result.ConnectionError))
                {
                    Debug.LogWarning("Connection error, retrying...");
                    yield return StartCoroutine(HitGetEnvironmentJson(url, envId));
                }
                else
                {
                    if (request.error != null)
                    {
                        if (environmentDetails.success == false)
                        {
                            Debug.LogWarning("Environment details retrieval failed, retrying...");
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
    public string DeepLink = "";

    public override void OnInspectorGUI()
    {
        DeepLink = EditorGUILayout.TextField("DeepLink", DeepLink);

        if (GUILayout.Button("Enter World"))
        {
            if (!DeepLink.IsNullOrEmpty())
            {
                ConstantsHolder.xanaConstants.isSummitDeepLink = false;
                ConstantsHolder.xanaConstants.isJoiningXANADeeplink = true;
                Debug.Log($"EditorTestDeeplinking: {DeepLink}");
                DynamicEventManager.Instance.XANADeeplink(DeepLink);
            }
        }
    }
}
#endif