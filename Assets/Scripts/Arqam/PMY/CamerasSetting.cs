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
        ArrowManager.Instance.slfieVirtualCam.m_Lens.FarClipPlane = 600;
        
        ArrowManager.Instance.selfieShootCamL.farClipPlane = 600;
        ArrowManager.Instance.selfieShootCamP.farClipPlane = 600;

        Camera freeFloatCam = AvatarManager.Instance.spawnPoint.GetComponent<PlayerControllerNew>().
            FreeFloatCamCharacterController.GetComponent<Camera>();
        freeFloatCam.farClipPlane = 600;
        
        
        Camera firstPersonCam = AvatarManager.Instance.spawnPoint.GetComponent<PlayerControllerNew>().
            firstPersonCameraObj.GetComponent<Camera>();
        firstPersonCam.farClipPlane = 600;
    }

}
