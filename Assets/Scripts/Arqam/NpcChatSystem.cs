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
        public int aiIds;
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
        id = npcDB[counter].aiIds;
        counter++;

        string prefix = "http://182.70.242.10:8032/api/v1/";
        string url = "update_user_prompt_en?id=";
        //id = 2;
        string midPart = "&prompt=";
        //msg = "I am student";
        string postUrl = prefix + url + id + midPart + msg;

        UnityWebRequest request = UnityWebRequest.Post(postUrl, "POST");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        /// <summary>
        /// Get response from api and send that response to chat socket
        /// </summary>
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
            Debug.LogError("Communication API Error: " + gameObject.name + fetchRequest.error);

        tempResponseNum--;
        //prevCounter = counter;
        if (tempResponseNum > 0)
        {
            if (responseChecker.Equals(ResponseChecker.CallAfterIterationEnd)) //playerMessages.Count is 0 &&
                StartCoroutine(SetApiData());
            else if (responseChecker.Equals(ResponseChecker.InstantlyCall))
            {
                responseChecker = ResponseChecker.CallAfterIterationEnd;
                //counter = 0;
            }
        }
        else
        {
            counter = 0;
            tempResponseNum = numOfResponseWantToShow;
        }
        yield return null;
    }


}


