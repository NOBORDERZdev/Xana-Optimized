using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SuperStar.Helpers;
using System.IO;
using System;
using UnityEngine.Networking;

public class FindFriendWithNameItem : MonoBehaviour
{
    public SearchUserRow searchUserRow;

    public TextMeshProUGUI userNameText;
    public TextMeshProUGUI userBioText;
    public Image profileImage;
    public TextMeshProUGUI followFollowingText;
    public Image followFollowingImage;
    public Color followColor, followingColor;
    public Color followingTextColor,followTextColor;

    public Sprite defaultSP;

    [SerializeField] GameObject MakeBfBtn;
    [SerializeField] GameObject RemoveBfBtn;
    public bool IsInFollowingTab;
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
        if (cnt > 0 && !string.IsNullOrEmpty(searchUserRow.avatar))
        {
            if (AssetCache.Instance.HasFile(searchUserRow.avatar))
            {
                AssetCache.Instance.LoadSpriteIntoImage(profileImage, searchUserRow.avatar, changeAspectRatio: true);
            }
        }
        else
        {
            cnt += 1;
        }
    }

    private void OnDisable()
    {
        if (!string.IsNullOrEmpty(searchUserRow.avatar))
        {
            AssetCache.Instance.RemoveFromMemory(profileImage.sprite);
            profileImage.sprite = null;
            //Resources.UnloadUnusedAssets();//every clear.......
            //Caching.ClearCache();
            APIManager.Instance.ResourcesUnloadAssetFile();//UnloadUnusedAssets file call every 15 items.......
        }
    }

    public void SetupData(SearchUserRow searchUserRow1, bool isFromSearch = false)
    {
        searchUserRow = searchUserRow1;

        userNameText.text = searchUserRow.name;
        if (searchUserRow.userProfile != null && !string.IsNullOrEmpty(searchUserRow.userProfile.bio)){ 
            userBioText.text =  APIManager.DecodedString(searchUserRow.userProfile.bio);
        }
        else
        {
            userBioText.text = "";
        }
        if (!string.IsNullOrEmpty(searchUserRow.avatar))
        {
            bool isUrlContainsHttpAndHttps = APIManager.Instance.CheckUrlDropboxOrNot(searchUserRow.avatar);
            if (isUrlContainsHttpAndHttps)
            {
                AssetCache.Instance.EnqueueOneResAndWait(searchUserRow.avatar, searchUserRow.avatar, (success) =>
                {
                    if (success)
                    {
                        AssetCache.Instance.LoadSpriteIntoImage(profileImage, searchUserRow.avatar, changeAspectRatio: true);
                    }
                });
            }
            else
            {
                GetImageFromAWS(searchUserRow.avatar, profileImage);
            }
        }
        FollowFollowingSetUp(searchUserRow.am_i_following);
        if (searchUserRow.am_i_following ||searchUserRow.is_following_me )
        {
           UpdateBfBtn(searchUserRow.is_my_close_friend);
        }
       
    }

    public void OnClickUserProfileButton()
    {
       Debug.Log("Search User id:" + searchUserRow.id);

        AllUserWithFeedRow feedRawData = new AllUserWithFeedRow();
        feedRawData.id = searchUserRow.id;
        feedRawData.name = searchUserRow.name;
        feedRawData.avatar = searchUserRow.avatar;
        feedRawData.UserProfile = searchUserRow.userProfile;
        feedRawData.FollowerCount = searchUserRow.followerCount;
        feedRawData.FollowingCount = searchUserRow.followingCount;
        feedRawData.feedCount = searchUserRow.feedCount;

        //FeedUIController.Instance.ShowLoader(true);

        OtherPlayerProfileData.Instance.currentFindFriendWithNameItemScript = this;

        OtherPlayerProfileData.Instance.FeedRawData = feedRawData;
        //OtherPlayerProfileData.Instance.OnSetUserUi(searchUserRow.isFollowing);
        //OtherPlayerProfileData.Instance.LoadData();

        OtherPlayerProfileData.Instance.backKeyManageList.Add("FindFriendScreen");//For back mamages.......

        //APIManager.Instance.RequesturlGetTaggedFeedsByUserId(FeedRawData.id, 1, FeedRawData.feedCount);//rik cmnt
        //APIManager.Instance.RequestGetFeedsByUserId(searchUserRow.id, 1, 30, "OtherPlayerFeed");

        //this api get any user profile data and feed for other player profile....... 
        SingleUserProfileData singleUserProfileData = new SingleUserProfileData();
        singleUserProfileData.id = searchUserRow.id;
        singleUserProfileData.name = searchUserRow.name;
        singleUserProfileData.email = "";
        singleUserProfileData.avatar = searchUserRow.avatar;
        singleUserProfileData.followerCount = searchUserRow.followerCount;
        singleUserProfileData.followingCount = searchUserRow.followingCount;
        singleUserProfileData.feedCount = searchUserRow.feedCount;
        singleUserProfileData.isFollowing = searchUserRow.is_following_me;

        SingleUserProfile singleUserProfile = new SingleUserProfile();
        singleUserProfileData.userProfile = singleUserProfile;
        if (searchUserRow.userProfile != null)
        {
            singleUserProfileData.userProfile.id = searchUserRow.userProfile.id;
            singleUserProfileData.userProfile.userId = searchUserRow.userProfile.userId;
            singleUserProfileData.userProfile.gender = searchUserRow.userProfile.gender;
            singleUserProfileData.userProfile.job = searchUserRow.userProfile.job;
            singleUserProfileData.userProfile.country = searchUserRow.userProfile.country;
            singleUserProfileData.userProfile.website = searchUserRow.userProfile.website;
            singleUserProfileData.userProfile.bio = searchUserRow.userProfile.bio;
        }

        OtherPlayerProfileData.Instance.RequestGetUserDetails(singleUserProfileData);
    }

    public void FollowFollowingSetUp(bool isFollowing)
    {
        if (isFollowing)
        {
            followFollowingText.text = TextLocalization.GetLocaliseTextByKey("Following");
            followFollowingImage.color = followingColor;
            followFollowingText.color= followingTextColor;
        }
        else
        {
            followFollowingText.text = TextLocalization.GetLocaliseTextByKey("Follow");
            followFollowingImage.color = followColor;
            followFollowingText.color= followTextColor;
        }
        //  GameManager.Instance.LocalizeTextText(followFollowingText);
        //followFollowingText.GetComponent<TextLocalization>().LocalizeTextText();
    }

    #region Follow Following Button click and follow and unfollowing api.......
    //this method is used to Follow Following button click
    public void OnclickFollowFollowingButton()
    {
        if (searchUserRow != null)
        {
            if (searchUserRow.am_i_following)
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
        }
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

           // FeedUIController.Instance.ShowLoader(false);//false api loader

            if (www.isNetworkError || www.isHttpError)
            {
                if (FeedUIController.Instance != null)
                {
                    FeedUIController.Instance.ShowLoader(false);
                }
                Debug.Log(www.error);
            }
            else
            {
                string data = www.downloadHandler.text;
                Debug.Log("follow user success data:" + data);

                searchUserRow.is_following_me = true;
                searchUserRow.am_i_following = true;
                FollowFollowingSetUp(true);
                if (FeedUIController.Instance != null)
                {
                    FeedUIController.Instance.ShowLoader(false);
                }
                //refresh Feed API.......
                APIController.Instance.RemoveFollowedUserFromHot(int.Parse(user_Id));

                FeedUIController.Instance.FollowingAddAndRemoveUnFollowedUser(int.Parse(user_Id), false);
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
            print("www" + www.downloadHandler);
            //FeedUIController.Instance.ShowLoader(false);//false api loader

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                if (FeedUIController.Instance != null)
                {
                    FeedUIController.Instance.ShowLoader(false);
                }
            }
            else
            {
                string data = www.downloadHandler.text;
               Debug.Log("user unfollow success data:" + data);
                searchUserRow.is_following_me = false;
                searchUserRow.am_i_following = false;
                //FollowFollowingSetUp(false);
                if (FeedUIController.Instance != null)
                {
                    FeedUIController.Instance.ShowLoader(false);
                }
                FeedUIController.Instance.FollowingAddAndRemoveUnFollowedUser(int.Parse(user_Id), true);
                if (IsInFollowingTab)
                {
                    FeedUIController.Instance.CheckFollowingCount();
                }
                GameManager.Instance.FriendsHomeManager.GetComponent<FriendHomeManager>().RemoveFriendFromHome(int.Parse(user_Id));
                this.gameObject.SetActive(false);
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
            return;
        }
    }

    public void OnClickUnFollowAndRefershAdFrndFollowing()
    {
        if (GetComponent<FollowingItemController>())
        {
            FeedUIController.Instance.ShowLoader(true);
            RequestUnFollowAUser(GetComponent<FollowingItemController>().followingRawData.userId.ToString());
            //FeedUIController.Instance.OnClickAddFriendFollowing();
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
        APIManager.Instance.AddBestFriend(searchUserRow.id,gameObject);
        GameManager.Instance.FriendsHomeManager.GetComponent<FriendHomeManager>().AddFriendToHome();
    }

    /// <summary>
    /// To Remove BFF that already are in BFF
    /// </summary>
    public void RemoveBff(){ 
          APIManager.Instance.RemoveBestFriend(searchUserRow.id,gameObject);
        GameManager.Instance.FriendsHomeManager.GetComponent<FriendHomeManager>().RemoveFriendFromHome(searchUserRow.id);
    }

    public void UpdateBfBtn(bool isBf){
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

    public void OffBffBtns(){ 
        MakeBfBtn.SetActive(false);
        RemoveBfBtn.SetActive(false);
    }
}