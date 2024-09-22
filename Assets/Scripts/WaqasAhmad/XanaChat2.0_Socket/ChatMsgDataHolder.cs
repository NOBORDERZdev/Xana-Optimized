using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChatMsgDataHolder : MonoBehaviour
{
    
    public TextMeshProUGUI MsgText;
    public GameObject HighLighter;
    public GameObject DotedBtn;
    
    public Image FlagBtn;
    public Sprite FlagActive, FlagInactive;
    public GameObject FlagLoader;

    public Image BlockBtn;
    public Sprite BlockActive, BlockInactive;
    public GameObject BlockLoader;

    int _MsgID;
    string _MsgTextString;
    string _SenderUserID;


    bool BtnActiveStatus = false;

    private void OnEnable()
    {
        BtnForcedStatus(false);
    }


    public void SetRequireData(string msgText, int msgId, string senderUserId, int isMessageBlocked)
    {
       _MsgID = msgId;
       _MsgTextString = msgText;   
       _SenderUserID = senderUserId;

        if(senderUserId.Equals(ConstantsHolder.userId))
            DotedBtn.SetActive(false);
        else
            DotedBtn.SetActive(true);


        if (isMessageBlocked == 1)
        {
            OnFlagUserApiCompleted(true);
        }
    }

    public void FillCellData(ChatUserData _MyData)
    {
        SetRequireData(_MyData.message, _MyData.message_id, _MyData.userId, _MyData.isMessageBlocked);

        string senderName = "";
        if (_MyData.isGuest)
            senderName = _MyData.guestusername;
        else
        {
            senderName = _MyData.name; 
            if(string.IsNullOrEmpty(senderName))
                senderName = _MyData.username;
        }

        if(string.IsNullOrEmpty(senderName))
            senderName = "XanaUser";
        ShowMsg(senderName, _MyData.message);
    }

    void ShowMsg(string senderName, string msg)
    {
        if (senderName.Length > 12)
        {
            MsgText.text = "<b>" + senderName.Substring(0, 12) + "...</b>" + " : " + msg;
        }
        else
        {
            MsgText.text = "<b>" + senderName + "</b>" + " : " + msg;
        }

        //if (!chatDialogBox.activeSelf && senderName != UserName)
        //{
        //    chatNotificationIcon.SetActive(true);
        //}
    }


    public void BtnForcedStatus(bool status)
    {
        FlagBtn.gameObject.SetActive(status);
        BlockBtn.gameObject.SetActive(status);
        HighLighter.SetActive(status);

        FlagLoader.SetActive(false);
        BlockLoader.SetActive(false);

        BtnActiveStatus = status;
    }

    // Calling this function From Unity Button
    public void BtnStatus()
    {
        bool status = BtnActiveStatus; // Save Current Status
        ChatSocketManager.instance.DisableAllBtn();

        BtnActiveStatus = !status;
        FlagBtn.gameObject.SetActive(BtnActiveStatus);
        BlockBtn.gameObject.SetActive(BtnActiveStatus);
        HighLighter.SetActive(BtnActiveStatus);
    }



    public void BlockUser()
    {
        if (BlockLoader.activeInHierarchy)
            return;

        BlockLoader.SetActive(true);
        ChatSocketManager.instance.BlockUser(_SenderUserID, OnBlockUserApiCompleted);
    }
    void OnBlockUserApiCompleted(bool apiStatus)
    {
        BlockLoader.SetActive(false);
        if (apiStatus)
            BlockBtn.sprite = BlockActive;
        else
            BlockBtn.sprite = BlockInactive;
    }

    public void FlagMsg()
    {

        if (FlagLoader.activeInHierarchy)
            return;

        FlagLoader.SetActive(true);
        ChatSocketManager.instance.FlagMessages(_MsgID, OnFlagUserApiCompleted);
    }
    void OnFlagUserApiCompleted(bool apiStatus)
    {
        FlagLoader.SetActive(false);

        if (apiStatus)
            FlagBtn.sprite = FlagActive;
        else
            FlagBtn.sprite = FlagInactive;
    }
}