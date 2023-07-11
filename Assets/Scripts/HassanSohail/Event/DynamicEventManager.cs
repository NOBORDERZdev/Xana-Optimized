using Photon.Pun.Demo.PunBasics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;
using Firebase.DynamicLinks;

public class DynamicEventManager : Singleton<DynamicEventManager>
{
    #region Variables

    [SerializeField] SceneManage sceneManage;

    //Json data response properties
    string data;

    //Deeplink data properties
    public delegate void DeepLink(string arg);
    public static DeepLink deepLink;

    //Event date time properties
    DateTime eventLocalStartDateTime, eventlocalEndDateTime, eventUnivStartDateTime, eventUnivEndDateTime;
    public string[] eventStartDateTime;
    public string[] eventEndDateTime;
    /// <Irfan script>
    string[] vec2 = null;
     private string EventURl = "/userCustomEvent/get-event-json/";

   // TestEvent  "https://api-test.xana.net/userCustomEvent/get-event-json/"
    //MainEvent https://app-api.xana.net/userCustomEvent/get-event-json/45

    //    ConstantsGod.API_BASEURL = "https://app-api.xana.net";
    //  ConstantsGod.API_BASEURL = "https://api-test.xana.net";

    private string EventArguments;
    private int PauseCount;
    private int FocusCount;
    private int StartFocusCounter;
    private string Auth = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6NTU3OCwiaWF0IjoxNjg2MDU4MDk1LCJleHAiOjE2ODYyMzA4OTV9.MfgqIxpKQP-YKlKGgWUMUit35Z2x2IGyFxquz7s_GMU";

    /// </IRFAN Scripts end here>
     #endregion  

    #region Unity Functions

    private void OnEnable()
    {
    
    }

    private void Awake()
    {
        XanaEventDetails.eventDetails = new XanaEventDetails();
        XanaEventDetails.eventDetails.DataIsInitialized = false;
    }

    private void Start()
    {
        //For testing
        //EventArguments = "547";
        EventArguments = "";
        PauseCount = 0;
        FocusCount = 0;
        StartFocusCounter = 1;
        DynamicLinks.DynamicLinkReceived += OnDynamicLink;
        DynamicEventManager.deepLink += InvokeDeepLink;

    }

    private void OnDisable()
    {
        DynamicEventManager.deepLink -= InvokeDeepLink;
    }
    private void OnApplicationPause(bool pause)
    {
        //print("Come to on Pause : " + pause);
        if (pause)
        {
            EventArguments = "";
            PauseCount += 1;
            // receivingn.Instance._text.text = "pause count is : " + PauseCount;
        }
    }
    private void OnApplicationFocus(bool focus)
    {
        //print("Come to on focus : " + focus);
        if (focus && PauseCount > 0)
        {
            StartFocusCounter = 2;
            FocusCount += 1;
            //  receivingn.Instance._text2.text = "Focus count is : " + FocusCount;
            DynamicLinks.DynamicLinkReceived += OnDynamicLink;
        }
    }

    #endregion

    #region Custom Functions

    private void OnDynamicLink(object sender, EventArgs args)
    {
        if (StartFocusCounter == 0)
        {
            return;
        }

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

        vec2 = dynamicLinkEventArgs.ReceivedDynamicLink.Url.OriginalString.Split("=");
        foreach (string word in vec2)
        {
            if (vec2[1] == word)
            {

                print("Argument are :" + word);
                EventArguments = word;
                DynamicLinks.DynamicLinkReceived += OnDynamicLinkEmpty;
                if (StartFocusCounter == 2 && (PlayerPrefs.GetInt("shownWelcome") == 1 || PlayerPrefs.GetInt("IsLoggedIn") == 1))
                {
                    InvokeDeepLink("focus");
                    StartFocusCounter = 0;

                }
                else if (StartFocusCounter == 1)
                {
                    StartFocusCounter = 0;
                }
                // receivingn.Instance._text2.text = "Arguments are  : " + word + "  Bool is  " + Startbool;
            }
        }
    }

    private void OnDynamicLinkEmpty(object sender, EventArgs args)
    {

    }

    //Deeplink related calls
    public void InvokeDeepLink(string _ArgData)
    {
        print("call dynamic link 33");
        print("come from:  " + _ArgData);
        //  receivingn.Instance._text.text = _ArgData;   
        if (EventArguments == "")
        {
            return;
        }
        string EventURl1WithID = "";

        EventURl1WithID = ConstantsGod.API_BASEURL+ EventURl  + EventArguments;

        print("Event API is : " + EventURl1WithID);
             StartCoroutine(HitGetEventJson(EventURl1WithID));
     }   

    //Fetching event data from server
    IEnumerator HitGetEventJson(string url)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            //request.SetRequestHeader("Authorization", Auth);
            yield return request.SendWebRequest();
            Debug.Log("Event data is here :  " + request.downloadHandler.text);
            EventDataDetails eventDetails = JsonUtility.FromJson<EventDataDetails>(request.downloadHandler.text);

            //  XanaEventDetails JsonDataObj1 = JsonUtility.FromJson<XanaEventDetails>(_jsonData);
            XanaEventDetails.eventDetails = eventDetails.data;

            //EventUserRoles JsonDataObj2 = JsonUtility.FromJson<EventUserRoles>(eventDetails.data.eventsUserRoles);  
            //XanaEventDetails.eventDetails.eventsUserRoles.Add(eventDetails.data.eventsUserRoles);





            if (!request.isHttpError && !request.isNetworkError)
            {
                //   myObject1 = myObject1.CreateFromJSON(request.downloadHandler.text);
                if (request.error == null)
                {
                    if (eventDetails.success == true)
                    {
                        //  GetEventJsonData(request.downloadHandler.text);

                        print("~~~~~~~~~ json" + request.downloadHandler.text);
                        XanaEventDetails.eventDetails.DataIsInitialized = true;
                        yield return new WaitForEndOfFrame();
                        if (!XanaEventDetails.eventDetails.name.Equals(""))
                        {
                            print("===============Checking event date time");
                            CheckEventDateTime();
                            //XanaEventDetails.eventDetails.eventType = "dj";
                        }
                        else
                        {
                            LoadingHandler.Instance.EventLoaderCanvas.SetActive(false);
                            print("===============Event Name is null");
                        }


                        print(eventDetails.data.id);
                    }
                }
            }

            else
            {
                if (request.Equals(UnityWebRequest.Result.ConnectionError))
                {
                    LoadingHandler.Instance.EventLoaderCanvas.SetActive(false);
                    yield return StartCoroutine(HitGetEventJson(url));
                    print("===============Network Error");
                }
                else
                {
                    LoadingHandler.Instance.EventLoaderCanvas.SetActive(false);
                    if (request.error != null)
                    {
                        if (eventDetails.success == false)
                        {
                            yield return StartCoroutine(HitGetEventJson(url));
                            if (!XanaEventDetails.eventDetails.name.Equals(""))
                            {
                                print("==============Checking JSON data");
                            }
                        }
                    }
                    else
                    {
                        print("==============Getting Error in request");
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
        //XanaEventDetails.eventDetails.eventsUserRoles.RemoveAt(XanaEventDetails.eventDetails.eventsUserRoles.Count - 1);
    }

    //Extracting environments data from backend through API
    /*    public void GetAllEnvNames()
        {
            StartCoroutine(GetAllEnvMusListAPI("/userCustomEvent/environments-for-dropdown"));
            StartCoroutine(GetAllEnvMusListAPI("/userCustomEvent/museums-for-dropdown", true));
        }

        IEnumerator GetAllEnvMusListAPI(string _url, bool _museumdata = false)
        {
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
            CallMuseumApiAgain:
                using (UnityWebRequest request = UnityWebRequest.Get(ConstantsGod.API_BASEURL + _url))
                {
                    request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
                    request.SendWebRequest();
                    while (!request.isDone)
                    {
                        yield return null;
                    }

                    if (!request.isHttpError && !request.isNetworkError)
                    {
                        if (request.error == null)
                        {
                            GetAllEnvData(request.downloadHandler.text, _museumdata);
                            if (!_museumdata)
                            {
                                print("=============== Environments data fetched: " + XanaEnvironmentsList._xanaenvlist.data.Count);
                                for (int i = 0; i < XanaEnvironmentsList._xanaenvlist.data.Count; i++)
                                {
                                    print("============Environment data: " + XanaEnvironmentsList._xanaenvlist.data[i].environment_name);
                                }
                            }
                            else if (_museumdata)
                            {
                                print("=============== Museum data fetched: " + XanaMuseumList._xanamuslist.data.Count);
                                for (int i = 0; i < XanaMuseumList._xanamuslist.data.Count; i++)
                                {
                                    print("============Museum data: " + XanaMuseumList._xanamuslist.data[i].name);
                                }
                            }
                            else
                                goto CallMuseumApiAgain;
                        }
                        else
                            goto CallMuseumApiAgain;
                    }
                    else
                    {
                        goto CallMuseumApiAgain;
                    }
                    request.Dispose();
                }
            }
        }

        public void GetAllEnvData(string m_JsonData, bool _museumdata)
        {
            if (!_museumdata)
            {
                XanaEnvironmentsList._xanaenvlist = JsonUtility.FromJson<XanaEnvironmentsList>(m_JsonData);
                EnvironementAPIData JsonDataObj2 = JsonUtility.FromJson<EnvironementAPIData>(m_JsonData);
                XanaEnvironmentsList._xanaenvlist.data.Add(JsonDataObj2);
            }
            else
            {
                XanaMuseumList._xanamuslist = JsonUtility.FromJson<XanaMuseumList>(m_JsonData);
                MuseumAPIData JsonDataObj2 = JsonUtility.FromJson<MuseumAPIData>(m_JsonData);
                XanaMuseumList._xanamuslist.data.Add(JsonDataObj2);
            }
        }*/

    //Checking event Date Time if its ended or not conditions
    public void CheckEventDateTime()
    {
        //Getting Event and System Date Time
        string EventStartDateTime = XanaEventDetails.eventDetails.startTime;
        string EventEndDateTime = XanaEventDetails.eventDetails.endTime;

        eventUnivStartDateTime = DateTime.Parse(EventStartDateTime);
        eventUnivEndDateTime = DateTime.Parse(EventEndDateTime);
        eventLocalStartDateTime = eventUnivStartDateTime.ToLocalTime();
        eventlocalEndDateTime = eventUnivEndDateTime.ToLocalTime();

        int _eventStartSystemDateTimediff = (int)(eventLocalStartDateTime - System.DateTime.Now).TotalMinutes;
        int _eventEndSystemDateTimediff = (int)(eventlocalEndDateTime - System.DateTime.Now).TotalMinutes;

        print("===================DIFF : " + _eventStartSystemDateTimediff);
        print("===================DIFFEND : " + _eventEndSystemDateTimediff);

        TimeSpan t = TimeSpan.FromMinutes(_eventStartSystemDateTimediff);
        string timeFormat = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                      t.Hours,
                      t.Minutes,
                      t.Seconds);
        print(timeFormat);

        if (_eventStartSystemDateTimediff > 0)
        {
            if (TimeSpan.FromMinutes(_eventStartSystemDateTimediff).Days > 0)
            {
                SetEventPopUpDialog("Not Event Day yet", "Will Start After:", timeFormat, true);
            }
            else
            {
                SetEventPopUpDialog("Event Time not started yet", "Will Start After:", timeFormat, true);
            }
            print("not started yet");
        }

        if (_eventStartSystemDateTimediff <= 0 && _eventEndSystemDateTimediff >= 0)
        {
            StartCoroutine(DelayLoadRemainingSceneData());
            LoadingHandler.Instance.EventLoaderCanvas.SetActive(true);
             print("On going Event");
        }

        if (_eventEndSystemDateTimediff < 0)
        {
            print("Event Ended");
            SetEventPopUpDialog("Event is Ended", "", "", true);
        }
    }

    IEnumerator DelayLoadRemainingSceneData()
    {
        yield return new WaitForSeconds(4f);
        LoadingHandler.Instance.EventLoaderCanvas.SetActive(false);
        SetSceneData();
    }

    //Loading scene for event after checking date time and event data
    public void SetSceneData()
    {
        XanaConstants.xanaConstants.userLimit = "10";
        //Set These Settings after loading Json Data
        LoadingHandler.Instance.ShowLoading();
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        if (XanaEventDetails.eventDetails.eventType.Equals("XANA_WORLD"))
        {
            FeedEventPrefab.m_EnvName = "Builder";
            XanaConstants.xanaConstants.builderMapID = int.Parse(XanaEventDetails.eventDetails.xana_world_id);
            XanaConstants.xanaConstants.isBuilderScene = true;
            print("***Scene is loading from deep linking***" + XanaConstants.xanaConstants.EnviornmentName);
            LoadingHandler.Instance.worldLoadingScreen.SetActive(false);
            //SceneManager.LoadScene("AddressableScene");
            LoadingHandler.Instance.LoadSceneByIndex("Builder");
        }
        else
        {
            if (!XanaConstants.xanaConstants.JjWorldSceneChange && !XanaConstants.xanaConstants.orientationchanged)
            {
                Screen.orientation = ScreenOrientation.LandscapeLeft;
            }
            if (XanaEventDetails.eventDetails.museumId.Equals(0))
            {
                XanaConstants.xanaConstants.EnviornmentName = XanaEventDetails.eventDetails.environmentName;
                FeedEventPrefab.m_EnvName = XanaEventDetails.eventDetails.environmentName;
                //XanaConstants.xanaConstants.EnviornmentName = "XANA Festival Stage";
                //FeedEventPrefab.m_EnvName = "XANA Festival Stage";
            }
            else
            {
                XanaConstants.xanaConstants.EnviornmentName = XanaEventDetails.eventDetails.museumName;
                FeedEventPrefab.m_EnvName = XanaEventDetails.eventDetails.museumName;
                //XanaConstants.xanaConstants.EnviornmentName = "XANA Festival Stage";
                //FeedEventPrefab.m_EnvName = "XANA Festival Stage";
            }
            print("***Scene is loading from deep linking***" + XanaConstants.xanaConstants.EnviornmentName);
            LoadingHandler.Instance.worldLoadingScreen.SetActive(false);
            SceneManager.LoadScene("AddressableScene");
        }
     
    }

    //UI related calls
    public void SetEventPopUpDialog(string _headingtext = "", string _descriptiontext = "", string _timertext = "", bool _panelstate = false)
    {

        PopupMessageController.Instance.SetText(PopupMessageController.Instance.headingText, _headingtext);

        PopupMessageController.Instance.SetText(PopupMessageController.Instance.descriptionText, _descriptiontext);

        PopupMessageController.Instance.SetText(PopupMessageController.Instance.timerText, _timertext);

        PopupMessageController.Instance.SetPanelState(_panelstate);

    }

    #endregion
}
