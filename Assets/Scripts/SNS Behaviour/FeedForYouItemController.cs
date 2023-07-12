using RenderHeads.Media.AVProVideo;
using SuperStar.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

public class FeedForYouItemController : MonoBehaviour
{
    public AllUserWithFeed FeedData;

    public AllUserWithFeedRow FeedRawData;

    public Image imgFeed, cameraIcon, profileImage;
    //public TextMeshProUGUI feedTitle;
    public TextMeshProUGUI feedPlayerName;
    public TextMeshProUGUI feedLike;

    public GameObject PhotoImage, VideoImage;
    public VideoPlayer feedVideoPlayer;

    public MediaPlayer feedMediaPlayer;
    public GameObject videoDisplay;
    // public TextMeshProUGUI tottlePostText;

    [Space]
    public bool isImageSuccessDownloadAndSave = false;
    public bool isReleaseFromMemoryOrNot = false;
    public bool isOnScreen;//check object is on screen or not

    public bool isVisible = false;
    float lastUpdateCallTime;

    bool isClearAfterMemory = false;

    private void OnDestroy()
    {
        if (!isClearAfterMemory)
        {
            if (!String.IsNullOrEmpty(FeedData.image) || !string.IsNullOrEmpty(FeedData.thumbnail))
            {
                AssetCache.Instance.RemoveFromMemory(imgFeed.sprite);
                imgFeed.sprite = null;
            }
            if (!string.IsNullOrEmpty(FeedRawData.avatar))
            {
                AssetCache.Instance.RemoveFromMemory(profileImage.sprite);
                profileImage.sprite = null;
            }
            APIManager.Instance.ResourcesUnloadAssetFile();//UnloadUnusedAssets file call every 15 items.......
        }
    }

    public void ClearMemoryAfterDestroyObj()
    {
        isClearAfterMemory = true;

        if (!String.IsNullOrEmpty(FeedData.image) || !string.IsNullOrEmpty(FeedData.thumbnail))
        {
            AssetCache.Instance.RemoveFromMemory(imgFeed.sprite);
            imgFeed.sprite = null;
        }
        if (!string.IsNullOrEmpty(FeedRawData.avatar))
        {
            AssetCache.Instance.RemoveFromMemory(profileImage.sprite);
            profileImage.sprite = null;
        }
    }

    int cnt = 0;
    private void OnEnable()
    {
        if (cnt > 0 && !isImageSuccessDownloadAndSave)
        {
            isVisible = true;
        }
        cnt += 1;
    }

    private void Update()//delete image after object out of screen
    {
        /*if (APIManager.Instance.isTestDefaultToken)//for direct SNS Scene Test....... 
        {
            return;
        }*/

        lastUpdateCallTime += Time.deltaTime;
        if (lastUpdateCallTime > 0.3f)//call every 0.4 sec
        {
            Vector3 mousePosNormal = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
            Vector3 mousePosNR = Camera.main.ScreenToViewportPoint(mousePosNormal);

            if (mousePosNR.y >= -0.1f && mousePosNR.y <= 1.1f)
            {
                isOnScreen = true;
            }
            else
            {
                isOnScreen = false;
            }

            lastUpdateCallTime = 0;
        }

        if (isVisible && isOnScreen)//this is check if object is visible on camera then load feed or video one time
        {
            isVisible = false;
            //Debug.LogError("Image download starting one time");
            DownloadAndLoadFeed();
        }
        else if (isImageSuccessDownloadAndSave)
        {
            if (isOnScreen)
            {
                if (isReleaseFromMemoryOrNot)
                {
                    isReleaseFromMemoryOrNot = false;
                    //re load from asset 
                    //Debug.LogError("Re Download Image");
                    if (!string.IsNullOrEmpty(FeedData.thumbnail))
                    {
                        if (AssetCache.Instance.HasFile(FeedData.thumbnail))
                        {
                            AssetCache.Instance.LoadSpriteIntoImage(imgFeed, FeedData.thumbnail, changeAspectRatio: true);
                            CheckAndSetResolutionOfImage(imgFeed.sprite);
                        }
                    }
                    else
                    {
                        if (AssetCache.Instance.HasFile(FeedData.image))
                        {
                            AssetCache.Instance.LoadSpriteIntoImage(imgFeed, FeedData.image, changeAspectRatio: true);
                            CheckAndSetResolutionOfImage(imgFeed.sprite);
                        }
                    }
                    if (!string.IsNullOrEmpty(FeedRawData.avatar))
                    {
                        if (AssetCache.Instance.HasFile(FeedRawData.avatar))
                        {
                            AssetCache.Instance.LoadSpriteIntoImage(profileImage, FeedRawData.avatar, changeAspectRatio: true);
                        }
                    }
                }
            }
            else
            {
                if (!isReleaseFromMemoryOrNot)
                {
                    //realse from memory 
                    isReleaseFromMemoryOrNot = true;
                    //Debug.LogError("remove from memory");
                    AssetCache.Instance.RemoveFromMemory(imgFeed.sprite);
                    imgFeed.sprite = null;
                    if (!string.IsNullOrEmpty(FeedRawData.avatar))
                    {
                        AssetCache.Instance.RemoveFromMemory(profileImage.sprite);
                        profileImage.sprite = null;
                    }
                    //Resources.UnloadUnusedAssets();//every clear.......
                    //Caching.ClearCache();
                    APIManager.Instance.ResourcesUnloadAssetFile();//UnloadUnusedAssets file call every 15 items.......
                }
            }
        }
    }

    public void LoadFeed()
    {
        feedLike.text = FeedData.likeCount.ToString();
        string FeedName = "";
        if (!string.IsNullOrEmpty(FeedRawData.name) && FeedRawData.name.Contains("\n"))
        {
            FeedName = FeedRawData.name.Replace("\n", string.Empty);
        }
        else
        {
            FeedName = FeedRawData.name;
        }
        feedPlayerName.text = FeedName;

        if (!string.IsNullOrEmpty(FeedData.image) || !string.IsNullOrEmpty(FeedData.thumbnail))//Feed For You Items Initiate Total Count Set.......
        {
            FeedUIController.Instance.hotForYouFeedInitiateTotalCount += 1;
        }

        isVisible = true;
        /*if (!APIManager.Instance.isTestDefaultToken)
        {
            isVisible = true;
        }
        else//only for test else part
        {
            DownloadAndLoadFeed();
        }*/
    }

    void DownloadAndLoadFeed()
    {
        if (!string.IsNullOrEmpty(FeedRawData.avatar))//set avatar image.......
        {
            bool isAvatarUrlFromDropbox = APIManager.Instance.CheckUrlDropboxOrNot(FeedRawData.avatar);
            //Debug.LogError("isAvatarUrlFromDropbox: " + isAvatarUrlFromDropbox + " :name:" + FeedRawData.name);
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

        if (!string.IsNullOrEmpty(FeedData.image))//FeedForYou image
        {
            bool isImageUrlFromDropbox = APIManager.Instance.CheckUrlDropboxOrNot(FeedData.image);
            //Debug.LogError("isImageUrlFromDropbox:  " + isImageUrlFromDropbox + " :name:" + FeedsByFollowingUserRowData.User.Name);
            if (isImageUrlFromDropbox)
            {
                AssetCache.Instance.EnqueueOneResAndWait(FeedData.image, FeedData.image, (success) =>
                {
                    if (success)
                    {
                        AssetCache.Instance.LoadSpriteIntoImage(imgFeed, FeedData.image, changeAspectRatio: true);
                        isImageSuccessDownloadAndSave = true;
                        CheckAndSetResolutionOfImage(imgFeed.sprite);
                        if (FeedUIController.Instance.hotForYouFeedInitiateTotalCount > 0)
                        {
                            FeedUIController.Instance.hotForYouFeedInitiateTotalCount -= 1;//Feed Hot For You items image loaded count Deincrease
                            //FeedUIController.Instance.hotForYouFeedImageLoadedCount += 1;//Feed Hot For You items image loaded count increase
                        }
                    }
                });
            }
            else
            {
                GetImageFromAWS(FeedData.image, imgFeed);//Get image from aws and save/load into asset cache.......
            }

            cameraIcon.gameObject.SetActive(false);
            PhotoImage.SetActive(true);
            //VideoImage.SetActive(false);
            feedMediaPlayer.gameObject.SetActive(false);
            videoDisplay.gameObject.SetActive(false);
        }
        else if (!string.IsNullOrEmpty(FeedData.video))//FeedForYou Video
        {
            if (!string.IsNullOrEmpty(FeedData.thumbnail))
            {
                bool isVideoUrlFromDropbox = APIManager.Instance.CheckUrlDropboxOrNot(FeedData.thumbnail);
                if (isVideoUrlFromDropbox)
                {
                    AssetCache.Instance.EnqueueOneResAndWait(FeedData.thumbnail, FeedData.thumbnail, (success) =>
                    {
                        if (success)
                        {
                            AssetCache.Instance.LoadSpriteIntoImage(imgFeed, FeedData.thumbnail, changeAspectRatio: true);
                            isImageSuccessDownloadAndSave = true;
                            CheckAndSetResolutionOfImage(imgFeed.sprite);
                            if (FeedUIController.Instance.hotForYouFeedInitiateTotalCount > 0)
                            {
                                FeedUIController.Instance.hotForYouFeedInitiateTotalCount -= 1;//Feed Hot For You items image loaded count Deincrease
                            }
                        }
                    });
                }
                else
                {
                    GetImageFromAWS(FeedData.thumbnail, imgFeed);//Get image from aws and save/load into asset cache.......
                }

                cameraIcon.gameObject.SetActive(true);
                PhotoImage.SetActive(true);
                feedMediaPlayer.gameObject.SetActive(false);
                videoDisplay.gameObject.SetActive(false);
            }
            else
            {
                bool isVideoUrlFromDropbox = APIManager.Instance.CheckUrlDropboxOrNot(FeedData.video);

                cameraIcon.gameObject.SetActive(true);
                //  Debug.LogError("FeedData.video " + FeedData.video);
                //VideoImage.SetActive(true);
                videoDisplay.gameObject.SetActive(true);
                feedMediaPlayer.gameObject.SetActive(true);
                PhotoImage.SetActive(false);

                if (isVideoUrlFromDropbox)
                {
                    //feedMediaPlayer.OpenMedia(new MediaPath(FeedData.video, MediaPathType.AbsolutePathOrURL), autoPlay: true);
                    feedMediaPlayer.OpenMedia(new MediaPath(FeedData.video, MediaPathType.AbsolutePathOrURL), autoPlay: false);
                    //feedMediaPlayer.Play();
                }
                else
                {
                    GetVideoUrl(FeedData.video);//get video url from aws and play.......
                }
            }
        }
    }

    public void OnClickCheckOtherPlayerProfile()
    {
        //FeedUIController.Instance.OnClickCheckOtherPlayerProfile();
    }

    public void OnClickFeedItem()
    {
        FeedUIController.Instance.feedFullViewScreenCallingFrom = "DiscoverTab";
        StartCoroutine(loadVideoFeed());
    }
    IEnumerator loadVideoFeed()
    {
        foreach (Transform item in FeedUIController.Instance.videofeedParent)
        {
            Destroy(item.gameObject);
        }
        int index = 0;
        //int pageIndex = 0;
        bool isMatch = false;
        //FeedUIController.Instance.ShowLoader(true);
        for (int i = 0; i < APIManager.Instance.allUserRootList.Count; i++)
        {
            for (int j = 0; j < APIManager.Instance.allUserRootList[i].feeds.Count; j++)
            {
                GameObject videofeedObject = Instantiate(APIController.Instance.videofeedPrefab, FeedUIController.Instance.videofeedParent);
                FeedVideoItem feedVideoItem = videofeedObject.GetComponent<FeedVideoItem>();
                feedVideoItem.FeedRawData = APIManager.Instance.allUserRootList[i];
                feedVideoItem.FeedData = APIManager.Instance.allUserRootList[i].feeds[j];
                feedVideoItem.LoadFeed();
                if (APIManager.Instance.allUserRootList[i].id == FeedRawData.id && !isMatch)
                {
                    if (APIManager.Instance.allUserRootList[i].feeds[j].id == FeedData.id)
                    {
                        //pageIndex = index;
                        FeedUIController.Instance.videoFeedRect.GetComponent<ScrollSnapRect>().startingPage = index;
                        isMatch = true;
                    }
                }
                index += 1;
            }
        }
        yield return new WaitForSeconds(0.1f);
        //FeedUIController.Instance.ShowLoader(false);
        FeedUIController.Instance.feedVideoScreen.SetActive(true);
        FeedUIController.Instance.videoFeedRect.GetComponent<ScrollSnapRect>().StartScrollSnap();

        // Debug.LogError("name : " + APIController.Instance.videofeedParent.name);
        FeedUIController.Instance.videofeedParent.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        yield return new WaitForSeconds(0.1f);
        //  Debug.LogError("name1111 : " + APIController.Instance.videofeedParent.name);
        FeedUIController.Instance.videofeedParent.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        // FeedUIController.Instance.videoFeedRect.GetComponent<ScrollSnapRect>().LerpToPage(pageIndex);
        FeedUIController.Instance.feedUiScreen.SetActive(false);
    }

    #region Get Image And Video From AWS
    public void GetVideoUrl(string key)
    {
        /*var request_1 = new GetPreSignedUrlRequest()
        {
            BucketName = AWSHandler.Instance.Bucketname,
            Key = key,
            Expires = DateTime.Now.AddHours(6)
        };
        //Debug.LogError("Feed Video file sending url request:" + AWSHandler.Instance._s3Client);

        AWSHandler.Instance._s3Client.GetPreSignedURLAsync(request_1, (callback) =>
        {
            if (callback.Exception == null)
            {
                string mediaUrl = callback.Response.Url;
                UnityToolbag.Dispatcher.Invoke(() =>
                {
                    if (this.isActiveAndEnabled)
                    {
                        //Debug.LogError("Feed Video URL " + mediaUrl);
                        //feedMediaPlayer.OpenMedia(new MediaPath(mediaUrl, MediaPathType.AbsolutePathOrURL), autoPlay: true);
                        feedMediaPlayer.OpenMedia(new MediaPath(mediaUrl, MediaPathType.AbsolutePathOrURL), autoPlay: false);
                    }
                });
            }
            else
                Debug.LogError(callback.Exception);
        });*/
        if (key != "")
        {
            string mediaUrl = "";
            if (key.Contains("https"))
            {
                mediaUrl = key;
            }
            else
            {
                mediaUrl = ConstantsGod.AWS_VIDEO_BASE_URL + key;
            }

            //Debug.Log($"<color=green> Video Key = FeedForYouItemController : </color>{mediaUrl}");

            UnityToolbag.Dispatcher.Invoke(() =>
            {
                bool videoPlay = feedMediaPlayer.OpenMedia(new MediaPath(mediaUrl, MediaPathType.AbsolutePathOrURL), autoPlay: false);
                if (videoPlay)
                {
                    isImageSuccessDownloadAndSave = true;
                }
            });
        }
    }

    public void GetImageFromAWS(string key, Image mainImage)
    {
        //Debug.LogError("GetImageFromAWS key:" + key);
        //GetExtentionType(key);
        if (AssetCache.Instance.HasFile(key))
        {
            //Debug.LogError("Image Available on Disk discover item");
            AssetCache.Instance.LoadSpriteIntoImage(mainImage, key, changeAspectRatio: true);
            if (mainImage == imgFeed && FeedUIController.Instance.hotForYouFeedInitiateTotalCount > 0)//Feed Hot For You items image loaded count increase
            {
                CheckAndSetResolutionOfImage(mainImage.sprite);
                isImageSuccessDownloadAndSave = true;
                FeedUIController.Instance.hotForYouFeedInitiateTotalCount -= 1;//Feed Hot For You items image loaded count Deincrease
                //FeedUIController.Instance.hotForYouFeedImageLoadedCount += 1;
            }
            return;
        }
        else
        {
            AssetCache.Instance.EnqueueOneResAndWait(key, (ConstantsGod.r_AWSImageKitBaseUrl + key), (success) =>
            {
                if (success)
                {
                    AssetCache.Instance.LoadSpriteIntoImage(mainImage, key, changeAspectRatio: true);
                    //tt AssetCache.Instance.LoadTexture2DIntoRawImage(imgFeedRaw, FeedData.image, changeAspectRatio: true);
                    //Debug.LogError("Save and Image download success Discover Item");
                    if (mainImage == imgFeed)
                    {
                        CheckAndSetResolutionOfImage(mainImage.sprite);
                        isImageSuccessDownloadAndSave = true;
                    }
                }
                if (mainImage == imgFeed && FeedUIController.Instance.hotForYouFeedInitiateTotalCount > 0)//Feed Hot For You items image loaded count increase
                {
                    FeedUIController.Instance.hotForYouFeedInitiateTotalCount -= 1;//Feed Hot For You items image loaded count Deincrease
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
    }*/
    #endregion

    #region Check And Set Image Orientation 
    public AspectRatioFitter aspectRatioFitter;
    public void CheckAndSetResolutionOfImage(Sprite feedImage)
    {
        float diff = feedImage.rect.width - feedImage.rect.height;

        //Debug.LogError("CheckAndSetResolutionOfImage:" + diff);
        if (diff < -160)
        {
            aspectRatioFitter.aspectRatio = 0.1f;
        }
        else
        {
            aspectRatioFitter.aspectRatio = 2.2f;
        }
    }
    #endregion
}