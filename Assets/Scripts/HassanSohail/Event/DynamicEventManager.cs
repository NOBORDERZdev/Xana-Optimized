using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using Firebase.DynamicLinks;
using Firebase.Crashlytics;


public class DynamicEventManager : Singleton<DynamicEventManager>
{
    #region Variables

    //DateTime eventLocalStartDateTime, eventlocalEndDateTime, eventUnivStartDateTime, eventUnivEndDateTime;
   // public string[] eventStartDateTime;
   // public string[] eventEndDateTime;
   // private string EventURl = "/userCustomEvent/get-event-json/";
    private string EnvironmentURl = "/world/get-world-custom-data/";
    /// Testnet https://api-test.xana.net/world/get-world-custom-data/:worldId
    // TestEvent  "https://api-test.xana.net/userCustomEvent/get-event-json/"
    //MainEvent https://app-api.xana.net/userCustomEvent/get-event-json/45
    //    ConstantsGod.API_BASEURL = "https://app-api.xana.net";
    //  ConstantsGod.API_BASEURL = "https://api-test.xana.net";
    private string EventArguments;
   // private int PauseCount;
   // private int StartFocusCounter;
    bool FirstTimeopen = true;
    #endregion

    #region Unity Functions

    //private void Awake()
    //{
    //   // XanaEventDetails.eventDetails = new XanaEventDetails();
    //   // XanaEventDetails.eventDetails.DataIsInitialized = false;
    //   // StartCoroutine(HitGetEnvironmentJson("https://api-test.xana.net" + EnvironmentURl + 406, "406"));
    //}
    private void Awake()
    {
        Application.deepLinkActivated += OpenEnvironmentDeeplink;
    }
    private void OnDestroy()
    {
        Application.deepLinkActivated -= OpenEnvironmentDeeplink;
    }
    private void Start()
    {
        //EventArguments = "";
        //PauseCount = 0;
        //StartFocusCounter = 1;
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                Firebase.FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;
                Crashlytics.ReportUncaughtExceptionsAsFatal = true;
                Firebase.FirebaseApp.LogLevel = Firebase.LogLevel.Debug;
               // BindAfterInitilization();
                ConstantsHolder.xanaConstants.isFirebaseInit = true;
                // InvokeDeepLink("focus");
                OpenEnvironmentDeeplink(Application.absoluteURL);
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
        Debug.LogError("ValidateLoginthenDeeplink ----> " + deeplinkUrl);
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
        Debug.LogError("Validate---Before ----> ");
      
        Debug.LogError("Validate---Before ----> "+ ConstantsHolder.loggedIn + " ---- " + ConstantsHolder.isWalletLogin + "  ----- "+ PlayerPrefs.GetString("PlayerName"));

        while (!ConstantsHolder.loggedIn || !ConstantsHolder.isWalletLogin)//PlayerPrefs.GetInt("IsLoggedIn") == 0 && PlayerPrefs.GetInt("shownWelcome") == 0 && PlayerPrefs.GetString("PlayerName").IsNullOrEmpty())
            yield return new WaitForSeconds(0.5f);

        Debug.LogError("Validate---After 007 ----> " + ConstantsHolder.loggedIn + " ---- " + ConstantsHolder.isWalletLogin + "  ----- " + PlayerPrefs.GetString("PlayerName"));

        yield return new WaitForSeconds(1.5f);
        Debug.LogError("Validate--- After ----> ");
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
                        Debug.LogError("ValidateLoginthenDeeplink ID ----> " + EventArguments);
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
                // Extract the substring between "ENV" and "&"
                string envSubstring = deeplinkUrl.Substring(envIndex + 3, ampersandIndex - envIndex - 3);

                if (FirstTimeopen)
                {
                    EventArguments = envSubstring;
                    FirstTimeopen = false;
                    Debug.LogError("IOS ---> ValidateLoginthenDeeplink ID ----> " + EventArguments);
                    InvokeDeepLinkEnvironment(EventArguments);
                }
            }
            else
            {
                Debug.LogError("IOS ----  ValidateLoginthenDeeplink ID ----> " + EventArguments);

            }
        }
#endif

        yield return new WaitForSeconds(1f);
    }
    public void InvokeDeepLinkEnvironment(string environmentIDf)
    {
        Debug.LogError("InvokeDeepLinkEnvironment ID ----> " + environmentIDf);
        if (EventArguments == "")
            return;

        Debug.LogError("InvokeDeepLinkEnvironment ID ----> " + (ConstantsGod.API_BASEURL + EnvironmentURl + environmentIDf));

        StartCoroutine(HitGetEnvironmentJson(ConstantsGod.API_BASEURL + EnvironmentURl + environmentIDf, environmentIDf));
    }
    IEnumerator HitGetEnvironmentJson(string url, string envId)
    {
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
                    Debug.LogError("HitGetEnvironmentJson ID --Successss--> " + envId);

                    if (environmentDetails.success == true)
                    {
                        Debug.LogError("HitGetEnvironmentJson ID Sucesss 3 ----> " + envId);

                        yield return new WaitForSeconds(4f);
                        LoadingHandler.Instance.ShowLoading();
                        Screen.orientation = ScreenOrientation.LandscapeLeft;
                        yield return new WaitForSeconds(3f);
                        if (environmentDetails.data.name.Contains("Xana Festival"))
                        {
                            ConstantsHolder.xanaConstants.userLimit = "16";
                        }
                        else
                        {
                            ConstantsHolder.xanaConstants.userLimit = "15";
                        }
                        //Set These Settings after loading Json Data
                        if (environmentDetails.data.entityType.Equals("XANA_WORLD"))
                        {
                            WorldItemView.m_EnvName = "Builder";
                            ConstantsHolder.xanaConstants.builderMapID = int.Parse(envId);
                            ConstantsHolder.xanaConstants.isBuilderScene = true;
                            LoadingHandler.Instance.worldLoadingScreen.SetActive(false);
                            LoadingHandler.Instance.LoadSceneByIndex("Builder", true);
                        }
                        else
                        {
                            if (!ConstantsHolder.xanaConstants.JjWorldSceneChange && !ConstantsHolder.xanaConstants.orientationchanged)
                                Screen.orientation = ScreenOrientation.LandscapeLeft;

                            ConstantsHolder.xanaConstants.EnviornmentName = environmentDetails.data.name;
                            WorldItemView.m_EnvName = environmentDetails.data.name;
                            LoadingHandler.Instance.worldLoadingScreen.SetActive(false);
                            SceneManager.LoadScene("GamePlayScene");
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("HitGetEnvironmentJson ID ----> " + envId);

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
    /*  public void BindAfterInitilization()
      {
          DynamicLinks.DynamicLinkReceived += OnDynamicLink;
          deepLink += InvokeDeepLink;
      }
      private void OnDisable()
      {
          deepLink -= InvokeDeepLink;
      }
      private void OnApplicationPause(bool pause)
      {
          if (pause)
          {
              EventArguments = "";
              PauseCount += 1;
          }
      }
      private void OnApplicationFocus(bool focus)
      {

          if (focus && PauseCount > 0)
          {
              StartFocusCounter = 2;
              DynamicLinks.DynamicLinkReceived += OnDynamicLink;
          }
      }
    #endregion

    IEnumerator ValidateLoginthenDeeplink(object sender, EventArgs args)
    {
        var dynamicLinkEventArgs = args as ReceivedDynamicLinkEventArgs;
        Debug.LogFormat("Received dynamic link {0}",
                        dynamicLinkEventArgs.ReceivedDynamicLink.Url.OriginalString);
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

        string[] vec2 = dynamicLinkEventArgs.ReceivedDynamicLink.Url.OriginalString.Split("=");
        foreach (string word in vec2)
        {
            if (vec2[1] == word)
            {
                if (word.Contains("ENV"))
                {
                    EventArguments = word.Replace("ENV", "");
                    DynamicLinks.DynamicLinkReceived += OnDynamicLinkEmpty;
                    if (StartFocusCounter == 2 && (PlayerPrefs.GetInt("shownWelcome") == 1 || PlayerPrefs.GetInt("IsLoggedIn") == 1))
                    {
                        InvokeDeepLinkEnvironment(EventArguments);
                        StartFocusCounter = 0;
                    }
                    else if (StartFocusCounter == 1)
                    {
                        if ((PlayerPrefs.GetInt("shownWelcome") == 1 || PlayerPrefs.GetInt("IsLoggedIn") == 1) && FirstTimeopen)
                        {
                            FirstTimeopen = false;
                            InvokeDeepLinkEnvironment(EventArguments);
                        }
                        StartFocusCounter = 0;
                    }
                }
                else
                {
                    EventArguments = word;
                    DynamicLinks.DynamicLinkReceived += OnDynamicLinkEmpty;
                    if (StartFocusCounter == 2 && (PlayerPrefs.GetInt("shownWelcome") == 1 || PlayerPrefs.GetInt("IsLoggedIn") == 1))
                    {
                        InvokeDeepLink("focus");
                        StartFocusCounter = 0;
                    }
                    else if (StartFocusCounter == 1)
                    {
                        if ((PlayerPrefs.GetInt("shownWelcome") == 1 || PlayerPrefs.GetInt("IsLoggedIn") == 1) && FirstTimeopen)
                        {
                            FirstTimeopen = false;
                            InvokeDeepLink("focus");
                        }
                        StartFocusCounter = 0;
                    }
                }

            }
        }
        yield return new WaitForSeconds(1f);

    }
    
    #region Custom Functions
    private void OnDynamicLink(object sender, EventArgs args)
    {
        if (StartFocusCounter == 0 && ConstantsHolder.xanaConstants.isFirebaseInit)
        {
            return;
        }
        StartCoroutine(ValidateLoginthenDeeplink(sender, args));
    }

    private void OnDynamicLinkEmpty(object sender, EventArgs args)
    {

    }

    public void InvokeDeepLink(string _ArgData)
    {
        if (EventArguments == "")
            return;

        StartCoroutine(HitGetEventJson(ConstantsGod.API_BASEURL + EventURl + EventArguments));
    }
    public void InvokeDeepLinkEnvironment(string environmentIDf)
    {
        if (EventArguments == "")
            return;

        StartCoroutine(HitGetEnvironmentJson(ConstantsGod.API_BASEURL + EnvironmentURl + environmentIDf,environmentIDf));
    }
    IEnumerator HitGetEventJson(string url)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            yield return request.SendWebRequest();
            EventDataDetails eventDetails = JsonUtility.FromJson<EventDataDetails>(request.downloadHandler.text);
            XanaEventDetails.eventDetails = eventDetails.data;

            if (!string.IsNullOrEmpty(eventDetails.data.xana_world_id))
            {
                ConstantsHolder.xanaConstants.MuseumID = eventDetails.data.xana_world_id;
            }
            else if (eventDetails.data.environmentId != 0)
            {
                ConstantsHolder.xanaConstants.MuseumID = eventDetails.data.environmentId.ToString();
            }
            else if (eventDetails.data.museumId != 0)
            {
                ConstantsHolder.xanaConstants.MuseumID = eventDetails.data.xana_world_id;
            }


            if (!request.isHttpError && !request.isNetworkError)
            {
                if (request.error == null)
                {
                    if (eventDetails.success == true)
                    {
                        XanaEventDetails.eventDetails.DataIsInitialized = true;
                        yield return new WaitForEndOfFrame();

                        StartCoroutine(DelayLoadRemainingSceneData());
                        LoadingHandler.Instance.EventLoaderCanvas.SetActive(true);
                    }
                }
            }

            else
            {
                if (request.Equals(UnityWebRequest.Result.ConnectionError))
                {
                    LoadingHandler.Instance.EventLoaderCanvas.SetActive(false);
                    yield return StartCoroutine(HitGetEventJson(url));
                }
                else
                {
                    LoadingHandler.Instance.EventLoaderCanvas.SetActive(false);
                    if (request.error != null)
                    {
                        if (eventDetails.success == false)
                        {
                            yield return StartCoroutine(HitGetEventJson(url));
                        }
                    }
             
                }
            }
            request.Dispose();
        }
    }
    

    public void GetEventJsonData(string _jsonData)
    {
        XanaEventDetails JsonDataObj1 = JsonUtility.FromJson<XanaEventDetails>(_jsonData);
        XanaEventDetails.eventDetails = JsonDataObj1;

        EventUserRoles JsonDataObj2 = JsonUtility.FromJson<EventUserRoles>(_jsonData);
        XanaEventDetails.eventDetails.eventsUserRoles.Add(JsonDataObj2);
    }
    //Checking event Date Time if its ended or not conditions
    public void CheckEventDateTime()
    {
        //Getting Event and System Date Time
        if (XanaEventDetails.eventDetails.recurrence)
        {
            for (int i = 0; i < XanaEventDetails.eventDetails.recurrence_dates.Length; i++)
            {
                if (ConvertStringToDateFormate(XanaEventDetails.eventDetails.recurrence_dates[i]))
                {
                    XanaEventDetails.eventDetails.startTime = XanaEventDetails.eventDetails.recurrence_dates[i];
                    break;
                }
            }
        }
        else
        {
            ConvertStringToDateFormate(XanaEventDetails.eventDetails.startTime);
        }

        int _eventStartSystemDateTimediff = (int)(eventLocalStartDateTime - System.DateTime.Now).TotalMinutes;
        int _eventEndSystemDateTimediff = (int)(eventlocalEndDateTime - System.DateTime.Now).TotalMinutes;

        TimeSpan t = TimeSpan.FromMinutes(_eventStartSystemDateTimediff);
        string dayTimeFormat = string.Format("{0:D2}d:{1:D2}h:{2:D2}m",
                      t.Days,
                      t.Hours,
                      t.Minutes);
        string hourTimeFormat = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
              t.Hours,
              t.Minutes,
              t.Seconds);

        if (_eventStartSystemDateTimediff > 0)
        {
            if (TimeSpan.FromMinutes(_eventStartSystemDateTimediff).Days > 0)
            {
                XanaEventDetails.eventDetails.DataIsInitialized = false;
                SetEventPopUpDialog("Not Event Day yet", "Will Start After:", dayTimeFormat, true);
            }
            else
            {
                if (_eventStartSystemDateTimediff > 0 && _eventStartSystemDateTimediff <= 30)
                {
                    StartCoroutine(DelayLoadRemainingSceneData());
                    LoadingHandler.Instance.EventLoaderCanvas.SetActive(true);
                }
                else
                {
                    XanaEventDetails.eventDetails.DataIsInitialized = false;
                    SetEventPopUpDialog("Event Time not started yet", "Will Start After:", hourTimeFormat, true);
                }
            }
        }
        if (_eventStartSystemDateTimediff <= 0 && _eventEndSystemDateTimediff >= 0)
        {
            StartCoroutine(DelayLoadRemainingSceneData());
            LoadingHandler.Instance.EventLoaderCanvas.SetActive(true);
        }
        if (_eventEndSystemDateTimediff < 0)
        {
            XanaEventDetails.eventDetails.DataIsInitialized = false;
            SetEventPopUpDialog("Event is Ended", "", "", true);
        }
    }
    public bool ConvertStringToDateFormate(string _eventDatetime)
    {
        eventUnivStartDateTime = DateTime.Parse(_eventDatetime);
        eventLocalStartDateTime = eventUnivStartDateTime.ToLocalTime();
        eventlocalEndDateTime = eventLocalStartDateTime.Add(TimeSpan.FromSeconds(XanaEventDetails.eventDetails.duration));
        if (System.DateTime.Now.Date.Equals(eventLocalStartDateTime.Date))
        {
            if (System.DateTime.Now.Date >= eventLocalStartDateTime.Date && System.DateTime.Now.Date <= eventlocalEndDateTime.Date)
            {
                XanaEventDetails.eventDetails.startTime = _eventDatetime;
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    IEnumerator DelayLoadRemainingSceneData()
    {
        yield return new WaitForSeconds(4f);
        LoadingHandler.Instance.EventLoaderCanvas.SetActive(false);
        LoadingHandler.Instance.ShowLoading();
        StartCoroutine(LoadingHandler.Instance.IncrementSliderValue(UnityEngine.Random.Range(6f, 10f)));
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        yield return new WaitForSeconds(4f);
        SetSceneData();
    }

    //Loading scene for event after checking date time and event data
    public void SetSceneData()
    {
        if (XanaEventDetails.eventDetails.environmentName.Contains("Xana Festival"))
        {
            ConstantsHolder.xanaConstants.userLimit = "16";
        }
        else
        {
            ConstantsHolder.xanaConstants.userLimit = "15";
        }
        //Set These Settings after loading Json Data
        if (XanaEventDetails.eventDetails.eventType.Equals("XANA_WORLD"))
        {
            WorldItemView.m_EnvName = "Builder";
            ConstantsHolder.xanaConstants.builderMapID = int.Parse(XanaEventDetails.eventDetails.xana_world_id);
            ConstantsHolder.xanaConstants.isBuilderScene = true;
            LoadingHandler.Instance.worldLoadingScreen.SetActive(false);
            LoadingHandler.Instance.LoadSceneByIndex("Builder", true);
        }
        else
        {
            if (!ConstantsHolder.xanaConstants.JjWorldSceneChange && !ConstantsHolder.xanaConstants.orientationchanged)
            {
                Screen.orientation = ScreenOrientation.LandscapeLeft;
            }
            if (XanaEventDetails.eventDetails.museumId.Equals(0))
            {
                ConstantsHolder.xanaConstants.EnviornmentName = XanaEventDetails.eventDetails.environmentName;
                WorldItemView.m_EnvName = XanaEventDetails.eventDetails.environmentName;
            }
            else
            {
                ConstantsHolder.xanaConstants.EnviornmentName = XanaEventDetails.eventDetails.museumName;
                WorldItemView.m_EnvName = XanaEventDetails.eventDetails.museumName;
            }
            LoadingHandler.Instance.worldLoadingScreen.SetActive(false); 
            SceneManager.LoadScene("GamePlayScene");
        }
    }
    //UI related calls
    public void SetEventPopUpDialog(string _headingtext = "", string _descriptiontext = "", string _timertext = "", bool _panelstate = false)
    {
        PopupMessageHandler.Instance.SetText(PopupMessageHandler.Instance.headingText, _headingtext);
        PopupMessageHandler.Instance.SetText(PopupMessageHandler.Instance.descriptionText, _descriptiontext);
        PopupMessageHandler.Instance.SetText(PopupMessageHandler.Instance.timerText, _timertext);
        PopupMessageHandler.Instance.SetPanelState(_panelstate);
    }
    */
    #endregion
}