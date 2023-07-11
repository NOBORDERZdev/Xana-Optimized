//using DG.Tweening;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.XR.ARFoundation;
//using UnityEngine.XR.ARFoundation.Samples;
//using UnityEngine.XR.ARSubsystems;

//[RequireComponent(typeof(ARFaceManager))]
//public class ARFacePoseTrackingManager : MonoBehaviour
//{
//    public static ARFacePoseTrackingManager Instance;

//    public ARPoseDriver _aRPoseDriver;

//    public GameObject playerHead;

//    public SkinnedMeshRenderer m_SkinnedMeshRenderer;

//    //public SkinnedMeshRenderer boySkinMeshRenderer;
//    //public SkinnedMeshRenderer girlSkinMeshRenderer;

//    //public bool isGirl = false;

//    ARFaceManager m_ARFaceManager;

//    void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//        }
//        m_ARFaceManager = GetComponent<ARFaceManager>();

//        /*if (AvatarStaticDataStoreHandler.isGirl)
//        {
//            m_SkinnedMeshRenderer = girlSkinMeshRenderer;
//        }
//        else
//        {
//            m_SkinnedMeshRenderer = boySkinMeshRenderer;
//        }*/

//#if UNITY_IOS
//            _aRPoseDriver.enabled = false;
//#endif
//    }

//    private void Start()
//    {
//        defaultTargetPos = moveTargetObj.transform.position;
//        //defaultRotation = RootAnimTargetObj.transform.rotation;
//        defaultRotation = new Quaternion(0, 0, 0, 1f);
//    }

//    private void Update()
//    {
//        if (!isVideoOpen)
//        {
//            SetARPoseOnAvatar();
//        }
//    }

//    private void OnDisable()
//    {
//        ToggleFaceDetection();
//    }

//    public void ToggleFaceDetection()
//    {
//        m_ARFaceManager.enabled = !m_ARFaceManager.enabled;

//        if (m_ARFaceManager.enabled)
//        {
//            SetAllPlanesActive(true);
//        }
//        else
//        {
//            SetAllPlanesActive(false);
//        }
//    }

//    void SetAllPlanesActive(bool value)
//    {
//        foreach (var face in m_ARFaceManager.trackables)
//        {
//            face.gameObject.SetActive(value);
//        }
//    }

//    public GameObject RootAnimTargetObj;
//    public GameObject moveTargetObj;
//    public bool isTracking = false;
//    bool lastState;
//    void SetARPoseOnAvatar()
//    {       
//        foreach (var face in m_ARFaceManager.trackables)
//        {
//            if (face.trackingState == TrackingState.Tracking)
//            {
//                isTracking = true;

//                Vector3 headRotation;
//#if UNITY_IOS
//                headRotation = new Vector3(-face.transform.rotation.eulerAngles.x, face.transform.rotation.eulerAngles.y, -face.transform.rotation.eulerAngles.z);
//#else
//                headRotation = new Vector3(-face.transform.rotation.eulerAngles.x, -face.transform.rotation.eulerAngles.y, face.transform.rotation.eulerAngles.z);
//#endif
//                playerHead.transform.localRotation = Quaternion.Lerp(playerHead.transform.localRotation, Quaternion.Euler(headRotation), 10 * Time.deltaTime);

//                if (RootAnimTargetObj != null)
//                {
//                    RootAnimTargetObj.transform.localRotation = Quaternion.Lerp(RootAnimTargetObj.transform.localRotation, Quaternion.Euler(headRotation), 10 * Time.deltaTime);
//                }

//                float finalXRotValue = 0;

//                if (face.transform.position.z <= 0.35f)
//                {
//                    finalXRotValue = -(0.4f - face.transform.position.z);
//                }
//                else if (face.transform.position.z > 0.6f)
//                {
//                    //finalXRotValue = (face.transform.position.z - 0.6f);
//                }

//                if (moveTargetObj != null)
//                {
//                    Vector3 movePos = new Vector3(moveTargetObj.transform.localPosition.x, moveTargetObj.transform.localPosition.y, Mathf.Clamp(finalXRotValue, -0.2f, 0f));
//                    moveTargetObj.transform.localPosition = Vector3.Lerp(moveTargetObj.transform.localPosition, movePos, 10 * Time.deltaTime);

//                    //Debug.LogError("face:" + face.transform.position.z + " :finalXRotValue:" + finalXRotValue + " :face postion:" + face.transform.localPosition.z + ":LocalPos:" + moveTargetObj.transform.localPosition);
//                }
//            }
//            else
//            {
//                isTracking = false;
//            }
//            //playerHead.transform.localRotation = Quaternion.Euler(headRotation);
//            //RootAnimTargetObj.transform.localRotation = Quaternion.Euler(headRotation);
//        }

//        //Debug.LogError("Count:" + m_ARFaceManager.trackables.count + "  :isTracking:" + isTracking);
//        if (m_ARFaceManager.trackables.count <= 0)
//        {
//            isTracking = false;
//        }

//        if (lastState != isTracking)
//        {
//            lastState = isTracking;
//            if (!isTracking && !isMoveToDefault)
//            {
//                ResetToDefaultAvatar();
//            }
//        }
//    }

//    public bool isVideoOpen = false;
//    public void SetDefaultMoveTargetObjPos()
//    {
//        if (moveTargetObj != null)
//        {
//            moveTargetObj.transform.localPosition = Vector3.zero;
//            isVideoOpen = false;
//        }
//    }

//    bool isMoveToDefault = false;
//    Vector3 defaultTargetPos;
//    Quaternion defaultRotation;
//    public void ResetToDefaultAvatar()
//    {
//        Debug.LogError("ResetToDefaultAvatar:" + playerHead.transform.localRotation + "   :defaultRotation:" + defaultRotation);
//        if (playerHead.transform.localRotation != defaultRotation)
//        {
//            isMoveToDefault = true;

//            if (playerHead != null)
//            {
//                playerHead.transform.DOLocalRotateQuaternion(defaultRotation, 0.5f).OnComplete(() =>
//                isMoveToDefault = false
//                );
//            }

//            if (RootAnimTargetObj != null)
//            {
//                RootAnimTargetObj.transform.DOLocalRotateQuaternion(defaultRotation, 0.5f);
//            }

//            if (moveTargetObj != null)
//            {
//                moveTargetObj.transform.DOMove(defaultTargetPos, 0.5f);
//            }
//        }
//    }
//}