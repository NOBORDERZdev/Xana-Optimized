using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR;

public class VRPlayerController : MonoBehaviour
{
    public RectTransform joystickHandle; // Reference to the UI joystick handle's RectTransform
    public float joystickRadius = 50f;   // The maximum radius for the joystick handle movement

    private JoyStickIssue onScreenJoystick; // Reference to the JoyStickIssue script
    private Vector2 movementInput;
    private bool isPointerDown = false;

    private void Start()
    {
        // Automatically find and assign the JoyStickIssue script
        onScreenJoystick = FindObjectOfType<JoyStickIssue>();

        if (onScreenJoystick == null)
        {
            Debug.LogError("JoyStickIssue script not found in the scene. Please ensure it is added to a GameObject.");
        }
    }

    private void Update()
    {
        if (onScreenJoystick == null) return;
        UpdateJoystickHandle();
        CheckButtonPress();
    }

    private void UpdateJoystickHandle()
    {
        // Get the input devices for the right hand
        var inputDevices = new List<UnityEngine.XR.InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, inputDevices);

        if (inputDevices.Count > 0)
        {
            UnityEngine.XR.InputDevice rightHandDevice = inputDevices[0];

            // Get the primary2DAxis (joystick) value
            if (rightHandDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out movementInput))
            {
                // Calculate the new position for the joystick handle
                Vector2 joystickPosition = movementInput * joystickRadius;

                // Update the joystick handle's anchored position
                joystickHandle.anchoredPosition = joystickPosition;

                // Simulate the on-screen joystick input
                SimulateJoystick(movementInput);
            }
            else
            {
                // Reset joystick handle position if no input is detected
                joystickHandle.anchoredPosition = Vector2.zero;
                SimulateJoystick(Vector2.zero);
            }
        }
        else
        {
            // Reset joystick handle position if no device is detected
            joystickHandle.anchoredPosition = Vector2.zero;
            SimulateJoystick(Vector2.zero);
        }
    }

    private void SimulateJoystick(Vector2 input)
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = RectTransformUtility.WorldToScreenPoint(null, joystickHandle.position)
        };

        if (input.magnitude > 0.1f) // Adjust threshold as needed
        {
            if (!isPointerDown)
            {
                onScreenJoystick.OnPointerDown(pointerEventData);
                isPointerDown = true;
            }
            onScreenJoystick.OnDrag(pointerEventData);
        }
        else
        {
            if (isPointerDown)
            {
                onScreenJoystick.OnPointerUp(pointerEventData);
                isPointerDown = false;
            }
        }
    }

    private void CheckButtonPress()
    {
        // Get the input devices for the right hand
        var inputDevices = new List<UnityEngine.XR.InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, inputDevices);

        if (inputDevices.Count > 0)
        {
            UnityEngine.XR.InputDevice rightHandDevice = inputDevices[0];

            // Check if the A button is pressed
            bool isPressed;
            if (rightHandDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out isPressed) && isPressed)
            {
                PlayerController.instance.Jump();
            }
        }
    }
}
