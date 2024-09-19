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
    private XanaChatSystem _CommonChatRef;
    public ChatSocketManager ChatSocketManagerInstance;
    [Header("This Class variables")]
    public XANASummitDataContainer XANASummitDataContainer;

    private List<GameObject> aiNPC = new List<GameObject>();

    private string npcName;
    private string npcURL;
    private bool _NPCInstantiated;
    private bool SummitNPC;
    private bool ChatActivated;

    public static int NPCCount=0;
    private void OnEnable()
    {
        NPCCount = 0;
        _CommonChatRef = LandscapeChatRef;
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

    void UpdateChatInstance(bool IsPortrait)
    {
        if (IsPortrait)
            _CommonChatRef = PortraitChatRef;
        else
            _CommonChatRef = PortraitChatRef;

    }

    private void Start()
    {
        //Dont remove this start method Intentionally added to hide inhertied start function
    }

    void LoadNPC()
    {
        if (ConstantsHolder.isFromXANASummit && !_NPCInstantiated)
        {
            _NPCInstantiated = true;
            SummitNPC = false;
            GetNPCDATA(ConstantsHolder.domeId);
        }

        if (WorldItemView.m_EnvName == "XANA Summit")
        {
            SummitNPC = true;
            _NPCInstantiated = true;
            GetNPCDATA(170);
        }
    }

    async void GetNPCDATA(int domeId)
    {
        bool flag = await XANASummitDataContainer.GetAIData(domeId);
        if (flag && SummitNPC)
            InstantiateSummitAINPC();
        else if (flag)
            InstantiateAINPC();
    }

    void InstantiateAINPC()
    {
        NPCCount = 0;
        for (int i = 0; i < XANASummitDataContainer.aiData.npcData.Count; i++)
        {
            GameObject AINPCAvatar;
            if (XANASummitDataContainer.aiData.npcData[i].avatarId > 10)
                AINPCAvatar = Instantiate(XANASummitDataContainer.femaleAIAvatar);
            else
                AINPCAvatar = Instantiate(XANASummitDataContainer.maleAIAvatar);

            aiNPC.Add(AINPCAvatar);
            AINPCAvatar.transform.position = new Vector3(XANASummitDataContainer.aiData.npcData[i].spawnPositionArray[0], XANASummitDataContainer.aiData.npcData[i].spawnPositionArray[1], XANASummitDataContainer.aiData.npcData[i].spawnPositionArray[2]);
            AINPCAvatar.name = XANASummitDataContainer.aiData.npcData[i].name;
            AINPCAvatar.GetComponent<SummitNPCAssetLoader>().npcName.text = XANASummitDataContainer.aiData.npcData[i].name;
            int avatarPresetId = XANASummitDataContainer.aiData.npcData[i].avatarId;
            //AINPCAvatar.GetComponent<SummitNPCAssetLoader>().json = XANASummitDataContainer.avatarJson[avatarPresetId - 1];
            AINPCAvatar.GetComponent<SummitNPCAssetLoader>().json = XANASummitDataContainer.aiData.npcData[i].avatarCategory;
            AINPCAvatar.GetComponent<SummitNPCAssetLoader>().Init();
            AINPCAvatar.GetComponent<AINPCTrigger>().npcID = XANASummitDataContainer.aiData.npcData[i].id;
            NPCCount++;
        }

        try
        {
            ReferencesForGamePlay.instance.SetPlayerCounter();
        }
        catch(Exception e)
        {

        }
    }

    void InstantiateSummitAINPC()
    {
        SummitNPC = false;
        for (int i = 0; i < XANASummitDataContainer.aiData.npcData.Count; i++)
        {
            GameObject AINPCAvatar;
            AINPCAvatar = Instantiate(XANASummitDataContainer.penguinAvatar);
            aiNPC.Add(AINPCAvatar);
            AINPCAvatar.transform.position = new Vector3(XANASummitDataContainer.aiData.npcData[i].spawnPositionArray[0], XANASummitDataContainer.aiData.npcData[i].spawnPositionArray[1], XANASummitDataContainer.aiData.npcData[i].spawnPositionArray[2]);
            AINPCAvatar.name = XANASummitDataContainer.aiData.npcData[i].name;
            AINPCAvatar.GetComponent<SetPenguinAIName>().NameText.text = XANASummitDataContainer.aiData.npcData[i].name;
            int avatarPresetId = XANASummitDataContainer.aiData.npcData[i].avatarId;
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
        ChatActivated = true;
        //foreach (string msg in welcomeMsgs)
           // _CommonChatRef.DisplayMsg_FromSocket(npcName, msg);
    }

    void RemoveAIChat(int npcId)
    {
        CloseChatBox();
        ClearOldMessages();
        RemoveAIListenerFromChatField();
        _CommonChatRef.LoadOldChat();
        ChatActivated = false;
    }

    void ClearOldMessages()
    {
        ChatSocketManagerInstance.ClearAllMessages();
        //_CommonChatRef.CurrentChannelText.text = string.Empty;
        //_CommonChatRef.PotriatCurrentChannelText.text = string.Empty;
    }

    void ClearInputField()
    {
        _CommonChatRef.InputFieldChat.text = "";
    }

    void OpenChatBox()
    {
        _CommonChatRef.isChatOpen = false;   //it will always open chat panel if collider with NPC
        _CommonChatRef.OpenCloseChatDialog();
        //_CommonChatRef.chatDialogBox.SetActive(true);
        //_CommonChatRef.chatNotificationIcon.SetActive(false);
        //_CommonChatRef.chatButton.GetComponent<Image>().enabled = true;
    }

    void CloseChatBox()
    {
        _CommonChatRef.isChatOpen = true;  //it will always close chat panel
        _CommonChatRef.OpenCloseChatDialog();
        //_CommonChatRef.chatDialogBox.SetActive(false);
        //_CommonChatRef.chatNotificationIcon.SetActive(false);
        //_CommonChatRef.chatButton.GetComponent<Image>().enabled = false;
    }


    void AddAIListenerOnChatField()
    {
        _CommonChatRef.InputFieldChat.onSubmit.RemoveAllListeners();
        _CommonChatRef.InputFieldChat.onSubmit.AddListener(SendMessageFromAI);
    }

    void RemoveAIListenerFromChatField()
    {
        _CommonChatRef.InputFieldChat.onSubmit.RemoveAllListeners();
        _CommonChatRef.InputFieldChat.onSubmit.AddListener(_CommonChatRef.OnEnterSend);
    }

    async void SendMessageFromAI(string s)
    {
        //_CommonChatRef.DisplayMsg_FromSocket(ConstantsHolder.userName, _CommonChatRef.InputFieldChat.text);
        ChatSocketManagerInstance.AddNewMsg(ConstantsHolder.userName, _CommonChatRef.InputFieldChat.text, "NPC", "NPC", 0);
        string url = npcURL + "&usr_id=" + ConstantsHolder.userId + "&input_string=" + _CommonChatRef.InputFieldChat.text;

        ClearInputField();

        UriBuilder uriBuilder = new UriBuilder(url);

        string response = await GetAIResponse(url);
        if(ChatActivated)
        {
            string res = JsonUtility.FromJson<AIResponse>(response).data;

            //_CommonChatRef.DisplayMsg_FromSocket(npcName, res);
            ChatSocketManagerInstance.AddNewMsg(npcName, res, "NPC", "NPC", 0);
        }
        
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
        for (int i = 0; i < aiNPC.Count; i++)
        {
            if (aiNPC[i] != null)
                Destroy(aiNPC[i]);
        }

        aiNPC.Clear();
        _NPCInstantiated = false;
    }


    [System.Serializable]
    public class AIResponse
    {
        public string data;
    }

}
