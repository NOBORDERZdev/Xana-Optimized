using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[System.Serializable]
public class MoveInputEvent : UnityEvent<float, float> { }
[System.Serializable]
public class LookInputEvent : UnityEvent<float, float> { }
[System.Serializable]
public class JumpEvent : UnityEvent<bool> { }
[System.Serializable]
public class SprintEvent : UnityEvent<bool> { }


public class InputContoller : MonoBehaviour
{
    Controls controls;
   public MoveInputEvent moveInputEvent;
    public LookInputEvent lookInputEvent;
    public JumpEvent jumpEvent;
    public SprintEvent sprintEvent;

    private void Awake()
    {

        controls = new Controls();
    }
    private void Update()
    {
        //Keyboard kb = InputSystem.GetDevice<Keyboard>();
        //if(kb.)
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
        controls.Gameplay.Move.performed+= OnMovePerformed;
        controls.Gameplay.Move.canceled += OnMovePerformed;
        controls.Gameplay.Action.performed += ShowDialog;
        controls.Gameplay.Back.performed += CloseDialog;
        controls.Gameplay.Back.performed += Continue;

        controls.Gameplay.Jump.performed += jumpNow;
        controls.Gameplay.Jump.canceled += jumpNow;
        controls.Gameplay.Look.performed += LookPerformed;

        controls.Gameplay.Sprint.performed += Sprintmethod;
        controls.Gameplay.Sprint.canceled += Sprintmethod;

    }

    private void Sprintmethod(InputAction.CallbackContext context)
    {
        bool sprint = context.ReadValueAsButton();
        sprintEvent.Invoke(sprint);
    }

    private void jumpNow(InputAction.CallbackContext context)
    {
        bool a = context.ReadValueAsButton();
        jumpEvent.Invoke(a);
        Debug.Log("Action jumpNow pressed");
    }

    private void Continue(InputAction.CallbackContext context)
    {
       // DialogueManager.dialogueManagerInstance.DisplayNextSentence();
    }

    private void LookPerformed(InputAction.CallbackContext context)
    {
        Vector2 deltaLook = context.ReadValue<Vector2>();
        lookInputEvent.Invoke(deltaLook.x, deltaLook.y);
      //   Debug.Log("deltaLook Input" + deltaLook);
    }



  

    private void ShowDialog(InputAction.CallbackContext context)
    {
       // DialogueManager.dialogueManagerInstance.StartDialogue();
      //  CanvusHandler.canvusHandlerInstance.Display();
        Debug.Log("Action E pressed" );
    }
    private void CloseDialog(InputAction.CallbackContext context)
    {
        CanvusHandler.canvusHandlerInstance.is_Trigger = false;
       // CanvusHandler.canvusHandlerInstance.Display();
        Debug.Log("Action Escape pressed");
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
       Vector2 moveInput=context.ReadValue<Vector2>();
        moveInputEvent.Invoke(moveInput.x, moveInput.y);
       // Debug.Log("Move Input"+ moveInput);
    }
}
