using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System;

[System.Serializable]
public class GrabThrow : UnityEvent { }
[System.Serializable]
public class BasicCombo : UnityEvent { }
[System.Serializable]
public class BasicCombo2 : UnityEvent { }
[System.Serializable]
public class SpecialCombo : UnityEvent { }
[System.Serializable]
public class Throw : UnityEvent { }
[System.Serializable]
public class BlockAttack : UnityEvent { }
[System.Serializable]
public class ThrowEscapse : UnityEvent { }


public class InputController : MonoBehaviour
{
    Movement controls;
    public MoveInputEvent moveInputEvent;
    public LookInputEvent lookInputEvent;
    public JumpEvent jumpEvent;
    public SprintEvent sprintEvent;
    public GrabThrow grabThrow;
    public BasicCombo basicCombo;
    public BasicCombo2 basicCombo2;
    public SpecialCombo specialCombo;
    public Throw _throw;
    public ThrowEscapse throwEscape;
    public BlockAttack blockAttack;
    private void Awake()
    {

        controls = new Movement();
    }

    private void OnEnable()
    {
        controls.PlayerMain.Enable();
        controls.PlayerMain.Move.performed += OnMovePerformed;
        controls.PlayerMain.Move.canceled += OnMovePerformed;

        controls.PlayerMain.Jump.performed += jumpNow;
        controls.PlayerMain.Jump.canceled += jumpNow;

        controls.PlayerMain.GrabThrow.performed += OnGrabThrow;
        //controls.PlayerMain.GrabThrow.canceled += OnGrabThrow;

        controls.PlayerMain.BasicCombo.performed += OnBasicCombo;
        //controls.PlayerMain.BasicCombo.canceled += OnBasicCombo;
        
        controls.PlayerMain.BasicCombo2.performed += OnBasicCombo2;
        //controls.PlayerMain.BasicCombo2.canceled += OnBasicCombo2;

        controls.PlayerMain.SpeicalCombo.performed += OnSpecialCombo;
        //controls.PlayerMain.SpeicalCombo.canceled += OnSpecialCombo;

        controls.PlayerMain.Throw.performed += OnThrow;
        //controls.PlayerMain.Throw.canceled += OnThrow;

        controls.PlayerMain.ThrowEscape.performed += OnThrowEscape;
        controls.PlayerMain.ThrowEscape.canceled += OnBlockStop;
        
    //    controls.PlayerMain.Block.performed += StartBlock;
     //   controls.PlayerMain.ThrowEscape.canceled += StartBlock;

    }


    private void OnDisable()
    {
        controls.PlayerMain.Enable();
        controls.PlayerMain.Move.performed -= OnMovePerformed;
        controls.PlayerMain.Move.canceled -= OnMovePerformed;

        controls.PlayerMain.Jump.performed -= jumpNow;
        controls.PlayerMain.Jump.canceled -= jumpNow;

        controls.PlayerMain.GrabThrow.performed -= OnGrabThrow;
        //controls.PlayerMain.GrabThrow.canceled -= OnGrabThrow;

        controls.PlayerMain.BasicCombo.performed -= OnBasicCombo;
        //controls.PlayerMain.BasicCombo.canceled -= OnBasicCombo;

        controls.PlayerMain.BasicCombo2.performed -= OnBasicCombo2;
        //controls.PlayerMain.BasicCombo2.canceled -= OnBasicCombo2;

        controls.PlayerMain.SpeicalCombo.performed -= OnSpecialCombo;
        //controls.PlayerMain.SpeicalCombo.canceled -= OnSpecialCombo;


        controls.PlayerMain.Throw.performed -= OnThrow;
        //controls.PlayerMain.Throw.canceled -= OnThrow;

        controls.PlayerMain.ThrowEscape.performed -= OnThrowEscape;
        controls.PlayerMain.ThrowEscape.canceled -= OnBlockStop;

      //  controls.PlayerMain.Block.performed -= StartBlock;
     //   controls.PlayerMain.Block.canceled -= StartBlock;

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
     }


    private void LookPerformed(InputAction.CallbackContext context)
    {
        Vector2 deltaLook = context.ReadValue<Vector2>();
        lookInputEvent.Invoke(deltaLook.x, deltaLook.y);
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        Vector2 moveInput = context.ReadValue<Vector2>();
        moveInputEvent.Invoke(moveInput.x, moveInput.y);
    }

    private void OnGrabThrow(InputAction.CallbackContext context)
    {
        grabThrow.Invoke();
    }
    private void OnBasicCombo(InputAction.CallbackContext context)
    {
        basicCombo.Invoke();
    }
    private void OnBasicCombo2(InputAction.CallbackContext context)
    {
        basicCombo2.Invoke();
    }
    private void OnSpecialCombo(InputAction.CallbackContext context)
    {
        specialCombo.Invoke();
    }
    private void OnThrow(InputAction.CallbackContext context)
    {
        _throw.Invoke();
    }
    private void OnThrowEscape(InputAction.CallbackContext context)
    {
        throwEscape.Invoke();
    }
    private void OnBlockStop(InputAction.CallbackContext context)
    {
        blockAttack.Invoke();
    }
}
