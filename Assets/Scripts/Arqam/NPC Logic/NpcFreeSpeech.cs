using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class NpcFreeSpeech : MonoBehaviour
{
    private NpcChatSystem npcChatSystem;
    [Serializable]
    public class FeedData
    {
        public string response_jp;
        public string response_en;
        public string emotion;
    }
    public FeedData feed;


    private void Awake()
    {
        npcChatSystem = GetComponent<NpcChatSystem>();
    }

    private void Start()
    {
        //StartCoroutine(SetApiData());
    }

    IEnumerator SetApiData()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.3f));  // 3f, 7f

        string ip = "";
        int id = 0;
        int temp = UnityEngine.Random.Range(0, npcChatSystem.npcAttributes.Count);
  
        if (!APIBasepointManager.instance.IsXanaLive)
        {
            ip = "http://182.70.242.10:8034/";
            id = npcChatSystem.npcAttributes[temp].aiIds;
        }
        else if (APIBasepointManager.instance.IsXanaLive)
        {
            ip = "http://15.152.55.82:8054/";
            id = npcChatSystem.npcAttributes[temp].actualAiIds;
        }

        string prefix = ip + "api/v2/text_from_userid_en_35?id=";
        string url = prefix + id;
        Debug.Log("<color=red> Communication URL(FreeAI): " + url + "</color>");

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            feed = JsonUtility.FromJson<FeedData>(request.downloadHandler.text);

            string responseFeed = "";
            if (LocalizationManager.forceJapanese || GameManager.currentLanguage == "ja")
                responseFeed = feed.response_jp;
            else
                responseFeed = feed.response_en;
            if (XanaChatSystem.instance)
                ChatSocketManager.onSendMsg?.Invoke(ConstantsHolder.xanaConstants.MuseumID, responseFeed, CallBy.FreeSpeechNpc, id.ToString());

            Debug.Log("Communication Response(FreeAI): " + responseFeed);
        }
        else
            Debug.LogError("Communication API Error(FreeAI): " + gameObject.name + request.error);

        yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.3f));   // 3f, 7f
        StartCoroutine(SetApiData());
    }


}
