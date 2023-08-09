using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;
using WalletConnectSharp.Core.Events;

public class BuildingDetect : MonoBehaviour
{
    [Header("Avatar Change")]
    public GameObject[] tmpModel;

    #region Special Item Fields

    [Header("Player Body Parts")]
    [SerializeField]
    private SkinnedMeshRenderer playerHair;
    [SerializeField]
    private SkinnedMeshRenderer playerBody;
    [SerializeField]
    private SkinnedMeshRenderer playerShirt;
    [SerializeField]
    private SkinnedMeshRenderer playerPants;
    [SerializeField]
    public SkinnedMeshRenderer playerShoes;

    [Header("Default Mats")]
    [SerializeField]
    private Material defaultHairMat;
    [SerializeField]
    private Material defaultBodyMat;
    [SerializeField]
    private Material defaultShirtMat;
    [SerializeField]
    private Material defaultPantsMat;
    [SerializeField]
    private Material defaultShoesMat;

    [Header("Gangster Character")]
    public GameObject gangsterCharacter;
    #endregion

    #region Avatar invisibility materials and gameobjects

    [Header("Player Head")]
    [SerializeField]
    private SkinnedMeshRenderer playerHead;

    [Header("Player Invisibility Material")]
    [SerializeField]
    private Material hologramMaterial;

    [Header("Default Head Materials")]
    [SerializeField]
    private Material[] defaultHeadMaterials;

    #endregion

    //vThirdPersonController cc;
    float defaultJumpHeight, defaultSprintSpeed;
    float powerProviderHeight, powerProviderSpeed;
    float powerUpTime, avatarChangeTime;
    IEnumerator powerUpCoroutine, avatarChangeCoroutine;

    [Space(15)]
    public AnimatorOverrideController ninjaOverrideAnimator;
    public RuntimeAnimatorController defaultAnimator;
    private Animator characterAnimator;
    public SkinnedMeshRenderer[] skinMeshs;

    [Header("Set default value of ProstProcessProfile vol vignette")]
    internal float defaultSmoothvalue;
    internal float defaultIntensityvalue;

    private void Awake()
    {
        hologramMaterial = GamificationComponentData.instance.hologramMaterial;
    }

    IEnumerator Start()
    {
        _playerControllerNew = GamificationComponentData.instance.playerControllerNew;

        defaultJumpHeight = _playerControllerNew.JumpVelocity;
        defaultSprintSpeed = _playerControllerNew.sprintSpeed;
        defaultMoveSpeed = _playerControllerNew.movementSpeed;

        powerUpCoroutine = playerPowerUp();
        avatarChangeCoroutine = PlayerAvatarChange();

        SIpowerUpCoroutine = SIPowerUp();
        if (vignette != null)
        {
            vignette.active = false;
            motionBlur.active = false;
        }

        yield return new WaitForSeconds(2f);

        //_playerControllerNew = GamificationComponentData.instance.playerControllerNew;
        //Initializing
        playerHair = GamificationComponentData.instance.avatarController.wornHair.GetComponent<SkinnedMeshRenderer>();
        playerPants = GamificationComponentData.instance.avatarController.wornPant.GetComponent<SkinnedMeshRenderer>();
        playerShirt = GamificationComponentData.instance.avatarController.wornShirt.GetComponent<SkinnedMeshRenderer>();
        playerShoes = GamificationComponentData.instance.avatarController.wornShose.GetComponent<SkinnedMeshRenderer>();
        playerBody = GamificationComponentData.instance.charcterBodyParts.Body;

        playerHead = GamificationComponentData.instance.charcterBodyParts.Head.GetComponent<SkinnedMeshRenderer>();

        defaultHeadMaterials = new Material[playerHead.sharedMesh.subMeshCount];
        for (int i = 0; i < playerHead.materials.Length; i++)
        {
            defaultHeadMaterials[i] = playerHead.materials[i];
        }

        defaultBodyMat = playerBody.material;
        defaultPantsMat = playerPants.material;
        defaultShirtMat = playerShirt.material;
        defaultHairMat = playerHair.material;
        defaultShoesMat = playerShoes.material;
    }

    private void OnEnable()
    {
        BuilderEventManager.ActivateAvatarInivisibility += AvatarInvisibilityApply;
        BuilderEventManager.DeactivateAvatarInivisibility += StopAvatarInvisibility;
    }
    private void OnDisable()
    {
        BuilderEventManager.ActivateAvatarInivisibility -= AvatarInvisibilityApply;
        BuilderEventManager.DeactivateAvatarInivisibility -= StopAvatarInvisibility;
    }

    #region Mubashir Avatar Work
    public void OnPowerProviderEnter(float time, float speed, float height)
    {
        powerUpTime = time;
        powerProviderHeight = height;
        powerProviderSpeed = speed;
        StopCoroutine(powerUpCoroutine);
        powerUpCoroutine = playerPowerUp();
        StartCoroutine(powerUpCoroutine);
    }

    #region Power Up Logic
    float powerUpCurTime;
    IEnumerator playerPowerUp()
    {
        _playerControllerNew.jumpHeight = powerProviderHeight;
        _playerControllerNew.sprintSpeed = powerProviderSpeed;
        powerUpCurTime = 0;
        while (powerUpCurTime < powerUpTime)
        {
            yield return new WaitForSeconds(1f);
            powerUpCurTime++;
        }
        _playerControllerNew.jumpHeight = defaultJumpHeight;
        _playerControllerNew.sprintSpeed = defaultSprintSpeed;
        yield return null;
    }
    #endregion
    #endregion

    #region Avatar Model Changing Logic

    int avatarIndex;
    public void OnAvatarChangerEnter(float time, int avatarIndex)
    {
        avatarChangeTime = time;
        this.avatarIndex = avatarIndex;
        StopCoroutine(avatarChangeCoroutine);
        avatarChangeCoroutine = PlayerAvatarChange();
        StartCoroutine(avatarChangeCoroutine);
    }
    float avatarTime;
    IEnumerator PlayerAvatarChange()
    {
        avatarTime = 0;

        ToggleSkinMesh(false);
        Animator tempAnimator = this.GetComponent<PlayerControllerNew>().animator;
        this.GetComponent<PlayerControllerNew>().animator = gangsterCharacter.GetComponent<Animator>();
        gangsterCharacter.SetActive(true);


        while (avatarChangeTime > avatarTime)
        {
            int minutes = (int)avatarChangeTime / 60;
            int seconds = (int)avatarChangeTime % 60;

            //_remainingText.text = $"{minutes:D2}:{seconds:D2}";

            avatarChangeTime = Mathf.Clamp(avatarChangeTime, 0, Mathf.Infinity);

            yield return new WaitForSecondsRealtime(1f);
            avatarChangeTime--;
        }

        this.GetComponent<PlayerControllerNew>().animator = tempAnimator;
        ToggleSkinMesh(true);

        gangsterCharacter.SetActive(false);

        yield return null;
    }

    void ToggleSkinMesh(bool state)
    {
        playerHair.enabled = state;
        playerBody.enabled = state;
        playerHead.enabled = state;
        playerPants.enabled = state;
        playerShirt.enabled = state;
        playerShoes.enabled = state;
    }
    #endregion

    #region Attizaz Special Item Work

    IEnumerator SIpowerUpCoroutine;
    GameObject _specialEffects;
    PlayerControllerNew _playerControllerNew;
    public static bool canRunCo = false;
    //public TextMeshProUGUI _remainingText;
    float _timer;
    float defaultMoveSpeed = 0;
    Shader defaultSkinShader, defaultClothShader;
    Shader newSkinShader, newClothShader;
    public void SpecialItemPowerUp(float time, float speed, float height)
    {
        defaultSkinShader = GamificationComponentData.instance.skinShader;
        defaultClothShader = GamificationComponentData.instance.cloathShader;
        newSkinShader = GamificationComponentData.instance.superMarioShader;
        newClothShader = GamificationComponentData.instance.superMarioShader2;

        BuilderEventManager.OnSpecialItemComponentCollisionEnter?.Invoke(time);

        //GetMaterialsFromCharacter();
        print("timer");
        powerUpTime = time;
        _timer = time;
        powerProviderHeight = height;
        powerProviderSpeed = speed;
        SIpowerUpCoroutine = SIPowerUp();
        canRunCo = true;

        if (_specialEffects == null)
        {
            GameObject effect = GamificationComponentData.instance.specialItemParticleEffect;
            _specialEffects = Instantiate(effect, ReferrencesForDynamicMuseum.instance.m_34player.transform);
        }
        StartCoroutine(SIpowerUpCoroutine);
    }
    public void StoppingCoroutine()
    {
        print("CoStop");
        canRunCo = false;
        StopCoroutine(SIpowerUpCoroutine);

    }
    IEnumerator SIPowerUp()
    {
        print("Calling Routine" + _timer);
        yield return new WaitForSeconds(0.2f);
        BuilderEventManager.SpecialItemPlayerPropertiesUpdate?.Invoke(powerProviderHeight, powerProviderSpeed);
        _specialEffects.gameObject.SetActive(true);
        ApplySuperMarioEffect();
        powerUpCurTime = 0;
        _playerControllerNew.specialItem = true;

        while (canRunCo && !_timer.Equals(0))//&&powerUpCurTime < powerUpTime)
        {
            _timer -= Time.deltaTime;
            _timer = Mathf.Clamp(_timer, 0, Mathf.Infinity);
            _playerControllerNew.movementSpeed = powerProviderSpeed;
            yield return null;
        }
        _specialEffects.gameObject.SetActive(false);
        ApplyDefaultEffect();

        _playerControllerNew.specialItem = false;
        _playerControllerNew.movementSpeed = defaultMoveSpeed;
        BuilderEventManager.SpecialItemPlayerPropertiesUpdate?.Invoke(defaultJumpHeight, defaultSprintSpeed);
    }

    private void ApplySuperMarioEffect()
    {
        playerHair.material.shader = newClothShader;
        playerBody.material.shader = newSkinShader;
        playerBody.material.SetFloat("_Outer_Glow", 2);
        playerShirt.material.shader = newClothShader;
        playerPants.material.shader = newClothShader;
        playerShoes.material.shader = newClothShader;
    }

    private void ApplyDefaultEffect()
    {
        playerHair.material.shader = defaultClothShader;
        playerBody.material.shader = defaultSkinShader;
        playerShirt.material.shader = defaultClothShader;
        playerPants.material.shader = defaultClothShader;
        playerShoes.material.shader = defaultClothShader;
        playerHead.sharedMaterials = defaultHeadMaterials;
    }

    public void StopSpecialItemComponent()
    {
        //_remainingText.gameObject.SetActive(false);
        if (_playerControllerNew.specialItem)
        {
            _playerControllerNew.specialItem = false;
            BuilderEventManager.OnSpecialItemComponentCollisionEnter?.Invoke(0);
        }
        if (_specialEffects)
        {
            _specialEffects.SetActive(false);
            if (_specialEffects.activeInHierarchy)
                ApplyDefaultEffect();
        }
        canRunCo = false;
        _playerControllerNew.jumpHeight = defaultJumpHeight;
        _playerControllerNew.sprintSpeed = defaultSprintSpeed;
        _playerControllerNew.movementSpeed = defaultMoveSpeed;
    }
    #endregion

    #region Avatar Invisibility 

    void AvatarInvisibilityApply()
    {
        playerHair.material = hologramMaterial;
        playerBody.material = hologramMaterial;
        playerShirt.material = hologramMaterial;
        playerPants.material = hologramMaterial;
        playerShoes.material = hologramMaterial;
        Material[] newMaterials = new Material[playerHead.sharedMesh.subMeshCount];

        // Assign the new material to all submeshes
        for (int i = 0; i < newMaterials.Length; i++)
        {
            newMaterials[i] = hologramMaterial;
        }

        // Apply the new materials to the SkinnedMeshRenderer
        playerHead.materials = newMaterials;
    }

    void StopAvatarInvisibility()
    {
        playerHair.material = defaultHairMat;
        playerBody.material = defaultBodyMat;
        playerShirt.material = defaultShirtMat;
        playerPants.material = defaultPantsMat;
        playerShoes.material = defaultShoesMat;
        playerHead.sharedMaterials = defaultHeadMaterials;
    }

    #endregion

    #region Camera Blur Effect
    Animator cameraAnimator;

    Volume volume;

    private MotionBlur motionBlur;
    private Vignette vignette;

    public void CameraEffect()
    {
        StopSpecialItemComponent();
        volume = GamificationComponentData.instance.postProcessVol;
        cameraAnimator = GamificationComponentData.instance.playerControllerNew.ActiveCamera.GetComponent<Animator>();
        StartCoroutine(WaitForEffect());
    }
    IEnumerator WaitForEffect()
    {

        cameraAnimator.SetBool("BlurrEffect", true);
        volume.profile.TryGet(out motionBlur);
        volume.profile.TryGet(out vignette);
        vignette.active = true;
        motionBlur.active = true;
        vignette.intensity.value = 0.33f;
        vignette.smoothness.value = 0.702f;

        yield return new WaitForSeconds(0.4f);

        float timeElapsed = 0f;
        while (timeElapsed < 0.2f)
        {
            vignette.intensity.value = Mathf.Lerp(0.33f, 0, timeElapsed / 0.2f);
            yield return null;
            timeElapsed += Time.deltaTime;
        }

        cameraAnimator.SetBool("BlurrEffect", false);
        vignette.intensity.value = defaultIntensityvalue;
        vignette.smoothness.value = defaultSmoothvalue;
        vignette.active = false;
        motionBlur.active = false;
    }
    #endregion
}