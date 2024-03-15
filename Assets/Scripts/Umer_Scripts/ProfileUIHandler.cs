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
    [Header("User Data Tabs Immitating Buttons")]
    public GameObject myProfileImitateTopPartButton;
    public GameObject OtherProfileImitateTopPartButton;

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
        _renderTexCamera.GetComponent<Camera>().targetTexture = null;
        _renderTexCamera.gameObject.SetActive(false);
        Object.Destroy(newRenderTexture);

        //newRenderTexture.Release();
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
        //_renderTexCamera.position = new Vector3(0f, 0.8f, -6f);
        _renderTexCamera.position = new Vector3(5000f, 0.86f, -5.27f);
        lightingObj = GameManager.Instance.FriendsHomeManager.GetComponent<FriendHomeManager>().profileLightingObj;
        menuLightingObj = GameManager.Instance.FriendsHomeManager.GetComponent<FriendHomeManager>().menuLightObj;
        AvatarRef = Instantiate(GameManager.Instance.FriendsHomeManager.GetComponent<FriendHomeManager>().FriendAvatarPrefab.gameObject);
        AvatarRef.GetComponent<FootStaticIK>().ikActive = true;
        AvatarRef.name = "UserPreviewAvatar";
        AvatarRef.transform.position = new Vector3(5000f, 0.069f, 0f);
        AvatarRef.GetComponent<Animator>().runtimeAnimatorController = _userIdleAnimator.runtimeAnimatorController;
        Destroy(AvatarRef.GetComponent<CharacterOnScreenNameHandler>());
        Destroy(AvatarRef.GetComponent<Actor>());
        Destroy(AvatarRef.GetComponent<PlayerPostBubbleHandler>());

        GameObject temp = Instantiate(avatarBgObject, AvatarRef.transform);
        //Destroy(AvatarRef.GetComponent<AvatarController>());
        //_userAvatarData = GameManager.Instance.mainCharacter.GetComponent<AvatarController>()._PCharacterData;
        //SetUserAvatarClothing();
    }

    public void SetCameraRenderTexture()
    {
        if (!newRenderTexture)
        {
            newRenderTexture = new RenderTexture(1024, 1024, 0, UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm);
            newRenderTexture.antiAliasing = 4;
            newRenderTexture.useMipMap = true;
            newRenderTexture.filterMode = FilterMode.Trilinear;
            //if (Application.platform == RuntimePlatform.Android)
            //{
                UniversalAdditionalCameraData _uaCamData = _renderTexCamera.GetComponent<Camera>().GetComponent<UniversalAdditionalCameraData>();
                _uaCamData.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
                _uaCamData.antialiasingQuality = AntialiasingQuality.High; //AntialiasingQuality.Low;
            //}

            //Graphics.Blit(m_RenderTexture, newRenderTexture);
            _renderTexCamera.GetComponent<Camera>().targetTexture = newRenderTexture;   // my changes
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
        AvatarRef.GetComponent<AvatarController>().SetAvatarClothDefault(AvatarRef, "Male");
    }

    public void SetMainScrolRefs()
    {
        if (MyProfileDataManager.Instance.gameObject.activeSelf)
        {
            mainscrollControllerRef.TopFixedObj = myProfileImitateTopPartButton;
            mainscrollControllerRef.headerObj = myProfileTopPartButton;
            mainscrollControllerRef.containerobj = myProfileUserPostPartObj.GetComponent<RectTransform>();
        }
        else
        {
            mainscrollControllerRef.TopFixedObj = OtherProfileImitateTopPartButton;
            mainscrollControllerRef.headerObj = OtherProfileTopPartButton;
            mainscrollControllerRef.containerobj = OtherProfileUserPostPartObj.GetComponent<RectTransform>();
        }
    }

    public void SwitchBetwenUserAndOtherProfileUI(bool _state)
    {
        if (_state)
        {
            myProfileTopPartButton.SetActive(_state);
            myProfileUserPostPartObj.SetActive(_state);
            myProfileImitateTopPartButton.SetActive(_state);
            OtherProfileTopPartButton.SetActive(!_state);
            OtherProfileUserPostPartObj.SetActive(!_state);
            OtherProfileImitateTopPartButton.SetActive(!_state);
        }
        else
        {
            myProfileTopPartButton.SetActive(_state);
            myProfileUserPostPartObj.SetActive(_state);
            myProfileImitateTopPartButton.SetActive(_state);
            OtherProfileTopPartButton.SetActive(!_state);
            OtherProfileUserPostPartObj.SetActive(!_state);
            OtherProfileImitateTopPartButton.SetActive(!_state);
        }
    }
}
