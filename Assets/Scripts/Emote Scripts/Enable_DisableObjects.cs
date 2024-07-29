﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enable_DisableObjects : MonoBehaviour
{
    //public Button SwitchCameraObject;
    //public Button CameraSnapObject;
    //public Button ChatObject;
    //public Button ReactionObject;
    //public Button EmoteObject;
    //public Button ActionsObject;
    public Button[] ButtontoUninteractable; //...Added by Abdullah
    public Button[] UiObjectsToDisable;
    public InputField ChatInputField;
    public GameObject ReactionPanel;
    public GameObject ActionPanel;
    public GameObject EmotePanel;
    [SerializeField] private GameObject _jump;
    [SerializeField] private GameObject _micOnButton;
    [SerializeField] private GameObject _micOffButton;
    [SerializeField] private GameObject _animationcloseButton;
    [SerializeField] private GameObject _screenRotateButton;
    private bool _wasMicOn;
    private bool _once = false;

    public static Enable_DisableObjects Instance;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    //private void OnEnable()
    //{
    //    PlayerController.PlayerIsWalking += OnPlayerWalking;
    //    PlayerController.PlayerIsIdle += OnPlayerIdle;

    //}
    //private void OnDisable()
    //{
    //    PlayerController.PlayerIsWalking -= OnPlayerWalking;
    //    PlayerController.PlayerIsIdle -= OnPlayerIdle;
    //}

    private void OnPlayerWalking()
    {
        if (ConstantsHolder.isPenguin)
            return;

        foreach (Button btns in ButtontoUninteractable)//...Added by Abdullah
        {
            btns.interactable = false;
            ChatInputField.interactable = false;
        }
        //Commented by Abdullah
        //SwitchCameraObject.interactable = false; Commented by Abdullah
        //ChatObject.interactable = false;
        //ActionsObject.interactable = false;
        //EmoteObject.interactable = false;
        //ReactionObject.interactable = false;
    }

    private void OnPlayerIdle()
    {
        if (ConstantsHolder.isPenguin)
            return;

        foreach (Button btns in ButtontoUninteractable)//...Added by Abdullah
        {
            btns.interactable = true;
            ChatInputField.interactable = true;
        }
        //Commented by Abdullah
        //SwitchCameraObject.interactable = true;
        //CameraSnapObject.interactable = true;
        //ChatObject.interactable = true;
        //ActionsObject.interactable = true;
        //EmoteObject.interactable = true;
        //ReactionObject.interactable = true;
    }

    public void EnableDisableUIObjects(bool setActive)
    {
        foreach (Button btns in UiObjectsToDisable)
        {
            btns.gameObject.SetActive(setActive);
        }
        ChatInputField.gameObject.SetActive(setActive);
        _jump.SetActive(setActive);
        ReactionPanel.SetActive(false);
        ActionPanel.SetActive(false);
        EmotePanel.SetActive(false);
        _animationcloseButton.SetActive(false);
        EnableDisableMicButton(setActive);
    }

    private void EnableDisableMicButton(bool setActive)
    {
        if (!_once)
        {
            if (_micOnButton.activeSelf)
            {
                _wasMicOn = true;
            }
            else
            {
                _wasMicOn = false;
            }
            _micOnButton.SetActive(setActive);
            _micOffButton.SetActive(setActive);
            _once = true;
        }
        else
        {
            _once = false;
            if (_wasMicOn)
            {
                _micOnButton.SetActive(setActive);

            }
            else
            {
                _micOffButton.SetActive(setActive);

            }
        }

    }

    public void DisableScreenRotaionButton()
    {
        _screenRotateButton.SetActive(false);
    }

}


