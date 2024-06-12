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
        BuilderEventManager.AINPCDeactivated += RemoveAIChat;
        BuilderEventManager.AfterPlayerInstantiated += LoadNPC;
    }
    private void OnDisable()
    {
        BuilderEventManager.AINPCActivated -= LoadAIChat;
        BuilderEventManager.AINPCDeactivated -= RemoveAIChat;
        BuilderEventManager.AfterPlayerInstantiated -= LoadNPC;
    }

    void LoadNPC()
    {
        if (ConstantsHolder.isFromXANASummit)
            GetNPCDATA(ConstantsHolder.domeId);
    }

    async void GetNPCDATA(int domeId)
    {
        bool flag = await XANASummitDataContainer.GetAIData(domeId);
        if (flag)
            InstantiateAINPC();
    }

    void InstantiateAINPC()
    {
        for (int i = 0; i < XANASummitDataContainer.aiData.npcData.Count; i++)
        {
            GameObject AINPCAvatar;
            if (XANASummitDataContainer.aiData.npcData[i].avatarId>10)
                AINPCAvatar = Instantiate(XANASummitDataContainer.femaleAIAvatar);
            else
                AINPCAvatar = Instantiate(XANASummitDataContainer.maleAIAvatar);


            AINPCAvatar.transform.position = new Vector3(XANASummitDataContainer.aiData.npcData[i].spawnPositionArray[0], XANASummitDataContainer.aiData.npcData[i].spawnPositionArray[1], XANASummitDataContainer.aiData.npcData[i].spawnPositionArray[2]);
            AINPCAvatar.name = XANASummitDataContainer.aiData.npcData[i].name;
            AINPCAvatar.GetComponent<SummitNPCAssetLoader>().npcName.text = XANASummitDataContainer.aiData.npcData[i].name;
            int avatarPresetId= XANASummitDataContainer.aiData.npcData[i].avatarId;
            AINPCAvatar.GetComponent<SummitNPCAssetLoader>().json = XANASummitDataContainer.avatarJson[avatarPresetId-1];
            AINPCAvatar.GetComponent<AINPCTrigger>().npcID = XANASummitDataContainer.aiData.npcData[i].id;
        }
    }

    void GetNPCInfo(int npcID)
    {
        for (int i = 0; i < XANASummitDataContainer.aiData.npcData.Count; i++)
        {
            if (XANASummitDataContainer.aiData.npcData[i].id == npcID)
            {
                npcName = XANASummitDataContainer.aiData.npcData[i].name;
                npcURL = XANASummitDataContainer.aiData.npcData[i].personalityURL;
            }
        }
    }

    void LoadAIChat(int npcID, string[] welcomeMsgs)
    {
        AddAIListenerOnChatField();
        GetNPCInfo(npcID);
        ClearOldMessages();
        OpenChatBox();
        foreach (string msg in welcomeMsgs)
            DisplayMsg_FromSocket(npcName, msg);
    }

    void RemoveAIChat(int npcId)
    {
        ClearOldMessages();
        RemoveAIListenerFromChatField();
        LoadOldChat();
    }

    void ClearOldMessages()
    {
        CurrentChannelText.text = string.Empty;
        PotriatCurrentChannelText.text = string.Empty;
    }

    void ClearInputField()
    {
        InputFieldChat.text = "";
        InputFieldChat.Select();
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

        string url = npcURL + "&usr_id="+ConstantsHolder.userId + "&input_string=" + InputFieldChat.text;

        ClearInputField();

        Debug.LogError(url);

        UriBuilder uriBuilder = new UriBuilder(url);

        string response = await GetAIResponse(url);

        string res = JsonUtility.FromJson<AIResponse>(response).data;

        DisplayMsg_FromSocket(npcName, res);
    }

    async Task<string> GetAIResponse(string url)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        await www.SendWebRequest();
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
