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

    private IEnumerator Start()
    {
        BuilderEventManager.AINPCActivated += LoadAIChat;
        npcURL = "http://182.70.242.10:8042/npc-chat?input_string=a&npc_id=21id00&personality_id=21personalityId00&usr_id=sadasddsasd&personality_name=asd";

        yield return new WaitForSeconds(5);
        npcName = "AI_1";
        LoadAIChat("AI_1",null);

    }
    private void OnDisable()
    {
        BuilderEventManager.AINPCActivated -= LoadAIChat;
    }

    void GetNPCInfo(string npcID)
    {
        for (int i = 0;i<XANASummitDataContainer.aiData.data.Count;i++)
        {
            if (XANASummitDataContainer.aiData.data[i].id.ToString()==npcID)
            {
                npcName = XANASummitDataContainer.aiData.data[i].name;
                npcURL= XANASummitDataContainer.aiData.data[i].personalityURL;
            }
        }
    }

    void LoadAIChat(string npcID, string[] welcomeMsgs)
    {
        AddAIListenerOnChatField();
        //GetNPCInfo(npcID);
        ClearOldMessages();
        OpenChatBox();
        //foreach (string msg in welcomeMsgs)
        //    DisplayMsg_FromSocket(npcName, msg);
    }

    void ClearOldMessages()
    {
        Debug.LogError("cleared msgs");
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

        string response=await GetAIResponse(url);

        string res=JsonUtility.FromJson<AIResponse>(response).data;

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
