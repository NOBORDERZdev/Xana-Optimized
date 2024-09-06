using PhysicsCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class PenguinLookPointTracker : MonoBehaviour
{
    public GameObject referenceObject; // Assign this in the inspector
    public float positionSmoothSpeed = 0.125f; // Adjust this value to control the movement speed
    public float rotationSmoothSpeed = 0.125f; // Adjust this value to control the rotation speed
    public CharacterManager characterManager;
    void FixedUpdate()
    {
        if (referenceObject != null)
        {
            // Capture position and rotation of the reference object
            //Vector3 targetPosition = referenceObject.transform.position;
            //Quaternion targetRotation = referenceObject.transform.rotation;

            // Smoothly interpolate to the target position and rotation
            // Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, positionSmoothSpeed * Time.deltaTime);
            //Quaternion smoothedRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothSpeed * Time.deltaTime);

            // Apply the smoothed position and rotation to this object
            //if (!characterManager.GetJumping())
            //{
            //    transform.position = referenceObject.transform.position;// smoothedPosition;
            //}
            //else
            //{ 
            //    transform.position = new Vector3(referenceObject.transform.position.x, transform.position.y, referenceObject.transform.position.z);    
            //}
            transform.DOMove(referenceObject.transform.position, 0.1f);
           // transform.position = referenceObject.transform.position;
            transform.DORotate(referenceObject.transform.rotation.eulerAngles, 0.1f);
            // transform.rotation = referenceObject.transform.rotation;// smoothedRotation;
        }
    }
}
