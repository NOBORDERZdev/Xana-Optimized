
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayButtonEvents : MonoBehaviour
{
    static GamePlayButtonEvents _inst;
    public static GamePlayButtonEvents inst
    {
        get
        {
            if (_inst == null) _inst = FindObjectOfType<GamePlayButtonEvents>();
            return _inst;
        }
    }

    public event Action OnGotoAnotherWorld;
    public event Action OnJumpBtnUpEvnt;
    public event Action OnJumpBtnDownEvnt;
    public event Action OnWordrobe;
    public event Action OnHelpButton;
    public event Action OnSettingButton;
    public event Action OnExitButton;
    public event Action OnOptionButton;
    public event Action OnPeope;
    public event Action OnAnnouncement;
    public event Action OnInvite;
    public event Action OnSwitchCamera;
    public event Action OnSelfieButton;
    public event Action OnEmoteSelectionClose;
    public event Action OpenAllAnimsPanel;
    public event Action<bool> AllAnimsPanelUpdate;
    public event Action StopEmoteAnimation;
    public event Action<bool> UpdateHelpObject;
    public event Action<AnimationData> OnAnimationSelected;
    public event Action<int> AnimationDataUpdated;
    public event Action<bool> SelfieBtnUpdate;
    public event Action<bool> SelfiePanleUpdateObjects;
    public event Action<bool> OnUpdateMuseumRoom;

    [HideInInspector]
    public int selectedActionIndex;

    public bool selectionPanelOpen;

    public void OnEnable()
    {
        selectedActionIndex = -1;
    }

    public void OnGotoAnotherWorldClick()
    {
        OnGotoAnotherWorld?.Invoke();
    }

    public void OnWordrobeClick()
    {
        OnWordrobe?.Invoke();
    }

    public void OnHelpButtonClick()
    {
        OnHelpButton?.Invoke();
    }

    public void OnSettingButtonClick()
    {
        OnSettingButton?.Invoke();
    }

    internal void OnJumpBtnUp()
    {
        OnJumpBtnUpEvnt?.Invoke();
    }

    internal void OnJumpBtnDown()
    {
        OnJumpBtnDownEvnt?.Invoke();
    }

    internal void CloseEmoteSelectionPanel()
    {
        OnEmoteSelectionClose?.Invoke();
    }

    internal void OnSelfieClick()
    {
        OnSelfieButton?.Invoke();
    }

    public void OnExitButtonClick()
    {
        OnExitButton?.Invoke();
    }

    public void OnClickOptionButton()
    {
        OnOptionButton?.Invoke();
    }

    public void OnSwitchCameraClick()
    {
        OnSwitchCamera?.Invoke();
    }

    public void OnPeopeClick()
    {
        OnPeope?.Invoke();
    }

    public void OnAnnouncementClick()
    {
        OnAnnouncement?.Invoke();
    }

    public void OnInviteClick()
    {
        OnInvite?.Invoke();
    }

    public void OnActionSelectionChanged(int index)
    {
        selectedActionIndex = index;
    }

    internal void OnAnimationSelect(AnimationData animData)
    {
        OnAnimationSelected?.Invoke(animData);
    }

    internal void AnimationDataUdpated(int index)
    {
        AnimationDataUpdated?.Invoke(index);
    }

    internal void AllAnimationsPanelUpdate(bool isOn)
    {
        AllAnimsPanelUpdate?.Invoke(isOn);
    }

    internal void OpenAllAnims()
    {

        OpenAllAnimsPanel?.Invoke();
    }



    public void ShowHelpObjects()
    {
        UpdateHelpObjects(true);
    }

    public void HideHelpObjects()
    {
        UpdateHelpObjects(false);
    }

    public void UpdateHelpObjects(bool isOn)
    {
        UpdateHelpObject?.Invoke(isOn);
    }

    public void UpdateSelfieBtn(bool canClick)
    {
        SelfieBtnUpdate?.Invoke(canClick);
    }

    public void SelfiePanleUpdateObject(bool isOn)
    {
        SelfiePanleUpdateObjects?.Invoke(isOn);
    }

    public void OnEmoteAnimationStop()
    {
        StopEmoteAnimation?.Invoke();
    }

    public void UpdateCanvasForMuseum(bool isOn)
    {
        OnUpdateMuseumRoom?.Invoke(isOn);
    }
}

