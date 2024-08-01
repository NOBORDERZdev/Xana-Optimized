using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class ShowNFTDetails : MonoBehaviour
{
    public GameObject displayPanel;
    public GameObject displayPanelOnlyImage;
    public GameObject displayPanelWithStats;
    public GameObject displayImagePanel;
    public GameObject displayImagePanelPortrait;

    public GameObject imageObject;
    public GameObject imageObjectWithStats;

    public GameObject displayImageWithoutDes;
    public GameObject displayImageWithoutDesPortrait;

    public GameObject displayVideoPanelParent;
    public GameObject displayVideoPanel;
    public GameObject displayVideoPanelWithStats;

    public GameObject displayVideoPanelPortrait;

    public GameObject imageObject_Potrait;
    public GameObject imageObjectWithStats_Potrait;

    public GameObject VideoObject;
    public GameObject VideoObjectWithDes;


    public TextMeshProUGUI descriptionText, titleText, usernameText, ownerText;
    public TextMeshProUGUI descriptionText_Potrait, titleText_Potrait, usernameText_Potrait, ownerText_Potrait;

    public TextMeshProUGUI videoDescriptionText, videoTitleText, videoUsernameText, videoOwnerText;
    public TextMeshProUGUI videoDescriptionText_Potrait, videoTitleText_Potrait, videoUsernameText_Potrait, videoOwnerText_Potrait;

    public string nftlinkText;
    public GameObject creatorimageObject;
    public GameObject ownerimageObject;
    public string ownerlinkText;
    public static ShowNFTDetails instance;
    public Sprite dummyprofileIcone;
    [SerializeField] NFTFromServer nFTFromServer;
    MetaDataInPrefab currData;
    public GameObject loadingImage;
    public bool hasStats;

    public GameObject potraiteLoaderIcon;
    public GameObject landscapeLoaderIcon;

    [SerializeField] List<NftViewwithDes> nftWithDes;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        if (displayPanel != null)
        {
            displayPanel.layer = LayerMask.NameToLayer("NFTDisplayPanel");
        }
    }

    private void OnEnable()
    {

        VideoObject.GetComponent<VideoPlayer>().errorReceived += M_VideoPlayerOnerrorReceived;
        VideoObject.GetComponent<VideoPlayer>().prepareCompleted += M_VideoPlayerOnprepareCompleted;

        VideoObjectWithDes.GetComponent<VideoPlayer>().errorReceived += M_VideoPlayerOnerrorReceived;
        VideoObjectWithDes.GetComponent<VideoPlayer>().prepareCompleted += M_VideoPlayerOnprepareCompletedWithStats;
    }

    public void ShowImage(MetaDataInPrefab data)
    {
        currData = data;

        imageObject.GetComponent<Image>().preserveAspect = true;
        imageObject.GetComponent<Image>().sprite = data.thunbNailImage;
        creatorimageObject.GetComponent<Image>().preserveAspect = true;
        creatorimageObject.GetComponent<Image>().sprite = data.thunbNailImage;
        ownerimageObject.GetComponent<Image>().preserveAspect = true;
        ownerimageObject.GetComponent<Image>().sprite = data.thunbNailImage;

        imageObject.SetActive(true);
        if (imageObject_Potrait)
        {
            imageObject_Potrait.GetComponent<Image>().preserveAspect = true;
            imageObject_Potrait.GetComponent<Image>().sprite = data.thunbNailImage;
            imageObject_Potrait.SetActive(true);
        }

        creatorimageObject.SetActive(true);
        ownerimageObject.SetActive(true);
        VideoObject.SetActive(false);
        VideoObjectWithDes.SetActive(false);
        displayPanel.SetActive(true);

        usernameText.text = "" + data.creatorDetails.username;
        ownerText.text = "" + data.creatorDetails.username;

        usernameText_Potrait.text = usernameText.text;
        ownerText_Potrait.text = ownerText.text;

        nftlinkText = data.nftLink;
        ownerlinkText = data.creatorLink;
        if (LocalizationManager.forceJapanese)
        {
            titleText.text = "" + data.metaData.name;
            descriptionText.text = "" + data.tokenDetails.ja_nft_description;
        }
        else
        {
            titleText.text = "" + data.metaData.name;
            descriptionText.text = "" + data.tokenDetails.en_nft_description;
        }

        titleText_Potrait.text = titleText.text;
        descriptionText_Potrait.text = descriptionText.text;
    }

    /// <summary>
    /// Function to show image with description or just image in dynamic musuem
    /// </summary>
    /// <param name="data"></param>
    public void ShowImage(DynamicGalleryData data)
    {
        // currData = mdip;
      
        displayPanelWithStats.SetActive(true);
        displayVideoPanelParent.SetActive(false);
        displayPanel.SetActive(true);

        //nftlinkText = data.detail.NFTWebPageLink;
        //ownerlinkText = data.detail.OwnerLink;
        displayVideoPanel.SetActive(false);
        displayVideoPanelWithStats.SetActive(false);
        displayVideoPanelPortrait.SetActive(false);

        VideoObjectWithDes.SetActive(false);
        
        

        if (data.detail.description.Length > 0)
        {
            int type;
            if (data.detail.ratio == "1:1")
            {
                type = 0;
            }
            else if (data.detail.ratio == "9:16")
            {
                type = 1;
            }
            else if (data.detail.ratio == "16:9")
            {
                type = 2;
            }
            else 
            {
                type = 0;
            }
            if (displayPanelOnlyImage != null)
                displayPanelOnlyImage.SetActive(false);

            /// fill data according to aspect ratio
            nftWithDes[type].imageLandscape.sprite = data.thunbNailImage;
            nftWithDes[type].imagePotraite.sprite = data.thunbNailImage;
            //imageObjectWithStats.GetComponent<Image>().preserveAspect = true;
            //imageObjectWithStats.GetComponent<Image>().sprite = data.thunbNailImage;

            //imageObjectWithStats_Potrait.GetComponent<Image>().preserveAspect = true;
            //imageObjectWithStats_Potrait.GetComponent<Image>().sprite = data.thunbNailImage;
            Debug.Log("Showing with stats ");

            if (LocalizationManager.forceJapanese || GameManager.currentLanguage.Equals("ja"))
            {
                nftWithDes[type].ownerTextLandscape.text = "" + data.detail.authorName[1];
                nftWithDes[type].titleTxtLandscape.text = "" + data.detail.title[1];
                nftWithDes[type].descriptionTxtLandscape.text = "" + data.detail.description[1];
            }
            //    ownerText.text = "" + data.detail.authorName[1];
            //    titleText.text = "" + data.detail.title[1];
            //    descriptionText.text = "" + data.detail.description[1];
            //}
            else
            {
                nftWithDes[type].ownerTextLandscape.text = "" + data.detail.authorName[0];
                nftWithDes[type].titleTxtLandscape.text = "" + data.detail.title[0];
                nftWithDes[type].descriptionTxtLandscape.text = "" + data.detail.description[0];
                //ownerText.text = "" + data.detail.authorName[0];
                //titleText.text = "" + data.detail.title[0];
                //descriptionText.text = "" + data.detail.description[0];
            }
            nftWithDes[type].ownerTextPotraite.text = nftWithDes[type].ownerTextLandscape.text;
            nftWithDes[type].titleTxtLandscapePotraite.text = nftWithDes[type].titleTxtLandscape.text;
            nftWithDes[type].descriptionTxtPotraite.text = nftWithDes[type].descriptionTxtLandscape.text;
            //ownerText_Potrait.text = ownerText.text;
            //titleText_Potrait.text = titleText.text;
            //descriptionText_Potrait.text = descriptionText.text;
            for (int i = 0; i < nftWithDes.Count; i++)
            {
                nftWithDes[i].potraiteObj.SetActive(false);
                nftWithDes[i].landscapeObj.SetActive(false);
            }

            if (ScreenOrientationManager._instance.isPotrait)
            {
                displayImagePanel.SetActive(false);
                displayImagePanelPortrait.SetActive(true);
                nftWithDes[type].landscapeObj.SetActive(false);
                nftWithDes[type].potraiteObj.SetActive(true);
            }
            else
            {
                nftWithDes[type].landscapeObj.SetActive(true);
                nftWithDes[type].potraiteObj.SetActive(false);
                displayImagePanelPortrait.SetActive(false);
                displayImagePanel.SetActive(true);
            }
            //displayImageWithoutDes.SetActive(false);
            //displayImageWithoutDesPortrait.SetActive(false);
        }

        else
        {
            imageObject.GetComponent<Image>().preserveAspect = true;
            imageObject.GetComponent<Image>().sprite = data.thunbNailImage;
            if (displayPanelWithStats != null)
                displayPanelWithStats.SetActive(false);


            //creatorimageObject.GetComponent<Image>().preserveAspect = true;
            //creatorimageObject.GetComponent<Image>().sprite = data.thunbNailImage;
            //ownerimageObject.GetComponent<Image>().preserveAspect = true;
            //ownerimageObject.GetComponent<Image>().sprite = data.thunbNailImage;

            imageObject.SetActive(true);
            if (imageObject_Potrait != null)
            {
                imageObject_Potrait.GetComponent<Image>().preserveAspect = true;
                imageObject_Potrait.GetComponent<Image>().sprite = data.thunbNailImage;
                imageObject_Potrait.SetActive(true);
            }

            //creatorimageObject.SetActive(true);
            //ownerimageObject.SetActive(true);
            VideoObject.SetActive(false);
            VideoObjectWithDes.SetActive(false);
            if (displayPanelOnlyImage != null)
                displayPanelOnlyImage.SetActive(true);

            if (ScreenOrientationManager._instance.isPotrait)
            {
                displayImagePanel.SetActive(false);
                displayImagePanelPortrait.SetActive(false);
                displayImageWithoutDes.SetActive(false);
                displayImageWithoutDesPortrait.GetComponent<Image>().sprite = imageObject.GetComponent<Image>().sprite;
                if (data.detail.ratio == "9:16")
                {
                    displayImageWithoutDesPortrait.transform.localPosition = new Vector3(-1.1111f, -3.2166f, 0);
                    displayImageWithoutDesPortrait.transform.localScale = Vector3.one;
                    displayImageWithoutDesPortrait.GetComponent<RectTransform>().sizeDelta = new Vector2(1170, 1711.35f);

                }
                else if (data.detail.ratio == "16:9")
                {
                    displayImageWithoutDesPortrait.transform.localPosition = new Vector3(-0.0039f, -3.2466f, 0);
                    displayImageWithoutDesPortrait.transform.localScale = Vector3.one;
                    displayImageWithoutDesPortrait.GetComponent<RectTransform>().sizeDelta = new Vector2(1170, 654);
                }
                else
                {
                    displayImageWithoutDesPortrait.transform.localPosition = new Vector3(-0.0039f, -3.0271f, 0);
                    displayImageWithoutDesPortrait.transform.localScale = Vector3.one;
                    displayImageWithoutDesPortrait.GetComponent<RectTransform>().sizeDelta = new Vector2(1170, 1170);
                }
                displayImageWithoutDesPortrait.SetActive(true);
            }
            else
            {
                displayImagePanelPortrait.SetActive(false);
                displayImagePanel.SetActive(false);
                displayImageWithoutDesPortrait.SetActive(false);
                displayImageWithoutDes.GetComponent<Image>().sprite = imageObject.GetComponent<Image>().sprite;
                Vector3 scale =  Vector3.one/*displayImageWithoutDes.transform.localScale*/;
                if (data.detail.ratio == "16:9")
                {
                    displayImageWithoutDes.transform.localPosition = Vector3.zero;
                    displayImageWithoutDes.transform.localScale = new Vector3(1.645f, scale.y, scale.z);
                    displayImageWithoutDesPortrait.GetComponent<RectTransform>().sizeDelta = new Vector2(530, 530);
                }
                else if (data.detail.ratio == "9:16")
                {
                    displayImageWithoutDes.transform.localPosition = Vector3.zero;
                    displayImageWithoutDes.transform.localScale = new Vector3(0.525f, 1.14185f, scale.z);
                    displayImageWithoutDesPortrait.GetComponent<RectTransform>().sizeDelta = new Vector2(530, 530);

                }
                else {
                    displayImageWithoutDes.transform.localPosition = Vector3.zero;
                    displayImageWithoutDes.transform.localScale = Vector3.one;
                    displayImageWithoutDesPortrait.GetComponent<RectTransform>().sizeDelta = new Vector2(530, 530);
                }
                displayImageWithoutDes.SetActive(true);
            }
        }

        // For Analytics NFT image Clicked = true
        UserAnalyticsHandler.onUpdateWorldRelatedStats?.Invoke(false, true, false, false);
    }

    public void ShowVideo(MetaDataInPrefab data)
    {
        currData = data;
        //if (videoObject.transform.childCount > 0)
        //{
        //    for(int i= videoObject.transform.childCount - 1; i >= 0; i--)
        //    {
        //        Destroy(videoObject.transform.GetChild(i));
        //    }
        //}
        if (data.metaData.description.Length > 0)
        {
            VideoObjectWithDes.transform.GetComponent<RawImage>().texture = data.videoPlayer.texture;
            if (data.thunbNailImage.texture.height / data.thunbNailImage.texture.width > 1.776f)
            {
                VideoObjectWithDes.transform.GetChild(0).GetComponent<RawImage>().rectTransform.localScale =
                    new Vector3(1 / ((data.thunbNailImage.texture.height * 1f) / (data.thunbNailImage.texture.width * 1f)), 1f, 1);
                print("Hight of Im is:" + data.thunbNailImage.texture.height);
            }
            else if (data.metaData.name.Contains("磁場の拡散（Diffusion of magnetic field）"))
            {
                VideoObjectWithDes.transform.GetComponent<RawImage>().rectTransform.localScale =
                    new Vector3(1, 0.8f / ((data.thunbNailImage.texture.height * 1f) / (data.thunbNailImage.texture.width * 1f)), 1);
                print("Hight of else Ima is:" + data.thunbNailImage.texture.height);
            }
            else
            {
                VideoObjectWithDes.transform.GetComponent<RawImage>().rectTransform.localScale =
                    new Vector3(1, 1f / ((data.thunbNailImage.texture.height * 1f) / (data.thunbNailImage.texture.width * 1f)), 1);
                print("Hight of else Image is:" + data.thunbNailImage.texture.height);
            }


        }
        else
        {
            VideoObject.transform.GetComponent<RawImage>().texture = data.videoPlayer.texture;
            if (data.thunbNailImage.texture.height / data.thunbNailImage.texture.width > 1.776f)
            {
                VideoObject.transform.GetChild(0).GetComponent<RawImage>().rectTransform.localScale =
                    new Vector3(1 / ((data.thunbNailImage.texture.height * 1f) / (data.thunbNailImage.texture.width * 1f)), 1f, 1);
                print("Hight of Im is:" + data.thunbNailImage.texture.height);
            }
            else if (data.metaData.name.Contains("磁場の拡散（Diffusion of magnetic field）"))
            {
                VideoObject.transform.GetComponent<RawImage>().rectTransform.localScale =
                    new Vector3(1, 0.8f / ((data.thunbNailImage.texture.height * 1f) / (data.thunbNailImage.texture.width * 1f)), 1);
                print("Hight of else Ima is:" + data.thunbNailImage.texture.height);
            }
            else
            {
                VideoObject.transform.GetComponent<RawImage>().rectTransform.localScale =
                    new Vector3(1, 1f / ((data.thunbNailImage.texture.height * 1f) / (data.thunbNailImage.texture.width * 1f)), 1);
                print("Hight of else Image is:" + data.thunbNailImage.texture.height);
            }


        }
        creatorimageObject.GetComponent<Image>().preserveAspect = true;
        creatorimageObject.GetComponent<Image>().sprite = dummyprofileIcone;
        ownerimageObject.GetComponent<Image>().preserveAspect = true;
        ownerimageObject.GetComponent<Image>().sprite = dummyprofileIcone;
        VideoObject.transform.rotation = Quaternion.identity;
        VideoObjectWithDes.transform.rotation = Quaternion.identity;
        data.videoPlayer.SetDirectAudioMute(0, false);

        videoUsernameText.text = "" + data.creatorDetails.username;
        videoOwnerText.text = "" + data.creatorDetails.username;

        videoUsernameText_Potrait.text = videoUsernameText.text;
        videoOwnerText_Potrait.text = videoOwnerText.text;

        nftlinkText = data.nftLink;
        ownerlinkText = data.creatorLink;
        if (GameManager.currentLanguage == "ja")
        {
            videoTitleText.text = "" + data.metaData.name;
            videoDescriptionText.text = "" + data.tokenDetails.ja_nft_description;
        }
        else
        {
            videoTitleText.text = "" + data.metaData.name;
            videoDescriptionText.text = "" + data.tokenDetails.en_nft_description;
        }

        videoTitleText_Potrait.text = videoTitleText.text;
        videoDescriptionText_Potrait.text = videoDescriptionText.text;


        //usernameText.text = "" + mdip.metaData.username;
        //videoObject.transform.GetChild(0).GetComponent<RawImage>().mainTexture.width = mdip.thunbNailImage.texture.width;
        //Instantiate(mdip.spriteObject, this.transform.position, this.transform.rotation, videoObject.transform);
        //videoObject.GetComponent<Image>().sprite = mdip.GetComponent<Image>().sprite;
        // imageObject.SetActive(false);
        VideoObject.SetActive(true);
        VideoObjectWithDes.SetActive(true);
        displayPanel.SetActive(true);

        // For Analytics NFT image Clicked = true
        UserAnalyticsHandler.onUpdateWorldRelatedStats?.Invoke(false, true, false, false);

        //if (imageObject_Potrait)
        //    imageObject_Potrait.SetActive(false);
    }

    /// <summary>
    /// Function to show video with description or just image in dynamic musuem
    /// </summary>
    /// <param name="data"></param>
    public void ShowVideo(DynamicGalleryData data)
    {
        displayPanelWithStats.SetActive(false);
        //VideoObjectWithDes.SetActive(true);
        displayVideoPanelParent.SetActive(true);
        if (data.detail.description.Length > 0)
        {
            int type;
            if (data.detail.ratio == "1:1")
            {
                type = 0;
            }
            else if (data.detail.ratio == "9:16")
            {
                type = 1;
            }
            else if (data.detail.ratio == "16:9")
            {
                type = 2;
            }
            else
            {
                type = 0;
            }
            //displayVideoPanel.SetActive(false);
            loadingImage.SetActive(true);
            SetLoader(ScreenOrientationManager._instance.isPotrait);
            //displayVideoPanel.GetComponent<Image>().enabled = false;
            
            VideoObjectWithDes.GetComponent<VideoPlayer>().url = data.detail.asset_link;
            VideoObjectWithDes.gameObject.SetActive(true);
            VideoObjectWithDes.GetComponent<RawImage>().enabled = false;
            
            if (LocalizationManager.forceJapanese || GameManager.currentLanguage.Equals("ja"))
            {
                nftWithDes[type].videoDescriptionTxtLandscape.text = "" + data.detail.description[1];
                nftWithDes[type].videoTitleLandscape.text = "" + data.detail.title[1];
                nftWithDes[type].videoOwnerTextLandscape.text = "" + data.detail.authorName[1];
            }
            else
            {
                nftWithDes[type].videoDescriptionTxtLandscape.text = "" + data.detail.description[0];
                nftWithDes[type].videoTitleLandscape.text = "" + data.detail.title[0];
                nftWithDes[type].videoOwnerTextLandscape.text = "" + data.detail.authorName[0];
                //videoOwnerText.text = "" + data.detail.authorName[0];
                //videoTitleText.text = "" + data.detail.title[0];
                //videoDescriptionText.text = "" + data.detail.description[0];
            }
            nftWithDes[type].videoOwnerTextPotraite.text = nftWithDes[type].videoOwnerTextLandscape.text;
            nftWithDes[type].titleTxtLandscapePotraite.text = nftWithDes[type].titleTxtLandscape.text;
            nftWithDes[type].descriptionTxtPotraite.text = nftWithDes[type].descriptionTxtLandscape.text;
            for (int i = 0; i < nftWithDes.Count; i++)
            {
                nftWithDes[i].videoLandscapeObj.SetActive(false);
                nftWithDes[i].videoPotraiteObj.SetActive(false);
            }

            //videoOwnerText_Potrait.text = videoOwnerText.text;
            //videoTitleText_Potrait.text = videoTitleText.text;
            //videoDescriptionText_Potrait.text = videoDescriptionText.text;


            if (ScreenOrientationManager._instance.isPotrait)
            {
                displayVideoPanelPortrait.SetActive(true);
                displayVideoPanelWithStats.SetActive(false);
                //VideoObjectWithDes.transform.localScale = new Vector3(2, 2, 2);
                //VideoObjectWithDes.transform.localPosition = new Vector3(0f, 300f, 0f);
                if (data.detail.ratio == "16:9")
                {
                    VideoObjectWithDes.transform.localScale = new Vector3(2, 2, 2);
                    VideoObjectWithDes.transform.localPosition = new Vector3(-0.726f, 323.908f, 0f);
                    VideoObjectWithDes.GetComponent<RectTransform>().sizeDelta = new Vector2(586.597f, 270.345f);
                }
                else if (data.detail.ratio == "9:16")
                {
                    VideoObjectWithDes.transform.localScale = new Vector3(2, 2, 2);
                    VideoObjectWithDes.transform.localPosition = new Vector3(0f, 371.526f, 0f);
                    VideoObjectWithDes.GetComponent<RectTransform>().sizeDelta = new Vector2(429.611f, 626.701f);
                }
                else
                {
                    VideoObjectWithDes.transform.localScale = new Vector3(2, 2, 2);
                    VideoObjectWithDes.transform.localPosition = new Vector3(1.045f, 320.178f, 0f);
                    VideoObjectWithDes.transform.localEulerAngles = new Vector3(0f, 0f, 90f);
                    VideoObjectWithDes.GetComponent<RectTransform>().sizeDelta = new Vector2(483.513f, 589.744f);
                }
                nftWithDes[type].videoLandscapeObj.SetActive(false);
                nftWithDes[type].videoPotraiteObj.SetActive(true);
            }
            else
            {
                displayVideoPanelPortrait.SetActive(false);
                displayVideoPanelWithStats.SetActive(true); 
                if (data.detail.ratio=="1:1")
                {
                    VideoObjectWithDes.transform.localScale = new Vector3(1, 1.225f, 1);
                    VideoObjectWithDes.transform.localPosition = new Vector3(-304.6486f, -0.5784f, 0f);
                    VideoObjectWithDes.transform.localEulerAngles = new Vector3(0f, 0f, 90f);
                    VideoObjectWithDes.GetComponent<RectTransform>().sizeDelta = new Vector2(471.8433f, 385.856f);
                }
                else if (data.detail.ratio == "9:16")
                {
                    VideoObjectWithDes.transform.localScale = new Vector3(0.58f, 1.28f, 1);
                    VideoObjectWithDes.transform.localPosition = new Vector3(-333, 0f, 0f);
                    VideoObjectWithDes.GetComponent<RectTransform>().sizeDelta = new Vector2(473f, 470f);
                }
                else
                {
                    VideoObjectWithDes.transform.localScale = new Vector3(1f, 1, 1);
                    VideoObjectWithDes.transform.localPosition = new Vector3(-235.4535f, -0.6664f, 0f);
                    VideoObjectWithDes.GetComponent<RectTransform>().sizeDelta = new Vector2(715.0989f, 488.3264f);
                }
                nftWithDes[type].videoLandscapeObj.SetActive(true);
                nftWithDes[type].videoPotraiteObj.SetActive(false);
            }
        }
        else
        {
            loadingImage.SetActive(true);
            SetLoader(ScreenOrientationManager._instance.isPotrait);
            VideoObject.GetComponent<VideoPlayer>().url = data.detail.asset_link;
            VideoObject.gameObject.SetActive(true);
            VideoObject.GetComponent<RawImage>().enabled = false;
            VideoObjectWithDes.gameObject.SetActive(false);
            displayVideoPanelParent.SetActive(false);
            displayPanelWithStats.SetActive(false);
            displayVideoPanel.SetActive(true);
            displayVideoPanelWithStats.SetActive(false);
            displayVideoPanelPortrait.SetActive(false);
            if (ScreenOrientationManager._instance.isPotrait)
            {
                //VideoObject.transform.localScale = new Vector3(2, 2, 2);
                //VideoObject.transform.localPosition = new Vector3(0f, 300f, 0f);
                if (data.detail.ratio == "16:9")
                {
                    VideoObject.transform.localScale = new Vector3(1, 1, 1);
                    VideoObject.transform.localPosition = new Vector3(0, 0, 0f);
                    VideoObject.GetComponent<RectTransform>().sizeDelta = new Vector2(1172.45f, 664.2413f);
                }
                else if (data.detail.ratio == "9:16")
                {
                    VideoObject.transform.localScale = new Vector3(1, 1, 1);
                    VideoObject.transform.localPosition = new Vector3(0.0971f, 0.195f, 0);
                    VideoObject.GetComponent<RectTransform>().sizeDelta = new Vector2(1172.45f, 1707.986f);
                }
                else
                {
                    VideoObject.transform.localScale = new Vector3(1, 1, 1);
                    VideoObject.transform.localPosition = new Vector3(0, 0, 0);
                    VideoObject.GetComponent<RectTransform>().sizeDelta = new Vector2(1172.45f, 1172.45f);
                }
            }
            else
            {
                Vector3 scale = Vector3.one /*VideoObject.transform.localScale*/;
                if (data.detail.ratio == "16:9")
                {
                    VideoObject.transform.localScale = new Vector3(1.645f, scale.y, scale.z);
                    VideoObject.GetComponent<RectTransform>().sizeDelta = new Vector2(530f, 530f);
                }
                else if (data.detail.ratio == "9:16")
                {
                    VideoObject.transform.localScale = new Vector3(0.525f, 1.1433f, scale.z);
                    VideoObject.GetComponent<RectTransform>().sizeDelta = new Vector2(530f, 530f);
                }
                else
                {
                    VideoObject.transform.localScale = new Vector3(1/*0.92390f*/,1 /*1.125f*/, 1);
                    VideoObject.transform.localPosition = new Vector3(0f, 0f, 0f);
                    VideoObject.GetComponent<RectTransform>().sizeDelta = new Vector2(530f, 530f);
                }
               
            }
        }

        //VideoObject.transform.GetComponent<RawImage>().texture = data.videoPlayer.texture;
        //VideoObjectWithDes.transform.GetComponent<RawImage>().texture = data.videoPlayer.texture;
        //if (data.thunbNailImage.texture.height / data.thunbNailImage.texture.width > 1.776f)
        //{
        //    VideoObject.transform.GetChild(0).GetComponent<RawImage>().rectTransform.localScale =
        //        new Vector3(1 / ((data.thunbNailImage.texture.height * 1f) / (data.thunbNailImage.texture.width * 1f)), 1f, 1);
        //    print("Hight of Im is:" + data.thunbNailImage.texture.height);
        //    VideoObjectWithDes.transform.GetChild(0).GetComponent<RawImage>().rectTransform.localScale =
        //        new Vector3(1 / ((data.thunbNailImage.texture.height * 1f) / (data.thunbNailImage.texture.width * 1f)), 1f, 1);
        //    print("Hight of Im is:" + data.thunbNailImage.texture.height);
        //}

        //else
        //{
        //    VideoObject.transform.GetComponent<RawImage>().rectTransform.localScale =
        //        new Vector3(1, 1f / ((data.thunbNailImage.texture.height * 1f) / (data.thunbNailImage.texture.width * 1f)), 1);
        //    print("Hight of else Image is:" + data.thunbNailImage.texture.height);
        //    VideoObjectWithDes.transform.GetComponent<RawImage>().rectTransform.localScale =
        //       new Vector3(1, 1f / ((data.thunbNailImage.texture.height * 1f) / (data.thunbNailImage.texture.width * 1f)), 1);
        //    print("Hight of else Image is:" + data.thunbNailImage.texture.height);
        //}
        //creatorimageObject.GetComponent<Image>().preserveAspect = true;
        //creatorimageObject.GetComponent<Image>().sprite = dummyprofileIcone;
        //ownerimageObject.GetComponent<Image>().preserveAspect = true;
        //ownerimageObject.GetComponent<Image>().sprite = dummyprofileIcone;
        //VideoObject.transform.rotation = Quaternion.identity;
        //VideoObjectWithDes.transform.rotation = Quaternion.identity;

        //mdip.videoPlayer.SetDirectAudioMute(0, false);
        //usernameText.text = "" + data.detail.OwnerName;
        //ownerText.text = "" + data.detail.OwnerName;
        //nftlinkText = data.detail.NFTWebPageLink;
        //ownerlinkText = data.detail.OwnerLink;
        //if (GameManager.currentLanguage == "ja")
        //{
        //    titleText.text = "" + data.detail.Title;
        //    descriptionText.text = "" + data.detail.Description;
        //}
        //else
        //{
        //    titleText.text = "" + data.detail.Title;
        //    descriptionText.text = "" + data.detail.Description;
        //}
        //usernameText.text = "" + mdip.metaData.username;
        //videoObject.transform.GetChild(0).GetComponent<RawImage>().mainTexture.width = mdip.thunbNailImage.texture.width;
        //Instantiate(mdip.spriteObject, this.transform.position, this.transform.rotation, videoObject.transform);
        //videoObject.GetComponent<Image>().sprite = mdip.GetComponent<Image>().sprite;
        // imageObject.SetActive(false);
        // VideoObject.transform.parent.gameObject.SetActive(true);
        // VideoObjectWithDes.transform.parent.gameObject.SetActive(true);

        displayImageWithoutDes.SetActive(false);
        displayImageWithoutDesPortrait.SetActive(false);
        displayPanel.SetActive(true);

        // For Analytics NFT image Clicked = true
        UserAnalyticsHandler.onUpdateWorldRelatedStats?.Invoke(false, true, false, false);

        //if (imageObject_Potrait)
        //    imageObject_Potrait.SetActive(false);
    }

    public void UpdateGif(Sprite currFrame)
    {
        imageObject.GetComponent<Image>().sprite = currFrame;
        if (imageObject_Potrait)
            imageObject_Potrait.GetComponent<Image>().sprite = currFrame;

        if (displayImageWithoutDes)
            displayImageWithoutDes.GetComponent<Image>().sprite = currFrame;
        if (displayImageWithoutDesPortrait)
            displayImageWithoutDesPortrait.GetComponent<Image>().sprite = currFrame;
    }
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            Debug.Log("---Astroboy Nft Close due to Application minimized");
            ClosePanel();
        }

    }
    public void ClosePanel()
    {
        ShowNFTDetails.instance.loadingImage.SetActive(false);
        SetLoader(ScreenOrientationManager._instance.isPotrait);
        potraiteLoaderIcon.SetActive(false);
        landscapeLoaderIcon.SetActive(false);
        VideoObject.gameObject.SetActive(false);
        displayPanel.SetActive(false);
        if (currData != null)
        {
            if (currData.videoPlayer != null)
            {
                currData.videoPlayer.SetDirectAudioMute(0, true);
                currData.videoPlayer.Pause();
            }
            currData.isVisible = false;
            currData = null;
        }

        imageObject.GetComponent<Image>().sprite = null;
        if (imageObject_Potrait)
            imageObject_Potrait.GetComponent<Image>().sprite = null;

        creatorimageObject.GetComponent<Image>().sprite = null;
        ownerimageObject.GetComponent<Image>().sprite = null;
        if (nFTFromServer != null && nFTFromServer.isDynamicMuseum)
        {
            GamePlayUIHandler.inst.gamePlayUIParent.SetActive(true);
        }

        Resources.UnloadUnusedAssets();
    }

    public void directtoUrl()
    {
        Application.OpenURL(nftlinkText);
    }

    public void owneroUrl()
    {
        Application.OpenURL(ownerlinkText);
    }

    private void M_VideoPlayerOnerrorReceived(VideoPlayer source, string message)
    {
        HandleErrors();
    }

    public void HandleErrors()
    {
        StopAllCoroutines();
        displayPanel.SetActive(false);
        VideoObject.transform.parent.transform.gameObject.SetActive(false);
        VideoObjectWithDes.transform.parent.transform.gameObject.SetActive(false);
        // errorPanel.SetActive(true);
    }

    private void M_VideoPlayerOnprepareCompleted(VideoPlayer source)
    {
        VideoObject.GetComponent<RawImage>().enabled = true;
        loadingImage.SetActive(false);
        SetLoader(ScreenOrientationManager._instance.isPotrait);
        VideoObject.GetComponent<VideoPlayer>().Play();
    }
    private void M_VideoPlayerOnprepareCompletedWithStats(VideoPlayer source)
    {
        VideoObjectWithDes.GetComponent<RawImage>().enabled = true;
        loadingImage.SetActive(false);
        SetLoader(ScreenOrientationManager._instance.isPotrait);
        VideoObjectWithDes.GetComponent<VideoPlayer>().Play();
    }

    /// <summary>
    /// Set loader according to oritention
    /// </summary>
    void SetLoader(bool isLandscape) {
        potraiteLoaderIcon.SetActive(false);
        landscapeLoaderIcon.SetActive(false);
        if (isLandscape) // is Landscape
        {
            potraiteLoaderIcon.SetActive(false);
            landscapeLoaderIcon.SetActive(true);
        }
        else // is potrate
        {
            potraiteLoaderIcon.SetActive(true);
            landscapeLoaderIcon.SetActive(false);
        }
    }
}

[Serializable]
public class NftViewwithDes
{
    public string name;
    public GameObject landscapeObj, potraiteObj, videoPotraiteObj, videoLandscapeObj;
    public Image imageLandscape, imagePotraite;
    //public RawImage videoLandscape, videoPotraite;
    public TextMeshProUGUI titleTxtLandscape, titleTxtLandscapePotraite, videoTitleLandscape, videoTitlePotraite;
    public TextMeshProUGUI ownerTextLandscape, ownerTextPotraite, videoOwnerTextLandscape, videoOwnerTextPotraite;
    public TextMeshProUGUI descriptionTxtLandscape, descriptionTxtPotraite, videoDescriptionTxtLandscape, videoDescriptionTxtPotraite;
}
