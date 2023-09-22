using System.Collections;
using UnityEngine;
using System;
using UnityEngine.Networking;

public class NpcChatSystem : MonoBehaviour
{
    [SerializeField]
    private FeedData feedData;


    void Start()
    {
        // Call the API request function
        StartCoroutine(SetApiData());
    }

    IEnumerator SetApiData()
    {
        string prefix = "http://182.70.242.10:8032/api/v1/";
        string url = "update_user_prompt_en?id=";
        int id = 2;
        string midPart = "&prompt=";
        string msg = "I am student";
        string postUrl = prefix + url + id + midPart + msg;

        UnityWebRequest request = UnityWebRequest.Post(postUrl, "POST");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        StartCoroutine(GetResponseData(prefix, id));
    }

    IEnumerator GetResponseData(string prefix, int id)
    {
        string postFix = "text_from_userid_en?id=";
        string fetchUrl = prefix + postFix + id;

        UnityWebRequest fetchRequest = UnityWebRequest.Get(fetchUrl);
        fetchRequest.downloadHandler = new DownloadHandlerBuffer();
        yield return fetchRequest.SendWebRequest();

        FeedData feed = new FeedData();
        if (fetchRequest.result == UnityWebRequest.Result.Success)
        {
            feed = JsonUtility.FromJson<FeedData>(fetchRequest.downloadHandler.text);
            Debug.Log("Communication Response: " + feed.response);
        }
        else
            Debug.LogError("Communication API Error: " + fetchRequest.error);
    }
}


[Serializable]
public class FeedData
{
    public int id;
    public string prompt;
    public string response;
}

