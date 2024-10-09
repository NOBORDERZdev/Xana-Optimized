using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using TouchPhase = UnityEngine.TouchPhase;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using PhysicsCharacterController;
using UFE3D;

public class XANAPartyCameraController : MonoBehaviour
{
    public enum OrientationType { Landscape, Portrait };
    public OrientationType orientationType;
    [Space(5)]
    float _editorLookSpeed = 0.01f;
    float _touchLookSpeed = 0.6f;
    [SerializeField]
    public CinemachineFreeLook cinemachine;
    private CinemachineFreeLook.Orbit[] originalOrbits;

    public Controls controls;
    public RectTransform freelookup;
    // public bool gyroCheck = false;
    public static XANAPartyCameraController instance;

    private float zoomScrollVal = 0;                // for editor testing only
    public float editorSensitivity = 0.05f;          // for editor testing only
    public bool lockRotation;                       // for editor testing only

    [Header("----------Zoom Section----------")]
    public float zoomInLimit = -2f;
    public float zoomOutLimit = 3.2f;
    public float zoomSensitivity = 5f;
    private float zoomFactor = 0f;
    private bool zoomInZoomOut = false;
    bool ZoomStarted = false;
    [HideInInspector]
    public bool _allowSyncedControl;
    public bool _allowRotation = true;

    [HideInInspector]
    public int m_PressCounter;
    //** Temp Variables
    float m_TempDistance;
    private Vector2 delta;
    public CharacterManager playerController;

    public Transform cameraPos;

    static float pinchDistanceDelta;
    static float pinchDistance;
    const float pinchRatio = 1;
    const float minPinchDistance = 1.5f;

    [HideInInspector]
    public bool isJoystickPressed = false;
    [HideInInspector]
    public bool isRotatingScreen = false;

    CharacterManager charcterBody;
    [SerializeField] GameObject pointObj;
    GameObject camRender;
    float midRigHeight, midRigRadius, topRigHeight, topRigRadius, bottomRigRadius, defaultZoomInLimit, defaultZoomOutLimit;
    [HideInInspector]
    public bool isReturn = false;

    private void OnEnable()
    {
        controls.Enable();
        //  ScreenOrientationManager.switchOrientation += SwitchOrientation;
        // BuilderEventManager.ChangeCameraHeight += ChangeCameraHeight;
    }
    private void OnDisable()
    {
        controls.Disable();
        // ScreenOrientationManager.switchOrientation -= SwitchOrientation;
        // BuilderEventManager.ChangeCameraHeight -= ChangeCameraHeight;

    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        controls = new Controls();
        cinemachine = this.GetComponent<CinemachineFreeLook>();
        //if (gyroCheck)
        //    InputSystem.EnableDevice(UnityEngine.InputSystem.Gyroscope.current);
    }

    private void Start()
    {
        //_touchLookSpeed = PlayerPrefs.GetFloat(ConstantsGod.CAMERA_SENSITIVITY, 0.75f);
        // _editorLookSpeed = PlayerPrefs.GetFloat(ConstantsGod.CAMERA_SENSITIVITY, 0.75f);
        //playerController = AvatarSpawnerOnDisconnect.Instance.spawnPoint.GetComponent<CharacterManager>();
        controls.Gameplay.SecondaryTouchContact.started += _ => ZoomStart();
        controls.Gameplay.SecondaryTouchContact.canceled += _ => ZoomEnd();
        cinemachine.m_BindingMode = CinemachineTransposer.BindingMode.LockToTargetOnAssign;

        originalOrbits = new CinemachineFreeLook.Orbit[cinemachine.m_Orbits.Length];
        midRigHeight = cinemachine.m_Orbits[1].m_Height;
        topRigHeight = cinemachine.m_Orbits[0].m_Height;
        midRigRadius = cinemachine.m_Orbits[1].m_Radius;
        topRigRadius = cinemachine.m_Orbits[0].m_Radius;
        bottomRigRadius = cinemachine.m_Orbits[2].m_Radius;
        defaultZoomInLimit = zoomInLimit;
        defaultZoomOutLimit = zoomOutLimit;
        originalOrbits[1].m_Radius = cinemachine.m_Orbits[1].m_Radius;    // get the radius of middle rig
        if (Application.isEditor)
        {
            //lookSpeed = 0.05f;
            zoomScrollVal = originalOrbits[1].m_Radius;
        }
        camRender = ReferencesForGamePlay.instance.randerCamera.gameObject;
    }

    //void SwitchOrientation()
    //{
    //    if (orientationType.Equals(OrientationType.Landscape))
    //        orientationType = OrientationType.Portrait;
    //    else if (orientationType.Equals(OrientationType.Portrait))
    //        orientationType = OrientationType.Landscape;
    //}

    public void AllowControl()
    {
        _allowRotation = true;
        controls.Enable();
    }
    public void DisAllowControl()
    {
        controls.Disable();
        _allowRotation = false;
        _allowSyncedControl = false;
    }

    private void Update()
    {
        if (isReturn) return;

        _allowRotation = true;

        if (!Application.isEditor && IsPointerOverUIObject() && !_allowSyncedControl)
        {
            _allowRotation = false;
        }

        if (playerController == null)
        {
            return;
        }

        if (Application.isEditor)
        {
            if (_allowRotation)
            {
                if (!lockRotation && Input.GetMouseButton(0))
                {
                    CameraControls_Editor();
                }
            }
            //if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
            //{
            //    zoomScrollVal += originalOrbits[1].m_Radius + editorSensitivity;
            //    zoomScrollVal = Mathf.Clamp(zoomScrollVal, zoomInLimit, zoomOutLimit);
            //    cinemachine.m_Orbits[1].m_Radius = zoomScrollVal;
            //}
            //else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
            //{
            //    zoomScrollVal -= originalOrbits[1].m_Radius - editorSensitivity;
            //    zoomScrollVal = Mathf.Clamp(zoomScrollVal, zoomInLimit, zoomOutLimit);
            //    cinemachine.m_Orbits[1].m_Radius -= originalOrbits[1].m_Radius - editorSensitivity;
            //}
        }
        else if (!Application.isEditor)
        {
            if (_allowRotation)
            {
                if (!zoomInZoomOut)
                    CameraControls_Mobile();        // use for cam rotation using touch input
                ZoomDetection();
            }
        }
        CameraPlayerMeshCollosionFind();
    }

    /// <summary>
    /// To check is camera in player mesh
    /// </summary>
    void CameraPlayerMeshCollosionFind()
    {
        if (charcterBody == null || pointObj == null)
        {
            if (GameplayEntityLoader.instance.PenguinPlayer)
            {
                charcterBody = GameplayEntityLoader.instance.PenguinPlayer.GetComponent<CharacterManager>();
                // pointObj = charcterBody.Body.gameObject;
            }
            else
            {
                return;
            }
        }

        float dist = Vector3.Distance(camRender.transform.position, pointObj.transform.position);
        if (dist < 0.01f)
        {
            charcterBody.HidePlayer();
        }
        else
        {
            charcterBody.ShowPlayer();
        }
    }

    void CameraControls_Editor()
    {
        //gyroCheck = CanvusHandler.canvusHandlerInstance.isGyro;
        delta = controls.Gameplay.Look.ReadValue<Vector2>();
        cinemachine.m_XAxis.Value += delta.x * 400 * _editorLookSpeed * Time.deltaTime;
        cinemachine.m_YAxis.Value += delta.y * 5 * _editorLookSpeed * Time.deltaTime;
    }

    void Longtouch()
    {

        // gyroCheck = CanvusHandler.canvusHandlerInstance.isGyro;
        delta = Vector2.zero;
        if (/*!gyroCheck && */PlayerSelfieController.Instance.disablecamera && Input.touchCount > 0
           /* && !playerController.sprint*/)
        {
            if (!isJoystickPressed)
            {
                if (Input.touchCount > 0)
                {
                    OneFingureTouch();
                }
                if (Input.touchCount > 1)
                    TwoFingureTouch();
            }
            else if (isJoystickPressed)
            {
                Touch t = Input.GetTouch(0);
                Touch t1 = new Touch();
                if (Input.touchCount > 1)
                    t1 = Input.GetTouch(1);

                if (isRotatingScreen)     // screen is already rotation before joystick down
                {
                    // ignore 2nd touch that will be joystick touch
                    if (t.phase == TouchPhase.Moved /*&& t.position.x > 500*/)
                    {
                        delta = Input.GetTouch(0).deltaPosition;
                        _allowSyncedControl = true;
                    }
                    else
                    {
                        _allowSyncedControl = false;
                    }
                }
                else if (!isRotatingScreen)
                {
                    // ignore 1st touch that will be joystick touch
                    if (t1.phase == TouchPhase.Moved /*&& t1.position.x > 500*/)
                    {
                        delta = t1.deltaPosition;
                        _allowSyncedControl = true;
                    }
                    else
                    {
                        _allowSyncedControl = false;
                    }
                }
            }
        }
    }
    void OneFingureTouch()
    {
        Touch t = Input.GetTouch(0);
        if (t.phase == TouchPhase.Moved /*&& t.position.x > 500*/)
        {
            delta = Input.GetTouch(0).deltaPosition;
            _allowSyncedControl = true;
        }
        else
        {
            _allowSyncedControl = false;
        }
    }
    void TwoFingureTouch()
    {
        Touch t = Input.GetTouch(0);
        Touch t1 = Input.GetTouch(1);
        Touch t2 = (t.position.x > t1.position.x) ? t : t1;

        if (t2.phase == TouchPhase.Moved /*&& t2.position.x > 500*/)
        {
            delta = t2.deltaPosition;
            _allowSyncedControl = true;
        }
        else
        {
            _allowSyncedControl = false;
        }
    }

    private void FixedUpdate()
    {
        if (_allowSyncedControl && _allowRotation/* && !playerController.isFirstPerson*/)
        {
            MoveCamera(delta);            // Rotate camera on the base input
        }
    }
    private void MoveCamera(Vector2 delta)
    {
        cinemachine.m_XAxis.Value += delta.x * 10 * _touchLookSpeed * Time.deltaTime;
        cinemachine.m_YAxis.Value += -delta.y * 0.08f * _touchLookSpeed * Time.deltaTime;
    }
    void CameraControls_Mobile()
    {
        Longtouch();
    }
    private void ZoomStart()
    {
        ZoomStarted = true;
    }
    private void ZoomEnd()
    {
        ZoomStarted = false;
    }
    void ZoomDetection()
    {
        if (!CheckCanZoom())
            return;
        if (m_PressCounter != 0 || isJoystickPressed) return;
        if (Input.touchCount == 2)
        {
            zoomInZoomOut = true;
            _allowSyncedControl = false;

            pinchDistance = pinchDistanceDelta = 0;

            Touch l_TouchOne = Input.GetTouch(0);
            Touch l_TouchTwo = Input.GetTouch(1);
            if (l_TouchOne.phase == TouchPhase.Moved && l_TouchTwo.phase == TouchPhase.Moved)
            {
                float l_Distance = Vector3.Distance(l_TouchOne.position, l_TouchTwo.position);
                float l_SpeedOne = (l_TouchOne.deltaPosition / l_TouchOne.deltaTime).magnitude;
                float l_SpeedTwo = (l_TouchTwo.deltaPosition / l_TouchTwo.deltaTime).magnitude;
                pinchDistance = Vector2.Distance(l_TouchOne.position, l_TouchTwo.position); // getting distance between two fingures. 
                float prevDistance = Vector2.Distance(l_TouchOne.position - l_TouchOne.deltaPosition, l_TouchTwo.position - l_TouchTwo.deltaPosition); // previuos distance between fingures.
                pinchDistanceDelta = pinchDistance - prevDistance;

                if (Mathf.Abs(pinchDistanceDelta) > minPinchDistance)//if it's greater than a minimum threshold, it's a pinch!
                {
                    pinchDistanceDelta *= pinchRatio;
                    if (l_SpeedOne > 100 && l_SpeedTwo > 100) // old value is 90 
                    {
                        if (l_Distance > m_TempDistance)
                            zoomFactor -= Time.deltaTime * pinchRatio * zoomSensitivity;

                        else if (l_Distance < m_TempDistance)
                            zoomFactor += Time.deltaTime * pinchRatio * zoomSensitivity;

                        zoomFactor = Mathf.Clamp(zoomFactor, zoomInLimit, zoomOutLimit);
                        cinemachine.m_Orbits[1].m_Radius = originalOrbits[1].m_Radius + zoomFactor;
                    }
                }
                else
                {
                    pinchDistance = pinchDistanceDelta = 0;
                    zoomInZoomOut = false;
                }

                m_TempDistance = l_Distance;
            }
        }
        else
            zoomInZoomOut = false;
    }

    bool CheckCanZoom()
    {
        //if (playerController.horizontal != 0 && playerController.vertical != 0)
        //    return false;
        //if (playerController.jumpNow)
        //    return false;
        //return true;
        return false;
    }


    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        for (int i = 0; i < results.Count; i++)
        {
            if (results[i].gameObject.layer == LayerMask.NameToLayer("NFTDisplayPanel") || results[i].gameObject.layer == LayerMask.NameToLayer("Ignore Raycast"))
            {
                ////Debug.Log("Object is hover===" + results[i].gameObject.name);
                return true;
            }
        }
        return false;
    }

    //Code for the builder world when triggering Assets Changer (Avatar Changer component)
    void ChangeCameraHeight(bool changeState)
    {
        if (changeState)
        {
            //Set zoom in-out value for Asset Changer Avatar
            zoomInLimit = 15;
            zoomOutLimit = 100;

            SetOrbitRadius(5, 20, 5);
            SetOrbitHeight(20, 5, cinemachine.m_Orbits[2].m_Height);
        }
        else
        {
            SetOrbitRadius(topRigRadius, midRigRadius, bottomRigRadius);
            SetOrbitHeight(topRigHeight, midRigHeight, cinemachine.m_Orbits[2].m_Height);

            //Reset zoom in-out value
            zoomInLimit = defaultZoomInLimit;
            zoomOutLimit = defaultZoomOutLimit;
        }
    }

    void SetOrbitRadius(float radius1, float radius2, float radius3)
    {
        cinemachine.m_Orbits[0].m_Radius = radius1;
        cinemachine.m_Orbits[1].m_Radius = radius2;
        cinemachine.m_Orbits[2].m_Radius = radius3;
    }

    void SetOrbitHeight(float height1, float height2, float height3)
    {
        cinemachine.m_Orbits[0].m_Height = height1;
        cinemachine.m_Orbits[1].m_Height = height2;
        cinemachine.m_Orbits[2].m_Height = height3;
    }

    public void SetReference(GameObject player, GameObject point)
    {
        playerController = player.GetComponent<CharacterManager>();
        pointObj = point;
    }

}



