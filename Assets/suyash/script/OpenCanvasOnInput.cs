using UnityEngine;
using UnityEngine.InputSystem;

public class OpenCanvasOnInput : MonoBehaviour
{
    public GameObject canvas; // Assign your Canvas GameObject here
    private bool isCanvasActive = false;

    // Define the Input Action
    public InputActionReference buttonPressAction;

    private void OnEnable()
    {
        // Enable the action when the object is enabled
        buttonPressAction.action.Enable();
    }

    private void OnDisable()
    {
        // Disable the action when the object is disabled
        buttonPressAction.action.Disable();
    }

    void Update()
    {
        // Check if the A button is pressed on the right Oculus controller
        if (buttonPressAction.action.triggered)
        {
            ToggleCanvas();
        }
    }

    private void ToggleCanvas()
    {
        isCanvasActive = !isCanvasActive;
        canvas.SetActive(isCanvasActive);
    }
}
