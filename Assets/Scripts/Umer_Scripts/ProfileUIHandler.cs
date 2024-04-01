using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using static StoreManager;

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

        //if ((GameManager.Instance.avatarGender==AvatarGender.Male && !maleAvatarRef) ||
        //    (GameManager.Instance.avatarGender == AvatarGender.Female && !femaleAvatarRef))
        //{
        //    InstantiateUserPreviewAvatar(GameManager.Instance.avatarGender);
        //}
        
        //else
        //{
        //    if (GameManager.Instance.avatarGender==AvatarGender.Male)
        //    {
        //        maleAvatarRef.SetActive(true);
        //        avatarRef = maleAvatarRef.transform.GetChild(0).gameObject;
        //    }
        //    else
        //    {
        //        femaleAvatarRef.SetActive(true);
        //        avatarRef = femaleAvatarRef.transform.GetChild(0).gameObject;
        //    }
        //}
    }

    private void OnDisable()
    {
        _renderTexCamera.GetComponent<Camera>().targetTexture = null;
        _renderTexCamera.gameObject.SetActive(false);
        Object.Destroy(newRenderTexture);

        //newRenderTexture.Release();
        menuLightingObj.SetActive(true);
        lightingObj.SetActive(false);
        if (maleAvatarRef)
            maleAvatarRef.SetActive(false);
        if(femaleAvatarRef)
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
                avatarRef = maleAvatarRef.transform.GetChild(0).gameObject;
                break;
            case "Female":
                if (femaleAvatarRef)
                    femaleAvatarRef.SetActive(true);
                else
                    InstantiateUserPreviewAvatar(AvatarGender.Female);

                if (maleAvatarRef)
                    maleAvatarRef.SetActive(false);
                avatarRef = femaleAvatarRef.transform.GetChild(0).gameObject;
                break;
        }
        menuLightingObj.SetActive(false);
        lightingObj.SetActive(true);
    }
    public void InstantiateUserPreviewAvatar(AvatarGender gender)
    {
        ref_FriendHomeManager = GameManager.Instance.FriendsHomeManager.GetComponent<FriendHomeManager>();
        _renderTexCamera.parent = null;
        //_renderTexCamera.position = new Vector3(0f, 0.8f, -6f);
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

        avatarRef = avatarRef.GetComponent<CharacterHandler>().avatarData.avatar_parent;
        avatarRef.name = "UserPreviewAvatar";
        //AvatarRef = Instantiate(GameManager.Instance.FriendsHomeManager.GetComponent<FriendHomeManager>().FriendAvatarPrefab.gameObject);
        avatarRef.GetComponent<FootStaticIK>().ikActive = true;
        avatarRef.transform.position = new Vector3(5000f, 0.069f, 0f);
        avatarRef.GetComponent<Animator>().runtimeAnimatorController = _userIdleAnimator.runtimeAnimatorController;
        Destroy(avatarRef.GetComponent<CharacterOnScreenNameHandler>());
        Destroy(avatarRef.GetComponent<Actor>());
        Destroy(avatarRef.GetComponent<PlayerPostBubbleHandler>());

        avatarRef.transform.parent.gameObject.SetActive(true);

        GameObject temp = Instantiate(avatarBgObject, avatarRef.transform);
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
        ActivateProfileAvatarByGender(_userAvatarData.gender);
        if (avatarRef)
        {
            _tempAvatarData = _userAvatarData;
            
            avatarRef.GetComponent<AvatarController>().InitializeFrndAvatar(_userAvatarData,avatarRef);
        }
    }

    public void SetUserAvatarDefaultClothing()
    {
        int _rand = Random.Range(0, 7);
        //if (GameManager.Instance.avatarGender == AvatarGender.Male)
        //{
        //    if (maleAvatarRef)
        //        maleAvatarRef.SetActive(true);
        //    if (femaleAvatarRef)
        //        femaleAvatarRef.SetActive(false);
        //    avatarRef = maleAvatarRef.transform.GetChild(0).gameObject;
        //}
        //else
        //{
        //    if (maleAvatarRef)
        //        maleAvatarRef.SetActive(false);
        //    if (femaleAvatarRef)
        //        femaleAvatarRef.SetActive(true);
        //    avatarRef = femaleAvatarRef.transform.GetChild(0).gameObject;
        //}
        avatarRef.GetComponent<AvatarController>().DownloadRandomFrndPresets(_rand);
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
