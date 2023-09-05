using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Models;
using UnityEngine;
using UnityEngine.Rendering;

public class GamificationComponentData : MonoBehaviour
{
    public static GamificationComponentData instance;

    //References for Gamification Components
    public BuildingDetect buildingDetect;
    public Volume postProcessVol;
    public RuntimeAnimatorController cameraBlurEffect;
    public GameObject specialItemParticleEffect;
    public Material hologramMaterial;
    public Shader superMarioShader;
    public Shader superMarioShader2;
    public Shader skinShader;
    public Shader cloathShader;
    public GameObject[] FootSteps;
    internal PlayerControllerNew playerControllerNew;
    internal AvatarController avatarController;
    internal CharcterBodyParts charcterBodyParts;
    internal IKMuseum ikMuseum;

    public Vector3 spawnPointPosition;
    public GameObject raycast;
    public GameObject katanaPrefab;
    public GameObject shurikenPrefab;
    public GameObject throwAimPrefab;
    public Material lineMaterial;
    public Ball ThrowBall;
    public GameObject handBall;

    public GameObject helpParentReference;
    public GameObject worldSpaceCanvas;

    public AudioSource audioSource;

    internal Vector3 Ninja_Throw_InitPosY;
    internal Vector3 Ninja_Throw_InitPosX;
    internal bool worldCameraEnable;

    public Shader proceduralRingShader;
    public Shader uberShader;
    //Orientation Changer
    public CanvasGroup landscapeCanvas;
    public CanvasGroup potraitCanvas;
    bool isPotrait = false;

    internal List<WarpFunctionComponent> warpComponentList = new List<WarpFunctionComponent>();

    public static Action WarpComponentLocationUpdate;

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        BuilderEventManager.ReSpawnPlayer += PlayerSpawnBlindfoldedDisplay;
        //ChangeOrientation
        BuilderEventManager.BuilderSceneOrientationChange += OrientationChange;
        //OnSelfiActive
        BuilderEventManager.UIToggle += UICanvasToggle;

        OrientationChange(false);
        warpComponentList.Clear();

        WarpComponentLocationUpdate += UpdateWarpFunctionData;
    }

    private void OnDisable()
    {
        BuilderEventManager.ReSpawnPlayer -= PlayerSpawnBlindfoldedDisplay;
        BuilderEventManager.BuilderSceneOrientationChange -= OrientationChange;
        BuilderEventManager.UIToggle -= UICanvasToggle;
        WarpComponentLocationUpdate -= UpdateWarpFunctionData;

    }

    public void PlayerSpawnBlindfoldedDisplay()
    {
        StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(0.2f);
        playerControllerNew.transform.position = spawnPointPosition;
        yield return new WaitForSeconds(0.3f);
        Physics.IgnoreLayerCollision(9, 22, false);

        Debug.Log("Blindfolded spawned");
    }

    #region OrientationChange
    void OrientationChange(bool orientation)
    {
        StartCoroutine(ChangeOrientation(orientation));
    }

    IEnumerator ChangeOrientation(bool orientation)
    {
        isPotrait = orientation;
        DisableUICanvas();
        yield return new WaitForSeconds(0.1f);
        if (isPotrait)
        {
            potraitCanvas.DOFade(1, 0.5f);
            potraitCanvas.blocksRaycasts = true;
            potraitCanvas.interactable = true;
        }
        else
        {
            landscapeCanvas.DOFade(1, 0.5f);
            landscapeCanvas.blocksRaycasts = true;
            landscapeCanvas.interactable = true;
        }

        yield return new WaitForSeconds(0.5f);

        BuilderEventManager.PositionUpdateOnOrientationChange?.Invoke();
    }

    void UICanvasToggle(bool state)
    {
        if (state)
        {
            DisableUICanvas();
        }
        else
            StartCoroutine(ChangeOrientation(isPotrait));
    }

    void DisableUICanvas()
    {
        landscapeCanvas.DOKill();
        landscapeCanvas.alpha = 0;
        landscapeCanvas.blocksRaycasts = false;
        landscapeCanvas.interactable = false;
        potraitCanvas.DOKill();
        potraitCanvas.alpha = 0;
        potraitCanvas.blocksRaycasts = false;
        potraitCanvas.interactable = false;
    }
    #endregion

    //    void UpdateWarpFunctionData()
    //    {
    //        foreach (WarpFunctionComponent warpFunctionComponent1 in warpComponentList)
    //        {
    //            foreach (WarpFunctionComponent warpFunctionComponent2 in warpComponentList)
    //            {
    //                if (warpFunctionComponent1 == warpFunctionComponent2)
    //                    continue;

    //                if(warpFunctionComponent1.warpFunctionComponentData.isWarpPortalStart)
    //                {
    //                    string startKey = warpFunctionComponent1.warpFunctionComponentData.warpPortalStartKeyValue;

    //if (startKey == warpFunctionComponent2.warpFunctionComponentData.warpPortalEndKeyValue)
    //                    {
    //                        PortalSystemEndPoint  portalSystemEndPoint= warpFunctionComponent1.warpFunctionComponentData.warpPortalDataEndPoint.Find(x => x.indexPortalEndKey == startKey);
    //                        portalSystemEndPoint.portalEndLocation = warpFunctionComponent2.transform.localPosition;
    //                        PortalSystemStartPoint portalSystemStartPoint = warpFunctionComponent2.warpFunctionComponentData.warpPortalDataStartPoint.Find(x => x.indexPortalStartKey == startKey);
    //                        portalSystemStartPoint.portalStartLocation = warpFunctionComponent1.transform.localPosition;
    //                    }
    //                }
    //                else
    //                {
    //                    string endKey = warpFunctionComponent1.warpFunctionComponentData.warpPortalEndKeyValue;

    //                    if (endKey == warpFunctionComponent2.warpFunctionComponentData.warpPortalStartKeyValue)
    //                    {
    //                        PortalSystemEndPoint portalSystemEndPoint = warpFunctionComponent2.warpFunctionComponentData.warpPortalDataEndPoint.Find(x => x.indexPortalEndKey == endKey);
    //                        portalSystemEndPoint.portalEndLocation = warpFunctionComponent1.transform.localPosition;
    //                        PortalSystemStartPoint portalSystemStartPoint = warpFunctionComponent1.warpFunctionComponentData.warpPortalDataStartPoint.Find(x => x.indexPortalStartKey == endKey);
    //                        portalSystemStartPoint.portalStartLocation = warpFunctionComponent2.transform.localPosition;
    //                    }
    //                }
    //            }
    //        }
    //    }

    void UpdateWarpFunctionData()
    {
        foreach (WarpFunctionComponent warpFunctionComponent1 in warpComponentList)
        {
            foreach (WarpFunctionComponent warpFunctionComponent2 in warpComponentList)
            {
                if (warpFunctionComponent1 == warpFunctionComponent2)
                    continue;

                WarpFunctionComponentData data1 = warpFunctionComponent1.warpFunctionComponentData;
                WarpFunctionComponentData data2 = warpFunctionComponent2.warpFunctionComponentData;

                if (data1.isWarpPortalStart)
                {
                    string startKey = data1.warpPortalStartKeyValue;

                    if (startKey == data2.warpPortalEndKeyValue)
                    {
                        Vector3 endPoint = warpFunctionComponent2.transform.position;
                        endPoint.y = warpFunctionComponent2.GetComponent<XanaItem>().m_renderer.bounds.extents.y + 2;
                        UpdateEndPortalLocations(data1.warpPortalDataEndPoint, startKey, endPoint);
                        Vector3 startPoint = warpFunctionComponent1.transform.position;
                        startPoint.y = warpFunctionComponent1.GetComponent<XanaItem>().m_renderer.bounds.extents.y + 2;
                        UpdateStartPortalLocations(data2.warpPortalDataStartPoint, startKey, startPoint);
                    }
                }
                else
                {
                    string endKey = data1.warpPortalEndKeyValue;
                    if (endKey == data2.warpPortalStartKeyValue)
                    {
                        Vector3 endPoint = warpFunctionComponent1.transform.position;
                        endPoint.y = warpFunctionComponent1.GetComponent<XanaItem>().m_renderer.bounds.extents.y + 2;
                        UpdateEndPortalLocations(data2.warpPortalDataEndPoint, endKey, endPoint);
                        Vector3 startPoint = warpFunctionComponent2.transform.position;
                        startPoint.y = warpFunctionComponent2.GetComponent<XanaItem>().m_renderer.bounds.extents.y + 2;
                        UpdateStartPortalLocations(data1.warpPortalDataStartPoint, endKey, startPoint);
                    }
                }
            }
        }
    }

    void UpdateStartPortalLocations(List<PortalSystemStartPoint> endPoints, string key, Vector3 location)
    {
        PortalSystemStartPoint portalSystemEndPoint = endPoints.Find(x => x.indexPortalStartKey == key);
        portalSystemEndPoint.portalStartLocation = location;
    }

    void UpdateEndPortalLocations(List<PortalSystemEndPoint> endPoints, string key, Vector3 location)
    {
        PortalSystemEndPoint portalSystemEndPoint = endPoints.Find(x => x.indexPortalEndKey == key);
        portalSystemEndPoint.portalEndLocation = location;
    }
}