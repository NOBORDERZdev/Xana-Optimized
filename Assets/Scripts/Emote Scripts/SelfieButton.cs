using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelfieButton : MonoBehaviour
{
    Button btn;

    public void Awake()
    {
        btn = GetComponent<Button>();
    }

    public void OnEnable()
    {
        if (GamePlayButtonEvents.inst != null) GamePlayButtonEvents.inst.SelfieBtnUpdate += SelfieBtnUpdated;
        btn.onClick.AddListener(OnSelfieClick);
    }


    public void OnDisable()
    {
        if (GamePlayButtonEvents.inst != null) GamePlayButtonEvents.inst.SelfieBtnUpdate -= SelfieBtnUpdated;
        btn.onClick.RemoveListener(OnSelfieClick);
    }

    private void OnSelfieClick()
    {
        GamePlayButtonEvents.inst.OnSelfieClick();
        BuilderEventManager.SelfiActive?.Invoke(true);
    }

    private void SelfieBtnUpdated(bool canClick)
    {
        btn.interactable = canClick;
    }
}
