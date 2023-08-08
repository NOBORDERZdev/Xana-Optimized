using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerController : MonoBehaviourPun, IPunObservable
{
    #region Variables
    [SerializeField] GameObject[] comboCollider;
    public float playerSpeed, grabDamage, range, specialBarFill;
    public bool isBlock, isSpecialAttack;
    [SerializeField] private float jumpHeight = 1.0f;
    public Transform hipTransform;
    [SerializeField] Animator anim;
    public Animator otherAnim;
    public PlayerController otherPlayerController;
    [SerializeField] RuntimeAnimatorController comboAction_1, comboAction_2, comboReaction_1, comboReaction_2, specialGrabThrow;
    public Image powerBar;

    public bool canJump = true;
    public bool layDown = false;

    Movement _playerMovement;
    public CharacterController controller;
    Vector3 playerVelocity;
    bool groundedPlayer;
    float gravityValue = -9.81f;
    int _basicCombo1Val = 0, _basicCombo2Val = 0, _specialComboVal = 0, _throwVal = 0, _grabVal = 0;
    public bool canAction = true;
    float attackRange = 0.8f;
    float comboHitTime = 0f;
    #endregion

    public Transform[] _playerGrabPosition;
    public Transform[] _playerSpecialGrabPosition;
    Transform _bringGrabHere;
    Transform _bringSpecialGrabHere;
    Transform _cameraMain;
    private Vector3 syncPosition;
    private Quaternion syncRotation;
    float _camShakeWaitTime = 0;
    #region Animator Hashes Vars
    private int basicCombo1Hash;
    private int basicCombo2Hash;
    private int hit1Hash;
    private int hit2Hash;
    private int hit3Hash;
    private int reactionHit_1Hash;
    private int reactionHit_2Hash;
    private int reactionHit_3Hash;
    private int blendHash;
    private int blendYHash;
    private int isGroundedHash;
    private int isJumpingHash;
    private int standJumpHash;
    private int bc1reactionTriggerHash;
    private int bc2reactionTriggerHash;
    private int throwHash;
    private int rollHash;
    private int specialComboHash;

    const int GRAB_CONST = 1;
    const int SPECIALCOMBO_CONST = 2;
    const int REACTION_1_CONST = 1;
    const int REACTION_2_CONST = 2;
    const int REACTION_3_CONST = 3;
    const int BC1_REACTION_TRIGGER_CONST = 1;
    const int BC2_REACTION_TRIGGER_CONST = 2;
    #endregion
    bool isAndroid = false;
    #region Unity Funcs
    private void Awake()
    {
        _playerMovement = new Movement();
        if (Application.platform == RuntimePlatform.Android) {
            isAndroid = true;
        }
    }

    private void OnEnable()
    {
        _playerMovement.Enable();
    }

    private void OnDisable()
    {
        _playerMovement.Disable();
    }

    private void Start()
    {
        if (photonView.IsMine)
        {
            powerBar = FightingModule.GameManager.instance.firstPowerBar;
        }
        else
        {
            powerBar = FightingModule.GameManager.instance.secondPowerBar;
        }
        powerBar.fillAmount = 0;
        if (photonView.IsMine)
        {
            _basicCombo1Val = ShotsSelectionData._instance._basicCombo1Val;
            _basicCombo2Val = ShotsSelectionData._instance._basicCombo2Val;
            _specialComboVal = ShotsSelectionData._instance._specialComboVal;
            _throwVal = ShotsSelectionData._instance._throwVal;
            //_grabVal = ShotsSelectionData._instance._grabVal;

            photonView.RPC("SetGrabPos", RpcTarget.AllBuffered);
            comboAction_1 = ShotsSelectionData._instance.combo1ActionAnims;
            comboAction_2 = ShotsSelectionData._instance.combo2ActionAnims;

            comboReaction_1 = ShotsSelectionData._instance.combo1ReactionAnims;
            comboReaction_2 = ShotsSelectionData._instance.combo2ReactionAnims;
        }
        _cameraMain = FightingModule.GameManager.instance.mainCamera.transform;
        basicCombo1Hash = Animator.StringToHash("BasicCombo_1");
        basicCombo2Hash = Animator.StringToHash("BasicCombo_2");

        hit1Hash = Animator.StringToHash("Hit_1");
        hit2Hash = Animator.StringToHash("Hit_2");
        hit3Hash = Animator.StringToHash("Hit_3");
        reactionHit_1Hash = Animator.StringToHash("reactionHit_1");
        reactionHit_2Hash = Animator.StringToHash("reactionHit_2");
        reactionHit_3Hash = Animator.StringToHash("reactionHit_3");
        blendHash = Animator.StringToHash("Blend");
        blendYHash = Animator.StringToHash("BlendY");
        isGroundedHash = Animator.StringToHash("IsGrounded");
        isJumpingHash = Animator.StringToHash("IsJumping");
        standJumpHash = Animator.StringToHash("standJump");
        bc1reactionTriggerHash = Animator.StringToHash("basicCombo1Reaction");
        bc2reactionTriggerHash = Animator.StringToHash("basicCombo2Reaction");
        specialComboHash = Animator.StringToHash("specialCombo");
        throwHash = Animator.StringToHash("throw");
        rollHash = Animator.StringToHash("Roll");

        switch (_throwVal)
        {
            case 0:
                _camShakeWaitTime = 1.30f;
                break;
            case 1:
                _camShakeWaitTime = 3.20f;
                break;
            case 2:
                _camShakeWaitTime = 1.25f;
                break;
            case 3:
                _camShakeWaitTime = 2.2f;
                break;
            case 4:
                _camShakeWaitTime = 2.7f;
                break;
            case 5:
                _camShakeWaitTime = 2.45f;
                break;
            case 6:
                _camShakeWaitTime = 1.1f;
                break;
            case 7:
                _camShakeWaitTime = 4.25f;
                break;
            case 8:
                _camShakeWaitTime = 4.15f;
                break;
            case 9:
                _camShakeWaitTime = 4.20f;
                break;
            case 10:
                _camShakeWaitTime = 2.30f;
                break;
            case 11:
                _camShakeWaitTime = 3.8f;
                break;
        }
    }

    [PunRPC]
    public void SetGrabPos()
    {
        _bringGrabHere = _playerGrabPosition[ShotsSelectionData._instance._throwVal];
        //  _bringSpecialGrabHere = _playerSpecialGrabPosition[ShotsSelectionData._instance._grabVal];
    }

    void Update()
    {
        if (canAction == true)
        {
            PlayerMove();
        }
        if (otherAnim != null && !otherPlayerController.layDown && !layDown) {
            transform.LookAt(otherAnim.transform);
        }
        else
        {
            anim.SetFloat(blendHash, 0);
            anim.SetFloat(blendYHash, 0);
        }
        if (layDown)
        {
            controller.center = new Vector3(0f, 0.15f, 0f);
            controller.radius = 0.15f;
            controller.height = 0f;
        }
        else
        {
            controller.center = new Vector3(0f, 0.76f, 0f);
            controller.radius = 0.29f;
            controller.height = 1.57f;
        }
    }
#endregion

    #region Combos ETC
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Sending data to other clients
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            // Receiving data from the owner client
            syncPosition = (Vector3)stream.ReceiveNext();
            syncRotation = (Quaternion)stream.ReceiveNext();
        }
    }

    public void PlayerMove()
    {
        if (photonView.IsMine)
        {
            groundedPlayer = controller.isGrounded;
            if (groundedPlayer && playerVelocity.y < 0)
            {
                canJump = true;
                anim.SetBool(isGroundedHash, true);
                anim.SetBool(isJumpingHash, false);
                anim.SetBool(standJumpHash, false);
                playerVelocity.y = -0.45f;
            }
            Vector2 movementInput = _playerMovement.PlayerMain.Move.ReadValue<Vector2>();
            //Vector2 movementInput = new Vector2(horizontal, vertical);
            //  Vector3 move = new Vector3(movementInput.x, 0, movementInput.y);
            Vector3 move = (_cameraMain.forward * movementInput.y + _cameraMain.right * movementInput.x);
            move.y = 0;
            controller.Move(move * Time.deltaTime * playerSpeed);

            if (move != Vector3.zero)
            {
                if (layDown)
                {
                    photonView.RPC("ResetLayDown", RpcTarget.All);
                }
                gameObject.transform.forward = move;

                //moveSpeed += Time.deltaTime;
                //moveSpeed = Mathf.Clamp(moveSpeed,0,3f);

                //anim.SetFloat("Blend", 3f);
                //anim.SetFloat("BlendY", 3f);
                anim.SetFloat(blendHash, movementInput.magnitude + 0.5f);
                anim.SetFloat(blendYHash, movementInput.magnitude + 0.5f);

            }
            else
            {
                anim.SetFloat(blendHash, 0);
                anim.SetFloat(blendYHash, 0);
            }
            playerVelocity.y += gravityValue * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime * 0.5f);
        }
        else
        {
            // Update position and rotation for remote players
            transform.position = Vector3.Lerp(transform.position, syncPosition, Time.deltaTime * 10);
            transform.rotation = Quaternion.Lerp(transform.rotation, syncRotation, Time.deltaTime * 10);
        }
    }

    //public void Jump()
    //{
    //    // Changes the height position of the player..
    //    //   if (_playerMovement.PlayerMain.Jump.triggered && groundedPlayer)
    //    if (photonView.IsMine)
    //    {
    //        if (canJump && canAction)
    //        {
    //            canJump = false;
    //            anim.SetTrigger("Jump");
    //            anim.SetBool(standJumpHash, true);
    //            anim.SetBool(isJumpingHash, true);
    //            anim.SetBool(isGroundedHash, false);
    //            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
    //        }
    //    }
    //}

    //public void GrabThrow()//Mubashir
    //{
    //    //  if (_playerMovement.PlayerMain.GrabThrow.triggered)
    //    if (photonView.IsMine)
    //    {
    //        if (canAction && canJump && !otherPlayerController.layDown && !layDown)
    //        {
    //            if (otherAnim != null && Vector3.Distance(this.transform.position, otherAnim.transform.position) < attackRange)
    //            {
    //                if (otherPlayerController.isBlock)
    //                {
    //                    otherPlayerController.photonView.RPC("ReleaseGrabBlock_RPC", RpcTarget.All, false);
    //                }
    //                if (anim.runtimeAnimatorController != specialGrabThrow)
    //                {
    //                    photonView.RPC("specialGrabThrowAnimator_RPC", RpcTarget.All);
    //                }
    //                photonView.RPC("PowerBar_RPC", RpcTarget.All);
    //                canAction = false;
    //                this.transform.LookAt(otherAnim.transform);
    //                // set the x-axis rotation of the object to zero
    //                Vector3 rotation = transform.rotation.eulerAngles;
    //                rotation.x = 0f;
    //                transform.rotation = Quaternion.Euler(rotation);
    //                int randVal = _grabVal;
    //                // string comboN = "specialGrab";
    //                int specialGrabHash = Animator.StringToHash("specialGrab");
    //                anim.SetFloat(specialGrabHash, randVal);
    //                otherPlayerController.photonView.RPC("Combos_RPC", RpcTarget.All, specialGrabHash, randVal, true);
    //                otherPlayerController.photonView.RPC("SetDamage", RpcTarget.All, grabDamage);
    //                StartCoroutine(StopAnim(specialGrabHash, -1, anim, false));
    //            }
    //        }
    //    }
    //}

    public void Throw()//Mubashir
    {
        print("THORRWWW");
        //  if (_playerMovement.PlayerMain.GrabThrow.triggered)
        if (photonView.IsMine)
        {
            if (canAction && canJump && !otherPlayerController.layDown && !layDown)
            {
                if (otherAnim != null && Vector3.Distance(this.transform.position, otherAnim.transform.position) < attackRange)
                {
                    if (!otherPlayerController.isBlock)
                    {
                        if (anim.runtimeAnimatorController != specialGrabThrow)
                        {
                            photonView.RPC("specialGrabThrowAnimator_RPC", RpcTarget.All);
                        }
                        photonView.RPC("PowerBar_RPC", RpcTarget.All);
                        canAction = false;
                        this.transform.LookAt(otherAnim.transform);
                        // set the x-axis rotation of the object to zero
                        Vector3 rotation = transform.rotation.eulerAngles;
                        rotation.x = 0f;
                        transform.rotation = Quaternion.Euler(rotation);
                        int randVal = _throwVal;
                        // string comboN = "throw";

                        anim.SetFloat(throwHash, randVal);
                        otherPlayerController.photonView.RPC("Combos_RPC", RpcTarget.All, GRAB_CONST, randVal, true);
                        otherPlayerController.photonView.RPC("SetDamage", RpcTarget.All, grabDamage, 1, _camShakeWaitTime);
                        StartCoroutine(StopAnim(throwHash, -1, anim, false));
                    }
                }
            }
        }
    }

    public void SpecialCombo()
    {
        //  if (_playerMovement.PlayerMain.GrabThrow.triggered)
        if (photonView.IsMine)
        {
            if (canAction && canJump && !otherPlayerController.layDown && !layDown)
            {
                if (powerBar.fillAmount == 1 && otherAnim != null && Vector3.Distance(this.transform.position, otherAnim.transform.position) < attackRange)
                {
                    if (anim.runtimeAnimatorController != specialGrabThrow)
                    {
                        photonView.RPC("specialGrabThrowAnimator_RPC", RpcTarget.All);
                    }
                    if (layDown)
                    {
                        photonView.RPC("ResetLayDown", RpcTarget.All);
                    }
                    canAction = false;
                    photonView.RPC("SpecialAttack_RPC", RpcTarget.All, true);
                    //string comboN = "specialCombo";
                    int randVal = _specialComboVal;// = Random.Range(0, 10);
                    foreach (GameObject obj in comboCollider)
                    {
                        obj.SetActive(true);
                    }
                    this.transform.LookAt(otherAnim.transform);
                    // set the x-axis rotation of the object to zero
                    Vector3 rotation = transform.rotation.eulerAngles;
                    rotation.x = 0f;
                    transform.rotation = Quaternion.Euler(rotation);
                    anim.SetFloat(specialComboHash, randVal);
                    otherPlayerController.photonView.RPC("Combos_RPC", RpcTarget.All, SPECIALCOMBO_CONST, randVal, false);
                    StartCoroutine(StopAnim(specialComboHash, -1, anim, false));
                    photonView.RPC("ResetPowerBar", RpcTarget.All);
                    if (isAndroid) {
                        VibrationManager.Vibrate();
                    }
                }
            }
        }
    }

    public void BasicCombo() //PUNCH
    {
        if (!photonView.IsMine)
            return;


        anim.SetFloat(blendHash, -1);
        anim.SetFloat(blendYHash, -1);

        if (isAndroid)
        {
            VibrationManager.Vibrate();
        }
        if (!canAction || !canJump)
        {
            if (anim.GetBool(hit2Hash) == false && (Time.time - comboHitTime) < 0.7f)
            {
                comboHitTime = Time.time;
                anim.SetBool(hit2Hash, true);
                if (otherAnim != null && Vector3.Distance(this.transform.position, otherAnim.transform.position) < attackRange)
                    otherPlayerController.photonView.RPC("HitCombos_RPC", RpcTarget.All, 0, REACTION_2_CONST);

            }
            else if (anim.GetBool(hit3Hash) == false && (Time.time - comboHitTime) < 0.7f)
            {
                comboHitTime = Time.time;
                anim.SetBool(hit3Hash, true);
                if (otherAnim != null && Vector3.Distance(this.transform.position, otherAnim.transform.position) < attackRange)
                    otherPlayerController.photonView.RPC("HitCombos_RPC", RpcTarget.All, 0, REACTION_3_CONST);
            }
            return;
        }

        if (anim.runtimeAnimatorController != comboAction_1)
        {
            photonView.RPC("Combo1Animator_RPC", RpcTarget.All);
        }
        if (otherAnim.runtimeAnimatorController != comboReaction_1)
        {
            if (!otherPlayerController.layDown)
                photonView.RPC("Combo1ReactionAnimator_RPC", RpcTarget.All);
        }
        if (layDown)
        {
            photonView.RPC("ResetLayDown", RpcTarget.All);
        }
        canAction = false;
        string comboN = "basicCombo";
        int randVal = _basicCombo1Val;
        comboHitTime = Time.time;
        anim.SetBool(hit1Hash, true);
        anim.SetTrigger(basicCombo1Hash);
        if (otherAnim != null && Vector3.Distance(this.transform.position, otherAnim.transform.position) < attackRange)
        {
            foreach (GameObject obj in comboCollider)
            {
                obj.SetActive(true);
            }
            this.transform.LookAt(otherAnim.transform);
            // set the x-axis rotation of the object to zero
            Vector3 rotation = transform.rotation.eulerAngles;
            rotation.x = 0f;
            transform.rotation = Quaternion.Euler(rotation);
            bool layDownBool = otherPlayerController.layDown;
            bool blockedBool = otherPlayerController.isBlock;
            if (!layDownBool && !blockedBool)
            {
                otherPlayerController.photonView.RPC("HitCombos_RPC", RpcTarget.All, BC1_REACTION_TRIGGER_CONST, REACTION_1_CONST);
            }
        }
        else
        {
            if (range > 0)
            {
                StartCoroutine(SetComboMove());
            }
        }
    }

    public void BasicCombo2() //KICK
    {
        if (!photonView.IsMine)
            return;

        anim.SetFloat(blendHash, -1);
        anim.SetFloat(blendYHash, -1);

        if (isAndroid)
        {
            VibrationManager.Vibrate();
        }
        if (!canAction || !canJump)
        {
            if (anim.GetBool(hit2Hash) == false && (Time.time - comboHitTime) < 0.7f)
            {
                comboHitTime = Time.time;
                anim.SetBool(hit2Hash, true);
                if (otherAnim != null && Vector3.Distance(this.transform.position, otherAnim.transform.position) < attackRange)
                    otherPlayerController.photonView.RPC("HitCombos_RPC", RpcTarget.All, 0, REACTION_2_CONST);
            }
            else if (anim.GetBool(hit3Hash) == false && (Time.time - comboHitTime) < 0.7f)
            {
                comboHitTime = Time.time;
                anim.SetBool(hit3Hash, true);
                if (otherAnim != null && Vector3.Distance(this.transform.position, otherAnim.transform.position) < attackRange)
                    otherPlayerController.photonView.RPC("HitCombos_RPC", RpcTarget.All, 0, REACTION_3_CONST);
            }
            return;
        }

        if (anim.runtimeAnimatorController != comboAction_2)
        {
            photonView.RPC("Combo2Animator_RPC", RpcTarget.All);
        }

        if (otherAnim.runtimeAnimatorController != comboReaction_2)
        {
            if (!otherPlayerController.layDown)
                photonView.RPC("Combo2ReactionAnimator_RPC", RpcTarget.All);
        }

        if (layDown)
        {
            photonView.RPC("ResetLayDown", RpcTarget.All);
        }

        canAction = false;
        string comboN = "basicCombo2";
        int randVal = _basicCombo2Val;
        comboHitTime = Time.time;
        anim.SetBool(hit1Hash, true);
        anim.SetTrigger(basicCombo2Hash);
        if (otherAnim != null && Vector3.Distance(this.transform.position, otherAnim.transform.position) < attackRange)
        {
            foreach (GameObject obj in comboCollider)
            {
                obj.SetActive(true);
            }
            this.transform.LookAt(otherAnim.transform);
            // set the x-axis rotation of the object to zero
            Vector3 rotation = transform.rotation.eulerAngles;
            rotation.x = 0f;
            transform.rotation = Quaternion.Euler(rotation);

            bool layDownBool = otherPlayerController.layDown;
            bool blockedBool = otherPlayerController.isBlock;
            if (!layDownBool && !blockedBool)
            {
                otherPlayerController.photonView.RPC("HitCombos_RPC", RpcTarget.All, BC2_REACTION_TRIGGER_CONST, REACTION_1_CONST);
            }
        }
        else
        {
            if (range > 0)
            {
                StartCoroutine(SetComboMove());
            }
        }
    }

    [PunRPC]
    public void Combo1Animator_RPC()
    {
        anim.runtimeAnimatorController = comboAction_1;
    }

    [PunRPC]
    public void Combo1ReactionAnimator_RPC()
    {
        otherAnim.runtimeAnimatorController = comboReaction_1;
    }

    [PunRPC]
    public void Combo2Animator_RPC()
    {
        anim.runtimeAnimatorController = comboAction_2;
    }

    [PunRPC]
    public void Combo2ReactionAnimator_RPC()
    {
        otherAnim.runtimeAnimatorController = comboReaction_2;
    }

    [PunRPC]
    public void specialGrabThrowAnimator_RPC()
    {
        anim.runtimeAnimatorController = specialGrabThrow;
        otherAnim.runtimeAnimatorController = specialGrabThrow;
    }

    [PunRPC]
    public void HitCombos_RPC(int comboTriggerHash = 0, int hitHash = 0)
    {
        if (!layDown)
            canAction = false;
        transform.LookAt(otherAnim.transform);
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
        //OlD Way
        //if (!string.IsNullOrEmpty(hitName))
        //    anim.SetBool(hitName, true);
        // if (!string.IsNullOrEmpty(hitHash))

        if (hitHash == 1)
        {
            hitHash = reactionHit_1Hash;
        }
        else if (hitHash == 2)
        {
            hitHash = reactionHit_2Hash;
        }
        else if (hitHash == 3)
        {
            hitHash = reactionHit_3Hash;
        }

        if (hitHash != 0)
        {
            anim.SetBool(hitHash, true);
        }
        //OLD WAy
        //if (!string.IsNullOrEmpty(comboTrigger))
        //    anim.SetTrigger(comboTrigger);

        // if (!string.IsNullOrEmpty(comboTrigger))
        if (comboTriggerHash == 1)
        {
            comboTriggerHash = bc1reactionTriggerHash;
        }
        else if (comboTriggerHash == 2)
        {
            comboTriggerHash = bc2reactionTriggerHash;
        }

        if (comboTriggerHash != 0)
        {
            anim.SetTrigger(comboTriggerHash);
        }
    }

    public void BasicComboEnded()
    {
        anim.SetBool(isGroundedHash, true);
        if (isSpecialAttack)
            photonView.RPC("SpecialAttack_RPC", RpcTarget.All, false);
        canAction = true;
        foreach (GameObject obj in comboCollider)
        {
            obj.SetActive(false);
        }
    }

    IEnumerator SetComboMove()
    {
        if (otherAnim != null)
        {
            transform.LookAt(otherAnim.transform);
            // set the x-axis rotation of the object to zero
            Vector3 rotation = transform.rotation.eulerAngles;
            rotation.x = 0f;
            transform.rotation = Quaternion.Euler(rotation);
            Vector3 direction = Vector3.Normalize(otherAnim.transform.position - transform.position);
            Vector3 newPosition = transform.position + (direction * range);
            transform.position = newPosition;
        }
        yield return null;
    }

    bool blockClicked = false;
    public void ThrowEsacpe()
    {
        if (photonView.IsMine)
        {
            if (canAction && canJump)
            {
                blockClicked = true;
                canAction = false;
                anim.SetBool("isBlocking", true);
                photonView.RPC("Block_RPC", RpcTarget.All, true);
                if (otherAnim != null)
                {
                    transform.LookAt(otherAnim.transform);
                    // set the x-axis rotation of the object to zero
                    Vector3 rotation = transform.rotation.eulerAngles;
                    rotation.x = 0f;
                    transform.rotation = Quaternion.Euler(rotation);
                }
            }
        }
    }

    public void BlockStop()
    {
        if (photonView.IsMine)
        {
            if (blockClicked)
            {
                anim.SetBool("isBlocking", false);
                photonView.RPC("Block_RPC", RpcTarget.All, false);
                canAction = true;
                blockClicked = false;
            }
        }
    }

    [PunRPC]
    public void ReleaseGrabBlock_RPC(bool b)
    {
        anim.SetBool("isBlocking", b);
        isBlock = b;
    }

    IEnumerator StopAnim(int currentParam, float _normal, Animator anim, bool layDownBool)
    {
        yield return new WaitForSeconds(0.25f);
        AnimatorControllerParameter[] parameters = anim.parameters;
        AnimationClip currentClip = null;
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        int currentAnimationHash = stateInfo.fullPathHash;
        float blendParameter = anim.GetFloat(currentParam);

        foreach (AnimatorControllerParameter parameter in parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Float)
            {
                if (anim.GetFloat(parameter.name) == blendParameter)
                {
                    currentClip = anim.GetCurrentAnimatorClipInfo(0)[0].clip;
                    break;
                }
            }
        }
        float animationLength = 0;
        if (currentClip != null)
        {
            animationLength = currentClip.length;
        }
        yield return new WaitForSeconds(animationLength + 0.25f);
        anim.SetBool(isGroundedHash, true);
        foreach (GameObject obj in comboCollider)
        {
            obj.SetActive(false);
        }
        canAction = true;
        if (isSpecialAttack)
            photonView.RPC("SpecialAttack_RPC", RpcTarget.All, false);
        layDown = layDownBool;
        bool isRoll = layDown;
        while (layDown == true)
        {
            yield return new WaitForSeconds(0.1f);
        }
        anim.SetBool(rollHash, isRoll);
        anim.SetFloat(currentParam, _normal);
    }

    [PunRPC]
    public void Block_RPC(bool b)
    {
        isBlock = b;
    }

    [PunRPC]
    public void SpecialAttack_RPC(bool b)
    {
        isSpecialAttack = b;
    }

    [PunRPC]
    public void Combos_RPC(int comboN, int randVal, bool layDownBool)
    {
        canAction = false;
        transform.LookAt(otherAnim.transform);
        // set the x-axis rotation of the object to zero
        Vector3 rotation = transform.rotation.eulerAngles;
        rotation.x = 0f;
        transform.rotation = Quaternion.Euler(rotation);
        if (comboN == 1)
        {
            comboN = throwHash;
        }
        else if (comboN == 2)
        {
            comboN = specialComboHash;
        }
        anim.SetFloat(comboN, randVal + 12);
        StartCoroutine(StopAnim(comboN, -1, anim, layDownBool));
        //if (comboN == "throw")
        //{
        if (comboN == throwHash)
        {
            otherAnim.gameObject.transform.position = _bringGrabHere.position;//First Test for Grabs
        }
        // }
        //else if (comboN == "specialGrab")
        //{
        //    otherAnim.gameObject.transform.position = _bringSpecialGrabHere.position;//First Test for Grabs
        //}
    }

    [PunRPC]
    public void PowerBar_RPC()
    {
        anim.SetBool(rollHash, false);
        if (powerBar.fillAmount < 1 && otherPlayerController.isBlock == false)
            powerBar.DOFillAmount(powerBar.fillAmount + specialBarFill, 0.5f);
        //powerBar.fillAmount += specialBarFill;
    }

    [PunRPC]
    public void ResetPowerBar()
    {
        powerBar.DOFillAmount(0, 0.5f);
        // powerBar.fillAmount = 0;
    }

    [PunRPC]
    public void ResetLayDown()
    {
        transform.position = new Vector3(hipTransform.position.x, transform.position.y, hipTransform.position.z);
        layDown = false;
    }

    public void ResetPlayer()
    {
        anim.runtimeAnimatorController = comboAction_1;
        layDown = false;
        canAction = true;
    }
    #endregion
}
