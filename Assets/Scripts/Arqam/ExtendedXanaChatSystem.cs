using Crosstales.BWF;
using UnityEngine;
using System;

public class ExtendedXanaChatSystem : XanaChatSystem
{
    public Action<string> AirinQuestion;

    private void OnEnable()
    {
        ChatSocketManager.onJoinRoom += UserJoinRoomAfterDisconnect;
    }
    private void OnDisable()
    {
        ChatSocketManager.onJoinRoom -= UserJoinRoomAfterDisconnect;
    }

    public void SendMessage(string msgData)
    {
        OnEnterSend();
    }

    public bool IsShowChatWindow()
    {
        if (isChatOpen) { return true; }
        else { return false; }
    }

    public void ShowMsgLocally(string senderName, string msgData)
    {
        this.CurrentChannelText.text = "<b>" + senderName + " : " + "</b>" + msgData + "\n" + this.CurrentChannelText.text;
        this.PotriatCurrentChannelText.text = "<b>" + senderName + " : " + "</b>" + msgData + "\n" + this.PotriatCurrentChannelText.text;
    }

    public void ShowAirinMsg(string senderName, string msgData, bool airinTyping)
    {
        if (!airinTyping)
        {
            ShowMsgLocally(senderName, msgData);
        }
        else if (airinTyping)
        {
            // Split the text into lines
            string[] lines = CurrentChannelText.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

            // Replace the first line with the AI response
            if (lines.Length > 0)
            {
                lines[0] = "<b>" + senderName + " : " + "</b>" + msgData;
            }
            else
            {
                // If there are no lines, just set the text to the AI response
                lines = new string[] { "<b>" + senderName + " : " + "</b>" + msgData };
            }

            this.CurrentChannelText.text = string.Join("\n", lines);
            this.PotriatCurrentChannelText.text = string.Join("\n", lines);
        }
    }

    public void ClearAirinTypingMsg()
    {
        string[] lines = CurrentChannelText.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length > 0)
        {
            if (lines[0] == "Airin : typing...") {
                lines[0] = "";
                this.CurrentChannelText.text = string.Join("", lines);
                this.PotriatCurrentChannelText.text = string.Join("", lines);
            }
        }
    }

    private void OnEnterSend()
    {
        string removeBadWords = string.IsNullOrEmpty(InputFieldChat.text) ? "" : BWFManager.Instance.ReplaceAll(InputFieldChat.text);
        if (!UserPassManager.Instance.CheckSpecificItem("Message Option/Chat option"))
        {
            this.InputFieldChat.text = "";
            removeBadWords = "";
            return;
        }

        // When User Activated the Airin for conversation
        ShowMsgLocally(UserName, removeBadWords);
        AirinQuestion?.Invoke(removeBadWords);

        this.InputFieldChat.text = "";
        removeBadWords = "";
    }

    private void UserJoinRoomAfterDisconnect(string _worldId)
    {
        if (IsShowChatWindow())
        {
            XanaChatSystem.instance.OpenCloseChatDialog();
        }
    }


}
