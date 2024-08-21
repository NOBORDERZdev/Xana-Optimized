using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static SummitAIChatHandler;

public class YoutubeVideoByScene : MonoBehaviour
{
    private StreamResponse _response;
    public StreamData Data;

    private string previousURL;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        previousURL = null;
        while (true)
        {
            StartCoroutine(GetStream());
            yield return new WaitForSeconds(5f);
        }

    }

    public IEnumerator GetStream()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(ConstantsGod.API_BASEURL + ConstantsGod.YOUTUBEVIDEOBYSCENE+ WorldItemView.m_EnvName))
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
                Data = null;
                //Debug.LogError("Youtube API returned no result");
            }
            else
            {
               // Debug.LogError("---------"+www.downloadHandler.text);
                _response = JsonUtility.FromJson<StreamResponse>(www.downloadHandler.text.Trim());
                if (_response!=null && _response.data != null)
                {
                    string incominglink = _response.data.link;
                    if (!incominglink.Equals(" "))
                    {
                        if (_response.data.isYoutubeURL)
                        {
                            Data = new StreamData(incominglink, _response.data.isLive, _response.data.isPlaying, _response.data.isYoutubeURL, _response.data.quality);
                        }
                    }
                    else
                    {
                        Debug.Log("No Link Found Turning off player");
                        Data = null;
                    }

                    if (string.IsNullOrEmpty(previousURL) || !previousURL.Equals(Data.URL) || !Data.isPlaying)
                    {
                        Debug.Log("New Link Found");
                        previousURL = Data.URL;
                        SetUpStream();
                    }
                }
            }
        }
    }


    private void SetUpStream()
    {

        if (Data.IsLive && Data.isPlaying)
        {
            
            YoutubePlayerRef.instance.liveStreamerRef.SetActive(true);
            YoutubePlayerRef.instance.normalPlayerParent.SetActive(false);

            YoutubePlayerLivestream player = YoutubePlayerRef.instance.liveStreamerRef.GetComponent<YoutubePlayerLivestream>();
            if (player)
            {
                Debug.LogError("YoutubeStreamController SetUpStream live stream.......:" + Data.URL);
                player.GetLivestreamUrl(Data.URL);
            }
        }
        else
        {
            Debug.LogError("YoutubeStreamController SetUpStream Normal.......:" + Data.URL + "   :isPlaying:" + Data.isPlaying);
            YoutubePlayerRef.instance.liveStreamerRef.SetActive(false);
            YoutubePlayerRef.instance.normalPlayerParent.SetActive(true);

            YoutubeSimplified player = YoutubePlayerRef.instance.normalPlayerParent.GetComponent<YoutubeSimplified>();

            if (player && Data.isPlaying)
            {
                player.url = Data.URL;
                player.Play();
            }

        }
    }


}
   
