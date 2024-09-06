using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

public class NpcToNpcChat : MonoBehaviour
{
    [Serializable]
    public class NPCAttributes
    {
        public string aiNames;
        [Header("For test user")]
        public int aiIds;
        [Header("For Live user")]
        public int actualAiIds;
    }
    public List<NPCAttributes> npcAttributes;
    private List<NPCAttributes> npcDB;
    /// <summary>
    /// Output data class
    /// </summary>
    [Serializable]
    public class ResponseData
    {
        public string response_jp;
        public string response_en;
        public string emotion;
    }
    public ResponseData responseData;

    [Serializable]
    public class FeedData
    {
        [Newtonsoft.Json.JsonProperty("in")]
        public InputData input_data;
        [Newtonsoft.Json.JsonProperty("out")]
        public OutputData output_data;
    }
    [System.Serializable]
    public class InputData
    {
        public int target_npc_id;
        public string target_npc_msg;
    }
    [System.Serializable]
    public class OutputData
    {
        public int user_id;
        public string user_msg_jp;
        public string user_msg_en;
        public string emotion;
    }
    public FeedData feed;

    private string msg = "";
    private int npcCounter = 0;
    private int npcThatStartConversation = 0;

    private void OnEnable()
    {
        //ChatSocketManager.instance.npcSendMsg += NpcReply;
    }
    private void OnDisable()
    {
        //ChatSocketManager.instance.npcSendMsg -= NpcReply;
    }

    void Start()
    {
        //StartCoroutine(FetchResponseFromWeb());
    }

    IEnumerator FetchResponseFromWeb()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 3f));  // 3f, 7f

        int temp = UnityEngine.Random.Range(0, npcAttributes.Count);
        npcThatStartConversation = temp;
        // add remaining npcs into list except selected one
        npcDB = new List<NPCAttributes>();
        for (int i = 0; i < npcAttributes.Count; i++)
        {
            if (i != temp)
                npcDB.Add(npcAttributes[i]);
        }

        // shuffle the elements in a list randomly
        int count = npcDB.Count;
        System.Random rand = new System.Random();
        while (count > 1)
        {
            count--;
            int k = rand.Next(count + 1);
            NPCAttributes value = npcDB[k];
            npcDB[k] = npcDB[count];
            npcDB[count] = value;
        }

        string ip = "";
        int id = 0;
        // same api as NPC free speech api
        if (!APIBasepointManager.instance.IsXanaLive)
        {
            ip = "http://182.70.242.10:8034/";
            id = npcAttributes[temp].aiIds;
        }
        else if (APIBasepointManager.instance.IsXanaLive)
        {
            ip = "http://15.152.55.82:8054/";
            id = npcAttributes[temp].actualAiIds;
        }

        string prefix = ip + "api/v2/text_from_userid_en_35?id=";
        string url = prefix + id;
        Debug.Log("<color=red> Communication URL(NpcToNpc): " + url + "</color>");

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            responseData = JsonUtility.FromJson<ResponseData>(request.downloadHandler.text);
            string responseFeed = "";
            if (LocalizationManager.forceJapanese || GameManager.currentLanguage == "ja")
                responseFeed = responseData.response_jp;
            else
                responseFeed = responseData.response_en;

            if (XanaChatSystem.instance)
                ChatSocketManager.onSendMsg?.Invoke(ConstantsHolder.xanaConstants.MuseumID, responseFeed, CallBy.NpcToNpc, id.ToString());
            Debug.Log("Communication Response(Npc Message)(NpcToNpc): " + responseFeed);
        }
        else
            Debug.LogError("Communication API Error(NpcToNpc): " + gameObject.name + request.error);
    }

    private void NpcReply(string data)
    {
        msg = data;
        StartCoroutine(GetMsgResponse());
    }
    IEnumerator GetMsgResponse()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 3f)); // 3f, 7f
        int id = 0;
        string ip = "";

        // same API as NPC to User api
        if (!APIBasepointManager.instance.IsXanaLive)          
        {
            ip = "http://182.70.242.10:8034/";
            id = npcDB[npcCounter].aiIds;
        }
        else if (APIBasepointManager.instance.IsXanaLive)
        {
            ip = "http://15.152.55.82:8054/";
            id = npcDB[npcCounter].actualAiIds;
        }

        string prefix = ip + "api/v2/text_from_usertext_en_35?user_id=";
        string targetData = "&target_id=";
        string messageData = "&msg=";
        string postUrl = prefix + id + targetData + npcAttributes[npcThatStartConversation].aiIds + messageData + msg;
        Debug.Log("<color=red> Communication URL(NpcToNpc): " + postUrl + "</color>");

        UnityWebRequest request = UnityWebRequest.Get(postUrl);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            feed = JsonUtility.FromJson<FeedData>(request.downloadHandler.text);

            string responseFeed = "";
            if (LocalizationManager.forceJapanese || GameManager.currentLanguage == "ja")
                responseFeed = feed.output_data.user_msg_jp;
            else
                responseFeed = feed.output_data.user_msg_en;

            if (XanaChatSystem.instance)
                ChatSocketManager.onSendMsg?.Invoke(ConstantsHolder.xanaConstants.MuseumID, responseFeed, CallBy.FreeSpeechNpc, id.ToString());
            Debug.Log("Communication Response(Npc Reply)(NpcToNpc): " + responseFeed);
        }
        else
            Debug.LogError("Communication API Error(NpcToNpc): " + gameObject.name + request.error);

        npcCounter++;
        //Debug.Log("(NpcToNpc) Count: " + npcDB.Count);
        if (npcCounter < npcDB.Count)
            StartCoroutine(GetMsgResponse());
        else
        {
            npcCounter = 0;
            yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 3f));  // 3f, 7f
            StartCoroutine(FetchResponseFromWeb());
        }
    }

}


