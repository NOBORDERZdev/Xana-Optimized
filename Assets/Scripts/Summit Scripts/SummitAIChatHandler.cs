using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SummitAIChatHandler : XanaChatSystem
{
    [Header("Base Class variables ↑")]

    [Header("This Class variables")]
    public XANASummitDataContainer XANASummitDataContainer;

    private string npcName;
    private string npcURL;

    private void OnEnable()
    {
        BuilderEventManager.AINPCActivated += LoadAIChat;
        BuilderEventManager.AfterPlayerInstantiated += LoadNPC;
        //npcURL = "http://182.70.242.10:8042/npc-chat?input_string=a&npc_id=21id00&personality_id=21personalityId00&usr_id=sadasddsasd&personality_name=asd";
    }
    private void OnDisable()
    {
        BuilderEventManager.AINPCActivated -= LoadAIChat;
        BuilderEventManager.AfterPlayerInstantiated -= LoadNPC;
    }

    void LoadNPC()
    {
        if (ConstantsHolder.isFromXANASummit)
            GetNPCDATA("6");
    }

    async void GetNPCDATA(string domeId)
    {
        bool flag = await XANASummitDataContainer.GetAIData(domeId);
        if (flag)
            InstantiateAINPC();
    }

    void InstantiateAINPC()
    {
        for (int i = 0; i < XANASummitDataContainer.aiData.root.Count; i++)
        {
            GameObject AIPrefab = Resources.Load("SummitAINPC") as GameObject;
            GameObject AINPCAvatar = Instantiate(AIPrefab);
            AINPCAvatar.transform.position = new Vector3(XANASummitDataContainer.aiData.root[i].spawnPosition[0], XANASummitDataContainer.aiData.root[i].spawnPosition[1], XANASummitDataContainer.aiData.root[i].spawnPosition[2]);
            AINPCAvatar.name = XANASummitDataContainer.aiData.root[i].name;
            AINPCAvatar.GetComponent<SummitNPCAssetLoader>().npcName.text = XANASummitDataContainer.aiData.root[i].name;
            AINPCAvatar.GetComponent<SummitNPCAssetLoader>().json = XANASummitDataContainer.aiData.root[i].avatarCategory;
            AINPCAvatar.GetComponent<AINPCTrigger>().npcID = XANASummitDataContainer.aiData.root[i].id;
        }
    }

    void GetNPCInfo(string npcID)
    {
        for (int i = 0; i < XANASummitDataContainer.aiData.root.Count; i++)
        {
            if (XANASummitDataContainer.aiData.root[i].id.ToString() == npcID)
            {
                npcName = XANASummitDataContainer.aiData.root[i].name;
                npcURL = XANASummitDataContainer.aiData.root[i].personalityURL;
            }
        }
    }

    void LoadAIChat(string npcID, string[] welcomeMsgs)
    {
        AddAIListenerOnChatField();
        GetNPCInfo(npcID);
        ClearOldMessages();
        OpenChatBox();
        foreach (string msg in welcomeMsgs)
            DisplayMsg_FromSocket(npcName, msg);
    }

    void ClearOldMessages()
    {
        CurrentChannelText.text = string.Empty;
        PotriatCurrentChannelText.text = string.Empty;
    }

    void OpenChatBox()
    {
        chatDialogBox.SetActive(true);
        chatNotificationIcon.SetActive(false);
        chatButton.GetComponent<Image>().enabled = true;
    }

    void AddAIListenerOnChatField()
    {
        InputFieldChat.onSubmit.RemoveAllListeners();
        InputFieldChat.onSubmit.AddListener(SendMessageFromAI);
    }

    void RemoveAIListenerFromChatField()
    {
        InputFieldChat.onSubmit.RemoveAllListeners();
        InputFieldChat.onSubmit.AddListener(OnEnterSend);
    }

    async void SendMessageFromAI(string s)
    {
        DisplayMsg_FromSocket(ConstantsHolder.userName, InputFieldChat.text);

        string url = npcURL + "&input_string=" + InputFieldChat.text;

        string response = await GetAIResponse(url);

        string res = JsonUtility.FromJson<AIResponse>(response).data;

        DisplayMsg_FromSocket(npcName, res);
    }

    async Task<string> GetAIResponse(string url)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SendWebRequest();
        while (!www.isDone)
            await System.Threading.Tasks.Task.Yield();

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            return www.error;
        }
        else
            return www.downloadHandler.text;
    }

    [System.Serializable]
    public class AIResponse
    {
        public string data;
    }

}
