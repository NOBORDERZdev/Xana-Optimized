using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

public class PostProcessManager : MonoBehaviour
{
    public Camera thirdPersonCam;
    public Camera freeCam;
    public Camera firstPersonCam;
    public Camera SelfieCam;
    public List<string> m_SceneNames;
    MyBeachSelfieCam selfieCam;
    // Start is called before the first frame update
    void Start()
    {
        if (GamePlayButtonEvents.inst != null)
            GamePlayButtonEvents.inst.OnSelfieButton += OnSelfieOpen;
    }
    public void SetPostProcessing() {
        if (selfieCam == null)
        {
            selfieCam = ReferencesForGamePlay.instance.m_34player.GetComponent<MyBeachSelfieCam>();
        }
        if (ConstantsHolder.xanaConstants.isBuilderScene)
        {
            firstPersonCam.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
            thirdPersonCam.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
            freeCam.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
            if (ReferencesForGamePlay.instance.m_34player != null && selfieCam != null)
            {
                selfieCam.Selfie.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
                selfieCam.SelfieCapture.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
                selfieCam.SelfieCapturepotrait.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
                selfieCam.SelfieCapturepotrait1.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
                selfieCam.SelfieCapture_CamRender.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
                selfieCam.SelfieCapture_CamRenderPotraiat.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
            }
        }
        else
        {
            if (CheckPostProcessEnable())
            {
                firstPersonCam.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
                thirdPersonCam.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
                freeCam.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
                if (ReferencesForGamePlay.instance.m_34player != null && selfieCam != null)
                {
                    selfieCam.Selfie.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
                    selfieCam.SelfieCapture.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
                    selfieCam.SelfieCapturepotrait.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
                    selfieCam.SelfieCapturepotrait1.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
                    selfieCam.SelfieCapture_CamRender.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
                    selfieCam.SelfieCapture_CamRenderPotraiat.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
                }
            }
        }
    }
    void OnSelfieOpen()
    {
        Debug.Log("on selfie open ");
        if (ReferencesForGamePlay.instance.m_34player != null)
        {
            SetPostProcessing();
            //if (FeedEventPrefab.m_EnvName.Contains("Xana Festival") || FeedEventPrefab.m_EnvName.Contains("NFTDuel Tournament"))
            //if (CheckPostProcessEnable())
            //{
            //    selfieCam.Selfie.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
            //    selfieCam.SelfieCapture.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
            //    selfieCam.SelfieCapturepotrait.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
            //    selfieCam.SelfieCapturepotrait1.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
            //    selfieCam.SelfieCapture_CamRender.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
            //    selfieCam.SelfieCapture_CamRenderPotraiat.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
            //}
            //else if (ConstantsHolder.xanaConstants.isBuilderScene)
            //{
            //    selfieCam.Selfie.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
            //    selfieCam.SelfieCapture.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
            //    selfieCam.SelfieCapturepotrait.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
            //    selfieCam.SelfieCapturepotrait1.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
            //    selfieCam.SelfieCapture_CamRender.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
            //    selfieCam.SelfieCapture_CamRenderPotraiat.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
            //}
        }
    }


    private void OnDisable()
    {
        if (GamePlayButtonEvents.inst != null)
            GamePlayButtonEvents.inst.OnSelfieButton -= OnSelfieOpen;
    }


    public bool CheckPostProcessEnable()
    {
        if (m_SceneNames.Contains(WorldItemView.m_EnvName))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}