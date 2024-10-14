using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Toyota;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SummitDomeImageHandler : MonoBehaviour
{
    public XANASummitDataContainer XANASummitDataContainer;
    public TMPro.TMP_FontAsset DometextFont;
    public Material DomeTextMaterial;
    public NFT_Holder_Manager CommonScreen;

    public static Action<int> ShowNftData;
    public static Action CloseLoaderObject;

    private Dictionary<string, Texture> _textureCache = new Dictionary<string, Texture>();

    AdvancedYoutubePlayer _youtubePlayer;
    GameObject _portraitLoader;
    GameObject _landscapeLoader;


    void OnEnable()
    {
        BuilderEventManager.AfterWorldOffcialWorldsInatantiated += ApplyDomeShader;
        ShowNftData += SetInfo;
        CloseLoaderObject += CloseLoader;
    }

    private void OnDisable()
    {
        BuilderEventManager.AfterWorldOffcialWorldsInatantiated -= ApplyDomeShader;
        ShowNftData -= SetInfo;
        CloseLoaderObject -= CloseLoader;
    }

    void ApplyDomeShader()
    {
        for (int i = 0; i < XanaWorldDownloader.AllDomes.Count; i++)
        {
            SummitDomeShaderApply SummitDomeShaderApplyRef = XanaWorldDownloader.AllDomes[i].GetComponent<SummitDomeShaderApply>();
            string[] DomeData = XANASummitDataContainer.GetDomeImage(SummitDomeShaderApplyRef.DomeId);
            SummitDomeShaderApplyRef.ImageUrl = DomeData[0];
            SummitDomeShaderApplyRef.LogoUrl = DomeData[2];
            if (!string.IsNullOrEmpty(DomeData[1]) && string.IsNullOrEmpty(DomeData[2]))
            {
                TMPro.TextMeshPro DomeText1 = SummitDomeShaderApplyRef.DomeText.AddComponent<TMPro.TextMeshPro>();
                DomeText1.font = DometextFont;
                DomeText1.fontMaterial = DomeTextMaterial;
                DomeText1.fontSize = 6f;
                DomeText1.alignment = TMPro.TextAlignmentOptions.Center;
                DomeText1.text = DomeData[1];
            }
            SummitDomeShaderApplyRef.Init();

        }
    }
    public void Enable_PDF_Panel()
    {
        if (!ScreenOrientationManager._instance.isPotrait)
            CommonScreen.pdfPanel_L.SetActive(true);
        else
            CommonScreen.pdfPanel_P.SetActive(true);

        ReferencesForGamePlay.instance.eventSystemObj.SetActive(false);
        PlayerCameraController.instance.isReturn = true;
    }
    public void EnableControlls()
    {

        if (GamePlayUIHandler.inst.gameObject.activeInHierarchy)
        {
            GamePlayUIHandler.inst.gamePlayUIParent.SetActive(true);
        }

        ReferencesForGamePlay.instance.eventSystemObj.SetActive(true);
        PlayerCameraController.instance.isReturn = false;
    }

    private PMY_Ratio DetermineRatio(string proportionType, string mediaType)
    {
        switch (proportionType)
        {
            case "1:1":
                return mediaType == "Video" || mediaType == "Live" ? PMY_Ratio.OneXOneWithoutDes : PMY_Ratio.OneXOneWithDes;
            case "16:9":
                return mediaType == "Video" || mediaType == "Live" ? PMY_Ratio.SixteenXNineWithoutDes : PMY_Ratio.SixteenXNineWithDes;
            case "9:16":
                return mediaType == "Video" || mediaType == "Live" ? PMY_Ratio.NineXSixteenWithoutDes : PMY_Ratio.NineXSixteenWithDes;
            case "4:3":
                return mediaType == "Video" || mediaType == "Live" ? PMY_Ratio.FourXThreeWithoutDes : PMY_Ratio.FourXThreeWithDes;
            default:
                return mediaType == "Video" || mediaType == "Live" ? PMY_Ratio.OneXOneWithoutDes : PMY_Ratio.OneXOneWithDes;
        }
    }

    public async void SetInfo(int domeID)
    {
        var domedata = XANASummitDataContainer.GetDomeData(domeID);
        string compersionPrfex;
        PMY_Ratio PMY_Ratio = DetermineRatio(domedata.proportionType, domedata.mediaType);

        int ratioId = (int)PMY_Ratio;
        if (domedata.mediaType == "Pdf")
        {
            if (string.IsNullOrEmpty(domedata.mediaUpload))
                return;

            CommonScreen.pdfViewer_L.FileURL = domedata.mediaUpload;
            CommonScreen.pdfViewer_P.FileURL = domedata.mediaUpload;
            Enable_PDF_Panel();
        }
        //else if (type == PMY_DataType.Quiz)
        //{
        //    quizPanel_L.GetComponent<PMY_QuizController>().SetQuizData(quizData);

        //    quizPanel_P.GetComponent<PMY_QuizController>().SetQuizData(quizData);
        //    EnableQuizPanel();
        //}

        else
        {
            var images = await DownloadDomeTexture(domedata.world360Image);
            // Setting Landscape Data
            CommonScreen.ratioReferences[ratioId].l_image.gameObject.SetActive(true);
            CommonScreen.ratioReferences[ratioId].p_image.gameObject.SetActive(true);

            CommonScreen.ratioReferences[ratioId].p_videoPlayer.gameObject.SetActive(true);
            CommonScreen.ratioReferences[ratioId].l_videoPlayer.gameObject.SetActive(true);
            if (ratioId < 4)
            {
                CommonScreen.ratioReferences[ratioId].l_Title.text = domedata.name;
                CommonScreen.ratioReferences[ratioId].l_Aurthur.text = domedata.creatorName;
                CommonScreen.ratioReferences[ratioId].l_Description.text = domedata.description;// + "\n" + "<link=" + url + "><u>" + url + "</u></link>";
            }
            if (domedata.mediaType == "Image")
            {
                var image = await DownloadDomeTexture(domedata.mediaUpload);
                CommonScreen.ratioReferences[ratioId].l_image.texture = image;
                CommonScreen.ratioReferences[ratioId].l_videoPlayer.gameObject.SetActive(false);
            }
            else
            {
                CommonScreen.ratioReferences[ratioId].l_image.gameObject.SetActive(false);
                CommonScreen.ratioReferences[ratioId].l_videoPlayer.url = domedata.mediaUpload;
            }

            // Setting Potraite Data
            if (ratioId < 4)
            {
                CommonScreen.ratioReferences[ratioId].p_Title.text = domedata.name; ;
                CommonScreen.ratioReferences[ratioId].p_Aurthur.text = domedata.creatorName;
                CommonScreen.ratioReferences[ratioId].p_Description.text = domedata.description;// + "\n" + "<link=" + url + "><u>" + url + "</u></link>";
            }
            CommonScreen.ratioReferences[ratioId].p_image.texture = images;
            if (domedata.mediaType == "Image")
            {
                var image = await DownloadDomeTexture(domedata.mediaUpload);
                CommonScreen.ratioReferences[ratioId].p_image.texture = image;
                CommonScreen.ratioReferences[ratioId].p_videoPlayer.gameObject.SetActive(false);
            }
            else
            {
                CommonScreen.ratioReferences[ratioId].p_image.gameObject.SetActive(false);
                CommonScreen.ratioReferences[ratioId].p_videoPlayer.url = domedata.mediaUpload;
            }

            if (!ScreenOrientationManager._instance.isPotrait) // for Landscape
            {
                CommonScreen.LandscapeObj.SetActive(true);
                CommonScreen.PotraiteObj.SetActive(false);
                CommonScreen.ratioReferences[ratioId].l_obj.SetActive(true);
                CommonScreen.ratioReferences[ratioId].p_obj.SetActive(false);
                if (domedata.mediaType == "Video" )
                {
                    _landscapeLoader = CommonScreen.ratioReferences[ratioId].l_Loader.gameObject;
                    _landscapeLoader.SetActive(true);
                    _portraitLoader = CommonScreen.ratioReferences[ratioId].p_Loader.gameObject;
                    _portraitLoader.SetActive(false);
                    _youtubePlayer = CommonScreen.ratioReferences[ratioId].l_obj.GetComponent<AdvancedYoutubePlayer>();
                    _youtubePlayer.ResetVideoURL();
                    if (domedata.videoType == "Live" && domedata.isYoutubeUrl)
                    {
                        CommonScreen.ratioReferences[ratioId].l_videoPlayer.GetComponent<RawImage>().enabled = false;
                        CommonScreen.ratioReferences[ratioId].l_videoPlayer.enabled = false;
                        CommonScreen.ratioReferences[ratioId].l_PrerecordedPlayer.SetActive(false);
                        CommonScreen.ratioReferences[ratioId].l_LivePlayer.SetActive(true);
                        _youtubePlayer.IsLive = true;
                        _youtubePlayer.VideoId = domedata.mediaUpload;
                        _youtubePlayer.PlayVideo();
                    }
                    else if (domedata.videoType == "Prerecorded" && domedata.isYoutubeUrl)
                    {
                        CommonScreen.ratioReferences[ratioId].l_videoPlayer.GetComponent<RawImage>().enabled = true;
                        CommonScreen.ratioReferences[ratioId].l_PrerecordedPlayer.SetActive(true);
                        CommonScreen.ratioReferences[ratioId].l_LivePlayer.SetActive(false);
                        _youtubePlayer.IsLive = false;
                        _youtubePlayer.VideoId = _youtubePlayer.ExtractVideoIdFromUrl(domedata.mediaUpload);
                        _youtubePlayer.PlayVideo();
                    }
                    else if (!domedata.isYoutubeUrl)
                    {
                        Debug.Log("Ratio " + ratioId + "  " + domedata.name +"  Video Url " + domedata.mediaUpload);
                        if (CommonScreen.ratioReferences[ratioId].l_PrerecordedPlayer)
                            CommonScreen.ratioReferences[ratioId].l_PrerecordedPlayer.SetActive(false);

                        if (CommonScreen.ratioReferences[ratioId].l_LivePlayer)
                            CommonScreen.ratioReferences[ratioId].l_LivePlayer.SetActive(false);

                        CommonScreen.ratioReferences[ratioId].l_videoPlayer.GetComponent<RawImage>().enabled = true;
                        CommonScreen.ratioReferences[ratioId].l_videoPlayer.enabled = true;
                        CommonScreen.ratioReferences[ratioId].l_videoPlayer.url = domedata.mediaUpload;
                        CommonScreen.ratioReferences[ratioId].l_videoPlayer.prepareCompleted += (vid) => { NFT_Holder_Manager.instance.videoReady(); };
                        CommonScreen.ratioReferences[ratioId].l_videoPlayer.Play();
                    }

                    //    OnVideoEnlargeAction?.Invoke();
                }
            }
            else // for Potraite
            {
                CommonScreen.LandscapeObj.SetActive(false);
                CommonScreen.PotraiteObj.SetActive(true);
                CommonScreen.ratioReferences[ratioId].l_obj.SetActive(false);
                CommonScreen.ratioReferences[ratioId].p_obj.SetActive(true);
                if (domedata.mediaType == "Video")
                {
                    _landscapeLoader = CommonScreen.ratioReferences[ratioId].l_Loader.gameObject;
                    _landscapeLoader.SetActive(false);
                    _portraitLoader = CommonScreen.ratioReferences[ratioId].p_Loader.gameObject;
                    _portraitLoader.SetActive(true);

                    if (domedata.videoType == "Live" && domedata.isYoutubeUrl)
                    {
                        CommonScreen.ratioReferences[ratioId].p_videoPlayer.GetComponent<RawImage>().enabled = false;
                        CommonScreen.ratioReferences[ratioId].p_videoPlayer.enabled = false;
                        CommonScreen.ratioReferences[ratioId].p_PrerecordedPlayer.SetActive(false);
                        CommonScreen.ratioReferences[ratioId].p_LivePlayer.SetActive(true);

                        CommonScreen.ratioReferences[ratioId].p_LivePlayer.GetComponent<StreamYoutubeVideo>().StreamYtVideo(domedata.mediaUpload, true);
                    }
                    else if (domedata.videoType == "Prerecorded" && domedata.isYoutubeUrl)
                    {
                        CommonScreen.ratioReferences[ratioId].p_videoPlayer.GetComponent<RawImage>().enabled = true;
                        CommonScreen.ratioReferences[ratioId].p_PrerecordedPlayer.SetActive(true);
                        CommonScreen.ratioReferences[ratioId].p_LivePlayer.SetActive(false);

                        CommonScreen.ratioReferences[ratioId].p_PrerecordedPlayer.GetComponent<StreamYoutubeVideo>().StreamYtVideo(domedata.mediaUpload, false);
                    }
                    else if (!domedata.isYoutubeUrl)
                    {
                        Debug.Log("Ratio " + ratioId + "  " + domedata.name);
                        CommonScreen.ratioReferences[ratioId].p_PrerecordedPlayer.SetActive(false);
                        CommonScreen.ratioReferences[ratioId].p_LivePlayer.SetActive(false);
                        CommonScreen.ratioReferences[ratioId].p_videoPlayer.GetComponent<RawImage>().enabled = true;
                        CommonScreen.ratioReferences[ratioId].p_videoPlayer.enabled = true;
                        CommonScreen.ratioReferences[ratioId].p_videoPlayer.prepareCompleted += (vid) => { NFT_Holder_Manager.instance.videoReady(); };
                        CommonScreen.ratioReferences[ratioId].p_videoPlayer.url = domedata.mediaUpload;
                        CommonScreen.ratioReferences[ratioId].p_videoPlayer.Play();

                    }

                    // OnVideoEnlargeAction.Invoke();
                }

            }
        }
        if (GamePlayUIHandler.inst.gameObject.activeInHierarchy)
        {
            GamePlayUIHandler.inst.gamePlayUIParent.SetActive(false);
        }

        async Task<Texture> DownloadDomeTexture(string url)
        {
            if (_textureCache.ContainsKey(url))
                return _textureCache[url];

            UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
            await request.SendWebRequest();
            if ((request.result == UnityWebRequest.Result.ConnectionError) || (request.result == UnityWebRequest.Result.ProtocolError))
                Debug.Log(request.error);
            else
            {
                var texture = DownloadHandlerTexture.GetContent(request);
                _textureCache[url] = texture;
                return texture;
            }
            request.Dispose();
            return null;
        }

    }

    void CloseLoader()
    {
        if (_landscapeLoader)
            _landscapeLoader.SetActive(false);
        if (_portraitLoader)
            _portraitLoader.SetActive(false);
    }
}
