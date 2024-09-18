using Cinemachine;
using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
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
    
    public float YourDownhillThreshold = 30f; // Adjust slope Threshold 
    public float CurrentSlope = 0f;
    private readonly float _rayOffsett = 0.5f;

    //[SerializeField]
    public float jumpHeight = 1.0f;
    public Transform cameraTransform = null;
    //public Transform cameraCharacterTransform = null;
    //public GameObject cmVcam;

    public bool sprint, _IsGrounded, jumpNow, sprint_Button, IsJumping,DebugColloision;

    internal CharacterController characterController = null;

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
    public static Action PlayerIsWalking;
    public static Action PlayerIsIdle;
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
    internal CinemachineFreeLook cinemachineFreeLook;
    float topRigDefaultRadius;

    internal float animationBlendValue = 0;
    internal Vector3 desiredMoveDirection;
    internal Vector3 desiredMoveDirectionFPP;

    private void OnEnable()
    {
        BuilderEventManager.OnHideOpenSword += HideorOpenSword;
        BuilderEventManager.OnAttackwithSword += AttackwithSword;
        BuilderEventManager.OnAttackwithShuriken += AttackwithShuriken;
        BuilderEventManager.OnThowThingsPositionSet += BallPositionSet;
        BuilderEventManager.OnThrowBall += ThrowBall;

        //Update jump height according to builder
        BuilderEventManager.ApplyPlayerProperties += PlayerJumpUpdate;
        BuilderEventManager.AfterPlayerInstantiated += RemoveLayerFromCameraCollider;
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
        BuilderEventManager.AfterPlayerInstantiated -= RemoveLayerFromCameraCollider;
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
        ActiveCamera = ReferencesForGamePlay.instance.randerCamera.gameObject;


        ////Update jump height according to builder
        //BuilderEventManager.ApplyPlayerProperties += PlayerJumpUpdate;

        RemoveLayerFromCameraCollider();

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (DebugColloision)
        {
            Debug.LogError("Colloided    " + collision.gameObject.name);
        }
    }

    private void RemoveLayerFromCameraCollider()
    {
        if (ConstantsHolder.xanaConstants.isBuilderScene)
        {
            CinemachineCollider cinemachineCollider = GameplayEntityLoader.instance.PlayerCamera.GetComponent<CinemachineCollider>();
            if (cinemachineCollider != null)
            {
                int noPostProcessingLayerIndex = LayerMask.NameToLayer("NoPostProcessing");
                //int characterLayerIndex = LayerMask.NameToLayer("Character");
                // Remove the layer from the collide against mask
                cinemachineCollider.m_CollideAgainst &= ~(1 << noPostProcessingLayerIndex);
                //cinemachineCollider.m_CollideAgainst &= ~(1 << characterLayerIndex);
            }
            cinemachineFreeLook = GameplayEntityLoader.instance.PlayerCamera.GetComponent<CinemachineFreeLook>();
            topRigDefaultRadius = cinemachineFreeLook.m_Orbits[0].m_Radius;
        }
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

            //SoundController.Instance.MusicSource.volume = 0;
            //  SoundController.Instance.MusicSource.mute = false;
        }

        if (other.tag == "YoutubeVideo")
        {
            Gamemanager._InstanceGM.ytVideoPlayer.SetDirectAudioVolume(0, 1);
            Gamemanager._InstanceGM.ytVideoPlayer.SetDirectAudioMute(0, false);
            SoundController.Instance.MusicSource.volume = 0;
            SoundController.Instance.MusicSource.mute = false;
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
            SoundController.Instance.MusicSource.volume = 0.19f;
        }

        if (other.tag == "YoutubeVideo")
        {
            Gamemanager._InstanceGM.ytVideoPlayer.SetDirectAudioVolume(0, 0);
            Gamemanager._InstanceGM.ytVideoPlayer.SetDirectAudioMute(0, true);
            SoundController.Instance.MusicSource.mute = false;
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
    Coroutine coroutine;
    // Toogle camera first person to therd person
    public void SwitchCameraButton()
    {
        //Debug.Log("0");

        isFirstPerson = !isFirstPerson;
        gravityVector.y = 0;

        GamePlayUIHandler.inst.OnChangehighlightedFPSbutton(isFirstPerson);
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
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
            coroutine = StartCoroutine(FadeImage(true));
            OnInvokeCameraChange(firstPersonCameraObj.GetComponent<Camera>());
            //gameObject.transform.localScale = new Vector3(0, 1, 0);
            DisablePlayerOnFPS();
            controllerCamera.SetActive(false);
            firstPersonCameraObj.tag = "MainCamera";
            ActiveCamera = firstPersonCameraObj;
            // MuseumRaycaster.instance.playerCamera = firstPersonCameraObj.GetComponent<Camera>();
            //animator.gameObject.GetComponent<PhotonAnimatorView>().m_SynchronizeParameters[animator.gameObject.GetComponent<PhotonAnimatorView>().m_SynchronizeParameters.Count - 1].SynchronizeType = PhotonAnimatorView.SynchronizeType.Continuous;
        }
        else
        {
            //Debug.Log("2");
            //MuseumRaycaster.instance.playerCamera = ReferencesForGamePlay.instance.randerCamera;
            gyroButton.SetActive(false);
            gyroButton_Portait.SetActive(false);

            firstPersonCameraObj.tag = "FirstPersonCamera";
            firstPersonCameraObj.SetActive(false);
            coroutine = StartCoroutine(FadeImage(true));
            OnInvokeCameraChange(ReferencesForGamePlay.instance.randerCamera);
            //gameObject.transform.localScale = new Vector3(1, 1, 1);
            controllerCamera.SetActive(true);
            EnablePlayerOnThirdPerson();
            ActiveCamera = ReferencesForGamePlay.instance.randerCamera.gameObject;
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

        if (ConstantsHolder.xanaConstants.isBuilderScene)
        {
            if (GamificationComponentData.instance.isAvatarChanger ||
                GamificationComponentData.instance.isBlindfoldedFootPrinting)
                BuilderEventManager.StopAvatarChangeComponent?.Invoke(false);
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

    public bool IsGoingDownhill()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, characterController.height / 2f + _rayOffsett))
        {
            CurrentSlope = Vector3.Angle(Vector3.up, hit.normal);
            return CurrentSlope >= 0 && CurrentSlope < YourDownhillThreshold;
        }
        else
        {
            // Character is not on ground, assume not downhill
            return false;
        }
    }


    private void Update()
    {
        if (animator == null)
            return;
        //IsGoingDownhill();
        //if (IsGrounded() && !IsJumping)
        //{
        //    gravityVector.y = gravityValue * Time.deltaTime;
        //    animator.SetBool("IsFalling", false);
        //}
        //else if (!IsGrounded() && characterController.velocity.y < -1)
        //    animator.SetBool("IsFalling", true);


        if (characterController.isGrounded && !IsJumping)
        {
            gravityVector.y = gravityValue * Time.deltaTime;
            animator.SetBool("IsFalling", false);
        }
        else if (!IsGoingDownhill())
            animator.SetBool("IsFalling", true);



        if (m_IsMovementActive)
        {
            if (isFirstPerson && !m_FreeFloatCam)
            {
                if (ActionManager.IsAnimRunning && isJoystickDragging)
                {
                    ActionManager.StopActionAnimation?.Invoke();
                    // EmoteAnimationHandler.Instance.StopAnimation();
                    //  EmoteAnimationHandler.Instance.StopAllCoroutines();
                }
                FirstPersonCameraMove(); // FOR FIRST PERSON MOVEMENT XX
            }
            if (!isFirstPerson && !m_FreeFloatCam)
            {
                if (ActionManager.IsAnimRunning && isJoystickDragging)
                {
                    if (ReferencesForGamePlay.instance.moveWhileDanceCheck == 0)
                    {
                        ActionManager.StopActionAnimation?.Invoke();
                        //  EmoteAnimationHandler.Instance.StopAnimation();
                        //  EmoteAnimationHandler.Instance.StopAllCoroutines();
                    }
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

            //Reset falling state when m_IsMovementActive = false.
            animator.SetBool("IsFalling", false);
        }

        //if (!PlayerSelfieController.Instance.m_IsSelfieFeatureActive)
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
        desiredMoveDirectionFPP = move;
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
                //  UpdateSefieBtn(false);
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
                    //UpdateSefieBtn(!GameplayEntityLoader.animClick);
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
            if (!PlayerSelfieController.Instance.m_IsSelfieFeatureActive)
                PlayerIsIdle?.Invoke();
            //  UpdateSefieBtn(!LoadEmoteAnimations.animClick);

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
        if (ConstantsHolder.xanaConstants.isXanaPartyWorld)
        {
            return;
        }
        m_FreeFloatCam = b;
        StopBuilderComponent();
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
        {
            if (!ConstantsHolder.xanaConstants.isBuilderScene)
                animator.GetComponent<IKMuseum>().m_ConsoleObjOther.SetActive(false);
            else if (!b)
            {
                DisablePlayerOnFPS();
            }
        }
        Debug.Log("FreeFloatCam" + FreeFloatCamCharacterController);
    }

    public void StopBuilderComponent()
    {
        if (ConstantsHolder.xanaConstants.isBuilderScene)
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
            else if (GamificationComponentData.instance.isBlindfoldedFootPrinting)
            {
                if (GamificationComponentData.instance.activeComponent != null)
                    GamificationComponentData.instance.activeComponent.StopBehaviour();
            }
            else if (GamificationComponentData.instance.isAvatarChanger)
                BuilderEventManager.StopAvatarChangeComponent?.Invoke(true);
        }
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

        if (!ReferencesForGamePlay.instance.m_34player)
            return;

        SpecialItemDoubleJump();

        if (!controllerCamera.activeInHierarchy && (horizontal != 0 || vertical != 0))
        {
            controllerCamera.SetActive(true);
            controllerCharacterRenderCamera.SetActive(true);
        }

        _IsGrounded = characterController.isGrounded;

        CalculateMovingPlatformSpeed();
        if (_IsGrounded)
        {
            canDoubleJump = false;
            animator.SetBool("canDoubleJump", canDoubleJump);
        }

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

        //Vector3 desiredMoveDirection = (forward * movementInput.y + right * movementInput.x).normalized;
        desiredMoveDirection = (forward * movementInput.y + right * movementInput.x).normalized;

        //Debug.Log("call hua for===="+ jumpNow + characterController.isGrounded + allowJump + Input.GetKeyDown(KeyCode.Space));
        //Debug.Log("MovmentInput:" + movementInput + "  :DesiredMoveDirection:" + desiredMoveDirection);
        if ((animator.GetCurrentAnimatorStateInfo(0).IsName("NormalStatus") || animator.GetCurrentAnimatorStateInfo(0).IsName("Dwarf Idle") || animator.GetCurrentAnimatorStateInfo(0).IsName("Animation")) && (((Input.GetKeyDown(KeyCode.Space) || IsJumpButtonPress) && characterController.isGrounded && !animator.IsInTransition(0))/* || (characterController.isGrounded && jumpNow && allowJump)*/))
        {
            if (ReferencesForGamePlay.instance.m_34player)
            {
                ReferencesForGamePlay.instance.m_34player.GetComponent<SoundEffects>().PlaySoundEffects(SoundEffects.Sounds.JumpSound);
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
                //checking moving platform
                if (movedPosition.sqrMagnitude != 0 && ConstantsHolder.xanaConstants.isBuilderScene)
                {
                    characterController.Move(movedPosition.normalized * (movedPosition.magnitude / Time.deltaTime) * Time.deltaTime);
                }
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
                // UpdateSefieBtn(false);
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
                    //checking moving platform
                    if (movedPosition.sqrMagnitude != 0 && ConstantsHolder.xanaConstants.isBuilderScene)
                    {
                        characterController.Move(movedPosition.normalized * (movedPosition.magnitude / Time.deltaTime) * Time.deltaTime);
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
                        //checking moving platform
                        if (movedPosition.sqrMagnitude != 0 && ConstantsHolder.xanaConstants.isBuilderScene)
                        {
                            characterController.Move(movedPosition.normalized * (movedPosition.magnitude / Time.deltaTime) * Time.deltaTime);
                        }
                        characterController.Move(desiredMoveDirection * currentSpeed * Time.deltaTime);
                        gravityVector.y += gravityValue * Time.deltaTime;
                        characterController.Move(gravityVector * Time.deltaTime);
                    }
                    else // walk start state
                    {
                        //checking moving platform
                        if (movedPosition.sqrMagnitude != 0 && ConstantsHolder.xanaConstants.isBuilderScene)
                        {
                            characterController.Move(movedPosition.normalized * (movedPosition.magnitude / Time.deltaTime) * Time.deltaTime);
                        }
                        characterController.Move(desiredMoveDirection * currentSpeed * Time.deltaTime);
                        gravityVector.y += gravityValue * Time.deltaTime;
                        characterController.Move(gravityVector * Time.deltaTime);
                    }
                }
            }
        }
        else // Reseating animator to idel when joystick is not moving-----
        {
            if (_IsGrounded) // this check is added because not to enable bottom buttons early while the char is in jump state //
            {
                if (!PlayerSelfieController.Instance.m_IsSelfieFeatureActive)
                    PlayerIsIdle?.Invoke();
                //  UpdateSefieBtn(!LoadEmoteAnimations.animClick);
            }
            //checking moving platform
            if (movedPosition.sqrMagnitude != 0 && ConstantsHolder.xanaConstants.isBuilderScene)
            {
                characterController.Move(movedPosition.normalized * (movedPosition.magnitude / Time.deltaTime) * Time.deltaTime);
            }
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
        //AvatarSpawnerOnDisconnect.Instance.loadassetsstreming(); 
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
        _IsGrounded = characterController.isGrounded;
        if (!_IsGrounded && specialItem && !canDoubleJump)
        {
            canDoubleJump = true;
            animator.SetBool("canDoubleJump", canDoubleJump);
            Invoke(nameof(StopDoubleJump), 0.2f);
            gravityVector.y = JumpVelocity * 2;
        }
        else if (_IsGrounded)
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("JumpMove") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Jump") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Falling") && !animator.GetCurrentAnimatorStateInfo(0).IsName("LandSoft") && !animator.IsInTransition(0))
                IsJumpButtonPress = true;
        }


        if (EmoteAnimationHandler.Instance.animatorremote != null && ReferencesForGamePlay.instance.m_34player.GetComponent<Animator>().GetBool("EtcAnimStart"))    //Added by Ali Hamza
            ReferencesForGamePlay.instance.m_34player.GetComponent<RpcManager>().BackToIdleAnimBeforeJump();

        if (ReferencesForGamePlay.instance.m_34player)
        {
            ReferencesForGamePlay.instance.m_34player.GetComponent<SoundEffects>().PlaySoundEffects(SoundEffects.Sounds.JumpSound);
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
                PlayerCameraController.instance.DisAllowControl();
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
                PlayerCameraController.instance.DisAllowControl();
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

        if (ActionManager.IsAnimRunning)
        {
            ActionManager.StopActionAnimation?.Invoke();

            //  EmoteAnimationHandler.Instance.StopAnimation();
            //  EmoteAnimationHandler.Instance.StopAllCoroutines();
        }

    }

    public void StopAnimationEmote()
    {
        if (ActionManager.IsAnimRunning)
        {
            ActionManager.StopActionAnimation?.Invoke();

            //  EmoteAnimationHandler.Instance.StopAnimation();
            //  EmoteAnimationHandler.Instance.StopAllCoroutines();
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
        PlayerCameraController.instance.AllowControl();
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
                if (PlayerSelfieController.Instance != null && !PlayerSelfieController.Instance.m_IsSelfieFeatureActive)
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

        if (!isNinjaMotion)
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

        if (ActionManager.IsAnimRunning)
        {
            ActionManager.StopActionAnimation?.Invoke();

            //  EmoteAnimationHandler.Instance.StopAnimation();
            // EmoteAnimationHandler.Instance.StopAllCoroutines();
        }
    }

    public void UpdateVelocity()
    {
        gravityVector.y = JumpVelocity;
    }


    //update player jump according to builder setting 
    void PlayerJumpUpdate(float jumpValue, float playerSpeed)
    {
        //sprintSpeed = 5;
        JumpVelocity = GamificationComponentData.instance.MapValue(jumpValue, Constants.minPlayerUIJump, Constants.maxPlayerUIJump, Constants.minPlayerJumpHeight, Constants.maxPlayerJumpHeight);
        sprintSpeed = GamificationComponentData.instance.MapValue(playerSpeed,
                Constants.minPlayerUISpeed, Constants.maxPlayerUISpeed, Constants.minPlayerSprintSpeed, Constants.maxPlayerSprintSpeed);
        speedMultiplier = playerSpeed;
        jumpMultiplier = jumpValue;
        //Store default speed when player update it's speed & jump height
        GamificationComponentData.instance.buildingDetect.DefaultSpeedStore();


    }

    void SpecialItemPlayerPropertiesUpdate(float jumpValue, float playerSpeed)
    {
        JumpVelocity = jumpValue;
        sprintSpeed = playerSpeed;
    }

    /// <SpecialItemDoubleJump>
    [HideInInspector]
    public bool specialItem = false;
    bool canDoubleJump = false;
    void SpecialItemDoubleJump()
    {
        if (!ConstantsHolder.xanaConstants.isBuilderScene)
            return;

        if ((Input.GetKeyDown(KeyCode.Space) || IsJumpButtonPress) && !_IsGrounded && !canDoubleJump && specialItem)
        {
            canDoubleJump = true;
            animator.SetBool("canDoubleJump", canDoubleJump);
            Invoke(nameof(StopDoubleJump), 0.2f);
            //Debug.Log("Double jump testing ");
            gravityVector.y = JumpVelocity * 2;
        }
    }

    void StopDoubleJump()
    {
        animator.SetBool("canDoubleJump", false);
    }
    /// </summary>

    public bool isNinjaMotion = false;
    public bool isMovementAllowed = true;
    public bool isThrow = false;
    ///////////////////////////////////////////
    ///Ninja Move
    //////////////////////////////////////////
    ///
    [SerializeField] private GameObject swordModel;
    [SerializeField] private Transform _ballSpawn;
    [SerializeField] private Transform swordHook, swordhandHook;

    bool isDrawSword = false;
    float timeToWalk = 0;
    void NinjaMove()
    {
        if (isFirstPerson /*|| animator.GetBool("standJump")*/)
            return;
        //animator.SetFloat("Blend", 0.0f);

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
                animator.SetFloat("Blend", 0.5f, 0.25f, Time.deltaTime);
                animator.SetFloat("BlendY", 3f);
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
                    animator.SetFloat("Blend", 0.8f, 0.25f, Time.deltaTime);
                    animator.SetFloat("BlendY", 3f);
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
                            animator.SetFloat("Blend", 0.6f, 0.25f, Time.deltaTime);
                            animator.SetFloat("BlendY", 3f);
                        }
                        if (timeToWalk > 3)
                        {
                            //movementSpeed = finalWalkSpeed + 1;
                            animator.SetFloat("BlendNX", 0.6f, 0.25f, Time.deltaTime); // applying values to animator.
                            animator.SetFloat("BlendNY", 0f, 0.25f, Time.deltaTime); // applying values to animator.
                            animator.SetFloat("Blend", 0.6f, 0.25f, Time.deltaTime);
                            animator.SetFloat("BlendY", 3f);
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
                        animator.SetFloat("Blend", 0.001f, 0.2f, Time.deltaTime);
                        animator.SetFloat("BlendY", 3f);
                        characterController.Move(desiredMoveDirection * currentSpeed * Time.deltaTime);
                        gravityVector.y += gravityValue * Time.deltaTime;
                        characterController.Move(gravityVector * Time.deltaTime);
                    }
                }
            }
        }
        else // Reseating animator to idel when joystick is not moving.
        {
            if (!PlayerSelfieController.Instance.m_IsSelfieFeatureActive)
                PlayerIsIdle?.Invoke();
            AnimationBehaviourNinjaMode();
            characterController.Move(desiredMoveDirection * currentSpeed * Time.deltaTime);
            gravityVector.y += gravityValue * Time.deltaTime;
            characterController.Move(gravityVector * Time.deltaTime);
            timeToWalk = 0;
            animator.SetFloat("animationSpeedMultiplier", 1);
            animator.SetFloat("BlendNX", 0f, 0.3f, Time.deltaTime);
            animator.SetFloat("BlendNY", 0f, 0.3f, Time.deltaTime);
            animator.SetFloat("Blend", 0.0f);
            animator.SetFloat("BlendY", 3f);
        }
        if (animator.GetBool("standJump"))
            animator.SetBool("standJump", false);
    }

    void AnimationBehaviourNinjaMode()
    {
        if (!_IsGrounded || animator.GetBool("IsFalling")) return;

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
        if (GamificationComponentData.instance.withMultiplayer)
            swordModel.GetComponent<NinjaSwordSyncing>().photonView.RPC("NinjaAttackSync", target: RpcTarget.Others, 1);
        animator.CrossFade("NinjaAttack", 0.1f);
        yield return new WaitForSecondsRealtime(0.8f);
        isMovementAllowed = true;
    }
    IEnumerator NinjaAttack2()
    {
        //StopCoroutine(NinjaAttack());
        isMovementAllowed = false;
        if (GamificationComponentData.instance.withMultiplayer)
            swordModel.GetComponent<NinjaSwordSyncing>().photonView.RPC("NinjaAttackSync", target: RpcTarget.Others, 2);
        animator.CrossFade("NinjaAmimationSlash3", 0.1f);
        yield return new WaitForSecondsRealtime(1f);
        isMovementAllowed = true;


    }
    IEnumerator NinjaAttack3()
    {
        //StopCoroutine(NinjaAttack());
        if (GamificationComponentData.instance.withMultiplayer)
            swordModel.GetComponent<NinjaSwordSyncing>().photonView.RPC("NinjaAttackSync", target: RpcTarget.Others, 3);
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
            if (GamificationComponentData.instance.withMultiplayer)
                swordModel.GetComponent<NinjaSwordSyncing>().photonView.RPC("SwordHolding", target: RpcTarget.Others, true);
            animator.CrossFade("SheathingSword", 0.2f);
            yield return new WaitForSecondsRealtime(0.8f);
            swordModel.transform.SetParent(swordhandHook, false);
            yield return new WaitForSecondsRealtime(0.1f);
            //swordModel.transform.localPosition = new Vector3(0.0729999989f, -0.0329999998f, -0.0140000004f);
            //swordModel.transform.localRotation = new Quaternion(0.725517809f, 0.281368196f, -0.0713528395f, 0.623990953f);

            swordModel.transform.localPosition = new Vector3(0.0370000005f, 0.0729999989f, 0.0120000001f);
            Quaternion newRotation = Quaternion.Euler(new Vector3(104.94f, 65.328f, 153.11f));
            swordModel.transform.localRotation = newRotation;

            isMovementAllowed = true;
        }
        if (!isDrawSword)
        {
            isMovementAllowed = false;
            if (GamificationComponentData.instance.withMultiplayer)
                swordModel.GetComponent<NinjaSwordSyncing>().photonView.RPC("SwordHolding", target: RpcTarget.Others, false);
            animator.CrossFade("Withdrawing", 0.2f);
            yield return new WaitForSecondsRealtime(1.3f);
            swordModel.transform.SetParent(swordHook, false);
            swordModel.transform.localPosition = new Vector3(-0.17f, 0.06f, 0.03f);
            swordModel.transform.localRotation = new Quaternion(0.89543f, -0.21528f, 0.28035f, -0.27066f);
            isMovementAllowed = true;
        }
    }
    IEnumerator ThrowFalse()
    {
        isMovementAllowed = false;
        yield return new WaitForSeconds(0.6f);
        GameObject spawned = PhotonNetwork.Instantiate("ShurikenPrefab", _ballSpawn.position, Quaternion.Euler(90, 90, 0));
        spawned.GetComponent<Shuriken>().photonView.RPC("AddForce", target: RpcTarget.All, ((transform.forward * 3000f) * 0.25f));

        animator.SetBool("NinjaThrow", false);
        yield return new WaitForSeconds(0.4f);
        isMovementAllowed = true;

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

        if (swordModel == null && time > 0)
        {
            swordModel = PhotonNetwork.Instantiate("Katana", Vector3.zero, new Quaternion(0, 0, 0, 0));
        }

        BuilderEventManager.OnNinjaMotionComponentCollisionEnter?.Invoke(time);
        NinjaCo = StartCoroutine(NinjaComponentTimer(time));
    }
    public IEnumerator NinjaComponentTimer(float time)
    {
        BuilderEventManager.DisableAnimationsButtons?.Invoke(false);
        isDrawSword = false;
        if (swordModel && time != 0)
        {
            swordModel.transform.SetParent(swordHook, false);
            swordModel.transform.localPosition = new Vector3(-0.17f, 0.06f, 0.03f);
            swordModel.transform.localRotation = new Quaternion(0.89543f, -0.21528f, 0.28035f, -0.27066f);
            //swordModel.SetActive(true);
        }
        yield return new WaitForSeconds(time);
        isNinjaMotion = false;
        StopCoroutine(nameof(DrawNinjaSword));
        if (swordModel)
        {
            //swordModel.SetActive(false);
            PhotonNetwork.Destroy(swordModel.GetComponent<PhotonView>());
        }
        animator.SetBool("NinjaJump", true);
        animator.SetBool("isNinjaMotion", false);
        animator.SetFloat("Blend", 0f, 0.0f, Time.deltaTime); // applying values to animator.
        animator.SetFloat("BlendY", 3f, 0.0f, Time.deltaTime);
        //Ninja_Throw(false);
        isDrawSword = false;
        JumpVelocity = GamificationComponentData.instance.MapValue(jumpMultiplier,
                Constants.minPlayerUIJump, Constants.maxPlayerUIJump, Constants.minPlayerJumpHeight, Constants.maxPlayerJumpHeight);
        sprintSpeed = GamificationComponentData.instance.MapValue(speedMultiplier,
                Constants.minPlayerUISpeed, Constants.maxPlayerUISpeed, Constants.minPlayerSprintSpeed, Constants.maxPlayerSprintSpeed);
        BuilderEventManager.DisableAnimationsButtons?.Invoke(true);
        isMovementAllowed = true;
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
    ThrowThingsTrejectorySyncing trejectoryMultiplayer;

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
        cinemachineFreeLook.m_Orbits[0].m_Radius = 0.55f;

    }
    Vector3 tempRotation, tempPostion;
    public float timeToStartAimLineRenderer, timeToStopAimLineRenderer;
    private Coroutine throwStart, throwEnd, throwAction;
    bool isThrowReady = false, isBallThrow = false;
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

                    trejectoryMultiplayer.photonView.RPC("Init", target: RpcTarget.Others, true, _ballSpawn.position, (ActiveCamera.transform.forward + curveOffset) * _force);
                    BuilderEventManager.DisableAnimationsButtons?.Invoke(false);
                    if (animator.GetBool("standJump"))
                        animator.SetBool("standJump", false);
                }
                else
                {
                    throwLineRenderer.enabled = false;
                    trajectoryController.colliderAim.SetActive(false);
                    handBall.SetActive(false);
                    trejectoryMultiplayer.photonView.RPC("Init", target: RpcTarget.Others, false, _ballSpawn.position, (ActiveCamera.transform.forward + curveOffset) * _force);
                    if (!isBallThrow)
                        BuilderEventManager.DisableAnimationsButtons?.Invoke(true);
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
        isBallThrow = true;
        animator.SetBool("throw", false);
        animator.SetBool("throwing", true);
        GameObject spawned = PhotonNetwork.Instantiate("Ball", handBall.transform.position, handBall.transform.rotation);
        if (GamificationComponentData.instance.withMultiplayer)
            spawned.GetComponent<Ball>().photonView.RPC("ThrowBall", RpcTarget.All, ((ActiveCamera.transform.forward + curveOffset) * _force), false);
        else
            spawned.GetComponent<Ball>().Init((ActiveCamera.transform.forward + curveOffset) * _force, false);
        yield return new WaitForSeconds(1f);
        animator.SetBool("throw", true);
        animator.SetBool("throwing", false);
        yield return new WaitForSeconds(0.7f);
        isThrowReady = true;
        isBallThrow = false;
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
        isBallThrow = false;
        BuilderEventManager.OnThrowThingsComponentDisable?.Invoke();
        cinemachineFreeLook.m_Orbits[0].m_Radius = topRigDefaultRadius;
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
        swordhandHook = GamificationComponentData.instance.ikMuseum.m_SelfieStick.transform.parent;
        swordHook = GamificationComponentData.instance.charcterBodyParts.pelvisBoneNewCharacter.transform;
        _ballSpawn = swordhandHook;

        if (trejectoryMultiplayer == null)
            trejectoryMultiplayer = PhotonNetwork.Instantiate("ThowTrejectory", Vector3.zero, Quaternion.identity).GetComponent<ThrowThingsTrejectorySyncing>();

        //Throw Things component
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
            handBall.transform.localPosition = new Vector3(0, 0, 0.044f);
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


    private Transform movingPlatform;
    private Vector3 rayOffset = new Vector3(0f, .02f, 0f);
    private float rayDistance = .3f;
    private Vector3 lastMovePlatformPosition;
    private Vector3 movedPosition;
    internal bool isOnMovingPlatform;
    private void CalculateMovingPlatformSpeed()
    {
        if (!ConstantsHolder.xanaConstants.isBuilderScene || GamificationComponentData.instance == null)
            return;

        if (!characterController.isGrounded)
        {
            if (movingPlatform != null)
            {
                movingPlatform = null;
                movedPosition = lastMovePlatformPosition = Vector3.zero;
            }
            return;
        }

        RaycastHit hitData;
        isOnMovingPlatform = Physics.Raycast(transform.position + rayOffset, -transform.up, out hitData, rayDistance, GamificationComponentData.instance.platformLayers);

        //Debug.DrawRay(transform.position + rayOffset, -transform.up * rayDistance, (hitData.collider != null) ? Color.red : Color.green);

        if (!isOnMovingPlatform)
        {
            movingPlatform = null;
            movedPosition = lastMovePlatformPosition = Vector3.zero;
            return;
        }

        if (movingPlatform == null || hitData.transform.GetInstanceID() != movingPlatform.GetInstanceID())
        {
            //  Debug.LogFormat("Moving TransformComponent: {0}, {1}", translateVar, translateVar.gameObject.name);
            if (hitData.transform.GetComponentInParent<TranslateComponent>() == null && hitData.transform.GetComponentInParent<TransformComponent>() == null) return;
            movingPlatform = hitData.transform;
            lastMovePlatformPosition = movingPlatform.position;
        }

        movedPosition = movingPlatform.position - lastMovePlatformPosition;
        lastMovePlatformPosition = movingPlatform.position;
    }
}