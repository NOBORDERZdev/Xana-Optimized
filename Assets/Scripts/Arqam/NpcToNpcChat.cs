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
    /// Out data class
    /// </summary>
    private class ResponseData
    {
        public string user_id;
        public string prompt;
        public string response;
    }
    private ResponseData responseData;
    private class FeedData
    {
        public int id;
        public string response;
    }
    private FeedData feed;
    private string msg = "";
    private int npcCounter = 0;

    private void OnEnable()
    {
        XanaChatSocket.instance.npcSendMsg += NpcReply;
    }
    private void OnDisable()
    {
        XanaChatSocket.instance.npcSendMsg -= NpcReply;
    }

    void Start()
    {
       // StartCoroutine(FetchResponseFromWeb());
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
            StartCoroutine(FetchResponseFromWeb());
    }

    IEnumerator FetchResponseFromWeb()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(3f, 7f));

        string prefix = "";
        int id = 0;
        int temp = UnityEngine.Random.Range(0, npcAttributes.Count);
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


        if (!APIBaseUrlChange.instance.IsXanaLive)
        {
            prefix = "http://182.70.242.10:8032/api/v1/text_from_userid_en_35?id=";
            id = npcAttributes[temp].aiIds;
        }
        else if (APIBaseUrlChange.instance.IsXanaLive)
        {
            prefix = "http://15.152.13.112:8032/api/v1/text_from_userid_en_35?id=";
            id = npcAttributes[temp].actualAiIds;
        }

        string url = prefix + id;
        Debug.Log("<color=red> Communication URL(NpcToNpc): " + url + "</color>");

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("NpctoNpc: " + request.downloadHandler.text);
            responseData = JsonUtility.FromJson<ResponseData>(request.downloadHandler.text);

            if (XanaChatSystem.instance)
                XanaChatSocket.onSendMsg?.Invoke(XanaConstants.xanaConstants.MuseumID, responseData.response, CallBy.NpcToNpc, id.ToString());
            Debug.Log("Communication Response(Npc Message)(NpcToNpc): " + responseData.response);
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
        yield return new WaitForSeconds(UnityEngine.Random.Range(3f, 7f));
        int id = 0;
        string prefix = "";
        if (!APIBaseUrlChange.instance.IsXanaLive)
        {
            prefix = "http://182.70.242.10:8032/api/v1/text_from_prompt_en?msg=";
            id = npcDB[npcCounter].aiIds;
        }
        else if (APIBaseUrlChange.instance.IsXanaLive)
        {
            prefix = "http://15.152.13.112:8032/api/v1/text_from_prompt_en?msg=";
            id = npcDB[npcCounter].actualAiIds;
        }

        string url = "&id=";
        string postUrl = prefix + msg + url + id;
        Debug.Log("<color=red> Communication URL(NpcToNpc): " + postUrl + "</color>");
        UnityWebRequest request = UnityWebRequest.Get(postUrl);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            feed = JsonUtility.FromJson<FeedData>(request.downloadHandler.text);

            if (XanaChatSystem.instance)
                XanaChatSocket.onSendMsg?.Invoke(XanaConstants.xanaConstants.MuseumID, feed.response, CallBy.FreeSpeechNpc, id.ToString());
            Debug.Log("Communication Response(Npc Reply)(NpcToNpc): " + feed.response);
        }
        else
            Debug.LogError("Communication API Error(NpcToNpc): " + gameObject.name + request.error);

        npcCounter++;
        Debug.Log("(NpcToNpc) Count: " + npcDB.Count);
        if (npcCounter < npcDB.Count)
            StartCoroutine(GetMsgResponse());
        else
        {
            npcCounter = 0;
            yield return new WaitForSeconds(UnityEngine.Random.Range(3f, 7f));
            StartCoroutine(FetchResponseFromWeb());
        }
    }

}


