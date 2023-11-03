using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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

    [SerializeField]
    public MeshRenderer playerFreeCamConsole;
    [SerializeField]
    public MeshRenderer playerFreeCamConsoleOther;

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
    [SerializeField]
    private Material defaultFreeCamConsoleMat;

    [Header("Gangster Character")]
    internal GameObject gangsterCharacter;
    GameObject AppearanceChange;
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
    IEnumerator powerUpCoroutine;

    Coroutine avatarChangeCoroutine;

    [Space(15)]
    public AnimatorOverrideController ninjaOverrideAnimator;
    public RuntimeAnimatorController defaultAnimator;
    private Animator characterAnimator;
    public SkinnedMeshRenderer[] skinMeshs;

    [Header("Set default value of ProstProcessProfile vol vignette")]
    internal float defaultSmootnesshvalue;
    internal float defaultIntensityvalue;

    float nameCanvasDefaultYpos;

    private void Awake()
    {
        hologramMaterial = GamificationComponentData.instance.hologramMaterial;
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(2f);

        _playerControllerNew = GamificationComponentData.instance.playerControllerNew;

        defaultJumpHeight = _playerControllerNew.JumpVelocity;
        defaultSprintSpeed = _playerControllerNew.sprintSpeed;
        defaultMoveSpeed = _playerControllerNew.movementSpeed;

        powerUpCoroutine = playerPowerUp();

        SIpowerUpCoroutine = SIPowerUp();
        if (vignette != null)
        {
            vignette.active = false;
            motionBlur.active = false;
        }
        //Initializing
        playerHair = GamificationComponentData.instance.avatarController.wornHair.GetComponent<SkinnedMeshRenderer>();
        playerPants = GamificationComponentData.instance.avatarController.wornPant.GetComponent<SkinnedMeshRenderer>();
        playerShirt = GamificationComponentData.instance.avatarController.wornShirt.GetComponent<SkinnedMeshRenderer>();
        playerShoes = GamificationComponentData.instance.avatarController.wornShose.GetComponent<SkinnedMeshRenderer>();
        playerBody = GamificationComponentData.instance.charcterBodyParts.Body;

        playerHead = GamificationComponentData.instance.charcterBodyParts.Head.GetComponent<SkinnedMeshRenderer>();

        playerFreeCamConsole = GamificationComponentData.instance.ikMuseum.ConsoleObj.GetComponent<MeshRenderer>();
        playerFreeCamConsoleOther = GamificationComponentData.instance.ikMuseum.m_ConsoleObjOther.GetComponent<MeshRenderer>();

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

        defaultFreeCamConsoleMat = playerFreeCamConsole.material;

        nameCanvasDefaultYpos = ArrowManager.Instance.nameCanvas.transform.localPosition.y;
    }

    private void OnEnable()
    {
        BuilderEventManager.ActivateAvatarInivisibility += AvatarInvisibilityApply;
        BuilderEventManager.DeactivateAvatarInivisibility += StopAvatarInvisibility;
        BuilderEventManager.StopAvatarChangeComponent += ToggleAvatarChangeComponent;
    }
    private void OnDisable()
    {
        BuilderEventManager.ActivateAvatarInivisibility -= AvatarInvisibilityApply;
        BuilderEventManager.DeactivateAvatarInivisibility -= StopAvatarInvisibility;
        BuilderEventManager.StopAvatarChangeComponent -= ToggleAvatarChangeComponent;
        ApplyDefaultEffect();

    }

    #region Avatar Work
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
    public void OnAvatarChangerEnter(float time, int avatarIndex, GameObject curObject)
    {
        if (tempAnimator != null)
        {
            this.GetComponent<Animator>().avatar = tempAnimator;
            this.GetComponent<Animator>().cullingMode = cullingMode;
        }
        BuilderEventManager.StopAvatarChangeComponent?.Invoke(true);
        //StopCoroutine(avatarChangeCoroutine);
        avatarChangeTime = time;
        avatarIndex = avatarIndex - 1;

        if (avatarIndex == 1)
        {
            Vector3 canvasPos = ArrowManager.Instance.nameCanvas.transform.localPosition;
            canvasPos.y = -1.3f;
            ArrowManager.Instance.nameCanvas.transform.localPosition = canvasPos;
        }

        if (avatarChangeCoroutine != null)
            StopCoroutine(avatarChangeCoroutine);
        gangsterCharacter = new GameObject("AvatarChange");
        gangsterCharacter.transform.SetParent(this.transform);
        gangsterCharacter.transform.localPosition = Vector3.zero;
        gangsterCharacter.transform.localEulerAngles = Vector3.zero;
        //gangsterCharacter.SetActive(false);

        Vector3 pos = gangsterCharacter.transform.position;
        pos.y = GamificationComponentData.instance.AvatarChangerModelNames[avatarIndex] == "Bear05" ? 0.1f : 0;
        AppearanceChange = PhotonNetwork.Instantiate(GamificationComponentData.instance.AvatarChangerModelNames[avatarIndex], pos, Quaternion.identity);

        var hash = new ExitGames.Client.Photon.Hashtable();
        hash.Add("avatarChanger", (avatarIndex + 1) + "," + curObject.GetComponent<XanaItem>().itemData.RuntimeItemID + "," + this.GetComponent<PhotonView>().ViewID);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        AppearanceChange.transform.SetParent(gangsterCharacter.transform);
        AppearanceChange.transform.localEulerAngles = Vector3.zero;
        CharacterControls cc = gangsterCharacter.GetComponentInChildren<CharacterControls>();
        if (cc != null)
        {
            cc.playerControler = GamificationComponentData.instance.playerControllerNew;
        }
        if (avatarIndex == 2)
        {
            GameObject cloneObject = Instantiate(curObject);

            Component[] components = cloneObject.GetComponents<Component>();
            for (int i = components.Length - 1; i >= 0; i--)
            {
                if (!(components[i] is Transform))
                {
                    Destroy(components[i]);
                }
            }

            cloneObject.transform.SetParent(AppearanceChange.transform);
            cloneObject.transform.localPosition = Vector3.zero;
            cloneObject.transform.localEulerAngles = Vector3.zero;
            cloneObject.SetActive(true);
        }



        //hide meshdata off character for FPS
        if (GamificationComponentData.instance.playerControllerNew.isFirstPerson)
        {
            Transform[] transforms = gangsterCharacter.gameObject.GetComponentsInChildren<Transform>();

            foreach (Transform childTransform in transforms)
            {
                if (childTransform.gameObject.GetComponent<Renderer>())
                {
                    childTransform.gameObject.GetComponent<Renderer>().enabled = false;
                }
            }
        }
        avatarChangeCoroutine = StartCoroutine(PlayerAvatarChange());

        GamificationComponentData.instance.isAvatarChanger = true;
    }
    float avatarTime;
    Avatar tempAnimator;
    AnimatorCullingMode cullingMode;
    IEnumerator PlayerAvatarChange()
    {
        avatarTime = 0;

        ToggleAvatarChangeComponent(false);
        if (tempAnimator == null)
        {
            tempAnimator = this.GetComponent<Animator>().avatar;
            cullingMode = this.GetComponent<Animator>().cullingMode;
        }


        gangsterCharacter.GetComponentInChildren<Animator>().enabled = false;
        yield return new WaitForSecondsRealtime(0.01f);
        this.GetComponent<Animator>().avatar = gangsterCharacter.GetComponentInChildren<Animator>().avatar;
        this.GetComponent<Animator>().cullingMode = gangsterCharacter.GetComponentInChildren<Animator>().cullingMode;

        if (!GamificationComponentData.instance.playerControllerNew.isFirstPerson)
            gangsterCharacter.SetActive(true);

        BuilderEventManager.OnAvatarChangeComponentTriggerEnter?.Invoke(avatarChangeTime);


        while (avatarChangeTime > avatarTime)
        {
            avatarChangeTime = Mathf.Clamp(avatarChangeTime, 0, Mathf.Infinity);
            yield return new WaitForSecondsRealtime(1f);
            avatarChangeTime--;
        }

        //this.GetComponent<Animator>().avatar = tempAnimator;
        ToggleAvatarChangeComponent(true);

        yield return null;
    }

    void ToggleAvatarChangeComponent(bool state)
    {
        if (state)
        {
            if (avatarChangeCoroutine != null)
                StopCoroutine(avatarChangeCoroutine);

            if (gangsterCharacter != null)
            {
                this.GetComponent<Animator>().avatar = tempAnimator;
                PhotonNetwork.Destroy(AppearanceChange.GetPhotonView());
                Destroy(gangsterCharacter);
            }
            BuilderEventManager.OnAvatarChangeComponentTriggerEnter?.Invoke(0);

            Vector3 canvasPos = ArrowManager.Instance.nameCanvas.transform.localPosition;
            canvasPos.y = nameCanvasDefaultYpos;
            ArrowManager.Instance.nameCanvas.transform.localPosition = canvasPos;
            if (avatarChangeCoroutine != null)
                StopCoroutine(avatarChangeCoroutine);
            avatarChangeCoroutine = null;

            GamificationComponentData.instance.isAvatarChanger = false;
        }
        else if (gangsterCharacter != null)
            gangsterCharacter.SetActive(true);

        if (!GamificationComponentData.instance.playerControllerNew.isFirstPerson)
        {
            playerHair.enabled = state;
            playerBody.enabled = state;
            playerHead.enabled = state;
            playerPants.enabled = state;
            playerShirt.enabled = state;
            playerShoes.enabled = state;
        }
    }
    #endregion

    #region Special Item Work

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
            //GameObject effect = GamificationComponentData.instance.specialItemParticleEffect;
            Vector3 pos = ReferrencesForDynamicMuseum.instance.m_34player.transform.position;
            pos.y += GamificationComponentData.instance.specialItemParticleEffect.transform.position.y;
            //Quaternion rot = ReferrencesForDynamicMuseum.instance.m_34player.transform.rotation;
            //rot.y+= GamificationComponentData.instance.specialItemParticleEffect.transform.rotation.y;
            _specialEffects = PhotonNetwork.Instantiate(GamificationComponentData.instance.specialItemParticleEffect.name, pos, GamificationComponentData.instance.specialItemParticleEffect.transform.rotation);
            _specialEffects.transform.SetParent(ReferrencesForDynamicMuseum.instance.m_34player.transform);

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
        //_specialEffects.gameObject.SetActive(true);
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
        //_specialEffects.gameObject.SetActive(false);
        if (_specialEffects)
            PhotonNetwork.Destroy(_specialEffects.GetPhotonView());
        ApplyDefaultEffect();
        _specialEffects = null;
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
            //_specialEffects.SetActive(false);
            //if (_specialEffects.activeInHierarchy)
            PhotonNetwork.Destroy(_specialEffects.GetPhotonView());
            ApplyDefaultEffect();
            _specialEffects = null;
        }
        canRunCo = false;
        _playerControllerNew.jumpHeight = defaultJumpHeight;
        _playerControllerNew.sprintSpeed = defaultSprintSpeed;
        _playerControllerNew.movementSpeed = defaultMoveSpeed;
    }
    #endregion

    #region Avatar Invisibility 
    //Hologram Material Set
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

        playerFreeCamConsole.material = hologramMaterial;
        playerFreeCamConsoleOther.material = hologramMaterial;
    }

    void StopAvatarInvisibility()
    {
        playerHair.material = defaultHairMat;
        playerBody.material = defaultBodyMat;
        playerShirt.material = defaultShirtMat;
        playerPants.material = defaultPantsMat;
        playerShoes.material = defaultShoesMat;
        playerHead.sharedMaterials = defaultHeadMaterials;
        playerFreeCamConsole.material = defaultFreeCamConsoleMat;
        playerFreeCamConsoleOther.material = defaultFreeCamConsoleMat;
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
        RuntimeAnimatorController cameraEffect = GamificationComponentData.instance.cameraBlurEffect;
        cameraAnimator = GamificationComponentData.instance.playerControllerNew.ActiveCamera.GetComponent<Animator>();
        if (cameraAnimator == null) cameraAnimator = GamificationComponentData.instance.playerControllerNew.ActiveCamera.AddComponent<Animator>();
        cameraAnimator.runtimeAnimatorController = cameraEffect;
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
        vignette.smoothness.value = defaultSmootnesshvalue;
        vignette.active = false;
        motionBlur.active = false;
    }
    #endregion
}