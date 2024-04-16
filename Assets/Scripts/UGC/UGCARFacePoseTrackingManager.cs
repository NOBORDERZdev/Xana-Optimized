﻿using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARFoundation.Samples;
using UnityEngine.XR.ARSubsystems;


public class UGCARFacePoseTrackingManager : MonoBehaviour
{
    public static UGCARFacePoseTrackingManager Instance;
    public bool isTracking = false;
    bool lastState;
    public ARPoseDriver _aRPoseDriver;
    public GameObject moveTargetObj;
    public GameObject playerHead;
    public GameObject playerBody;
    public GameObject cameraTransform;
    public GameObject mirrorARFace;
    public GameObject mirrorARFace2;
    public Vector3 headRotation;
    public float bodyRotRatio;
    public SkinnedMeshRenderer maleDFaceskinRenderer;
    public SkinnedMeshRenderer feMaleDFaceskinRenderer;
    public ARFaceManager m_ARFaceManager;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        if (moveTargetObj != null)
        {
            defaultTargetPos = moveTargetObj.transform.position;
        }
        //defaultRotation = RootAnimTargetObj.transform.rotation;
        defaultRotation = new Quaternion(0, 0, 0, 1f);

        playerHead= CharacterHandler.instance.GetActiveAvatarData().avatar_face.gameObject;
        playerBody = CharacterHandler.instance.GetActiveAvatarData().avatar_body.gameObject;
    }

    private void Update()
    {
        SetARPoseOnAvatar();
#if UNITY_EDITOR
        RotatePlayerHeadManually();
#endif
    }

    public float rotationSpeed = 1.0f;
    public void RotatePlayerHeadManually()
    {
        float x = Input.GetAxis("Vertical") * rotationSpeed;
        float y = Input.GetAxis("Horizontal") * rotationSpeed;
        headRotation.x += x;
        headRotation.y += y;
        if (Input.GetKey(KeyCode.Z))
        {
            headRotation.z += Time.deltaTime * 50;
        }
        if (Input.GetKey(KeyCode.X))
        {
            headRotation.z -= Time.deltaTime * 50;
        }
        /*Vector3 bodyRot= new Vector3(0, headRotation.y/ bodyRotRatio, 0);
        playerBody.transform.rotation = Quaternion.Euler(bodyRot);*/
        playerHead.transform.rotation = Quaternion.Euler(headRotation);
    }
    private void OnDisable()
    {
        ToggleFaceDetection();
    }

    public void ToggleFaceDetection()
    {
        if (m_ARFaceManager == null)
            return;

        m_ARFaceManager.enabled = !m_ARFaceManager.enabled;

        if (m_ARFaceManager.enabled)
        {
            SetAllPlanesActive(true);
        }
        else
        {
            SetAllPlanesActive(false);
        }
    }

    void SetAllPlanesActive(bool value)
    {
        foreach (var face in m_ARFaceManager.trackables)
        {
            face.gameObject.SetActive(value);
        }
    }

    void SetARPoseOnAvatar()
    {
        foreach (var face in m_ARFaceManager.trackables)
        {
            if (face.trackingState == TrackingState.Tracking)
            {
                isTracking = true;

                Vector3 headRotation;
#if UNITY_IOS
                //headRotation = new Vector3(-face.transform.rotation.eulerAngles.x, face.transform.rotation.eulerAngles.y, -face.transform.rotation.eulerAngles.z);
                headRotation = new Vector3(face.transform.rotation.eulerAngles.x, face.transform.rotation.eulerAngles.y, face.transform.rotation.eulerAngles.z);

                mirrorARFace.transform.rotation = Quaternion.Euler(headRotation);

                Vector3 faceAngle = mirrorARFace.transform.rotation.eulerAngles;

                // For y-component, use angle difference between camera and face.
                Quaternion differenceRotation = mirrorARFace.transform.rotation * Quaternion.Inverse(cameraTransform.transform.rotation);
                Vector3 differenceAngles = differenceRotation.eulerAngles;

                faceAngle.y = differenceAngles.y;
                mirrorARFace2.transform.rotation = Quaternion.Euler(faceAngle);

                float yRot = faceAngle.y / bodyRotRatio;

                if (yRot>100)
                {
                    yRot = 120 - yRot;
                    float tempy=360 - yRot;
                    yRot = tempy;
                }

                Vector3 bodyRot = new Vector3(0, yRot, 0);
                playerBody.transform.localRotation=Quaternion.Euler(bodyRot);
                playerHead.transform.localRotation = Quaternion.Lerp(playerHead.transform.localRotation, Quaternion.Euler(mirrorARFace2.transform.rotation.eulerAngles), 10 * Time.deltaTime);
#else
                headRotation = new Vector3(face.transform.rotation.eulerAngles.x, -face.transform.rotation.eulerAngles.y, -face.transform.rotation.eulerAngles.z);
                
                float yRot=face.transform.rotation.eulerAngles.y/bodyRotRatio;
                if (yRot>100)
                {
                    yRot = 120 - yRot;
                }
                else
                {
                    float tempy=360-yRot;
                    yRot = tempy;
                }

                Vector3 bodyRot = new Vector3(0,yRot,0);
                playerBody.transform.localRotation=Quaternion.Euler(bodyRot);
                playerHead.transform.localRotation = Quaternion.Lerp(playerHead.transform.localRotation, Quaternion.Euler(headRotation), 10 * Time.deltaTime);
#endif

                float finalXRotValue = 0;

                if (face.transform.position.z <= 0.35f)
                {
                    finalXRotValue = -(0.4f - face.transform.position.z);
                }
                else if (face.transform.position.z > 0.6f)
                {
                    //finalXRotValue = (face.transform.position.z - 0.6f);
                }

                if (moveTargetObj != null)
                {
                    Vector3 movePos = new Vector3(moveTargetObj.transform.localPosition.x, moveTargetObj.transform.localPosition.y, Mathf.Clamp(finalXRotValue, -0.15f, 0.1f));
                    moveTargetObj.transform.localPosition = Vector3.Lerp(moveTargetObj.transform.localPosition, movePos, 10 * Time.deltaTime);

                    //Debug.LogError("face:" + face.transform.position.z + " :finalXRotValue:" + finalXRotValue + " :face postion:" + face.transform.localPosition.z + ":LocalPos:" + moveTargetObj.transform.localPosition);
                }
            }
            else
            {
                isTracking = false;
            }
            //playerHead.transform.localRotation = Quaternion.Euler(headRotation);
            //RootAnimTargetObj.transform.localRotation = Quaternion.Euler(headRotation);
        }

        //Debug.LogError("Count:" + m_ARFaceManager.trackables.count + "  :isTracking:" + isTracking);
        if (m_ARFaceManager.trackables.count <= 0)
        {
            isTracking = false;
        }

        if (lastState != isTracking)
        {
            lastState = isTracking;
            if (!isTracking && !isMoveToDefault)
            {
                ResetToDefaultAvatar();
            }
        }
    }

    public void SetDefaultMoveTargetObjPos()
    {
        if (moveTargetObj != null)
        {
            moveTargetObj.transform.localPosition = Vector3.zero;
        }
    }

    bool isMoveToDefault = false;
    Vector3 defaultTargetPos;
    Quaternion defaultRotation;
    public void ResetToDefaultAvatar()
    {
        Debug.LogError("ResetToDefaultAvatar:" + playerHead.transform.localRotation + "   :defaultRotation:" + defaultRotation);
        if (playerHead.transform.localRotation != defaultRotation)
        {
            isMoveToDefault = true;

            if (playerHead != null)
            {
                playerHead.transform.DOLocalRotateQuaternion(defaultRotation, 0.5f).OnComplete(() =>
                isMoveToDefault = false
                );
            }

            if (moveTargetObj != null)
            {
                moveTargetObj.transform.DOMove(defaultTargetPos, 0.5f);
            }
        }
    }
}