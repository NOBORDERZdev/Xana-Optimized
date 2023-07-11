using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIEventHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public GameObject m_Camera;

    public void OnPointerDown(PointerEventData eventData)
    {
        //CameraLook.instance.m_PressCounter++;
        m_Camera.GetComponent<CameraLook>().m_PressCounter++;
        //print("inside");
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        //CameraLook.instance.m_PressCounter--;
        m_Camera.GetComponent<CameraLook>().m_PressCounter--;
    }
}
