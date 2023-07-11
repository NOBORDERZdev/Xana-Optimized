using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonClickAndHold : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    bool m_interactable = true;
    public bool interactable
    {
        get { return m_interactable; }
        set
        {
            m_interactable = value;
        }
    }
    bool isPressed = false;
    float time;
    public float pressAndHoldTime = 1f;
    public float clickTime = .1f;
    public bool eligibleClickWhilePressed = true;
    public event Action Clicked;
    public event Action LongClicked;
    public UnityEvent onClick;
    public UnityEvent onLongClick;

    void Awake()
    {
        interactable = m_interactable;
    }

    public void Update()
    {
        UpdatePointerClick();
    }

    public void OnPointerClick()
    {
        if (interactable)
        {
            Clicked?.Invoke();
            onClick.Invoke();
        }
    }

    public void OnLongClick()
    {
        if (interactable)
        {
            Debug.Log("LongClick Done.");
          //  GamePlayButtonEvents.inst.selectionPanelOpen = true;
            LongClicked?.Invoke();
            onLongClick.Invoke();
        }
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerPressRaycast.gameObject.GetInstanceID().Equals(gameObject.GetInstanceID()))
        {
            isPressed = true;
        }
        else
        {
            isPressed = false;
        }
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
    }

  
    private void UpdatePointerClick()
    {
        if (isPressed)
        {
            time += Time.deltaTime;
            if (eligibleClickWhilePressed && time > pressAndHoldTime)
            {
                //print("------> user long click with auto call " + time);
                OnLongClick();
                isPressed = false;
                time = 0;
            }
        }
        else if (time > 0)
        {
            if(time >= pressAndHoldTime)
            {
                //print("------> user long click " + time);
                OnLongClick();
            }
            else if (time >= clickTime)
            {
                //print("------> user short click " + time);
                OnPointerClick();
            }
            time = 0;
        }
    }

}
