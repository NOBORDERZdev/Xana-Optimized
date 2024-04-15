﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FP_CamControllerMouseLook : MonoBehaviour
{

    public float mouseSensitivity = 100f;
    public Transform playerBody;

    private float xRotation = 0f;

    public PlayerController playerController;
    [Header("Gyro")]
    private float x;
    private float y;
    public GameObject onToggal, onToggal_Port;
    public GameObject offToggal, offToggal_port;
    //   public Toggle gyroToggle;
    [SerializeField] private bool isGyroOn = false;
    [SerializeField] private bool gyroEnabled;
    readonly float sensitivity = 50.0f;
    private Gyroscope gyro;

    // Mobile Device
    private Vector2 delta;
    private bool _allowSyncedControl;
    private bool _allowRotation = true;


    // Start is called before the first frame update
    void Start()
    {
        //  gyroToggle.isOn = false;
        onToggal.SetActive(false);
        onToggal_Port.SetActive(false);

        gyroEnabled = EnableGyro();
        playerBody = transform.GetComponentInParent<PlayerController>().gameObject.transform;

    }
    // Update is called once per frame
    void Update()
    {
        if (!playerController.isFirstPerson && !playerController.m_FreeFloatCam)
            return;
        if (!Application.isEditor && IsPointerOverUI() && !_allowSyncedControl)
        {
            _allowRotation = false;
            _allowSyncedControl = false;
        }
        else
        {
            _allowRotation = true;
            //_allowSyncedControl = true;
        }


#if UNITY_EDITOR
        if (!isGyroOn && _allowRotation)
        {
            float mouseX;
            float mouseY;
            mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 55f);
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            playerBody.Rotate(Vector3.up * (mouseX));
        }
#elif UNITY_IOS || UNITY_ANDROID
        if (_allowRotation)
        {
            Longtouch();
        }
#endif
        // Gyro Rotation
        if (isGyroOn && _allowRotation)
        {
            if (gyroEnabled)
            {
                GyroRotation();
            }
        }
    }
    private void FixedUpdate()
    {
        if (isGyroOn)
            return;
        if (_allowSyncedControl && _allowRotation && !playerController.m_FreeFloatCam && playerController.isFirstPerson)
        {
            MoveCamera(delta);
        }

        if (playerController.m_FreeFloatCam && _allowRotation)
        {
            MoveCameraFreeFloat();
        }
#if UNITY_EDITOR
        if (_allowRotation)
            MouseMovement();

#endif

    }


    void MouseMovement()
    {
#if UNITY_EDITOR

        // Get mouse movement
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotate the camera horizontally (around the y-axis)
        yRotation += mouseX;
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);

        // Calculate vertical rotation
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Rotate the camera vertically (around the x-axis)
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);

#endif


    }

    // Mobile Touch
    void Longtouch()
    {

        delta = Vector2.zero;
        if (playerController.horizontal != 0 && playerController.vertical != 0)//&& Input.touchCount > 0)
        {
            if (Input.touchCount > 1)
            {
                Touch t = Input.GetTouch(0);
                Touch t1 = Input.GetTouch(1);
                Touch t2 = (t.position.x > t1.position.x) ? t : t1;
                //Touch t2 = (t.position.x > 500) ? t : t1;
                if (t2.phase == TouchPhase.Moved /*&& t2.position.x > 500*/)// && (playerController.horizontal == 0 && playerController.vertical == 0))
                {
                    delta = t2.deltaPosition;
                    _allowSyncedControl = true;
                }
                else
                {
                    _allowSyncedControl = false;
                }
            }
            else if (Input.touchCount > 0)
            {
                Touch t = Input.GetTouch(0);
                if (t.phase == TouchPhase.Moved /*&& t.position.x > 500*/) // && (playerController.horizontal == 0 && playerController.vertical == 0))
                {
                    delta = Input.GetTouch(0).deltaPosition;
                    _allowSyncedControl = true;
                }
                else
                {
                    _allowSyncedControl = false;
                }
            }

        }
        else if (playerController.horizontal == 0 && playerController.vertical == 0) // && Input.touchCount > 0)
        {
            if (Input.touchCount > 0)
            {
                Touch t = Input.GetTouch(0);
                if (t.phase == TouchPhase.Moved /*&& t.position.x > 500*/) // && (playerController.horizontal == 0 && playerController.vertical == 0))
                {
                    delta = Input.GetTouch(0).deltaPosition;
                    _allowSyncedControl = true;
                }
                else
                {
                    _allowSyncedControl = false;
                }
            }
            if (Input.touchCount > 1)
            {
                Touch t = Input.GetTouch(0);
                Touch t1 = Input.GetTouch(1);
                Touch t2 = (t.position.x > t1.position.x) ? t : t1;
                //Touch t2 = (t.position.x > 500) ? t : t1;
                if (t2.phase == TouchPhase.Moved /*&& t2.position.x > 500*/)// && (playerController.horizontal == 0 && playerController.vertical == 0))
                {
                    delta = t2.deltaPosition;
                    _allowSyncedControl = true;
                }
                else
                {
                    _allowSyncedControl = false;
                }
            }
        }



    }

    private float yRotation = 0f;
    public float touchSensitivity = 0.1f;

    private void MoveCameraFreeFloat()
    {

        // Get touch count
        int touchCount = Input.touchCount;

        // Check for touch input
        if (touchCount > 0)
        {
            // Get the first touch position
            Touch touch = Input.GetTouch(0);
            Vector2 touchDelta = touch.deltaPosition;

            // Rotate the camera horizontally (around the y-axis)
            yRotation += touchDelta.x * touchSensitivity;
            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);

            // Calculate vertical rotation
            xRotation -= touchDelta.y * touchSensitivity;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            // Rotate the camera vertically (around the x-axis)
            transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        }


    }
    private void MoveCamera(Vector2 delta)
    {
        xRotation -= delta.y * 10 * PlayerCameraController.instance.lookSpeedd * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation, -90f, 55f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * (delta.x * 10 * PlayerCameraController.instance.lookSpeedd * Time.deltaTime));
    }

    public void OnToggle()
    {
        offToggal.SetActive(true);
        offToggal_port.SetActive(true);

        onToggal.SetActive(false);
        onToggal_Port.SetActive(false);
        isGyroOn = false;
    }
    public void offToggel()
    {
        offToggal.SetActive(false);
        offToggal_port.SetActive(false);

        onToggal.SetActive(true);
        onToggal_Port.SetActive(true);
        isGyroOn = true;
    }
    private bool EnableGyro()
    {
        if (SystemInfo.supportsGyroscope)
        {
            gyro = Input.gyro;
            gyro.enabled = true;
            return true;
        }
        return false;
    }
    void GyroRotation()
    {
        x = Input.gyro.rotationRate.x;
        y = Input.gyro.rotationRate.y;
        float xFiltered = FilterGyroValues(x);
        RotateUpDown(xFiltered * sensitivity);
        float yFiltered = FilterGyroValues(y);
        RotateRightLeft(yFiltered * sensitivity);
    }
    float FilterGyroValues(float axis)
    {
        if (axis < -0.1 || axis > 0.1)
        {
            return axis;
        }
        else
        {
            return 0;
        }
    }
    public void RotateUpDown(float axis)
    {
        transform.RotateAround(transform.position, transform.right, -axis * Time.deltaTime);
    }
    //rotate the camera rigt and left (y rotation)
    public void RotateRightLeft(float axis)
    {
        playerBody.RotateAround(transform.position, Vector3.up, -axis * Time.deltaTime);
    }


    public bool IsPointerOverUI()
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

}
