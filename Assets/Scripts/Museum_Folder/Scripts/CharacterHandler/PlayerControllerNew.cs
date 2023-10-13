﻿using Cinemachine;
using Metaverse;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class PlayerControllerNew : MonoBehaviour
{


    public delegate void CameraChangeDelegate(Camera camera);
    public static event CameraChangeDelegate CameraChangeDelegateEvent;

    public static void OnInvokeCameraChange(Camera camera)
    {
        CameraChangeDelegateEvent?.Invoke(camera);
    }
    private float inputThershold = 0.002f; // thershold for input
    float sprintThresold = .85f;
    //[SerializeField]
    public float movementSpeed = 1.0f;
    private float movementSpeedTemp = 2.0f;
    private float currentSpeed = 0f;
    public float sprintSpeed = 2f;
    private float speedSmoothVelocity = 0.1f;
    private float speedSmoothTime = 0.1f;
    private float rotationSpeed = 0.25f;    // .4 previously
    private float gravityValue = -9.81f;

    public float JumpVelocity = 3;
    //[SerializeField]
    public float jumpHeight = 1.0f;

    public Transform cameraTransform = null;
    //public Transform cameraCharacterTransform = null;
    //public GameObject cmVcam;

    public bool sprint, _IsGrounded, jumpNow, sprint_Button, IsJumping;

    private CharacterController characterController = null;

    public Animator animator = null;

    [Header("Controller Camera")]
    public GameObject controllerCamera;
    public GameObject controllerCharacterRenderCamera;

    [HideInInspector]
    public float horizontal;
    [HideInInspector]
    public float vertical;

    bool canSend = true;

    //[HideInInspector]
    public bool m_IsMovementActive;
    public bool allowTeleport = true;
    public static bool isJoystickDragging;
    #region PUBLIC_VAR
    [Header("First Person Camera")]
    [HideInInspector] public GameObject playerRig;
    private float gravity = -9.81f;
    private float firstPersonCurrentSpeed;
    public GameObject firstPersonCameraObj;
    //public GameObject CanvasObject;
    [HideInInspector] public Vector3 camStartPosition;
    public Image fadeImage;
    public bool isFirstPerson = false;
    public GameObject gyroButton;
    public GameObject gyroButton_Portait;
    public static event Action PlayerIsWalking;
    public static event Action PlayerIsIdle;
    [HideInInspector] public Vector3 gravityVector;
    public GameObject ActiveCamera; // ethier TPS OR FPS CAM
    public bool IsJumpButtonPress = false;
    #endregion

    #region PRIVATE_VAR
    private Vector3 JumpTimePostion;
    private Vector3 CanvasJumpTimePostion;
    public Vector3 playerPos;
    public Vector3 PlayerVelocity;
    private Vector3 velocity;
    [SerializeField] private RectTransform innerJoystick; // joystick reference.
    [SerializeField] private RectTransform innerJoystick_Portrait; // joystick reference.
    public bool allowJump = true; // true means can jump

    private bool npcSelected;
    private float runingJumpResetInterval = .45f; //runing tps jump animation time to reset.
    private float idelJumpResetInterval = 1f; //runing tps jump animation time to reset.
    #endregion
    bool allowFpsJump = true;

    #region Player Properties Update

    float originalSprintSpeed = 1;
    float originalJumpSpeed = 1;
    float speedMultiplier = 1;
    float jumpMultiplier = 1;
    #endregion
    [SerializeField]
    CinemachineFreeLook cinemachineFreeLook;

    internal float animationBlendValue = 0;
    private void OnEnable()
    {
        BuilderEventManager.OnHideOpenSword += HideorOpenSword;
        BuilderEventManager.OnAttackwithSword += AttackwithSword;
        BuilderEventManager.OnAttackwithShuriken += AttackwithShuriken;
        BuilderEventManager.OnThowThingsPositionSet += BallPositionSet;
        BuilderEventManager.OnThrowBall += ThrowBall;

        //Update jump height according to builder
        BuilderEventManager.ApplyPlayerProperties += PlayerJumpUpdate;
        BuilderEventManager.SpecialItemPlayerPropertiesUpdate += SpecialItemPlayerPropertiesUpdate;
    }
    private void OnDisable()
    {
        BuilderEventManager.OnHideOpenSword -= HideorOpenSword;
        BuilderEventManager.OnAttackwithSword -= AttackwithSword;
        BuilderEventManager.OnAttackwithShuriken -= AttackwithShuriken;
        BuilderEventManager.OnThowThingsPositionSet -= BallPositionSet;
        BuilderEventManager.OnThrowBall -= ThrowBall;

        //Update jump height according to builder
        BuilderEventManager.ApplyPlayerProperties -= PlayerJumpUpdate;
        BuilderEventManager.SpecialItemPlayerPropertiesUpdate -= SpecialItemPlayerPropertiesUpdate;

    }

    void Start()
    {

        originalSprintSpeed = sprintSpeed;
        originalJumpSpeed = JumpVelocity;

        Debug.Log("Player Controller New Start");
        gyroButton.SetActive(false);
        gyroButton_Portait.SetActive(false);

        firstPersonCameraObj.SetActive(false);
        m_IsMovementActive = true;
        sprint = false;
        characterController = GetComponent<CharacterController>();
        // animator = GetComponent<Animator>();
        movementSpeedTemp = movementSpeed;
        //firstPersonTempSpeed = firstPersonMoveSpeed;
        camStartPosition = firstPersonCameraObj.transform.localPosition;
        // cameraTransform = Camera.main.transform;

        innerJoystick.gameObject.AddComponent<JoyStickIssue>();
        innerJoystick_Portrait.gameObject.AddComponent<JoyStickIssue>();

        if (GamePlayButtonEvents.inst != null)
        {
            GamePlayButtonEvents.inst.OnSwitchCamera += SwitchCameraButton;
            GamePlayButtonEvents.inst.OnJumpBtnDownEvnt += JumpAllowed;
            GamePlayButtonEvents.inst.OnJumpBtnUpEvnt += JumpNotAllowed;
        }
        //if (GamePlayButtonEvents.inst != null) GamePlayButtonEvents.inst.OnJumpBtnUpEvnt += JumpNotAllowed;
        ActiveCamera = ReferrencesForDynamicMuseum.instance.randerCamera.gameObject;


        ////Update jump height according to builder
        //BuilderEventManager.ApplyPlayerProperties += PlayerJumpUpdate;

        // RFM: we need a reference to the local player to set its position.
        //RFM.Globals.player = this;
        // RFM


    }


    public void OnDestroy()
    {
        if (GamePlayButtonEvents.inst != null)
        {
            GamePlayButtonEvents.inst.OnSwitchCamera -= SwitchCameraButton;
            GamePlayButtonEvents.inst.OnJumpBtnDownEvnt -= JumpAllowed;
            GamePlayButtonEvents.inst.OnJumpBtnUpEvnt -= JumpNotAllowed;
        }

        ////Update jump height according to builder
        //BuilderEventManager.ApplyPlayerProperties -= PlayerJumpUpdate;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "LiveStream")
        {
            Gamemanager._InstanceGM.m_youtubeAudio.volume = 1f;
            Gamemanager._InstanceGM.mediaPlayer.AudioVolume = 1;
            //Gamemanager._InstanceGM.mediaPlayer.AudioMuted = false;

            //SoundManager.Instance.MusicSource.volume = 0;
            //  SoundManager.Instance.MusicSource.mute = false;
        }

        if (other.tag == "YoutubeVideo")
        {
            Gamemanager._InstanceGM.ytVideoPlayer.SetDirectAudioVolume(0, 1);
            Gamemanager._InstanceGM.ytVideoPlayer.SetDirectAudioMute(0, false);
            SoundManager.Instance.MusicSource.volume = 0;
            SoundManager.Instance.MusicSource.mute = false;
        }

        if (other.CompareTag("message"))
        {
            other.GetComponent<DialogueTrigger>().ChangeInteractableButton(true);
        }

        if (other.CompareTag("Voice") && !npcSelected)
        {
            npcSelected = true;
            VoiceTrigger voiceTrigger = other.gameObject.GetComponent<VoiceTrigger>();
            DialoguesManagerVoice.Instance.StartDialogue(voiceTrigger);
            Debug.Log("NPC COLLIDE : " + other.gameObject.name);
            other.GetComponent<VoiceTrigger>().startTalking = true;
            other.GetComponent<VoiceTrigger>().ChangeInteractableButton(true);
            other.GetComponent<VoiceTrigger>().Chracter.GetComponent<NPCRandomMovement>().stopanimation = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "LiveStream")
        {
            //Gamemanager._InstanceGM.mediaPlayer.AudioVolume = 0;
            //Gamemanager._InstanceGM.mediaPlayer.AudioMuted = false;
            Gamemanager._InstanceGM.m_youtubeAudio.volume = 0f;
            SoundManager.Instance.MusicSource.volume = 0.19f;
        }

        if (other.tag == "YoutubeVideo")
        {
            Gamemanager._InstanceGM.ytVideoPlayer.SetDirectAudioVolume(0, 0);
            Gamemanager._InstanceGM.ytVideoPlayer.SetDirectAudioMute(0, true);
            SoundManager.Instance.MusicSource.mute = false;
        }

        if (other.CompareTag("message"))
        {
            other.GetComponent<DialogueTrigger>().ChangeInteractableButton(false);

            if (DialoguesManager.Instance != null)
                DialoguesManager.Instance.messageBox.SetActive(false);
        }

        if (other.CompareTag("Voice"))
        {
            if (other.GetComponent<VoiceTrigger>().startTalking)
            {
                npcSelected = false;
                other.GetComponent<VoiceTrigger>().startTalking = false;
            }
            other.GetComponent<VoiceTrigger>().ChangeInteractableButton(false);
            other.GetComponent<VoiceTrigger>().Chracter.GetComponent<NPCRandomMovement>().stopanimation = false;
        }
    }

    // Start is called before the first frame update


    IEnumerator FadeImage(bool fadeAway)
    {
        // fade from opaque to transparent
        if (fadeAway)
        {
            // loop over 1 second backwards
            for (float i = 1; i >= 0; i -= Time.deltaTime)
            {
                // set color with i as alpha
                fadeImage.color = new Color(0, 0, 0, i);
                yield return null;
            }
        }
        // fade from transparent to opaque
        else
        {
            // loop over 1 second
            for (float i = 0; i <= 1; i += Time.deltaTime)
            {
                // set color with i as alpha
                fadeImage.color = new Color(0, 0, 0, i);
                yield return null;
            }
        }
    }

    // Toogle camera first person to therd person
    public void SwitchCameraButton()
    {
        //Debug.Log("0");
        isFirstPerson = !isFirstPerson;
        gravityVector.y = 0;

        CanvasButtonsHandler.inst.OnChangehighlightedFPSbutton(isFirstPerson);
        if (isFirstPerson)
        {

            //Debug.Log("1");
            //Debug.Log("first person call ");
            //Enable_DisableObjects.Instance.ActionsObject.GetComponent<Button>().interactable = true;
            //Enable_DisableObjects.Instance.EmoteObject.GetComponent<Button>().interactable = true;
            //Enable_DisableObjects.Instance.ReactionObject.GetComponent<Button>().interactable = true;
            //  UpdateSefieBtn(false);
            gyroButton.SetActive(true);
            gyroButton_Portait.SetActive(true);

            firstPersonCameraObj.SetActive(true);
            StartCoroutine(FadeImage(true));
            OnInvokeCameraChange(firstPersonCameraObj.GetComponent<Camera>());
            //gameObject.transform.localScale = new Vector3(0, 1, 0);
            DisablePlayerOnFPS();
            ActiveCamera = firstPersonCameraObj;
            // MuseumRaycaster.instance.playerCamera = firstPersonCameraObj.GetComponent<Camera>();
            //animator.gameObject.GetComponent<PhotonAnimatorView>().m_SynchronizeParameters[animator.gameObject.GetComponent<PhotonAnimatorView>().m_SynchronizeParameters.Count - 1].SynchronizeType = PhotonAnimatorView.SynchronizeType.Continuous;
        }
        else
        {
            //Debug.Log("2");
            //MuseumRaycaster.instance.playerCamera = ReferrencesForDynamicMuseum.instance.randerCamera;
            gyroButton.SetActive(false);
            gyroButton_Portait.SetActive(false);

            firstPersonCameraObj.SetActive(false);
            StartCoroutine(FadeImage(true));
            OnInvokeCameraChange(ReferrencesForDynamicMuseum.instance.randerCamera);
            //gameObject.transform.localScale = new Vector3(1, 1, 1);
            EnablePlayerOnThirdPerson();
            ActiveCamera = ReferrencesForDynamicMuseum.instance.randerCamera.gameObject;
            //animator.gameObject.GetComponent<PhotonAnimatorView>().m_SynchronizeParameters[animator.gameObject.GetComponent<PhotonAnimatorView>().m_SynchronizeParameters.Count - 1].SynchronizeType = PhotonAnimatorView.SynchronizeType.Disabled;
        }

        if (animator != null)
        {
            animator.gameObject.GetComponent<ArrowManager>().CallFirstPersonRPC(isFirstPerson);
        }
    }

    public void DisablePlayerOnFPS()
    {
        Transform[] transforms = animator.gameObject.GetComponentsInChildren<Transform>();
        for (int i = 0; i < transforms.Length; i++)
        {
            if (transforms[i].gameObject.layer != LayerMask.NameToLayer("Arrow") && (transforms[i].gameObject.GetComponent<Renderer>() || transforms[i].gameObject.GetComponent<MeshRenderer>()))
            {
                transforms[i].gameObject.GetComponent<Renderer>().enabled = false;
                if (transforms[i].gameObject.name == "Eye_Left" || transforms[i].gameObject.name == "Eye_Right") //this is written becuase can't disable mesh renderer
                {
                    transforms[i].gameObject.transform.localScale = Vector3.zero;
                }
            }
            else if (transforms[i].GetComponent<CanvasGroup>())
                transforms[i].GetComponent<CanvasGroup>().alpha = 0;
        }
    }

    void EnablePlayerOnThirdPerson()
    {
        Transform[] transforms = animator.gameObject.GetComponentsInChildren<Transform>();
        for (int i = 0; i < transforms.Length; i++)
        {
            if (transforms[i].gameObject.layer != LayerMask.NameToLayer("Arrow") && (transforms[i].gameObject.GetComponent<Renderer>() || transforms[i].gameObject.GetComponent<MeshRenderer>()))
            {
                transforms[i].gameObject.GetComponent<Renderer>().enabled = true;
                if (transforms[i].gameObject.name == "Eye_Left" || transforms[i].gameObject.name == "Eye_Right") //this is written becuase can't disable mesh renderer
                {
                    transforms[i].gameObject.transform.localScale = Vector3.one;
                }
            }
            else if (transforms[i].GetComponent<CanvasGroup>())
                transforms[i].GetComponent<CanvasGroup>().alpha = 1;
        }
    }

    // first person camera off when user switch to selfie mode
    public void SwitchToSelfieMode()
    {
        if (isFirstPerson)
        {
            EnablePlayerOnThirdPerson();
            firstPersonCameraObj.SetActive(false);
        }
        else
        {
            isFirstPerson = false;
        }

    }

    // Update is called once per frame

    private void Update()
    {
        if (animator == null)
            return;

        if (characterController.isGrounded && !IsJumping)
        {
            gravityVector.y = gravityValue * Time.deltaTime;
            animator.SetBool("IsFalling", false);
        }
        else if (!characterController.isGrounded && characterController.velocity.y < -1)
            animator.SetBool("IsFalling", true);

        if (m_IsMovementActive)
        {
            if (isFirstPerson && !m_FreeFloatCam)
            {
                if (EmoteAnimationPlay.Instance.isAnimRunning && isJoystickDragging)
                {
                    EmoteAnimationPlay.Instance.StopAnimation();
                    EmoteAnimationPlay.Instance.StopAllCoroutines();
                }
                FirstPersonCameraMove(); // FOR FIRST PERSON MOVEMENT XX
            }
            if (!isFirstPerson && !m_FreeFloatCam)
            {
                if (EmoteAnimationPlay.Instance.isAnimRunning && isJoystickDragging)
                {
                    EmoteAnimationPlay.Instance.StopAnimation();
                    EmoteAnimationPlay.Instance.StopAllCoroutines();
                }
                Move();
                // CanvasObject = GameObject.FindGameObjectWithTag("HeadItem").gameObject;
            }
            if (m_FreeFloatCam)
            {
                FreeFloatMove();
            }


        }
        else
        {
            characterController.Move(Vector3.zero);
            gravityVector.y += gravityValue * Time.deltaTime;
            characterController.Move(gravityVector * Time.deltaTime);
            animator.SetFloat("Blend", 0.0f);
            if (isFirstPerson)
            {
                animator.SetFloat("BlendY", 0.0f);
            }
            else
            {
                animator.SetFloat("BlendY", 3f);
            }
            restJoyStick();
        }

        //if (!SelfieController.Instance.m_IsSelfieFeatureActive)
        //{
        //    
        //}
    }

    /// <summary>
    /// First person movement 
    /// </summary>
    public void FirstPersonCameraMove()
    {
        Vector2 movementInput = new Vector2(horizontal, vertical);

        Vector3 move = transform.right * movementInput.x + transform.forward * movementInput.y;
        _IsGrounded = characterController.isGrounded;
        animator.SetBool("IsGrounded", _IsGrounded);
        if (characterController.velocity.y < 0)
        {
            animator.SetBool("standJump", false);
        }
        //Debug.Log("MovmentInput:" + movementInput + "  :Move:" + move);
        if (animator != null && movementInput.sqrMagnitude >= inputThershold)
        {
            float horizontal1 = horizontal * 1.2f;
            if (horizontal1 > 1f)
            {
                horizontal1 = 1f;
            }
            else if (horizontal1 < -1f)
            {
                horizontal1 = -1f;
            }

            float vertical1 = vertical * 1.2f;
            if (vertical1 > 1f)
            {
                vertical1 = 1f;
            }
            else if (vertical1 < -1f)
            {
                vertical1 = -1f;
            }
            animator.SetFloat("Blend", horizontal1, speedSmoothTime, Time.deltaTime);
            animator.SetFloat("BlendY", vertical1, speedSmoothTime, Time.deltaTime);
        }
        else
        {
            animator.SetFloat("Blend", 0f);
            animator.SetFloat("BlendY", 0f);
        }


        if ((Input.GetKeyDown(KeyCode.LeftShift) || sprint_Button) && !sprint)
        {
            sprint = true;
            movementSpeed = sprintSpeed;
        }
        //  if (!sprint && movementSpeed == sprtintSpeed) 
        if ((Input.GetKeyUp(KeyCode.LeftShift) || !sprint_Button) && sprint)
        {
            sprint = false;
            movementSpeed = movementSpeedTemp;
        }

        float targetSpeed = movementSpeed * movementInput.magnitude;
        firstPersonCurrentSpeed = Mathf.SmoothDamp(firstPersonCurrentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);

        if (movementInput.sqrMagnitude >= inputThershold)
        {
            if (movementInput.sqrMagnitude >= sprintThresold)
            {
                //Debug.Log("Move Sprint:" + firstPersonSprintSpeed + "    :Move:" + move);
                characterController.Move(move * sprintSpeed * Time.deltaTime);
                velocity.y += gravity * Time.deltaTime;
                characterController.Move(velocity * Time.deltaTime);
                if (animator != null)
                {
                    animator.SetFloat("Blend", 0.25f * sprintSpeed, speedSmoothTime, Time.deltaTime);
                    animator.SetFloat("BlendY", 3f, speedSmoothTime, Time.deltaTime);
                }
            }
            else
            {
                //Debug.Log("Move current:" + firstPersonCurrentSpeed + "    :Move:" + move);
                PlayerIsWalking?.Invoke();
                UpdateSefieBtn(false);
                //if (Mathf.Abs(horizontal) > .5f || Mathf.Abs(vertical) > .5f)
                if ((Mathf.Abs(horizontal) <= .85f || Mathf.Abs(vertical) <= .85f))
                {
                    characterController.Move(move * firstPersonCurrentSpeed * Time.deltaTime);

                    velocity.y += gravity * Time.deltaTime;

                    if (animator != null)
                    {
                        float walkSpeed = 0.2f * currentSpeed; // Smoothing animator.
                        if (walkSpeed >= 0.2f && walkSpeed <= 0.45f) // changing walk speed to fix blend between walk and run.
                        {
                            walkSpeed = 0.15f;
                        }
                        animator.SetFloat("Blend", walkSpeed, speedSmoothTime, Time.deltaTime); // applying values to animator.
                        animator.SetFloat("BlendY", 3f, speedSmoothTime, Time.deltaTime); // applying values to animator.
                    }
                    characterController.Move(velocity * Time.deltaTime);
                }
                else if ((Mathf.Abs(horizontal) <= .001f || Mathf.Abs(vertical) <= .001f))
                {
                    //PlayerIsIdle?.Invoke();
                    //UpdateSefieBtn(!LoadFromFile.animClick);
                    if (animator != null)
                    {
                        animator.SetFloat("Blend", 0.23f * 0, speedSmoothTime, Time.deltaTime);
                        animator.SetFloat("BlendY", 3f, speedSmoothTime, Time.deltaTime);
                    }
                    if (!_IsGrounded)
                    {
                        characterController.Move(move * firstPersonCurrentSpeed * Time.deltaTime);
                        velocity.y += gravity * Time.deltaTime;
                        characterController.Move(velocity * Time.deltaTime);
                    }
                }
            }
        }
        else
        {
            PlayerIsIdle?.Invoke();
            UpdateSefieBtn(!LoadEmoteAnimations.animClick);

            characterController.Move(move * firstPersonCurrentSpeed * Time.deltaTime);
            velocity.y += gravity * Time.deltaTime;
            characterController.Move(velocity * Time.deltaTime);
            animator.SetFloat("Blend", 0.0f);
            animator.SetFloat("BlendY", 3f);
        }

        if ((animator.GetCurrentAnimatorStateInfo(0).IsName("NormalStatus") || animator.GetCurrentAnimatorStateInfo(0).IsName("Dwarf Idle") || animator.GetCurrentAnimatorStateInfo(0).IsName("Animation")) && (((Input.GetKeyDown(KeyCode.Space) || IsJumpButtonPress) && (characterController.isGrounded) && !animator.IsInTransition(0))/* || (jumpNow && allowJump && allowFpsJump && !IsJumping && characterController.isGrounded)*/)) // FPS jump
        {
            IsJumpButtonPress = false;
            allowFpsJump = false;
            jumpNow = false;
            allowJump = false;
            if (animator != null)
            {
                //animator.SetBool("IsJumping", true);
                tpsJumpAnim();
                IsJumping = true;
            }
            //move.y = 10;
            velocity.y = JumpVelocity;
            Vector3 diff = playerRig.transform.localPosition - camStartPosition;
            JumpTimePostion = firstPersonCameraObj.transform.localPosition;
            //CanvasJumpTimePostion = CanvasObject.transform.localPosition;
            //StartCoroutine(CanvasJump(diff));
            //Debug.Log("FirstPersonCameraMove jump");
            StartCoroutine(Jump(diff));
        }
        /*else if (characterController.isGrounded && velocity.y < 0 && !IsJumping)
        {
            animator.SetBool("IsJumping", false);
            animator.SetBool("standJump", false);
            jumpNow = false;
        }*/

        float values = animator.GetFloat("Blend");

        animationBlendValue = values;
    }

    /// <summary>
    /// FOR DUMMY JUMP BECAUSE OF PLAYER HEAD MOVE CONSTANTLY
    /// </summary>
    /// <param name="diff"> DIFF IS THE CAMERA START POSTION TO PALYER HEAD POSTION DIFFRENCE FOR JUMP END POINT </param>
    /// <returns></returns>
    IEnumerator Jump(Vector3 diff)
    {
        //Debug.Log("Jump");
        float progress = 0.0f;
        float d = camStartPosition.y + diff.y;

        float tempWait = 0.75f;
        if (horizontal != 0.0f || vertical != 0.0f) // is runing 
        {
            tempWait /= 3f;
        }
        //Debug.Log("jump TempWait:" + tempWait + "    :Horizontal:" + horizontal + "    :Vertical:" + vertical);

        while (progress < tempWait)//0.75f
        {
            progress += Time.deltaTime;
            JumpTimePostion.y = Mathf.Lerp(JumpTimePostion.y, d, progress);
            firstPersonCameraObj.transform.localPosition = JumpTimePostion; //Vector3.Lerp(firstPersonCameraObj.transform.position, endPos, progress);
            yield return null;
        }
        StartCoroutine(nameof(JumpEnd));
    }

    IEnumerator CanvasJump(Vector3 diff)
    {
        float progress = 0;

        float d = camStartPosition.y + diff.y;
        while (progress < 0.2f)
        {
            progress += Time.deltaTime;
            //CanvasJumpTimePostion.y = Mathf.Lerp(CanvasJumpTimePostion.y, d, progress);
            //CanvasObject.transform.localPosition = CanvasJumpTimePostion; //Vector3.Lerp(firstPersonCameraObj.transform.position, endPos, progress);
            yield return null;
        }
        // StartCoroutine(nameof(CanvasJumpEnd));
    }

    /// <summary>
    /// DUMMY JUMP END 
    /// </summary>
    /// <returns></returns>
    IEnumerator JumpEnd()
    {
        float progress = 0.0f;

        float tempWait = 0.35f;
        float tempWait2 = 0.3f;
        if (horizontal != 0.0f || vertical != 0.0f) // is runing 
        {
            tempWait /= 3f;
            tempWait2 /= 3f;
        }
        //Debug.Log("JumpEnd TempWait:" + tempWait + "   :TempWait2:" + tempWait2 + "    :Horizontal:"+horizontal + "    :Vertical:" + vertical);

        while (progress < tempWait)//0.35f
        {
            progress += Time.deltaTime;
            firstPersonCameraObj.transform.localPosition = Vector3.Lerp(firstPersonCameraObj.transform.localPosition, new Vector3(0f, 1.1f, 0.1f), progress);
            yield return null;
        }
        firstPersonCameraObj.transform.localPosition = new Vector3(0f, 1.1f, 0.1f);
        //Debug.Log("Jump end");
        Invoke(nameof(JumpNotAllowed), tempWait2);//0.3f
        allowFpsJump = true;
    }

    //IEnumerator CanvasJumpEnd()
    //{
    //    float progress = 0;
    //    while (progress < 0.2f)
    //    {
    //        progress += Time.deltaTime;
    //        CanvasObject.transform.localPosition = Vector3.Lerp(CanvasObject.transform.localPosition, new Vector3(0f, 0f, 0f), progress);
    //        yield return null;
    //    }
    //    CanvasObject.transform.localPosition = new Vector3(0f, 0f, 0f);
    //    //allowJump = true;
    //}


    public CharacterController FreeFloatCamCharacterController;
    public bool m_FreeFloatCam;
    private float Default;
    private float maxHeight;
    public float MaxHeightOffSet = 4.0f;

    public void GetPlayerTransform()
    {
        Default = FreeFloatCamCharacterController.transform.position.y;
        maxHeight = Default + MaxHeightOffSet;
    }

    private void ResetCamPos()
    {
        Vector3 newPos = new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z);

        FreeFloatCamCharacterController.transform.SetPositionAndRotation(newPos, transform.rotation);
    }

    public void FreeFloatMove()
    {
        Vector2 movementInput = new Vector2(horizontal, vertical);

        Vector3 moveDirection = (FreeFloatCamCharacterController.transform.right * movementInput.x + FreeFloatCamCharacterController.transform.forward * movementInput.y).normalized;

        FreeFloatCamCharacterController.Move(moveDirection * sprintSpeed * Time.deltaTime);

        if (FreeFloatCamCharacterController.transform.position.y < maxHeight)
        {

            FreeFloatCamCharacterController.Move(moveDirection * sprintSpeed * Time.deltaTime);
        }
        else
        {
            FreeFloatCamCharacterController.transform.position = new Vector3(FreeFloatCamCharacterController.transform.position.x, FreeFloatCamCharacterController.transform.position.y - 0.1f, FreeFloatCamCharacterController.transform.position.z);


        }


    }


    public void FreeFloatToggleButton(bool b)
    {
        ButtonsToggleOnOff(b);
    }

    public void ButtonsToggleOnOff(bool b)
    {
        m_FreeFloatCam = b;
        if (XanaConstants.xanaConstants.isBuilderScene)
        {
            if (isNinjaMotion)
            {
                isNinjaMotion = false;
                NinjaComponentTimerStart(0);
            }
            else if (isThrowModeActive)
            {
                StartCoroutine(nameof(ThrowEnd));
                isThrow = false;
                isThrowModeActive = false;
                BuilderEventManager.OnThrowThingsComponentDisable?.Invoke();
            }

            BuilderEventManager.StopAvatarChangeComponent?.Invoke(true);
        }
        FreeFloatCamCharacterController.gameObject.SetActive(b);
        animator.SetBool("freecam", b);
        animator.GetComponent<IKMuseum>().ConsoleObj.SetActive(isFirstPerson == true ? false : b);

        if (!b)
        {
            ResetCamPos();
            //RPC call
            animator.GetComponent<IKMuseum>().RPCForFreeCamDisable();
        }
        else
        {

            GetPlayerTransform();
            //RPC call
            animator.GetComponent<IKMuseum>().RPCForFreeCamEnable();
        }

        if (isFirstPerson)
            animator.GetComponent<IKMuseum>().m_ConsoleObjOther.SetActive(false);
        Debug.Log("FreeFloatCam" + FreeFloatCamCharacterController);
    }


    void Move()
    {
        if (isNinjaMotion)
        {
            if (!animator.GetBool("isNinjaMotion"))
                animator.SetBool("isNinjaMotion", true);

            //AnimationBehaviourNinjaMode();
            if (isMovementAllowed)
                NinjaMove();
            return;
        }

        if (!isMovementAllowed)
            return;

        if (isFirstPerson /*|| animator.GetBool("standJump")*/)
            return;

        SpecialItemDoubleJump();

        if (!controllerCamera.activeInHierarchy && (horizontal != 0 || vertical != 0))
        {
            controllerCamera.SetActive(true);
            if (controllerCharacterRenderCamera != null)
            {
                controllerCharacterRenderCamera.SetActive(true);
            }
        }

        _IsGrounded = characterController.isGrounded;
        if (_IsGrounded)
            canDoubleJump = false;

        animator.SetBool("IsGrounded", _IsGrounded);
        if (characterController.velocity.y < 0)
        {
            animator.SetBool("standJump", false);
        }

        Vector2 movementInput = new Vector2(horizontal, vertical);//new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.transform.right;

        forward.y = 0;
        right.y = 0;

        //    forward.Normalize();
        //    right.Normalize();

        Vector3 desiredMoveDirection = (forward * movementInput.y + right * movementInput.x).normalized;
        //Debug.Log("call hua for===="+ jumpNow + characterController.isGrounded + allowJump + Input.GetKeyDown(KeyCode.Space));
        //Debug.Log("MovmentInput:" + movementInput + "  :DesiredMoveDirection:" + desiredMoveDirection);
        if ((animator.GetCurrentAnimatorStateInfo(0).IsName("NormalStatus") || animator.GetCurrentAnimatorStateInfo(0).IsName("Dwarf Idle") || animator.GetCurrentAnimatorStateInfo(0).IsName("Animation")) && (((Input.GetKeyDown(KeyCode.Space) || IsJumpButtonPress) && characterController.isGrounded && !animator.IsInTransition(0))/* || (characterController.isGrounded && jumpNow && allowJump)*/))
        {
            if (ReferrencesForDynamicMuseum.instance.m_34player)
            {
                ReferrencesForDynamicMuseum.instance.m_34player.GetComponent<SoundEffects>().PlaySoundEffects(SoundEffects.Sounds.JumpSound);
            }
            allowJump = false;
            IsJumpButtonPress = false;
            //Debug.Log("call hua for 1==="+ jumpNow + characterController.isGrounded + allowJump + Input.GetKeyDown(KeyCode.Space));
            jumpNow = false;
            if (animator != null)
            {
                //animator.SetBool("IsJumping", true);
                tpsJumpAnim();
                IsJumping = true;
            }
            //Vector3 diff = playerRig.transform.localPosition - camStartPosition;
            //  CanvasJumpTimePostion = CanvasObject.transform.localPosition;
            //  gravityVector.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            //  StartCoroutine(CanvasJump(diff));
            if (!isFirstPerson)
            {
                if (horizontal != 0.0f || vertical != 0.0f) // is runing 
                {
                    Invoke(nameof(JumpNotAllowed), runingJumpResetInterval);
                }
                else // is idel jump
                {
                    Invoke(nameof(JumpNotAllowed), idelJumpResetInterval);
                }
            }
        }
        //else if (gravityVector.y < 0)
        //{
        //    //gravityVector = Vector3.zero;
        //    //gravityVector.y = 0;
        //    print("disabling jump from MOVE");
        //    animator.SetBool("IsJumping", false);
        //    jumpNow = false;
        //}

        //  if (sprint && movementSpeed!=sprtintSpeed)
        if ((Input.GetKeyDown(KeyCode.LeftShift) || sprint_Button) && !sprint)
        {
            sprint = true;
            movementSpeed = sprintSpeed;
        }

        //  if (!sprint && movementSpeed == sprtintSpeed) 
        if ((Input.GetKeyUp(KeyCode.LeftShift) || !sprint_Button) && sprint)
        {
            sprint = false;
            movementSpeed = movementSpeedTemp;
        }

        if (desiredMoveDirection != Vector3.zero && movementInput.sqrMagnitude >= inputThershold)
        {
            if (!isFirstPerson)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), rotationSpeed);
            }
        }

        float targetSpeed = movementSpeed * movementInput.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);

        //if (horizontal != 0.0f || vertical != 0.0f)
        //{
        //    animator.SetBool("isMoving", true);
        //}
        //else
        //{
        //    animator.SetBool("isMoving", false);
        //}

        //running condition
        //print("movementInput.sqrMagnitude " + movementInput.sqrMagnitude);
        if (movementInput.sqrMagnitude /*!=0.0f*/ >= inputThershold || IsJumping)
        {
            /*if (animator != null)
            {
               Debug.Log("FP Normal Movement.......");
                animator.SetFloat("Blend", horizontal, speedSmoothTime, Time.deltaTime);
                animator.SetFloat("BlendY", vertical, speedSmoothTime, Time.deltaTime);
            }*/

            if (movementInput.sqrMagnitude >= sprintThresold)
            {
                //Debug.Log("Move Sprint:" + sprtintSpeed + "    :DesiredMoveDirection:" + desiredMoveDirection);
                characterController.Move(desiredMoveDirection * sprintSpeed * Time.deltaTime);

                gravityVector.y += gravityValue * Time.deltaTime;

                characterController.Move(gravityVector * Time.deltaTime);
                //  if(sprint)
                if (animator != null)
                {
                    animator.SetFloat("Blend", 0.25f * sprintSpeed, speedSmoothTime, Time.deltaTime);
                    animator.SetFloat("BlendY", 3f, speedSmoothTime, Time.deltaTime);
                }
            }
            else// player is walking
            {
                //Debug.Log("Move Current:" + currentSpeed + "    :DesiredMoveDirection:" + desiredMoveDirection);
                PlayerIsWalking?.Invoke();
                UpdateSefieBtn(false);
                if ((Mathf.Abs(horizontal) <= .85f || Mathf.Abs(vertical) <= .85f)) // walk
                {
                    if (animator != null)
                    {
                        float walkSpeed = 0.2f * currentSpeed; // Smoothing animator.
                        if (walkSpeed >= 0.2f && walkSpeed <= 0.45f) // changing walk speed to fix blend between walk and run.
                        {
                            walkSpeed = 0.15f;
                        }
                        animator.SetFloat("Blend", walkSpeed, speedSmoothTime, Time.deltaTime); // applying values to animator.
                        animator.SetFloat("BlendY", 3f, speedSmoothTime, Time.deltaTime); // applying values to animator.
                    }
                    characterController.Move(desiredMoveDirection * currentSpeed * Time.deltaTime);

                    gravityVector.y += gravityValue * Time.deltaTime;

                    characterController.Move(gravityVector * Time.deltaTime);
                }
                else if ((Mathf.Abs(horizontal) <= .001f || Mathf.Abs(vertical) <= .001f))
                {
                    if (animator != null)
                    {
                        animator.SetFloat("Blend", 0.23f * 0, speedSmoothTime, Time.deltaTime);
                        animator.SetFloat("BlendY", 3f, speedSmoothTime, Time.deltaTime);
                    }
                    if (!_IsGrounded) // is in jump
                    {
                        characterController.Move(desiredMoveDirection * currentSpeed * Time.deltaTime);
                        gravityVector.y += gravityValue * Time.deltaTime;
                        characterController.Move(gravityVector * Time.deltaTime);
                    }
                    else // walk start state
                    {
                        characterController.Move(desiredMoveDirection * currentSpeed * Time.deltaTime);
                        gravityVector.y += gravityValue * Time.deltaTime;
                        characterController.Move(gravityVector * Time.deltaTime);
                    }
                }
            }
        }
        else // Reseating animator to idel when joystick is not moving.
        {
            PlayerIsIdle?.Invoke();
            UpdateSefieBtn(!LoadEmoteAnimations.animClick);
            characterController.Move(desiredMoveDirection * currentSpeed * Time.deltaTime);
            gravityVector.y += gravityValue * Time.deltaTime;
            characterController.Move(gravityVector * Time.deltaTime);

            animator.SetFloat("Blend", 0.0f);
            animator.SetFloat("BlendY", 3f);
        }

        if (horizontal > 0.4f || vertical > 0.4f)
        {
            canSend = false;
            // SocketConnection.instance.sendMovementData(movementInput.magnitude,transform.position, transform.rotation);
            // Debug.LogWarning("Sending Gooooo");
        }
        else
        {
            if (!canSend)
            {
                //  Debug.LogWarning("Sending Stop");
                // SocketConnection.instance.sendMovementData(0f ,transform.position, transform.rotation);
            }
            canSend = true;
        }

        float values = animator.GetFloat("Blend");

        animationBlendValue = values;
    }

    bool lastSelfieCanClick = false;
    void UpdateSefieBtn(bool canClick)
    {
        if (canClick != lastSelfieCanClick)
        {
            lastSelfieCanClick = canClick;
            if (GamePlayButtonEvents.inst != null)
            {
                GamePlayButtonEvents.inst.UpdateSelfieBtn(canClick);
            }
        }
    }

    public void animationPlay()
    {
        //Debug.Log("persistentdata pat");
        //AvatarManager.Instance.loadassetsstreming(); 
    }

    public void OnMoveInput(float horizontal, float vertical)
    {
        if (horizontal != 0.0f || vertical != 0.0f)
        {
            this.vertical = vertical;
            this.horizontal = horizontal;
        }
        else
        {
            this.vertical = 0.0f;
            this.horizontal = 0.0f;
        }
        
    }

    void ClientEnd(float animationFloat, Transform transformPos)
    {
        this.transform.position = transformPos.position;
        animator.SetFloat("Blend", 0.5f * animationFloat, speedSmoothTime, Time.deltaTime);
        //animator.SetFloat("BlendY", 0.5f * animationFloat, speedSmoothTime, Time.deltaTime);
    }

    public void PlayerJump(bool jump)
    {
        //jumpNow = jump;
    }

    public void Jump()
    {
        if (EmoteAnimationPlay.Instance.animatorremote != null && ReferrencesForDynamicMuseum.instance.m_34player.GetComponent<Animator>().GetBool("EtcAnimStart"))    //Added by Ali Hamza
            ReferrencesForDynamicMuseum.instance.m_34player.GetComponent<RpcManager>().BackToIdleAnimBeforeJump();

        if (ReferrencesForDynamicMuseum.instance.m_34player)
        {
            ReferrencesForDynamicMuseum.instance.m_34player.GetComponent<SoundEffects>().PlaySoundEffects(SoundEffects.Sounds.JumpSound);
        }

        if (_IsGrounded)
        {
            IsJumpButtonPress = true;
        }
        else if (!_IsGrounded && specialItem && !canDoubleJump)
        {
            canDoubleJump = true;
            gravityVector.y = JumpVelocity * 2;
        }


    }

    public void JumpAllowed()
    {
        if (isFirstPerson)
        {
            if (!IsJumping && characterController.isGrounded)
            {
                jumpNow = true;
                IsJumping = true;

                //tpsJumpAnim();
                //Debug.Log("JumpAllowed");
                //jump camera start...
                Vector3 diff = playerRig.transform.localPosition - camStartPosition;
                JumpTimePostion = firstPersonCameraObj.transform.localPosition;
                StartCoroutine(Jump(diff));
                //...
                velocity.y = JumpVelocity;
                CameraLook.instance.DisAllowControl();
                //Invoke(nameof(JumpNotAllowed), 0.1f);
            }
        }
        else
        {
            if (!IsJumping && allowJump && characterController.isGrounded/*&& ( allowJump || isFirstPerson )*/ )
            {
                allowJump = false;
                jumpNow = true;
                //tpsJumpAnim();
                CameraLook.instance.DisAllowControl();
                if (isFirstPerson)
                {
                    //Debug.Log("JumpAllowed1111111");
                    Invoke(nameof(JumpNotAllowed), 1.3f);
                }
                else
                {
                    if (horizontal != 0.0f || vertical != 0.0f) // is runing 
                    {
                        Invoke(nameof(JumpNotAllowed), runingJumpResetInterval);
                    }
                    else // is idel jump
                    {
                        Invoke(nameof(JumpNotAllowed), idelJumpResetInterval);
                    }
                }
            }
        }

        if (EmoteAnimationPlay.Instance.isAnimRunning)
        {
            EmoteAnimationPlay.Instance.StopAnimation();
            EmoteAnimationPlay.Instance.StopAllCoroutines();
        }

    }

    public void JumpNotAllowed()
    {
        //Debug.Log("JumpNotAllowed");
        IsJumping = false;
        jumpNow = false;
        allowJump = true;

        animator.SetBool("IsJumping", false);
        //animator.SetBool("standJump", false);
        CameraLook.instance.AllowControl();
    }

    public void PlayerSprint(bool sprint)
    {
        //  Debug.Log("PlayerController : Move Input sprint" + sprint );
        sprint_Button = sprint;
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)// check is game paused
        {
            restJoyStick();
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            StartCoroutine(OnFocus());
        }
    }

    IEnumerator OnFocus()
    {
        yield return new WaitForSeconds(.5f);
        if (isFirstPerson)
        {
            if (animator != null)
            {
                if (SelfieController.Instance != null && !SelfieController.Instance.m_IsSelfieFeatureActive)
                    DisablePlayerOnFPS();
            }
            else
            {
                SwitchCameraButton();
            }
        }
    }

    /// <summary>
    /// To rest Joystick positoin and input parameters.
    /// </summary>
    public void restJoyStick()
    {
        this.horizontal = 0.0f;
        this.vertical = 0.0f;

        innerJoystick.anchoredPosition = Vector3.zero;
        innerJoystick_Portrait.anchoredPosition = Vector3.zero;

        innerJoystick.GetComponent<JoyStickIssue>().ResetJoyStick();
        innerJoystick_Portrait.GetComponent<JoyStickIssue>().ResetJoyStick();

        characterController.Move(Vector3.zero);
        //JumpNotAllowed();
        //StopCoroutine(nameof(Jump));
        //StopCoroutine(nameof(JumpEnd));
    }

    void tpsJumpAnim()
    {
        // Play TPS animation according to the movement speed 
        if (horizontal != 0.0f || vertical != 0.0f)
        {
            if (animator.GetBool("IsEmote"))
            {
                animator.SetBool("IsEmote", false);
            }
            /*if (!animator.GetBool("IsJumping"))
                animator.SetBool("IsJumping", true);*/
            animator.SetBool("standJump", true);
        }
        else
        {
            if (animator.GetBool("IsEmote"))
            {
                animator.SetBool("IsEmote", false);
            }
            if (!animator.GetBool("standJump"))
            {
                animator.SetBool("standJump", true);
            }
        }
        Invoke(nameof(UpdateVelocity), .1f);

        if (EmoteAnimationPlay.Instance.isAnimRunning)
        {
            EmoteAnimationPlay.Instance.StopAnimation();
            EmoteAnimationPlay.Instance.StopAllCoroutines();
        }
    }

    public void UpdateVelocity()
    {
        gravityVector.y = JumpVelocity;
    }


    //update player jump according to builder setting 
    void PlayerJumpUpdate(float jumpValue, float playerSpeed)
    {
        sprintSpeed = 5;
        JumpVelocity += (jumpValue - 1);
        sprintSpeed += (playerSpeed - 1);
        speedMultiplier = playerSpeed;
        jumpMultiplier = jumpValue;
    }

    void SpecialItemPlayerPropertiesUpdate(float jumpValue, float playerSpeed)
    {
        JumpVelocity = jumpValue;
        sprintSpeed = playerSpeed;
    }

    /// <SpecialItemDoubleJump>
    public bool specialItem = false;
    bool canDoubleJump = false;
    void SpecialItemDoubleJump()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || IsJumpButtonPress) && !_IsGrounded && !canDoubleJump && specialItem)
        {
            canDoubleJump = true;
            Debug.Log("Double jump testing ");
            gravityVector.y = JumpVelocity * 2;
        }
    }
    /// </summary>


    public bool isNinjaMotion = false;
    public bool isMovementAllowed = true;
    public bool isThrow = false;
    ///////////////////////////////////////////
    ///Ninja Move
    //////////////////////////////////////////
    ///
    [SerializeField] private GameObject _shurikenPrefab, swordModel;
    [SerializeField] private Transform _ballSpawn;
    [SerializeField] private Transform swordHook, swordhandHook;

    bool isDrawSword = false;
    float timeToWalk = 0;
    void NinjaMove()
    {
        if (isFirstPerson /*|| animator.GetBool("standJump")*/)
            return;

        _IsGrounded = characterController.isGrounded;
        animator.SetBool("NinjaJump", _IsGrounded);
        animator.SetBool("IsGrounded", _IsGrounded);
        Vector2 movementInput = new Vector2(horizontal, vertical);
        Vector3 forward = controllerCamera.transform.forward;
        Vector3 right = controllerCamera.transform.right;
        forward.y = 0;
        right.y = 0;


        Vector3 desiredMoveDirection = (forward * movementInput.y + right * movementInput.x).normalized;


        if ((animator.GetCurrentAnimatorStateInfo(0).IsName("NinjaTree") && (Input.GetKeyDown(KeyCode.Space) || IsJumpButtonPress) && characterController.isGrounded))
        {
            allowJump = false;
            IsJumpButtonPress = false;
            print("Jump Key Press");
            if (animator != null)
            {
                animator.SetFloat("BlendNX", 0.5f, 0.25f, Time.deltaTime);
                animator.SetFloat("BlendNY", 0.5f, 0.25f, Time.deltaTime);
                tpsJumpAnim();
                IsJumping = true;
            }
            jumpNow = false;
            if (!isFirstPerson)
            {
                if (horizontal != 0.0f || vertical != 0.0f) // is runing 
                {
                    Invoke(nameof(JumpNotAllowed), runingJumpResetInterval);
                }
                else // is idel jump
                {
                    Invoke(nameof(JumpNotAllowed), idelJumpResetInterval);
                }
            }
        }

        if ((Input.GetKeyDown(KeyCode.LeftShift) || sprint_Button) && !sprint)
        {
            sprint = true;
            movementSpeed = sprintSpeed + 2;
        }

        if ((Input.GetKeyUp(KeyCode.LeftShift) || !sprint_Button) && sprint)
        {
            sprint = false;
            movementSpeed = sprintSpeed;
        }

        if (desiredMoveDirection != Vector3.zero && movementInput.sqrMagnitude >= inputThershold)
        {
            if (!isFirstPerson)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), rotationSpeed);
            }
        }

        float targetSpeed = movementSpeed * movementInput.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);




        //running condition
        if (movementInput.sqrMagnitude /*!=0.0f*/ >= inputThershold)
        {

            if (movementInput.sqrMagnitude >= sprintThresold && sprint)
            {
                //throw
                AnimationBehaviourNinjaMode();
                characterController.Move(desiredMoveDirection * movementSpeed * Time.deltaTime);

                gravityVector.y += gravityValue * Time.deltaTime;
                characterController.Move(gravityVector * Time.deltaTime);

                if (animator != null)
                {
                    animator.SetFloat("BlendNX", 0.8f, 0.25f, Time.deltaTime);
                    animator.SetFloat("BlendNY", 0f, 0.25f, Time.deltaTime);
                }

            }
            else// player is walking
            {

                PlayerIsWalking?.Invoke();

                if ((Mathf.Abs(horizontal) <= .85f || Mathf.Abs(vertical) <= .85f)) // walk
                {
                    timeToWalk += Time.deltaTime;

                    if (animator != null)
                    {
                        float walkSpeed = 0.2f * currentSpeed; // Smoothing animator.
                        if (walkSpeed >= 0.2f && walkSpeed <= 0.45f) // changing walk speed to fix blend between walk and run.
                        {
                            walkSpeed = 0.15f;
                        }
                        if (timeToWalk <= 3)
                        {
                            //movementSpeed = finalWalkSpeed;
                            animator.SetFloat("BlendNX", 0.6f, 0.25f, Time.deltaTime); // applying values to animator.
                            animator.SetFloat("BlendNY", 0f, 0.25f, Time.deltaTime);
                        }
                        if (timeToWalk > 3)
                        {
                            //movementSpeed = finalWalkSpeed + 1;
                            animator.SetFloat("BlendNX", 0.6f, 0.25f, Time.deltaTime); // applying values to animator.
                            animator.SetFloat("BlendNY", 0f, 0.25f, Time.deltaTime); // applying values to animator.
                        }


                    }
                    //throw
                    AnimationBehaviourNinjaMode();
                    characterController.Move(desiredMoveDirection * currentSpeed * Time.deltaTime);


                    gravityVector.y += gravityValue * Time.deltaTime;
                    characterController.Move(gravityVector * Time.deltaTime);
                }
                else if ((Mathf.Abs(horizontal) <= .001f || Mathf.Abs(vertical) <= .001f))
                {
                    //standing jump

                    if (!_IsGrounded) // is in jump
                    {
                        characterController.Move(desiredMoveDirection * currentSpeed * Time.deltaTime);
                        gravityVector.y += gravityValue * Time.deltaTime;
                        characterController.Move(gravityVector * Time.deltaTime);
                    }
                    else // walk start state
                    {
                        animator.SetFloat("BlendNX", 0.001f, 0.2f, Time.deltaTime);
                        animator.SetFloat("BlendNY", 0.001f, 0.2f, Time.deltaTime);
                        characterController.Move(desiredMoveDirection * currentSpeed * Time.deltaTime);
                        gravityVector.y += gravityValue * Time.deltaTime;
                        characterController.Move(gravityVector * Time.deltaTime);
                    }
                }
            }
        }
        else // Reseating animator to idel when joystick is not moving.
        {
            PlayerIsIdle?.Invoke();
            AnimationBehaviourNinjaMode();
            characterController.Move(desiredMoveDirection * currentSpeed * Time.deltaTime);
            gravityVector.y += gravityValue * Time.deltaTime;
            characterController.Move(gravityVector * Time.deltaTime);
            timeToWalk = 0;
            animator.SetFloat("animationSpeedMultiplier", 1);
            animator.SetFloat("BlendNX", 0f, 0.3f, Time.deltaTime);
            animator.SetFloat("BlendNY", 0f, 0.3f, Time.deltaTime);
        }
    }

    void AnimationBehaviourNinjaMode()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.E))
            attackwithSword = true;
#endif

        if (attackwithSword && animator.GetCurrentAnimatorStateInfo(0).IsName("NinjaTree") && isNinjaMotion && isDrawSword)
        {
            StartCoroutine(NinjaAttack());
        }

        if (attackwithSword && animator.GetCurrentAnimatorStateInfo(0).IsName("NinjaAttack") && isNinjaMotion && isDrawSword)
        {
            StopCoroutine(NinjaAttack());
            StartCoroutine(NinjaAttack2());
        }

        if (attackwithSword && animator.GetCurrentAnimatorStateInfo(0).IsName("NinjaAmimationSlash3") && isNinjaMotion && isDrawSword)
        {
            StopCoroutine(NinjaAttack2());
            StartCoroutine(NinjaAttack3());
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Q))
            hideoropenSword = true;
#endif

        if (hideoropenSword && animator.GetCurrentAnimatorStateInfo(0).IsName("NinjaTree") && isNinjaMotion)
        {

            isDrawSword = !isDrawSword;
            StartCoroutine(DrawNinjaSword());

        }

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(1))
            attackwithShuriken = true;
#endif


        if (attackwithShuriken && animator.GetCurrentAnimatorStateInfo(0).IsName("NinjaTree") && isNinjaMotion)
        {
            animator.SetBool("NinjaThrow", true);
            StartCoroutine(ThrowFalse());
        }
        attackwithSword = false;
        attackwithShuriken = false;
        hideoropenSword = false;
    }

    IEnumerator NinjaAttack()
    {
        //StopCoroutine(NinjaAttack());
        isMovementAllowed = false;
        animator.CrossFade("NinjaAttack", 0.1f);
        yield return new WaitForSecondsRealtime(0.8f);
        isMovementAllowed = true;
    }
    IEnumerator NinjaAttack2()
    {
        //StopCoroutine(NinjaAttack());
        isMovementAllowed = false;
        animator.CrossFade("NinjaAmimationSlash3", 0.1f);
        yield return new WaitForSecondsRealtime(1f);
        isMovementAllowed = true;


    }
    IEnumerator NinjaAttack3()
    {
        //StopCoroutine(NinjaAttack());
        isMovementAllowed = false;
        animator.CrossFade("Sword And Shield Attack", 0.1f);
        yield return new WaitForSecondsRealtime(1.5f);
        isMovementAllowed = true;


    }

    IEnumerator DrawNinjaSword()
    {
        if (isDrawSword)
        {
            isMovementAllowed = false;
            animator.CrossFade("SheathingSword", 0.2f);
            yield return new WaitForSecondsRealtime(0.8f);
            swordModel.transform.SetParent(swordhandHook, false);
            yield return new WaitForSecondsRealtime(0.1f);
            swordModel.transform.localPosition = new Vector3(0.0729999989f, -0.0329999998f, -0.0140000004f);
            swordModel.transform.localRotation = new Quaternion(0.725517809f, 0.281368196f, -0.0713528395f, 0.623990953f);
            isMovementAllowed = true;


        }
        if (!isDrawSword)
        {
            isMovementAllowed = false;
            animator.CrossFade("Withdrawing", 0.2f);
            yield return new WaitForSecondsRealtime(1.3f);
            swordModel.transform.SetParent(swordHook, false);
            swordModel.transform.localPosition = new Vector3(-0.149000004f, 0.0500000007f, 0.023f);
            swordModel.transform.localRotation = new Quaternion(-0.149309605f, -0.19390057f, 0.966789007f, 0.0736774057f);
            isMovementAllowed = true;
        }
    }
    IEnumerator ThrowFalse()
    {
        isMovementAllowed = false;
        yield return new WaitForSeconds(0.6f);
        GameObject spawned = Instantiate(_shurikenPrefab, _ballSpawn.position, Quaternion.Euler(90, 90, 0));
        spawned.GetComponent<Rigidbody>().AddForce((transform.forward * 3000f) * 0.25f);
        //(cinemachineFreeLook.m_YAxis.Value<=0.1f?0.1f: cinemachineFreeLook.m_YAxis.Value));
        //Toast.Show(cinemachineFreeLook.m_YAxis.Value+"");
        animator.SetBool("NinjaThrow", false);
        yield return new WaitForSeconds(0.4f);
        isMovementAllowed = true;
        Destroy(spawned, 5f);
    }

    Coroutine NinjaCo;
    public void NinjaComponentTimerStart(float time)
    {
        if (NinjaCo != null)
        {
            StopCoroutine(NinjaCo);
            NinjaCo = null;
        }
        //else
        //    return;
        if (isThrowModeActive)
        {
            isThrow = false;
            isThrowModeActive = false;
        }

        if (throwMainCo != null)
            throwMainCo = null;
        BuilderEventManager.OnNinjaMotionComponentCollisionEnter?.Invoke(time);
        NinjaCo = StartCoroutine(NinjaComponentTimer(time));
    }
    public IEnumerator NinjaComponentTimer(float time)
    {
        isDrawSword = false;
        if (swordModel && time != 0)
        {
            swordModel.transform.SetParent(swordHook, false);
            swordModel.transform.localPosition = new Vector3(-0.149000004f, 0.0500000007f, 0.023f);
            swordModel.transform.localRotation = new Quaternion(-0.149309605f, -0.19390057f, 0.966789007f, 0.0736774057f);
            swordModel.SetActive(true);
        }
        yield return new WaitForSeconds(time);
        isNinjaMotion = false;
        if (swordModel)
        {
            swordModel.SetActive(false);
        }
        animator.SetBool("NinjaJump", true);
        animator.SetBool("isNinjaMotion", false);
        animator.SetFloat("Blend", 0f, 0.0f, Time.deltaTime); // applying values to animator.
        animator.SetFloat("BlendY", 3f, 0.0f, Time.deltaTime);
        //Ninja_Throw(false);
        isDrawSword = false;
        JumpVelocity = originalJumpSpeed + (jumpMultiplier - 1);
        sprintSpeed = originalSprintSpeed + (speedMultiplier - 1);
    }
    bool attackwithSword, attackwithShuriken, hideoropenSword;
    void AttackwithSword() => attackwithSword = true;
    void AttackwithShuriken() => attackwithShuriken = true;
    void HideorOpenSword() => hideoropenSword = true;


    /// <summary>
    /// Throw Mode
    /// </summary>
    /// 
    [SerializeField] private LineRenderer throwLineRenderer;
    [SerializeField] private GameObject handBall;
    [SerializeField] private TrajectoryController trajectoryController;
    [SerializeField] private float _force = 20;
    Coroutine throwCo;
    public LayerMask hitMask;
    public float raycastDistance = 100f;
    private Coroutine throwMainCo;
    public bool isThrowModeActive = false;

    public void ThrowMotion()
    {
        trajectoryController = this.GetComponent<TrajectoryController>();
        throwLineRenderer = this.GetComponent<LineRenderer>();
        if (isNinjaMotion)
        {
            NinjaComponentTimerStart(0);
            isNinjaMotion = false;
            animator.SetBool("isNinjaMotion", false);
        }
        isThrow = true;
        isThrowModeActive = true;
        if (throwMainCo == null)
            throwMainCo = StartCoroutine(Throw());
    }
    Vector3 tempRotation, tempPostion;
    public float timeToStartAimLineRenderer, timeToStopAimLineRenderer;
    private Coroutine throwStart, throwEnd, throwAction;
    bool isThrowReady = false;
    public Vector3 curveOffset;
    internal bool isThrowPose = true;

    IEnumerator Throw()
    {
        while (isThrowModeActive)
        {
            yield return new WaitForSeconds(0f);
            if (!isNinjaMotion && _IsGrounded)
            {


                if (isThrow && isThrowReady)
                {
                    tempRotation = ActiveCamera.transform.eulerAngles;
                    tempRotation.x = this.transform.eulerAngles.x;
                    tempRotation.z = this.transform.eulerAngles.z;
                    this.transform.eulerAngles = tempRotation;
                    //Debug.Log("Throw Pose Active");
                    trajectoryController.UpdateTrajectory(_ballSpawn.position, (ActiveCamera.transform.forward + curveOffset) * _force);
                    throwLineRenderer.enabled = true;
                    trajectoryController.colliderAim.SetActive(true);
                    handBall.SetActive(true);
                }
                else
                {
                    throwLineRenderer.enabled = false;
                    trajectoryController.colliderAim.SetActive(false);
                    handBall.SetActive(false);
                }

                //Debug.Log("Throw Mode Active");

#if UNITY_EDITOR
                if (Input.GetKeyDown(KeyCode.Q) && throwAction == null)
                    throwBallPositionSet = true;

                if (Input.GetKeyDown(KeyCode.E) && isThrowReady)
                    throwBall = true;
#endif

                if (throwBallPositionSet && !animator.GetCurrentAnimatorStateInfo(0).IsName("throwing") && throwAction == null)
                {
                    if (isThrowPose && throwEnd == null && (animator.GetCurrentAnimatorStateInfo(0).IsName("NormalStatus") || animator.GetCurrentAnimatorStateInfo(0).IsName("Dwarf Idle")))
                    {
                        isThrowPose = !isThrowPose;
                        if (throwStart == null)
                        {
                            throwStart = StartCoroutine(ThrowStart());
                        }
                    }
                    if (!isThrowPose && animator.GetCurrentAnimatorStateInfo(0).IsName("throw") && throwStart == null && throwAction == null)
                    {
                        isThrowPose = !isThrowPose;
                        if (throwEnd == null)
                        {
                            throwEnd = StartCoroutine(ThrowEnd());
                        }
                    }
                    throwBallPositionSet = false;
                }

                if (throwBall && isThrowReady && throwStart == null && throwEnd == null)
                {
                    isThrowReady = false;
                    if (throwAction == null && animator.GetCurrentAnimatorStateInfo(0).IsName("throw"))
                    {
                        throwAction = StartCoroutine(ThrowAction());
                    }
                    throwBall = false;
                }
            }
        }
    }
    IEnumerator ThrowStart()
    {
        Debug.Log("Throw Start Co");
        animator.SetFloat("Blend", 0f, 0.2f, Time.deltaTime); // applying values to animator.
        animator.SetFloat("BlendY", 3f, 0.2f, Time.deltaTime);
        animator.SetBool("throw", true);
        isMovementAllowed = false;
        //DOTween.To(() => cameraController.tpCamera._camera.fieldOfView, x => cameraController.tpCamera._camera.fieldOfView = x, 45, 1).SetUpdate(true);
        yield return new WaitForSeconds(0.7f);
        handBall.SetActive(true);
        isThrowReady = true;
        StopCoroutine(throwStart);
        throwStart = null;
    }
    IEnumerator ThrowAction()
    {
        //throwLineRenderer.enabled = false;
        //handBall.SetActive(false);
        //trajectoryController.colliderAim.SetActive(false);
        animator.SetBool("throw", false);
        animator.SetBool("throwing", true);
        Debug.Log("Throw Action Co");
        //if (isThrowReady)
        //{
        var spawned = Instantiate(GamificationComponentData.instance.ThrowBall, handBall.transform.position, handBall.transform.rotation);
        spawned.Init(((ActiveCamera.transform.forward + curveOffset) * _force), false);
        Destroy(spawned.gameObject, 10);
        //}
        //isThrowReady = false;
        yield return new WaitForSeconds(1f);
        animator.SetBool("throw", true);
        animator.SetBool("throwing", false);
        yield return new WaitForSeconds(0.7f);
        //handBall.SetActive(true);
        //trajectoryController.colliderAim.SetActive(true);
        isThrowReady = true;
        StopCoroutine(throwAction);
        throwAction = null;
    }
    public IEnumerator ThrowEnd()
    {
        Debug.Log("Throw End Co");
        yield return new WaitForSeconds(0f);
        //throwLineRenderer.enabled = false;
        //handBall.SetActive(false);
        //trajectoryController.colliderAim.SetActive(false);
        animator.SetBool("throw", false);
        animator.SetBool("throwFalse", false);
        animator.SetFloat("Blend", 0f, 0.0f, Time.deltaTime); // applying values to animator.
        animator.SetFloat("BlendY", 3f, 0.0f, Time.deltaTime);
        isThrowReady = false;

        yield return new WaitForSeconds(01.3f);
        isMovementAllowed = true;
        if (throwEnd != null)
            StopCoroutine(throwEnd);
        throwEnd = null;
    }

    internal void ThrowThingsEnded()
    {
        StartCoroutine(ThrowEnd());
        isThrow = false;
        isThrowModeActive = false;
        BuilderEventManager.OnThrowThingsComponentDisable?.Invoke();
    }

    bool throwBallPositionSet, throwBall;

    void BallPositionSet()
    {
        if (throwAction == null)
            throwBallPositionSet = true;
    }
    void ThrowBall()
    {
        if (isThrowReady)
            throwBall = true;
    }

    public void Ninja_Throw(bool state, int index = 0)
    {
        if (swordModel == null)
        {
            swordModel = Instantiate(GamificationComponentData.instance.katanaPrefab, transform.parent);
            swordModel.transform.localPosition = Vector3.zero;
            swordModel.transform.localScale = Vector3.one;
            swordModel.SetActive(false);
        }

        _shurikenPrefab = GamificationComponentData.instance.shurikenPrefab;
        swordhandHook = GamificationComponentData.instance.ikMuseum.m_SelfieStick.transform.parent;
        swordHook = GamificationComponentData.instance.charcterBodyParts.PelvisBone.transform;
        _ballSpawn = swordhandHook;
        if (trajectoryController == null)
            trajectoryController = gameObject.AddComponent<TrajectoryController>();
        trajectoryController.resolution = 300;
        trajectoryController.distance = 4;
        trajectoryController.aimCollsion = GamificationComponentData.instance.throwAimPrefab;
        trajectoryController.colliderAim = GamificationComponentData.instance.throwAimPrefab;

        LineRenderer lr = GetComponent<LineRenderer>();
        lr.enabled = false;
        lr.widthMultiplier = 0.05f;
        lr.material = GamificationComponentData.instance.lineMaterial;
        lr.numCornerVertices = 30;
        lr.numCapVertices = 31;
        curveOffset = Vector3.up * 0.7f;

        if (handBall == null)
        {
            handBall = Instantiate(GamificationComponentData.instance.handBall, swordhandHook);
            handBall.transform.localPosition = new Vector3(0.08f, -0.088f, -0.006f);
            handBall.transform.localRotation = Quaternion.Euler(0, -25.06f, 0);
            handBall.SetActive(false);
        }

        _force = 15f;

        if (index == 0)
        {
            isNinjaMotion = true;
            animator.SetBool("isNinjaMotion", true);
        }
        else
            ThrowMotion();
    }
}