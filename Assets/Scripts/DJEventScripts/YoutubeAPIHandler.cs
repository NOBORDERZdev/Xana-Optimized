using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering.Universal;
using WebSocketSharp;

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

    //string OrdinaryUTCdateOfSystem = "2023-08-10T14:45:00.000Z";
    //DateTime OrdinarySystemDateTime, localENDDateTime, univStartDateTime, univENDDateTime;

    private Camera mainCam;
    private void Start()
    {
        if(WorldItemView.m_EnvName.Contains("BreakingDown Arena") || WorldItemView.m_EnvName.Contains("DJ Event") || WorldItemView.m_EnvName.Contains("XANA Festival Stage") || WorldItemView.m_EnvName.Contains("Xana Festival") || WorldItemView.m_EnvName.Contains("NFTDuel Tournament"))
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
                        Data = new StreamData(XanaEventDetails.eventDetails.youtubeUrl, XanaEventDetails.eventDetails.youtubeUrl_isActive, true);
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
                                Data = new StreamData(incominglink, _response.data.isLive, _response.data.isPlaying);
                                _urlDataInitialized = true;
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
                            Data = new StreamData(XanaEventDetails.eventDetails.youtubeUrl, XanaEventDetails.eventDetails.youtubeUrl_isActive, true);
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
                                    Data = new StreamData(incominglink, _response.data.isLive, _response.data.isPlaying);
                                    _urlDataInitialized = true;
                                    // print("Stage 3 video link:" + Data);
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
                            Data = new StreamData(XanaEventDetails.eventDetails.youtubeUrl, XanaEventDetails.eventDetails.youtubeUrl_isActive, true);
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
                                    Data = new StreamData(incominglink, _response.data.isLive, _response.data.isPlaying);
                                    _urlDataInitialized = true;
                                    // print("Stage 3 video link:" + Data);
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
                        Data = new StreamData(XanaEventDetails.eventDetails.youtubeUrl, XanaEventDetails.eventDetails.youtubeUrl_isActive, true);
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

                    yield return www.SendWebRequest();

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
                                Data = new StreamData(incominglink, _response.data.isLive, _response.data.isPlaying);
                                _urlDataInitialized = true;
                                // print("Stage 3 video link:" + Data);
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
                        Data = new StreamData(XanaEventDetails.eventDetails.youtubeUrl, XanaEventDetails.eventDetails.youtubeUrl_isActive, true);
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
                                Data = new StreamData(incominglink, _response.data.isLive, _response.data.isPlaying);
                                _urlDataInitialized = true;
                                // print("Stage 3 video link:" + Data);
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
                    Debug.Log("Youtube API returned no result");
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
                                bool _isLiveVideo = _apiResponse.videoData.type.Contains("Live")? true : false;
                                Data = new StreamData(incominglink, _isLiveVideo, true);
                                OldAWSURL = "";
                            }
                            else//For AWS Video playing
                            {
                                if (OldAWSURL != _apiResponse.videoData.url)
                                {
                                    if (GetComponent<AvProLiveVideoSoundEnabler>())
                                    {
                                        GetComponent<AvProLiveVideoSoundEnabler>().EnableVideoScreen(false);
                                    }
                                    GetComponent<StreamYoutubeVideo>().mediaPlayer.gameObject.SetActive(false);
                                    GetComponent<StreamYoutubeVideo>().mediaPlayer.enabled = false;
                                    GetComponent<StreamYoutubeVideo>().videoPlayer.gameObject.SetActive(true);
                                    GetComponent<StreamYoutubeVideo>().videoPlayer.enabled = true;

                                    //SoundController.Instance.videoPlayerSource = gameObject.GetComponent<StreamYoutubeVideo>().videoPlayer.GetComponent<AudioSource>();
                                    //SoundSettings.soundManagerSettings.videoSource = gameObject.GetComponent<StreamYoutubeVideo>().videoPlayer.GetComponent<AudioSource>();
                                    //SoundSettings.soundManagerSettings.setNewSliderValues();

                                    gameObject.GetComponent<StreamYoutubeVideo>().videoPlayer.url = _apiResponse.videoData.url;
                                    gameObject.GetComponent<StreamYoutubeVideo>().videoPlayer.Play();
                                    OldAWSURL = _apiResponse.videoData.url;
                                }
                                Data = null;
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

}
//}

[System.Serializable]
public class StreamData
{
    public string URL;
    public bool IsLive;
    public bool isPlaying;

    public StreamData(string URL, bool isLive, bool isPlaying)
    {
        this.URL = URL;
        this.IsLive = isLive;
        this.isPlaying = isPlaying;
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
}
