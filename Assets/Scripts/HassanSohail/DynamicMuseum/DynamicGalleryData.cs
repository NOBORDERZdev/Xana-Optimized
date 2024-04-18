using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.Video;
using XanaNFT;
using UnityEngine.UI;


public class DynamicGalleryData : MonoBehaviour
{
    //public MetaData metaData;
    //public TokenDetails tokenDetails;
    //public CreatorDetails creatorDetails;
    public S3NftDetail detail;

    public Text CreatorName;

    //public string nftLink;
    //public string creatorLink;
    public Sprite thunbNailImage;
    public GameObject spriteObject;
    public GameObject SpotLightObj;
    //------------------------------------
    //For videos
    public VideoClip videoClip;
    public SpriteRenderer spriteRenderer;
    public VideoPlayer videoPlayer;
    public VideoPlayer videoPlayerWithStats;
    public bool VideoPlaying;
    public bool isVideo = false;
    public string squareSize;
    public string potraiteSize;
    public string landscapeSize;
    public Vector3 spotLightPrefabPos;
    // Start is called before the first frame update
    public RaycastHit hit;
    public Camera playerCamera;

    public float rayDistance = 4f;

    public Fram_Image NewFrame;

    public GameObject frame;
    Material mat;
    public bool isDynamicMuseum = false;
    public GameObject framePrefab;
    public GameObject spotLightPrefab;

    public bool SqureImage;
    public Material FrameMat;
    
    // private int layerMask;

    //private bool canOpenPicture = true;


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
    public string VideoLink;

    private void Start()
    {
        mat = (Material)Resources.Load("FramMaterial");
        if (playerCamera == null)
        {
            playerCamera = ReferencesForGamePlay.instance.randerCamera;
        }

      
        GameObject go = new GameObject("FrameManager");
        go.AddComponent<CreateFrameHelper>();

        if (FrameMat!=null)
        {
        CreateFrameHelper.instance.Materials= FrameMat;

        }


    }
    public void StartNow()
    {
        frame = this.transform.GetChild(0).gameObject;
        frame.SetActive(false);
        //print("StartNow");
        if (detail != null && detail.check) //sss
            StartCoroutine(DownloadImageAndShow());
        
    }

    private void OnApplicationFocus()
    {
        StartCoroutine(PlayVideoOnFocus());
        //Invoke(nameof(PauseVideo), 0.75f);
    }


    private void OnApplicationPause()
    {
        Invoke(nameof(PauseVideo), 0.1f);
    }

    private string templink;

    private GameObject BoxTrigger;

    private void AddBoxColliderToPlayVideo()
    {
        BoxTrigger = new GameObject();
        BoxTrigger.name = "BoxTrigger";
        BoxTrigger.AddComponent<BoxCollider>();
        BoxTrigger.GetComponent<BoxCollider>().isTrigger=true;
        BoxTrigger.GetComponent<BoxCollider>().size = new Vector3(ColliderSize.x, ColliderSize.y, ColliderSize.z);
       // BoxTrigger.transform.SetPositionAndRotation(transform.position,transform.rotation);
        BoxTrigger.transform.parent = transform;
        BoxTrigger.transform.localPosition = new Vector3(-0.109f, -1, -1.32f);

        BoxTrigger.AddComponent<PlayVideoBoxCollider>();
    }

    private IEnumerator DownloadImageAndShow()
    {
        while (Application.internetReachability == NetworkReachability.NotReachable)
        {
            yield return new WaitForEndOfFrame();
            print("Internet Not Reachable");
        }
        print("~~~~~~");
        templink = "";
        if (detail.asset_link.EndsWith(".jpg") || detail.asset_link.EndsWith(".png"))
        {
            isVideo = false;
            templink = detail.asset_link;
            //templink = templink.Replace("https://cdn.xana.net/", "https://aydvewoyxq.cloudimg.io/_apitestxana_/");
            print("~~~~~~~~~~~~~~~~~"+ detail.ratio);
            if (detail.ratio != "" && detail.ratio != null)
            {
                if (detail.ratio == "1:1") // is sqaure 
                {
                    templink = templink + squareSize;
                }
                else if (detail.ratio == "9:16") // is potraite 
                {
                    templink = templink + potraiteSize;
                }
                else // is lanscape 
                {
                    templink = templink + landscapeSize;
                }
            }
            else
            {
                templink = templink + squareSize;
            }
           

            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(templink))
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
                        spriteObject.transform.parent = this.transform;
                        spriteObject.transform.position = this.transform.position;
                        spriteObject.transform.rotation = this.transform.rotation;
                        spriteObject.transform.localPosition = new Vector3(0, 0, .01f);
                        spriteObject.AddComponent<SpriteRenderer>().sprite = thunbNailImage;


                      



                        if (!CreateDynamicFrames)
                        {
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


                        }


                        NFTFromServer.RemoveOne();
                        this.gameObject.AddComponent<UnityEngine.BoxCollider>().center = ColiderPos /*new Vector3(0.0411f, 1.069336f, 0.0712f)*/;
                        this.gameObject.GetComponent<UnityEngine.BoxCollider>().size = ColliderSize /*new Vector3(2.515f, 2.288f, 0.3574f)*/;


                        CreateFrame(detail.ratio);

                        spriteObject.transform.localScale = new Vector3(0.44f, 0.44f, 0.44f);

                        // upadte sprite position After Creating Frame
                        spriteObject.transform.localPosition = new Vector3(-0.04f, 0.04f, .01f);
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

        if (detail.asset_link.EndsWith(".mp4") || detail.asset_link.EndsWith(".mov"))
        {
            //spriteObject = new GameObject();
            //spriteObject.name = "SpriteObject";
            //spriteObject.AddComponent<SpriteRenderer>();
            /// Downloading video thumbinal image to show in world frame
            /// 

            templink = detail.thumbnail;
            if (detail.ratio != "" && detail.ratio != null)
            {
                if (detail.ratio == "1:1") // is sqaure 
                {
                    templink = templink + "?width=512&height=512";
                }
                else if (detail.ratio == "9:16") // is potraite 
                {
                    templink = templink + potraiteSize;
                }
                else // is lanscape 
                {
                    templink = templink + landscapeSize;
                }
            }
            else
            {
                templink = templink + "?width=512&height=512";
            }
            print("Image create on index !!!!!!!!" + detail.index + "path is : "+ templink);
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(templink))
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
                        spriteObject.transform.parent = this.transform;
                        spriteObject.transform.position = this.transform.position;
                        spriteObject.transform.rotation = this.transform.rotation;
                        spriteObject.transform.localPosition = new Vector3(0, 0, .01f);
                        spriteObject.AddComponent<SpriteRenderer>().sprite = thunbNailImage;
                       

                        if (!CreateDynamicFrames)
                        {
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


                        }


                        NFTFromServer.RemoveOne();
                        this.gameObject.AddComponent<UnityEngine.BoxCollider>().center = ColiderPos;/*new Vector3(0, .75f, 0);*/
                        this.gameObject.GetComponent<UnityEngine.BoxCollider>().size = new Vector3(ColliderSize.x, ColliderSize.y, ColliderSize.z); /*new Vector3(2f, 1.65f, 0.5f);*/
                        //AddBoxColliderToPlayVideo();


                        CreateFrame(detail.ratio);

                        if (XanaEventDetails.eventDetails.DataIsInitialized)
                        {
                            if (detail.ratio == "1:1" ) // is sqaure 
                            {
                                //square
                                spriteObject.transform.localScale = new Vector3(0.28f, 0.23f, 0.44f);
                            }
                            else if (detail.ratio == "9:16" ) // is potraite 
                            {
                                //potrate
                                spriteObject.transform.localScale = new Vector3(0.23f, 0.155f, 0.2f);
                            }
                            else // is lanscape 
                            {
                                //landscape
                                spriteObject.transform.localScale = new Vector3(0.3f, 0.39f, 0.44f);
                            }
                        }

                        else
                        {
                            spriteObject.transform.localScale = new Vector3(0.50f, 0.42f, 0.44f);
                        }

                        if((APIBasepointManager.instance.IsXanaLive && XanaEventDetails.eventDetails.DataIsInitialized) || (APIBasepointManager.instance.IsXanaLive && !XanaEventDetails.eventDetails.DataIsInitialized))
                        {
                            spriteObject.transform.localScale = new Vector3(0.44f, 0.44f, 0.44f);
                        }
                       

                        // upadte sprite position After Creating Frame
                        spriteObject.transform.localPosition = new Vector3(-0.04f, 0.04f, .01f);
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
                isVideo = true;
            }
            /// 




            //videoPlayer = spriteObject.AddComponent<VideoPlayer>();
            //videoPlayer.source = VideoSource.Url;
            //videoPlayer.url = detail.asset_link;
            //videoPlayer.SetDirectAudioMute(0, true);
            //videoPlayer.playOnAwake = false;

            //while (!videoPlayer.isPrepared)
            //{
            //    yield return new WaitForEndOfFrame();
            //}


#if UNITY_IOS
                    //    thunbNailImage = Sprite.Create(new Texture2D((int)videoPlayer.texture.height, (int)videoPlayer.texture.width), new Rect(0f, 0f, (int)videoPlayer.texture.height, (int)videoPlayer.texture.width), new Vector2(0.5f, 0f));
#elif UNITY_ANDROID
           // thunbNailImage = Sprite.Create(new Texture2D((int)videoPlayer.height, (int)videoPlayer.width), new Rect(0f, 0f, (int)videoPlayer.height, (int)videoPlayer.width), new Vector2(0.5f, 0f));
#endif
                      //  spriteObject.transform.position = this.transform.position;
                      //  spriteObject.transform.rotation = this.transform.rotation;

                      ////  videoPlayer.renderMode = VideoRenderMode.MaterialOverride;
                      // // videoPlayer.isLooping = true;
                      //  //videoPlayer.targetMaterialRenderer = spriteObject.GetComponent<SpriteRenderer>();
                      //  //spriteObject.GetComponent<SpriteRenderer>().sprite = thunbNailImage;
                      //  spriteObject.GetComponent<SpriteRenderer>().flipX = true;
                      //  spriteObject.transform.parent = this.transform;

                      //  spriteObject.transform.localPosition = new Vector3(0, 0, .01f);

                      //  print(videoPlayer.width + " " + videoPlayer.height);

//#if UNITY_IOS
//                        if ((videoPlayer.texture.width * 1f) / (videoPlayer.texture.height * 1f) > 1.778f)
//                        {
//                            spriteObject.transform.localScale = new Vector3(1.8f / (videoPlayer.texture.width / 100f), 1.87f / (videoPlayer.texture.height / 100f), 1);
//                        }
//                        else
//                        {
//                            spriteObject.transform.localScale = new Vector3(1.8f / (videoPlayer.texture.height / 100f), 1.8f / (videoPlayer.texture.width / 100f), 1);
//                        }
//                        print(videoPlayer.texture.width + " " + videoPlayer.texture.height);
//#elif UNITY_ANDROID
//                        if ((videoPlayer.width * 1f) / (videoPlayer.height * 1f) > 1.778f)
//                        {
//                            spriteObject.transform.localScale = new Vector3(1.8f / (videoPlayer.width / 100f), 1.8f / (videoPlayer.height / 100f), 1);
//                        }
//                        else
//                        {
//                            spriteObject.transform.localScale = new Vector3(1.8f / (videoPlayer.height / 100f), 1.8f / (videoPlayer.width / 100f), 1);
//                        }

//#endif
                        //spriteObject.transform.Rotate(new Vector3(0, 0, 0));


                        //NFTFromServer.RemoveOne();
            //this.gameObject.AddComponent<UnityEngine.BoxCollider>().center = new Vector3(0, .75f, 0);
            //this.gameObject.GetComponent<UnityEngine.BoxCollider>().size = new Vector3(ColliderSize.x, ColliderSize.y, ColliderSize.z); /*new Vector3(2f, 1.65f, 0.5f);*/
            //isVideo = true;

            //CreateFrame(detail.ratio);
            //spriteObject.transform.localScale = new Vector3(0.113894f, 0.113894f, 1f);
            //// upadte sprite position After Creating Frame
            //spriteObject.transform.localPosition = new Vector3(-0.04f, 0.04f, .01f);

            //Invoke(nameof(PauseVideo), 1f);

        }
        yield return null;
    }

    public bool CreateDynamicFrames;

    public Vector3 FrameLocalScale;
    public Vector3 FrameLocalPos;
    public Vector3 SpotLightPos;
    public Vector3 ColliderSize;
    public Vector3 ColiderPos;


    // Waqas Ahmad
    private void CreateFrame(string ratio)
    {
       

        if (CreateDynamicFrames)
        {
            NewFrame.render = spriteObject;
            NewFrame.CreateCubeFrame();
        }
        else
        {
            GameObject frame = Instantiate(framePrefab, this.gameObject.transform);
            frame.transform.localPosition = new Vector3(FrameLocalPos.x, FrameLocalPos.y /*1.062f*/, FrameLocalPos.z);
            frame.transform.localEulerAngles = new Vector3(90, -180.0f, 0);
            frame.transform.localScale = new Vector3(FrameLocalScale.x, FrameLocalScale.y, FrameLocalScale.z);

            //spriteObject.transform.localScale = new Vector3(0.44f, 0.44f, 0.44f);
        }

        //spriteObject.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);


        //float offsetValue = 0.083f;
        //Vector3 tempScale;
        //float offsetValue_z = 0;
        //float xValue = 0, yValue = 0;

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
        //    temp1.name = "upSide" + i;
        //    GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //    temp.GetComponent<MeshRenderer>().material = mat; //Resources.Load("FrameMaterial");
        //    temp.transform.parent = temp1.transform;
        //    // temp.GetComponent<BoxCollider>().enabled = false;
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
        //    //temp1.transform.localPosition = new Vector3(temp1.transform.localPosition.x, temp1.transform.localPosition.y, 0);

        //    // Offset By WaqasAhmad

        //    if (isDynamicMuseum)
        //    {
        //        switch (i)
        //        {
        //            case 0:
        //                offsetValue_z = -0.78f;
        //                xValue = 0;
        //                yValue = 1;
        //                break;

        //            case 1:
        //                offsetValue_z = -0.93f;
        //                xValue = 0;
        //                yValue = 0;
        //                break;

        //            case 2:
        //                offsetValue_z = -0.78f;
        //                xValue = -1;
        //                yValue = 0;
        //                break;

        //            case 3:
        //                offsetValue_z = -0.93f;
        //                xValue = -1;
        //                yValue = 1;
        //                break;

        //            default:
        //                break;
        //        }
        //    }
        //    //else
        //    //{
        //    //    offsetValue = 0.05f;
        //    //    switch (i)
        //    //    {
        //    //        case 0:
        //    //            xValue = -1;
        //    //            yValue = 0;
        //    //            break;

        //    //        case 1:
        //    //            xValue = 0;
        //    //            yValue = 0;
        //    //            break;

        //    //        case 2:
        //    //            xValue = 1;
        //    //            yValue = 0;
        //    //            break;

        //    //        case 3:
        //    //            xValue = 0;
        //    //            yValue = 0;
        //    //            break;

        //    //        default:
        //    //            break;
        //    //    }
        //    //}
        //    temp1.transform.localPosition = new Vector3(temp1.transform.localPosition.x + (xValue * offsetValue), temp1.transform.localPosition.y + (yValue * offsetValue), 0);

        //    tempScale = temp1.transform.localScale;
        //    tempScale.z += offsetValue_z;
        //    temp1.transform.localScale = tempScale; 
        //}
        GameObject spotLightObj = Instantiate(spotLightPrefab, this.gameObject.transform);
        spotLightObj.transform.localPosition = spotLightPrefabPos/*new Vector3(0, 2.859f, 0.598f)*/ ;
        spotLightObj.transform.localEulerAngles = new Vector3(-22.857f, 180f, 0f);
        GameObject lightTemp = Instantiate(SpotLightObj);
        lightTemp.transform.SetParent(this.gameObject.transform);
        lightTemp.gameObject.transform.localScale = new Vector3(spriteObject.transform.localScale.x, spriteObject.transform.localScale.y, spriteObject.transform.localScale.z);
        lightTemp.gameObject.transform.localPosition = /*SpotLightPos*/ new Vector3(0, SpotLightPos.y , SpotLightPos.z);
        lightTemp.gameObject.transform.localEulerAngles = Vector3.zero;
        Vector3 rot = this.gameObject.transform.localEulerAngles;
        this.gameObject.transform.localEulerAngles = new Vector3(-10, rot.y, rot.z);
    }

    private bool Paused;

    private void PauseVideo()
    {
        videoPlayer.Pause();
        videoPlayerWithStats.Pause();
        Paused = true;
    }

    private IEnumerator PlayVideoOnFocus()
    {
        if (Paused)
        {
            if (detail.description.Length > 0)
            {
                videoPlayerWithStats.enabled = false;
                yield return new WaitForSeconds(0.5f);
                videoPlayerWithStats.enabled = true;
                yield return new WaitForSeconds(1f);
                videoPlayerWithStats.Play();
            }
            else
            {
                videoPlayer.enabled = false;
                yield return new WaitForSeconds(0.5f);
                videoPlayer.enabled = true;
                yield return new WaitForSeconds(1f);
                videoPlayer.Play();
            }
            Invoke(nameof(PauseVideo), 0.1f);
            Paused = false;
        }
       
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
        if (PlayerCameraController.IsPointerOverUIObject()) return;

        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            if (hit.collider.gameObject.name == this.gameObject.name)
            {
                if (isVideo && !PlayerSelfieController.Instance.t_nftMuseums && !ShowNFTDetails.instance.displayPanel.activeInHierarchy && !TopMenuButtonController.Instance.Settings_pressed)
                {
                    print("showing video");
                    GamePlayUIHandler.inst.gamePlayUIParent.SetActive(false);
                    ShowNFTDetails.instance.ShowVideo(this);
                    if (hit.collider.GetComponent<DynamicGalleryData>().detail.description.Length > 0)
                    {
                        videoPlayerWithStats.Play();
                        print("showing video");
                    }
                    else
                    {
                        videoPlayer.Play();
                    }
                    if (ReferencesForGamePlay.instance.playerControllerNew)
                        ReferencesForGamePlay.instance.playerControllerNew.restJoyStick();
                }
               else if (!isVideo && !PlayerSelfieController.Instance.t_nftMuseums && !ShowNFTDetails.instance.displayPanel.activeInHierarchy && !TopMenuButtonController.Instance.Settings_pressed)
                {
                    //print(PlayerSelfieController.Instance.m_IsSelfieFeatureActive);
                    GamePlayUIHandler.inst.gamePlayUIParent.SetActive(false);
                    ShowNFTDetails.instance.ShowImage(this.GetComponent<DynamicGalleryData>());
                    isVisible = true;

                    if (ReferencesForGamePlay.instance.playerControllerNew)
                        ReferencesForGamePlay.instance.playerControllerNew.restJoyStick();
                    print("Clicked on Image");
                }
               
                
            }
        }

        

#if !UNITY_EDITOR

    }
#endif
    }



    //public void Update()
    //{
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

    //    if (isGif)
    //    {
    //        if (mFrames == null)
    //        {
    //            return;
    //        }

    //        mTime += Time.deltaTime;

    //        if (mTime >= mFrameDelay[mCurFrame])
    //        {
    //            mCurFrame = (mCurFrame + 1) % mFrames.Count;
    //            mTime = 0.0f;

    //            Sprite currFrame = Sprite.Create(mFrames[mCurFrame], new Rect(0f, 0f, mFrames[mCurFrame].width, mFrames[mCurFrame].height), new Vector2(.5f, 0f));
    //            spriteObject.GetComponent<SpriteRenderer>().sprite = currFrame;

    //            if (isVisible)
    //                ShowNFTDetails.instance.UpdateGif(currFrame);
    //        }
    //    }


    //}
}

