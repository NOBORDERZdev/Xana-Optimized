using System.Collections;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;


public class NpcChatSystem : MonoBehaviour
{
    public enum ResponseChecker { CallAfterIterationEnd, InstantlyCall };
    public ResponseChecker responseChecker = ResponseChecker.CallAfterIterationEnd;

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

    private int id = 0;
    private string msg = "Hello";
    private int numOfResponseWantToShow = 5;
    private int counter = 0;
    private int tempResponseNum = 0;

    private Queue<string> playerMessages = new Queue<string>();

    private class FeedData
    {
        public int id;
        public string response;
    }
    private FeedData feed;

    private void Awake()
    {
        npcDB = new List<NPCAttributes>();
        for (int i = 0; i < numOfResponseWantToShow; i++)
        {
            int rand = UnityEngine.Random.Range(0, npcAttributes.Count);
            npcDB.Add(npcAttributes[rand]);      // Set npc names
            npcAttributes.RemoveAt(rand);
        }
        tempResponseNum = numOfResponseWantToShow;
    }

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

    private void PlayerSendMsg(string msgData)
    {
        if (counter != 0)
            responseChecker = ResponseChecker.InstantlyCall;

        /// <summary>
        /// Reset the data when new message come
        /// </summary>
        counter = 0;
        tempResponseNum = numOfResponseWantToShow;

        playerMessages.Enqueue(msgData);
        // Call the API request function
        StartCoroutine(SetApiData());
    }
    IEnumerator SetApiData()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 3f));
        if (counter is 0 && playerMessages.Count > 0)
            msg = playerMessages.Dequeue();

        //for live http://15.152.13.112:8032/
        //for test http://182.70.242.10:8032/
        string prefix = "";
        if (!APIBaseUrlChange.instance.IsXanaLive)
        {
            id = npcDB[counter].aiIds;
            prefix = "http://182.70.242.10:8032/api/v1/text_from_prompt_en?msg=";
        }
        else if (APIBaseUrlChange.instance.IsXanaLive)
        {
            id = npcDB[counter].actualAiIds;
            prefix = "http://15.152.13.112:8032/api/v1/text_from_prompt_en?msg=";
        }
        counter++;

        string url = "&id=";
        string postUrl = prefix + msg + url + id;
        Debug.Log("<color=red> Communication URL(UserAI): " + postUrl + "</color>");
        UnityWebRequest request = UnityWebRequest.Get(postUrl);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            feed = JsonUtility.FromJson<FeedData>(request.downloadHandler.text);

            if (XanaChatSystem.instance)
                XanaChatSocket.onSendMsg?.Invoke(XanaConstants.xanaConstants.MuseumID, feed.response, id.ToString());
            Debug.Log("Communication Response(UserAI): " + feed.response);
        }
        else
            Debug.LogError("Communication API Error(UserAI): " + gameObject.name + request.error);

        tempResponseNum--;
        if (tempResponseNum > 0)
        {
            if (responseChecker.Equals(ResponseChecker.CallAfterIterationEnd))
                StartCoroutine(SetApiData());
            else if (responseChecker.Equals(ResponseChecker.InstantlyCall))
                responseChecker = ResponseChecker.CallAfterIterationEnd;
        }
        else
        {
            counter = 0;
            tempResponseNum = numOfResponseWantToShow;
        }
        yield return null;
    }

}



