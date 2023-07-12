//using System.Text;
//using Unity.Collections;
//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;
//using UnityEngine.XR.ARFoundation;
//using UnityEngine.XR.ARSubsystems;
//using System;
//using System.Collections;

//namespace UnityEngine.XR.ARFoundation.Samples
//{
//    public class DisplayFaceInfo : MonoBehaviour
//    {
//        [SerializeField]
//        TextMeshProUGUI m_FaceInfoText;

//        [SerializeField]
//        GameObject InfoPanel;

//        public bool isTracking = false;
//        public bool lastState;

//        ARFaceManager m_FaceManager;

//        ARFaceModuleManager aRFaceModuleManager;
//        LiveVideoRoomManager liveVideoRoomManager;

//        void Start()
//        {
//            m_FaceManager = GetComponent<ARFaceManager>();

//            aRFaceModuleManager = ARFaceModuleManager.Instance;
//            liveVideoRoomManager = LiveVideoRoomManager.Instance;
//        }

//        void CheckTrakingState()
//        {
//            foreach (var face in m_FaceManager.trackables)
//            {
//                if (face.trackingState == TrackingState.Tracking)
//                {
//                    isTracking = true;
//                }
//                else
//                {
//                    isTracking = false;
//                }              
//            }

//            if (m_FaceManager.trackables.count <= 0)
//            {
//                isTracking = false;
//            }

//            //Debug.LogError("isTracking:" + isTracking + "    :lastState:" + lastState);
//            if(aRFaceModuleManager.captureViewScreen.activeSelf || aRFaceModuleManager.CreatePostScreen.activeSelf
//                || liveVideoRoomManager.videoPlayerUIScreen.activeSelf || liveVideoRoomManager.imageSelectionUIScreen.activeSelf)
//            {
//                return;
//            }

//            if (lastState != isTracking)
//            {
//                lastState = isTracking;
//                if (isTracking)
//                {
//                    m_FaceInfoText.text = TextLocalization.GetLocaliseTextByKey("Your face tracking started");
//                }
//                else 
//                { 
//                    m_FaceInfoText.text = TextLocalization.GetLocaliseTextByKey("Oops! Your face can't be detected Please go to a brighter place and look straight into the camera");
//                }
//                InfoPanel.SetActive(true);

//                InfoPopupDisable();
//            }
//        }

//        void FixedUpdate()
//        {
//            CheckTrakingState();
//        }

//        Coroutine infoPanelCo;
//        void InfoPopupDisable()
//        {
//            if(infoPanelCo != null)
//            {
//                StopCoroutine(infoPanelCo);
//            }
//            infoPanelCo = StartCoroutine(DisableInfoPanel());
//        }

//        IEnumerator DisableInfoPanel()
//        {
//            yield return new WaitForSeconds(2f);
//            InfoPanel.SetActive(false);
//        }
//    }
//}