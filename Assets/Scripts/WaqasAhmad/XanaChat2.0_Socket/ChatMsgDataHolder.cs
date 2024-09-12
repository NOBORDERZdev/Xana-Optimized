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

    string _MsgID;
    string _MsgTextString;
    string _SenderUserID;


    bool BtnActiveStatus = false;

    private void OnEnable()
    {
        BtnForcedStatus(false);
    }


    public void SetRequireData(string msgText, string msgId, string senderUserId, int isMessageBlocked)
    {
       _MsgID = msgId;
       _MsgTextString = msgText;   
       _SenderUserID = senderUserId;

        if (isMessageBlocked == 1)
        {
            OnFlagUserApiCompleted(true);
        }
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