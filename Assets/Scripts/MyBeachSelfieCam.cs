using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MyBeachSelfieCam : MonoBehaviour
{
    public GameObject Selfie;
    public Camera SelfieCapture;
    public Camera SelfieCapturepotrait;
    public Camera SelfieCapturepotrait1;
    public GameObject SelfieCapture_CamRender;
    public GameObject SelfieCapture_CamRenderPotraiat;


    public List<string> m_SceneNames;
    // Start is called before the first frame update

    private void Start()
    {
        if (ReferencesForGamePlay.instance.MainPlayerParent.GetComponent<PlayerController>().animator)
        {
            RuntimeAnimatorController animator = ReferencesForGamePlay.instance.MainPlayerParent.GetComponent<PlayerController>().animator.runtimeAnimatorController;
            EmoteAnimationHandler.Instance.controller = animator;
        }
        SelfieCapturePP();
    }
    public void SelfieCapturePP() {
        if (CheckPostProcessEnable())
        {
            Selfie.GetComponent<Camera>().GetUniversalAdditionalCameraData().renderPostProcessing = true;
            SelfieCapture.GetUniversalAdditionalCameraData().renderPostProcessing = true;
            SelfieCapturepotrait.GetUniversalAdditionalCameraData().renderPostProcessing = true;
            SelfieCapturepotrait1.GetUniversalAdditionalCameraData().renderPostProcessing = true;
        }
    }


    bool CheckPostProcessEnable()
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
