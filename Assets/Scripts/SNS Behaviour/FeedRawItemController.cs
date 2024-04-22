using SuperStar.Helpers;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FeedRawItemController : MonoBehaviour
{
    public AllUserWithFeedRow FeedRawData;

    public Transform hotItemPrefabParent;
    public Image profileImage;
    public TextMeshProUGUI feedTitle;
    public TextMeshProUGUI feedPlayerName;
    public TextMeshProUGUI feedLike;
    public TextMeshProUGUI tottlePostText;

    public Image followButtonImage;
    public Sprite followSprite, followingSprite;
    public TextMeshProUGUI followText;
    public Color followtextColor, FollowingTextColor;
    public bool isFollow = false;
    public Sprite defultProfileImage;

    private void OnDestroy()
    {
        if (!string.IsNullOrEmpty(FeedRawData.avatar))
        {
            AssetCache.Instance.RemoveFromMemory(profileImage.sprite);
            profileImage.sprite = null;
            SNS_APIManager.Instance.ResourcesUnloadAssetFile();//UnloadUnusedAssets file call every 15 items.......
        }
    }

    public void ClearMororyAfterDestroyObject()
    {
        if (!string.IsNullOrEmpty(FeedRawData.avatar))
        {
            AssetCache.Instance.RemoveFromMemory(profileImage.sprite);
            profileImage.sprite = null;
        }
    }

    public void LoadFeed(AllUserWithFeedRow allUserWithFeedRow)
    {
        FeedRawData = allUserWithFeedRow;

        if (!string.IsNullOrEmpty(FeedRawData.avatar))
        {
            bool isAvatarUrlFromDropbox = SNS_APIManager.Instance.CheckUrlDropboxOrNot(FeedRawData.avatar);
            //Debug.LogError("isAvatarUrlFromDropbox: " + isAvatarUrlFromDropbox + " :name:" + FeedsByFollowingUserRowData.User.Name);
            if (isAvatarUrlFromDropbox)
            {
                AssetCache.Instance.EnqueueOneResAndWait(FeedRawData.avatar, FeedRawData.avatar, (success) =>
                {
                    if (success)
                        AssetCache.Instance.LoadSpriteIntoImage(profileImage, FeedRawData.avatar, changeAspectRatio: true);
                });
            }
            else
            {
                GetImageFromAWS(FeedRawData.avatar, profileImage);//Get image from aws and save/load into asset cache.......
            }
        }

        feedPlayerName.text = FeedRawData.name;
        //Debug.LogError("FeedRawItemController00000:" + FeedRawData.feedCount + "  :Posts");
        tottlePostText.text = (FeedRawData.feedCount + TextLocalization.GetLocaliseTextByKey("Posts"));
        //Debug.LogError("FeedRawItemController11111:" + FeedRawData.feedCount + "  :Posts");
        int tottlehotFeeditem;
        if (FeedRawData.feeds.Count > 3)
        {
            tottlehotFeeditem = 3;
        }
        else
        {
            tottlehotFeeditem = FeedRawData.feeds.Count;
        }

        //for (int i = 0; i < tottlehotFeeditem; i++)
        //{
        //    GameObject hotFeedFeedObject = Instantiate(SNS_APIController.Instance.hotItemPrefab, hotItemPrefabParent);
        //    hotFeedFeedObject.GetComponent<FeedItemController>().FeedData = FeedRawData.feeds[i];
        //    hotFeedFeedObject.GetComponent<FeedItemController>().FeedRawData = FeedRawData;
        //    hotFeedFeedObject.GetComponent<FeedItemController>().LoadFeed();
        //    hotFeedFeedObject.name = "HotColloection_" + FeedRawData.feeds[i].id.ToString();
        //}
    }

    public void OnClickCheckOtherPlayerProfile()
    {
        //Debug.LogError("id ;" + FeedRawData.id + "count :" + FeedRawData.feedCount);
        //FeedUIController.Instance.ShowLoader(true);

        SNS_APIController.Instance.currentFeedRawItemController = this;

        OtherPlayerProfileData.Instance.FeedRawData = FeedRawData;
        //OtherPlayerProfileData.Instance.OnSetUserUi(isFollow);
        //OtherPlayerProfileData.Instance.LoadData();

        OtherPlayerProfileData.Instance.backKeyManageList.Add("HotTabScreen");//For back mamages.......

        //SNS_APIManager.Instance.RequestGetFeedsByUserId(FeedRawData.id, 1, 30, "OtherPlayerFeed");

        //this api get any user profile data and feed for other player profile....... 
        SingleUserProfileData singleUserProfileData = new SingleUserProfileData();
        singleUserProfileData.id = FeedRawData.id;
        singleUserProfileData.name = FeedRawData.name;
        singleUserProfileData.email = FeedRawData.email;
        singleUserProfileData.avatar = FeedRawData.avatar;
        singleUserProfileData.followerCount = FeedRawData.FollowerCount;
        singleUserProfileData.followingCount = FeedRawData.FollowingCount;
        singleUserProfileData.feedCount = FeedRawData.feedCount;

        SingleUserProfile singleUserProfile = new SingleUserProfile();
        singleUserProfileData.userProfile = singleUserProfile;
        if (FeedRawData.UserProfile != null)
        {
            singleUserProfileData.userProfile.id = FeedRawData.UserProfile.id;
            singleUserProfileData.userProfile.userId = FeedRawData.UserProfile.userId;
            singleUserProfileData.userProfile.gender = FeedRawData.UserProfile.gender;
            singleUserProfileData.userProfile.job = FeedRawData.UserProfile.job;
            singleUserProfileData.userProfile.country = FeedRawData.UserProfile.country;
            singleUserProfileData.userProfile.website = FeedRawData.UserProfile.website;
            singleUserProfileData.userProfile.bio = FeedRawData.UserProfile.bio;
        }

        OtherPlayerProfileData.Instance.RequestGetUserDetails(singleUserProfileData);
    }

    public void OnClickFollowUserButton()
    {
        //Debug.LogError("OnClickFollowUser id" + FeedRawData.id.ToString() + " :FollowText:" + followText.text);
        if (followText.text == "Follow" || followText.text == "フォロー")
        {
            FeedUIController.Instance.ShowLoader(true);

            SNS_APIController.Instance.currentFeedRawItemController = this;
            SNS_APIManager.Instance.RequestFollowAUser(FeedRawData.id.ToString(), "Feed");
        }
    }

    public void OnFollowUserSuccessful()
    {
        followButtonImage.sprite = followingSprite;
        followText.text = TextLocalization.GetLocaliseTextByKey("Following");
        followText.color = FollowingTextColor;

        //SNS_APIController.Instance.RemoveFollowedUserFromHot(FeedRawData.id);
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