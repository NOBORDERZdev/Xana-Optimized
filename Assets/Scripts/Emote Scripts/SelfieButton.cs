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
        // if (GamePlayButtonEvents.inst != null) GamePlayButtonEvents.inst.SelfieBtnUpdate += SelfieBtnUpdated; // no need for this function enable and disable class doing this !
        btn.onClick.AddListener(OnSelfieClick);
    }


    public void OnDisable()
    {
        //  if (GamePlayButtonEvents.inst != null) GamePlayButtonEvents.inst.SelfieBtnUpdate -= SelfieBtnUpdated; // no need for this function enable and disable class doing this !
        btn.onClick.RemoveListener(OnSelfieClick);
    }

    private void OnSelfieClick()
    {
        ActionManager.StopActionAnimation?.Invoke();
        GamePlayButtonEvents.inst.OnSelfieClick();
        BuilderEventManager.UIToggle?.Invoke(true);
        PlayerController.PlayerIsWalking?.Invoke();
        ReferencesForGamePlay.instance.playerControllerNew.StopBuilderComponent();
    }

    private void SelfieBtnUpdated(bool canClick)
    {
        btn.interactable = canClick;
    }
}