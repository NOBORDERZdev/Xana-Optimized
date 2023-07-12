using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SuperStar.Helpers;
using System.IO;
using System;
//using Amazon.S3.Model;
using UnityEngine.Networking;
public class FollowerItemController : MonoBehaviour
{
    public AllFollowersRows followerRawData;

    public TextMeshProUGUI userNameText;
    public Image profileImage;
    public TextMeshProUGUI followFollowingText;
    public Image followFollowingImage;
    public Color followColor, followingColor;    

    public Sprite defaultSP;

    private void Awake()
    {
        defaultSP = profileImage.sprite;
    }

    public int cnt = 0;
    private void OnEnable()
    {
        if (defaultSP != null)
        {
            profileImage.sprite = defaultSP;
        }
        if (cnt > 0 && !string.IsNullOrEmpty(followerRawData.follower.avatar))
        {
            if (AssetCache.Instance.HasFile(followerRawData.follower.avatar))
            {
                AssetCache.Instance.LoadSpriteIntoImage(profileImage, followerRawData.follower.avatar, changeAspectRatio: true);
            }
        }
        else
        {
            cnt += 1;
        }
    }

    private void OnDisable()
    {
        if (!string.IsNullOrEmpty(followerRawData.follower.avatar))
        {
            AssetCache.Instance.RemoveFromMemory(profileImage.sprite);
            profileImage.sprite = null;
            //Resources.UnloadUnusedAssets();//every clear.......
            //Caching.ClearCache();
            APIManager.Instance.ResourcesUnloadAssetFile();//UnloadUnusedAssets file call every 15 items.......
        }
    }

    public void SetupData(AllFollowersRows allFollowerRow)
    {
        followerRawData = allFollowerRow;

        userNameText.text = followerRawData.follower.name;
        if (!string.IsNullOrEmpty(followerRawData.follower.avatar))
        {
            bool isUrlContainsHttpAndHttps = APIManager.Instance.CheckUrlDropboxOrNot(followerRawData.follower.avatar);
            if (isUrlContainsHttpAndHttps)
            {
                AssetCache.Instance.EnqueueOneResAndWait(followerRawData.follower.avatar, followerRawData.follower.avatar, (success) =>
                {
                    if (success)
                    {
                        AssetCache.Instance.LoadSpriteIntoImage(profileImage, followerRawData.follower.avatar, changeAspectRatio: true);
                    }
                });
            }
            else
            {
                GetImageFromAWS(followerRawData.follower.avatar, profileImage);
            }
        }
        FollowFollowingSetUp(false);
    }

    public void OnClickUserProfileButton()
    {
        if (!PremiumUsersDetails.Instance.CheckSpecificItem("sns_feed",false))
        {
            //PremiumUsersDetails.Instance.PremiumUserUI.SetActive(true);
            print("Please Upgrade to Premium account");
            return;
        }
        else
        {
            print("Horayyy you have Access");
        }
        //if (XanaConstants.xanaConstants != null)
        //{
        //    if (XanaConstants.xanaConstants.r_isSNSComingSoonActive)
        //    {
        //        print("sns features coming soon.......");
        //        return;
        //    }
        //}
        //return;
        //Debug.LogError("Search User id:" + followerRawData.follower.id);

        AllUserWithFeedRow feedRawData = new AllUserWithFeedRow();
        feedRawData.id = followerRawData.follower.id;
        feedRawData.name = followerRawData.follower.name;
        feedRawData.avatar = followerRawData.follower.avatar;
        feedRawData.UserProfile = followerRawData.follower.userProfile;
        feedRawData.FollowerCount = followerRawData.followerCount;
        feedRawData.FollowingCount = followerRawData.followingCount;
        feedRawData.feedCount = followerRawData.feedCount;

        //FeedUIController.Instance.ShowLoader(true);

        OtherPlayerProfileData.Instance.currentFollowerItemScript = this;//assign current follower item script for other player profile

        OtherPlayerProfileData.Instance.FeedRawData = feedRawData;
        //OtherPlayerProfileData.Instance.OnSetUserUi(followerRawData.isFollowing);
        //OtherPlayerProfileData.Instance.LoadData();        

        OtherPlayerProfileData.Instance.backKeyManageList.Add("FollowerFollowingListScreen");//For back mamages.......

        //APIManager.Instance.RequestGetFeedsByUserId(followerRawData.follower.id, 1, 30, "OtherPlayerFeed");

        //this api get any user profile data and feed for other player profile....... 
        SingleUserProfileData singleUserProfileData = new SingleUserProfileData();
        singleUserProfileData.id = followerRawData.follower.id;
        singleUserProfileData.name = followerRawData.follower.name;
        singleUserProfileData.email = followerRawData.follower.email;
        singleUserProfileData.avatar = followerRawData.follower.avatar;
        singleUserProfileData.followerCount = followerRawData.followerCount;
        singleUserProfileData.followingCount = followerRawData.followingCount;
        singleUserProfileData.feedCount = followerRawData.feedCount;
        singleUserProfileData.isFollowing = followerRawData.isFollowing;

        SingleUserProfile singleUserProfile = new SingleUserProfile();
        singleUserProfileData.userProfile = singleUserProfile;
        if (followerRawData.follower.userProfile != null)
        {
            singleUserProfileData.userProfile.id = followerRawData.follower.userProfile.id;
            singleUserProfileData.userProfile.userId = followerRawData.follower.userProfile.userId;
            singleUserProfileData.userProfile.gender = followerRawData.follower.userProfile.gender;
            singleUserProfileData.userProfile.job = followerRawData.follower.userProfile.job;
            singleUserProfileData.userProfile.country = followerRawData.follower.userProfile.country;
            singleUserProfileData.userProfile.website = followerRawData.follower.userProfile.website;
            singleUserProfileData.userProfile.bio = followerRawData.follower.userProfile.bio;
        }

        OtherPlayerProfileData.Instance.RequestGetUserDetails(singleUserProfileData);
    }

    public void FollowFollowingSetUp(bool isFollowing)
    {
        if (isFollowing)
        {
            followFollowingText.text = TextLocalization.GetLocaliseTextByKey("Following");
            followFollowingImage.color = followingColor;

        }
        else
        {
            followFollowingText.text = TextLocalization.GetLocaliseTextByKey("Follow");
            followFollowingImage.color = followColor;
        }
        //  GameManager.Instance.LocalizeTextText(followFollowingText);
        //followFollowingText.GetComponent<TextLocalization>().LocalizeTextText();
    }

    #region Follow Following Button click and follow and unfollowing api.......
    //this method is used to Follow Following button click
    public void OnclickFollowFollowingButton()
    {
        /*if (followingRawData != null)
        {
            if (searchUserRow.isFollowing)
            {
                Debug.LogError("UnFollow User call:" + searchUserRow.id);
                FeedUIController.Instance.ShowLoader(true);//active api loader
                //unfollow
                RequestUnFollowAUser(searchUserRow.id.ToString());
            }
            else
            {
                Debug.LogError("Follow User call:" + searchUserRow.id);
                FeedUIController.Instance.ShowLoader(true);//active api loader
                //follow
                RequestFollowAUser(searchUserRow.id.ToString());
            }
        }*/
    }

    public void RequestFollowAUser(string user_Id)
    {
        StartCoroutine(IERequestFollowAUser(user_Id));
    }
    public IEnumerator IERequestFollowAUser(string user_Id)
    {
        WWWForm form = new WWWForm();
        form.AddField("userId", user_Id);

        using (UnityWebRequest www = UnityWebRequest.Post((ConstantsGod.API_BASEURL + ConstantsGod.r_url_FollowAUser), form))
        {
            www.SetRequestHeader("Authorization", APIManager.Instance.userAuthorizeToken);

            yield return www.SendWebRequest();

            FeedUIController.Instance.ShowLoader(false);//false api loader

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string data = www.downloadHandler.text;
                Debug.Log("follow user success data:" + data);

                //searchUserRow.isFollowing = true;
                FollowFollowingSetUp(true);

                //refresh Feed API.......
                APIController.Instance.RemoveFollowedUserFromHot(int.Parse(user_Id));
            }
        }
    }

    public void RequestUnFollowAUser(string user_Id)
    {
        StartCoroutine(IERequestUnFollowAUser(user_Id));
    }

    public IEnumerator IERequestUnFollowAUser(string user_Id)
    {
        WWWForm form = new WWWForm();
        form.AddField("userId", user_Id);

        using (UnityWebRequest www = UnityWebRequest.Post((ConstantsGod.API_BASEURL + ConstantsGod.r_url_UnFollowAUser), form))
        {
            www.SetRequestHeader("Authorization", APIManager.Instance.userAuthorizeToken);

            yield return www.SendWebRequest();

            FeedUIController.Instance.ShowLoader(false);//false api loader

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string data = www.downloadHandler.text;
                Debug.LogError("user unfollow success data:" + data);
                //searchUserRow.isFollowing = false;
                FollowFollowingSetUp(false);
            }
        }
    }
    #endregion

    #region Get Image From AWS
    public void GetImageFromAWS(string key, Image mainImage)
    {
        //Debug.LogError("GetImageFromAWS key:" + key);
        //GetExtentionType(key);
        if (AssetCache.Instance.HasFile(key))
        {
            //Debug.LogError("Chat Image Available on Disk");
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
        //Debug.LogError("ExtentionType: " + extension);
        if (extension == "png" || extension == "jpg" || extension == "jpeg" || extension == "gif" || extension == "bmp" || extension == "tiff" || extension == "heic")
        {
            currentExtention = ExtentionType.Image;
            return ExtentionType.Image;
        }
        else if (extension == "mp4" || extension == "mov" || extension == "wav" || extension == "avi")
        {
            currentExtention = ExtentionType.Video;
            // Debug.LogError("vvvvvvvvvvvvvvvvvvvvvvvvvvvv");
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
}