using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

public class XLWorldInfo : MonoBehaviour
{
    public int id;

    private Texture2D _texture;

    //public GameObject imgVideo16x9;
    //public GameObject imgVideo9x16;
    public GameObject imgVideo1x1;
    //public GameObject imgVideo4x3;
    public string imageLink;
    public JjRatio _imgVideoRatio;
    public JjWorldChanger worldChanger;
    // Start is called before the first frame update
    void Start()
    {
        imgVideo1x1.AddComponent<Button>();
        imgVideo1x1.GetComponent<Button>().onClick.AddListener(() => OpenWorldInfo());
    }

    private void OnDisable()
    {
        if (imgVideo1x1.GetComponent<RawImage>().texture != null)
        {
            DestroyImmediate(imgVideo1x1.GetComponent<RawImage>().texture, true);
            imgVideo1x1.GetComponent<RawImage>().texture = null;
        }
    }
    public void InitData(int index,string imageurl, JjRatio imgvideores, MediaType dataType)
    {
        id = index;
        imageLink = imageurl;
        _imgVideoRatio = imgvideores;
        if (dataType == MediaType.Image)
        {
            SetImage();
        }
    } 
    void SetImage()
    {
        if (imgVideo1x1)
            imgVideo1x1.SetActive(false);
        StartCoroutine(GetSprite(imageLink, (response) =>
        {
            
            if (_imgVideoRatio == JjRatio.OneXOneWithDes || _imgVideoRatio == JjRatio.OneXOneWithoutDes)
            {
                if (imgVideo1x1)
                {
                    //if (imgVideoFrame1x1)
                    //{
                    //    EnableImageVideoFrame(imgVideoFrame1x1);
                    //}
                    imgVideo1x1.SetActive(true);
                    imgVideo1x1.GetComponent<RawImage>().texture = response;
                    imgVideo1x1.GetComponent<VideoPlayer>().enabled = false;
                    if (imgVideo1x1.transform.childCount > 0)
                    {
                        foreach (Transform g in imgVideo1x1.transform)
                        {
                            g.gameObject.GetComponent<RawImage>().texture = response;
                            g.gameObject.SetActive(true);
                        }
                    }
                }
            }

        }));
    }
    IEnumerator GetSprite(string path, System.Action<Texture> callback)
    {
        while (Application.internetReachability == NetworkReachability.NotReachable)
        {
            yield return new WaitForEndOfFrame();
            print("Internet Not Reachable");
        }

        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(path))
        {
            www.SendWebRequest();
            while (!www.isDone)
            {
                yield return null;
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("ERror in loading sprite" + www.error);
            }
            else
            {
                if (www.isDone)
                {
                    Texture2D loadedTexture = DownloadHandlerTexture.GetContent(www);
                    _texture = loadedTexture;
                    //var rect = new Rect(1, 1, 1, 1);
                    //thunbNailImage = Sprite.Create(loadedTexture, new Rect(0f, 0f, loadedTexture.width, loadedTexture.height), new Vector2(.5f, 0f));
                    //Texture2D tempTex = ((DownloadHandlerTexture)www.downloadHandler).texture;
                    //Sprite sprite = /*Sprite.Create(tempTex, rect, new Vector2(0.5f, 0.5f))*/ Sprite.Create(loadedTexture, new Rect(0f, 0f, loadedTexture.width, loadedTexture.height), new Vector2(.5f, 0f));
                    //print("Texture is " + sprite);
                    callback(_texture);
                }
            }
            www.Dispose();
        }
    }
    public void OpenWorldInfo()
    {
        if (PlayerSelfieController.Instance.m_IsSelfieFeatureActive) return;

        //Reset joystick when OpenWorldInfo
        if (ReferencesForGamePlay.instance.playerControllerNew)
            ReferencesForGamePlay.instance.playerControllerNew.restJoyStick();

        //JjInfoManager.Instance.firebaseEventName = firebaseEventName;
        if (XanaLobbyManager.Instance != null)
        {
            if (GameManager.currentLanguage.Contains("en") && !LocalizationManager.forceJapanese)
            {
                XanaLobbyManager.Instance.SetInfo(JjRatio.OneXOneWithDes, XanaLobbyManager.Instance.worldsData[id].world_name, XanaLobbyManager.Instance.worldsData[id].users.name, XanaLobbyManager.Instance.worldsData[id].description, _texture,MediaType.Image);
            }
            else if (LocalizationManager.forceJapanese || GameManager.currentLanguage.Equals("ja"))
            {
                XanaLobbyManager.Instance.SetInfo(JjRatio.OneXOneWithDes, XanaLobbyManager.Instance.worldsData[id].world_name, XanaLobbyManager.Instance.worldsData[id].users.name, XanaLobbyManager.Instance.worldsData[id].description, _texture, MediaType.Image);
            }
        }
    }
}
