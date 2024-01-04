using Metaverse;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CamerasSetting : MonoBehaviour
{
    private Camera mainCam;

    private void OnEnable()
    {
        BuilderEventManager.AfterWorldOffcialWorldsInatantiated += SetCamerasSetting;
    }

    private void OnDisable()
    {
        BuilderEventManager.AfterWorldOffcialWorldsInatantiated -= SetCamerasSetting;
    }

    // Start is called before the first frame update
    void Start()
    {
        mainCam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        UniversalAdditionalCameraData mainCamData = mainCam.GetComponent<UniversalAdditionalCameraData>();
        if (mainCamData != null)
            mainCamData.renderPostProcessing = true;
        else
            Debug.Log("<color=red>UniversalAdditionalCameraData component not found.</color>");

        //SetCamerasSetting();
    }

    private void SetCamerasSetting()
    {
        ArrowManager.Instance.slfieVirtualCam.m_Lens.FarClipPlane = 600;
        UniversalAdditionalCameraData selfieCamData = ArrowManager.Instance.selfieCam.
            GetComponent<UniversalAdditionalCameraData>();
        if (selfieCamData != null)
            selfieCamData.renderPostProcessing = true;
        else
            Debug.Log("<color=red>UniversalAdditionalCameraData component not found.</color>");

        ArrowManager.Instance.selfieShootCamL.farClipPlane = 600;
        ArrowManager.Instance.selfieShootCamP.farClipPlane = 600;

        Camera freeFloatCam = AvatarManager.Instance.spawnPoint.GetComponent<PlayerControllerNew>().
            FreeFloatCamCharacterController.GetComponent<Camera>();
        freeFloatCam.farClipPlane = 600;
        UniversalAdditionalCameraData freeFloatCamData = freeFloatCam.GetComponent<UniversalAdditionalCameraData>();
        if (freeFloatCamData != null)
            freeFloatCamData.renderPostProcessing = true;
        else
            Debug.Log("<color=red>UniversalAdditionalCameraData component not found.</color>");
        
        Camera firstPersonCam = AvatarManager.Instance.spawnPoint.GetComponent<PlayerControllerNew>().
            firstPersonCameraObj.GetComponent<Camera>();
        firstPersonCam.farClipPlane = 600;
        UniversalAdditionalCameraData firstPersonCamData = firstPersonCam.GetComponent<UniversalAdditionalCameraData>();
        if (firstPersonCamData != null)
            firstPersonCamData.renderPostProcessing = true;
        else
            Debug.Log("<color=red>UniversalAdditionalCameraData component not found.</color>");
    }

}
