using RenderHeads.Media.AVProVideo;
using SuperStar.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SaveAttechmentScript : MonoBehaviour
{
    //public ChatAttachmentsRow attachmentsRow;

    //public Image attechmentImage;
    //public GameObject mediaPlayer, VideoPlayer;

    //private void OnDisable()
    //{
    //    AssetCache.Instance.RemoveFromMemory(attechmentImage.sprite);
    //    attechmentImage.sprite = null;
    //    //Resources.UnloadUnusedAssets();//every clear.......
    //    //Caching.ClearCache();
    //    SNS_APIManager.Instance.ResourcesUnloadAssetFile();//UnloadUnusedAssets file call every 15 items.......
    //}

    //public void LoadData()
    //{
    //    //GetObject(attachmentsRow.url);
    //    if (!string.IsNullOrEmpty(attachmentsRow.url))
    //    {
    //        GetAndLoadMediaFile(attachmentsRow.url);
    //    }
    //}

    //public void SetVideoUi(bool isVideo)
    //{
    //    if (isVideo)
    //    {
    //        attechmentImage.gameObject.SetActive(false);
    //        mediaPlayer.SetActive(true);
    //        VideoPlayer.SetActive(true);
    //    }
    //    else
    //    {
    //        attechmentImage.gameObject.SetActive(true);
    //        mediaPlayer.SetActive(false);
    //        VideoPlayer.SetActive(false);
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
    //                    mediaPlayer.GetComponent<MediaPlayer>().OpenMedia(new MediaPath(mediaUrl, MediaPathType.AbsolutePathOrURL), autoPlay: true);
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

    //        Debug.Log($"<color=green> Video Key = FeedVideoItem : </color>{mediaUrl}");
    //        UnityToolbag.Dispatcher.Invoke(() =>
    //        {
    //            if (this.isActiveAndEnabled)
    //            {
    //                //Debug.LogError("Chat Video URL " + mediaUrl);
    //                SetVideoUi(true);
    //                mediaPlayer.GetComponent<MediaPlayer>().OpenMedia(new MediaPath(mediaUrl, MediaPathType.AbsolutePathOrURL), autoPlay: true);
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
    //        return;
    //    }
    //    else
    //    {
    //        AssetCache.Instance.EnqueueOneResAndWait(key, (ConstantsGod.r_AWSImageKitBaseUrl + key), (success) =>
    //        {
    //            if (success)
    //            {
    //                AssetCache.Instance.LoadSpriteIntoImage(mainImage, key, changeAspectRatio: true);
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
}