using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine.Video;

public class FetchSteamableURL : MonoBehaviour
{
    [Tooltip("Enter Any youtube video URL")]
    public string youtubeVideoURL = "https://www.youtube.com/watch?v=o56ywukKZLY";
    [Space(5)]
    public VideoPlayer videoPlayer;

    private void OnEnable()
    {
        StreamYtVideo(youtubeVideoURL);
    }
    private void OnDisable()
    {
        if (videoPlayer.renderMode == VideoRenderMode.RenderTexture)
            videoPlayer.targetTexture.Release();
    }
    public void StreamYtVideo(string Url)
    {
        StartCoroutine(GetStreamableUrl(Url));
    }
    public IEnumerator GetStreamableUrl(string Url)
    {
        WWWForm form = new WWWForm();
        form.AddField("youtubeUrl", Url);
        using (UnityWebRequest www = UnityWebRequest.Post((ConstantsGod.API_BASEURL + ConstantsGod.GetStreamableYoutubeUrl), form))  //"https://api-test.xana.net"
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
                GetYoutubeStreamableVideo getYoutubeStreamableVideo = JsonConvert.DeserializeObject<GetYoutubeStreamableVideo>(data);
                videoPlayer.gameObject.SetActive(true);
                videoPlayer.url = getYoutubeStreamableVideo.data.downloadableUrl;
                videoPlayer.Play();
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
