using System.Collections;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;

public class NpcChatSystem : MonoBehaviour
{
    public enum ResponseChecker { CallAfterIterationEnd, InstantlyCall };
    public ResponseChecker responseChecker = ResponseChecker.CallAfterIterationEnd;
    public enum NameType { EnglishName, JapaneseName };

    [Serializable]
    public class NPCAttributes
    {
        public string aiNames;
        [Header("For test user")]
        public int aiIds;
        [Header("For Live user")]
        public int actualAiIds;
        public NameType nameType;
    }
    public List<NPCAttributes> npcAttributes;
    //[HideInInspector]
    public List<NPCAttributes> npcDB;
    public static Action<NpcChatSystem> npcNameAction;

    private int id = 0;
    private string msg = "Hello";
    private int numOfResponseWantToShow = 5;
    private int enNamePeriority = 3;
    private int jpNamePeriority = 2;
    private int totalFreeSpeechNpc = 5;
    private int counter = 0;
    private int tempResponseNum = 0;
    private const int userId = 0;
    private Queue<string> playerMessages = new Queue<string>();

    [System.Serializable]
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

    private void Awake()
    {
        //npcDB = new List<NPCAttributes>();
        //// my changes
        //List<NPCAttributes> japaneseNames = npcAttributes.FindAll(npc => npc.nameType == NameType.JapaneseName);
        //List<NPCAttributes> englishNames = npcAttributes.FindAll(npc => npc.nameType == NameType.EnglishName);
        //// select english name npc for chat
        //for (int i = 0; i < enNamePeriority; i++)
        //{
        //    int rand = UnityEngine.Random.Range(0, englishNames.Count);
        //    npcDB.Add(englishNames[rand]);      // Set npc for chat
        //    englishNames.RemoveAt(rand);
        //}
        //// select japanese name npc for chat
        //for (int i = 0; i < jpNamePeriority; i++)
        //{
        //    int rand = UnityEngine.Random.Range(0, japaneseNames.Count);
        //    npcDB.Add(japaneseNames[rand]);      // Set npc for chat
        //    japaneseNames.RemoveAt(rand);
        //}
        //// rest of them select for npc free speech
        //npcAttributes.Clear();
        //int temp = 0;
        //for (int i = 0; i < enNamePeriority + jpNamePeriority; i++)
        //{
        //    if (i < enNamePeriority)
        //        npcAttributes.Add(englishNames[i]);
        //    else
        //    {
        //        npcAttributes.Add(japaneseNames[temp]);
        //        temp++;
        //    }
        //}
        //englishNames.Clear();
        //japaneseNames.Clear();

        //ShuffleNpcs();     // shuffle selected user chat npc 
        //ShuffleFreeNpcs(); // shuffle free speech selected user chat npc 
        //numOfResponseWantToShow = enNamePeriority + jpNamePeriority;
        //// my changes end

        //tempResponseNum = numOfResponseWantToShow;
        //npcNameAction?.Invoke(this);      // update npc model name according to npc chat name
        //BuilderEventManager.AfterWorldOffcialWorldsInatantiated+=InvokeNPCName;
    }

    void InvokeNPCName()
    {
       // npcNameAction?.Invoke(this);      // update npc model name according to npc chat name
    }   

    private void ShuffleNpcs()
    {
        int count = npcDB.Count;
        System.Random random = new System.Random();
        while (count > 1)
        {
            count--;
            int k = random.Next(count + 1);
            NPCAttributes value = npcDB[k];
            npcDB[k] = npcDB[count];
            npcDB[count] = value;
            ////Debug.Log("<color=red>User NPC: " + npcDB[count].aiNames + "</color>");
        }
    }
    private void ShuffleFreeNpcs()
    {
        int count = npcAttributes.Count;
        System.Random random = new System.Random();
        while (count > 1)
        {
            count--;
            int k = random.Next(count + 1);
            NPCAttributes value = npcAttributes[k];
            npcAttributes[k] = npcAttributes[count];
            npcAttributes[count] = value;
            ////Debug.Log("FreeSpeech NPC: " + npcAttributes[count].aiNames);
        }
    }
    private void OnEnable()
    {
        //if (XanaChatSystem.instance)
            //XanaChatSystem.instance.npcAlert += PlayerSendMsg;
    }

    private void OnDisable()
    {
        //if (XanaChatSystem.instance)
            //XanaChatSystem.instance.npcAlert -= PlayerSendMsg;
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
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.3f));
        if (counter is 0 && playerMessages.Count > 0)
            msg = playerMessages.Dequeue();

        string ip = "";
        if (!APIBasepointManager.instance.IsXanaLive)
        {
            id = npcDB[counter].aiIds;
            ip = "http://182.70.242.10:8034/";
        }
        else if (APIBasepointManager.instance.IsXanaLive)
        {
            id = npcDB[counter].actualAiIds;
            ip = "http://15.152.55.82:8054/";
        }
        counter++;

        string prefix = ip + "api/v2/text_from_usertext_en_35?user_id=";
        string targetData = "&target_id=";
        string messageData = "&msg=";
        string postUrl = prefix + id + targetData + userId + messageData + msg;
        //Debug.Log("<color=red> Communication URL(UserAI): " + postUrl + "</color>");

        UnityWebRequest request = UnityWebRequest.Get(postUrl);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            feed = JsonUtility.FromJson<FeedData>(request.downloadHandler.text);
            InputData inputD = feed.input_data;
            OutputData outputD = feed.output_data;

            string responseFeed = "";
            if (LocalizationManager.forceJapanese || GameManager.currentLanguage == "ja")
                responseFeed = feed.output_data.user_msg_jp;
            else
                responseFeed = feed.output_data.user_msg_en;

            if (XanaChatSystem.instance)
            {
                ChatSocketManager.onSendMsg?.Invoke(ConstantsHolder.xanaConstants.MuseumID, responseFeed, CallBy.UserNpc, id.ToString());
                //Debug.Log("Communication Response(UserAI): " + responseFeed);

                if (NpcSpawner.npcSpawner)
                    NpcSpawner.npcSpawner.npcModel[counter - 1].GetComponent<NPC.NpcChatBillboard>().ShowNpcMessage(responseFeed);
            }
            else
                //Debug.LogError("Communication API Error(UserAI): " + gameObject.name + request.error);

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
                ShuffleNpcs();
            }
            yield return null;
        }

    }
}



