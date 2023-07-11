//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.XR.ARFoundation;

//[RequireComponent(typeof(ARFaceManager))]
//public class ARFaceTrackManager : MonoBehaviour
//{
//    ARFaceManager m_ARFaceManager;

//    void Awake()
//    {
//        m_ARFaceManager = GetComponent<ARFaceManager>();
//    }

//    private void OnDisable()
//    {
//        ToggleFaceDetection();
//    }

//    public void ToggleFaceDetection()
//    {
//        m_ARFaceManager.enabled = !m_ARFaceManager.enabled;
//        Debug.LogError("ToggleFaceDetection:" + m_ARFaceManager.enabled);
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
//            face.gameObject.SetActive(value);
//    }
//}