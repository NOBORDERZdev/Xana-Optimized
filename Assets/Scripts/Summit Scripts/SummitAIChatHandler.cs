using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SummitAIChatHandler : MonoBehaviour
{
    [Header("Base Class variables ↑")]

    public XanaChatSystem LandscapeChatRef;
    public XanaChatSystem PortraitChatRef;
    private XanaChatSystem CommonChatRef;

    [Header("This Class variables")]
    public XANASummitDataContainer XANASummitDataContainer;

    private List<GameObject> aiNPC=new List<GameObject>();

    private string npcName;
    private string npcURL;

    private void OnEnable()
    {
        CommonChatRef = LandscapeChatRef;
        BuilderEventManager.AINPCActivated += LoadAIChat;
        BuilderEventManager.AINPCDeactivated += RemoveAIChat;
        BuilderEventManager.AfterPlayerInstantiated += LoadNPC;
        BuilderEventManager.ResetSummit += ResetOnExit;
        ScreenOrientationManager.switchOrientation += UpdateChatInstance;
    }
    private void OnDisable()
    {
        BuilderEventManager.AINPCActivated -= LoadAIChat;
        BuilderEventManager.AINPCDeactivated -= RemoveAIChat;
        BuilderEventManager.AfterPlayerInstantiated -= LoadNPC;
        BuilderEventManager.ResetSummit -= ResetOnExit;
        ScreenOrientationManager.switchOrientation -= UpdateChatInstance;
    }

    void UpdateChatInstance(bool _isPortrait)
    {
        if(_isPortrait)
            CommonChatRef = PortraitChatRef;
        else
            CommonChatRef = PortraitChatRef;

    }

    private void Start()
    {
        //Dont remove this start method Intentionally added to hide inhertied start function
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

            aiNPC.Add(AINPCAvatar);
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
            CommonChatRef.DisplayMsg_FromSocket(npcName, msg);
    }

    void RemoveAIChat(int npcId)
    {
        ClearOldMessages();
        RemoveAIListenerFromChatField();
        CommonChatRef.LoadOldChat();
    }

    void ClearOldMessages()
    {
        CommonChatRef.CurrentChannelText.text = string.Empty;
        CommonChatRef.PotriatCurrentChannelText.text = string.Empty;
    }

    void ClearInputField()
    {
        CommonChatRef.InputFieldChat.text = "";
    }

    void OpenChatBox()
    {
        CommonChatRef.chatDialogBox.SetActive(true);
        CommonChatRef.chatNotificationIcon.SetActive(false);
        CommonChatRef.chatButton.GetComponent<Image>().enabled = true;
    }

    void CloseChatBox()
    {
        CommonChatRef.chatDialogBox.SetActive(false);
        CommonChatRef.chatNotificationIcon.SetActive(false);
        CommonChatRef.chatButton.GetComponent<Image>().enabled = false;
    }


    void AddAIListenerOnChatField()
    {
        CommonChatRef.InputFieldChat.onSubmit.RemoveAllListeners();
        CommonChatRef.InputFieldChat.onSubmit.AddListener(SendMessageFromAI);
    }

    void RemoveAIListenerFromChatField()
    {
        CommonChatRef.InputFieldChat.onSubmit.RemoveAllListeners();
        CommonChatRef.InputFieldChat.onSubmit.AddListener(CommonChatRef.OnEnterSend);
    }

    async void SendMessageFromAI(string s)
    {
        CommonChatRef.DisplayMsg_FromSocket(ConstantsHolder.userName, CommonChatRef.InputFieldChat.text);

        string url = npcURL + "&usr_id="+ConstantsHolder.userId + "&input_string=" + CommonChatRef.InputFieldChat.text;

        ClearInputField();

        Debug.LogError(url);

        UriBuilder uriBuilder = new UriBuilder(url);

        string response = await GetAIResponse(url);

        string res = JsonUtility.FromJson<AIResponse>(response).data;

        CommonChatRef.DisplayMsg_FromSocket(npcName, res);
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


    void ResetOnExit()
    {
        ClearInputField();
        CloseChatBox();
        DestroyNPC();
    }

    void DestroyNPC()
    {
        for(int i = 0;i<aiNPC.Count;i++)
        {
            if (aiNPC[i] != null)
                Destroy(aiNPC[i]);
        }

        aiNPC.Clear();
    }


    [System.Serializable]
    public class AIResponse
    {
        public string data;
    }

}
