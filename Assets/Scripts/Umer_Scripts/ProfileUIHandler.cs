using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

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
    public GameObject AvatarRef;
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
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        InstantiateUserPreviewAvatar();
    }

    private void OnEnable()
    {
        GameManager.Instance.defaultSelection = 10;
        SetCameraRenderTexture();
        if (AvatarRef)
        {
            menuLightingObj.SetActive(false);
            lightingObj.SetActive(true);
            AvatarRef.SetActive(true);
        }
    }

    private void OnDisable()
    {
        if (_renderTexCamera != null)
        {
            _renderTexCamera.GetComponent<Camera>().targetTexture = null;
            _renderTexCamera.gameObject.SetActive(false);
        }
        
        Object.Destroy(newRenderTexture);

        if (AvatarRef)
        {
            menuLightingObj.SetActive(true);
            lightingObj.SetActive(false);
            AvatarRef.SetActive(false);
        }
    }

    private void Start()
    {
        mainButtonPanelScriptRef.sameSelectionScript = immitateMainButtonPanelScriptRef;
        otherUserButtonPanelScriptRef.sameSelectionScript = immitateOtherUserButtonPanelScriptRef;
    }

    public void InstantiateUserPreviewAvatar()
    {
        _renderTexCamera.parent = null;
        _renderTexCamera.position = new Vector3(5000f, 0.86f, -5.27f);

        var friendHomeManager = GameManager.Instance.FriendsHomeManager.GetComponent<FriendHomeManager>();
        lightingObj = friendHomeManager.profileLightingObj;
        menuLightingObj = friendHomeManager.menuLightObj;

        AvatarRef = Instantiate(friendHomeManager.FriendAvatarPrefab.gameObject);
        AvatarRef.GetComponent<FootStaticIK>().ikActive = true;
        AvatarRef.name = "UserPreviewAvatar";
        AvatarRef.transform.position = new Vector3(5000f, 0.069f, 0f);
        AvatarRef.GetComponent<Animator>().runtimeAnimatorController = _userIdleAnimator.runtimeAnimatorController;

        Destroy(AvatarRef.GetComponent<CharacterOnScreenNameHandler>());
        Destroy(AvatarRef.GetComponent<Actor>());
        Destroy(AvatarRef.GetComponent<PlayerPostBubbleHandler>());

        Instantiate(avatarBgObject, AvatarRef.transform);
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
        if (AvatarRef)
        {
            _tempAvatarData = _userAvatarData;
            AvatarRef.GetComponent<AvatarController>().InitializeFrndAvatar(_userAvatarData,AvatarRef);
        }
    }

    public void SetUserAvatarDefaultClothing()
    {
        int _rand = Random.Range(0, 13);
        AvatarRef.GetComponent<AvatarController>().DownloadRandomFrndPresets(_rand);
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
