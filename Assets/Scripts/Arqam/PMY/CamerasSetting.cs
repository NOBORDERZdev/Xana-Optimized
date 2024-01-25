using Metaverse;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CamerasSetting : MonoBehaviour
{

    private void OnEnable()
    {
        BuilderEventManager.AfterWorldOffcialWorldsInatantiated += SetCamerasSetting;
    }

    private void OnDisable()
    {
        BuilderEventManager.AfterWorldOffcialWorldsInatantiated -= SetCamerasSetting;
    }

    
    private void SetCamerasSetting()
    {
        ArrowManager.Instance.slfieVirtualCam.m_Lens.FarClipPlane = 620;
        
        ArrowManager.Instance.selfieShootCamL.farClipPlane = 620;
        ArrowManager.Instance.selfieShootCamP.farClipPlane = 620;

        Camera freeFloatCam = AvatarManager.Instance.spawnPoint.GetComponent<PlayerControllerNew>().
            FreeFloatCamCharacterController.GetComponent<Camera>();
        freeFloatCam.farClipPlane = 620;
        
        
        Camera firstPersonCam = AvatarManager.Instance.spawnPoint.GetComponent<PlayerControllerNew>().
            firstPersonCameraObj.GetComponent<Camera>();
        firstPersonCam.farClipPlane = 620;
    }

}
