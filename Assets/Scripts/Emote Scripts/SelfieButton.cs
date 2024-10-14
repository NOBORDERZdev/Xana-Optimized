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
        //added condition to prevent selfie open issue while jumping and falling from environment
        if (!ReferencesForGamePlay.instance.playerControllerNew._IsGrounded)
            return;
        if (ActionManager.IsAnimRunning) //for stop dance animation
        {
            ActionManager.StopActionAnimation?.Invoke();
        }
        EmoteReactionUIHandler.lastEmotePlayed = null;

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