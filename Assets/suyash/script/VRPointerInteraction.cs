using UnityEngine;

public class VRPointerInteraction : MonoBehaviour
{
    private VRPointerSetup pointerSetup;
    public Color defaultColor = Color.white;
    public Color hoverColor = Color.yellow;

    void Start()
    {
        pointerSetup = GetComponent<VRPointerSetup>();
    }

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(pointerSetup.controllerTransform.position, pointerSetup.controllerTransform.forward, out hit, pointerSetup.pointerDistance))
        {
            if (hit.collider.CompareTag("UIElement"))
            {
                pointerSetup.pointerImage.color = hoverColor;
            }
            else
            {
                pointerSetup.pointerImage.color = defaultColor;
            }
        }
        else
        {
            pointerSetup.pointerImage.color = defaultColor;
        }
    }
}
