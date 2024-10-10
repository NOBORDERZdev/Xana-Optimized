/*InputManager.cs
* Description: Handles user input.
*/
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [Tooltip("Forward force applied to player")]
    public float force = 1250.0f;
    public float startForce = 600f;

    [Tooltip("Rotation speed of the player")]
    public float rotationSpeed = 75.0f;
    public float rotLimitAngle = 80f;

    [Tooltip("Amount of torque applied to 'lift' the player as they turn")]
    public float turnLift = 300.0f;

    [Tooltip("Amount to artificially assist the player in staying upright")]
    public float uprightAssist = 1.0f;

    public bool canRotate = false;
    private bool isTouchingCrab = false;
    public bool isTouchingGround = false;
    private UnityEngine.Gyroscope gyro;
    private Rigidbody rb;
    private float vertical;
    private float horizontal;

    Controls controls;
    private void Awake()
    {
        controls = new Controls();
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
        controls.Gameplay.Move.performed += OnMovePerformed;
        controls.Gameplay.Move.canceled += OnMovePerformed;
        force = 1250.0f;
        startForce = 600f;
    }
    private void OnDisable()
    {
        controls.Gameplay.Move.performed -= OnMovePerformed;
        controls.Gameplay.Move.canceled -= OnMovePerformed;
    }
    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        Vector2 moveInput = context.ReadValue<Vector2>();
        horizontal = moveInput.normalized.x;
        vertical = moveInput.normalized.y;
    }
    private float GetTilt()
    {
        if (!canRotate) return 0;

        float direction = 0.0f;

        if (gyro != null && gyro.enabled)
        {
            direction = (gyro.attitude * Vector3.left).z;
        }
        else
        {
            if (Input.GetKey(KeyCode.D))
                direction += 1.0f;

            if (Input.GetKey(KeyCode.A))
                direction -= 1.0f;

            // for mobile device
            direction += horizontal;

            float angleY = rb.rotation.eulerAngles.y > 180f ? rb.rotation.eulerAngles.y - 360 : rb.rotation.eulerAngles.y;

            if (angleY < -rotLimitAngle && direction < 0) direction = 0;

            if (angleY > rotLimitAngle && direction > 0) direction = 0;
        }

        return direction;
    }
    void FixedUpdate()
    {
        if (isTouchingGround)
        {
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0.0f, GetTilt() * rotationSpeed * Time.deltaTime, 0.0f));
            rb.AddRelativeTorque(GetTilt() * Vector3.back * turnLift * Time.deltaTime);
            rb.AddForce(rb.rotation * Vector3.forward * force * Time.deltaTime);

            rb.MoveRotation(Quaternion.Euler(Mathf.MoveTowardsAngle(FixAngle(rb.rotation.eulerAngles.x), 0.0f, uprightAssist * Time.deltaTime), rb.rotation.eulerAngles.y, rb.rotation.eulerAngles.z));
        }
        else
        {
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0.0f, GetTilt() * rotationSpeed * 2 * Time.deltaTime, 0.0f));
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.name == "Ground_Main_Sand")
            isTouchingGround = true;
    }
    void OnCollisionStay(Collision collision)
    {
        if (collision.collider.gameObject.name == "Ground_Main_Sand")
            isTouchingGround = true;
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.collider.gameObject.name == "Ground_Main_Sand")
            isTouchingGround = false;
    }

    private float FixAngle(float angle)
    {
        if (angle < 0.0f)
            angle += 360.0f;
        else
            while (angle > 360.0f)
                angle -= 360.0f;

        return angle;
    }

    public void OnTouchingCrab()
    {
        if (force <= 150f) force = 150;
        else force *= 0.7f;

        if (force < 150f) force = 150;
    }
    public void StopMove()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
    }
}

