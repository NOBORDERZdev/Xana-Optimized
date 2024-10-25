using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using TouchPhase = UnityEngine.TouchPhase;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class PlayerCameraController : MonoBehaviour
{
    [Space(5)]
    public float lookSpeed;
    public float lookSpeedd;
    [SerializeField]
    private CinemachineFreeLook cinemachine;
    private CinemachineFreeLook.Orbit[] originalOrbits;

    private Controls controls;
    public RectTransform freelookup;
    public bool gyroCheck = false;
    public static PlayerCameraController instance;

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
    public PlayerController playerController;

    public Transform cameraPos;

    static float pinchDistanceDelta;
    static float pinchDistance;
    const float pinchRatio = 1;
    const float minPinchDistance = 1.5f;

    [HideInInspector]
    public bool isJoystickPressed = false;
    [HideInInspector]
    public bool isRotatingScreen = false;

    CharacterBodyParts charcterBody;
    [SerializeField] GameObject pointObj;
    GameObject camRender;
    float midRigHeight, midRigRadius, topRigHeight, topRigRadius, bottomRigRadius, defaultZoomInLimit, defaultZoomOutLimit;
    [HideInInspector]
    public bool isReturn = false;

    private bool EnableRecenter = false,startRecentering = true;
    private Coroutine RecenterTime;
    private void OnEnable()
    {
        controls.Enable();
        BuilderEventManager.ChangeCameraHeight += ChangeCameraHeight;
    }
    private void OnDisable()
    {
        controls.Disable();
        BuilderEventManager.ChangeCameraHeight -= ChangeCameraHeight;

    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        controls = new Controls();
        cinemachine = this.GetComponent<CinemachineFreeLook>();
        if (gyroCheck)
            InputSystem.EnableDevice(UnityEngine.InputSystem.Gyroscope.current);
    }

    private void Start()
    {
        lookSpeedd = PlayerPrefs.GetFloat(ConstantsGod.CAMERA_SENSITIVITY, 0.75f);
        lookSpeed = PlayerPrefs.GetFloat(ConstantsGod.CAMERA_SENSITIVITY, 0.75f);
        playerController = AvatarSpawnerOnDisconnect.Instance.spawnPoint.GetComponent<PlayerController>();
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
            lookSpeed = 0.05f;
            zoomScrollVal = originalOrbits[1].m_Radius;
        }
        camRender = ReferencesForGamePlay.instance.randerCamera.gameObject;
    }

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
            if (ReferencesForGamePlay.instance.m_34player)
            {
                charcterBody = ReferencesForGamePlay.instance.m_34player.GetComponent<CharacterBodyParts>();
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
        gyroCheck = CanvusHandler.canvusHandlerInstance.isGyro;
        if (!gyroCheck)
        {
            delta = controls.Gameplay.Look.ReadValue<Vector2>();
            cinemachine.m_XAxis.Value += delta.x * 400 * lookSpeed * Time.deltaTime;
            cinemachine.m_YAxis.Value += delta.y * 5 * lookSpeed * Time.deltaTime;
        }
    }

    void Longtouch()
    {

        gyroCheck = CanvusHandler.canvusHandlerInstance.isGyro;
        delta = Vector2.zero;
        if (!gyroCheck && PlayerSelfieController.Instance.disablecamera && Input.touchCount > 0
            && !playerController.sprint)
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
        if (_allowSyncedControl && _allowRotation && !playerController.isFirstPerson)
        {
            MoveCamera(delta);            // Rotate camera on the base input
        }
        if (delta == Vector2.zero)
        {
            if (startRecentering)
            {
                cinemachine.m_RecenterToTargetHeading.m_enabled = EnableRecenter;
            }
            else
            {

                if (EnableRecenter)
                {
                    startRecentering = true;
                    if (RecenterTime != null) { StopCoroutine(RecenterTime); }
                    RecenterTime = StartCoroutine(updateRecenterTime());
                  
                }
            }

        }
        else
        {
            cinemachine.m_RecenterToTargetHeading.m_enabled = false;
            startRecentering = false;
            cinemachine.m_RecenterToTargetHeading.m_RecenteringTime = .5f;
        }

    }
    private void MoveCamera(Vector2 delta)
    {
        cinemachine.m_XAxis.Value += delta.x * 10 * lookSpeedd * Time.deltaTime;
        cinemachine.m_YAxis.Value += -delta.y * 0.08f * lookSpeedd * Time.deltaTime;
    }

    private IEnumerator updateRecenterTime()
    {
       
  
        
        yield return new WaitForSeconds(1f);
        cinemachine.m_RecenterToTargetHeading.m_RecenteringTime = 0.1f;
        RecenterTime = null;
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
        if (playerController.horizontal != 0 && playerController.vertical != 0)
            return false;
        if (playerController.jumpNow)
            return false;
        return true;
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
            // SetOrbitRadius(5, 20, 5);
            // SetOrbitHeight(20, 5, cinemachine.m_Orbits[2].m_Height);
            GamePlayUIHandler.inst.ref_PlayerControllerNew.GetComponent<CharacterController>().radius = 1.4f;
            GamePlayUIHandler.inst.ref_PlayerControllerNew.GetComponent<CharacterController>().center = new Vector3(0,2.3f,0);
            SetOrbitRadius(1.4f, 1f, 1.3f);
            SetOrbitHeight(1.5f, 3f, 1.9f);
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

    public void EnableCameraRecenter()
    {
        cinemachine.m_RecenterToTargetHeading.m_RecenteringTime = 0.1f;

        cinemachine.m_RecenterToTargetHeading.m_enabled = true;
        EnableRecenter = true;   
        
    }
    public void DisableCameraRecenter()
    {
        cinemachine.m_RecenterToTargetHeading.m_enabled = false;
        EnableRecenter = false;
    }


}