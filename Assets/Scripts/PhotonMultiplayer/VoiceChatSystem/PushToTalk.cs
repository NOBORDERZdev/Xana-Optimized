using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PushToTalk : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool isPressed;

    void OnEnable()
    {
        if(ConstantsHolder.xanaConstants.pushToTalk)
        {
            gameObject.AddComponent<EventTrigger>().AddListener(EventTriggerType.PointerDown, OnPointerDownC);
            gameObject.AddComponent<EventTrigger>().AddListener(EventTriggerType.PointerUp, OnPointerUpC);
        }
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {

    }
    public void OnPointerUp(PointerEventData pointerEventData)
    {

    }

    public void OnPointerDownC(BaseEventData pointerEventData)
    {
        XanaVoiceChat.instance.PushToTalk(true);
    }
    public void OnPointerUpC(BaseEventData pointerEventData)
    {
        XanaVoiceChat.instance.PushToTalk(false);
    }
}