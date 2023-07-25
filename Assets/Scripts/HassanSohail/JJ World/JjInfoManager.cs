using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

public class JjInfoManager : MonoBehaviour
{
    [SerializeField] bool IsJjWorld;
    [NonReorderable]
    public List<RatioReferences> ratioReferences;
    [NonReorderable]
    [SerializeField] List<VideoPlayer> VideoPlayers;
    [SerializeField] GameObject LandscapeObj;
    [SerializeField] GameObject PotraiteObj;

    [NonReorderable]
    public List<JJWorldInfo> worldInfos;

    [NonReorderable]
    public List<GameObject> NftPlaceholder;
    public static JjInfoManager Instance { get; private set; }
    
    [SerializeField] RenderTexture renderTexture;
    [SerializeField] int RetryChances = 3;
    [SerializeField] int JJMusuemId;
    int ratioId;
    int videoRetry=0;

    JjRatio _Ratio;
    string _Title;
    string _Aurthor;
    string _Des;
    Sprite _image;
    DataType _Type;
    string _VideoLink;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnEnable()
    {
        foreach (VideoPlayer player in VideoPlayers)
        {
            player.errorReceived += ErrorOnVideo;
            player.prepareCompleted += VideoReady;
        }
    }

    private void Start()
    {
        if (IsJjWorld)
        {
            IntJjInfoManager();
        }
    }

    /// <summary>
    /// It will clear the worldInfos list and Set infos
    /// </summary>
    public async void IntJjInfoManager()
    {
        //worldInfos.Clear();
        //for (int i = 0; i < NftPlaceholder.Count; i++)
        //{
        //    worldInfos.Add(null);
        //}
        StringBuilder apiUrl = new StringBuilder();
        apiUrl.Append(ConstantsGod.API_BASEURL + ConstantsGod.JJWORLDASSET+JJMusuemId);

        try
        {
             using (UnityWebRequest request = UnityWebRequest.Get(apiUrl.ToString()) ){ 
                request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
                await request.SendWebRequest();
                if (request.isNetworkError || request.isHttpError)
                {
                    Debug.Log("<color=red>" + request.error + " </color>");
                }
                else
                {
                    //Debug.Log("Get UnReadMessagesCount Success!");
                    StringBuilder data = new StringBuilder();
                    data.Append(request.downloadHandler.text);
                    Debug.Log("JJ World Req" + data.ToString());
                    JjJson json = JsonConvert.DeserializeObject<JjJson>(data.ToString());
                    InitData(json);

                }
            }

        }
        catch
        {
            Debug.Log("<color=red>jj APi not call in "+ XanaConstants.xanaConstants.EnviornmentName+"</color>");
        }
        //finally
        //{

        //}

    }


    void InitData(JjJson data)
    {
       
        int nftPlaceHolder = NftPlaceholder.Count;
        List<JjAsset> worldData = data.data;
        for (int i = 0; i < nftPlaceHolder; i++)
        {
            //worldInfos.Add(null);
            //Sprite tempSprite;
            bool isWithDes=false; 
            string compersionPrfex = "";
            if (/*worldData[i].check*/ true)
            {
                switch (worldData[i].ratio)
                {
                    case "1:1":
                        compersionPrfex = "?width=512&height=512";
                        break;
                    case "16:9":
                        compersionPrfex = "?width=500&height=600";
                        break;
                    case "9:16":
                        compersionPrfex = "?width=700&height=500";
                        break;
                    case "4:3":
                        //compersionPrfex = "?width=640&height=480";
                         compersionPrfex = "?width=512&height=512";
                        break;
                    default:
                        compersionPrfex = "?width=512&height=512";
                        break;
                }
                StartCoroutine(GetSprite(data.data[i].asset_link + compersionPrfex, i, (response, Index) =>
              {
                  print("Response is " + response);
                //tempSprite = response;

                // Setting NFT type (Image / Video)
                if (worldData[Index].media_type.Contains("IMAGE"))
                  {
                      print("in image if " + Index);
                      NftPlaceholder[Index].GetComponent<SpriteRenderer>().sprite = response;
                      if (NftPlaceholder[Index].GetComponent<BoxCollider>())
                      {
                         Destroy( NftPlaceholder[Index].GetComponent<BoxCollider>() );
                      }
                      NftPlaceholder[Index].AddComponent<BoxCollider>();
                    //worldInfos[Index].Type = DataType.Image;
                    worldInfos[Index].WorldImage = response;
                  }
                  else
                  {
                    // Video
                    worldInfos[Index].Type = DataType.Video;
                      if (worldData[Index].youtubeUrlCheck)
                      {
                          worldInfos[Index].VideoLink = worldData[Index].youtubeUrl;
                      }
                      else
                      {
                          worldInfos[Index].VideoLink = worldData[Index].asset_link;
                      }
                  }
                // Setting NFT type Content (Des/ WithoutDes)
                if (!string.IsNullOrEmpty(worldData[Index].title[0]) && !string.IsNullOrEmpty(worldData[Index].authorName[0]) && !string.IsNullOrEmpty(worldData[Index].description[0]))
                  {
                      isWithDes = true;
                      worldInfos[Index].Title = worldData[Index].title;
                      worldInfos[Index].Aurthor = worldData[Index].authorName;
                      worldInfos[Index].Des = worldData[Index].description;
                      switch (worldData[Index].ratio)
                      {
                          case "1:1":
                              worldInfos[Index].JjRatio = JjRatio.OneXOneWithDes;
                              break;
                          case "16:9":
                              worldInfos[Index].JjRatio = JjRatio.SixteenXNineWithDes;
                              break;
                          case "9:16":
                              worldInfos[Index].JjRatio = JjRatio.NineXSixteenWithDes;
                              break;
                          case "4:3":
                              worldInfos[Index].JjRatio = JjRatio.FourXThreeWithDes;
                              break;
                          default:
                              worldInfos[Index].JjRatio = JjRatio.OneXOneWithDes;
                              break;
                      }
                  }
                  else
                  {
                      isWithDes = false;
                      worldInfos[Index].Title = null;
                      worldInfos[Index].Aurthor = null;
                      worldInfos[Index].Des = null;
                      switch (worldData[Index].ratio)
                      {
                          case "1:1":
                              worldInfos[Index].JjRatio = JjRatio.OneXOneWithoutDes;
                              break;
                          case "16:9":
                              worldInfos[Index].JjRatio = JjRatio.SixteenXNineWithoutDes;
                              break;
                          case "9:16":
                              worldInfos[Index].JjRatio = JjRatio.NineXSixteenWithoutDes;
                              break;
                          case "4:3":
                              worldInfos[Index].JjRatio = JjRatio.FourXThreeWithoutDes;
                              break;
                          default:
                              worldInfos[Index].JjRatio = JjRatio.OneXOneWithoutDes;
                              break;
                      }
                  }
              }));
            }
            else
            {
                NftPlaceholder[i].SetActive(false);
            }
        }

    }

    IEnumerator GetSprite(string path , int index, System.Action<Sprite, int> callback) {
        while (Application.internetReachability == NetworkReachability.NotReachable)
        {
            yield return new WaitForEndOfFrame();
            print("Internet Not Reachable");
        }

        using ( UnityWebRequest www = UnityWebRequestTexture.GetTexture(path))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success) {
                Debug.Log("ERror in loading sprite"+www.error);
            }
            else {
                 if (www.isDone)
                 {
                    Texture2D loadedTexture = DownloadHandlerTexture.GetContent(www);
                    var rect = new Rect(1, 1, 1, 1);
                    //thunbNailImage = Sprite.Create(loadedTexture, new Rect(0f, 0f, loadedTexture.width, loadedTexture.height), new Vector2(.5f, 0f));
                    var tempTex = ((DownloadHandlerTexture)www.downloadHandler).texture;
                    Sprite sprite = /*Sprite.Create(tempTex, rect, new Vector2(0.5f, 0.5f))*/ Sprite.Create(loadedTexture, new Rect(0f, 0f, loadedTexture.width, loadedTexture.height), new Vector2(.5f, 0f));
                    print("Texture is "+sprite);
                    callback(sprite, index);
                 }
            }
        }
    }

    //Sprite DownloadSprite( string path)
    //{
    //    UnityWebRequest www = UnityWebRequest.Get(path);
    //    yield return www.WaitForCompletion();

    //     if (www.IsSuccess) {
    //        Texture2D texture = www.GetTexture();
    //        spriteImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
    //    }
    //}

    public void SetInfo(JjRatio ratio, string title, string aurthur, string des, Sprite image, DataType type, string videoLink)
    {
        _Ratio = ratio;
        _Title = title;
        _Aurthor = aurthur;
        _Des = des;
        _image = image;
        _Type = type;
        _VideoLink = videoLink;

        ratioId = ((int)ratio);
        renderTexture.Release();
        // Setting Landscape Data
        ratioReferences[ratioId].l_image.gameObject.SetActive(true);
        ratioReferences[ratioId].p_image.gameObject.SetActive(true);
        ratioReferences[ratioId].p_videoPlayer.gameObject.SetActive(true);
        ratioReferences[ratioId].l_videoPlayer.gameObject.SetActive(true);
        ratioReferences[ratioId].l_Title.text = title;
        ratioReferences[ratioId].l_Aurthur.text = aurthur;
        ratioReferences[ratioId].l_Description.text = des;
        if (type == DataType.Image)
        {
            ratioReferences[ratioId].l_image.sprite = image;
            ratioReferences[ratioId].l_videoPlayer.gameObject.SetActive(false);
        }
        else
        {
            ratioReferences[ratioId].l_image.gameObject.SetActive(false);
            ratioReferences[ratioId].l_videoPlayer.url = videoLink;
        }

        // Setting Potraite Data
        ratioReferences[ratioId].p_Title.text = title;
        ratioReferences[ratioId].p_Aurthur.text = aurthur;
        ratioReferences[ratioId].p_Description.text = des;
        ratioReferences[ratioId].p_image.sprite = image;
        if (type == DataType.Image)
        {
            ratioReferences[ratioId].p_image.sprite = image;
            ratioReferences[ratioId].p_videoPlayer.gameObject.SetActive(false);
        }
        else
        {
            ratioReferences[ratioId].p_image.gameObject.SetActive(false);
            ratioReferences[ratioId].p_videoPlayer.url = videoLink;
        }
        if (!ChangeOrientation_waqas._instance.isPotrait) // for Landscape
        {
            LandscapeObj.SetActive(true);
            PotraiteObj.SetActive(false);
            ratioReferences[ratioId].l_obj.SetActive(true);
            ratioReferences[ratioId].p_obj.SetActive(false);
            if (type == DataType.Video)
            {
                ratioReferences[ratioId].l_Loader.SetActive(true);
                ratioReferences[ratioId].p_Loader.SetActive(false);
                ratioReferences[ratioId].l_videoPlayer.Play();
            }
        }
        else // for Potraite
        {
            LandscapeObj.SetActive(false);
            PotraiteObj.SetActive(true);
            ratioReferences[ratioId].l_obj.SetActive(false);
            ratioReferences[ratioId].p_obj.SetActive(true);
            if (type == DataType.Video)
            {
                ratioReferences[ratioId].l_Loader.SetActive(false);
                ratioReferences[ratioId].p_Loader.SetActive(true);
                ratioReferences[ratioId].p_videoPlayer.Play();
            }
        }
        if (CanvasButtonsHandler.inst.gameObject.activeInHierarchy)
        {
            CanvasButtonsHandler.inst.gamePlayUIParent.SetActive(false);
        }
        // infoParent.SetActive(true);
    }

    public void CloseInfoPop()
    {
        ratioReferences[ratioId].l_obj.SetActive(false);
        ratioReferences[ratioId].p_obj.SetActive(false);
        ratioReferences[ratioId].p_Loader.SetActive(false);
        ratioReferences[ratioId].l_Loader.SetActive(false); 
        LandscapeObj.SetActive(false);
        PotraiteObj.SetActive(false);
        if (CanvasButtonsHandler.inst.gameObject.activeInHierarchy)
        {
             CanvasButtonsHandler.inst.gamePlayUIParent.SetActive(true);
        }
      
    }

    private void ErrorOnVideo(VideoPlayer source, string message)
    {
        if (videoRetry <= RetryChances)
        {
            videoRetry++;
            SetInfo(_Ratio,_Title,_Aurthor,_Des,_image,_Type,_VideoLink);
        }
        else
        {
            videoRetry = 0;
            CloseInfoPop();
        }
    }
    private void VideoReady(VideoPlayer source)
    {
        ratioReferences[ratioId].p_Loader.SetActive(false);
        ratioReferences[ratioId].l_Loader.SetActive(false);
        videoRetry = 0;
    }
    private void OnDisable()
    {
        foreach (VideoPlayer player in VideoPlayers)
        {
            player.errorReceived -= ErrorOnVideo;
            player.prepareCompleted -= VideoReady;
        }
    }
}

[Serializable]
public class JJWorldInfo {
    public string[] Title;
    public string[] Aurthor;
    public string[] Des;
    public DataType Type;
    public Sprite WorldImage;
    public string VideoLink;
    public JjRatio JjRatio;
}

public enum DataType { 
    Image,
    Video
}

public enum JjRatio { 
    OneXOneWithDes,
    SixteenXNineWithDes,
    NineXSixteenWithDes,
    FourXThreeWithDes,

    OneXOneWithoutDes,
    SixteenXNineWithoutDes,
    NineXSixteenWithoutDes,
    FourXThreeWithoutDes,
    //OneXOneWithDesPotraite,
    //SixteenXNineWithDesPotraite,
    //NineXSixteenWithDesPotraite,
    //FourXThreeWithDesPotraite,
    //OneXOneWithoutDesPotraite,
    //SixteenXNineWithoutDesPotraite,
    //NineXSixteenWithoutDesPotraite,
    //FourXThreeWithoutDesPotraite,
}

[Serializable]
public class RatioReferences
{
    public string name;

    public GameObject l_obj;
    public TMP_Text l_Title;
    public TMP_Text l_Aurthur;
    public TMP_Text l_Description;
    public Image l_image;
    public VideoPlayer l_videoPlayer;
    public GameObject l_Loader;

    public GameObject p_obj;
    public TMP_Text p_Title;
    public TMP_Text p_Aurthur;
    public TMP_Text p_Description;
    public Image p_image;
    public VideoPlayer p_videoPlayer;
    public GameObject p_Loader;
}


public class JjJson{ 
    public bool success;
    public List<JjAsset> data;
    public string msg;
}

public class JjAsset{ 
    public int id;
    public int museumId;
    public int index;
    public string asset_link;
    public bool check;
    public string[] authorName;
    public string[] description;
    public string[] title;
    public string ratio;
    public string thumbnail;
    public string media_type;
    public string env_class;
    public string user_id;
    public string event_id;
    public string category;
    public bool youtubeUrlCheck;
    public string youtubeUrl;
    public DateTime createdAt;
    public DateTime updatedAt;
    public string event_env_class;
}