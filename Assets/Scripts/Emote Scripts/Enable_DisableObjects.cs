using System;
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
    public InputField ChatInputField;
    public GameObject ReactionPanel;
    public GameObject ActionPanel;
    public GameObject EmotePanel;
    public static Enable_DisableObjects Instance;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    private void OnEnable()
    {
        PlayerControllerNew.PlayerIsWalking += OnPlayerWalking;
        PlayerControllerNew.PlayerIsIdle += OnPlayerIdle;

    }
    private void OnDisable()
    {
        PlayerControllerNew.PlayerIsWalking -= OnPlayerWalking;
        PlayerControllerNew.PlayerIsIdle -= OnPlayerIdle;
    }

    private void OnPlayerWalking()
    {
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
}


