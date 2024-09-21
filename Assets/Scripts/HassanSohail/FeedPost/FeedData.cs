using SuperStar.Helpers;
using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using DG.Tweening;

public class FeedData : MonoBehaviour
{
   [SerializeField] Sprite defaultProfileImage;
   [SerializeField] Image ProfileImage;
   [SerializeField] TMP_Text DisplayName;
   [SerializeField] TMP_Text PostText;
   [SerializeField] TMP_Text Date;
   [SerializeField] TMP_Text Likes;
   [SerializeField] Image Heart;
   [SerializeField] Sprite LikedHeart;
   [SerializeField] Sprite UnLikedHeart;
   [SerializeField] Color LikedColor;
   [SerializeField] Color UnLikedColor;
   [SerializeField] Button LikeBtn;
    //private bool isFeedScreen =true;
    public FeedResponseRow _data;
    bool isLiked = false;
    bool isEnable = false;
    int timeUpdateInterval = 1;
    FeedScroller scrollerController;
    public bool isProfileScene = false;
    public static event Action<BackButtonHandler.screenTabs> OnScreenTabStateChange;
    public void SetFeedPrefab(FeedResponseRow data, bool isFeed = true ){
        if (gameObject.activeInHierarchy)
        {
            _data = data;
            if (data.text_post != "null" && !(string.IsNullOrEmpty(data.text_post)))
            {
                DisplayName.text = data.user.name;
                if (DisplayName.text.Length > 15)
                {
                    DisplayName.text = DisplayName.text.Substring(0, 15) + "...";
                }
                PostText.text = data.text_post;
                isEnable = true;
                //Likes.text = data.like_count.ToString();
                UpdateLikeCount(data.like_count);
                timeUpdateInterval = 1;
                if (isEnable)
                {
                    gameObject.GetComponent<FeedData>().StopAllCoroutines();
                    Date.text = CalculateTimeDifference(Convert.ToDateTime(_data.createdAt)).ToString();
                }

                if (data.isLikedByUser)
                {
                    isLiked = true;
                    Likes.color = LikedColor;
                }
                else isLiked = false;
                UpdateHeart();
                if (!String.IsNullOrEmpty(data.user.avatar) && !data.user.avatar.Equals("null"))
                {
                    StartCoroutine(GetProfileImage(data.user.avatar));
                }
                else
                {
                    ProfileImage.sprite = defaultProfileImage;
                }

                //isFeedScreen = !isFeed; //To assign back data to prefab items in case of no pooling in OnEnable
                //if (isFeed)
                //{
                //    Invoke(nameof(HieghtListUpdateWithDelay), 0.08f);
                //}
                //else
                //{
                //    Invoke(nameof(SetProfileFeedWithWait), 0.08f);
                //}
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

    void SetProfileFeedWithWait()
    {
         PostHieghtUpdateForProfileVisit();
    }


    public void onclickFeedUserProfileButton()
    {
        if (isProfileScene)
            return;
        //print("Getting Click here" + _data.user_id);
        SNS_APIManager.Instance.GetFeedUserProfileData<FeedData>(_data.user_id, this);
        OnScreenTabStateChange?.Invoke(BackButtonHandler.screenTabs.FeedProfile);
    }

    public void SetupFeedUserProfile(SearchUserRow _feedUserData)
    {
        //print("Getting Click here name" + _feedUserData.name);
        //Debug.Log("Search User id:" + _feedUserData.id);
        int bodyLayer = LayerMask.NameToLayer("Body");
        SNS_APIManager.Instance.RequestGetUserLatestAvatarData<FeedData>(_feedUserData.id.ToString(), this);
        if (MyProfileDataManager.Instance)
        {
            MyProfileDataManager.Instance.OtherPlayerdataObj.SetActive(true);
            OtherPlayerProfileData.Instance.ResetMainScrollDefaultTopPos();
            MyProfileDataManager.Instance.myProfileScreen.SetActive(true);
            FeedUIController.Instance.profileFollowerFollowingListScreen.SetActive(false);
            MyProfileDataManager.Instance.gameObject.SetActive(false);
            FeedUIController.Instance.AddFriendPanel.SetActive(false);
        }
        else
        {
            OtherPlayerProfileData.Instance.myPlayerdataObj.SetActive(false);
            OtherPlayerProfileData.Instance.ResetMainScrollDefaultTopPos();
            OtherPlayerProfileData.Instance.myPlayerdataObj.GetComponent<MyProfileDataManager>().myProfileScreen.SetActive(true);
            //MyProfileDataManager.Instance.myProfileScreen.SetActive(true);
            FeedUIController.Instance.profileFollowerFollowingListScreen.SetActive(false);
            FeedUIController.Instance.AddFriendPanel.SetActive(false);
            //MyProfileDataManager.Instance.gameObject.SetActive(false);
        }
        ProfileUIHandler.instance._renderTexCamera.GetComponent<Camera>().cullingMask &= ~(1 << bodyLayer);
        ProfileUIHandler.instance.SwitchBetweenUserAndOtherProfileUI(false);
        ProfileUIHandler.instance.SetMainScrollRefs();
        ProfileUIHandler.instance.editProfileBtn.SetActive(false);
        if (_feedUserData.am_i_following)
        {
            ProfileUIHandler.instance.followProfileBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Unfollow";
        }
        else
        {
            ProfileUIHandler.instance.followProfileBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Follow";
        }
        ProfileUIHandler.instance.followProfileBtn.SetActive(true);
        //ProfileUIHandler.instance.SetUserAvatarDefaultClothing();

        AllUserWithFeedRow feedRawData = new AllUserWithFeedRow();
        feedRawData.id = _feedUserData.id;
        feedRawData.name = _feedUserData.name;
        feedRawData.avatar = _feedUserData.avatar;
        feedRawData.UserProfile = _feedUserData.userProfile;
        feedRawData.FollowerCount = _feedUserData.followerCount;
        feedRawData.FollowingCount = _feedUserData.followingCount;
        feedRawData.feedCount = _feedUserData.feedCount;

        //FeedUIController.Instance.ShowLoader(true);

        //OtherPlayerProfileData.Instance.currentFindFriendWithNameItemScript = this;

        //OtherPlayerProfileData.Instance.FeedRawData = feedRawData;
        ////OtherPlayerProfileData.Instance.OnSetUserUi(_feedUserData.isFollowing);
        ////OtherPlayerProfileData.Instance.LoadData();

        //OtherPlayerProfileData.Instance.backKeyManageList.Add("FindFriendScreen");//For back mamages.......

        //SNS_APIManager.Instance.RequesturlGetTaggedFeedsByUserId(FeedRawData.id, 1, FeedRawData.feedCount);//rik cmnt
        //SNS_APIManager.Instance.RequestGetFeedsByUserId(_feedUserData.id, 1, 30, "OtherPlayerFeed");

        //this api get any user profile data and feed for other player profile....... 
        SingleUserProfileData singleUserProfileData = new SingleUserProfileData();
        singleUserProfileData.id = _feedUserData.id;
        singleUserProfileData.name = _feedUserData.name;
        singleUserProfileData.email = "";
        singleUserProfileData.avatar = _feedUserData.avatar;
        singleUserProfileData.followerCount = _feedUserData.followerCount;
        singleUserProfileData.followingCount = _feedUserData.followingCount;
        singleUserProfileData.feedCount = _feedUserData.feedCount;
        singleUserProfileData.isFollowing = _feedUserData.is_following_me;
        singleUserProfileData.userOccupiedAssets = _feedUserData.userOccupiedAssets;
        print("_feedUserData occupied asstes count: " + _feedUserData.userOccupiedAssets.Count);
        print("singleUserProfileData occupied asstes count: " + singleUserProfileData.userOccupiedAssets.Count);

        SingleUserProfile singleUserProfile = new SingleUserProfile();
        singleUserProfileData.userProfile = singleUserProfile;
        if (_feedUserData.userProfile != null)
        {
            singleUserProfileData.userProfile.id = _feedUserData.userProfile.id;
            singleUserProfileData.userProfile.userId = _feedUserData.userProfile.userId;
            singleUserProfileData.userProfile.gender = _feedUserData.userProfile.gender;
            singleUserProfileData.userProfile.job = _feedUserData.userProfile.job;
            singleUserProfileData.userProfile.country = _feedUserData.userProfile.country;
            singleUserProfileData.userProfile.website = _feedUserData.userProfile.website;
            singleUserProfileData.userProfile.bio = _feedUserData.userProfile.bio;
        }
        OtherPlayerProfileData.Instance.RequestGetUserDetails(singleUserProfileData, true);
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

    void HieghtListUpdateWithDelay(){
        scrollerController.AddInHeightList(_data.id, gameObject.transform.GetChild(0).gameObject.GetComponent<RectTransform>().CalculateHeight());
        RectTransform rectTemp = gameObject.transform.GetChild(0).gameObject.GetComponent<RectTransform>();
        Vector2 temp = new Vector2(rectTemp.rect.width , rectTemp.rect.height );

       gameObject.transform.GetComponent<LayoutElement>().DOMinSize(temp, 0.8f, true) ;
       //gameObject.GetComponent<LayoutElement>().minHeight = gameObject.transform.GetChild(0).gameObject.GetComponent<RectTransform>().CalculateHeight();
      // scrollerController.scroller.ReloadData();
     }

    void PostHieghtUpdateForProfileVisit()
    {
        RectTransform rectTemp = gameObject.transform.GetChild(0).gameObject.GetComponent<RectTransform>();
        Vector2 temp = new Vector2(rectTemp.rect.width, rectTemp.rect.height);

        gameObject.transform.GetComponent<LayoutElement>().DOMinSize(temp, 0.8f, true);
    }

    public string CalculateTimeDifference(DateTime postTime)
   {
        if (isEnable && gameObject.activeInHierarchy)
        {
            DateTime currentTime = DateTime.Now;
            TimeSpan timeDifference = currentTime - postTime;
            StartCoroutine(ReCallingTimeDifference(postTime));
            if (timeDifference.TotalMinutes <= 1){
                timeUpdateInterval =1;
                return $"{Math.Floor(timeDifference.TotalSeconds)}s";
            }
            else if (timeDifference.TotalMinutes <= 60)
            {
                timeUpdateInterval =60;
                return $"{Math.Floor(timeDifference.TotalMinutes)}m";
            }
            else if (timeDifference.TotalHours <= 24)
            {
                timeUpdateInterval =3600;
                return $"{Math.Floor(timeDifference.TotalHours)}h";
            }
            else if (timeDifference.TotalDays < 30){
                timeUpdateInterval =86400;
                return $"{Math.Floor(timeDifference.TotalDays)}d"; 
             }
            else if (timeDifference.TotalDays <= 365)
            {
                 timeUpdateInterval =86400;
                return $"{Math.Floor(timeDifference.TotalDays / 30)}mo";
            }
            else
            {
                timeUpdateInterval =86400;
                return $"{Math.Floor(timeDifference.TotalDays / 365)}y";
            }
        }
        else
        {
            return "";
        }
    }

    IEnumerator ReCallingTimeDifference(DateTime postTime){
        yield return new WaitForSecondsRealtime(timeUpdateInterval);
        Date.text = CalculateTimeDifference(postTime).ToString(); 
    }
    IEnumerator GetProfileImage(string url)
    {
        yield return new WaitForSeconds(1);
        if (!string.IsNullOrEmpty(url))
        {
           // bool isUrlContainsHttpAndHttps = SNS_APIManager.Instance.CheckUrlDropboxOrNot(url);
             AssetCache.Instance.EnqueueOneResAndWait(url, url, (success) =>
             {
                    if (success)
                    {
                        AssetCache.Instance.LoadSpriteIntoImage(ProfileImage, url, changeAspectRatio: true);
                    }
              });
            //if (isUrlContainsHttpAndHttps)
            //{
            //    AssetCache.Instance.EnqueueOneResAndWait(url, url, (success) =>
            //    {
            //        if (success)
            //        {
            //            AssetCache.Instance.LoadSpriteIntoImage(ProfileImage, url, changeAspectRatio: true);
            //        }
            //    });
            //}
            //else
            //{
            //    GetImageFromAWS(url, ProfileImage);
            //}
        }
        //string newUrl = url+"?width=256&height=256";
        //using (WWW www = new WWW(url))
        //{
        //    yield return www;
        //    if (ProfileImage != null && www.texture!= null)
        //    {
        //        ProfileImage.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), Vector2.zero);
        //    }
        //    www.Dispose();
        //}
     }


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

    public void LikeUnlikePost()
    {
        LikeBtn.interactable = false;
        StartCoroutine(LikeUnLike());
    }

    IEnumerator LikeUnLike()
    {
        string url = ConstantsGod.API_BASEURL +ConstantsGod.FeedLikeDislikePost;
        int feedId = _data.id;
        WWWForm form = new WWWForm();
        form.AddField("textPostId", feedId);
        using (UnityWebRequest www = UnityWebRequest.Post(url,form))
        {
            www.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            www.SendWebRequest();
            while(!www.isDone)
            {
                yield return null;
            }
            if (www.isNetworkError || www.isHttpError)
            {
                LikeBtn.interactable= true;
                StartCoroutine(LikeUnLike());
            }
            else
            {  
                LikeResponse likeResponse = JsonUtility.FromJson<LikeResponse>(www.downloadHandler.text);
                UpdateLikeCount(likeResponse.data.likeCount);
                //Likes.text =  likeResponse.data.likeCount.ToString();
               
                if(!isProfileScene)
                    isLiked = !isLiked;
                else
                {
                    _data.isLikedByUser = !_data.isLikedByUser;
                    isLiked = _data.isLikedByUser;
                }

                if (scrollerController)
                    scrollerController.updateLikeCount(feedId,likeResponse.data.likeCount,isLiked);
                UpdateHeart();
                LikeBtn.interactable= true;
            }
        }
    }

    void UpdateHeart()
    {
        if (isLiked)
        {
            Heart.sprite = LikedHeart;
            Likes.color = LikedColor;
        }
        else
        {
            Heart.sprite = UnLikedHeart;
            Likes.color = UnLikedColor;
        }
    }

    public void UnPoolPrefab(){ 
        _data=null;
        isLiked = false;
        UpdateHeart();
    }

    public int GetFeedId(){
        return _data.id;
    }

    public void UpdateLikeCount(int count){
        if (count > -1)
            Likes.text = count.ToString();
        else
            Likes.text = "0";
    }

    public void SetFeedUiController(FeedScroller controller){ 
        scrollerController = controller;    
    }

    private void OnEnable()
    {
        //print("ON ENABLE GETTING CALLED: " + isFeedScreen);
        if (/*isProfileScene && */_data.user_id != 0)
        {
            if (isProfileScene)
            {
                SetFeedPrefab(_data, false);
            }
            else
            {
                SetFeedPrefab(_data);
            }
            // Sending back same data to initialize prefab elements in case of no pooling
        }
    }

    private void OnDisable()
    {
        ProfileImage.sprite= defaultProfileImage;
        DisplayName.text = "";
        PostText.text = "";
        Date.text = "";
        Likes.text = "";
        Likes.color = UnLikedColor;
        Heart.sprite = UnLikedHeart;
        timeUpdateInterval =1;
        isLiked = false;
        isEnable = false;
        gameObject.GetComponent<FeedData>().StopAllCoroutines();
    }

}

[Serializable]
public class LikeResponse  { 
    public bool success;
    public likeCountClass data;
    public string msg;
}

[Serializable]
public class likeCountClass  { 
    public int likeCount;
}

