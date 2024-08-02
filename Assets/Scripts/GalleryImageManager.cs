using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Parameters;
using UnityEngine.Networking;
using UnityEngine.PlayerLoop;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class GalleryImageManager : MonoBehaviour
{
    public static GalleryImageManager Instance;

    [Header("Overlay Museum Panels")]
    public GameObject[] overlayPanels;
    public GameObject[] picturePanel;
    public List<GameObject> NFTSpawnPoints;
    public List<GameObject> StaticNFTSpawnPoints;

    public MuseumRaycaster museumRaycaster;

    [Header("Museum Canvas")]
    public GameObject m_MuseumCanvas;

    [Header("Picture And Banner Panel Canvas")]
    public GameObject m_PictureBannerCanvas;

    [Header("Picture Description Panel")]
    public GameObject m_PictureDescriptionPanel;
    public GameObject m_Picture;
    public GameObject cancel;
    public GameObject cancel_Portrait;
    public GameObject m_Certificate; // Each picture has a certificate but few picture have special certificates
    public GameObject m_SpecialCertificate;
    public GameObject m_DescriptionText;
    public GameObject m_LoadingIcon;
    public GameObject m_ExceptionText;
    public GameObject NFTLoader
    {
        get
        {
            if (ScreenOrientationManager._instance.isPotrait && cancel_Portrait != null)
            {
                m_LoadingIcon.transform.localScale = Vector3.one * 0.5f;
            }
            else
            {
                m_LoadingIcon.transform.localScale = Vector3.one;
            }
            return m_LoadingIcon;
        }
    }

    [Header("Pictures Sprites")]
    public Sprite[] m_PictureSprites;

    [Header("Certificates Sprites")]
    public Sprite[] m_CertificateSprites;
    public GameObject certificate;
    public string[] certificateTexts;
    public GameObject certificateTitle;

    [Header("Sprite Download Links")]
    public string[] m_PictureSpriteDownloadLinks;
    public bool m_IsLoadingImagesFromServerSide;

    [Header("Special Painting Textures")]
    public Texture[] m_SpecialPaintingTexture;

    Dictionary<int, int> m_SpecialPaintingIndex = new Dictionary<int, int>();

    // ** To Display Banner Images on Canvas

    [Header("Banner Picture Panel")]
    public GameObject m_BannerPicturePanel;
    public GameObject m_HorizontalBanner;
    public GameObject m_VerticalBanner;


    [Header("Banner Picture Sprites")]
    public Sprite[] m_BannerSprites;
    public string[] m_BannerSpriteDownloadLinks;


    [Header("Mayor And Judge Frames")]
    public Sprite[] m_MayorJudgeFrameSprites;

    // ** IsPictureDescriptionPanelActive ** //
    [HideInInspector]
    public bool m_IsDescriptionPanelActive;

    // ** IsShowingLoadingBar ** //
    bool m_IsShowingLoadingBar;
    Sprite m_DownloadedSprite;

    [Header("Video Panel")]
    public GameObject m_VideoPanel;
    public VideoPlayer m_VideoPlayer;
    public GameObject XanaSummitNFTHandler;
    public RenderTexture m_RenderTexture;
    public bool m_IsVideo;
    public bool scenec_cheker = false;

    public GameObject errorPanel;

    public bool isGOz;
    private int l_VideoIndex;
    private int m_PictureIndex;
    private bool m_IsSpecialPainting;
    private bool m_IsPortrait;
    public bool changeAlphaValue_descPanel = false;
    public bool IsStaticMuseum = false;

    void Awake()
    {
        Instance = this;
        try
        {
            m_PictureDescriptionPanel.gameObject.GetComponent<Image>().enabled = true;

            if (!changeAlphaValue_descPanel)
                m_PictureDescriptionPanel.gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 0);

            m_PictureDescriptionPanel.gameObject.layer = LayerMask.NameToLayer("NFTDisplayPanel");
        }
        catch (Exception e)
        {
            //Debug.Log(e.ToString());
        }
        if (ConstantsHolder.isFromXANASummit)
        {
            if (IsStaticMuseum)
            {
                SetStaticNFTState();
                GameObject _summitNFTHandlerRef = Instantiate(XanaSummitNFTHandler, Vector3.zero, Quaternion.identity);
                _summitNFTHandlerRef.GetComponentInChildren<SummitDomeNFTDataController>().NFTDataHandlerScrptRef = this;
            }
        }
    }

    void Start()
    {
        if (overlayPanels[0] == null)
        {
            for (int i = 0; i < overlayPanels.Length; i++)
            {
                overlayPanels[i] = ReferencesForGamePlay.instance.overlayPanels[i];
            }
        }

        m_SpecialPaintingIndex.Add(5, 0);
        m_SpecialPaintingIndex.Add(21, 1);
        m_SpecialPaintingIndex.Add(29, 2);
        m_SpecialPaintingIndex.Add(34, 3);
        m_SpecialPaintingIndex.Add(38, 4);
        m_SpecialPaintingIndex.Add(39, 5);
        if (SceneManager.GetActiveScene().name == "MuseumSceneLatest")
        {
            scenec_cheker = true;
        }
        else
        {
            scenec_cheker = false;
        }

        // LoadPictureSpriteDownloadLinks();
    }


    private void Update()
    {
        if (m_IsShowingLoadingBar)
        {
            m_LoadingIcon.GetComponent<RectTransform>().rotation *= Quaternion.Euler(Vector3.forward * -4);
        }
    }

    private void OnEnable()
    {
        m_VideoPlayer.errorReceived += M_VideoPlayerOnerrorReceived;
        m_VideoPlayer.prepareCompleted += M_VideoPlayerOnprepareCompleted;

        //if (m_MuseumCanvas == null)
        {
            m_MuseumCanvas = ReferencesForGamePlay.instance.workingCanvas;
        }
    }

    private void OnDisable()
    {
        m_VideoPlayer.errorReceived -= M_VideoPlayerOnerrorReceived;
        m_VideoPlayer.prepareCompleted -= M_VideoPlayerOnprepareCompleted;
    }

    public void SetStaticNFTState()
    {
        foreach (GameObject obj in StaticNFTSpawnPoints)
        {
            obj.SetActive(false);
        }
    }

    private void M_VideoPlayerOnprepareCompleted(VideoPlayer source)
    {
        m_VideoPlayer.gameObject.SetActive(true);
        if (ScreenOrientationManager._instance.isPotrait && cancel_Portrait != null)
            cancel_Portrait.SetActive(true);
        else
            cancel.SetActive(true);

        m_PictureDescriptionPanel.SetActive(true);
        if (certificate != null)
        {
            certificate.SetActive(true);
            certificateTitle.GetComponent<Text>().text = certificateTexts[l_VideoIndex];
        }
        if (!isGOz)
        {
            string l_Sentence = string.Join("", DialogueManager.Instance.m_Dialogues[l_VideoIndex].m_Sentences);
            m_DescriptionText.GetComponent<Text>().text = l_Sentence;
        }

        ActiveVideo();
    }

    private void M_VideoPlayerOnerrorReceived(VideoPlayer source, string message)
    {
        HandleErrors();
    }

    public void HandleErrors()
    {
        StopAllCoroutines();
        errorPanel.SetActive(true);
    }

    public void Retry()
    {
        switch (GalleryImageDetails.CurrentContext)
        {
            case Context.IsBanner:
                LoadBannerImagePanel(m_IsPortrait, m_PictureIndex);
                break;
            case Context.IsVideo:
                LoadVideo(l_VideoIndex);
                break;
            case Context.IsPortrait:
                LoadPictureDescriptionPanel(m_PictureIndex, m_IsSpecialPainting);
                break;
            case Context.IsLandscape:
                LoadPictureDescriptionPanel(m_PictureIndex, m_IsSpecialPainting);
                break;

        }

        errorPanel.SetActive(false);
    }

    void LoadPictureSpriteDownloadLinks()
    {
        string[] links = System.IO.File.ReadAllLines(Application.dataPath + "/MuseumFiles/TextFiles/NewDownloadLinks.txt");
        m_PictureSpriteDownloadLinks = new string[links.Length];

        for (int i = 0; i < links.Length; i++)
        {
            m_PictureSpriteDownloadLinks[i] = links[i];
        }
    }


    // ** On Enable And Disable Picture Description Panel

    public void OnEnablePictureDescriptionPanel()
    {

    }
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            Debug.Log("---GOZ Nft Close due to Application minimized");
            OnDisablePictureDescriptionPanel();
        }

    }
    public void OnDisablePictureDescriptionPanel()
    {
        if (m_IsVideo)
        {
            m_VideoPlayer.targetTexture.Release();
            m_VideoPlayer.targetTexture = null;
            m_VideoPanel.SetActive(false);
            m_IsVideo = false;
        }

        m_IsDescriptionPanelActive = false;
        m_Picture.SetActive(false);
        //m_ExceptionText.SetActive(false);
        m_SpecialCertificate.SetActive(false);
        m_PictureBannerCanvas.SetActive(false);

        print("Waqas : Reference : " + ReferencesForGamePlay.instance + "  " + ReferencesForGamePlay.instance.workingCanvas);

        // Update Museum Panel reference
        m_MuseumCanvas = ReferencesForGamePlay.instance.workingCanvas;
        m_MuseumCanvas.SetActive(true);
        m_PictureDescriptionPanel.SetActive(false);

        cancel.SetActive(false);

        if (cancel_Portrait != null)
            cancel_Portrait.SetActive(false);

        StartCoroutine(museumRaycaster.WaitForCooldown());
    }


    // ** Load Picture Description Panel

    public bool CheckCanClickOnImage()
    {

        foreach (GameObject _panel in overlayPanels)
        {
            if (_panel.activeInHierarchy)
            {
                return false;
            }
        }

        return true;
    }

    public bool CheckIfPicturePanelIsOpen()
    {
        foreach (GameObject _panel in picturePanel)
        {
            if (_panel.activeInHierarchy)
            {
                return false;
            }
        }

        return true;
    }



    public void LoadPictureDescriptionPanel(int l_PictureIndex, bool l_IsSpecialPainting)
    {
        m_PictureIndex = l_PictureIndex;
        m_IsSpecialPainting = l_IsSpecialPainting;

        m_MuseumCanvas = ReferencesForGamePlay.instance.workingCanvas;
        m_MuseumCanvas.SetActive(false);
        m_IsDescriptionPanelActive = true;

        m_PictureBannerCanvas.SetActive(true);
        m_PictureDescriptionPanel.SetActive(true);

        if (m_CertificateSprites.Length > 0)
            m_Certificate.GetComponent<Image>().sprite = m_CertificateSprites[l_PictureIndex - 1];

        if (certificateTitle != null)
        {
            certificate.SetActive(true);
            certificateTitle.GetComponent<Text>().text = certificateTexts[l_PictureIndex];
        }

        if (scenec_cheker)
        {
            string l_Sentence = string.Join("", DialogueManager.Instance.m_Dialogues[l_PictureIndex - 1].m_Sentences);
            m_DescriptionText.GetComponent<Text>().text = l_Sentence;
        }
        else if (!scenec_cheker)
        {
            if (DialogueManager.Instance)
            {
                string l_Sentence = string.Join("", DialogueManager.Instance.m_Dialogues[l_PictureIndex].m_Sentences);
                m_DescriptionText.GetComponent<Text>().text = l_Sentence;
            }

        }


        // ** If it is a special painting

        if (l_IsSpecialPainting)
        {
            print(m_SpecialPaintingIndex[l_PictureIndex].ToString());
            m_SpecialCertificate.SetActive(true);
            m_SpecialCertificate.GetComponent<RawImage>().texture = m_SpecialPaintingTexture[m_SpecialPaintingIndex[l_PictureIndex]];
        }

        if (m_IsLoadingImagesFromServerSide)
        {
            m_IsShowingLoadingBar = true; // showing loading icon
            NFTLoader.SetActive(true);

            //m_LoadingIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(-350, 0);

            StopAllCoroutines();
            //Debug.Log("Picture Index: " + l_PictureIndex);
            if (m_PictureSpriteDownloadLinks[l_PictureIndex - 1] != "")
            {
                StartCoroutine(DownloadImage(m_PictureSpriteDownloadLinks[l_PictureIndex - 1]));
            }
            else
            {
                m_IsShowingLoadingBar = false; // hidding loading icon
                NFTLoader.SetActive(false);
                //m_ExceptionText.SetActive(true);
            }
        }
        else
        {
            m_Picture.SetActive(true);
            if (ScreenOrientationManager._instance.isPotrait && cancel_Portrait != null)
                cancel_Portrait.SetActive(true);
            else
                cancel.SetActive(true);

            m_Picture.GetComponent<Image>().sprite = m_PictureSprites[(l_PictureIndex - 1)];
        }
    }



    public IEnumerator DownloadImage(string l_DownloadAdress)
    {
        Texture2D l_Texture = null;
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(l_DownloadAdress))
        {
            yield return uwr.SendWebRequest();

            while (!uwr.isDone)
            {
                yield return uwr;
            }

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                //m_ExceptionText.SetActive(true);
                //m_ExceptionText.GetComponent<Text>().text = uwr.error;
                HandleErrors();
            }
            else
            {
                l_Texture = ((DownloadHandlerTexture)uwr.downloadHandler).texture;
                m_DownloadedSprite = Sprite.Create(l_Texture, new Rect(0, 0, l_Texture.width, l_Texture.height), new Vector2(l_Texture.width / 2, l_Texture.height / 2));
                m_IsShowingLoadingBar = false;

                uwr.Dispose();
            }
        }
        yield return null;

        m_Picture.SetActive(true);

        if (ScreenOrientationManager._instance.isPotrait && cancel_Portrait != null)
            cancel_Portrait.SetActive(true);
        else
            cancel.SetActive(true);

        m_Picture.GetComponent<Image>().sprite = m_DownloadedSprite; // applying downloaded sprite to the picture object
        NFTLoader.SetActive(false);
    }


    // ** Load Banner Images Panel

    public void LoadBannerImagePanel(bool l_IsPortrait, int l_PictureIndex)
    {
        m_PictureIndex = l_PictureIndex;
        m_IsPortrait = l_IsPortrait;

        // Update Museum Panel reference
        m_MuseumCanvas = ReferencesForGamePlay.instance.workingCanvas;
        m_MuseumCanvas.SetActive(false);

        m_IsDescriptionPanelActive = true;
        m_PictureBannerCanvas.SetActive(true);

        m_BannerPicturePanel.SetActive(true);

        m_IsShowingLoadingBar = true; // showing loading icon
        NFTLoader.SetActive(true);
        NFTLoader.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

        StopAllCoroutines();

        if (m_BannerSpriteDownloadLinks[l_PictureIndex - 1] != "")
            StartCoroutine(DownloadBannerImage(m_BannerSpriteDownloadLinks[l_PictureIndex - 1], l_IsPortrait));
    }


    public IEnumerator DownloadBannerImage(string l_DownloadAdress, bool l_IsPortrait)
    {
        Texture2D l_Texture = null;
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(l_DownloadAdress))
        {
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                //m_ExceptionText.SetActive(true);
                m_ExceptionText.GetComponent<Text>().text = uwr.error;
                //Debug.Log(uwr.error);
                HandleErrors();
            }
            else
            {
                l_Texture = ((DownloadHandlerTexture)uwr.downloadHandler).texture;
                m_DownloadedSprite = Sprite.Create(l_Texture, new Rect(0, 0, l_Texture.width, l_Texture.height), new Vector2(l_Texture.width / 2, l_Texture.height / 2));
                m_IsShowingLoadingBar = false;
                uwr.Dispose();
            }
        }
        yield return null;

        if (l_IsPortrait)
        {
            m_VerticalBanner.SetActive(true);
            //m_VerticalBanner.GetComponent<Image>().sprite = m_BannerSprites[l_PictureIndex];
            m_VerticalBanner.GetComponent<Image>().sprite = m_DownloadedSprite;
        }
        else
        {
            m_HorizontalBanner.SetActive(true);
            //m_HorizontalBanner.GetComponent<Image>().sprite = m_BannerSprites[l_PictureIndex];
            m_HorizontalBanner.GetComponent<Image>().sprite = m_DownloadedSprite;
        }

        //m_Picture.SetActive(true);
        //m_Picture.GetComponent<Image>().sprite = m_DownloadedSprite; // applying downloaded sprite to the picture object
        NFTLoader.SetActive(false);
    }


    public void CloseBannerImagePanel()
    {
        m_BannerPicturePanel.SetActive(false);
        m_HorizontalBanner.SetActive(false);
        m_VerticalBanner.SetActive(false);
        m_PictureBannerCanvas.SetActive(false);
        m_IsDescriptionPanelActive = false;

        print("Waqas : Reference : " + ReferencesForGamePlay.instance);
        // Update Museum Panel reference
        m_MuseumCanvas = ReferencesForGamePlay.instance.workingCanvas;
        m_MuseumCanvas.SetActive(true);

        certificate.SetActive(false);
        StartCoroutine(museumRaycaster.WaitForCooldown());

        if (m_RenderTexture != null)
            m_RenderTexture.Release();
    }


    // ** Load Judge And Mayor Frame

    public void LoadMayorAndJudgeFrame(int l_FrameIndex)
    {
        // Update Museum Panel reference
        m_MuseumCanvas = ReferencesForGamePlay.instance.workingCanvas;
        m_MuseumCanvas.SetActive(false);

        m_IsDescriptionPanelActive = true;
        m_PictureBannerCanvas.SetActive(true);
        m_BannerPicturePanel.SetActive(true);
        m_VerticalBanner.SetActive(true);
        m_VerticalBanner.GetComponent<Image>().sprite = m_MayorJudgeFrameSprites[l_FrameIndex];
    }


    public void LoadVideo(int l_VideoIndex)
    {
        this.l_VideoIndex = l_VideoIndex;
        m_VideoPlayer.targetTexture = m_RenderTexture;
        m_IsShowingLoadingBar = true;
        NFTLoader.SetActive(true);
        m_VideoPlayer.url = m_PictureSpriteDownloadLinks[l_VideoIndex];
        m_VideoPlayer.gameObject.SetActive(true);
        m_PictureBannerCanvas.SetActive(true);
        m_PictureDescriptionPanel.SetActive(true);
        m_VideoPlayer.Prepare();

        // Update References For for switching Orientation
        m_MuseumCanvas = ReferencesForGamePlay.instance.workingCanvas;
        m_MuseumCanvas.SetActive(false);
    }

    void ActiveVideo()
    {
        NFTLoader.SetActive(false);
        m_IsShowingLoadingBar = false;
        m_VideoPanel.SetActive(true);

    }


}