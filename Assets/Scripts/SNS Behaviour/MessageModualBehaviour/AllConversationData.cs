using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using SuperStar.Helpers;
using System.IO;
using UnityEngine.EventSystems;

public class AllConversationData : MonoBehaviour
{
    public ChatGetConversationDatum allChatGetConversationDatum;

    public TextMeshProUGUI textTitle;
    public TextMeshProUGUI textLastMessage;
    //public Text textLastMessage;
    public TextMeshProUGUI textTime;

    public GameObject UnReadCountobject;
    public TextMeshProUGUI UnreadMessageText;

    public GameObject muteObject;

    public Image profileImage;

    public Image backgroundImage;
    public Color unreadColor;

    public List<string> groupName = new List<string>();

    public Sprite defaultSP;

    public Sprite circleSP, whiteCircleSP;

    public Sprite squareSP, whiteSquareSP;

    public Image profileBGImg;

    public string avatarUrl;

    ScrollRect currentScrollRect;

    private void Awake()
    {
        defaultSP = profileImage.sprite;

        currentScrollRect = MessageController.Instance.searchManagerAllConversation.mainScrollView.GetComponent<ScrollRect>();
        if (this.transform.parent.name== "searchContainer")
        {
            currentScrollRect = MessageController.Instance.searchManagerAllConversation.searchScrollView.GetComponent<ScrollRect>();
        }

        EventTrigger trigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entryBegin = new EventTrigger.Entry(), entryDrag = new EventTrigger.Entry(), entryEnd = new EventTrigger.Entry(), entrypotential = new EventTrigger.Entry()
            , entryScroll = new EventTrigger.Entry();

        entryBegin.eventID = EventTriggerType.BeginDrag;
        entryBegin.callback.AddListener((data) => 
        {
            pressed = false;
            currentScrollRect.OnBeginDrag((PointerEventData)data); 
        });
        trigger.triggers.Add(entryBegin);

        entryDrag.eventID = EventTriggerType.Drag;
        entryDrag.callback.AddListener((data) => 
        {
            pressed = false;
            currentScrollRect.OnDrag((PointerEventData)data); 
        });
        trigger.triggers.Add(entryDrag);

        entryEnd.eventID = EventTriggerType.EndDrag;
        entryEnd.callback.AddListener((data) => 
        {
            currentScrollRect.OnEndDrag((PointerEventData)data); 
        });
        trigger.triggers.Add(entryEnd);

        entrypotential.eventID = EventTriggerType.InitializePotentialDrag;
        entrypotential.callback.AddListener((data) => 
        {
            currentScrollRect.OnInitializePotentialDrag((PointerEventData)data); 
        });
        trigger.triggers.Add(entrypotential);

        entryScroll.eventID = EventTriggerType.Scroll;
        entryScroll.callback.AddListener((data) => 
        {
            currentScrollRect.OnScroll((PointerEventData)data); 
        });
        trigger.triggers.Add(entryScroll);
    }

    public int cnt = 0;
    private void OnEnable()
    {
        if (defaultSP != null)
        {
            profileImage.sprite = defaultSP;
        }
        if (cnt > 0 && !string.IsNullOrEmpty(avatarUrl))
        {
            if (AssetCache.Instance.HasFile(avatarUrl))
            {
                AssetCache.Instance.LoadSpriteIntoImage(profileImage, avatarUrl, changeAspectRatio: true);
            }
        }
        else
        {
            cnt += 1;
        }
    }

    private void OnDisable()
    {
        if (!string.IsNullOrEmpty(avatarUrl))
        {
            AssetCache.Instance.RemoveFromMemory(profileImage.sprite);
            profileImage.sprite = null;
            //Resources.UnloadUnusedAssets();//every clear.......
            //Caching.ClearCache();
            APIManager.Instance.ResourcesUnloadAssetFile();//UnloadUnusedAssets file call every 15 items.......
        }
    }

    public void LoadFeed()
    {
        if (allChatGetConversationDatum.group != null && allChatGetConversationDatum.receivedGroupId != 0)
        {
            profileBGImg.sprite = whiteSquareSP;
            defaultSP = squareSP;
            profileImage.sprite = defaultSP;

            //Debug.LogError("ConReciver:" + allChatGetConversationDatum.group.name);
            if (!string.IsNullOrEmpty(allChatGetConversationDatum.group.name))
            {
                textTitle.text = allChatGetConversationDatum.group.name;
            }

            if (!string.IsNullOrEmpty(allChatGetConversationDatum.group.avatar))//rik Get and set Avatar From AWS.......
            {
                avatarUrl = allChatGetConversationDatum.group.avatar;
                bool isUrlContainsHttpAndHttps = APIManager.Instance.CheckUrlDropboxOrNot(allChatGetConversationDatum.group.avatar);
                if (isUrlContainsHttpAndHttps)
                {
                    AssetCache.Instance.EnqueueOneResAndWait(allChatGetConversationDatum.group.avatar, allChatGetConversationDatum.group.avatar, (success) =>
                    {
                        if (success)
                        {
                            AssetCache.Instance.LoadSpriteIntoImage(profileImage, allChatGetConversationDatum.group.avatar, changeAspectRatio: true);
                        }
                    });
                }
                else
                {
                    GetImageFromAWS(allChatGetConversationDatum.group.avatar, profileImage);
                }
            }
        }
        else
        {
            profileBGImg.sprite = whiteCircleSP;
            defaultSP = circleSP;
            profileImage.sprite = defaultSP;

            if (allChatGetConversationDatum.ConReceiver != null && allChatGetConversationDatum.ConReceiver.id == APIManager.Instance.userId)
            {
                //Debug.LogError("ConReciver:" + allChatGetConversationDatum.ConSender.name);
                if (!string.IsNullOrEmpty(allChatGetConversationDatum.ConSender.name))
                {
                    textTitle.text = allChatGetConversationDatum.ConSender.name;
                }
                else
                {
                    textTitle.text = allChatGetConversationDatum.ConSender.email;
                }
                if (!string.IsNullOrEmpty(allChatGetConversationDatum.ConSender.avatar))
                {
                    avatarUrl = allChatGetConversationDatum.ConSender.avatar;
                    //Debug.LogError("AllConversation ConSender:" + allChatGetConversationDatum.ConSender.name);
                    bool isUrlContainsHttpAndHttps = APIManager.Instance.CheckUrlDropboxOrNot(allChatGetConversationDatum.ConSender.avatar);
                    if (isUrlContainsHttpAndHttps)
                    {
                        AssetCache.Instance.EnqueueOneResAndWait(allChatGetConversationDatum.ConSender.avatar, allChatGetConversationDatum.ConSender.avatar, (success) =>
                        {
                            if (success)
                            {
                                AssetCache.Instance.LoadSpriteIntoImage(profileImage, allChatGetConversationDatum.ConSender.avatar, changeAspectRatio: true);
                            }
                        });
                    }
                    else
                    {
                        GetImageFromAWS(allChatGetConversationDatum.ConSender.avatar, profileImage);
                    }
                }
            }
            else
            {
                //Debug.LogError("ConReciver:" + allChatGetConversationDatum.ConReceiver.name);
                if (!string.IsNullOrEmpty(allChatGetConversationDatum.ConReceiver.name))
                {
                    textTitle.text = allChatGetConversationDatum.ConReceiver.name;
                }
                else
                {
                    textTitle.text = allChatGetConversationDatum.ConReceiver.email;
                }
                if (!string.IsNullOrEmpty(allChatGetConversationDatum.ConReceiver.avatar))
                {
                    avatarUrl = allChatGetConversationDatum.ConReceiver.avatar;
                    //Debug.LogError("AllConversation ConR1:" + allChatGetConversationDatum.ConReceiver.name);
                    bool isUrlContainsHttpAndHttps = APIManager.Instance.CheckUrlDropboxOrNot(allChatGetConversationDatum.ConReceiver.avatar);
                    if (isUrlContainsHttpAndHttps)
                    {
                        AssetCache.Instance.EnqueueOneResAndWait(allChatGetConversationDatum.ConReceiver.avatar, allChatGetConversationDatum.ConReceiver.avatar, (success) =>
                        {
                            if (success)
                            {
                                AssetCache.Instance.LoadSpriteIntoImage(profileImage, allChatGetConversationDatum.ConReceiver.avatar, changeAspectRatio: true);
                            }
                        });
                    }
                    else
                    {
                        GetImageFromAWS(allChatGetConversationDatum.ConReceiver.avatar, profileImage);
                    }
                }
            }
        }
        if (!string.IsNullOrEmpty(allChatGetConversationDatum.lastMsg))
        {
            //allChatGetConversationDatum.lastMsg = APIManager.DecodedString(allChatGetConversationDatum.lastMsg);
            string lastMSG = APIManager.DecodedString(allChatGetConversationDatum.lastMsg);
            //Debug.LogError("LastMSG:" + lastMSG + " :DataLastMSG:" + allChatGetConversationDatum.lastMsg);
            if (lastMSG.Contains("Left"))
            {
                textLastMessage.text = lastMSG.Replace("Left", TextLocalization.GetLocaliseTextByKey("Left"));
            }
            else if(lastMSG.Contains("attachments"))
            {
                textLastMessage.text = lastMSG.Replace("attachments", TextLocalization.GetLocaliseTextByKey("attachments"));
            }
            else
            {
                textLastMessage.text = lastMSG;
            }
        }
        else
        {
            textLastMessage.text = APIManager.DecodedString(allChatGetConversationDatum.lastMsg);
        }

        DateTime timeUtc = allChatGetConversationDatum.updatedAt;
        DateTime today = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, TimeZoneInfo.Local);

        if (allChatGetConversationDatum.conversationsReadCounts[0].unReadCount > 0)
        {
            UnReadCountobject.SetActive(true);
            UnreadMessageText.text = allChatGetConversationDatum.conversationsReadCounts[0].unReadCount.ToString();
            backgroundImage.color = unreadColor;
        }
        else
        {
            backgroundImage.color = Color.white;
        }

        if (allChatGetConversationDatum.isMutedConversations)
        {
            muteObject.SetActive(true);
            if (!UnReadCountobject.activeSelf)
            {
                muteObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, muteObject.GetComponent<RectTransform>().anchoredPosition.y);
            }
        }
        TimeSpan timeDiff = (DateTime.Now - today);
        //TimeSpan timeDiff = (DateTime.UtcNow - allChatGetConversationDatum.updatedAt);
        //Debug.LogError("minuts : " + timeDiff.TotalMinutes + "  :updatedAt: " + allChatGetConversationDatum.updatedAt + "  :timediff:" + timeDiff + "   :name:"+textTitle.text);
        //Debug.LogError("TimeDiff:" + today.Date.ToString("yyyy/MM/dd"));

        if (timeDiff.TotalMinutes < 1)
        {
            textTime.text = TextLocalization.GetLocaliseTextByKey("Just Now");
            //textTime.gameObject.GetComponent<TextLocalization>().LocalizeTextText();
        }
        else if (timeDiff.TotalMinutes > 1 && timeDiff.TotalMinutes <= 60)
        {
            textTime.text = Mathf.Round((float)(timeDiff.TotalMinutes)) + "m";
        }
        /*else if (timeDiff.TotalMinutes <= 1440 && today.Date != DateTime.Now.Date)
        {
            textTime.text = timeDiff.Days.ToString() + "d";
        }*/
        else if (timeDiff.TotalMinutes > 60 && timeDiff.TotalMinutes <= 1440)
        {
            textTime.text = Mathf.Round((float)(timeDiff.TotalMinutes / 60)) + "h";
        }
        else if (timeDiff.TotalMinutes > 1440 && timeDiff.TotalMinutes <= 2880)
        {
            textTime.text = timeDiff.Days.ToString() + "d"; 
            //textTime.text = "Yesterday";
        }
        else if (timeDiff.TotalMinutes > 2880 && timeDiff.TotalMinutes <= 43200)
        {
            textTime.text = timeDiff.Days.ToString() + "d";
        }
        else if (timeDiff.Days >= 30)
        {
            textTime.text = today.Date.ToString("yyyy/MM/dd");
            //textTime.text ="long time ago";
        }
        APIController.Instance.allConversationList.Add(textTitle.text);
    }

    //this method is used to reset message unread count.......
    public void ResetUnReadMessageCount()
    {
        allChatGetConversationDatum.conversationsReadCounts[0].unReadCount = 0;
        UnReadCountobject.SetActive(false);
        backgroundImage.color = Color.white;
    }

    public void OnlyLoadProfileImage(string avtarUrl) 
    {
        if (!string.IsNullOrEmpty(avtarUrl))
        {
            if (AssetCache.Instance.HasFile(avtarUrl))
            {
                AssetCache.Instance.LoadSpriteIntoImage(profileImage, avtarUrl, changeAspectRatio: true);
            }
        }
        else
        {
            profileImage.sprite = defaultSP;
        }
    }

    public void OnClickStartMessage()
    {
        //Debug.LogError(currentScrollRect.velocity);
        if (MessageController.Instance.deleteConfirmationScreen.activeSelf || currentScrollRect.velocity != Vector2.zero)
        {
            //Debug.LogError("Do not open Chat screen.......");
            return;
        }
        pressed = false;
        StopCountDownCo();//this is used to stop countdown coroutine if is started.......

        MessageController.Instance.chatPrefabParent.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

        APIController.Instance.allChatMessageId.Clear();
        APIController.Instance.chatTimeList.Clear();
        MessageController.Instance.currentConversationData = this;
        MessageController.Instance.currentChatPage = 1;
        MessageController.Instance.isChatDataLoaded = false;

        string TempChatTitle = MessageController.Instance.chatTitleText.text.ToString();

        foreach (Transform item in MessageController.Instance.chatPrefabParent)
        {
            Destroy(item.gameObject);
        }

        if (allChatGetConversationDatum.receiverId != 0)
        {
            MessageController.Instance.LoaderShow(true);//rik loader active.......

            //Debug.LogError("receiverId" + allChatGetConversationDatum.receiverId);
            if (allChatGetConversationDatum.receiverId == APIManager.Instance.userId)
            {
                APIManager.Instance.RequestChatGetMessages(1, 50, allChatGetConversationDatum.senderId, 0, "Conversation");
            }
            else
            {
                APIManager.Instance.RequestChatGetMessages(1, 50, allChatGetConversationDatum.receiverId, 0, "Conversation");
            }
            MessageController.Instance.chatTitleText.text = textTitle.text;
        }
        else if (allChatGetConversationDatum.receivedGroupId != 0)
        {
            MessageController.Instance.LoaderShow(true);//rik loader active.......

            //Debug.LogError("receivedGroupId" + allChatGetConversationDatum.receivedGroupId);
            APIManager.Instance.RequestChatGetMessages(1, 50, 0, allChatGetConversationDatum.receivedGroupId, "Conversation");
            if (!string.IsNullOrEmpty(allChatGetConversationDatum.group.name))
            {
                MessageController.Instance.chatTitleText.text = allChatGetConversationDatum.group.name;
            }
            else
            {
                for (int i = 0; i < allChatGetConversationDatum.group.groupUsers.Count; i++)
                {
                    groupName.Add(allChatGetConversationDatum.group.groupUsers[i].user.name);
                }
                CreateChatTitleString(groupName);
                MessageController.Instance.chatTitleText.text = chatTitlestr;
            }
        }
        MessageController.Instance.allChatGetConversationDatum = allChatGetConversationDatum;

        if (MessageController.Instance.chatTitleText.text != TempChatTitle)
        {
            //MessageController.Instance.typeMessageText.text = "";
            MessageController.Instance.chatTypeMessageInputfield.Text = "";
            MessageController.Instance.OnChatVoiceOrSendButtonEnable();
        }

        bool isGroup = false;
        if (allChatGetConversationDatum.group != null && allChatGetConversationDatum.receivedGroupId != 0)
        {
            isGroup = true;
            MessageController.Instance.defaultChatScreenTopUserImage = MessageController.Instance.defaultUserImageSquare;
        }
        else
        {
            isGroup = false;
            MessageController.Instance.defaultChatScreenTopUserImage = MessageController.Instance.defaultUserImageRound;
        }

        MessageController.Instance.ChatScreenTopUserProfileBGSetUp(isGroup);//set default bg for user profile(round or square)
        
        if (!string.IsNullOrEmpty(avatarUrl))
        {
            AssetCache.Instance.LoadSpriteIntoImage(MessageController.Instance.chatScreenTopUserImage, avatarUrl, changeAspectRatio: true);
        }
        else
        {            
            MessageController.Instance.ChatScreenTopUserProfileSetUp(MessageController.Instance.defaultChatScreenTopUserImage, isGroup);
        }
        MessageController.Instance.isDirectMessage = false;
    }

    public string chatTitlestr;
    public void CreateChatTitleString(List<string> memberList)
    {
        chatTitlestr = "";
        for (int i = 0; i < memberList.Count; i++)
        {
            if (i < memberList.Count - 1)
            {
                chatTitlestr += memberList[i] + ",";
            }
            else
            {
                chatTitlestr += memberList[i];
            }
        }
    }

    #region Get Image From AWS
    public void GetImageFromAWS(string key, Image avatarImage)
    {
        //Debug.LogError("GetImageFromAWS key:" + key);
        //GetExtentionType(key);
        if (AssetCache.Instance.HasFile(key))
        {
            //Debug.LogError("Image Available on Disk");
            AssetCache.Instance.LoadSpriteIntoImage(avatarImage, key, changeAspectRatio: true);
            return;
        }
        else
        {
            AssetCache.Instance.EnqueueOneResAndWait(key, (ConstantsGod.r_AWSImageKitBaseUrl + key), (success) =>
            {
                if (success)
                {
                    AssetCache.Instance.LoadSpriteIntoImage(avatarImage, key, changeAspectRatio: true);
                    //Debug.LogError("Save and Image download success");
                }
            });
        }
    }

    /*public static ExtentionType currentExtention;
    public static ExtentionType GetExtentionType(string path)
    {
        if (string.IsNullOrEmpty(path))
            return (ExtentionType)0;

        string extension = Path.GetExtension(path);
        if (string.IsNullOrEmpty(extension))
            return (ExtentionType)0;

        if (extension[0] == '.')
        {
            if (extension.Length == 1)
                return (ExtentionType)0;

            extension = extension.Substring(1);
        }

        extension = extension.ToLowerInvariant();
        //Debug.LogError("ExtentionType " + extension);
        if (extension == "png" || extension == "jpg" || extension == "jpeg" || extension == "gif" || extension == "bmp" || extension == "tiff" || extension == "heic")
        {
            currentExtention = ExtentionType.Image;
            return ExtentionType.Image;
        }
        else if (extension == "mp4" || extension == "mov" || extension == "wav" || extension == "avi")
        {
            currentExtention = ExtentionType.Video;
            return ExtentionType.Video;
        }
        else if (extension == "mp3" || extension == "aac" || extension == "flac")
        {
            currentExtention = ExtentionType.Audio;
            return ExtentionType.Audio;
        }
        return (ExtentionType)0;
    }*/
    #endregion

    #region Press and hold to Delete Conversation.......
    public bool pressed = false;
    private const float copyTextThreshold = 0.3f; // seconds
    Coroutine countDownCo;

    //this method is used to pointer down and start copy Threshold.......
    public void PointerDownStart()
    {
        if (!pressed)
        {
            StopCountDownCo();
            // Start counting
            countDownCo = StartCoroutine(Countdown());
        }
    }

    //this method is used to pointer up and stop copy Threshold.......
    public void PointerUpStop()
    {
        pressed = false;
    }

    void StopCountDownCo()
    {
        if (countDownCo != null)
        {
            StopCoroutine(countDownCo);
        }
    }

    private IEnumerator Countdown()
    {
        // First wait a short time to make sure it's not a tap
        yield return new WaitForSeconds(0.01f);
        pressed = true;

        if (!pressed)
            yield break;

        // Animate the countdown
        float startTime = Time.time, ratio = 0f;
        while (pressed && (ratio = (Time.time - startTime) / copyTextThreshold) < 1.0f)
        {
            yield return null;
        }

        if (pressed)//Copy Message.......
        {
            pressed = false;
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }

            //Debug.LogError("conversation item seleted for delete and enable delete conversation popup");
            MessageController.Instance.deleteConfirmationCurrentConversationDataScript = this;
            if (allChatGetConversationDatum.group != null && allChatGetConversationDatum.receivedGroupId != 0)
            {
                if (allChatGetConversationDatum.group.createdBy == APIManager.Instance.userId)//group admin is this user.......
                {
                    MessageController.Instance.ShowSetupDeleteConfirmationScreen("Delete", "Are you sure you want to delete group?");
                }
                else//group admin is not this user.......
                {
                    MessageController.Instance.ShowSetupDeleteConfirmationScreen("Leave", "Are you sure you want to leave group?");
                }
            }
            else//one to one conversation.......
            {
                MessageController.Instance.ShowSetupDeleteConfirmationScreen("Delete", "Are you sure you want to delete conversation?");
            }
        }
    }
    #endregion
}