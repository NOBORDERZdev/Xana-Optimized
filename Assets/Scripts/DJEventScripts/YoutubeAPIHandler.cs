using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering.Universal;
using UnityEngine.Video;

public class YoutubeAPIHandler : MonoBehaviour
{

    private StreamResponse _response;
    public SummitVideoData _apiResponse = new SummitVideoData();

    private int DataIndex = 4;
    public StreamData Data;
    bool _urlDataInitialized = false;
    public string OldAWSURL = "xyz";
    public int summitAreaID;
    public int SummitVideoIndex;
    public AdvancedYoutubePlayer VideoPlayerRef;
    public bool IsSummitDomeWorld = false;

    //string OrdinaryUTCdateOfSystem = "2023-08-10T14:45:00.000Z";
    //DateTime OrdinarySystemDateTime, localENDDateTime, univStartDateTime, univENDDateTime;

    private Camera mainCam;

    private void OnEnable()
    {
        if (!VideoPlayerRef)
        {
            VideoPlayerRef = GetComponent<AdvancedYoutubePlayer>();
        }
    }
    private void Start()
    {
        if (WorldItemView.m_EnvName.Contains("BreakingDown Arena") || WorldItemView.m_EnvName.Contains("DJ Event") || WorldItemView.m_EnvName.Contains("XANA Festival Stage") || WorldItemView.m_EnvName.Contains("Xana Festival") || WorldItemView.m_EnvName.Contains("NFTDuel Tournament"))
        {
            if (GameObject.FindGameObjectWithTag("MainCamera") != null)
            {
                if (GameObject.FindGameObjectWithTag("MainCamera").GetComponent<UniversalAdditionalCameraData>())
                {
                    GameObject.FindGameObjectWithTag("MainCamera").GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
                }
            }
        }

    }

    public IEnumerator GetStream()
    {
        //print("===================Get Stream" + _urlDataInitialized);
        WWWForm form = new WWWForm();

        //form.AddField("token", "piyush55");
        //if (!_urlDataInitialized)
        //{
        if (WorldItemView.m_EnvName.Contains("DJ Event"))
        {
            //if (GameObject.FindGameObjectWithTag("MainCamera") != null)
            //{
            //    if (GameObject.FindGameObjectWithTag("MainCamera").GetComponent<UniversalAdditionalCameraData>())
            //    {
            //        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
            //    }
            //}
            //Debug.Log("call hua kya he");
            if (XanaEventDetails.eventDetails.DataIsInitialized)
            {
                if (checkEventStartTime())
                {
                    if (!XanaEventDetails.eventDetails.youtubeUrl.Equals(null))
                    {
                        //print("============Setting Youtube Link Data" + XanaEventDetails.eventDetails.youtubeUrl);
                        Data = new StreamData(XanaEventDetails.eventDetails.youtubeUrl, XanaEventDetails.eventDetails.youtubeUrl_isActive, true, true, "Standard");
                        _urlDataInitialized = true;
                    }
                    else
                    {
                        //print("================No youtube link found");
                        Data = null;
                    }
                }
                //XanaEventDetails.eventDetails.DataIsInitialized = false;
            }
            else
            {
                using (UnityWebRequest www = UnityWebRequest.Get(ConstantsGod.API_BASEURL + ConstantsGod.SHARELINKS))
                {

                    www.timeout = 10;

                    yield return www.SendWebRequest();

                    while (!www.isDone)
                    {
                        yield return null;
                    }
                    if (www.isHttpError || www.isNetworkError)
                    {
                        _response = null;
                        Data = null;
                        Debug.Log("Youtube API returned no result");
                    }
                    else
                    {
                        _response = JsonUtility.FromJson<StreamResponse>(www.downloadHandler.text.Trim());
                        if (_response != null)
                        {
                            string incominglink = _response.data.link;
                            if (!string.IsNullOrEmpty(incominglink))
                            {
                                if (_response.data.isYoutubeURL)
                                {
                                    Data = new StreamData(incominglink, _response.data.isLive, _response.data.isPlaying, _response.data.isYoutubeURL, _response.data.quality);
                                    _urlDataInitialized = true;
                                    OldAWSURL = "xyz";
                                }
                                else//For AWS Video playing
                                {
                                    PlayAWSVideoSetup();
                                }
                            }
                            else
                            {
                                Debug.Log("No Link Found Turning off player");
                                Data = null;
                            }
                        }

                    }
                }
            }
        }
        else if (WorldItemView.m_EnvName.Contains("XANA Festival Stage"))
        {
            if (WorldItemView.m_EnvName.Contains("Dubai."))
            {
                //if (GameObject.FindGameObjectWithTag("MainCamera") != null)
                //{
                //    if (GameObject.FindGameObjectWithTag("MainCamera").GetComponent<UniversalAdditionalCameraData>())
                //    {
                //        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
                //    }
                //}
                //Debug.Log("call hua kya he");
                if (XanaEventDetails.eventDetails.DataIsInitialized)
                {
                    if (checkEventStartTime())
                    {
                        if (!XanaEventDetails.eventDetails.youtubeUrl.Equals(null))
                        {
                            //print("============Setting Youtube Link Data" + XanaEventDetails.eventDetails.youtubeUrl);
                            Data = new StreamData(XanaEventDetails.eventDetails.youtubeUrl, XanaEventDetails.eventDetails.youtubeUrl_isActive, true, true, "Standard");
                            _urlDataInitialized = true;
                        }
                        else
                        {
                            //print("================No youtube link found");
                            Data = null;
                        }
                    }
                    //XanaEventDetails.eventDetails.DataIsInitialized = false;
                }
                else
                {
                    //print("============Setting WWW data");
                    using (UnityWebRequest www = UnityWebRequest.Get(ConstantsGod.API_BASEURL + ConstantsGod.YOUTUBEVIDEOBYSCENE + WorldItemView.m_EnvName))
                    {
                        www.timeout = 10;

                        yield return www.SendWebRequest();

                        while (!www.isDone)
                        {
                            yield return null;
                        }
                        if (www.isHttpError || www.isNetworkError)
                        {
                            _response = null;
                            //Debug.Log("Youtube API returned no result");
                        }
                        else
                        {
                            //Debug.Log("You tube respns===" + www.downloadHandler.text.Trim());
                            _response = JsonUtility.FromJson<StreamResponse>(www.downloadHandler.text.Trim());
                            if (_response != null)
                            {
                                string incominglink = _response.data.link;
                                if (!string.IsNullOrEmpty(incominglink))
                                {
                                    if (_response.data.isYoutubeURL)
                                    {
                                        Data = new StreamData(incominglink, _response.data.isLive, _response.data.isPlaying, _response.data.isYoutubeURL, _response.data.quality);
                                        _urlDataInitialized = true;
                                        OldAWSURL = "xyz";
                                    }
                                    else//For AWS Video playing
                                    {
                                        PlayAWSVideoSetup();
                                    }
                                }
                                else
                                {
                                    Debug.Log("No Link Found Turning off player");
                                    Data = null;
                                }
                            }

                        }
                    }
                }
            }
            else
            {
                //if (GameObject.FindGameObjectWithTag("MainCamera") != null)
                //{
                //    if (GameObject.FindGameObjectWithTag("MainCamera").GetComponent<UniversalAdditionalCameraData>())
                //    {
                //        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
                //    }
                //}
                //Debug.Log("call hua kya he");
                if (XanaEventDetails.eventDetails.DataIsInitialized)
                {
                    if (checkEventStartTime())
                    {
                        if (!XanaEventDetails.eventDetails.youtubeUrl.Equals(null))
                        {
                            //print("============Setting Youtube Link Data" + XanaEventDetails.eventDetails.youtubeUrl);
                            Data = new StreamData(XanaEventDetails.eventDetails.youtubeUrl, XanaEventDetails.eventDetails.youtubeUrl_isActive, true, true, "Standard");
                            _urlDataInitialized = true;
                        }
                        else
                        {
                            //print("================No youtube link found");
                            Data = null;
                        }
                    }
                    //XanaEventDetails.eventDetails.DataIsInitialized = false;
                }
                else
                {
                    //print("============Setting WWW data");
                    using (UnityWebRequest www = UnityWebRequest.Get(ConstantsGod.API_BASEURL + ConstantsGod.SHAREDEMOS))
                    {
                        www.timeout = 10;

                        yield return www.SendWebRequest();

                        while (!www.isDone)
                        {
                            yield return null;
                        }
                        if (www.isHttpError || www.isNetworkError)
                        {
                            _response = null;
                            Data = null;
                            Debug.Log("Youtube API returned no result");
                        }
                        else
                        {
                            _response = JsonUtility.FromJson<StreamResponse>(www.downloadHandler.text.Trim());
                            if (_response != null)
                            {
                                string incominglink = _response.data.link;
                                if (!string.IsNullOrEmpty(incominglink))
                                {
                                    if (_response.data.isYoutubeURL)
                                    {
                                        Data = new StreamData(incominglink, _response.data.isLive, _response.data.isPlaying, _response.data.isYoutubeURL, _response.data.quality);
                                        _urlDataInitialized = true;
                                        OldAWSURL = "xyz";
                                    }
                                    else//For AWS Video playing
                                    {
                                        PlayAWSVideoSetup();
                                    }
                                }
                                else
                                {
                                    Debug.Log("No Link Found Turning off player");
                                    Data = null;
                                }
                            }

                        }
                    }
                }
            }
        }
        else if (WorldItemView.m_EnvName.Contains("XANA Lobby") || WorldItemView.m_EnvName.Contains("Xana Festival") || WorldItemView.m_EnvName.Contains("NFTDuel Tournament"))
        {
            //if (GameObject.FindGameObjectWithTag("MainCamera") != null)
            //{
            //    if (GameObject.FindGameObjectWithTag("MainCamera").GetComponent<UniversalAdditionalCameraData>())
            //    {
            //        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
            //    }
            //}
            //Debug.Log("call hua kya he");
            if (XanaEventDetails.eventDetails.DataIsInitialized)
            {
                if (checkEventStartTime())
                {
                    if (!XanaEventDetails.eventDetails.youtubeUrl.Equals(null))
                    {
                        //print("============Setting Youtube Link Data" + XanaEventDetails.eventDetails.youtubeUrl);
                        Data = new StreamData(XanaEventDetails.eventDetails.youtubeUrl, XanaEventDetails.eventDetails.youtubeUrl_isActive, true, true, "Standard");
                        _urlDataInitialized = true;
                    }
                    else
                    {
                        //print("================No youtube link found");
                        Data = null;
                    }
                    //XanaEventDetails.eventDetails.DataIsInitialized = false;
                }
            }
            else
            {
                //print("============Setting WWW data");
                //Debug.LogError("WaqasApi============" + ConstantsGod.API_BASEURL + ConstantsGod.YOUTUBEVIDEOBYSCENE + FeedEventPrefab.m_EnvName);
                using (UnityWebRequest www = UnityWebRequest.Get(ConstantsGod.API_BASEURL + ConstantsGod.YOUTUBEVIDEOBYSCENE + WorldItemView.m_EnvName))
                {
                    www.timeout = 10;

                    www.SendWebRequest();

                    while (!www.isDone)
                    {
                        yield return null;
                    }
                    if (www.isHttpError || www.isNetworkError)
                    {
                        _response = null;
                        Debug.Log("<color=red> Youtube API returned no result </color>");
                    }
                    else
                    {
                        _response = JsonUtility.FromJson<StreamResponse>(www.downloadHandler.text.Trim());
                        if (_response != null)
                        {
                            string incominglink = _response.data.link;
                            if (!string.IsNullOrEmpty(incominglink))
                            {
                                if (_response.data.isYoutubeURL)
                                {
                                    Data = new StreamData(incominglink, _response.data.isLive, _response.data.isPlaying, _response.data.isYoutubeURL, _response.data.quality);
                                    _urlDataInitialized = true;
                                    OldAWSURL = "xyz";
                                    // print("Stage 3 video link:" + Data);
                                }
                                else//For AWS Video playing
                                {
                                    PlayAWSVideoSetup();
                                }
                            }
                            else
                            {
                                Debug.Log("No Link Found Turning off player");
                                Data = null;
                            }
                        }

                    }
                }
            }
        }
        else if (WorldItemView.m_EnvName.Contains("BreakingDown Arena"))
        {
            //if (GameObject.FindGameObjectWithTag("MainCamera") != null)
            //{
            //    if (GameObject.FindGameObjectWithTag("MainCamera").GetComponent<UniversalAdditionalCameraData>())
            //    {
            //        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
            //    }
            //}
            //Debug.Log("call hua kya he");
            if (XanaEventDetails.eventDetails.DataIsInitialized)
            {
                if (checkEventStartTime())
                {
                    if (!XanaEventDetails.eventDetails.youtubeUrl.Equals(null))
                    {
                        //print("============Setting Youtube Link Data" + XanaEventDetails.eventDetails.youtubeUrl);
                        Data = new StreamData(XanaEventDetails.eventDetails.youtubeUrl, XanaEventDetails.eventDetails.youtubeUrl_isActive, true, true, "Standard");
                        _urlDataInitialized = true;
                    }
                    else
                    {
                        //print("================No youtube link found");
                        Data = null;
                    }
                }
                //XanaEventDetails.eventDetails.DataIsInitialized = false;
            }
            else
            {
                //print("============Setting WWW data");
                using (UnityWebRequest www = UnityWebRequest.Get(ConstantsGod.API_BASEURL + ConstantsGod.YOUTUBEVIDEOBYSCENE + WorldItemView.m_EnvName))
                {
                    www.timeout = 10;

                    yield return www.SendWebRequest();

                    while (!www.isDone)
                    {
                        yield return null;
                    }
                    if (www.isHttpError || www.isNetworkError)
                    {
                        _response = null;
                        Debug.Log("Youtube API returned no result");
                    }
                    else
                    {
                        //Debug.Log("You tube respns===" + www.downloadHandler.text.Trim());
                        _response = JsonUtility.FromJson<StreamResponse>(www.downloadHandler.text.Trim());
                        if (_response != null)
                        {
                            string incominglink = _response.data.link;
                            if (!string.IsNullOrEmpty(incominglink))
                            {
                                if (_response.data.isYoutubeURL)
                                {
                                    Data = new StreamData(incominglink, _response.data.isLive, _response.data.isPlaying, _response.data.isYoutubeURL, _response.data.quality);
                                    _urlDataInitialized = true;
                                    OldAWSURL = "xyz";
                                }
                                else//For AWS Video playing
                                {
                                    PlayAWSVideoSetup();
                                }
                            }
                            else
                            {
                                Debug.Log("No Link Found Turning off player");
                                Data = null;
                            }
                        }

                    }
                }
            }
        }
        else if (ConstantsHolder.xanaConstants.EnviornmentName.Contains("XANA Summit"))
        {
            using (UnityWebRequest www = UnityWebRequest.Get(ConstantsGod.API_BASEURL + ConstantsGod.SUMMITYOUTUBEVIDEOBYID + summitAreaID + "/" + SummitVideoIndex))
            {
                www.timeout = 10;

                www.SendWebRequest();

                while (!www.isDone)
                {
                    yield return null;
                }
                if (www.isHttpError || www.isNetworkError)
                {
                    _apiResponse = null;
                    //    Debug.Log("Youtube API returned no result");
                }
                else
                {
                    _apiResponse = JsonUtility.FromJson<SummitVideoData>(www.downloadHandler.text.Trim());
                    if (_apiResponse != null)
                    {
                        string incominglink = _apiResponse.videoData.url;
                        if (!string.IsNullOrEmpty(incominglink))
                        {
                            if (_apiResponse.videoData.isYoutube)
                            {
                                bool _isLiveVideo = _apiResponse.videoData.type.Contains("Live") ? true : false;
                                Data = new StreamData(incominglink, _isLiveVideo, _apiResponse.videoData.isPlaying, _apiResponse.videoData.isYoutube, "standard");
                                OldAWSURL = "xyz";
                            }
                            else//For AWS Video playing
                            {
                                if (OldAWSURL != _apiResponse.videoData.url)
                                {
                                    _response = null;
                                }
                                if (_response == null)
                                {
                                    _response = new StreamResponse();
                                    _response.data = new IncomingData();
                                    _response.data.link = _apiResponse.videoData.url;
                                    _response.data.isLive = false;
                                    _response.data.isPlaying = _apiResponse.videoData.isPlaying;
                                    _response.data.isYoutubeURL = _apiResponse.videoData.isYoutube;
                                }
                                PlayAWSVideoSetup();
                            }
                        }
                        else
                        {
                            //   Debug.Log("No Link Found Turning off player");
                            Data = null;
                        }
                    }
                }
            }
        }
        else if (ConstantsHolder.isFromXANASummit && IsSummitDomeWorld)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(ConstantsGod.API_BASEURL + ConstantsGod.GETSINGLEDOME + ConstantsHolder.domeId))
            {
                www.timeout = 10;

                www.SendWebRequest();

                while (!www.isDone)
                {
                    yield return null;
                }
                if (www.isHttpError || www.isNetworkError)
                {
                    _apiResponse = null;
                    //    Debug.Log("Youtube API returned no result");
                }
                else
                {
                    SingleDomeData _tempApiResponse = new SingleDomeData();
                    _tempApiResponse = JsonUtility.FromJson<SingleDomeData>(www.downloadHandler.text.Trim());
                    if (_tempApiResponse.dome.mediaType == "Video")
                    {
                        _apiResponse.videoData.url = _tempApiResponse.dome.mediaUpload;
                        _apiResponse.videoData.isYoutube = _tempApiResponse.dome.isYoutubeUrl;
                        _apiResponse.videoData.type = _tempApiResponse.dome.videoType;
                        _apiResponse.videoData.isPlaying = true;

                        if (_apiResponse != null)
                        {
                            string incominglink = _apiResponse.videoData.url;
                            if (!string.IsNullOrEmpty(incominglink))
                            {
                                if (_apiResponse.videoData.isYoutube)
                                {
                                    bool _isLiveVideo = _apiResponse.videoData.type.Contains("Live") ? true : false;
                                    Data = new StreamData(incominglink, _isLiveVideo, _apiResponse.videoData.isPlaying, _apiResponse.videoData.isYoutube, "standard");
                                    OldAWSURL = "xyz";
                                }
                                else//For AWS Video playing
                                {
                                    if (OldAWSURL != _apiResponse.videoData.url)
                                    {
                                        _response = null;
                                    }
                                    if (_response == null)
                                    {
                                        _response = new StreamResponse();
                                        _response.data = new IncomingData();
                                        _response.data.link = _apiResponse.videoData.url;
                                        _response.data.isLive = false;
                                        _response.data.isPlaying = _apiResponse.videoData.isPlaying;
                                        _response.data.isYoutubeURL = _apiResponse.videoData.isYoutube;
                                    }
                                    PlayAWSVideoSetup();
                                }
                            }
                            else
                            {
                                //   Debug.Log("No Link Found Turning off player");
                                Data = null;
                            }
                        }
                    }

                }
            }
            //var domedata = GameplayEntityLoader.instance.XanaSummitDataContainerObject.GetDomeData(ConstantsHolder.domeId);
        }

    }

    public bool checkEventStartTime()
    {
        //        univStartDateTime = DateTime.Parse(OrdinaryUTCdateOfSystem);
        //OrdinarySystemDateTime = univStartDateTime.ToLocalTime();
        //Debug.Log("-------------Checking video play time");
        DateTime eventUnivStartDateTime = DateTime.Parse(XanaEventDetails.eventDetails.startTime);
        DateTime eventLocalStartDateTime = eventUnivStartDateTime.ToLocalTime();
        int _eventStartSystemDateTimediff = (int)(eventLocalStartDateTime - System.DateTime.Now).TotalMinutes;
        //Debug.Log("-------------Event API start time is" + XanaEventDetails.eventDetails.startTime);
        //Debug.Log("-------------Event Converted start time is" + eventLocalStartDateTime);
        //Debug.Log("-------------Start time and system time diff" + _eventStartSystemDateTimediff);
        if (_eventStartSystemDateTimediff <= 0)
        {
            return true;
        }
        //Debug.Log("-------------Not found yet");
        return false;
    }

    public void PlayAWSVideoSetup()
    {
        if (OldAWSURL != _response.data.link)
        {
            VideoPlayerRef.VideoPlayer.gameObject.SetActive(false);
            VideoPlayerRef.VideoPlayer.enabled = false;
            Invoke(nameof(SetupAWSScreen), 0.2f);
        }
        Data = null;
    }

    void SetupAWSScreen()
    {
        VideoPlayerRef.EnableVideoScreen(false);
        VideoPlayerRef.VideoPlayer.playOnAwake = true;
        VideoPlayerRef.AVProVideoPlayer.gameObject.SetActive(false);
        VideoPlayerRef.AVProVideoPlayer.enabled = false;
        VideoPlayerRef.VideoPlayer.gameObject.SetActive(true);
        VideoPlayerRef.VideoPlayer.enabled = true;

        //SoundController.Instance.videoPlayerSource = gameObject.GetComponent<StreamYoutubeVideo>().videoPlayer.GetComponent<AudioSource>();
        //SoundSettings.soundManagerSettings.videoSource = gameObject.GetComponent<StreamYoutubeVideo>().videoPlayer.GetComponent<AudioSource>();
        //SoundSettings.soundManagerSettings.setNewSliderValues();

        VideoPlayerRef.VideoPlayer.url = _response.data.link;
        VideoPlayerRef.VideoPlayer.prepareCompleted -= OnPrepareCompleted;
        VideoPlayerRef.VideoPlayer.Prepare();

        // Assign the individual function to the prepareCompleted event
        VideoPlayerRef.VideoPlayer.prepareCompleted += OnPrepareCompleted;

        //gameObject.GetComponent<StreamYoutubeVideo>().videoPlayer.time = 0;
        //gameObject.GetComponent<StreamYoutubeVideo>().videoPlayer.Play();
        OldAWSURL = _response.data.link;
    }

    // Define the function to be called when prepareCompleted is triggered
    void OnPrepareCompleted(VideoPlayer vp)
    {
        vp.time = 0;
        vp.Play();
        SetBGMAudioSound();
    }

    public void SetBGMAudioSound()
    {
        if (gameObject.GetComponent<BGMVolumeControlOnTrigger>())
        {
            if (gameObject.GetComponent<BGMVolumeControlOnTrigger>().IsPlayerCollided)
            {
                gameObject.GetComponent<BGMVolumeControlOnTrigger>().SetBGMAudioOnTrigger(true);
            }
        }
    }

}

[System.Serializable]
public class StreamData
{
    public string URL;
    public bool IsLive;
    public bool isPlaying;
    public bool isYoutubeURL;
    public string quality;

    public StreamData(string URL, bool isLive, bool isPlaying, bool isYoutubeURL, string quality)
    {
        this.URL = URL;
        this.IsLive = isLive;
        this.isPlaying = isPlaying;
        this.isYoutubeURL = isYoutubeURL;
        this.quality = quality;
    }

}

[System.Serializable]
public partial class StreamResponse
{
    public bool success;
    public string msg;
    public IncomingData data;


}

[System.Serializable]
public partial class IncomingData
{
    public long id;
    public string link;
    public bool isLive;
    public bool isYoutubeURL;
    public string quality;
    public object createdAt;
    public object updatedAt;
    public bool isPlaying;
}

[System.Serializable]
public class SummitVideoData
{
    public SummitVideoDetails videoData;
}

[System.Serializable]
public class SummitVideoDetails
{
    public int id;
    public int areaId;
    public string areaName;
    public int index;
    public string url;
    public string type;
    public bool isYoutube;
    public bool isPlaying;
}

[System.Serializable]
public class SingleDomeData
{
    public DomeGeneralData dome;
}

[System.Serializable]
public class DomeGeneralData
{
    public string mediaType;
    public string proportionType;
    public bool isYoutubeUrl;
    public string videoType;
    public string mediaUpload;
}
