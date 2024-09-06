using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using SuperStar.Helpers;
//using Amazon.S3.Model;
using UnityEngine.Networking;
using System.IO;
//using Amazon.S3;
//using Amazon.CognitoIdentity.Model;
using RenderHeads.Media.AVProVideo;
using UnityEngine.EventSystems;
using UnityEngine.UI.Extensions;

public class ChatDataScript : MonoBehaviour
{
    //public ChatGetMessagesRow MessageRow;

    //public TextMeshProUGUI chatMessageText;
    //public ContentSizeFitter contentSizeFitter;
    //public TextMeshProUGUI messageTimeText;
    //public Image attechmentImage;

    //public GameObject videoIcon;
    //public GameObject mediaPlayer, VideoPlayer;
    //public TextMeshProUGUI senderName;
    //public string attechMentUrl;

    //public bool isVideo;

    //public string loadedImageKey1;
    //public int cnt = 0;
    //private void OnEnable()
    //{
    //    if (!string.IsNullOrEmpty(loadedImageKey1) && cnt > 0)
    //    {
    //        //Debug.LogError("load image");
    //        GetImageFromAWS(loadedImageKey1, attechmentImage);
    //    }
    //    cnt += 1;
    //}

    //private void OnDisable()
    //{
    //    if (!string.IsNullOrEmpty(loadedImageKey1))
    //    {
    //        AssetCache.Instance.RemoveFromMemory(attechmentImage.sprite);
    //        attechmentImage.sprite = null;
    //        //Resources.UnloadUnusedAssets();//every clear.......
    //        //Caching.ClearCache();
    //        SNS_APIManager.Instance.ResourcesUnloadAssetFile();//UnloadUnusedAssets file call every 15 items.......
    //    }
    //}

    //public void LoadFeed()
    //{
    //    //attechMentUrl = AWSHandler.Instance.URL;
    //    double minuts = (System.DateTime.Now - MessageRow.createdAt).TotalMinutes;
    //    if (!string.IsNullOrEmpty(MessageRow.message.msg))
    //    {
    //        chatMessageText.text = SNS_APIManager.DecodedString(MessageRow.message.msg);
    //        //CheckAndSetTextMinWidth();
    //        Invoke("CheckAndSetTextMinWidth", 0.013f);
    //    }
    //    if (MessageRow.message.attachments.Count > 0)
    //    {
    //        //Debug.LogError("aaa " + MessageRow.message.attachments[0].url);
    //        //getUrl(MessageRow.message.attachments[0].url);
    //        //GetObject(MessageRow.message.attachments[0].url);//old
    //        GetAndLoadMediaFile(MessageRow.message.attachments[0].url);
    //    }
    //    if (MessageRow.receivedGroupId != 0)
    //    {
    //        if (senderName != null)
    //        {
    //            senderName.gameObject.SetActive(true);
    //        }
    //        if (MessageRow.senderId != SNS_APIManager.Instance.userId)
    //        {
    //            senderName.text = MessageRow.sender.name;
    //        }
    //    }
    //    else
    //    {
    //        if (senderName != null)
    //        {
    //            senderName.gameObject.SetActive(false);
    //        }
    //    }

    //    DateTime timeUtc = MessageRow.updatedAt;
    //    DateTime today = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, TimeZoneInfo.Local);
    //    // Debug.LogError("Time" + today);
    //    //Debug.LogError("Time" + today.Date);
    //    int hour;
    //    if (today.Hour >= 12)
    //    {
    //        if (today.Hour > 12)
    //        {
    //            hour = (today.Hour - 12);
    //        }
    //        else
    //        {
    //            hour = today.Hour;
    //        }
    //        messageTimeText.text = hour + ":" + today.Minute.ToString("00") + " PM";
    //    }
    //    else
    //    {
    //        messageTimeText.text = today.Hour + ":" + today.Minute.ToString("00") + " AM";
    //    }
    //}

    //public void CheckAndSetTextMinWidth()
    //{
    //    //Debug.LogError("msg" + MessageRow.message.msg);
    //    //Debug.LogError(chatMessageText.GetComponent<RectTransform>().sizeDelta.x + " :width:" + Screen.width);
        
    //    if (chatMessageText.GetComponent<RectTransform>().sizeDelta.x > (Screen.width - 200))
    //    {
    //        chatMessageText.GetComponent<LayoutElement>().preferredWidth = Screen.width - 300;
    //        chatMessageText.transform.parent.GetComponent<VerticalLayoutGroup>().padding = new RectOffset(50, 50, 24, 24);

    //        SetEventTrigger();
    //    }
    //    else
    //    {
    //        if (chatMessageText.GetComponent<RectTransform>().sizeDelta.x == 0)
    //        {
    //            Invoke("CheckAndSetTextMinWidth", 0.013f);
    //            return;
    //        }
    //    }
    //    contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
    //    Invoke("WaitToReset", 0.022f);
    //}

    //void WaitToReset()
    //{
    //    contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    //}

    //public EventTrigger eventTrigger;
    //void SetEventTrigger()
    //{
    //    //Debug.LogError("SetEventTrigger");
    //    if (eventTrigger != null)
    //    {
    //        EventTrigger trigger = eventTrigger;
    //        EventTrigger.Entry entryBegin = new EventTrigger.Entry(), entryDrag = new EventTrigger.Entry(), entryEnd = new EventTrigger.Entry(), entrypotential = new EventTrigger.Entry()
    //            , entryScroll = new EventTrigger.Entry();

    //        entryBegin.eventID = EventTriggerType.BeginDrag;
    //        entryBegin.callback.AddListener((data) =>
    //        {
    //            pressed = false;
    //            SNS_MessageController.Instance.chatScrollMain.GetComponent<ScrollRectFasterEx>().OnBeginDrag((PointerEventData)data);
    //        });
    //        trigger.triggers.Add(entryBegin);

    //        entryDrag.eventID = EventTriggerType.Drag;
    //        entryDrag.callback.AddListener((data) =>
    //        {
    //            pressed = false;
    //            SNS_MessageController.Instance.chatScrollMain.GetComponent<ScrollRectFasterEx>().OnDrag((PointerEventData)data);
    //        });
    //        trigger.triggers.Add(entryDrag);

    //        entryEnd.eventID = EventTriggerType.EndDrag;
    //        entryEnd.callback.AddListener((data) =>
    //        {
    //            SNS_MessageController.Instance.chatScrollMain.GetComponent<ScrollRectFasterEx>().OnEndDrag((PointerEventData)data);
    //        });
    //        trigger.triggers.Add(entryEnd);

    //        entrypotential.eventID = EventTriggerType.InitializePotentialDrag;
    //        entrypotential.callback.AddListener((data) =>
    //        {
    //            SNS_MessageController.Instance.chatScrollMain.GetComponent<ScrollRectFasterEx>().OnInitializePotentialDrag((PointerEventData)data);
    //        });
    //        trigger.triggers.Add(entrypotential);

    //        entryScroll.eventID = EventTriggerType.Scroll;
    //        entryScroll.callback.AddListener((data) =>
    //        {
    //            SNS_MessageController.Instance.chatScrollMain.GetComponent<ScrollRectEx>().OnScroll((PointerEventData)data);
    //        });
    //        trigger.triggers.Add(entryScroll);
    //    }
    //}

    ////this method is used to click on Video or image button.......
    //public void OnClicVideoOrImageButton()
    //{
    //    if (isVideo)
    //    {
    //        SNS_MessageController.Instance.OnShowChatVideoOrImage(null, mediaPlayer.GetComponent<MediaPlayer>());
    //    }
    //    else
    //    {
    //        SNS_MessageController.Instance.OnShowChatVideoOrImage(attechmentImage.sprite, null);
    //    }
    //} 

    //public void GetAndLoadMediaFile(string key)
    //{
    //    isVideo = false;
    //    //Debug.LogError("GetAndLoadMediaFile: " + key);
    //    GetExtentionType(key);
    //    //Debug.LogError("currentExtention:   " + currentExtention);
    //    if (currentExtention == ExtentionType.Image)
    //    {
    //        bool isAvatarUrlFromDropbox = SNS_APIManager.Instance.CheckUrlDropboxOrNot(key);
    //        //Debug.LogError("isAvatarUrlFromDropbox: " + isAvatarUrlFromDropbox + " :name:" + FeedsByFollowingUserRowData.User.Name);

    //        if (isAvatarUrlFromDropbox)
    //        {
    //            AssetCache.Instance.EnqueueOneResAndWait(key, key, (success) =>
    //            {
    //                if (success)
    //                {
    //                    AssetCache.Instance.LoadSpriteIntoImage(attechmentImage, key, changeAspectRatio: true);
    //                    CheckAndSetResolutionOfImage(attechmentImage.sprite);
    //                    loadedImageKey1 = key;
    //                }
    //            });
    //        }
    //        else
    //        {
    //            GetImageFromAWS(key, attechmentImage);
    //        }
    //    }
    //    else if(currentExtention == ExtentionType.Video)
    //    {
    //        isVideo = true;
    //        SetVideoUi(true);
    //        GetVideoUrl(key);
    //    }           
    //}

    //public void SetVideoUi(bool isVideo)
    //{
    //    //Debug.LogError("Chad Data Script SetVideoUI:" + isVideo + "   :id:" + MessageRow.id);
    //    if (isVideo)
    //    {
    //        //Debug.LogError("Media player000:" + mediaPlayer.activeSelf);
    //        attechmentImage.gameObject.SetActive(false);
    //        videoIcon.SetActive(true);
    //        mediaPlayer.SetActive(true);
    //        VideoPlayer.SetActive(true);
    //    }
    //    else
    //    {
    //        //Debug.LogError("Media player111:" + mediaPlayer.activeSelf);
    //        attechmentImage.gameObject.SetActive(true);
    //        mediaPlayer.SetActive(false);
    //        VideoPlayer.SetActive(false);
    //        videoIcon.SetActive(false);
    //    }
    //}

    //#region Get Image and Video From AWS
    //public void GetVideoUrl(string key)
    //{
    //    /*var request_1 = new GetPreSignedUrlRequest()
    //    {
    //        BucketName = AWSHandler.Instance.Bucketname,
    //        Key = key,
    //        Expires = DateTime.Now.AddHours(6)
    //    };
    //    //Debug.LogError("Chat Video file sending url request:"+AWSHandler.Instance._s3Client);
    //    //AWSHandler.Instance.GetObject(key);
    //    AWSHandler.Instance._s3Client.GetPreSignedURLAsync(request_1, (callback) =>
    //    {
    //        if (callback.Exception == null)
    //        {
    //            string mediaUrl = callback.Response.Url;
    //            UnityToolbag.Dispatcher.Invoke(() =>
    //            {
    //                if (this.isActiveAndEnabled)
    //                {
    //                    //Debug.LogError("Chat Video URL " + mediaUrl);
    //                    //SetVideoUi(true);
    //                    //mediaPlayer.GetComponent<MediaPlayer>().OpenMedia(new MediaPath(mediaUrl, MediaPathType.RelativeToPersistentDataFolder), autoPlay: true);
    //                    mediaPlayer.GetComponent<MediaPlayer>().OpenMedia(new MediaPath(mediaUrl, MediaPathType.AbsolutePathOrURL), autoPlay: false);
    //                    //mediaPlayer.GetComponent<MediaPlayer>().OpenMedia(new MediaPath(mediaUrl, MediaPathType.AbsolutePathOrURL), autoPlay: true);
    //                    //mediaPlayer.GetComponent<MediaPlayer>().Play();
    //                }
    //            });
    //        }
    //        else
    //            Debug.LogError(callback.Exception);
    //    });*/

    //    if (key != "")
    //    {
    //        string mediaUrl = "";

    //        if (key.Contains("https"))
    //        {
    //            mediaUrl = key;
    //        }
    //        else
    //        {
    //            mediaUrl = ConstantsGod.AWS_VIDEO_BASE_URL + key;
    //        }

    //        Debug.Log($"<color=green> Video Key = followingUserFeedItem : </color>{mediaUrl}");
    //        UnityToolbag.Dispatcher.Invoke(() =>
    //        {
    //            if (this.isActiveAndEnabled)
    //            {
    //                //Debug.LogError("Chat Video URL " + mediaUrl);
    //                //SetVideoUi(true);
    //                //mediaPlayer.GetComponent<MediaPlayer>().OpenMedia(new MediaPath(mediaUrl, MediaPathType.RelativeToPersistentDataFolder), autoPlay: true);
    //                mediaPlayer.GetComponent<MediaPlayer>().OpenMedia(new MediaPath(mediaUrl, MediaPathType.AbsolutePathOrURL), autoPlay: false);
    //                //mediaPlayer.GetComponent<MediaPlayer>().OpenMedia(new MediaPath(mediaUrl, MediaPathType.AbsolutePathOrURL), autoPlay: true);
    //                //mediaPlayer.GetComponent<MediaPlayer>().Play();
    //            }
    //        });
    //    }
    //}

    //public void GetImageFromAWS(string key, Image mainImage)
    //{
    //    //Debug.LogError("GetImageFromAWS key:" + key);
    //    if (AssetCache.Instance.HasFile(key))
    //    {
    //        //Debug.LogError("Chat Image Available on Disk");
    //        AssetCache.Instance.LoadSpriteIntoImage(mainImage, key, changeAspectRatio: true);
    //        CheckAndSetResolutionOfImage(mainImage.sprite);
    //        loadedImageKey1 = key;
    //        return;
    //    }
    //    else
    //    {
    //        AssetCache.Instance.EnqueueOneResAndWait(key, (ConstantsGod.r_AWSImageKitBaseUrl + key), (success) =>
    //        {
    //            if (success)
    //            {
    //                AssetCache.Instance.LoadSpriteIntoImage(mainImage, key, changeAspectRatio: true);
    //                CheckAndSetResolutionOfImage(mainImage.sprite);
    //                loadedImageKey1 = key;
    //                //Debug.LogError("Save and Image download success");
    //            }
    //        });
    //    }
    //}

    //public static ExtentionType currentExtention;
    //public static ExtentionType GetExtentionType(string path)
    //{
    //    if (string.IsNullOrEmpty(path))
    //        return (ExtentionType)0;

    //    string extension = Path.GetExtension(path);
    //    if (string.IsNullOrEmpty(extension))
    //        return (ExtentionType)0;

    //    if (extension[0] == '.')
    //    {
    //        if (extension.Length == 1)
    //            return (ExtentionType)0;

    //        extension = extension.Substring(1);
    //    }

    //    extension = extension.ToLowerInvariant();
    //    //Debug.LogError("ExtentionType: " + extension);
    //    if (extension == "png" || extension == "jpg" || extension == "jpeg" || extension == "gif" || extension == "bmp" || extension == "tiff" || extension == "heic")
    //    {
    //        currentExtention = ExtentionType.Image;
    //        return ExtentionType.Image;
    //    }
    //    else if (extension == "mp4" || extension == "mov" || extension == "wav" || extension == "avi")
    //    {
    //        currentExtention = ExtentionType.Video;
    //        // Debug.LogError("vvvvvvvvvvvvvvvvvvvvvvvvvvvv");
    //        return ExtentionType.Video;
    //    }
    //    else if (extension == "mp3" || extension == "aac" || extension == "flac")
    //    {
    //        currentExtention = ExtentionType.Audio;
    //        return ExtentionType.Audio;
    //    }

    //    return (ExtentionType)0;
    //}
    //#endregion    

    //#region Check And Set Image Orientation 
    //public AspectRatioFitter aspectRatioFitter;
    //public void CheckAndSetResolutionOfImage(Sprite feedImage)
    //{
    //    float diff = feedImage.rect.width - feedImage.rect.height;

    //    //Debug.LogError("CheckAndSetResolutionOfImage:" + diff);
    //    if (diff < -150f)
    //    {
    //        aspectRatioFitter.aspectRatio = 0.1f;
    //    }
    //    else
    //    {
    //        aspectRatioFitter.aspectRatio = 2f;
    //    }
    //}
    //#endregion

    //#region Press and hold to copy message.......
    //public bool pressed = false;
    //private const float copyTextThreshold = 1f; // seconds
    ////this method is used to pointer down and start copy Threshold.......
    //public void PointerDownStartCopy()
    //{
    //    if (!pressed)
    //    {
    //        // Start counting
    //        StartCoroutine(Countdown());
    //    }
    //}

    ////this method is used to pointer up and stop copy Threshold.......
    //public void PointerUpStopCopy()
    //{
    //    pressed = false;
    //}

    //private IEnumerator Countdown()
    //{
    //    // First wait a short time to make sure it's not a tap
    //    yield return new WaitForSeconds(0.01f);
    //    pressed = true;

    //    if (!pressed)
    //        yield break;

    //    // Animate the countdown
    //    float startTime = Time.time, ratio = 0f;
    //    while (pressed && (ratio = (Time.time - startTime) / copyTextThreshold) < 1.0f)
    //    {
    //        yield return null;
    //    }

    //    if (pressed)//Copy Message.......
    //    {
    //        pressed = false;
    //        //if (!string.IsNullOrEmpty(MessageRow.message.msg))
    //        if (!string.IsNullOrEmpty(chatMessageText.text))
    //        {
    //            Debug.LogError("Copy Message.......");
    //            EasyUI.Toast.Toast.Show(TextLocalization.GetLocaliseTextByKey("Text copied!"), 1f);
    //            GUIUtility.systemCopyBuffer = chatMessageText.text;

    //            /*TextEditor textEditor = new TextEditor();
    //            textEditor.multiline = true;
    //            textEditor.text = chatMessageText.text;
    //            textEditor.Copy();*/
    //        }
    //    }
    //}
    //#endregion
}