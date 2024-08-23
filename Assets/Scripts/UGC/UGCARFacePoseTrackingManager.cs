using DG.Tweening;
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

    public ARPoseDriver AR_PoseDriver;
    public ARFaceManager AR_FaceManager;
    public GameObject MoveTargetObj;
    public GameObject PlayerHead;
    public GameObject PlayerBody;
    public GameObject MaleHeadTarget;
    public GameObject FemaleHeadTarget;
    public GameObject MaleBodyTarget;
    public GameObject FemaleBodyTarget;
    public GameObject CameraTransform;
    public GameObject MirrorARFace;
    public GameObject MirrorARFace2;
    public SkinnedMeshRenderer MaleDFaceskinRenderer;
    public SkinnedMeshRenderer FemaleDFaceskinRenderer;

    [SerializeField] private Vector3 _headRotation;
    [SerializeField] private bool _isTracking = false;
    [SerializeField] private float _bodyRotRatio;
    private bool _lastState;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        if (MoveTargetObj != null)
        {
            defaultTargetPos = MoveTargetObj.transform.position;
        }
        //defaultRotation = RootAnimTargetObj.transform.rotation;
        defaultRotation = new Quaternion(0, 0, 0, 1f);

        Invoke(nameof(SetReference), 1f);
    }

    public void SetReference()
    {
        if (CharacterHandler.instance.activePlayerGender == AvatarGender.Male)
        {
            PlayerHead = MaleHeadTarget;
            PlayerBody = MaleBodyTarget;
        }
        else
        {
            PlayerHead = FemaleHeadTarget;
            PlayerBody = FemaleBodyTarget;
        }
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
        _headRotation.x += x;
        _headRotation.y += y;
        if (Input.GetKey(KeyCode.Z))
        {
            _headRotation.z += Time.deltaTime * 50;
        }
        if (Input.GetKey(KeyCode.X))
        {
            _headRotation.z -= Time.deltaTime * 50;
        }
        /*Vector3 bodyRot= new Vector3(0, headRotation.y/ bodyRotRatio, 0);
        playerBody.transform.rotation = Quaternion.Euler(bodyRot);*/
        PlayerHead.transform.rotation = Quaternion.Euler(_headRotation);
    }
    private void OnDisable()
    {
        ToggleFaceDetection();
    }

    public void ToggleFaceDetection()
    {
        if (AR_FaceManager == null)
            return;

        AR_FaceManager.enabled = !AR_FaceManager.enabled;

        if (AR_FaceManager.enabled)
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
        foreach (var face in AR_FaceManager.trackables)
        {
            face.gameObject.SetActive(value);
        }
    }

    void SetARPoseOnAvatar()
    {
        foreach (var face in AR_FaceManager.trackables)
        {
            if (face.trackingState == TrackingState.Tracking)
            {
                _isTracking = true;

                Vector3 headRotation;
#if UNITY_IOS
                //headRotation = new Vector3(-face.transform.rotation.eulerAngles.x, face.transform.rotation.eulerAngles.y, -face.transform.rotation.eulerAngles.z);
                headRotation = new Vector3(face.transform.rotation.eulerAngles.x, face.transform.rotation.eulerAngles.y, face.transform.rotation.eulerAngles.z);

                MirrorARFace.transform.rotation = Quaternion.Euler(headRotation);

                Vector3 faceAngle = MirrorARFace.transform.rotation.eulerAngles;

                // For y-component, use angle difference between camera and face.
                Quaternion differenceRotation = MirrorARFace.transform.rotation * Quaternion.Inverse(CameraTransform.transform.rotation);
                Vector3 differenceAngles = differenceRotation.eulerAngles;

                faceAngle.y = differenceAngles.y;
                MirrorARFace2.transform.rotation = Quaternion.Euler(faceAngle);

                float yRot = faceAngle.y / _bodyRotRatio;

                if (yRot > 100)
                {
                    yRot = 120 - yRot;
                    float tempy = 360 - yRot;
                    yRot = tempy;
                }

                Vector3 bodyRot = new Vector3(0, yRot, 0);
                PlayerBody.transform.localRotation = Quaternion.Euler(bodyRot);
                PlayerHead.transform.localRotation = Quaternion.Lerp(PlayerHead.transform.localRotation, Quaternion.Euler(MirrorARFace2.transform.rotation.eulerAngles), 10 * Time.deltaTime);
#else
                headRotation = new Vector3(face.transform.rotation.eulerAngles.x, -face.transform.rotation.eulerAngles.y, -face.transform.rotation.eulerAngles.z);
                
                float yRot=face.transform.rotation.eulerAngles.y/_bodyRotRatio;
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
                PlayerBody.transform.localRotation=Quaternion.Euler(bodyRot);
                PlayerHead.transform.localRotation = Quaternion.Lerp(PlayerHead.transform.localRotation, Quaternion.Euler(headRotation), 10 * Time.deltaTime);
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

                if (MoveTargetObj != null)
                {
                    Vector3 movePos = new Vector3(MoveTargetObj.transform.localPosition.x, MoveTargetObj.transform.localPosition.y, Mathf.Clamp(finalXRotValue, -0.15f, 0.1f));
                    MoveTargetObj.transform.localPosition = Vector3.Lerp(MoveTargetObj.transform.localPosition, movePos, 10 * Time.deltaTime);

                    //Debug.LogError("face:" + face.transform.position.z + " :finalXRotValue:" + finalXRotValue + " :face postion:" + face.transform.localPosition.z + ":LocalPos:" + moveTargetObj.transform.localPosition);
                }
            }
            else
            {
                _isTracking = false;
            }
            //playerHead.transform.localRotation = Quaternion.Euler(headRotation);
            //RootAnimTargetObj.transform.localRotation = Quaternion.Euler(headRotation);
        }

        //Debug.LogError("Count:" + m_ARFaceManager.trackables.count + "  :isTracking:" + isTracking);
        if (AR_FaceManager.trackables.count <= 0)
        {
            _isTracking = false;
        }

        if (_lastState != _isTracking)
        {
            _lastState = _isTracking;
            if (!_isTracking && !isMoveToDefault)
            {
                ResetToDefaultAvatar();
            }
        }
    }

    public void SetDefaultMoveTargetObjPos()
    {
        if (MoveTargetObj != null)
        {
            MoveTargetObj.transform.localPosition = Vector3.zero;
        }
    }

    bool isMoveToDefault = false;
    Vector3 defaultTargetPos;
    Quaternion defaultRotation;
    public void ResetToDefaultAvatar()
    {
        Debug.LogError("ResetToDefaultAvatar:" + PlayerHead.transform.localRotation + "   :defaultRotation:" + defaultRotation);
        if (PlayerHead.transform.localRotation != defaultRotation)
        {
            isMoveToDefault = true;

            if (PlayerHead != null)
            {
                PlayerHead.transform.DOLocalRotateQuaternion(defaultRotation, 0.5f).OnComplete(() =>
                isMoveToDefault = false
                );
            }

            if (MoveTargetObj != null)
            {
                MoveTargetObj.transform.DOMove(defaultTargetPos, 0.5f);
            }
        }
    }
}