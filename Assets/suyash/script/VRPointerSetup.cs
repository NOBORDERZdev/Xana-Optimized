using UnityEngine;
using UnityEngine.UI;

public class VRPointerSetup : MonoBehaviour
{
    public Transform controllerTransform; // Assign your controller's transform in the inspector
    public float pointerDistance = 10.0f; // Distance from the controller to the pointer
    public Sprite pointerSprite;          // Assign your pointer sprite in the inspector
    public float pointerSize = 0.1f;      // Size of the pointer
    public LayerMask uiLayerMask;         // Layer mask to detect UI elements

    private GameObject pointer;
    private RectTransform pointerRectTransform;
    public Image pointerImage;
    private Canvas pointerCanvas;

    void Start()
    {
        CreatePointer();
    }

    void Update()
    {
        UpdatePointerPositionAndRotation();
    }

    private void CreatePointer()
    {
        // Create a new GameObject for the pointer
        pointer = new GameObject("VRPointer");

        // Add a RectTransform component
        pointerRectTransform = pointer.AddComponent<RectTransform>();
        pointerRectTransform.sizeDelta = new Vector2(pointerSize, pointerSize); // Use manual size

        // Add an Image component
        pointerImage = pointer.AddComponent<Image>();
        pointerImage.sprite = pointerSprite; // Set the sprite
        pointerImage.color = Color.white;    // Initial color

        // Set the Canvas for the pointer
        pointerCanvas = pointer.AddComponent<Canvas>();
        pointerCanvas.renderMode = RenderMode.WorldSpace;

        // Optional: Add a CanvasScaler for resolution independence
        CanvasScaler scaler = pointer.AddComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 10;

        // Set the pointer as a child of the main camera (or controller) for easier positioning
        pointer.transform.SetParent(controllerTransform, false);

        // Adjust the scale and position
        pointerRectTransform.localScale = Vector3.one; // Keep scale at one since we are setting the size manually
        pointerRectTransform.localPosition = new Vector3(0, 0, pointerDistance); // Position at the end of the ray
    }

    private void UpdatePointerPositionAndRotation()
    {
        // Perform a raycast to detect UI elements
        RaycastHit hit;
        Vector3 forward = controllerTransform.forward;
        bool isHoveringUI = Physics.Raycast(controllerTransform.position, forward, out hit, pointerDistance, uiLayerMask);

        // Set pointer position based on raycast hit
        float distance = isHoveringUI ? hit.distance - 0.01f : pointerDistance; // Slight offset to bring pointer in front
        pointer.transform.position = controllerTransform.position + forward * distance;

        // Rotate the pointer to face forward
        pointer.transform.rotation = Quaternion.LookRotation(forward);

        // Scale the pointer manually using the provided size
        pointer.transform.localScale = new Vector3(pointerSize, pointerSize, pointerSize);
    }
}
