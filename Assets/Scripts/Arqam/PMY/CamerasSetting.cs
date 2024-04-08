using Metaverse;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CamerasSetting : MonoBehaviour
{
    public enum SceneType { Lobby, Other };
    public SceneType sceneType;

    private void OnEnable()
    {
        BuilderEventManager.AfterPlayerInstantiated += SetCamerasSetting;
    }

    private void OnDisable()
    {
        BuilderEventManager.AfterPlayerInstantiated -= SetCamerasSetting;
    }


    private void SetCamerasSetting()
    {
        if (sceneType.Equals(SceneType.Other))
        {
            LoadFromFile.instance.PlayerCamera.m_Lens.NearClipPlane = 0.01f;
            return;
        }
        // For TPC camera
        LoadFromFile.instance.PlayerCamera.m_Lens.FarClipPlane = 630;
        LoadFromFile.instance.PlayerCamera.m_Lens.NearClipPlane = 0.01f;
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
