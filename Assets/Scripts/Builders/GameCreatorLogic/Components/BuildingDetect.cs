using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;

public class BuildingDetect : MonoBehaviour
{
    [Header("Avatar Change")]
    public GameObject curModel;
    public AvatarController avatarController;
    public GameObject[] tmpModel;

    public PlayerControllerNew PC;
    public float defaultJumpHeight, defaultSprintSpeed;
    public float powerProviderHeight, powerProviderSpeed, ninjaPlayerSpeed;
    public float powerUpTime, avatarChangeTime, ninjaUpTime;
    IEnumerator powerUpCoroutine, avatarChangeCoroutine, ninjaModeCoroutine;

    public AnimatorOverrideController ninjaOverrideAnimator;
    public RuntimeAnimatorController defaultAnimator;
    private Animator characterAnimator;
    private void Awake()
    {
        //PC = this.gameObject.GetComponent<PlayerControllerNew>();
        //characterAnimator = curModel.GetComponent<Animator>();
        PC = FindObjectOfType<PlayerControllerNew>();
        characterAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (transform.parent.GetComponent<PlayerControllerNew>() != null)
        {
            PC = transform.parent.GetComponent<PlayerControllerNew>();
            if (PC != null)
            {
                defaultJumpHeight = PC.JumpVelocity;
                defaultSprintSpeed = PC.sprintSpeed;
            }
        }
        powerUpCoroutine = playerPowerUp();
        avatarChangeCoroutine = playerAvatarChange();
        ninjaModeCoroutine = playerNinjaMode();
        SIpowerUpCoroutine = SIPowerUp();
        avatarController = GetComponent<AvatarController>();
        Debug.Log("Building Detect Start");
    }
    #region Mubashir Avatar Work
    public void OnPowerProviderEnter(float time, float speed, float height)
    {
        powerUpTime = time;
        powerProviderHeight = height;
        powerProviderSpeed = speed;
        if (powerUpCoroutine != null)
        {
            StopCoroutine(powerUpCoroutine);
        }
        powerUpCoroutine = playerPowerUp();
        StartCoroutine(powerUpCoroutine);
    }
    int avatarIndex;
    public void OnAvatarChangerEnter(float time, int avatarIndex)
    {
        avatarChangeTime = time;
        this.avatarIndex = avatarIndex;
        StopCoroutine(avatarChangeCoroutine);
        avatarChangeCoroutine = playerAvatarChange();
        StartCoroutine(avatarChangeCoroutine);
    }

    public void OnNinjaModeEnter(float time, float speed)
    {
        ninjaUpTime = time;
        ninjaPlayerSpeed = speed;
        if (ninjaModeCoroutine != null)
            StopCoroutine(ninjaModeCoroutine);
        ninjaModeCoroutine = playerNinjaMode();
        StartCoroutine(ninjaModeCoroutine);
    }


    #region Power Up Logic
    float powerUpCurTime;
    IEnumerator playerPowerUp()
    {
        PC.JumpVelocity = powerProviderHeight;
        PC.sprintSpeed = powerProviderSpeed;
        powerUpCurTime = 0;
        while (powerUpCurTime < powerUpTime)
        {
            yield return new WaitForSeconds(1f);
            powerUpCurTime++;
        }
        PC.JumpVelocity = defaultJumpHeight;
        PC.sprintSpeed = defaultSprintSpeed;
        yield return null;
    }
    #endregion


    #region Ninja  Logic


    [SerializeField] private GameObject _ballPrefab;
    [SerializeField] private float _force = 20;
    float ninjaCurTime;
    [SerializeField] private Transform _ballSpawn;
    [SerializeField] private CinemachineFreeLook cf;
    [SerializeField] private GameObject crossHairAimNinja;

    bool startingAnimation = false;
    bool animationControlling = false;
    IEnumerator playerNinjaMode()
    {
        PC.sprintSpeed = ninjaPlayerSpeed;
        ninjaCurTime = 0;
        crossHairAimNinja.SetActive(true);
        startingAnimation = true;
        while (ninjaCurTime < ninjaUpTime)
        {

            yield return new WaitForSeconds(1f);

            ninjaCurTime++;

        }
        startingAnimation = false;
        characterAnimator.runtimeAnimatorController = defaultAnimator;
        PC.sprintSpeed = defaultSprintSpeed;
        crossHairAimNinja.SetActive(false);
        yield return null;

    }
    private void Update()
    {
        if (startingAnimation)
        {
            AnimationBehaviourNinjaMode();
        }

    }
    void AnimationBehaviourNinjaMode()
    {
        animationControlling = Input.GetKey(KeyCode.G) ? true : false;
        if (animationControlling)
        {

            characterAnimator.SetBool("throw", true);
        }
        if (Input.GetKeyUp(KeyCode.G) && characterAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f && characterAnimator.GetCurrentAnimatorStateInfo(0).IsName("throw"))
        {
            this.transform.rotation = Quaternion.Euler(0, PC.controllerCamera.transform.rotation.eulerAngles.y, 0);
            characterAnimator.SetBool("throw", false);
            characterAnimator.SetBool("throwing", true);

            Invoke("ThrowFalse", 0.1f);
        }
        else if (!animationControlling)
        {
            characterAnimator.SetBool("throw", false);
            characterAnimator.SetBool("throwing", false);
        }
        characterAnimator.runtimeAnimatorController = ninjaOverrideAnimator;
    }
    void ThrowFalse()
    {
        GameObject spawned = Instantiate(_ballPrefab, _ballSpawn.position, _ballSpawn.rotation);
        spawned.GetComponent<Rigidbody>().AddForce((PC.controllerCamera.transform.forward * _force) * cf.m_YAxis.Value);
        Destroy(spawned, 1f);
    }
    #endregion

    #region Avatar Model Changing Logic
    float avatarTime;
    IEnumerator playerAvatarChange()
    {
        avatarTime = 0;
        curModel.gameObject.SetActive(false);
        avatarController.wornHair.SetActive(false);
        avatarController.wornPant.SetActive(false);
        avatarController.wornShirt.SetActive(false);
        avatarController.wornShose.SetActive(false);
        for (int i = 0; i < tmpModel.Length; i++)
        {
            if (i == this.avatarIndex)
                tmpModel[i].gameObject.SetActive(true);
            else
                tmpModel[i].gameObject.SetActive(false);
        }
        while (avatarTime < avatarChangeTime)
        {
            yield return new WaitForSecondsRealtime(1f);
            avatarTime++;
        }
        avatarController.wornHair.SetActive(true);
        avatarController.wornPant.SetActive(true);
        avatarController.wornShirt.SetActive(true);
        avatarController.wornShose.SetActive(true);
        curModel.gameObject.SetActive(true);
        tmpModel[this.avatarIndex].gameObject.SetActive(false);
        yield return null;
    }
    #endregion
    #endregion

    #region Attizaz Special Item Work

    IEnumerator SIpowerUpCoroutine;
    public GameObject _specialEffects;
    public static bool canRunCo = false;
    public TextMeshProUGUI _remainingText;
    float _timer;
    float defaultMoveSpeed = 0;
    public void SpecialItemPowerUp(float time, float speed, float height)
    {
        print("timer");
        powerUpTime = time;
        _timer = time;
        powerProviderHeight = height;
        powerProviderSpeed = speed;
        //  canRunCo = false;
        SIpowerUpCoroutine = SIPowerUp();
        // StopCoroutine(SIpowerUpCoroutine);
        canRunCo = true;
        StartCoroutine(SIpowerUpCoroutine);
    }
    public void StoppingCoroutine()
    {
        print("CoStop");
        canRunCo = false;
        //  SIpowerUpCoroutine = SIPowerUp();
        StopCoroutine(SIpowerUpCoroutine);

    }
    IEnumerator SIPowerUp()
    {
        print("Calling Routine" + _timer);
        if (PC != null)
        {
            PC.JumpVelocity = powerProviderHeight;
            PC.sprintSpeed = powerProviderSpeed;
        }
        //   _specialEffects.Play();
        _specialEffects.gameObject.SetActive(true);
        powerUpCurTime = 0;

        _remainingText.gameObject.SetActive(true);
        while (canRunCo && !_timer.Equals(0))//&&powerUpCurTime < powerUpTime)
        {
            _timer -= Time.deltaTime;
            _remainingText.text = _timer.ToString("00");
            _timer = Mathf.Clamp(_timer, 0, Mathf.Infinity);
            PC.movementSpeed = powerProviderSpeed;
            yield return null;
            // powerUpCurTime++;
        }
        //_specialEffects.Stop();
        // canRunCo = false;
        _specialEffects.gameObject.SetActive(false);
        _remainingText.gameObject.SetActive(false);
        if (PC != null)
        {
            PC.JumpVelocity = defaultJumpHeight;
            PC.sprintSpeed = defaultSprintSpeed;
        }//  yield return null;
    }
    #endregion
}