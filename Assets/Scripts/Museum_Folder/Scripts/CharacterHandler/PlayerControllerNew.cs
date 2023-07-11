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
    public bool allowTeleport=true;
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


    void Start()
    {

      

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


        //Update jump height according to builder
        BuilderEventManager.ApplyPlayerProperties += PlayerJumpUpdate;


       

    }


    public void OnDestroy()
    {
        if (GamePlayButtonEvents.inst != null)
        {
            GamePlayButtonEvents.inst.OnSwitchCamera -= SwitchCameraButton;
            GamePlayButtonEvents.inst.OnJumpBtnDownEvnt -= JumpAllowed;
            GamePlayButtonEvents.inst.OnJumpBtnUpEvnt -= JumpNotAllowed;
        }

        //Update jump height according to builder
        BuilderEventManager.ApplyPlayerProperties -= PlayerJumpUpdate;
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
        //Debug.LogError("0");
        isFirstPerson = !isFirstPerson;
        gravityVector.y = 0;

        CanvasButtonsHandler.inst.OnChangehighlightedFPSbutton(isFirstPerson);
        if (isFirstPerson)
        {

            //Debug.LogError("1");
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
            //Debug.LogError("2");
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
            if  (isFirstPerson && !m_FreeFloatCam)
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
        //Debug.LogError("MovmentInput:" + movementInput + "  :Move:" + move);
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
                //Debug.LogError("Move Sprint:" + firstPersonSprintSpeed + "    :Move:" + move);
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
                //Debug.LogError("Move current:" + firstPersonCurrentSpeed + "    :Move:" + move);
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
            //Debug.LogError("FirstPersonCameraMove jump");
            StartCoroutine(Jump(diff));
        }
        /*else if (characterController.isGrounded && velocity.y < 0 && !IsJumping)
        {
            animator.SetBool("IsJumping", false);
            animator.SetBool("standJump", false);
            jumpNow = false;
        }*/
    }

    /// <summary>
    /// FOR DUMMY JUMP BECAUSE OF PLAYER HEAD MOVE CONSTANTLY
    /// </summary>
    /// <param name="diff"> DIFF IS THE CAMERA START POSTION TO PALYER HEAD POSTION DIFFRENCE FOR JUMP END POINT </param>
    /// <returns></returns>
    IEnumerator Jump(Vector3 diff)
    {
        //Debug.LogError("Jump");
        float progress = 0.0f;
        float d = camStartPosition.y + diff.y;

        float tempWait = 0.75f;
        if (horizontal != 0.0f || vertical != 0.0f) // is runing 
        {
            tempWait /= 3f;
        }
        //Debug.LogError("jump TempWait:" + tempWait + "    :Horizontal:" + horizontal + "    :Vertical:" + vertical);

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
        //Debug.LogError("JumpEnd TempWait:" + tempWait + "   :TempWait2:" + tempWait2 + "    :Horizontal:"+horizontal + "    :Vertical:" + vertical);

        while (progress < tempWait)//0.35f
        {
            progress += Time.deltaTime;
            firstPersonCameraObj.transform.localPosition = Vector3.Lerp(firstPersonCameraObj.transform.localPosition, new Vector3(0f, 1.1f, 0.1f), progress);
            yield return null;
        }
        firstPersonCameraObj.transform.localPosition = new Vector3(0f, 1.1f, 0.1f);
        //Debug.LogError("Jump end");
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
        Vector3 newPos = new Vector3(transform.position.x, transform.position.y+2f, transform.position.z); 

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
        FreeFloatCamCharacterController.gameObject.SetActive(b);
        animator.SetBool("freecam", b);
        animator.GetComponent<IKMuseum>().ConsoleObj.SetActive(b);

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
       

        Debug.Log("FreeFloatCam" + FreeFloatCamCharacterController);
    }


    void Move()
    {
        if (isFirstPerson /*|| animator.GetBool("standJump")*/)
            return;

        if (!controllerCamera.activeInHierarchy && (horizontal != 0 || vertical != 0))
        {
            controllerCamera.SetActive(true);
            controllerCharacterRenderCamera.SetActive(true);
        }

        _IsGrounded = characterController.isGrounded;
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
        //Debug.LogError("call hua for===="+ jumpNow + characterController.isGrounded + allowJump + Input.GetKeyDown(KeyCode.Space));
        //Debug.LogError("MovmentInput:" + movementInput + "  :DesiredMoveDirection:" + desiredMoveDirection);
        if ((animator.GetCurrentAnimatorStateInfo(0).IsName("NormalStatus") || animator.GetCurrentAnimatorStateInfo(0).IsName("Dwarf Idle") || animator.GetCurrentAnimatorStateInfo(0).IsName("Animation")) && (((Input.GetKeyDown(KeyCode.Space) || IsJumpButtonPress) && characterController.isGrounded && !animator.IsInTransition(0))/* || (characterController.isGrounded && jumpNow && allowJump)*/))
        {
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
                Debug.LogError("FP Normal Movement.......");
                animator.SetFloat("Blend", horizontal, speedSmoothTime, Time.deltaTime);
                animator.SetFloat("BlendY", vertical, speedSmoothTime, Time.deltaTime);
            }*/

            if (movementInput.sqrMagnitude >= sprintThresold)
            {
                //Debug.LogError("Move Sprint:" + sprtintSpeed + "    :DesiredMoveDirection:" + desiredMoveDirection);
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
                //Debug.LogError("Move Current:" + currentSpeed + "    :DesiredMoveDirection:" + desiredMoveDirection);
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
        if (_IsGrounded)
        {
            IsJumpButtonPress = true;
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
                //Debug.LogError("JumpAllowed");
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
                    //Debug.LogError("JumpAllowed1111111");
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
        //Debug.LogError("JumpNotAllowed");
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
        JumpVelocity += (jumpValue - 1);
        sprintSpeed += (playerSpeed - 1);
    }
}