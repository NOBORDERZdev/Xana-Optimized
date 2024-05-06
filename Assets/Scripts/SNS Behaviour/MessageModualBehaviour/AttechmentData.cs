//using Amazon.S3.Model;
using RenderHeads.Media.AVProVideo;
using SuperStar.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class AttechmentData : MonoBehaviour
{
    //public ChatAttachmentsRow attachmentsRow;

    //public Image attechmentImage;
    //public GameObject mediaPlayer, VideoPlayer;
    //public GameObject videoIcon;
    //public bool isChooseAttechmentScreen;

    //private void OnDestroy()
    //{
    //    if (!isChooseAttechmentScreen)
    //    {
    //        AssetCache.Instance.RemoveFromMemory(attechmentImage.sprite);
    //        attechmentImage.sprite = null;
    //        SNS_APIManager.Instance.ResourcesUnloadAssetFile();//UnloadUnusedAssets file call every 15 items.......
    //    }
    //}

    //private void OnDisable()
    //{
    //    if (isChooseAttechmentScreen)
    //    {
    //        AssetCache.Instance.RemoveFromMemory(attechmentImage.sprite);
    //        attechmentImage.sprite = null;
    //        //Resources.UnloadUnusedAssets();//every clear.......
    //        //Caching.ClearCache();
    //        SNS_APIManager.Instance.ResourcesUnloadAssetFile();//UnloadUnusedAssets file call every 15 items.......
    //    }
    //}

    //public void LoadData(bool isChooseAttechmentScreen1)
    //{
    //    isChooseAttechmentScreen = isChooseAttechmentScreen1;
    //    if (!string.IsNullOrEmpty(attachmentsRow.url))
    //    {
    //        GetAndLoadMediaFile(attachmentsRow.url);
    //    }
    //}

    //public void GetAndLoadMediaFile(string key)
    //{
    //    //Debug.LogError("GetAndLoadMediaFile: " + key);
    //    GetExtentionType(key);
    //    //Debug.LogError("currentExtention:   " + currentExtention);
    //    if (currentExtention == ExtentionType.Image)
    //    {
    //        bool isUrlContainsHttpAndHttps = SNS_APIManager.Instance.CheckUrlDropboxOrNot(key);
    //        if (isUrlContainsHttpAndHttps)
    //        {
    //            AssetCache.Instance.EnqueueOneResAndWait(key, key, (success) =>
    //            {
    //                if (success)
    //                {
    //                    AssetCache.Instance.LoadSpriteIntoImage(attechmentImage, key, changeAspectRatio: true);
    //                    CheckAndSetResolutionOfImage(attechmentImage.sprite);
    //                }
    //            });
    //        }
    //        else
    //        {
    //            GetImageFromAWS(key, attechmentImage);
    //        }
    //    }
    //    else if (currentExtention == ExtentionType.Video)
    //    {
    //        GetVideoUrl(key);
    //    }
    //}   

    //public void OnClickSeeFullImage()
    //{        
    //    foreach (Transform item in SNS_MessageController.Instance.saveAttechmentParent)
    //    {
    //        Destroy(item.gameObject);
    //    }
    //    int index = 0;
    //    int pageIndex = 0;
    //    bool isMatch = false;
    //    for (int i = 0; i < SNS_APIManager.Instance.AllChatAttachmentsRoot.data.rows.Count; i++)
    //    {
    //        GameObject attechment = Instantiate(SNS_APIController.Instance.saveAttechmentPrefab, SNS_MessageController.Instance.saveAttechmentParent);
    //        attechment.GetComponent<SaveAttechmentScript>().attachmentsRow = SNS_APIManager.Instance.AllChatAttachmentsRoot.data.rows[i];
    //        attechment.GetComponent<SaveAttechmentScript>().LoadData();

    //        //Debug.LogError("id:" + SNS_APIManager.Instance.AllChatAttachmentsRoot.data.rows[i].id);
    //        //Debug.LogError("ids: " + attachmentsRow.id);
    //        if (SNS_APIManager.Instance.AllChatAttachmentsRoot.data.rows[i].id == attachmentsRow.id && !isMatch)
    //        {
    //            pageIndex = index;
    //            SNS_MessageController.Instance.saveAttechmentParent.transform.parent.GetComponent<ScrollSnapRect>().startingPage = pageIndex;
    //            SNS_MessageController.Instance.SaveAttachmentDetailsSetup(pageIndex);
    //            isMatch = true;
    //        }
    //        else
    //        {
    //            //Debug.LogError("index: " + index);
    //            index += 1;
    //        }
    //    }
    //    SNS_MessageController.Instance.AttechmentDownloadScreen.SetActive(true);
    //    SNS_MessageController.Instance.saveAttechmentParent.transform.parent.GetComponent<ScrollSnapRect>().StartScrollSnap();
    //}

    //public void SetVideoUi(bool isVideo)
    //{
    //    if (isVideo)
    //    {
    //        if(attechmentImage!=null)
    //            attechmentImage.gameObject.SetActive(false);
    //        mediaPlayer.SetActive(true);
    //        VideoPlayer.SetActive(true);
    //        videoIcon.SetActive(true);
    //    }
    //    else
    //    {
    //        if (attechmentImage != null)
    //            attechmentImage.gameObject.SetActive(true);
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
    //    //Debug.LogError("Chat Video file sending url request:" + AWSHandler.Instance._s3Client);
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
    //                    SetVideoUi(true);
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
    //                SetVideoUi(true);
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
    //    //GetExtentionType(key);
    //    if (AssetCache.Instance.HasFile(key))
    //    {
    //        //Debug.LogError("Image Available on Disk");
    //        AssetCache.Instance.LoadSpriteIntoImage(mainImage, key, changeAspectRatio: true);
    //        CheckAndSetResolutionOfImage(mainImage.sprite);
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
    //    //Debug.LogError("ExtentionType " + extension);
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
}