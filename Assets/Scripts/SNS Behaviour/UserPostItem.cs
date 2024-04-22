using RenderHeads.Media.AVProVideo;
using SuperStar.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UserPostItem : MonoBehaviour
{
    //public AllFeedByUserIdRow userData;
    //public AllTextPostByUserIdRow userTextPostData;
    //public TaggedFeedsByUserIdRow tagUserData;
    //public FeedsByFollowingUser feedUserData;
    ////public AllFeedByUserIdData userPostData;

    //public Image imgFeed, cameraIcon;
    //public GameObject PhotoImage, VideoImage;
    //public MediaPlayer feedMediaPlayer;
    //public GameObject videoDisplay;
    //public string avtarUrl;

    //public bool isVideoFeed = false;
    //[Space]
    //public bool isImageSuccessDownloadAndSave = false;
    //public bool isReleaseFromMemoryOrNot = false;
    //public bool isOnScreen;//check object is on screen or not

    //public bool isVisible = false;
    //float lastUpdateCallTime;

    //private void OnDisable()
    //{
    //    if (!isVideoFeed)
    //    {
    //        AssetCache.Instance.RemoveFromMemory(imgFeed.sprite);
    //        imgFeed.sprite = null;
    //        isReleaseFromMemoryOrNot = true;
    //        //Resources.UnloadUnusedAssets();//every clear.......
    //        //Caching.ClearCache();
    //        SNS_APIManager.Instance.ResourcesUnloadAssetFile();//UnloadUnusedAssets file call every 15 items.......
    //    }
    //    /*else
    //    {
    //        if (WaitRePlayVideoCo != null)
    //        {
    //            StopCoroutine(WaitRePlayVideoCo);
    //        }
    //    }*/
    //}

    //public int cnt = 0;
    //private void OnEnable()
    //{
    //    if (isVideoFeed && cnt > 0)
    //    {
    //        RePlayVideoAfterEnable();
    //    }
    //    cnt += 1;
    //    //GameManager.Instance.m_MainCamera.gameObject.SetActive(true);
    //}

    //private void Update()//delete image after object out of screen
    //{
    //    /*if (SNS_APIManager.Instance.isTestDefaultToken)//for direct SNS Scene Test....... 
    //    {
    //        return;
    //    }
    //    else*/
    //    if (isVideoFeed)
    //    {
    //        return;
    //    }
    //    GameManager.Instance.m_MainCamera.gameObject.SetActive(true);
    //    lastUpdateCallTime += Time.deltaTime;
    //    if (lastUpdateCallTime > 0.3f )//call every 0.4 sec
    //    {
    //        Vector3 mousePosNormal = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
    //        Vector3 mousePosNR = Camera.main.ScreenToViewportPoint(mousePosNormal);

    //        if (mousePosNR.y >= -0.1f && mousePosNR.y <= 1.1f)
    //        {
    //            isOnScreen = true;
    //        }
    //        else
    //        {
    //            isOnScreen = false;
    //        }

    //        lastUpdateCallTime = 0;
    //    }

    //    if (isVisible && isOnScreen)//this is check if object is visible on camera then load feed or video one time
    //    {
    //        isVisible = false;
    //        //Debug.Log("Image download starting one time");
    //        if (transform.GetSiblingIndex() == 0)
    //        {
    //            Invoke("DownloadAndLoadFeed", 2f);
    //        }
    //        else
    //        {
    //            DownloadAndLoadFeed();
    //        }
    //    }
    //    else if (isImageSuccessDownloadAndSave)
    //    {
    //        if (isOnScreen)
    //        {
    //            if (isReleaseFromMemoryOrNot)
    //            {
    //                isReleaseFromMemoryOrNot = false;
    //                //re load from asset 
    //                //Debug.Log("Re Download Image");
    //                if (!string.IsNullOrEmpty(userData.thumbnail))
    //                {
    //                    if (AssetCache.Instance.HasFile(userData.thumbnail))
    //                    {
    //                        AssetCache.Instance.LoadSpriteIntoImage(imgFeed, userData.thumbnail, changeAspectRatio: true);
    //                        CheckAndSetResolutionOfImage(imgFeed.sprite);
    //                    }
    //                }
    //                else
    //                {
    //                    if (AssetCache.Instance.HasFile(userData.Image))
    //                    {
    //                        AssetCache.Instance.LoadSpriteIntoImage(imgFeed, userData.Image, changeAspectRatio: true);
    //                        CheckAndSetResolutionOfImage(imgFeed.sprite);
    //                    }
    //                }
    //            }
    //        }
    //        else
    //        {
    //            if (!isReleaseFromMemoryOrNot)
    //            {
    //                //realse from memory 
    //                isReleaseFromMemoryOrNot = true;
    //                //Debug.Log("remove from memory");
    //                AssetCache.Instance.RemoveFromMemory(imgFeed.sprite);
    //                imgFeed.sprite = null;
    //                //Resources.UnloadUnusedAssets();//every clear.......
    //                //Caching.ClearCache();
    //                SNS_APIManager.Instance.ResourcesUnloadAssetFile();//UnloadUnusedAssets file call every 15 items.......
    //            }
    //        }
    //    }
    //}

    //public void LoadFeed()
    //{
    //    isVisible = true;
    //    /*if (!SNS_APIManager.Instance.isTestDefaultToken)
    //    {
    //        isVisible = true;
    //    }
    //    else//only for test else part
    //    {
    //        DownloadAndLoadFeed();
    //    }*/
    //}

    ////void DownloadAndLoadFeed()
    ////{
    ////    if (userData.Id != 0)
    ////    {
    ////        if (!string.IsNullOrEmpty(userData.Image))
    ////        {
    ////            bool isImageUrlFromDropbox = SNS_APIManager.Instance.CheckUrlDropboxOrNot(userData.Image);
    ////            //Debug.Log("isImageUrlFromDropbox:  " + isImageUrlFromDropbox);
    ////            if (isImageUrlFromDropbox)
    ////            {
    ////                AssetCache.Instance.EnqueueOneResAndWait(userData.Image, userData.Image, (success) =>
    ////                {
    ////                    if (success)
    ////                    {
    ////                        AssetCache.Instance.LoadSpriteIntoImage(imgFeed, userData.Image, changeAspectRatio: true);
    ////                        CheckAndSetResolutionOfImage(imgFeed.sprite);
    ////                        isImageSuccessDownloadAndSave = true;
    ////                    }
    ////                    else
    ////                    {
    ////                       Debug.Log("Download Failed");
    ////                    }
    ////                });
    ////            }
    ////            else
    ////            {
    ////                GetImageFromAWS(userData.Image, imgFeed);//Get image from aws and save/load into asset cache.......
    ////            }

    ////            cameraIcon.gameObject.SetActive(false);
    ////            PhotoImage.SetActive(true);
    ////            feedMediaPlayer.gameObject.SetActive(false);
    ////            videoDisplay.gameObject.SetActive(false);
    ////        }
    ////        else if (!string.IsNullOrEmpty(userData.Video))
    ////        {
    ////            if (!string.IsNullOrEmpty(userData.thumbnail))
    ////            {
    ////                bool isImageUrlFromDropbox = SNS_APIManager.Instance.CheckUrlDropboxOrNot(userData.thumbnail);
    ////                //Debug.Log("isImageUrlFromDropbox:  " + isImageUrlFromDropbox);
    ////                if (isImageUrlFromDropbox)
    ////                {
    ////                    AssetCache.Instance.EnqueueOneResAndWait(userData.thumbnail, userData.thumbnail, (success) =>
    ////                    {
    ////                        if (success)
    ////                        {
    ////                            AssetCache.Instance.LoadSpriteIntoImage(imgFeed, userData.thumbnail, changeAspectRatio: true);
    ////                            isImageSuccessDownloadAndSave = true;
    ////                            CheckAndSetResolutionOfImage(imgFeed.sprite);
    ////                        }
    ////                        else
    ////                        {
    ////                            if (reloadImageCount < 3)//if Thumbnail url is not live then reload after 2 secound....... 
    ////                            {
    ////                                StartCoroutine(IsReloadFeedImageIfFaield());
    ////                                reloadImageCount += 1;
    ////                            }
    ////                           Debug.Log("Thumbnail download failed");
    ////                        }
    ////                    });
    ////                }
    ////                else
    ////                {
    ////                    GetImageFromAWS(userData.thumbnail, imgFeed);//Get image from aws and save/load into asset cache.......
    ////                }
    ////                cameraIcon.gameObject.SetActive(true);
    ////                PhotoImage.SetActive(true);
    ////                feedMediaPlayer.gameObject.SetActive(false);
    ////                videoDisplay.gameObject.SetActive(false);
    ////            }
    ////            else
    ////            {
    ////                bool isVideoUrlFromDropbox = SNS_APIManager.Instance.CheckUrlDropboxOrNot(userData.Video);
    ////                isVideoFeed = true;

    ////                //Debug.Log("FeedData.video " + userData.Video);
    ////                cameraIcon.gameObject.SetActive(true);
    ////                PhotoImage.SetActive(false);

    ////                videoDisplay.gameObject.SetActive(true);
    ////                feedMediaPlayer.gameObject.SetActive(true);

    ////                if (isVideoUrlFromDropbox)
    ////                {
    ////                    //feedMediaPlayer.OpenMedia(new MediaPath(userData.Video, MediaPathType.AbsolutePathOrURL), autoPlay: true);
    ////                    feedMediaPlayer.OpenMedia(new MediaPath(userData.Video, MediaPathType.AbsolutePathOrURL), autoPlay: false);
    ////                    //feedMediaPlayer.Play();
    ////                    mediaUrl = userData.Video;
    ////                }
    ////                else
    ////                {
    ////                    GetVideoUrl(userData.Video);//get video url from aws and play.......
    ////                }
    ////            }
    ////        }
    ////    }
    ////    else if (tagUserData.id != 0)
    ////    {
    ////        if (!string.IsNullOrEmpty(tagUserData.feed.image))
    ////        {
    ////            bool isImageUrlFromDropbox = SNS_APIManager.Instance.CheckUrlDropboxOrNot(tagUserData.feed.image);
    ////            //Debug.Log("isImageUrlFromDropbox:  " + isImageUrlFromDropbox);
    ////            if (isImageUrlFromDropbox)
    ////            {
    ////                AssetCache.Instance.EnqueueOneResAndWait(tagUserData.feed.image, tagUserData.feed.image, (success) =>
    ////                {
    ////                    if (success)
    ////                    {
    ////                        AssetCache.Instance.LoadSpriteIntoImage(imgFeed, tagUserData.feed.image, changeAspectRatio: true);
    ////                        isImageSuccessDownloadAndSave = true;
    ////                    }
    ////                });
    ////            }
    ////            else
    ////            {
    ////                GetImageFromAWS(tagUserData.feed.image, imgFeed);//Get image from aws and save/load into asset cache.......
    ////            }

    ////            cameraIcon.gameObject.SetActive(false);
    ////            PhotoImage.SetActive(true);
    ////            feedMediaPlayer.gameObject.SetActive(false);
    ////            videoDisplay.gameObject.SetActive(false);
    ////        }
    ////        else if (!string.IsNullOrEmpty(tagUserData.feed.video))
    ////        {
    ////            bool isVideoUrlFromDropbox = SNS_APIManager.Instance.CheckUrlDropboxOrNot(tagUserData.feed.video);
    ////            isVideoFeed = true;

    ////            //Debug.Log("FeedData.video " + tagUserData.feed.video);
    ////            cameraIcon.gameObject.SetActive(true);
    ////            PhotoImage.SetActive(false);

    ////            videoDisplay.gameObject.SetActive(true);
    ////            feedMediaPlayer.gameObject.SetActive(true);

    ////            if (isVideoUrlFromDropbox)
    ////            {
    ////                //feedMediaPlayer.OpenMedia(new MediaPath(tagUserData.feed.video, MediaPathType.AbsolutePathOrURL), autoPlay: true);
    ////                feedMediaPlayer.OpenMedia(new MediaPath(tagUserData.feed.video, MediaPathType.AbsolutePathOrURL), autoPlay: false);
    ////                //feedMediaPlayer.Play();
    ////                mediaUrl = tagUserData.feed.video;
    ////            }
    ////            else
    ////            {
    ////                GetVideoUrl(tagUserData.feed.video);//get video url from aws and play.......
    ////            }
    ////        }
    ////    }
    ////}

    ////this is only for once thumbnail url is not live then once thumbnail download failed then refresh 3 time with 3 second duration.......
    //int reloadImageCount = 0;
    //IEnumerator IsReloadFeedImageIfFaield()
    //{
    //    yield return new WaitForSeconds(3f);
    //    isVisible = true;
    //}

    //public void OnClickPostItem()
    //{
    //    StartCoroutine(loadVideoFeed());
    //}
    //IEnumerator loadVideoFeed()
    //{
    //    foreach (Transform item in FeedUIController.Instance.videofeedParent)
    //    {
    //        Destroy(item.gameObject);
    //    }
    //    int index = 0;
    //    int pageIndex = 0;
    //    //FeedUIController.Instance.ShowLoader(true);
    //   Debug.Log("FeedUIController.Instance.feedFullViewScreenCallingFrom= > " + FeedUIController.Instance.feedFullViewScreenCallingFrom);
    //   Debug.Log("userData.Id= > " + userData.Id);
    //    if (userData.Id != 0)
    //    {
    //        List<AllFeedByUserIdRow> feedRowsDataList = new List<AllFeedByUserIdRow>();
    //        if (MyProfileDataManager.Instance.myProfileScreen.activeSelf)
    //        {
    //           Debug.Log("H1 > ");
    //            FeedUIController.Instance.feedFullViewScreenCallingFrom = "MyProfile";
    //            if (isVideoFeed)
    //                if (!string.IsNullOrEmpty(userData.Video))
    //                {
    //                    feedRowsDataList = MyProfileDataManager.Instance.allMyFeedVideoRootDataList;
    //                }
    //                else
    //                {
    //                    feedRowsDataList = MyProfileDataManager.Instance.allMyFeedImageRootDataList;
    //                }
    //        }
    //        else if (FeedUIController.Instance.otherPlayerProfileScreen.activeSelf)
    //        {
    //           Debug.Log("H2 > ");
    //            FeedUIController.Instance.feedFullViewScreenCallingFrom = "OtherProfile";
    //            if (isVideoFeed)
    //                if (!string.IsNullOrEmpty(userData.Video))
    //                {
    //                    feedRowsDataList = OtherPlayerProfileData.Instance.allMyFeedVideoRootDataList;
    //                }
    //                else
    //                {
    //                    feedRowsDataList = OtherPlayerProfileData.Instance.allMyFeedImageRootDataList;
    //                }
    //        }
    //        else if (FeedUIController.Instance.forYouFeedTabContainer.gameObject.activeInHierarchy)
    //        {
    //           Debug.Log("H3 > ");
    //            FeedUIController.Instance.feedFullViewScreenCallingFrom = "FeedPage";
    //            //if (!string.IsNullOrEmpty(userData.Video))
    //            //{
    //            //    feedRowsDataList = MyProfileDataManager.Instance.allMyFeedVideoRootDataList;
    //            //}
    //            //else
    //            //{
    //            //    feedRowsDataList = MyProfileDataManager.Instance.allMyFeedImageRootDataList;
    //            //}
    //            feedRowsDataList = SNS_APIManager.Instance.allFeedWithUserIdRoot.Data.Rows;
    //        }
    //        else
    //        {
    //           Debug.Log("H4 > ");
    //            FeedUIController.Instance.feedFullViewScreenCallingFrom = "";
    //            feedRowsDataList = SNS_APIManager.Instance.allFeedWithUserIdRoot.Data.Rows;
    //        }
    //       Debug.Log("FeedUIController.Instance.feedFullViewScreenCallingFrom= > " + FeedUIController.Instance.feedFullViewScreenCallingFrom);

    //        for (int i = 0; i < feedRowsDataList.Count; i++)
    //        {
    //            GameObject videofeedObject = Instantiate(SNS_APIController.Instance.PostVideoFeedPrefab, FeedUIController.Instance.videofeedParent);

    //            PostFeedVideoItem postFeedVideoItem = videofeedObject.GetComponent<PostFeedVideoItem>();

    //            postFeedVideoItem.userData = feedRowsDataList[i];
    //            postFeedVideoItem.feedUserData = feedUserData;
    //            //postFeedVideoItem.avatarUrl = avtarUrl;
    //            postFeedVideoItem.avatarUrl = OtherPlayerProfileData.Instance.singleUserProfileData.avatar;
    //            postFeedVideoItem.LoadFeed();

    //            if (feedRowsDataList[i].Id == userData.Id)
    //            {
    //                pageIndex = index;
    //                FeedUIController.Instance.videoFeedRect.GetComponent<ScrollSnapRect>().startingPage = pageIndex;
    //            }
    //            index += 1;
    //        }
    //    }
    //    else if (tagUserData.id != 0)
    //    {
    //        for (int i = 0; i < SNS_APIManager.Instance.taggedFeedsByUserIdRoot.data.rows.Count; i++)
    //        {
    //            GameObject videofeedObject = Instantiate(SNS_APIController.Instance.PostVideoFeedPrefab, FeedUIController.Instance.videofeedParent);
    //            videofeedObject.GetComponent<PostFeedVideoItem>().tagUserData = SNS_APIManager.Instance.taggedFeedsByUserIdRoot.data.rows[i];
    //            //videofeedObject.GetComponent<PostFeedVideoItem>().avatarUrl = avtarUrl;
    //            videofeedObject.GetComponent<PostFeedVideoItem>().LoadFeed();

    //            if (SNS_APIManager.Instance.taggedFeedsByUserIdRoot.data.rows[i].id == tagUserData.id)
    //            {
    //                pageIndex = index;
    //                FeedUIController.Instance.videoFeedRect.GetComponent<ScrollSnapRect>().startingPage = pageIndex;
    //            }
    //            index += 1;
    //        }
    //    }
    //    yield return new WaitForSeconds(0.1f);
    //    //FeedUIController.Instance.ShowLoader(false);
    //    //FeedUIController.Instance.feedVideoScreen.SetActive(true);
    //    FeedUIController.Instance.videoFeedRect.GetComponent<ScrollSnapRect>().StartScrollSnap();
    //    FeedUIController.Instance.videofeedParent.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
    //    yield return new WaitForSeconds(0.1f);
    //    FeedUIController.Instance.videofeedParent.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    //    switch (FeedUIController.Instance.feedFullViewScreenCallingFrom)
    //    {
    //        case "MyProfile":
    //            MyProfileDataManager.Instance.mainPostContainer.gameObject.SetActive(false);
    //            break;
    //        case "OtherProfile":
    //            OtherPlayerProfileData.Instance.mainPostContainer.gameObject.SetActive(false);
    //            break;
    //        default:
    //            break;
    //    }
    //}

    //#region Get Image And Video From AWS
    //string mediaUrl = "";
    //public void GetVideoUrl(string key)
    //{
    //    /*var request_1 = new GetPreSignedUrlRequest()
    //    {
    //        BucketName = AWSHandler.Instance.Bucketname,
    //        Key = key,
    //        Expires = DateTime.Now.AddHours(6)
    //    };
    //    //Debug.Log("Feed Video file sending url request:" + AWSHandler.Instance._s3Client);

    //    AWSHandler.Instance._s3Client.GetPreSignedURLAsync(request_1, (callback) =>
    //    {
    //        if (callback.Exception == null)
    //        {
    //            string mediaUrl = callback.Response.Url;
    //            UnityToolbag.Dispatcher.Invoke(() =>
    //            {
    //                if (this.isActiveAndEnabled)
    //                {
    //                    //Debug.Log("Feed Video URL " + mediaUrl);
    //                    //feedMediaPlayer.OpenMedia(new MediaPath(mediaUrl, MediaPathType.AbsolutePathOrURL), autoPlay: true); ;
    //                    feedMediaPlayer.OpenMedia(new MediaPath(mediaUrl, MediaPathType.AbsolutePathOrURL), autoPlay: false);
    //                    //feedMediaPlayer.Play();
    //                }
    //            });
    //        }
    //        else
    //           Debug.Log(callback.Exception);
    //    });*/

    //    if (key != "")
    //    {
    //        mediaUrl = "";

    //        if (key.Contains("https"))
    //        {
    //            mediaUrl = key;
    //        }
    //        else
    //        {
    //            mediaUrl = ConstantsGod.AWS_VIDEO_BASE_URL + key;
    //        }

    //        //Debug.Log($"<color=green> Video Key = FeedVideoItem : </color>{mediaUrl}");
    //        UnityToolbag.Dispatcher.Invoke(() =>
    //        {
    //            if (this.isActiveAndEnabled)
    //            {
    //                //Debug.Log("Feed Video URL " + mediaUrl);
    //                //feedMediaPlayer.OpenMedia(new MediaPath(mediaUrl, MediaPathType.AbsolutePathOrURL), autoPlay: true); ;
    //                feedMediaPlayer.OpenMedia(new MediaPath(mediaUrl, MediaPathType.AbsolutePathOrURL), autoPlay: false);
    //                //feedMediaPlayer.Play();
    //            }
    //        });
    //    }
    //}

    //Coroutine WaitRePlayVideoCo;
    //public void RePlayVideoIfGetError()
    //{
    //    //Debug.Log("Video Play in Error Replay Video after some seconds:" + mediaUrl + "    Buffer:"+ feedMediaPlayer.Control.IsBuffering());
    //    if (!string.IsNullOrEmpty(mediaUrl) && !feedMediaPlayer.Control.IsBuffering())
    //    {
    //        if (WaitRePlayVideoCo != null)
    //        {
    //            StopCoroutine(WaitRePlayVideoCo);
    //        }
    //        WaitRePlayVideoCo = StartCoroutine(WaitRePlayVideoIfGetError());
    //    }
    //}

    //IEnumerator WaitRePlayVideoIfGetError()
    //{
    //    yield return new WaitForSeconds(10f);//if video get error then wait and replay video.......
    //    feedMediaPlayer.OpenMedia(new MediaPath(mediaUrl, MediaPathType.AbsolutePathOrURL), autoPlay: false);
    //}

    //public void RePlayVideoAfterEnable()
    //{
    //    //Debug.Log("Re Enable:" + mediaUrl + "    Buffer:" + feedMediaPlayer.Control.IsBuffering() + "     CanPlay:" + feedMediaPlayer.Control.CanPlay());
    //    if (!string.IsNullOrEmpty(mediaUrl) && !feedMediaPlayer.Control.IsBuffering() && !feedMediaPlayer.Control.CanPlay())
    //    {
    //        feedMediaPlayer.OpenMedia(new MediaPath(mediaUrl, MediaPathType.AbsolutePathOrURL), autoPlay: false);
    //    }
    //}

    ////public void GetImageFromAWS(string key, Image mainImage)
    ////{
    ////    //Debug.Log("GetImageFromAWS key:" + key);
    ////    //GetExtentionType(key);
    ////    if (AssetCache.Instance.HasFile(key))
    ////    {
    ////        //Debug.Log("Image Available on Disk");
    ////        AssetCache.Instance.LoadSpriteIntoImage(mainImage, key, changeAspectRatio: true);
    ////        CheckAndSetResolutionOfImage(mainImage.sprite);
    ////        isImageSuccessDownloadAndSave = true;
    ////        return;
    ////    }
    ////    else
    ////    {
    ////        if (!string.IsNullOrEmpty(FeedUIController.Instance.createFeedLastPickFilePath))//for set after upload image imidiate for first time.......
    ////        {
    ////            if (FeedUIController.Instance.createFeedLastPickFileName == key)
    ////            {
    ////                mainImage.sprite = FeedUIController.Instance.createFeedImage.sprite;
    ////                CheckAndSetResolutionOfImage(mainImage.sprite);
    ////                FeedUIController.Instance.lastPostCreatedImageDownload = true;
    ////            }
    ////        }

    ////        AssetCache.Instance.EnqueueOneResAndWait(key, (ConstantsGod.r_AWSImageKitBaseUrl + key), (success) =>
    ////        {
    ////            if (success)
    ////            {
    ////                AssetCache.Instance.LoadSpriteIntoImage(mainImage, key, changeAspectRatio: true);
    ////                //tt AssetCache.Instance.LoadTexture2DIntoRawImage(imgFeedRaw, FeedData.image, changeAspectRatio: true);
    ////                //Debug.Log("Save and Image download success profile post Item");
    ////                CheckAndSetResolutionOfImage(mainImage.sprite);
    ////                isImageSuccessDownloadAndSave = true;

    ////                if (FeedUIController.Instance.createFeedLastPickFileName == key && FeedUIController.Instance.lastPostCreatedImageDownload)//after download and load last created post then clear created post data.......
    ////                {
    ////                    FeedUIController.Instance.ResetAndClearCreateFeedData();
    ////                }
    ////            }
    ////            else
    ////            {
    ////               Debug.Log("Download failed");
    ////            }
    ////        });
    ////    }
    ////}

    ///*public static ExtentionType currentExtention;
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
    //    //Debug.Log("ExtentionType " + extension);
    //    if (extension == "png" || extension == "jpg" || extension == "jpeg" || extension == "gif" || extension == "bmp" || extension == "tiff" || extension == "heic")
    //    {
    //        currentExtention = ExtentionType.Image;
    //        return ExtentionType.Image;
    //    }
    //    else if (extension == "mp4" || extension == "mov" || extension == "wav" || extension == "avi")
    //    {
    //        currentExtention = ExtentionType.Video;
    //        return ExtentionType.Video;
    //    }
    //    else if (extension == "mp3" || extension == "aac" || extension == "flac")
    //    {
    //        currentExtention = ExtentionType.Audio;
    //        return ExtentionType.Audio;
    //    }
    //    return (ExtentionType)0;
    //}*/
    //#endregion

    //#region Check And Set Image Orientation 
    //public AspectRatioFitter aspectRatioFitter;
    //public void CheckAndSetResolutionOfImage(Sprite feedImage)
    //{
    //    float diff = feedImage.rect.width - feedImage.rect.height;

    //    //Debug.Log("CheckAndSetResolutionOfImage:" + diff);
    //    if (diff < -160)
    //    {
    //        aspectRatioFitter.aspectRatio = 0.1f;
    //    }
    //    else
    //    {
    //        aspectRatioFitter.aspectRatio = 2.25f;
    //    }
    //}
    //#endregion
}