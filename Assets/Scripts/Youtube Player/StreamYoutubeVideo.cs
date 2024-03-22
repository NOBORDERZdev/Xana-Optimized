using Newtonsoft.Json;
using RenderHeads.Media.AVProVideo;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;

public class StreamYoutubeVideo : MonoBehaviour
{
    public string streamAbleUrl;
    private string oldUrl;
    public MediaPlayer mediaPlayer;
    public VideoPlayer videoPlayer;
    public static StreamYoutubeVideo instance;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StreamYtVideo(string Url,bool isLive)
    {
        if (oldUrl != Url)
        {
            oldUrl = Url;
            StartCoroutine(GetStreamableUrl(Url,isLive));
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
                Debug.Log(www.error);
            }
            else
            {
                string data = www.downloadHandler.text;
                GetYoutubeStreamableVideo getYoutubeStreamableVideo= JsonConvert.DeserializeObject<GetYoutubeStreamableVideo>(data);
                streamAbleUrl =getYoutubeStreamableVideo.data.downloadableUrl;
                if (isLive)
                {
                    mediaPlayer.OpenMedia(MediaPathType.AbsolutePathOrURL, streamAbleUrl, true);
                    mediaPlayer.Play();
                }
                else
                {
                    videoPlayer.source = VideoSource.Url;
                    videoPlayer.url = streamAbleUrl;
                    videoPlayer.Play();
                }
            }
        }
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