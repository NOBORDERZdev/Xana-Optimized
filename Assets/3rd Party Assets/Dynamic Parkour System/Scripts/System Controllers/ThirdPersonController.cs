/*
Dynamic Parkour System grants parkour capabilities to any humanoid character model.
Copyright (C) 2021  Èric Canela Sol
Contact: knela96@gmail.com or @knela96 twitter

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG;
using Cinemachine;
using DG.Tweening;
using Photon.Pun;

namespace Climbing
{
    [RequireComponent(typeof(MovementCharacterController))]
    [RequireComponent(typeof(AnimationCharacterController))]
    [RequireComponent(typeof(DetectionCharacterController))]
    [RequireComponent(typeof(CameraController))]
    [RequireComponent(typeof(VaultingController))]

    public class ThirdPersonController : MonoBehaviour/*Pun*/
    {
        public InputCharacterController characterInput;
        [HideInInspector] public MovementCharacterController characterMovement;
        [HideInInspector] public AnimationCharacterController characterAnimation;
        [HideInInspector] public DetectionCharacterController characterDetection;
        [HideInInspector] public VaultingController vaultingController;
        public bool isGrounded = false;
        public bool allowMovement = true;
        public bool onAir = false;
        public bool isJumping = false;
        public bool inSlope = false;
        public bool isVaulting = false;
        public bool dummy = false;

        [Header("Cameras")]
        public CameraController cameraController;
        public Transform mainCamera;
        public Transform freeCamera;
        public CinemachineFreeLook runCamera;
        public CinemachineVirtualCamera sliderCamera;


        [Header("Step Settings")]
        [Range(0, 10.0f)] public float stepHeight = 0.8f;
        public float stepVelocity = 0.2f;

        [Header("Colliders")]
        public CapsuleCollider normalCapsuleCollider;
        public CapsuleCollider slidingCapsuleCollider;

        [Header("VFX")]
        public GameObject slideParticleEffect;

        private float turnSmoothTime = 0.1f;
        private float turnSmoothVelocity;
        public PhotonView photonView;
        private void Awake()
        {
            photonView = transform.parent.GetComponent<PhotonView>();

            if (photonView.IsMine)
            {
                characterInput = CanvasButtonsHandler.inst.RFMInputController;
                CanvasButtonsHandler.inst.jumpAction += JumpAction;
                CanvasButtonsHandler.inst.slideAction += SlideAction;
            }
            else
            {
                characterInput = GetComponent<InputCharacterController>();
            }

            characterMovement = GetComponent<MovementCharacterController>();
            characterAnimation = GetComponent<AnimationCharacterController>();
            characterDetection = GetComponent<DetectionCharacterController>();
            vaultingController = GetComponent<VaultingController>();

            if (cameraController == null)
                Debug.LogError("Attach the Camera Controller located in the Free Look Camera");
        }

        private void Start()
        {
            characterMovement.OnLanded += characterAnimation.Land;
            characterMovement.OnFall += characterAnimation.Fall;
        }

        public void JumpAction(bool jump)
        {
            GetComponent<PhotonView>().RPC("JumpRPC", RpcTarget.Others, jump, GetComponent<PhotonView>().ViewID);
        }

        public void SlideAction(bool slide)
        {
            GetComponent<PhotonView>().RPC("SlideRPC", RpcTarget.Others, slide, GetComponent<PhotonView>().ViewID);
        }
        [PunRPC]
        public void JumpRPC(bool isJump, int photonViewId)
        {
            Debug.LogError("JumpRPC Called: " + isJump + "  " + photonViewId + "  " + GetComponent<PhotonView>().ViewID);
            if (GetComponent<PhotonView>().ViewID == photonViewId)
            {
                characterInput.jump = isJump;
            }
        }

        [PunRPC]

        public void SlideRPC(bool isDrop, int photonViewId)
        {
            Debug.LogError("SlideRPC Called: " + isDrop + "  " + photonViewId + "  " + GetComponent<PhotonView>().ViewID);
            if (GetComponent<PhotonView>().ViewID == photonViewId)
            {
                characterInput.drop = isDrop;
            }
        }

        void Update()
        {
            //Detect if Player is on Ground
            isGrounded = OnGround();

            if (!transform.parent.gameObject.GetComponent<PhotonView>().IsMine) return;

            //Get Input if controller and movement are not disabled
            if (!dummy && allowMovement && photonView.IsMine)
            {
                AddMovementInput(characterInput.movement);

                //Detects if Joystick is being pushed hard
                if (characterInput.run && characterInput.movement.magnitude > 0.5f)
                {
                    ToggleRun();
                }
                else if (!characterInput.run)
                {
                    ToggleWalk();
                }
            }
        }

        private bool OnGround()
        {
            return characterDetection.IsGrounded(stepHeight);
        }

        public void AddMovementInput(Vector2 direction)
        {
            Vector3 translation = Vector3.zero;

            translation = GroundMovement(direction);

            characterMovement.SetVelocity(Vector3.ClampMagnitude(translation, 1.0f));
        }

        Vector3 GroundMovement(Vector2 input)
        {
            Vector3 direction = new Vector3(input.x, 0f, input.y).normalized;


            Vector3 translation;
            if (!freeCamera)
            {
                translation = Vector3.zero;
            }
            else
            {
                //Gets direction of movement relative to the camera rotation
                freeCamera.eulerAngles = new Vector3(0, mainCamera.eulerAngles.y, 0);
                /*Vector3 */
                translation = freeCamera.transform.forward * input.y + freeCamera.transform.right * input.x;
                translation.y = 0;
            }

            //Detects if player is moving to any direction
            if (translation.magnitude > 0)
            {
                RotatePlayer(direction);
                characterAnimation.animator.SetBool("Released", false);
            }
            else
            {
                ToggleWalk();
                characterAnimation.animator.SetBool("Released", true);
            }

            return translation;
        }

        public void RotatePlayer(Vector3 direction)
        {
            //Get direction with camera rotation
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + mainCamera.eulerAngles.y;

            //Rotate Mesh to Movement
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }
        public Quaternion RotateToCameraDirection(Vector3 direction)
        {
            //Get direction with camera rotation
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + mainCamera.eulerAngles.y;

            //Rotate Mesh to Movement
            return Quaternion.Euler(0f, targetAngle, 0f);
        }

        public void ResetMovement()
        {
            characterMovement.ResetSpeed();
        }

        public void ToggleRun()
        {
            if (characterMovement.GetState() != MovementState.Running)
            {
                characterMovement.SetCurrentState(MovementState.Running);
                DOTween.To(() => runCamera.m_Lens.FieldOfView, x => runCamera.m_Lens.FieldOfView = x, 60, 1);
                DOTween.To(() => sliderCamera.m_Lens.FieldOfView, x => sliderCamera.m_Lens.FieldOfView = x, 60, 0.3f);
                characterMovement.curSpeed = characterMovement.RunSpeed;
                characterAnimation.animator.SetBool("Run", true);
            }
        }
        public void ToggleWalk()
        {
            if (characterMovement.GetState() != MovementState.Walking)
            {
                characterMovement.SetCurrentState(MovementState.Walking);
                DOTween.To(() => runCamera.m_Lens.FieldOfView, x => runCamera.m_Lens.FieldOfView = x, 40, 1);
                DOTween.To(() => sliderCamera.m_Lens.FieldOfView, x => sliderCamera.m_Lens.FieldOfView = x, 40, 0.6f);
                characterMovement.curSpeed = characterMovement.walkSpeed;
                characterAnimation.animator.SetBool("Run", false);
            }
        }


        public float GetCurrentVelocity()
        {
            return characterMovement.GetVelocity().magnitude;
        }

        public void DisableController()
        {
            characterMovement.SetKinematic(true);
            characterMovement.enableFeetIK = false;
            dummy = true;
            allowMovement = false;
        }
        public void EnableController()
        {
            characterMovement.SetKinematic(false);
            characterMovement.EnableFeetIK();
            characterMovement.ApplyGravity();
            characterMovement.stopMotion = false;
            dummy = false;
            allowMovement = true;
        }
    }
}