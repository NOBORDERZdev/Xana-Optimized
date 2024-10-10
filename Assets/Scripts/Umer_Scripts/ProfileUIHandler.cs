using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using static InventoryManager;

public class ProfileUIHandler : MonoBehaviour
{
    public static ProfileUIHandler instance;
    [Header("User Data Tabs Buttons")]
    public GameObject myProfileTopPartButton;
    public GameObject OtherProfileTopPartButton;
    public GameObject UserTagsParent;
    public GameObject TagPrefab;
    public Button followerBtn;
    public Button followingBtn;
    public GameObject editProfileBtn;
    public GameObject followProfileBtn;
    public GameObject avatarBgObject;


    [Space]
    [Header("User Avatar Preview Objects")]
    public GameObject avatarRef;
    public GameObject maleAvatarRef;
    public GameObject femaleAvatarRef;
    public Transform _renderTexCamera;
    RenderTexture newRenderTexture;
    public RawImage AvatarPreviewImgRef;
    public AnimatorOverrideController _userIdleAnimator;
    public SavingCharacterDataClass _tempAvatarData;

    [Space]
    [Header("User Post Containers")]
    public GameObject myProfileUserPostPartObj;
    public GameObject OtherProfileUserPostPartObj;

    [Space]
    [Header("User Data Tabs Imitating Buttons")]

    [Space]
    [Header("Script References")]
    public MainScrollController mainscrollControllerRef;
    public SelectionItemScript mainButtonPanelScriptRef;
    public SelectionItemScript otherUserButtonPanelScriptRef;
    public SelectionItemScript immitateMainButtonPanelScriptRef;
    public SelectionItemScript immitateOtherUserButtonPanelScriptRef;

    private GameObject menuLightingObj;
    private GameObject lightingObj;

    private FriendHomeManager ref_FriendHomeManager;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        //InstantiateUserPreviewAvatar();
    }

    private void OnEnable()
    {
        GameManager.Instance.defaultSelection = 10;
        SetCameraRenderTexture();

        if ((CharacterHandler.instance.activePlayerGender == AvatarGender.Male && !maleAvatarRef) ||
            (CharacterHandler.instance.activePlayerGender == AvatarGender.Female && !femaleAvatarRef))
        {
            InstantiateUserPreviewAvatar(CharacterHandler.instance.activePlayerGender);
        }
        else
        {
            if (CharacterHandler.instance.activePlayerGender == AvatarGender.Male)
            {
                maleAvatarRef.SetActive(true);
                avatarRef = maleAvatarRef.gameObject;
            }
            else
            {
                femaleAvatarRef.SetActive(true);
                avatarRef = femaleAvatarRef.gameObject;
            }
        }

        MyProfileDataManager.Instance.UpdateBackButtonOnClickListener();
    }

    private void OnDisable()
    {
        if (_renderTexCamera != null)
        {
            _renderTexCamera.GetComponent<Camera>().targetTexture = null;
            _renderTexCamera.gameObject.SetActive(false);
        }

        // Properly destroy the newRenderTexture
        Object.Destroy(newRenderTexture);

        // Set game objects
        menuLightingObj.SetActive(true);
        lightingObj.SetActive(false);
       
        if (maleAvatarRef)
            maleAvatarRef.SetActive(false);
        if (femaleAvatarRef)
            femaleAvatarRef.SetActive(false);
    }

    private void Start()
    {
        mainButtonPanelScriptRef.sameSelectionScript = immitateMainButtonPanelScriptRef;
        otherUserButtonPanelScriptRef.sameSelectionScript = immitateOtherUserButtonPanelScriptRef;
        //ref_FriendHomeManager = GameManager.Instance.FriendsHomeManager.GetComponent<FriendHomeManager>();
    }
    public void ActivateProfileAvatarByGender(string gender)
    {
        switch (gender)
        {
            case "Male":
                if (maleAvatarRef)
                    maleAvatarRef.SetActive(true);
                else
                    InstantiateUserPreviewAvatar(AvatarGender.Male);

                if (femaleAvatarRef)
                    femaleAvatarRef.SetActive(false);
                avatarRef = maleAvatarRef.gameObject;
                break;
            case "Female":
                if (femaleAvatarRef)
                    femaleAvatarRef.SetActive(true);
                else
                    InstantiateUserPreviewAvatar(AvatarGender.Female);

                if (maleAvatarRef)
                    maleAvatarRef.SetActive(false);
                avatarRef = femaleAvatarRef.gameObject;
                break;
        }
        if (avatarRef.GetComponent<EyesBlinking>() != null)
        {
            avatarRef.GetComponent<EyesBlinking>().StoreBlendShapeValues();
            StartCoroutine(avatarRef.GetComponent<EyesBlinking>().BlinkingStartRoutine());
        }
        menuLightingObj.SetActive(false);
        lightingObj.SetActive(true);
    }
    public void InstantiateUserPreviewAvatar(AvatarGender gender)
    {
        ref_FriendHomeManager = GameManager.Instance.FriendsHomeManager.GetComponent<FriendHomeManager>();
        _renderTexCamera.parent = null;
        _renderTexCamera.position = new Vector3(5000f, 0.86f, -5.27f);
        lightingObj = ref_FriendHomeManager.profileLightingObj;
        menuLightingObj = ref_FriendHomeManager.menuLightObj;
        if (gender == AvatarGender.Male)
        {
            maleAvatarRef = Instantiate(ref_FriendHomeManager.maleFriendAvatarPrefab.gameObject);
            avatarRef = maleAvatarRef;
            avatarRef.name = "MaleUserPreviewAvatar_Parent";
        }
        else
        {
            femaleAvatarRef = Instantiate(ref_FriendHomeManager.femaleFriendAvatarPrefab.gameObject);
            avatarRef = femaleAvatarRef;
            avatarRef.name = "FemaleUserPreviewAvatar_Parent";
        }

        avatarRef.GetComponent<FootStaticIK>().ikActive = true;
        avatarRef.transform.position = new Vector3(5000f, 0.069f, 0f);
        avatarRef.GetComponent<Animator>().runtimeAnimatorController = _userIdleAnimator.runtimeAnimatorController;
        Destroy(avatarRef.GetComponent<CharacterOnScreenNameHandler>());
        Destroy(avatarRef.GetComponent<Actor>());
        Destroy(avatarRef.GetComponent<PlayerPostBubbleHandler>());

        avatarRef.gameObject.SetActive(true);

        Instantiate(avatarBgObject, avatarRef.transform);
    }


    public void SetCameraRenderTexture()
    {
        if (newRenderTexture == null)
        {
            newRenderTexture = new RenderTexture(1024, 1024, 0, UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm)
            {
                antiAliasing = 4,
                useMipMap = true,
                filterMode = FilterMode.Trilinear
            };

            var camera = _renderTexCamera.GetComponent<Camera>();
            var uaCamData = camera.GetComponent<UniversalAdditionalCameraData>();
            uaCamData.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
            uaCamData.antialiasingQuality = AntialiasingQuality.High;

            camera.targetTexture = newRenderTexture;
            AvatarPreviewImgRef.texture = newRenderTexture;
            _renderTexCamera.gameObject.SetActive(true);
        }
    }

    public void SetUserAvatarClothing(SavingCharacterDataClass _userAvatarData)
    {
        ActivateProfileAvatarByGender(_userAvatarData.gender);
        if (avatarRef)
        {
            _tempAvatarData = _userAvatarData;
            
            avatarRef.GetComponent<AvatarController>().InitializeFrndAvatar(_userAvatarData,avatarRef);
            Invoke(nameof(AddAvatarLayertoRenderCam), 0.4f);
        }
    }

    void AddAvatarLayertoRenderCam()
    {
        int bodyLayer = LayerMask.NameToLayer("Body");
        ProfileUIHandler.instance._renderTexCamera.GetComponent<Camera>().cullingMask |= (1 << bodyLayer);
    }

    public void SetUserAvatarDefaultClothing()
    {
        ActivateProfileAvatarByGender(Random.Range(0f, 2f) <= 1f ? "Male" : "Female");
        int _rand = Random.Range(0, avatarRef.GetComponent<CharacterBodyParts>().randomPresetData.Length);   
        avatarRef.GetComponent<AvatarController>().DownloadRandomFrndPresets(_rand);
        Invoke(nameof(AddAvatarLayertoRenderCam), 0.4f);
    }
    public void SetUserAvatarRandomClothingForProfile(int randPreset, string gender)
    {
        ActivateProfileAvatarByGender(gender);
        avatarRef.GetComponent<AvatarController>().DownloadRandomFrndPresets(randPreset);
    }
    public void SetMainScrollRefs()
    {
        bool isMyProfileActive = MyProfileDataManager.Instance.gameObject.activeSelf;
        mainscrollControllerRef.headerObj = isMyProfileActive ? myProfileTopPartButton : OtherProfileTopPartButton;
        mainscrollControllerRef.containerobj = (isMyProfileActive ? myProfileUserPostPartObj : OtherProfileUserPostPartObj).GetComponent<RectTransform>();
    }

    public void SwitchBetweenUserAndOtherProfileUI(bool _state)
    {
        myProfileTopPartButton.SetActive(_state);
        myProfileUserPostPartObj.SetActive(_state);
        OtherProfileTopPartButton.SetActive(!_state);
        OtherProfileUserPostPartObj.SetActive(!_state);
    }
}
