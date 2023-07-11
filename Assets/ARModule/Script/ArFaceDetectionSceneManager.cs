//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEngine.UI;
//using UnityEngine.XR.ARFoundation;
//using UnityEngine.XR.ARSubsystems;

//public class ArFaceDetectionSceneManager : MonoBehaviour
//{
//    public static ArFaceDetectionSceneManager Instance;

//    public GameObject uiCan;

//    public CurrentSelectedScreen currentSelectedScreen;

//    public GameObject bgWallObj;

//    public ARCameraBackground aRCameraBackground;
//    public Material actionCustomMat;

//    public GameObject mainAvatar;

//    [Header("Capture Screen")]
//    public GameObject captureViewScreen;
//    public Image displayCatureImage;

//    [Header("Room Screen")]
//    public Vector3 roomCharMainPos; 
//    public float roomCharMainScale; 

//    [Header("Action Detection")]
//    public GameObject actionAvatarHead;
//    public Vector3 actionCharMainPos;
//    public float actionCharMainScale;

//    [Header("Face detection")]
//    public ARFaceManager aRFaceManager;

//    [Header("Plan Detection")]
//    public ARPlaneManager aRPlaneManager;
//    public ARPointCloudManager aRPointCloudManager;
//    public ARRaycastManager aRRaycastManager;
//    public PlaceOnPlane placeOnPlane;

//    private void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//        }
//    }

//    // Start is called before the first frame update
//    void Start()
//    {
//        DefaultSetScreen();        
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        //this method is used to action avatar following.......
//        ActionHeadMovement();
//    }

//    public void DefaultSetScreen()
//    {
//        currentSelectedScreen = CurrentSelectedScreen.Room;

//        SetARCameraCustomMaterial(true);
//        AvatarAndBGWallActive(true);
//        ActionHeadActive(false);

//        //set default main avatar position and rotation.......
//        SetMainAvatarPositionAndRotation(roomCharMainPos, 180, roomCharMainScale);

//        aRFaceManager.enabled = false;
//        aRPlaneManager.enabled = false;
//        aRPointCloudManager.enabled = false;
//        aRRaycastManager.enabled = false;
//        placeOnPlane.enabled = false;
//    }

//    void SetMainAvatarPositionAndRotation(Vector3 pos, int rotation, float scale)
//    {
//        Vector3 endRotation = new Vector3(0, rotation, 0);
//        mainAvatar.transform.rotation = Quaternion.Euler(endRotation);

//        mainAvatar.transform.position = pos;

//        Vector3 endScale = new Vector3(scale, scale, scale);
//        mainAvatar.transform.localScale = endScale;
//    }

//    //this method is used to Action Button click.......
//    public void OnARActionButtonClick()
//    {
//        currentSelectedScreen = CurrentSelectedScreen.Action;

//        SetARCameraCustomMaterial(true);
//        AvatarAndBGWallActive(true);

//        ActionHeadActive(true);

//        if (!aRFaceManager.enabled)
//        {
//            aRFaceManager.enabled = true;
//        }

//        //Disable all arface object in to screen.......
//        GameObject[] arfaces = GameObject.FindGameObjectsWithTag("ARFace");
//        for (int i = 0; i < arfaces.Length; i++)
//        {
//            arfaces[i].transform.GetChild(0).gameObject.SetActive(false);
//        }

//        //disable all Ar Plan object in to screen.......
//        ArPlanObjectDisable();

//        //set default main avatar position and rotation.......
//        SetMainAvatarPositionAndRotation(actionCharMainPos, 180, actionCharMainScale);
//    }

//    GameObject ActionFollowFaceObject;
//    public void ActionHeadMovement()
//    {
//        Debug.LogError("currentSelectedScreen:" + currentSelectedScreen);
//        if (currentSelectedScreen == CurrentSelectedScreen.Action)
//        {
//            GameObject[] arfaces = GameObject.FindGameObjectsWithTag("ARFace");
//            for (int i = 0; i < arfaces.Length; i++)
//            {
//                arfaces[i].transform.GetChild(0).gameObject.SetActive(false);
//            }
//            Debug.LogError("ar lenght:" + arfaces.Length);
//            if (arfaces.Length > 0)
//            {
//                ActionFollowFaceObject = arfaces[0];
//                actionAvatarHead.transform.rotation = ActionFollowFaceObject.transform.rotation;
//                //ActionHeadObj.transform.rotation = Quaternion.Euler(new Vector3(ActionFollowFaceObject.transform.rotation.x * -1, ActionFollowFaceObject.transform.rotation.y * -1, ActionFollowFaceObject.transform.rotation.z * -1));
//            }
//            else
//            {
//                actionAvatarHead.transform.rotation = Quaternion.Euler(Vector3.zero);
//            }
//        }
//    }

//    void AvatarAndBGWallActive(bool isactive)
//    {
//        Debug.LogError("AvatarAndBGWallActive:"+isactive);
//        mainAvatar.SetActive(isactive);
//        bgWallObj.SetActive(isactive);
//    }

//    void ActionHeadActive(bool isactive)
//    {
//        actionAvatarHead.SetActive(isactive);
//    }
       

//    //this method is used to SetARCameraBackground customMaterial Active, inActive and set.......
//    void SetARCameraCustomMaterial(bool isenable)
//    {
//        if (isenable)
//        {
//            aRCameraBackground.useCustomMaterial = isenable;
//            aRCameraBackground.customMaterial = actionCustomMat;
//        }
//        else
//        {
//            aRCameraBackground.customMaterial = null;
//            aRCameraBackground.useCustomMaterial = isenable;
//        }
//    }

//    //this method is used to Room Button click.......
//    public void OnRoomButtonclick()
//    {
//        currentSelectedScreen = CurrentSelectedScreen.Room;

//        SetARCameraCustomMaterial(true);
//        AvatarAndBGWallActive(true);

//        //Disable all arface object in to screen.......
//        GameObject[] arfaces = GameObject.FindGameObjectsWithTag("ARFace");
//        for (int i = 0; i < arfaces.Length; i++)
//        {
//            arfaces[i].transform.GetChild(0).gameObject.SetActive(false);
//        }

//        //disable all Ar Plan object in to screen.......
//        ArPlanObjectDisable();

//        ActionHeadActive(false);

//        //set default main avatar position and rotation.......
//        SetMainAvatarPositionAndRotation(roomCharMainPos, 180, roomCharMainScale);
//    }

//    //this method is used to Normal Button click.......
//    public void OnARSelfieButtonClick()
//    {
//        currentSelectedScreen = CurrentSelectedScreen.Normal;

//        if (!aRFaceManager.enabled)
//        {
//            aRFaceManager.enabled = true;
//        }

//        SetARCameraCustomMaterial(false);

//        GameObject[] arfaces = GameObject.FindGameObjectsWithTag("ARFace");
//        for (int i = 0; i < arfaces.Length; i++)
//        {
//            arfaces[i].transform.GetChild(0).gameObject.SetActive(true);
//        }

//        AvatarAndBGWallActive(false);
//        ActionHeadActive(false);
//        ArPlanObjectDisable();
//    }

//    //this method is used to AR button click.......
//    public void OnARButtonClick()
//    {
//        currentSelectedScreen = CurrentSelectedScreen.AR;

//        if (!aRPlaneManager.enabled)
//        {
//            aRPlaneManager.enabled = true;
//            aRPointCloudManager.enabled = true;
//            aRRaycastManager.enabled = true;
//            placeOnPlane.enabled = true;

//            SetARCameraCustomMaterial(false);
//        }

//        //this is used to disable all ARFace in to screen.......
//        if (aRFaceManager.enabled)
//        {
//            GameObject[] arfaces = GameObject.FindGameObjectsWithTag("ARFace");
//            for (int i = 0; i < arfaces.Length; i++)
//            {
//                arfaces[i].gameObject.SetActive(false);
//            }
//            aRFaceManager.enabled = false;
//        }

//        AvatarAndBGWallActive(false);
//        ActionHeadActive(false);
//    }

//    //this method is used todisable all plan in to screen.......
//    void ArPlanObjectDisable()
//    {
//        if (aRPlaneManager.enabled)
//        {
//            GameObject[] arPointCloud = GameObject.FindGameObjectsWithTag("ARPointCloud");
//            for (int i = 0; i < arPointCloud.Length; i++)
//            {
//                arPointCloud[i].SetActive(false);
//            }
//            GameObject[] arPlanes = GameObject.FindGameObjectsWithTag("ARPlane");
//            for (int i = 0; i < arPlanes.Length; i++)
//            {
//                arPlanes[i].SetActive(false);
//            }

//            aRPlaneManager.enabled = false;
//            aRPointCloudManager.enabled = false;
//            aRRaycastManager.enabled = false;
//            placeOnPlane.enabled = false;
//        }
//    }

//    //this method is used to close button click.......
//    public void OnCloseButtonClick()
//    {
//        //currentSelectedScreen = CurrentSelectedScreen.Room;
//        //SceneManager.LoadScene("MainScene");
//    }

//    public void OnCaptureButtonClick()
//    {
//        uiCan.SetActive(false);
//        Capture();
//    }

//    private Camera newCam;
//    private Texture2D screenshot;
//    private RenderTexture screenshotRT;

//    public void Capture()
//    {
//        GameObject g = new GameObject();
//        g.transform.parent = Camera.main.transform;
//        g.transform.localPosition = Vector3.zero;
//        g.transform.localRotation = Quaternion.Euler(Vector3.zero);
//        g.SetActive(false);
//        newCam = g.AddComponent<Camera>();

//        newCam.enabled = false;

//        screenshotRT = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Default);

//        StartCoroutine(LoadTexture());
//    }

//    IEnumerator LoadTexture()
//    {
//        newCam.aspect = Camera.main.aspect;
//        newCam.fieldOfView = Camera.main.fieldOfView;
//        newCam.orthographic = Camera.main.orthographic;
//        newCam.orthographicSize = Camera.main.orthographicSize;
//        newCam.backgroundColor = Camera.main.backgroundColor;
//        newCam.cullingMask = 1 << 8;
//        newCam.cullingMask = ~newCam.cullingMask;
//        newCam.depth = 1;

//        yield return new WaitForEndOfFrame();

//        newCam.targetTexture = screenshotRT;
//        newCam.Render();
//        screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
//        screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
//        screenshot.Apply();
//        newCam.targetTexture = null;
//        Sprite captureSp = Sprite.Create(screenshot, new Rect(0, 0, screenshot.width, screenshot.height), new Vector2(0, 0), 100f, 0, SpriteMeshType.FullRect);

//        uiCan.SetActive(true);
//        displayCatureImage.sprite = captureSp;
//        captureViewScreen.SetActive(true);
//        //byte[] imageByte = screenshot.EncodeToPNG();
//        string filename = "ArCapture" + System.DateTime.Now.ToString("MM-dd-yy_hh-mm-ss") + ".png";
//        NativeGallery.SaveImageToGallery(screenshot, "ArCapture", filename, null);
//        //NativeGallery.SaveImageToGallery(imageByte, "ArCapture", filename);
//    }
//}

//public enum CurrentSelectedScreen
//{
//    Action,
//    Room,
//    Normal,
//    AR
//}