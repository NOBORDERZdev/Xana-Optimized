using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.Serialization;

public class JoyStickIssue : OnScreenControl, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData == null)
            throw new System.ArgumentNullException(nameof(eventData));

        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponentInParent<RectTransform>(), eventData.position, eventData.pressEventCamera, out m_PointerDownPos);

        PlayerController.isJoystickDragging = true;
        PlayerCameraController.instance.isJoystickPressed = true;

        if (PlayerCameraController.instance._allowSyncedControl)
            PlayerCameraController.instance.isRotatingScreen = true;
        else if (!PlayerCameraController.instance._allowSyncedControl)
        {
            if (Input.touchCount > 1)
            {
                if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(1).fingerId))
                    PlayerCameraController.instance.isRotatingScreen = true;
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (ScreenOrientationManager._instance.isPotrait)
        { movementRange = 60; }
        
        if (eventData == null)
            throw new System.ArgumentNullException(nameof(eventData));
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponentInParent<RectTransform>(), eventData.position, eventData.pressEventCamera, out var position);
        var delta = position - m_PointerDownPos;

        delta = Vector2.ClampMagnitude(delta, movementRange);
        ((RectTransform)transform).anchoredPosition = m_StartPos + (Vector3)delta;

        var newPos = new Vector2(delta.x / movementRange, delta.y / movementRange);
        SendValueToControl(newPos);
        PlayerController.isJoystickDragging = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ((RectTransform)transform).anchoredPosition = m_StartPos;
        SendValueToControl(Vector2.zero);

        PlayerController.isJoystickDragging = false;
        PlayerCameraController.instance.isJoystickPressed = false;
        PlayerCameraController.instance.isRotatingScreen = false;
    }
    public void ResetJoyStick()
    {
        ((RectTransform)transform).anchoredPosition = m_StartPos;
        SendValueToControl(Vector2.zero);
    }
    private void Start()
    {
        m_StartPos = ((RectTransform)transform).anchoredPosition;
        controlPath = GetComponent<OnScreenStick>().controlPath;
        Destroy(GetComponent<OnScreenStick>());
    }

    public float movementRange
    {
        get => m_MovementRange;
        set => m_MovementRange = value;
    }

    [FormerlySerializedAs("movementRange")]
    [SerializeField]
    private float m_MovementRange = 30;

    [InputControl(layout = "Vector2")]
    [SerializeField]
    private string m_ControlPath;

    private Vector3 m_StartPos;
    private Vector2 m_PointerDownPos;

    protected override string controlPathInternal
    {
        get => m_ControlPath;
        set => m_ControlPath = value;
    }
}
