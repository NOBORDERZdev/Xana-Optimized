using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.Video;
using XanaNFT;

public class MetaDataInPrefab : MonoBehaviour
{
    public MetaData metaData;
    public TokenDetails tokenDetails;
    public CreatorDetails creatorDetails;
    public string nftLink;
    public string creatorLink;
    public Sprite thunbNailImage;
    public GameObject spriteObject;

    //------------------------------------
    //For videos
    public VideoClip videoClip;
    public SpriteRenderer spriteRenderer;
    public VideoPlayer videoPlayer;

    public bool isVideo = false;

    // Start is called before the first frame update
    public RaycastHit hit;
    public Camera playerCamera;

    public float rayDistance = 4f;

    public GameObject frame;
    Material mat;
    public bool isDynamicMuseum = false;

    // private int layerMask;

    //private bool canOpenPicture = true;

    public PlayerController playerControllerNew;
    /// <summary>
    /// variables for gif 
    /// </summary>
    private List<Texture2D> mFrames = new List<Texture2D>();
    private List<float> mFrameDelay = new List<float>();

    private int mCurFrame = 0;
    private float mTime = 0.0f;
    private Sprite gifTexture;
    private bool isGif;
    [HideInInspector()]
    public bool isVisible;
    private void Start()
    {
        mat = (Material)Resources.Load("FramMaterial");
        playerControllerNew = ReferencesForGamePlay.instance.MainPlayerParent.GetComponent<PlayerController>();
        if (playerCamera == null)
        {
            playerCamera = ReferencesForGamePlay.instance.randerCamera;
        }
    }
    public void StartNow()
    {
        frame = this.transform.GetChild(0).gameObject;
        frame.SetActive(false);
        //print("StartNow");
        if (metaData.thumbnft != null)
            StartCoroutine(DownloadImageAndShow());
    }

    private IEnumerator DownloadImageAndShow()
    {
        while (Application.internetReachability == NetworkReachability.NotReachable)
        {
            yield return new WaitForEndOfFrame();
            print("Internet Not Reachable");
        }
        if (metaData.image.EndsWith(".mp4") || metaData.image.EndsWith(".MOV"))
        {
            using (UnityWebRequest request = UnityWebRequest.Get(metaData.image))//"?tr=w-400,tr=h-400"
            {
                //print("request");
                //request.SetRequestHeader("Authorization", PlayerPrefs.GetString("GuestToken"));
                //print("Sending Web Request");
                request.SendWebRequest();
                while (!request.isDone)
                {
                    yield return null;
                }
                //print("Web Request completed");
                //thunbNailImage = new Sprite();
                yield return new WaitForEndOfFrame();
                //print(nftDetails.data[0].tokenDetails.zh_nft_description);
                if (!request.isHttpError && !request.isNetworkError)
                {
                    if (request.error == null)
                    {
                     

                        //Texture2D loadedTexture = DownloadHandlerTexture.GetContent(request);
                        string[] names = metaData.thumbnft.Split('/');
                        string fileName = names[names.Length - 1];
                        print(fileName);
                        print(Application.persistentDataPath);
                        File.WriteAllBytes(Application.persistentDataPath + "/" + fileName, request.downloadHandler.data);
                        spriteObject = new GameObject();
                        spriteObject.name = "SpriteObject";
                        videoPlayer = spriteObject.AddComponent<VideoPlayer>();
                        videoPlayer.source = VideoSource.Url;
                        string videoURL = "";
                        if (metaData.image == "https://ik.imagekit.io/xanalia/award/1631447513792.mp4")
                        {
                            videoURL = "https://cdn.xana.net/xanaprod/Defaults/1647964402571_1631447513792.mp4";
                        }
                        else if (metaData.image == "https://ik.imagekit.io/xanalia/award/1640776522185.mp4")
                        {
                            videoURL = "https://cdn.xana.net/xanaprod/Defaults/1647964503763_1640776522185.mp4";
                        }
                        else if (metaData.image == "https://ik.imagekit.io/xanalia/award/1634037496490.mp4")
                        {
                            videoURL = "https://cdn.xana.net/xanaprod/Defaults/1647964429481_1634037496490.mp4";
                        }
                        else if (metaData.image == "https://ik.imagekit.io/xanalia/award/1640866737197.mp4")
                        {
                            videoURL = "https://cdn.xana.net/xanaprod/Defaults/1647964526933_1640866737197.mp4";
                        }
                        else if (metaData.image == "https://ik.imagekit.io/xanalia/award/1639885977430.MOV")
                        {
                            videoURL = "https://cdn.xana.net/xanaprod/Defaults/1647964452260_1639885977430.mp4";
                        }
                        else if (metaData.image == "https://ik.imagekit.io/xanalia/award/1640323809898.mp4")
                        {
                            videoURL = "https://cdn.xana.net/xanaprod/Defaults/1647964476252_1640323809898.mp4";
                        }
                        else if (metaData.image == "https://ik.imagekit.io/xanalia/award/1640961826804.mp4")
                        {
                            videoURL = "https://cdn.xana.net/xanaprod/Defaults/1647964549464_1640961826804.mp4";
                        }
                        else if (metaData.image == "https://ik.imagekit.io/xanalia/award/1636461565419.mp4")
                        {
                            videoURL = "https://cdn.xana.net/xanaprod/Defaults/1648014631946_1636461565419.mp4";
                        }
                        else
                        {
                            videoURL = metaData.image;
                        }
                        videoPlayer.url = videoURL;
                        videoPlayer.SetDirectAudioMute(0, true);// = VideoAudioOutputMode.None;
                        while (!videoPlayer.isPrepared)
                        {
                            yield return new WaitForEndOfFrame();
                        }
#if UNITY_IOS
                        print(videoPlayer.texture.width + " " + videoPlayer.texture.height);
#elif UNITY_ANDROID
                        print(videoPlayer.width + " " + videoPlayer.height);
#endif
                        //////////////////////////////////////////////////

                        ///////////////////////////////////////////////
                        spriteObject.AddComponent<SpriteRenderer>();
#if UNITY_IOS
                        thunbNailImage = Sprite.Create(new Texture2D((int)videoPlayer.texture.height, (int)videoPlayer.texture.width), new Rect(0f, 0f, (int)videoPlayer.texture.height, (int)videoPlayer.texture.width), new Vector2(0.5f, 0f));
#elif UNITY_ANDROID
                        thunbNailImage = Sprite.Create(new Texture2D((int)videoPlayer.height, (int)videoPlayer.width), new Rect(0f, 0f, (int)videoPlayer.height, (int)videoPlayer.width), new Vector2(0.5f, 0f));
#endif
                        spriteObject.transform.position = this.transform.position;
                        spriteObject.transform.rotation = this.transform.rotation;
                        videoPlayer.renderMode = VideoRenderMode.MaterialOverride;
                        videoPlayer.isLooping = true;
                        videoPlayer.targetMaterialRenderer = spriteObject.GetComponent<SpriteRenderer>();
                        spriteObject.GetComponent<SpriteRenderer>().sprite = thunbNailImage;
                        spriteObject.GetComponent<SpriteRenderer>().flipX = true;
                        spriteObject.transform.parent = this.transform;
                        if (spriteObject.transform.parent.name.Contains("25") || spriteObject.transform.parent.name.Contains("26"))
                        {
                            spriteObject.transform.localPosition = new Vector3(0, 0, .07f);
                        }
                        else
                        {
                            spriteObject.transform.localPosition = new Vector3(0, 0, .01f);
                        }

#if UNITY_IOS
                        if ((videoPlayer.texture.width * 1f) / (videoPlayer.texture.height * 1f) > 1.778f)
                        {
                            spriteObject.transform.localScale = new Vector3(1.8f / (videoPlayer.texture.width / 100f), 1.87f / (videoPlayer.texture.height / 100f), 1);
                        }
                        else
                        {
                            spriteObject.transform.localScale = new Vector3(1.8f / (videoPlayer.texture.height / 100f), 1.8f / (videoPlayer.texture.width / 100f), 1);
                        }
                        print(videoPlayer.texture.width + " " + videoPlayer.texture.height);
#elif UNITY_ANDROID
                        if ((videoPlayer.width * 1f) / (videoPlayer.height * 1f) > 1.778f)
                        {
                            spriteObject.transform.localScale = new Vector3(1.8f / (videoPlayer.width / 100f), 1.8f / (videoPlayer.height / 100f), 1);
                        }
                        else
                        {
                            spriteObject.transform.localScale = new Vector3(1.8f / (videoPlayer.height / 100f), 1.8f / (videoPlayer.width / 100f), 1);
                        }
                        print(videoPlayer.width + " " + videoPlayer.height);
#endif
                        spriteObject.transform.Rotate(new Vector3(0, 0, 0));

                        NFTFromServer.RemoveOne();
                        this.gameObject.AddComponent<UnityEngine.BoxCollider>().center = new Vector3(0, .75f, 0);
                        this.gameObject.GetComponent<UnityEngine.BoxCollider>().size = new Vector3(2f, 1.65f, 0.5f);
                        isVideo = true;

                        CreateFrame();

                        // Move into a function By WaqasAhmad
                        {
                            //Vector3[] abc = GetSpriteCorners(spriteObject.GetComponent<SpriteRenderer>());

                            //if ((this.transform.localEulerAngles.y > 45 && this.transform.localEulerAngles.y < 105) || (this.transform.localEulerAngles.y > 235 && this.transform.localEulerAngles.y < 315))
                            //{
                            //    //print(abc[0] + " " + abc[1] + " " + abc[2] + " " + abc[3]);
                            //    abc[0].x = abc[1].x = abc[2].x = abc[3].x;
                            //}
                            //else
                            //{
                            //    abc[1].x = abc[0].x;
                            //    abc[1].z = abc[0].z;

                            //    abc[2].y = abc[1].y;
                            //    abc[2].z = abc[1].z;

                            //    abc[3].x = abc[2].x;
                            //    abc[3].z = abc[2].z;
                            //}
                            //for (int i = 0; i < abc.Length; i++)
                            //{
                            //    GameObject temp1 = new GameObject();
                            //    temp1.name = "upSide";
                            //    GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            //    temp.GetComponent<MeshRenderer>().material = mat; //Resources.Load("FrameMaterial");
                            //    temp.transform.parent = temp1.transform;
                            //    //temp.GetComponent<BoxCollider>().enabled = false;
                            //    Destroy(temp.GetComponent<BoxCollider>());
                            //    temp.transform.position = new Vector3(temp1.transform.position.x, temp1.transform.position.y, temp1.transform.position.z + .5f);
                            //    temp1.transform.position = abc[i];
                            //    temp1.transform.parent = this.transform;
                            //    temp1.transform.localScale = new Vector3(.1f, .1f, 0f);// this.transform.localScale;
                            //    float distance;
                            //    if (i == abc.Length - 1)
                            //    {
                            //        temp1.transform.LookAt(abc[0]);
                            //        distance = Math.Abs(Vector3.Distance(abc[i], abc[0]));
                            //    }
                            //    else
                            //    {
                            //        temp1.transform.LookAt(abc[i + 1]);
                            //        distance = Math.Abs(Vector3.Distance(abc[i], abc[i + 1]));
                            //    }
                            //    temp1.transform.localScale = new Vector3(.1f, .1f, (distance * 2) + .1f);
                            //    temp1.transform.localPosition = new Vector3(temp1.transform.localPosition.x, temp1.transform.localPosition.y, 0);
                            //}
                        }
                     
                    }
                    Invoke(nameof(PauseVideo), 2);
                }
                else
                {
                    if (request.isNetworkError)
                    {
                        yield return StartCoroutine(DownloadImageAndShow());
                        print("Network Error");
                    }
                    else
                    {
                        yield return StartCoroutine(DownloadImageAndShow());
                        if (request.error != null)
                        {
                        }
                    }
                }
                request.Dispose();
            }
        }
        else if (metaData.thumbnft.EndsWith(".gif"))
        {
            using (UnityWebRequest request = UnityWebRequest.Get(metaData.thumbnft + "?tr=w-400,tr=h-400"))
            {
                request.SendWebRequest();
                while (!request.isDone)
                {
                    yield return null;
                }
                yield return new WaitForEndOfFrame();
                if (!request.isHttpError && !request.isNetworkError)
                {
                    if (request.error == null)
                    {
                        Texture2D loadedTexture = null; //= ((DownloadHandlerTexture)request.downloadHandler).texture;
                        byte[] imageData = request.downloadHandler.data;


                        using (var decoder = new MG.GIF.Decoder(imageData))
                        {
                            var img = decoder.NextImage();
                            loadedTexture = img.CreateTexture();
                            //while (img != null)
                            //{
                            //    yield return null;
                            //    mFrames.Add(img.CreateTexture());
                            //    mFrameDelay.Add(img.Delay / 1000.0f);
                            //    img = decoder.NextImage();
                            //}
                        }


                        thunbNailImage = Sprite.Create(loadedTexture, new Rect(0f, 0f, loadedTexture.width, loadedTexture.height), new Vector2(.5f, 0f));
                        spriteObject = new GameObject();
                        spriteObject.name = "SpriteObject";
                        //Instantiate(new GameObject("SpriteObject"), this.transform.position, this.transform.rotation, this.transform);
                        spriteObject.transform.parent = this.transform;
                        spriteObject.transform.position = this.transform.position;
                        spriteObject.transform.rotation = this.transform.rotation;
                        if (spriteObject.transform.parent.name.Contains("25") || spriteObject.transform.parent.name.Contains("26"))
                        {
                            spriteObject.transform.localPosition = new Vector3(0, 0, .07f);
                        }
                        else
                        {
                            spriteObject.transform.localPosition = new Vector3(0, 0, .01f);
                        }

                        spriteObject.AddComponent<SpriteRenderer>().sprite = thunbNailImage;//Sprite.Create(loadedTexture, new Rect(0f, 0f, loadedTexture.width, loadedTexture.height), new Vector2(.5f, 0f));
                        //StartCoroutine(forcePosFix(spriteObject));
                        spriteObject.GetComponent<SpriteRenderer>().flipX = true;
                        if ((thunbNailImage.texture.width * 1f) > (thunbNailImage.texture.height * 1f))
                        {
                            spriteObject.transform.localScale = new Vector3(2.4f / (thunbNailImage.texture.width / 100f), 2.4f / (thunbNailImage.texture.width / 100f), 1);
                        }
                        else if ((thunbNailImage.texture.width * 1f) < (thunbNailImage.texture.height * 1f))
                        {
                            spriteObject.transform.localScale = new Vector3(1.8f / (thunbNailImage.texture.height / 100f), 1.8f / (thunbNailImage.texture.height / 100f), 1);
                        }
                        else
                        {
                            spriteObject.transform.localScale = new Vector3(1.8f / (thunbNailImage.texture.height / 100f), 1.8f / (thunbNailImage.texture.height / 100f), 1);
                        }
                        NFTFromServer.RemoveOne();
                        this.gameObject.AddComponent<UnityEngine.BoxCollider>().center = new Vector3(0, .75f, 0);
                        this.gameObject.GetComponent<UnityEngine.BoxCollider>().size = new Vector3(2f, 1.65f, 0.5f);
                        isVideo = false;

                        CreateFrame();
                        // Move into a function By WaqasAhmad
                        {
                            //Vector3[] abc = GetSpriteCorners(spriteObject.GetComponent<SpriteRenderer>());

                            //if ((this.transform.localEulerAngles.y > 45 && this.transform.localEulerAngles.y < 105) || (this.transform.localEulerAngles.y > 235 && this.transform.localEulerAngles.y < 315))
                            //{
                            //    //print(abc[0] + " " + abc[1] + " " + abc[2] + " " + abc[3]);
                            //    abc[0].x = abc[1].x = abc[2].x = abc[3].x;
                            //}
                            //else
                            //{
                            //    abc[1].x = abc[0].x;
                            //    abc[1].z = abc[0].z;

                            //    abc[2].y = abc[1].y;
                            //    abc[2].z = abc[1].z;

                            //    abc[3].x = abc[2].x;
                            //    abc[3].z = abc[2].z;
                            //}
                            //for (int i = 0; i < abc.Length; i++)
                            //{
                            //    GameObject temp1 = new GameObject();
                            //    temp1.name = "upSide";
                            //    GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            //    temp.GetComponent<MeshRenderer>().material = mat; //Resources.Load("FrameMaterial");
                            //    temp.transform.parent = temp1.transform;
                            //    //temp.GetComponent<BoxCollider>().enabled = false;
                            //    Destroy(temp.GetComponent<BoxCollider>());

                            //    temp.transform.position = new Vector3(temp1.transform.position.x, temp1.transform.position.y, temp1.transform.position.z + .5f);
                            //    temp1.transform.position = abc[i];
                            //    temp1.transform.parent = this.transform;
                            //    temp1.transform.localScale = new Vector3(.1f, .1f, 0f);// this.transform.localScale;
                            //    float distance;
                            //    if (i == abc.Length - 1)
                            //    {
                            //        temp1.transform.LookAt(abc[0]);
                            //        distance = Math.Abs(Vector3.Distance(abc[i], abc[0]));
                            //    }
                            //    else
                            //    {
                            //        temp1.transform.LookAt(abc[i + 1]);
                            //        distance = Math.Abs(Vector3.Distance(abc[i], abc[i + 1]));
                            //    }
                            //    temp1.transform.localScale = new Vector3(.1f, .1f, (distance * 2) + .1f);
                            //    temp1.transform.localPosition = new Vector3(temp1.transform.localPosition.x, temp1.transform.localPosition.y, 0);
                            //}
                        }
                        using (var decoder = new MG.GIF.Decoder(imageData))
                        {
                            var img = decoder.NextImage();
                            while (img != null)
                            {
                                yield return null;
                                mFrames.Add(img.CreateTexture());
                                mFrameDelay.Add(img.Delay / 1000.0f);
                                img = decoder.NextImage();
                            }
                        }
                    }
                }
                else
                {
                    if (request.isNetworkError)
                    {
                        yield return StartCoroutine(DownloadImageAndShow());
                        print("Network Error");
                    }
                    else
                    {
                        if (request.error != null)
                        {
                            yield return StartCoroutine(DownloadImageAndShow());
                        }
                    }
                }
                request.Dispose();
            }
            isGif = true;
        }
        else
        {
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(metaData.thumbnft + "?tr=w-400,tr=h-400"))
            {
                request.SendWebRequest();
                while (!request.isDone)
                {
                    yield return null;
                }
                yield return new WaitForEndOfFrame();
                if (!request.isHttpError && !request.isNetworkError)
                {
                    if (request.error == null)
                    {
                        Texture2D loadedTexture = DownloadHandlerTexture.GetContent(request);
                        thunbNailImage = Sprite.Create(loadedTexture, new Rect(0f, 0f, loadedTexture.width, loadedTexture.height), new Vector2(.5f, 0f));
                        spriteObject = new GameObject();
                        spriteObject.name = "SpriteObject";
                        //Instantiate(new GameObject("SpriteObject"), this.transform.position, this.transform.rotation, this.transform);
                        spriteObject.transform.parent = this.transform;
                        spriteObject.transform.position = this.transform.position;
                        spriteObject.transform.rotation = this.transform.rotation;
                        if (spriteObject.transform.parent.name.Contains("25") || spriteObject.transform.parent.name.Contains("26"))
                        {
                            spriteObject.transform.localPosition = new Vector3(0, 0, .07f);
                        }
                        else
                        {
                            spriteObject.transform.localPosition = new Vector3(0, 0, .01f);
                        }

                        spriteObject.AddComponent<SpriteRenderer>().sprite = thunbNailImage;

                        spriteObject.GetComponent<SpriteRenderer>().flipX = true;
                        if ((thunbNailImage.texture.width * 1f) > (thunbNailImage.texture.height * 1f))
                        {
                            spriteObject.transform.localScale = new Vector3(2.4f / (thunbNailImage.texture.width / 100f), 2.4f / (thunbNailImage.texture.width / 100f), 1);
                        }
                        else if ((thunbNailImage.texture.width * 1f) < (thunbNailImage.texture.height * 1f))
                        {
                            spriteObject.transform.localScale = new Vector3(1.8f / (thunbNailImage.texture.height / 100f), 1.8f / (thunbNailImage.texture.height / 100f), 1);
                        }
                        else
                        {
                            spriteObject.transform.localScale = new Vector3(1.8f / (thunbNailImage.texture.height / 100f), 1.8f / (thunbNailImage.texture.height / 100f), 1);
                        }
                        NFTFromServer.RemoveOne();
                        this.gameObject.AddComponent<UnityEngine.BoxCollider>().center = new Vector3(0, .75f, 0);
                        this.gameObject.GetComponent<UnityEngine.BoxCollider>().size = new Vector3(2f, 1.65f, 0.5f);
                        isVideo = false;

                        CreateFrame();
                    }
                }
                else
                {
                    if (request.isNetworkError)
                    {
                        yield return StartCoroutine(DownloadImageAndShow());
                        print("Network Error");
                    }
                    else
                    {
                        if (request.error != null)
                        {
                            yield return StartCoroutine(DownloadImageAndShow());
                        }
                    }
                }
                request.Dispose();
            }
        }
        yield return null;

    }



    // Waqas Ahmad
    void CreateFrame()
    {
        float offsetValue = 0.049f;
        float xValue = 0, yValue = 0;

        Vector3[] abc = GetSpriteCorners(spriteObject.GetComponent<SpriteRenderer>());
        if ((this.transform.localEulerAngles.y > 45 && this.transform.localEulerAngles.y < 105) || (this.transform.localEulerAngles.y > 235 && this.transform.localEulerAngles.y < 315))
        {
            //print(abc[0] + " " + abc[1] + " " + abc[2] + " " + abc[3]);
            abc[0].x = abc[1].x = abc[2].x = abc[3].x;
        }
        else
        {
            abc[1].x = abc[0].x;
            abc[1].z = abc[0].z;

            abc[2].y = abc[1].y;
            abc[2].z = abc[1].z;

            abc[3].x = abc[2].x;
            abc[3].z = abc[2].z;
        }
        for (int i = 0; i < abc.Length; i++)
        {
            GameObject temp1 = new GameObject();
            temp1.name = "upSide" + i;
            GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            temp.GetComponent<MeshRenderer>().material = mat; //Resources.Load("FrameMaterial");
            temp.transform.parent = temp1.transform;
            // temp.GetComponent<BoxCollider>().enabled = false;
            Destroy(temp.GetComponent<BoxCollider>());
            temp.transform.position = new Vector3(temp1.transform.position.x, temp1.transform.position.y, temp1.transform.position.z + .5f);
            temp1.transform.position = abc[i];
            temp1.transform.parent = this.transform;
            temp1.transform.localScale = new Vector3(.1f, .1f, 0f);// this.transform.localScale;
            float distance;
            if (i == abc.Length - 1)
            {
                temp1.transform.LookAt(abc[0]);
                distance = Math.Abs(Vector3.Distance(abc[i], abc[0]));
            }
            else
            {
                temp1.transform.LookAt(abc[i + 1]);
                distance = Math.Abs(Vector3.Distance(abc[i], abc[i + 1]));
            }
            temp1.transform.localScale = new Vector3(.1f, .1f, (distance * 2) + .1f);
            //temp1.transform.localPosition = new Vector3(temp1.transform.localPosition.x, temp1.transform.localPosition.y, 0);

            // Offset By WaqasAhmad

            if(isDynamicMuseum)
            {
                switch (i)
                {
                    case 0:
                        xValue = 0;
                        yValue = 1;
                        break;

                    case 1:
                        xValue = 0;
                        yValue = 0;
                        break;

                    case 2:
                        xValue = -1;
                        yValue = 0;
                        break;

                    case 3:
                        xValue = -1;
                        yValue = 1;
                        break;

                    default:
                        break;
                }
            }
            //else
            //{
            //    offsetValue = 0.05f;
            //    switch (i)
            //    {
            //        case 0:
            //            xValue = -1;
            //            yValue = 0;
            //            break;

            //        case 1:
            //            xValue = 0;
            //            yValue = 0;
            //            break;

            //        case 2:
            //            xValue = 1;
            //            yValue = 0;
            //            break;

            //        case 3:
            //            xValue = 0;
            //            yValue = 0;
            //            break;

            //        default:
            //            break;
            //    }
            //}
            temp1.transform.localPosition = new Vector3(temp1.transform.localPosition.x + (xValue * offsetValue), temp1.transform.localPosition.y + (yValue * offsetValue), 0);
        }
    }


    void PauseVideo()
    {
        videoPlayer.Pause();
    }

    private void SetFrameNow(VideoPlayer source, long frameIdx)
    {
        print(source.width + " " + source.height);
    }

    public static Vector3[] GetSpriteCorners(SpriteRenderer renderer)
    {
        Vector3 topRight = renderer.transform.TransformPoint(renderer.sprite.bounds.max);
        Vector3 topLeft = renderer.transform.TransformPoint(new Vector3(renderer.sprite.bounds.max.x, renderer.sprite.bounds.min.y, renderer.sprite.bounds.max.z));
        Vector3 botLeft = renderer.transform.TransformPoint(renderer.sprite.bounds.min);
        Vector3 botRight = renderer.transform.TransformPoint(new Vector3(renderer.sprite.bounds.min.x, renderer.sprite.bounds.max.y, renderer.sprite.bounds.max.z));
        return new Vector3[] { topRight, topLeft, botLeft, botRight };
    }


    private void OnMouseUp()
    {
#if !UNITY_EDITOR
            if (Input.touchCount == 1)
            {
#endif
        if (playerControllerNew.isFirstPerson)
            playerCamera = playerControllerNew.firstPersonCameraObj.GetComponent<Camera>();
        else
            playerCamera = ReferencesForGamePlay.instance.randerCamera;
        if (PlayerCameraController.IsPointerOverUIObject()) return;

        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            if (hit.collider.gameObject.name == this.gameObject.name)
            {
                if (isVideo && !PlayerSelfieController.Instance.t_nftMuseums && !ShowNFTDetails.instance.displayPanel.activeInHierarchy && !TopMenuButtonController.Instance.Settings_pressed)
                {
                    print("showing video");
                    videoPlayer.Play();
                    ShowNFTDetails.instance.ShowVideo(this);
                    if (ReferencesForGamePlay.instance.playerControllerNew)
                        ReferencesForGamePlay.instance.playerControllerNew.restJoyStick();
                }
                else if (!isVideo && !PlayerSelfieController.Instance.t_nftMuseums && !ShowNFTDetails.instance.displayPanel.activeInHierarchy && !TopMenuButtonController.Instance.Settings_pressed)
                {
                    print(PlayerSelfieController.Instance.m_IsSelfieFeatureActive);
                    ShowNFTDetails.instance.ShowImage(this);
                    if (ReferencesForGamePlay.instance.playerControllerNew)
                        ReferencesForGamePlay.instance.playerControllerNew.restJoyStick();
                    isVisible = true;
                }
            }
        }

#if !UNITY_EDITOR

                }
#endif
    }


    public void Update()
    {
        //if (Input.GetMouseButtonUp(0))
        //{
        //    Debug.LogError("open image again");
        //#if !UNITY_EDITOR
        //            if (Input.touchCount == 1)
        //            {
        //#endif
        //            if (EmoteAnimationHandler.Instance.isEmoteActive || PlayerCameraController.IsPointerOverUIObject()) return;

        //            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        //            if (Physics.Raycast(ray, out hit, rayDistance))
        //            {
        //                if (hit.collider.gameObject.name == this.gameObject.name)
        //                {
        //                    if (isVideo && !PlayerSelfieController.Instance.t_nftMuseums && !ShowNFTDetails.instance.displayPanel.activeInHierarchy && !TopMenuButtonController.Instance.Settings_pressed)
        //                    {
        //                        print("showing video");
        //                        ShowNFTDetails.instance.ShowVideo(this);
        //                    }
        //                    else if (!isVideo && !PlayerSelfieController.Instance.t_nftMuseums && !ShowNFTDetails.instance.displayPanel.activeInHierarchy && !TopMenuButtonController.Instance.Settings_pressed)
        //                    {
        //                        print(PlayerSelfieController.Instance.m_IsSelfieFeatureActive);
        //                        ShowNFTDetails.instance.ShowImage(this);
        //                        isVisible = true;
        //                    }
        //                }
        //            }

        //#if !UNITY_EDITOR

        //                }
        //#endif
        // }

        if (isGif)
        {
            if (mFrames == null)
            {
                return;
            }

            mTime += Time.deltaTime;

            if (mTime >= mFrameDelay[mCurFrame])
            {
                mCurFrame = (mCurFrame + 1) % mFrames.Count;
                mTime = 0.0f;

                Sprite currFrame = Sprite.Create(mFrames[mCurFrame], new Rect(0f, 0f, mFrames[mCurFrame].width, mFrames[mCurFrame].height), new Vector2(.5f, 0f));
                spriteObject.GetComponent<SpriteRenderer>().sprite = currFrame;

                if (isVisible)
                    ShowNFTDetails.instance.UpdateGif(currFrame);
            }
        }


    }
}

