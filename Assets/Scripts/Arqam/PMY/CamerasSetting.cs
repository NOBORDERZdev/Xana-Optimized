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
        // For TPC camera
        LoadFromFile.instance.PlayerCamera.m_Lens.FarClipPlane = 630;

        // For selfie camera
        ArrowManager.Instance.slfieVirtualCam.m_Lens.FarClipPlane = 630;
        // For selfie shoot cameras
        ArrowManager.Instance.selfieShootCamL.farClipPlane = 630;
        ArrowManager.Instance.selfieShootCamP.farClipPlane = 630;

        // For free float camera
        Camera freeFloatCam = AvatarManager.Instance.spawnPoint.GetComponent<PlayerControllerNew>().
            FreeFloatCamCharacterController.GetComponent<Camera>();
        freeFloatCam.farClipPlane = 630;
        
        // For first person camera
        Camera firstPersonCam = AvatarManager.Instance.spawnPoint.GetComponent<PlayerControllerNew>().
            firstPersonCameraObj.GetComponent<Camera>();
        firstPersonCam.farClipPlane = 630;
    }

}
