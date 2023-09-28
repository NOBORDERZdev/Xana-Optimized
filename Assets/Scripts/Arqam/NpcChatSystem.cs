using System.Collections;
using UnityEngine;
using System;
using UnityEngine.Networking;

public class NpcChatSystem : MonoBehaviour
{
    public int id = 0;
    private string msg = "Hello";

    public class FeedData
    {
        public int id;
        public string response;
    }
    private FeedData feed;

    private void OnEnable()
    {
        if (XanaChatSystem.instance)
            XanaChatSystem.instance.npcAlert += PlayerSendMsg;
    }
    private void OnDisable()
    {
        if (XanaChatSystem.instance)
            XanaChatSystem.instance.npcAlert -= PlayerSendMsg;
    }
    void Start()
    {
        CoroutineUtils.Instance.CallHiddenCoroutine();
    }

    private void PlayerSendMsg(string msgData)
    {
        msg = msgData;
        // Call the API request function
        StartCoroutine(SetApiData());
    }


    IEnumerator SetApiData()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(2f, 6f));
        string prefix = "http://182.70.242.10:8032/api/v1/";
        string url = "update_user_prompt_en?id=";
        //id = 2;
        string midPart = "&prompt=";
        //msg = "I am student";
        string postUrl = prefix + url + id + midPart + msg;

        UnityWebRequest request = UnityWebRequest.Post(postUrl, "POST");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        StartCoroutine(GetResponseData(prefix));
    }

    IEnumerator GetResponseData(string prefix)
    {
        string postFix = "text_from_userid_en?id=";
        string fetchUrl = prefix + postFix + id;

        UnityWebRequest fetchRequest = UnityWebRequest.Get(fetchUrl);
        fetchRequest.downloadHandler = new DownloadHandlerBuffer();
        yield return fetchRequest.SendWebRequest();

        if (fetchRequest.result == UnityWebRequest.Result.Success)
        {
            feed = JsonUtility.FromJson<FeedData>(fetchRequest.downloadHandler.text);

            if (XanaChatSystem.instance)
                XanaChatSocket.onSendMsg?.Invoke(XanaConstants.xanaConstants.MuseumID, feed.response, id.ToString());
            Debug.Log("Communication Response: " + feed.response);
        }
        else
            Debug.LogError("Communication API Error: " + fetchRequest.error);
    }
}


