using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class CharacterControls : MonoBehaviour
{
    internal PlayerControllerNew playerControler;

    private Coroutine routine;

    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float cameraHeight;
    [SerializeField] private float stepOffset;


     private float old_walkSpeed;
     private float old_sprintSpeed;
     private float old_jumpHeight;
     private float old_cameraHeight;
     private float old_stepOffset;

    private void OnEnable() => AddListners();
    private void OnDisable() => RemoveListners();


    private void AddListners()
    {
        StoreCurretValues();
        routine = StartCoroutine(OvverideCurrentValues());

    }
    private void RemoveListners()
    {
        RestoreOldValues();
        StopCoroutine(routine);
    }

    private void StoreCurretValues()
    {
        //old_walkSpeed = playerControler.walkSpeed;
        old_sprintSpeed = playerControler.sprintSpeed;
        old_jumpHeight = playerControler.jumpHeight;
        old_stepOffset = playerControler.GetComponent<CharacterController>().stepOffset;
        //old_cameraHeight = playerControler.controllerCamera.GetComponent<vThirdPersonCamera>().height;
    }
    IEnumerator OvverideCurrentValues()
    {
        CharacterController cc = playerControler.GetComponent<CharacterController>();
        //vThirdPersonCamera vtc = playerControler.controllerCamera.GetComponent<vThirdPersonCamera>();


       

        while (true)
        {
            yield return new WaitForEndOfFrame();
            cc.stepOffset = stepOffset;
            //vtc.height = cameraHeight;
            //playerControler.finalWalkSpeed = walkSpeed;
            //playerControler.walkSpeed = walkSpeed;
            playerControler.sprintSpeed = sprintSpeed;
            playerControler.jumpHeight = jumpHeight;
        }
           
    }
    private void RestoreOldValues()
    {
        //playerControler.finalWalkSpeed = old_walkSpeed;
        //playerControler.walkSpeed = old_walkSpeed;
        playerControler.sprintSpeed = old_sprintSpeed;
        playerControler.jumpHeight = old_jumpHeight;
        playerControler.GetComponent<CharacterController>().stepOffset = old_stepOffset;
        //playerControler.controllerCamera.GetComponent<vThirdPersonCamera>().height = old_cameraHeight;
    }
}
