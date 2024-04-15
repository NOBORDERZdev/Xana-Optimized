using SuperStar.Helpers;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FeedCommentItemController : MonoBehaviour
{
    //public CommentRow commentRow;
    //public Image profileImage;
    //public TextMeshProUGUI userNameText;
    //public TextMeshProUGUI descriptionText;
    //public TextMeshProUGUI timeText;
    //public static ExtentionType currentExtention;

    //public Sprite defaultSP;


    //private void Awake()
    //{
    //    defaultSP = profileImage.sprite;
    //}

    //int cnt = 0;
    //private void OnEnable()
    //{
    //    if (defaultSP != null)
    //    {
    //        profileImage.sprite = defaultSP;
    //    }
    //    if (cnt > 0 && commentRow.user != null)
    //    {
    //        if (!string.IsNullOrEmpty(commentRow.user.avatar))
    //        {
    //            if (AssetCache.Instance.HasFile(commentRow.user.avatar))
    //            {
    //                AssetCache.Instance.LoadSpriteIntoImage(profileImage, commentRow.user.avatar, changeAspectRatio: true);
    //            }
    //        }
    //    }
    //    else
    //    {
    //        cnt += 1;
    //    }
    //}

    //private void OnDisable()
    //{
    //    if (commentRow.user != null)
    //    {
    //        if (!string.IsNullOrEmpty(commentRow.user.avatar))
    //        {
    //            AssetCache.Instance.RemoveFromMemory(profileImage.sprite);
    //            profileImage.sprite = null;
    //            //Resources.UnloadUnusedAssets();//every clear.......
    //            //Caching.ClearCache();
    //            SNS_APIManager.Instance.ResourcesUnloadAssetFile();//UnloadUnusedAssets file call every 15 items.......
    //        }
    //    }
    //}

    ////public void OnClickCommentUserProfile()
    ////{
    ////    FeedUIController.Instance.commentPanel.SetActive(false);
    ////    //FeedUIController.Instance.feedVideoScreen.SetActive(false);
    ////    if (MyProfileDataManager.Instance != null && MyProfileDataManager.Instance.myProfileData.id == commentRow.user.id)
    ////    {
    ////        FeedUIController.Instance.bottomTabManager.OnClickProfileButton();
    ////    }
    ////    else
    ////    {
    ////        OtherPlayerProfileData.Instance.RequestGetUserDetails(commentRow.user.id);
    ////    }
    ////}
    //public void SetupData(CommentRow commentRowData)
    //{
    //    commentRow = commentRowData;

    //    if (commentRow.user != null)
    //    {
    //        userNameText.text = commentRow.user.name;
    //        if (!string.IsNullOrEmpty(commentRow.user.avatar))
    //        {
    //            bool isUrlContainsHttpAndHttps = SNS_APIManager.Instance.CheckUrlDropboxOrNot(commentRow.user.avatar);
    //            if (isUrlContainsHttpAndHttps)
    //            {
    //                AssetCache.Instance.EnqueueOneResAndWait(commentRow.user.avatar, commentRow.user.avatar, (success) =>
    //                {
    //                    if (success)
    //                    {
    //                        AssetCache.Instance.LoadSpriteIntoImage(profileImage, commentRow.user.avatar, changeAspectRatio: true);
    //                    }
    //                });
    //            }
    //            else
    //            {
    //                GetAndLoadMediaFile(commentRow.user.avatar);
    //            }
    //        }
    //    }
    //    descriptionText.text = SNS_APIManager.DecodedString(commentRow.comment);
    //    //timeText.text = commentRow.updatedAt.ToString();
    //    timeText.text = FeedUIController.Instance.GetConvertedTimeString(commentRow.updatedAt);
    //    //timeText.text = FeedUIController.Instance.GetConvertedTimeStringSpecifyKind(commentRow.updatedAt);
    //}


    //public void GetAndLoadMediaFile(string key)
    //{
    //    //Debug.LogError("GetAndLoadMediaFile: " + key);
    //    GetExtentionType(key);
    //    //Debug.LogError("currentExtention:   " + currentExtention);
    //    if (currentExtention == ExtentionType.Image)
    //    {
    //        GetImageFromAWS(key, profileImage);
    //    }
    //}

    //#region Get Image From AWS
    //public void GetImageFromAWS(string key, Image mainImage)
    //{
    //    //Debug.LogError("GetImageFromAWS key:" + key);
    //    if (AssetCache.Instance.HasFile(key))
    //    {
    //        //Debug.LogError("Chat Image Available on Disk");
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
    //            }
    //        });
    //    }
    //}

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
}