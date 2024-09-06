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

public class FollowingItemController : MonoBehaviour
{
    public AllFollowingRow followingRawData;

    public TextMeshProUGUI userNameText;
    public TextMeshProUGUI BioText;

    public Image profileImage;
    public TextMeshProUGUI followFollowingText;
    public Image followFollowingImage;
    public Color followColor, followingColor;

    [SerializeField] GameObject MakeBfBtn;
    [SerializeField] GameObject RemoveBfBtn;
    public static event Action<BackButtonHandler.screenTabs> OnScreenTabStateChange;

    public Sprite defaultSP;
    int userId ;
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
        if (cnt > 0 && !string.IsNullOrEmpty(followingRawData.following.avatar))
        {
            if (AssetCache.Instance.HasFile(followingRawData.following.avatar))
            {
                AssetCache.Instance.LoadSpriteIntoImage(profileImage, followingRawData.following.avatar, changeAspectRatio: true);
            }
        }
        else
        {
            cnt += 1;
        }
    }

    private void OnDisable()
    {
        if (!string.IsNullOrEmpty(followingRawData.following.avatar))
        {
            AssetCache.Instance.RemoveFromMemory(profileImage.sprite);
            profileImage.sprite = null;
            //Resources.UnloadUnusedAssets();//every clear.......
            //Caching.ClearCache();
            SNS_APIManager.Instance.ResourcesUnloadAssetFile();//UnloadUnusedAssets file call every 15 items.......
        }
    }

    public void SetupData(AllFollowingRow allFollowingRow, bool isFromProfile= true, bool _emptyElement = false)
    {
        if (!_emptyElement)
        {
            followingRawData = allFollowingRow;
            userNameText.text = followingRawData.following.name;
            if (BioText != null)
            {
                //BioText.text = followingRawData.following.userProfile.bio;
                if (followingRawData.following != null && followingRawData.following.userProfile != null && !string.IsNullOrEmpty(followingRawData.following.userProfile.bio))
                {
                    BioText.text = SNS_APIManager.DecodedString(followingRawData.following.userProfile.bio);
                }
                else
                {
                    BioText.text = "";
                }
            }
            if (!string.IsNullOrEmpty(followingRawData.following.avatar))
            {
                bool isUrlContainsHttpAndHttps = SNS_APIManager.Instance.CheckUrlDropboxOrNot(followingRawData.following.avatar);
                if (isUrlContainsHttpAndHttps)
                {
                    AssetCache.Instance.EnqueueOneResAndWait(followingRawData.following.avatar, followingRawData.following.avatar, (success) =>
                    {
                        if (success)
                        {
                            AssetCache.Instance.LoadSpriteIntoImage(profileImage, followingRawData.following.avatar, changeAspectRatio: true);
                        }
                    });
                }
                else
                {
                    GetImageFromAWS(followingRawData.following.avatar, profileImage);
                }
            }
            if (isFromProfile)
            {
                FollowFollowingSetUp(false);
            }
            UpdateBfBtn(true, allFollowingRow.following.is_close_friend);
            //UpdateBfBtn(allFollowingRow.following.is_close_friend);
        }
        else
        {
            profileImage.transform.parent.gameObject.SetActive(!_emptyElement);
            followFollowingImage.gameObject.SetActive(!_emptyElement);
            userNameText.text = "";
            BioText.text = "";
        }
    }

    public void OnClickUserProfileButton()
    {
        print("Follower id :" + followingRawData.following.id);
        FeedUIController.Instance.ShowLoader(true);
        SNS_APIManager.Instance.RequestGetUserLatestAvatarData<FollowingItemController>(followingRawData.following.id.ToString(), this);
        MyProfileDataManager.Instance.OtherPlayerdataObj.SetActive(true);
        OtherPlayerProfileData.Instance.ResetMainScrollDefaultTopPos();
        MyProfileDataManager.Instance.myProfileScreen.SetActive(true);
        OtherPlayerProfileData.Instance.myPlayerdataObj.SetActive(false);
        ProfileUIHandler.instance.SwitchBetweenUserAndOtherProfileUI(false);
        ProfileUIHandler.instance.SetMainScrollRefs();
        ProfileUIHandler.instance.editProfileBtn.SetActive(false);
            ProfileUIHandler.instance.followProfileBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Unfollow";
        ProfileUIHandler.instance.followProfileBtn.SetActive(true);
        ProfileUIHandler.instance.SetUserAvatarDefaultClothing();
        //if ( followingRawData.userOccupiedAssets.Count > 0 )
        //{
        //    ProfileUIHandler.instance.SetUserAvatarClothing(followingRawData.userOccupiedAssets[0].json);
        //}
        //else
        //{
        //    ProfileUIHandler.instance.SetUserAvatarDefaultClothing();
        //}
        if (!UserPassManager.Instance.CheckSpecificItem("sns_feed", false))
        {
            //UserPassManager.Instance.PremiumUserUI.SetActive(true);
            print("Please Upgrade to Premium account");
            return;
        }
        else
        {
            print("Horayyy you have Access");
        }
        //if (ConstantsHolder.xanaConstants != null)
        //{
        //    if (ConstantsHolder.xanaConstants.r_isSNSComingSoonActive)
        //    {
        //        print("sns features coming soon.......");
        //        return;
        //    }
        //}
        //return;
       Debug.Log("Search User id:" + followingRawData.following.id);

        AllUserWithFeedRow feedRawData = new AllUserWithFeedRow();
        feedRawData.id = followingRawData.following.id;
        feedRawData.name = followingRawData.following.name;
        feedRawData.avatar = followingRawData.following.avatar;
        feedRawData.UserProfile = followingRawData.following.userProfile;
        feedRawData.FollowerCount = followingRawData.followerCount;
        feedRawData.FollowingCount = followingRawData.followingCount;
        feedRawData.feedCount = followingRawData.feedCount;

        //FeedUIController.Instance.ShowLoader(true);

       OtherPlayerProfileData.Instance.currentFollowingItemScript = this;
        //OtherPlayerProfileData.Instance.currentFollowingItemScript = this;//assign current following item script for other player profile

        OtherPlayerProfileData.Instance.FeedRawData = feedRawData;
        //OtherPlayerProfileData.Instance.OnSetUserUi(followingRawData.isFollowing);
        //OtherPlayerProfileData.Instance.LoadData();       

        OtherPlayerProfileData.Instance.backKeyManageList.Add("FollowerFollowingListScreen");//For back mamages.......

        //SNS_APIManager.Instance.RequestGetFeedsByUserId(followingRawData.following.id, 1, 30, "OtherPlayerFeed");

        //this api get any user profile data and feed for other player profile....... 
        SingleUserProfileData singleUserProfileData = new SingleUserProfileData();
        singleUserProfileData.id = followingRawData.following.id;
        singleUserProfileData.name = followingRawData.following.name;
        singleUserProfileData.email = followingRawData.following.email;
        singleUserProfileData.avatar = followingRawData.following.avatar;
        singleUserProfileData.followerCount = followingRawData.followerCount;
        singleUserProfileData.followingCount = followingRawData.followingCount;
        singleUserProfileData.feedCount = followingRawData.feedCount;
        singleUserProfileData.isFollowing = followingRawData.isFollowing;

        SingleUserProfile singleUserProfile = new SingleUserProfile();
        singleUserProfileData.userProfile = singleUserProfile;
        if (followingRawData.following.userProfile != null)
        {
            singleUserProfileData.userProfile.id = followingRawData.following.userProfile.id;
            singleUserProfileData.userProfile.userId = followingRawData.following.userProfile.userId;
            singleUserProfileData.userProfile.gender = followingRawData.following.userProfile.gender;
            singleUserProfileData.userProfile.job = followingRawData.following.userProfile.job;
            singleUserProfileData.userProfile.country = followingRawData.following.userProfile.country;
            singleUserProfileData.userProfile.website = followingRawData.following.userProfile.website;
            singleUserProfileData.userProfile.bio = followingRawData.following.userProfile.bio;
        }

        OtherPlayerProfileData.Instance.RequestGetUserDetails(singleUserProfileData);
        OnScreenTabStateChange?.Invoke(BackButtonHandler.screenTabs.FriendsProfile);
    }

    public void DressUpUserAvatar()
    {
        ////Other player avatar initialization required here
        if (SNS_APIManager.Instance.VisitedUserAvatarData != null)
        {
            ProfileUIHandler.instance.SetUserAvatarClothing(SNS_APIManager.Instance.VisitedUserAvatarData.json);
        }
        else
        {
            ProfileUIHandler.instance.SetUserAvatarDefaultClothing();
        }
    }

    public void FollowFollowingSetUp(bool isFollowing)
    {
        if (isFollowing)
        {
            followFollowingText.text = TextLocalization.GetLocaliseTextByKey("Following");
            followFollowingImage.color = followingColor;
            UpdateBfBtn(false);

        }
        else
        {
            followFollowingText.text = TextLocalization.GetLocaliseTextByKey("Follow");
            followFollowingImage.color = followColor;
            MakeBfBtn.SetActive(false);
            RemoveBfBtn.SetActive(false);
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
               Debug.Log("UnFollow User call:" + searchUserRow.id);
                FeedUIController.Instance.ShowLoader(true);//active api loader
                //unfollow
                RequestUnFollowAUser(searchUserRow.id.ToString());
            }
            else
            {
               Debug.Log("Follow User call:" + searchUserRow.id);
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
            www.SetRequestHeader("Authorization", SNS_APIManager.Instance.userAuthorizeToken);

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
                //SNS_APIController.Instance.RemoveFollowedUserFromHot(int.Parse(user_Id));
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
            www.SetRequestHeader("Authorization", SNS_APIManager.Instance.userAuthorizeToken);

            yield return www.SendWebRequest();

            FeedUIController.Instance.ShowLoader(false);//false api loader

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string data = www.downloadHandler.text;
               Debug.Log("user unfollow success data:" + data);
                //searchUserRow.isFollowing = false;
                FollowFollowingSetUp(false);
                if (SNS_APIManager.Instance.BFCount > 0)
                    SNS_APIManager.Instance.BFCount -= 1;
            }
        }
    }
    #endregion

    #region Get Image From AWS
    public void GetImageFromAWS(string key, Image mainImage)
    {
        //Debug.Log("GetImageFromAWS key:" + key);
        //GetExtentionType(key);
        if (AssetCache.Instance.HasFile(key))
        {
            //Debug.Log("Chat Image Available on Disk");
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
        //Debug.Log("ExtentionType: " + extension);
        if (extension == "png" || extension == "jpg" || extension == "jpeg" || extension == "gif" || extension == "bmp" || extension == "tiff" || extension == "heic")
        {
            currentExtention = ExtentionType.Image;
            return ExtentionType.Image;
        }
        else if (extension == "mp4" || extension == "mov" || extension == "wav" || extension == "avi")
        {
            currentExtention = ExtentionType.Video;
            //Debug.Log("vvvvvvvvvvvvvvvvvvvvvvvvvvvv");
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


    /// <summary>
    /// To Add Following in BFF list
    /// </summary>
    public void AddBff(){ 
        SNS_APIManager.Instance.AddBestFriend(followingRawData.userId,gameObject);
        GameManager.Instance.FriendsHomeManager.GetComponent<FriendHomeManager>().AddFriendToHome();
    }

    /// <summary>
    /// To Remove BFF that already are in BFF
    /// </summary>
    public void RemoveBff(){ 
          SNS_APIManager.Instance.RemoveBestFriend(followingRawData.userId,gameObject);
        GameManager.Instance.FriendsHomeManager.GetComponent<FriendHomeManager>().RemoveFriendFromHome(followingRawData.userId);
    }

    public void UpdateBfBtn(bool isBf){
        if (MakeBfBtn== null || RemoveBfBtn == null)
        {
            return;
        }
        if (isBf)
        {
            MakeBfBtn.SetActive(false);
            RemoveBfBtn.SetActive(true);
        }
        else
        {
            MakeBfBtn.SetActive(true);
            RemoveBfBtn.SetActive(false);
        }
       
    }
    public void UpdateBfBtn(bool isfollowing,bool isCloseFriend)
    {
        if (MakeBfBtn == null || RemoveBfBtn == null)
        {
            return;
        }
        if (isfollowing && isCloseFriend)
        {
            MakeBfBtn.SetActive(false);
            RemoveBfBtn.SetActive(true);
        }
        else if(isfollowing && !isCloseFriend)
        {
            MakeBfBtn.SetActive(true);
            RemoveBfBtn.SetActive(false);
        }
        else
        {
            MakeBfBtn.SetActive(false);
            RemoveBfBtn.SetActive(false);
        }

    }
}