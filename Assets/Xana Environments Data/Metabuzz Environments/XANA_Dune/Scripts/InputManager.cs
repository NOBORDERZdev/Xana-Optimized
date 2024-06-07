/*InputManager.cs
* Description: Handles user input.
*/
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    //public JoyStick joyStick;
    [Tooltip("Forward force applied to player")]
    public float force = 1250.0f;

    [Tooltip("Rotation speed of the player")]
    public float rotationSpeed = 75.0f;

    [Tooltip("Amount of torque applied to 'lift' the player as they turn")]
    public float turnLift = 750.0f;

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
        gyro = Input.gyro;

        if (PlayerPrefs.GetInt("gyro") == 1)
            gyro.enabled = true;
        else
            gyro.enabled = false;

        horizontal = ReferencesForGamePlay.instance.MainPlayerParent.GetComponent<PlayerController>().horizontal;
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
        controls.Gameplay.Move.performed += OnMovePerformed;
        controls.Gameplay.Move.canceled += OnMovePerformed;
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
        // Debug.Log("Move Input"+ moveInput);
    }
    float getTilt()
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
            //direction += horizontal;
            Debug.Log("Sohaib skating horizontal: " + horizontal);
            direction += horizontal;
            Debug.Log("Sohaib skating direction: " + direction);

        }

        return direction;
    }
    void FixedUpdate()
    {
        if (isTouchingGround)
        {
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0.0f, getTilt() * rotationSpeed * Time.deltaTime, 0.0f));
            rb.AddRelativeTorque(getTilt() * Vector3.back * turnLift * Time.deltaTime);
            rb.AddForce(rb.rotation * Vector3.forward * force * Time.deltaTime);

            rb.MoveRotation(Quaternion.Euler(Mathf.MoveTowardsAngle(FixAngle(rb.rotation.eulerAngles.x), 0.0f, uprightAssist * Time.deltaTime), rb.rotation.eulerAngles.y, rb.rotation.eulerAngles.z));
        }
        else
        {
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0.0f, getTilt() * rotationSpeed * 2 * Time.deltaTime, 0.0f));
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.tag == "Terrain")
            isTouchingGround = true;
    }
    void OnCollisionStay(Collision collision)
    {
        if (collision.collider.gameObject.tag == "Terrain")
            isTouchingGround = true;
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.collider.gameObject.tag == "Terrain")
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
        force *= 0.7f;
        Debug.Log(force);
        if (force < 150f) force = 150;
        //if (isTouchingCrab) return;
        //isTouchingCrab = true;

        //StartCoroutine(Stun(3f));
    }

    // not use, just test for stun, can erase this function
    IEnumerator Stun(float stunTime)
    {
        float curr = 0;


        Debug.Log("stun start");
        while (curr <= stunTime)
        {
            curr += Time.deltaTime;

            yield return null;
        }

        isTouchingCrab = false;
        Debug.Log("stun finish");
    }
}

