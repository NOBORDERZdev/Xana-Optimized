using RenderHeads.Media.AVProVideo;
using SuperStar.Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System;
//using Amazon.S3.Model;
using System.Linq;

public class FeedVideoItem : MonoBehaviour
{ }
//    public AllUserWithFeed FeedData;

//    public AllUserWithFeedRow FeedRawData;

//    public FeedsByFollowingUser feedUserData;

//    public HotFeed hotFeed;

//    public Image imgFeed, profileImage;
//    public GameObject PhotoImage;

//    public MediaPlayer feedMediaPlayer;
//    public GameObject videoDisplay;
//    public GameObject commentBtn;
//    public GameObject likeCountBtn;
//    public Color likeSelectionColor;

//    public TextMeshProUGUI userName;
//    public TextMeshProUGUI descriptionText;

//    public TextMeshProUGUI timeText;

//    [Space]
//    public bool isVideoOrImage = false;
//    public bool isImageSuccessDownloadAndSave = false;
//    public bool isReleaseFromMemoryOrNot = false;
//    public bool isOnScreen = false;//check object is on screen or not

//    public bool isVisible = false;
//    float lastUpdateCallTime;

//    public Sprite defaultProfileSP;

//    private void Start()
//    {
//        float value0 = FeedUIController.Instance.feedVideoButtonPanelImage.rect.width;
//        float value1 = FeedUIController.Instance.feedVideoButtonPanelImage.rect.height;
//        //Debug.Log("height : " + Screen.height + "width " + (Screen.height - 150) + ":value:"+value1);
//        this.GetComponent<RectTransform>().sizeDelta = new Vector2(value0, value1);
//    }

//    private void OnEnable()
//    {
//        RefreshDecTextContentSizeFitter();
//    }

//    private void OnDisable()
//    {
//        if (!isVideoOrImage)
//        {
//            AssetCache.Instance.RemoveFromMemory(imgFeed.sprite);
//            imgFeed.sprite = null;
//        }
//        else
//        {
//            feedMediaPlayer.CloseMedia();
//        }

//        if (!string.IsNullOrEmpty(FeedRawData.avatar))
//        {
//            AssetCache.Instance.RemoveFromMemory(profileImage.sprite);
//            profileImage.sprite = null;
//        }
//        SNS_APIManager.Instance.ResourcesUnloadAssetFile();
//    }

//    private void Update()//delete image after object out of screen
//    {
//        /*if (SNS_APIManager.Instance.isTestDefaultToken)//for direct SNS Scene Test....... 
//        {
//            return;
//        }*/

//        lastUpdateCallTime += Time.deltaTime;
//        if (lastUpdateCallTime > 0.3f)//call every 0.4 sec
//        {
//            Vector3 mousePosNormal = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
//            Vector3 mousePosNR = Camera.main.ScreenToViewportPoint(mousePosNormal);

//            if (mousePosNR.y >= -0.4f && mousePosNR.y <= 1.4f)
//            {
//                isOnScreen = true;
//            }
//            else
//            {
//                isOnScreen = false;
//            }

//            lastUpdateCallTime = 0;
//        }

//        if (isVisible && isOnScreen)//this is check if object is visible on camera then load feed or video one time
//        {
//            isVisible = false;
//            //Debug.Log("Image download starting one time");
//            DownloadAndLoadFeed();
//        }
//        //else if(isImageSuccessDownloadAndSave && isPrintDebug)
//        else if (isImageSuccessDownloadAndSave)
//        {
//            if (isOnScreen)
//            {
//                if (isReleaseFromMemoryOrNot)
//                {
//                    isReleaseFromMemoryOrNot = false;
//                    //re load from asset 
//                    //Debug.Log("Re Download Image and play video");
//                    if (!isVideoOrImage)//if false then load image otherwise play video
//                    {
//                        if (AssetCache.Instance.HasFile(FeedData.image))
//                        {
//                            AssetCache.Instance.LoadSpriteIntoImage(imgFeed, FeedData.image, changeAspectRatio: true);
//                        }
//                        else
//                        {
//                            isVisible = true;
//                        }
//                        if (!string.IsNullOrEmpty(FeedRawData.avatar))
//                        {
//                            if (AssetCache.Instance.HasFile(FeedRawData.avatar))
//                            {
//                                AssetCache.Instance.LoadSpriteIntoImage(profileImage, FeedRawData.avatar, changeAspectRatio: true);
//                            }
//                        }
//                    }
//                    else
//                    {
//                        feedMediaPlayer.Play();
//                        Debug.Log("feedMediaPlayer.Play");
//                    }
//                }
//            }
//            else
//            {
//                if (!isReleaseFromMemoryOrNot)
//                {
//                    //realse from memory 
//                    isReleaseFromMemoryOrNot = true;
//                    //Debug.Log("remove from memory and stop video");
//                    if (!isVideoOrImage)//if false then Remove image otherwise Stop video
//                    {
//                        AssetCache.Instance.RemoveFromMemory(imgFeed.sprite);
//                        imgFeed.sprite = null;
//                        if (!string.IsNullOrEmpty(FeedRawData.avatar))
//                        {
//                            AssetCache.Instance.RemoveFromMemory(profileImage.sprite);
//                            profileImage.sprite = null;
//                            profileImage.sprite = defaultProfileSP;
//                        }
//                        Resources.UnloadUnusedAssets();//Every clear
//                        //Caching.ClearCache();
//                        //GC.Collect();
//                        //SNS_APIManager.Instance.ResourcesUnloadAssetFile();//UnloadUnusedAssets file call every 15 items.......
//                    }
//                    else
//                    {
//                        feedMediaPlayer.Stop();
//                        Debug.Log("feedMediaPlayer.Stop");
//                    }
//                }
//            }
//        }
//    }

//    public void LoadFeed()
//    {
//        descriptionText.text = "";

//        if (FeedRawData != null)
//        {
//            feedUserData.Id = FeedRawData.id;
//            feedUserData.Name = FeedRawData.name;
//            feedUserData.Email = FeedRawData.email;
//            feedUserData.Avatar = FeedRawData.avatar;
//        }
//        userName.text = SNS_APIManager.DecodedString(FeedData.title);
//        //TRiken
//        if (hotFeed != null)
//        {
//            feedUserData.Id = hotFeed.id;
//            feedUserData.Name = hotFeed.user.name;
//            feedUserData.Email = hotFeed.user.email;
//            feedUserData.Avatar = hotFeed.user.avatar;
//            userName.text = SNS_APIManager.DecodedString(hotFeed.title);
//        }

//        Invoke("UserNameSizeSetUp", 0.2f);

//        if (hotFeed != null)
//        {
//            //for comment count.......
//            CommentCountUISetup(hotFeed.commentCount);
//            //for like count.......
//            LikeCountAndUISetup(hotFeed.likeCount);
//        }
//        else
//        {
//            //for comment count.......
//            CommentCountUISetup(FeedData.commentCount);
//            //for like count.......
//            LikeCountAndUISetup(FeedData.likeCount);
//        }


//        if (!string.IsNullOrEmpty(FeedData.descriptions))//new
//        {
//            descriptionText.text = SNS_APIManager.DecodedString(FeedData.descriptions);
//            SetupDecPart(descriptionText.text);
//        }
//        else
//        {
//            descriptionText.text = "";
//            seeMoreDecObj.SetActive(false);
//        }

//        if (FeedData.updatedAt != null)
//        {
//            timeText.text = FeedUIController.Instance.GetConvertedTimeString(FeedData.updatedAt);
//        }
//        if (hotFeed.updatedAt != null)
//        {
//            timeText.text = FeedUIController.Instance.GetConvertedTimeString(hotFeed.updatedAt);

//        }
//        //feedTitle.text = FeedData.title;
//        //feedLike.text = FeedData.likeCount.ToString();

//        GetShareURL();

//        isVisible = true;
//        /*if (!SNS_APIManager.Instance.isTestDefaultToken)
//        {
//            isVisible = true;
//        }
//        else//only for test else part
//        {
//            DownloadAndLoadFeed();
//        }*/
//    }

//    void UserNameSizeSetUp()
//    {
//        if (userName.GetComponent<RectTransform>().sizeDelta.x > 550f)
//        {
//            userName.GetComponent<LayoutElement>().preferredWidth = 550f;
//        }
//    }

//    void DownloadAndLoadFeed()
//    {
//        //Riken
//        /*if (!string.IsNullOrEmpty(FeedRawData.avatar))//set avatar image.......
//        {
//            bool isAvatarUrlFromDropbox = SNS_APIManager.Instance.CheckUrlDropboxOrNot(FeedRawData.avatar);
//            //Debug.Log("isAvatarUrlFromDropbox: " + isAvatarUrlFromDropbox + " :name:" + FeedsByFollowingUserRowData.User.Name);
//            if (isAvatarUrlFromDropbox)
//            {
//                AssetCache.Instance.EnqueueOneResAndWait(FeedRawData.avatar, FeedRawData.avatar, (success) =>
//                {
//                    if (success)
//                        AssetCache.Instance.LoadSpriteIntoImage(profileImage, FeedRawData.avatar, changeAspectRatio: true);
//                });
//            }
//            else
//            {
//                GetImageFromAWS(FeedRawData.avatar, profileImage);//Get image from aws and save/load into asset cache.......
//            }
//        }

//        if (!string.IsNullOrEmpty(FeedData.image))
//        {
//            bool isImageUrlFromDropbox = SNS_APIManager.Instance.CheckUrlDropboxOrNot(FeedData.image);
//            //Debug.Log("isImageUrlFromDropbox:  " + isImageUrlFromDropbox + " :name:" + FeedsByFollowingUserRowData.User.Name);
//            if (isImageUrlFromDropbox)
//            {
//                AssetCache.Instance.EnqueueOneResAndWait(FeedData.image, FeedData.image, (success) =>
//                {
//                    if (success)
//                    {
//                        AssetCache.Instance.LoadSpriteIntoImage(imgFeed, FeedData.image, changeAspectRatio: true);
//                        isImageSuccessDownloadAndSave = true;
//                    }
//                });
//            }
//            else
//            {
//                GetImageFromAWS(FeedData.image, imgFeed);//Get image from aws and save/load into asset cache.......
//            }

//            PhotoImage.SetActive(true);
//            feedMediaPlayer.gameObject.SetActive(false);
//            videoDisplay.gameObject.SetActive(false);
//        }
//        else if (!string.IsNullOrEmpty(FeedData.video))
//        {
//            bool isVideoUrlFromDropbox = SNS_APIManager.Instance.CheckUrlDropboxOrNot(FeedData.video);
//            isVideoOrImage = true;

//            //Debug.Log("FeedData.video " + FeedData.video);
//            PhotoImage.SetActive(false);
//            videoDisplay.gameObject.SetActive(true);
//            feedMediaPlayer.gameObject.SetActive(true);

//            if (isVideoUrlFromDropbox)
//            {
//                bool videoPlay = feedMediaPlayer.OpenMedia(new MediaPath(FeedData.video, MediaPathType.AbsolutePathOrURL), autoPlay: true);
//                //feedMediaPlayer.Play();
//                if (videoPlay)
//                {
//                    isImageSuccessDownloadAndSave = true;
//                }
//            }
//            else
//            {
//                GetVideoUrl(FeedData.video);//get video url from aws and play.......
//            }
//        }*/
//        if (!string.IsNullOrEmpty(hotFeed.user.avatar))//set avatar image.......
//        {
//            bool isAvatarUrlFromDropbox = SNS_APIManager.Instance.CheckUrlDropboxOrNot(hotFeed.user.avatar);
//            //Debug.Log("isAvatarUrlFromDropbox: " + isAvatarUrlFromDropbox + " :name:" + FeedsByFollowingUserRowData.User.Name);
//            if (isAvatarUrlFromDropbox)
//            {
//                AssetCache.Instance.EnqueueOneResAndWait(hotFeed.user.avatar, hotFeed.user.avatar, (success) =>
//                {
//                    if (success)
//                        AssetCache.Instance.LoadSpriteIntoImage(profileImage, hotFeed.user.avatar, changeAspectRatio: true);
//                });
//            }
//            else
//            {
//                GetImageFromAWS(hotFeed.user.avatar, profileImage);//Get image from aws and save/load into asset cache.......
//            }
//        }

//        if (!string.IsNullOrEmpty(hotFeed.image))
//        {
//            bool isImageUrlFromDropbox = SNS_APIManager.Instance.CheckUrlDropboxOrNot(hotFeed.image);
//            //Debug.Log("isImageUrlFromDropbox:  " + isImageUrlFromDropbox + " :name:" + FeedsByFollowingUserRowData.User.Name);
//            if (isImageUrlFromDropbox)
//            {
//                AssetCache.Instance.EnqueueOneResAndWait(hotFeed.image, hotFeed.image, (success) =>
//                {
//                    if (success)
//                    {
//                        AssetCache.Instance.LoadSpriteIntoImage(imgFeed, hotFeed.image, changeAspectRatio: true);
//                        isImageSuccessDownloadAndSave = true;
//                    }
//                });
//            }
//            else
//            {
//                GetImageFromAWS(hotFeed.image, imgFeed);//Get image from aws and save/load into asset cache.......
//            }

//            PhotoImage.SetActive(true);
//            feedMediaPlayer.gameObject.SetActive(false);
//            videoDisplay.gameObject.SetActive(false);
//        }
//        else if (!string.IsNullOrEmpty(hotFeed.video))
//        {
//            bool isVideoUrlFromDropbox = SNS_APIManager.Instance.CheckUrlDropboxOrNot(hotFeed.video);
//            isVideoOrImage = true;

//            //Debug.Log("FeedData.video " + FeedData.video);
//            PhotoImage.SetActive(false);
//            videoDisplay.gameObject.SetActive(true);
//            feedMediaPlayer.gameObject.SetActive(true);

//            if (isVideoUrlFromDropbox)
//            {
//                bool videoPlay = feedMediaPlayer.OpenMedia(new MediaPath(hotFeed.video, MediaPathType.AbsolutePathOrURL), autoPlay: true);
//                //feedMediaPlayer.Play();
//                if (videoPlay)
//                {
//                    isImageSuccessDownloadAndSave = true;
//                }
//            }
//            else
//            {
//                GetVideoUrl(hotFeed.video);//get video url from aws and play.......
//            }
//        }
//    }

//    //this method is used to user profile button click.......
//    public void OnClickPlayerProfileButton()
//    {
//       Debug.Log("Feed Id :" + FeedData.id + "   :User Id :" + FeedRawData.id);
//        //Riken
//        /*OtherPlayerProfileData.Instance.FeedRawData = FeedRawData;
//        //OtherPlayerProfileData.Instance.OnSetUserUi(isFollow);
//        //OtherPlayerProfileData.Instance.LoadData();

//        OtherPlayerProfileData.Instance.backKeyManageList.Add("HotTabScreen");//For back mamages.......

//        //SNS_APIManager.Instance.RequestGetFeedsByUserId(FeedRawData.id, 1, 30, "OtherPlayerFeed");

//        //this api get any user profile data and feed for other player profile....... 
//        SingleUserProfileData singleUserProfileData = new SingleUserProfileData();
//        singleUserProfileData.id = FeedRawData.id;
//        singleUserProfileData.name = FeedRawData.name;
//        singleUserProfileData.email = FeedRawData.email;
//        singleUserProfileData.avatar = FeedRawData.avatar;
//        singleUserProfileData.followerCount = FeedRawData.FollowerCount;
//        singleUserProfileData.followingCount = FeedRawData.FollowingCount;
//        singleUserProfileData.feedCount = FeedRawData.feedCount;

//        SingleUserProfile singleUserProfile = new SingleUserProfile();
//        singleUserProfileData.userProfile = singleUserProfile;
//        if (FeedRawData.UserProfile != null)
//        {
//            singleUserProfileData.userProfile.id = FeedRawData.UserProfile.id;
//            singleUserProfileData.userProfile.userId = FeedRawData.UserProfile.userId;
//            singleUserProfileData.userProfile.gender = FeedRawData.UserProfile.gender;
//            singleUserProfileData.userProfile.job = FeedRawData.UserProfile.job;
//            singleUserProfileData.userProfile.country = FeedRawData.UserProfile.country;
//            singleUserProfileData.userProfile.website = FeedRawData.UserProfile.website;
//            singleUserProfileData.userProfile.bio = FeedRawData.UserProfile.bio;
//        }*/
//        //Riken
//        if (hotFeed.user.id == MyProfileDataManager.Instance.myProfileData.id)
//        {
//            FeedUIController.Instance.feedFullViewScreenCallingFrom = "FeedPage";
//            FeedUIController.Instance.OnClickVideoItemBackButton(false);
//        }
//        else
//        {
//            //FeedUIController.Instance.feedVideoScreen.SetActive(false);
//            OtherPlayerProfileData.Instance.RequestGetUserDetails(hotFeed.user.id);
//        }
//        //OtherPlayerProfileData.Instance.RequestGetUserDetails(singleUserProfileData);
//    }

//    #region Get Image And Video From AWS
//    public void GetVideoUrl(string key)
//    {
//        /*var request_1 = new GetPreSignedUrlRequest()
//        {
//            BucketName = AWSHandler.Instance.Bucketname,
//            Key = key,
//            Expires = DateTime.Now.AddHours(6)
//        };
//       //Debug.Log("Feed Video file sending url request:" + AWSHandler.Instance._s3Client);

//        AWSHandler.Instance._s3Client.GetPreSignedURLAsync(request_1, (callback) =>
//        {
//            if (callback.Exception == null)
//            {
//                string mediaUrl = callback.Response.Url;
//                UnityToolbag.Dispatcher.Invoke(() =>
//                {
//                    if (this.isActiveAndEnabled)
//                    {
//                        //Debug.Log("Feed Video URL " + mediaUrl);
//                        bool videoPlay = feedMediaPlayer.OpenMedia(new MediaPath(mediaUrl, MediaPathType.AbsolutePathOrURL), autoPlay: true);
//                        //feedMediaPlayer.Play();
//                        if (videoPlay)
//                        {
//                            isImageSuccessDownloadAndSave = true;
//                        }
//                    }
//                });
//            }
//            else
//               Debug.Log(callback.Exception);
//        });*/

//        if (key != "")
//        {
//            string mediaUrl = "";

//            if (key.Contains("https"))
//            {
//                mediaUrl = key;
//            }
//            else
//            {
//                mediaUrl = ConstantsGod.AWS_VIDEO_BASE_URL + key;
//            }

//            Debug.Log($"<color=green> Video Key = FeedVideoItem : </color>{mediaUrl}");
//            UnityToolbag.Dispatcher.Invoke(() =>
//            {
//                if (this.isActiveAndEnabled)
//                {
//                    //Debug.Log("Feed Video URL " + mediaUrl);
//                    bool videoPlay = feedMediaPlayer.OpenMedia(new MediaPath(mediaUrl, MediaPathType.AbsolutePathOrURL), autoPlay: true);
//                    //feedMediaPlayer.Play();
//                    if (videoPlay)
//                    {
//                        isImageSuccessDownloadAndSave = true;
//                    }
//                }
//            });
//        }
//    }

//    public void GetImageFromAWS(string key, Image mainImage)
//    {
//        //Debug.Log("GetImageFromAWS key:" + key);
//        GetExtentionType(key);
//        if (AssetCache.Instance.HasFile(key))
//        {
//            //Debug.Log("Image Available on Disk");
//            AssetCache.Instance.LoadSpriteIntoImage(mainImage, key, changeAspectRatio: true);
//            if (mainImage == imgFeed)
//            {
//                isImageSuccessDownloadAndSave = true;
//            }
//            return;
//        }
//        else
//        {
//            AssetCache.Instance.EnqueueOneResAndWait(key, (ConstantsGod.r_AWSImageKitBaseUrl + key), (success) =>
//            {
//                if (success)
//                {
//                    AssetCache.Instance.LoadSpriteIntoImage(mainImage, key, changeAspectRatio: true);
//                    if (mainImage == imgFeed)
//                    {
//                        isImageSuccessDownloadAndSave = true;
//                    }
//                }
//            });
//        }
//    }

//    public static ExtentionType currentExtention;
//    public static ExtentionType GetExtentionType(string path)
//    {
//        if (string.IsNullOrEmpty(path))
//            return (ExtentionType)0;

//        string extension = Path.GetExtension(path);
//        if (string.IsNullOrEmpty(extension))
//            return (ExtentionType)0;

//        if (extension[0] == '.')
//        {
//            if (extension.Length == 1)
//                return (ExtentionType)0;

//            extension = extension.Substring(1);
//        }

//        extension = extension.ToLowerInvariant();
//        //Debug.Log("ExtentionType " + extension);
//        if (extension == "png" || extension == "jpg" || extension == "jpeg" || extension == "gif" || extension == "bmp" || extension == "tiff" || extension == "heic")
//        {
//            currentExtention = ExtentionType.Image;
//            return ExtentionType.Image;
//        }
//        else if (extension == "mp4" || extension == "mov" || extension == "wav" || extension == "avi")
//        {
//            currentExtention = ExtentionType.Video;
//            return ExtentionType.Video;
//        }
//        else if (extension == "mp3" || extension == "aac" || extension == "flac")
//        {
//            currentExtention = ExtentionType.Audio;
//            return ExtentionType.Audio;
//        }
//        return (ExtentionType)0;
//    }
//    #endregion

//    #region See more and lee dec method and reference
//    [Space]
//    [Header("See More Dec Reference")]
//    public GameObject seeMoreDecObj;
//    public GameObject seeMoreButtonTextObj;
//    public GameObject seeLessButtonTextObj;

//    public ContentSizeFitter userDetailsContentSizeFitter;

//    public void OnClickDecSeeMoreLessButton()
//    {
//        if (seeMoreButtonTextObj.activeSelf)
//        {
//            //Riken
//            //descriptionText.text = SNS_APIManager.DecodedString(FeedData.descriptions);
//            descriptionText.text = SNS_APIManager.DecodedString(hotFeed.descriptions);
//            SeeMoreLessBioTextSetup(false);
//        }
//        else
//        {
//            descriptionText.text = tempMinDecLineStr;
//            SeeMoreLessBioTextSetup(true);
//        }

//        RefreshDecTextContentSizeFitter();
//    }

//    void RefreshDecTextContentSizeFitter()
//    {
//        if (waitToRefreshCo != null)
//        {
//            StopCoroutine(waitToRefreshCo);
//        }

//        waitToRefreshCo = StartCoroutine(WaitToRefreshUserDetails());
//    }

//    Coroutine waitToRefreshCo;
//    IEnumerator WaitToRefreshUserDetails()
//    {
//        userDetailsContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
//        yield return new WaitForSeconds(0.01f);
//        userDetailsContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
//    }

//    void SeeMoreLessBioTextSetup(bool isSeeMore)
//    {
//        seeMoreButtonTextObj.SetActive(isSeeMore);
//        seeLessButtonTextObj.SetActive(!isSeeMore);
//    }

//    string tempMinDecLineStr = "";
//    public void SetupDecPart(string strText)
//    {
//        int numLines = strText.Split('\n').Length;
//        //Debug.Log("Des Line Count:" + numLines);

//        if (numLines > 2)
//        {
//            string[] subLineSTR = strText.Split('\n').Take(2).ToArray();
//            //Debug.Log("Result:" + subLineSTR);

//            tempMinDecLineStr = "";
//            for (int i = 0; i < subLineSTR.Length; i++)
//            {
//                tempMinDecLineStr += subLineSTR[i] + "\n";
//            }
//            descriptionText.text = tempMinDecLineStr;

//            SeeMoreLessBioTextSetup(true);
//            seeMoreDecObj.SetActive(true);
//        }
//        else
//        {
//            //false see more button
//            seeMoreDecObj.SetActive(false);
//        }

//        if (this.gameObject.activeInHierarchy)
//        {
//            RefreshDecTextContentSizeFitter();
//        }
//    }
//    #endregion

//    #region Share Image or Video Referense or methods.......

//    public string shareMediaUrl = "";
//    //this method is used to get Share media url from AWS.......
//    void GetShareURL()
//    {
//        //Riken
//        /*if (!string.IsNullOrEmpty(FeedData.image))
//        {
//            bool isImageUrlFromDropbox = SNS_APIManager.Instance.CheckUrlDropboxOrNot(FeedData.image);
//            if (isImageUrlFromDropbox)
//            {
//                shareMediaUrl = FeedData.image;
//            }
//            else
//            {
//                GetFeedUrl(FeedData.image);
//            }
//        }
//        else if (!string.IsNullOrEmpty(FeedData.video))
//        {
//            bool isVideoUrlFromDropbox = SNS_APIManager.Instance.CheckUrlDropboxOrNot(FeedData.video);
//            if (isVideoUrlFromDropbox)
//            {
//                shareMediaUrl = FeedData.video;
//            }
//            else
//            {
//                GetFeedUrl(FeedData.video);
//            }
//        }*/
//        if (!string.IsNullOrEmpty(hotFeed.image))
//        {
//            bool isImageUrlFromDropbox = SNS_APIManager.Instance.CheckUrlDropboxOrNot(hotFeed.image);
//            if (isImageUrlFromDropbox)
//            {
//                shareMediaUrl = hotFeed.image;
//            }
//            else
//            {
//                GetFeedUrl(hotFeed.image);
//            }
//        }
//        else if (!string.IsNullOrEmpty(hotFeed.video))
//        {
//            bool isVideoUrlFromDropbox = SNS_APIManager.Instance.CheckUrlDropboxOrNot(hotFeed.video);
//            if (isVideoUrlFromDropbox)
//            {
//                shareMediaUrl = hotFeed.video;
//            }
//            else
//            {
//                GetFeedUrl(hotFeed.video);
//            }
//        }
//    }

//    public void GetFeedUrl(string key)
//    {
//        /*string tempFeedURl = "";
//        var request_1 = new GetPreSignedUrlRequest()
//        {
//            BucketName = AWSHandler.Instance.Bucketname,
//            Key = key,
//            Expires = DateTime.Now.AddHours(6)
//        };
//        AWSHandler.Instance._s3Client.GetPreSignedURLAsync(request_1, (callback) =>
//        {
//            if (callback.Exception == null)
//            {
//                tempFeedURl = callback.Response.Url;
//                string[] feedUrlArr = tempFeedURl.Split(char.Parse("?"));
//                shareMediaUrl = feedUrlArr[0];
//            }
//            else
//               Debug.Log(callback.Exception);
//        });*/

//        if (key.Contains("https://"))
//        {
//            shareMediaUrl = key;
//        }
//        else
//        {
//            string tempFeedURl = ConstantsGod.AWS_VIDEO_BASE_URL + key;
//            string[] feedUrlArr = tempFeedURl.Split(char.Parse("?"));
//            shareMediaUrl = feedUrlArr[0];
//        }
//    }

//    //this method is used to share button click.......
//    public void OnClickShareButton()
//    {
//        new NativeShare().AddFile("Scoial Image")
//       .SetSubject("Subject goes here").SetText("Xana App!").SetUrl(shareMediaUrl)
//       .SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
//       .Share();
//    }
//    #endregion

//    #region Comment Button Methods.......
//    //this method is used to Set Comment count on text....... 
//    public void CommentCountUISetup(int commentCount)
//    {
//        string commentCountSTR = FeedUIController.Instance.GetAbreviation(commentCount);
//        if (commentCountSTR != "0")
//        {
//            commentBtn.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = commentCountSTR;
//            //commentBtn.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -3);
//        }
//        else
//        {
//            commentBtn.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
//            //commentBtn.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -23);
//        }
//    }

//    public void OnClickCommentButton(bool isRefresh)
//    {
//        //this method is get comment list and check if current feed this on multiple time not get comment list.......
//        //Riken
//        //SNS_APIManager.Instance.CommentListGetAndClickFeedCommentButton(FeedData.id, isRefresh, FeedData.commentCount);
//        SNS_APIManager.Instance.CommentListGetAndClickFeedCommentButton(hotFeed.id, isRefresh, hotFeed.commentCount);
//        if (!isRefresh)
//        {
//            FeedUIController.Instance.OpenCommentPanel();
//        }
//    }
//    #endregion

//    #region Like or DisLike Button Methods.......
//    //this method is setup likeCount text and image.......
//    public void LikeCountAndUISetup(int likeCount)
//    {
//        string likeCountSTR = FeedUIController.Instance.GetAbreviation(likeCount);
//        if (likeCountSTR != "0")
//        {
//            likeCountBtn.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = likeCountSTR;
//            //likeCountBtn.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -3);
//        }
//        else
//        {
//            likeCountBtn.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
//            //likeCountBtn.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -30);
//        }
//        //Riken
//        //if (FeedData.isLike)
//        if (hotFeed.isLike)
//        {
//            likeCountBtn.transform.GetChild(0).GetComponent<Image>().color = likeSelectionColor;
//        }
//        else
//        {
//            likeCountBtn.transform.GetChild(0).GetComponent<Image>().color = Color.white;
//        }
//    }

//    //this method is used to like or dislike button click.......
//    public void OnClickLikeOrDisLikeButton()
//    {
//        //Riken
//        //SNS_APIManager.Instance.RequestLikeOrDisLikeFeed(FeedData.id.ToString(), likeCountBtn.GetComponent<Button>());
//        SNS_APIManager.Instance.RequestLikeOrDisLikeFeed(hotFeed.id.ToString(), likeCountBtn.GetComponent<Button>());
//    }
//    #endregion
//}