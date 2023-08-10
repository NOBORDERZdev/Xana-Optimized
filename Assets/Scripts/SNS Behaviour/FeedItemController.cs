using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SuperStar.Helpers;
using UnityEngine.UI;
using UnityEngine.Video;
using RenderHeads.Media.AVProVideo;
using System.IO;
using System;
using UnityEngine.Networking;

public class FeedItemController : MonoBehaviour
{
    public AllUserWithFeed FeedData;
    public AllUserWithFeedRow FeedRawData;
    //public AllUserWithFeedData userData;
    public HotFeed HotFeed;

    public RawImage imgFeedRaw;
    public Image imgFeed, cameraIcon;
    public Image userImg;
    public TextMeshProUGUI userNameText;
    public GameObject PhotoImage, VideoImage;
    //public VideoPlayer feedVideoPlayer;
    public MediaPlayer feedMediaPlayer;
    public GameObject videoDisplay;
    /*public TextMeshProUGUI feedTitle;
     public TextMeshProUGUI feedPlayerName;
     public TextMeshProUGUI feedLike;
     public TextMeshProUGUI tottlePostText;*/

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
            //Riken
            /*if (!string.IsNullOrEmpty(FeedData.image) || !string.IsNullOrEmpty(FeedData.thumbnail))
            {
                AssetCache.Instance.RemoveFromMemory(imgFeed.sprite);
                imgFeed.sprite = null;
                APIManager.Instance.ResourcesUnloadAssetFile();//UnloadUnusedAssets file call every 15 items.......
            }*/
            if (!string.IsNullOrEmpty(HotFeed.image) || !string.IsNullOrEmpty(HotFeed.thumbnail))
            {
                AssetCache.Instance.RemoveFromMemory(imgFeed.sprite);
                imgFeed.sprite = null;
                APIManager.Instance.ResourcesUnloadAssetFile();//UnloadUnusedAssets file call every 15 items.......
            }
        }
    }

    public void ClearMemoryAfterDestroyObj()
    {
        isClearAfterMemory = true;
        //Riken
        /*if (!string.IsNullOrEmpty(FeedData.image) || !string.IsNullOrEmpty(FeedData.thumbnail))
        {
            AssetCache.Instance.RemoveFromMemory(imgFeed.sprite);
            imgFeed.sprite = null;
        }*/
        if (!string.IsNullOrEmpty(HotFeed.image) || !string.IsNullOrEmpty(HotFeed.thumbnail))
        {
            AssetCache.Instance.RemoveFromMemory(imgFeed.sprite);
            imgFeed.sprite = null;
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
            //Debug.Log("Image download starting one time");
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
                    //Debug.Log("Re Download Image");
                    //Riken
                    /*if (!string.IsNullOrEmpty(FeedData.thumbnail))
                    {
                        if (AssetCache.Instance.HasFile(FeedData.thumbnail))
                        {
                            AssetCache.Instance.LoadSpriteIntoImage(imgFeed, FeedData.thumbnail, changeAspectRatio: true);
                            CheckAndSetResolutionOfImage(imgFeed.sprite);
                            //tt AssetCache.Instance.LoadTexture2DIntoRawImage(imgFeedRaw, FeedData.image, changeAspectRatio: true);
                        }
                    }
                    else
                    {
                        if (AssetCache.Instance.HasFile(FeedData.image))
                        {
                            AssetCache.Instance.LoadSpriteIntoImage(imgFeed, FeedData.image, changeAspectRatio: true);
                            CheckAndSetResolutionOfImage(imgFeed.sprite);
                            //tt AssetCache.Instance.LoadTexture2DIntoRawImage(imgFeedRaw, FeedData.image, changeAspectRatio: true);
                        }
                    }*/
                    if (!string.IsNullOrEmpty(HotFeed.thumbnail))
                    {
                        if (AssetCache.Instance.HasFile(HotFeed.thumbnail))
                        {
                            AssetCache.Instance.LoadSpriteIntoImage(imgFeed, HotFeed.thumbnail, changeAspectRatio: true);
                            CheckAndSetResolutionOfImage(imgFeed.sprite);
                            //tt AssetCache.Instance.LoadTexture2DIntoRawImage(imgFeedRaw, FeedData.image, changeAspectRatio: true);
                        }
                    }
                    else
                    {
                        if (AssetCache.Instance.HasFile(HotFeed.image))
                        {
                            AssetCache.Instance.LoadSpriteIntoImage(imgFeed, HotFeed.image, changeAspectRatio: true);
                            CheckAndSetResolutionOfImage(imgFeed.sprite);
                            //tt AssetCache.Instance.LoadTexture2DIntoRawImage(imgFeedRaw, FeedData.image, changeAspectRatio: true);
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
                    //Debug.Log("remove from memory");
                    AssetCache.Instance.RemoveFromMemory(imgFeed.sprite);
                    //tt AssetCache.Instance.RemoveFromMemory(FeedData.image, true);
                    imgFeed.sprite = null;
                    //tt imgFeedRaw.texture = null;
                    //Resources.UnloadUnusedAssets();//every clear.......
                    //Caching.ClearCache();
                    APIManager.Instance.ResourcesUnloadAssetFile();//UnloadUnusedAssets file call every 15 items.......
                }
            }
        }
    }

    public void LoadFeed()
    {
        //feedTitle.text = FeedData.title;
        //feedLike.text = FeedData.likeCount.ToString();
        //Riken
        /*if (!string.IsNullOrEmpty(FeedData.image) || !string.IsNullOrEmpty(FeedData.thumbnail))//Feed Hot Collection Items Initiate Total Count Set.......
        {
            FeedUIController.Instance.hotFeedInitiateTotalCount += 1;
        }*/
        if (!string.IsNullOrEmpty(HotFeed.image) || !string.IsNullOrEmpty(HotFeed.thumbnail))//Feed Hot Collection Items Initiate Total Count Set.......
        {
            FeedUIController.Instance.hotFeedInitiateTotalCount += 1;
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
        //Riken
        /*if (!string.IsNullOrEmpty(FeedData.image))
        {
            bool isImageUrlFromDropbox = APIManager.Instance.CheckUrlDropboxOrNot(FeedData.image);
            //Debug.Log("isImageUrlFromDropbox:  " + isImageUrlFromDropbox + " :name:" + FeedsByFollowingUserRowData.User.Name);
            if (isImageUrlFromDropbox)
            {
                AssetCache.Instance.EnqueueOneResAndWait(FeedData.image, FeedData.image, (success) =>
                {
                    if (success)
                    {
                        AssetCache.Instance.LoadSpriteIntoImage(imgFeed, FeedData.image, changeAspectRatio: true);
                        //tt AssetCache.Instance.LoadTexture2DIntoRawImage(imgFeedRaw, FeedData.image, changeAspectRatio: true);
                        isImageSuccessDownloadAndSave = true;
                        CheckAndSetResolutionOfImage(imgFeed.sprite);
                        if (FeedUIController.Instance.hotFeedInitiateTotalCount > 0)
                        {
                            FeedUIController.Instance.hotFeedInitiateTotalCount -= 1;//Feed Hot items image loaded count Decrease
                            //FeedUIController.Instance.HotFeedImageLoadedCount += 1;//Feed Hot items image loaded count increase
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
            //feedVideoPlayer.gameObject.SetActive(false);
            feedMediaPlayer.gameObject.SetActive(false);
            videoDisplay.gameObject.SetActive(false);
            //Debug.Log("imagefeed");
        }
        else if (!string.IsNullOrEmpty(FeedData.video))
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
                            //tt AssetCache.Instance.LoadTexture2DIntoRawImage(imgFeedRaw, FeedData.thumbnail, changeAspectRatio: true);
                            isImageSuccessDownloadAndSave = true;
                            CheckAndSetResolutionOfImage(imgFeed.sprite);
                            if (FeedUIController.Instance.hotFeedInitiateTotalCount > 0)
                            {
                                FeedUIController.Instance.hotFeedInitiateTotalCount -= 1;//Feed Hot items image loaded count Decrease
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
                //Debug.Log("FeedData.video " + FeedData.video);
                PhotoImage.SetActive(false);
                videoDisplay.gameObject.SetActive(true);
                feedMediaPlayer.gameObject.SetActive(true);

                if (isVideoUrlFromDropbox)
                {
                    //feedMediaPlayer.OpenMedia(new MediaPath(FeedData.video, MediaPathType.AbsolutePathOrURL), autoPlay: true);
                    feedMediaPlayer.OpenMedia(new MediaPath(FeedData.video, MediaPathType.AbsolutePathOrURL), autoPlay: false);
                }
                else
                {
                    GetVideoUrl(FeedData.video);//get video url from aws and play.......
                }
            }
        }*/
        if (HotFeed.user != null)
        {
            userNameText.text = HotFeed.user.name;
        }
        if (!string.IsNullOrEmpty(HotFeed.image))
        {
            bool isImageUrlFromDropbox = APIManager.Instance.CheckUrlDropboxOrNot(HotFeed.image);
            //Debug.Log("isImageUrlFromDropbox:  " + isImageUrlFromDropbox + " :name:" + FeedsByFollowingUserRowData.User.Name);
            if (isImageUrlFromDropbox)
            {
                AssetCache.Instance.EnqueueOneResAndWait(HotFeed.image, HotFeed.image, (success) =>
                {
                    if (success)
                    {
                        AssetCache.Instance.LoadSpriteIntoImage(imgFeed, HotFeed.image, changeAspectRatio: true);
                        //tt AssetCache.Instance.LoadTexture2DIntoRawImage(imgFeedRaw, FeedData.image, changeAspectRatio: true);
                        isImageSuccessDownloadAndSave = true;
                        CheckAndSetResolutionOfImage(imgFeed.sprite);
                        if (FeedUIController.Instance.hotFeedInitiateTotalCount > 0)
                        {
                            FeedUIController.Instance.hotFeedInitiateTotalCount -= 1;//Feed Hot items image loaded count Decrease
                            //FeedUIController.Instance.HotFeedImageLoadedCount += 1;//Feed Hot items image loaded count increase
                        }
                    }
                });
            }
            else
            {
                GetImageFromAWS(HotFeed.image, imgFeed);//Get image from aws and save/load into asset cache.......
            }
            if (!string.IsNullOrEmpty(HotFeed.user.avatar))
            {
                bool isImageUrlFromDropbox1 = APIManager.Instance.CheckUrlDropboxOrNot(HotFeed.user.avatar);
                //Debug.Log("isImageUrlFromDropbox:  " + isImageUrlFromDropbox + " :name:" + FeedsByFollowingUserRowData.User.Name);
                if (isImageUrlFromDropbox1)
                {
                    AssetCache.Instance.EnqueueOneResAndWait(HotFeed.user.avatar, HotFeed.user.avatar, (success) =>
                    {
                        if (success)
                        {
                            AssetCache.Instance.LoadSpriteIntoImage(userImg, HotFeed.user.avatar, changeAspectRatio: true);
                            //tt AssetCache.Instance.LoadTexture2DIntoRawImage(imgFeedRaw, FeedData.image, changeAspectRatio: true);
                            isImageSuccessDownloadAndSave = true;
                            CheckAndSetResolutionOfImage(userImg.sprite);
                        }
                    });
                }
                else
                {
                    GetImageFromAWS(HotFeed.user.avatar, userImg);//Get image from aws and save/load into asset cache.......
                }
            }
            cameraIcon.gameObject.SetActive(false);
            PhotoImage.SetActive(true);
            //VideoImage.SetActive(false);
            //feedVideoPlayer.gameObject.SetActive(false);
            feedMediaPlayer.gameObject.SetActive(false);
            videoDisplay.gameObject.SetActive(false);
            //Debug.Log("imagefeed");
        }
        else if (!string.IsNullOrEmpty(HotFeed.video))
        {
            if (!string.IsNullOrEmpty(HotFeed.thumbnail))
            {
                bool isVideoUrlFromDropbox = APIManager.Instance.CheckUrlDropboxOrNot(HotFeed.thumbnail);
                if (isVideoUrlFromDropbox)
                {
                    AssetCache.Instance.EnqueueOneResAndWait(HotFeed.thumbnail, HotFeed.thumbnail, (success) =>
                    {
                        if (success)
                        {
                            AssetCache.Instance.LoadSpriteIntoImage(imgFeed, HotFeed.thumbnail, changeAspectRatio: true);
                            //tt AssetCache.Instance.LoadTexture2DIntoRawImage(imgFeedRaw, FeedData.thumbnail, changeAspectRatio: true);
                            isImageSuccessDownloadAndSave = true;
                            CheckAndSetResolutionOfImage(imgFeed.sprite);
                            if (FeedUIController.Instance.hotFeedInitiateTotalCount > 0)
                            {
                                FeedUIController.Instance.hotFeedInitiateTotalCount -= 1;//Feed Hot items image loaded count Decrease
                            }
                        }
                    });
                }
                else
                {
                    GetImageFromAWS(HotFeed.thumbnail, imgFeed);//Get image from aws and save/load into asset cache.......
                }

                cameraIcon.gameObject.SetActive(true);
                PhotoImage.SetActive(true);
                feedMediaPlayer.gameObject.SetActive(false);
                videoDisplay.gameObject.SetActive(false);
            }
            else
            {
                bool isVideoUrlFromDropbox = APIManager.Instance.CheckUrlDropboxOrNot(HotFeed.video);

                cameraIcon.gameObject.SetActive(true);
                //Debug.Log("FeedData.video " + FeedData.video);
                PhotoImage.SetActive(false);
                videoDisplay.gameObject.SetActive(true);
                feedMediaPlayer.gameObject.SetActive(true);

                if (isVideoUrlFromDropbox)
                {
                    //feedMediaPlayer.OpenMedia(new MediaPath(FeedData.video, MediaPathType.AbsolutePathOrURL), autoPlay: true);
                    feedMediaPlayer.OpenMedia(new MediaPath(HotFeed.video, MediaPathType.AbsolutePathOrURL), autoPlay: false);
                }
                else
                {
                    GetVideoUrl(HotFeed.video);//get video url from aws and play.......
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
        FeedUIController.Instance.feedFullViewScreenCallingFrom = "HotTab";
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
        //Riken
        /*for (int i = 0; i < APIManager.Instance.allUserRootList.Count; i++)
        {
            for (int j = 0; j < APIManager.Instance.allUserRootList[i].feeds.Count; j++)
            {
                GameObject videofeedObject = Instantiate(APIController.Instance.videofeedPrefab, FeedUIController.Instance.videofeedParent);
                videofeedObject.GetComponent<FeedVideoItem>().FeedRawData = APIManager.Instance.allUserRootList[i];
                videofeedObject.GetComponent<FeedVideoItem>().FeedData = APIManager.Instance.allUserRootList[i].feeds[j];
                videofeedObject.GetComponent<FeedVideoItem>().LoadFeed();

                if (APIManager.Instance.allUserRootList[i].id == FeedRawData.id && !isMatch)
                {
                    if (APIManager.Instance.allUserRootList[i].feeds[j].id == FeedData.id)
                    {
                        // pageIndex = index;
                        FeedUIController.Instance.videoFeedRect.GetComponent<ScrollSnapRect>().startingPage = index;
                        isMatch = true;
                    }
                }
                index += 1;
            }
        }*/

        for (int i = 0; i < APIManager.Instance.allhotFeedRoot.data.rows.Count; i++)
        {
            GameObject videofeedObject = Instantiate(APIController.Instance.videofeedPrefab, FeedUIController.Instance.videofeedParent);
            //Riken
            /*videofeedObject.GetComponent<FeedVideoItem>().FeedRawData = APIManager.Instance.allUserRootList[i];
            videofeedObject.GetComponent<FeedVideoItem>().FeedData = APIManager.Instance.allUserRootList[i].feeds[j];*/
            videofeedObject.GetComponent<FeedVideoItem>().hotFeed = APIManager.Instance.allhotFeedRoot.data.rows[i];
            videofeedObject.GetComponent<FeedVideoItem>().LoadFeed();

            if (APIManager.Instance.allhotFeedRoot.data.rows[i].id == HotFeed.id && !isMatch)
            {
                if (APIManager.Instance.allhotFeedRoot.data.rows[i].id == HotFeed.id)
                {
                    // pageIndex = index;
                   Debug.Log("Matched" + FeedData.id);
                    FeedUIController.Instance.videoFeedRect.GetComponent<ScrollSnapRect>().startingPage = index;
                    isMatch = true;
                }
            }
            index += 1;
        }

        yield return new WaitForSeconds(0.1f);
        //FeedUIController.Instance.ShowLoader(false);
        FeedUIController.Instance.feedVideoScreen.SetActive(true);
        FeedUIController.Instance.videoFeedRect.GetComponent<ScrollSnapRect>().StartScrollSnap();
        // FeedUIController.Instance.videoFeedRect.GetComponent<ScrollSnapRect>().LerpToPage(pageIndex);
        //Debug.Log("name : " + FeedUIController.Instance.videofeedParent.name);
        FeedUIController.Instance.videofeedParent.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        yield return new WaitForSeconds(0.1f);
        FeedUIController.Instance.videofeedParent.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
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
        //Debug.Log("Feed Video file sending url request:" + AWSHandler.Instance._s3Client);

        AWSHandler.Instance._s3Client.GetPreSignedURLAsync(request_1, (callback) =>
        {
            if (callback.Exception == null)
            {
                string mediaUrl = callback.Response.Url;
                UnityToolbag.Dispatcher.Invoke(() =>
                {
                    if (this !=null && this.isActiveAndEnabled)
                    {
                        //Debug.Log("Feed Video URL " + mediaUrl);
                        //feedMediaPlayer.OpenMedia(new MediaPath(mediaUrl, MediaPathType.AbsolutePathOrURL), autoPlay: true);
                        feedMediaPlayer.OpenMedia(new MediaPath(mediaUrl, MediaPathType.AbsolutePathOrURL), autoPlay: false);
                        //feedMediaPlayer.Play();
                    }
                });
            }
            else
               Debug.Log(callback.Exception);
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

            //Debug.Log("Video Key : " + mediaUrl);

            UnityToolbag.Dispatcher.Invoke(() =>
            {
                feedMediaPlayer.OpenMedia(new MediaPath(mediaUrl, MediaPathType.AbsolutePathOrURL), autoPlay: false);
            });
        }
    }

    public void GetImageFromAWS(string key, Image mainImage)
    {
        //Debug.Log("GetImageFromAWS key:" + key);
        //GetExtentionType(key);
        if (AssetCache.Instance.HasFile(key))
        {
            //Debug.Log("Image Available on Disk hot item");
            AssetCache.Instance.LoadSpriteIntoImage(mainImage, key, changeAspectRatio: true);
            CheckAndSetResolutionOfImage(mainImage.sprite);
            //tt AssetCache.Instance.LoadTexture2DIntoRawImage(imgFeedRaw, key, changeAspectRatio: true);
            isImageSuccessDownloadAndSave = true;
            if (FeedUIController.Instance.hotFeedInitiateTotalCount > 0)
            {
                FeedUIController.Instance.hotFeedInitiateTotalCount -= 1;//Feed Hot items image loaded count Decrease
                //FeedUIController.Instance.HotFeedImageLoadedCount += 1;//Feed Hot items image loaded count increase
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
                    CheckAndSetResolutionOfImage(mainImage.sprite);
                    //tt AssetCache.Instance.LoadTexture2DIntoRawImage(imgFeedRaw, FeedData.image, changeAspectRatio: true);
                    //Debug.Log("Save and Image download success hot Item");
                    isImageSuccessDownloadAndSave = true;
                    if (FeedUIController.Instance.hotFeedInitiateTotalCount > 0)
                    {
                        FeedUIController.Instance.hotFeedInitiateTotalCount -= 1;//Feed Hot items image loaded count Decrease
                    }
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
        //Debug.Log("ExtentionType " + extension);
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

        //Debug.Log("CheckAndSetResolutionOfImage:" + diff);
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