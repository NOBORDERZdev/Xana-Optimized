using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using SuperStar.Helpers;

public class FeedStoryAndCategoryItem : MonoBehaviour
{
    public AllFollowersRows followerData;

    public Image profileImage;
    public TextMeshProUGUI userNameText;

    public bool isVisible = false;
    float lastUpdateCallTime;

    private void OnDestroy()
    {
        if (!string.IsNullOrEmpty(followerData.follower.avatar))
        {
            AssetCache.Instance.RemoveFromMemory(profileImage.sprite);
            profileImage.sprite = null;
            SNS_APIManager.Instance.ResourcesUnloadAssetFile();//UnloadUnusedAssets file call every 15 items.......
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isVisible)//this is check if object is visible on camera then load feed or video one time
        {
            lastUpdateCallTime += Time.deltaTime;
            if (lastUpdateCallTime > 0.4f)//call every 0.4 sec
            {
                Vector3 mousePosNormal = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
                Vector3 mousePosNR = Camera.main.ScreenToViewportPoint(mousePosNormal);
                //Debug.LogError("NormalPos:" + mousePosNR);
                if (mousePosNR.x <= 1.05f)
                {
                    isVisible = false;
                    //Debug.LogError("Image download starting one time");
                    DownloadAndLoadImage();
                }

                lastUpdateCallTime = 0;
            }
        }
    }

    public void LoadData(AllFollowersRows allFollowersRows)
    {
        followerData = allFollowersRows;

        if (!string.IsNullOrEmpty(followerData.follower.name))
        {
            userNameText.text = followerData.follower.name;
        }
        else
        {
            userNameText.text = followerData.follower.name;
        }

        isVisible = true;
        /*if (!SNS_APIManager.Instance.isTestDefaultToken)
        {
            isVisible = true;
        }
        else//only for test else part
        {
            DownloadAndLoadImage();
        }*/
    }

    //this method is used to Download and Load image.......
    void DownloadAndLoadImage()
    {
        if (!string.IsNullOrEmpty(followerData.follower.avatar))//rik for avatar user
        {
            bool isAvatarUrlFromDropbox = SNS_APIManager.Instance.CheckUrlDropboxOrNot(followerData.follower.avatar);
            //Debug.LogError("isAvatarUrlFromDropbox: " + isAvatarUrlFromDropbox + " :name:" + FeedsByFollowingUserRowData.User.Name);
            if (isAvatarUrlFromDropbox)
            {
                AssetCache.Instance.EnqueueOneResAndWait(followerData.follower.avatar, followerData.follower.avatar, (success) =>
                {
                    if (success)
                        AssetCache.Instance.LoadSpriteIntoImage(profileImage, followerData.follower.avatar, changeAspectRatio: true);
                });
            }
            else
            {
                GetImageFromAWS(followerData.follower.avatar, profileImage);//Get image from aws and save/load into asset cache.......
            }
        }
    }

    #region Get Image From AWS
    public void GetImageFromAWS(string key, Image mainImage)
    {
        //Debug.LogError("GetImageFromAWS key:" + key);
        GetExtentionType(key);
        if (AssetCache.Instance.HasFile(key))
        {
            //Debug.LogError("Image Available on Disk");
            AssetCache.Instance.LoadSpriteIntoImage(mainImage, key, changeAspectRatio: true);
            return;
        }
        else
        {
            AssetCache.Instance.EnqueueOneResAndWait(key, (ConstantsGod.r_AWSImageKitBaseUrl + key), (success) =>
            {
                if (success)
                {
                    AssetCache.Instance.LoadSpriteIntoImage(mainImage, key, changeAspectRatio: true);
                }
            });
        }
    }

    public static ExtentionType currentExtention;
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
    }
    #endregion
}