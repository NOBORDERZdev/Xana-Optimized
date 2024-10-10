using Newtonsoft.Json;
using RenderHeads.Media.AVProVideo;
using SimpleJSON;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;
using UnityEngine.Events;

public class StreamYoutubeVideo : MonoBehaviour
{
    public string streamAbleUrl;
    public string oldUrl;
    public GameObject LiveVideoUIRef;
    public SummitDomeNFTDataController SumitDomeNftCntrlrRef;
    public MediaPlayer mediaPlayer;
    public VideoPlayer videoPlayer;
    public UnityEvent liveVideoPlay;

    //Store ID for Builder Scene
    public string id;

    public void StreamYtVideo(string Url, bool isLive)
    {
        if (oldUrl != Url)
        {
            StartCoroutine(GetStreamableUrl(Url, isLive));
        }
        else if(isLive)
        {
            PlayLiveVideo();
        }
        else if (!isLive)
        {
            PlayPrerecordedVideo();
        }
        else
        {
            if (SumitDomeNftCntrlrRef)
            {
                SumitDomeNftCntrlrRef.TurnOfLdrOnPlayLiveVideo();
            }
        }
    }

    public IEnumerator GetStreamableUrl(string Url, bool isLive)
    {
        WWWForm form = new WWWForm();
        form.AddField("youtubeUrl", Url);
        using (UnityWebRequest www = UnityWebRequest.Post((ConstantsGod.API_BASEURL + ConstantsGod.GetStreamableYoutubeUrl), form))
        {
            www.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);

            yield return www.SendWebRequest();
            while (!www.isDone)
            {
                yield return null;
            }
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log("SteamError:" + www.error);
            }
            else
            {
                oldUrl = Url;
                string data = www.downloadHandler.text;
                GetYoutubeStreamableVideo getYoutubeStreamableVideo = JsonConvert.DeserializeObject<GetYoutubeStreamableVideo>(data);
                streamAbleUrl = getYoutubeStreamableVideo.data.downloadableUrl;
                if (isLive)
                {
                    PlayLiveVideo();
                }
                else
                {
                    PlayPrerecordedVideo();
                }
            }
            www.Dispose();
        }
    }


    private void PlayLiveVideo()
    {
        mediaPlayer.OpenMedia(MediaPathType.AbsolutePathOrURL, streamAbleUrl, true);
        mediaPlayer.Play();
        liveVideoPlay.Invoke();
        //BuilderEventManager.YoutubeVideoLoadedCallback?.Invoke(id);
    }

    private void PlayPrerecordedVideo()
    {
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = streamAbleUrl;

        if (ConstantsHolder.xanaConstants.isBuilderScene)
        {
            videoPlayer.Prepare();
            videoPlayer.prepareCompleted += VideoPrepared;
        }
        else
            videoPlayer.Play();
    }

    void VideoPrepared(VideoPlayer vp)
    {
        vp.Play();
        BuilderEventManager.YoutubeVideoLoadedCallback?.Invoke(id);
    }
}
[System.Serializable]
public class GetYoutubeStreamableVideo
{
    public bool success;
    public YoutubeStreamData data;
    public string msg;
}
public class YoutubeStreamData
{
    public string downloadableUrl;
}