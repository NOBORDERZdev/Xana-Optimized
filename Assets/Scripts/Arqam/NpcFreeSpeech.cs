using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class NpcFreeSpeech : MonoBehaviour
{
    private NpcChatSystem npcChatSystem;
    private class FeedData
    {
        public string user_id;
        public string prompt;
        public string response;
    }
    private FeedData feed;


    private void Awake()
    {
        npcChatSystem = GetComponent<NpcChatSystem>();
    }

    private void Start()
    {
         StartCoroutine(SetApiData());
    }

    IEnumerator SetApiData()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(3f, 5f));

        //for live http://15.152.13.112:8032/
        //for test http://182.70.242.10:8032/
        string prefix = "http://15.152.13.112:8032/api/v1/text_from_userid_en_35?id=";
        int temp = UnityEngine.Random.Range(0, npcChatSystem.npcAttributes.Count);
        int id = npcChatSystem.npcAttributes[temp].aiIds;

        string url = prefix + id;
        Debug.Log("<color=red> Communication URL: " + url + "</color>");

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("<color=red> Communication URL: " + request.downloadHandler.text + "</color>");

            feed = JsonUtility.FromJson<FeedData>(request.downloadHandler.text);

            if (XanaChatSystem.instance)
                XanaChatSocket.onSendMsg?.Invoke(XanaConstants.xanaConstants.MuseumID, feed.response, id.ToString());
            Debug.Log("Communication Response: " + feed.response);
        }
        else
            Debug.LogError("Communication API Error: " + gameObject.name + request.error);

        yield return new WaitForSeconds(UnityEngine.Random.Range(3f, 5f));
        StartCoroutine(SetApiData());
    }


}
