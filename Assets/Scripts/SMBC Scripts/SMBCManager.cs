using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SMBCManager : MonoBehaviour
{
    public static SMBCManager Instance;

    //Orientation Changer
    //public CanvasGroup LandscapeCanvas;
    //public CanvasGroup PotraitCanvas;
    [HideInInspector]
    public string CurrentPlanetName;
    public QuizDataLoader QuizDataLoader;
    public PlayerCanvas PlayerCanvas;

    //Name canvas
    internal Canvas NameCanvas;

    PlayerCanvas _keyObject;
    bool _isPotrait = false;
    bool _requireKeyCollected = false;
    bool _requireAxeCollected = false;
    bool _forestRocketPartCollected = false;
    bool _icyRocketPartCollected = false;
    bool _volcanicRocketPartCollected = false;
    QuizData _quizData;
    int _keyCounter = 0;
    List<string> _npcText;

    public static Action OnIntroductryPanelClicked;
    public static Action AssignPlayerCanvasRef;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        //BuilderEventManager.BuilderSceneOrientationChange += OrientationChange;
        BuilderEventManager.OnSMBCRocketCollected += BackToEarthWithDelay;
        BuilderEventManager.OnSMBCQuizWrongAnswer += BackToEarthWithDelay;
        SceneManager.sceneLoaded += ResetData;
        //OrientationChange(false);
    }

    private void OnDisable()
    {
        //BuilderEventManager.BuilderSceneOrientationChange -= OrientationChange;
        BuilderEventManager.OnSMBCRocketCollected -= BackToEarthWithDelay;
        BuilderEventManager.OnSMBCQuizWrongAnswer -= BackToEarthWithDelay;
        SceneManager.sceneLoaded -= ResetData;


    }

    #region OrientationChange
    //void OrientationChange(bool orientation)
    //{
    //    StartCoroutine(ChangeOrientation(orientation));
    //}

    //IEnumerator ChangeOrientation(bool orientation)
    //{
    //    _isPotrait = orientation;
    //    DisableUICanvas();
    //    yield return new WaitForSeconds(0.1f);
    //    if (_isPotrait)
    //    {
    //        PotraitCanvas.DOFade(1, 0.5f);
    //        PotraitCanvas.blocksRaycasts = true;
    //        PotraitCanvas.interactable = true;
    //    }
    //    else
    //    {
    //        LandscapeCanvas.DOFade(1, 0.5f);
    //        LandscapeCanvas.blocksRaycasts = true;
    //        LandscapeCanvas.interactable = true;
    //    }

    //    yield return new WaitForSeconds(0.5f);

    //    BuilderEventManager.PositionUpdateOnOrientationChange?.Invoke();
    //}

    //void UICanvasToggle(bool state)
    //{
    //    if (state)
    //    {
    //        DisableUICanvas();
    //    }
    //    else
    //        StartCoroutine(ChangeOrientation(_isPotrait));
    //}

    //void DisableUICanvas()
    //{
    //    LandscapeCanvas.DOKill();
    //    LandscapeCanvas.alpha = 0;
    //    LandscapeCanvas.blocksRaycasts = false;
    //    LandscapeCanvas.interactable = false;
    //    PotraitCanvas.DOKill();
    //    PotraitCanvas.alpha = 0;
    //    PotraitCanvas.blocksRaycasts = false;
    //    PotraitCanvas.interactable = false;
    //}
    #endregion

    void ResetData(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "XANA Summit" || scene.name == "Home")
        {
            Destroy(gameObject);
        }
        _quizData = null;
        _npcText = null;
        _keyCounter = 0;
    }
    public void InitQuizComponent(SMBCQuizComponent quizComponent)
    {
        if (_quizData == null)
            WorldLoaded();
        quizComponent.Init(_quizData.WorldQuizComponentData);
    }

    public void InitNPCText(SMBCNPC npcs)
    {
        if (_npcText == null)
            NPCTextLoad(npcs.PlanetName);
        npcs.Init(_npcText);
    }

    void WorldLoaded()
    {
        CurrentPlanetName = CurrentPlanetName.Replace(" ", "_");
        _quizData = QuizDataLoader.GetQuizData(CurrentPlanetName);
    }

    void NPCTextLoad(string planetName)
    {
        switch (planetName)
        {
            case "Earth_Planet":
                _npcText = QuizDataLoader.EarthPlanetNPC;
                break;
            case "REDMoon_Planet":
                _npcText = QuizDataLoader.RedMoonPlanetNPC;
                break;
            default:
                _npcText = null;
                break;
        }
    }
    public void AddKey()
    {
        _keyCounter++;
        AddKeyObjectOnPlayerHead();
        if (_keyCounter >= 5)
            _requireKeyCollected = true;
    }

    public void RemoveKey()
    {
        if (_keyCounter > 0)
            _keyCounter--;
        AddKeyObjectOnPlayerHead();
    }

    public void AddRocketPart()
    {
        Debug.LogError("Add rocket part => " + CurrentPlanetName);
        switch (CurrentPlanetName)
        {
            case "SMBC_Forest_Planet":
                _forestRocketPartCollected = true;
                break;
            case "SMBC_Icy_Planet":
                _icyRocketPartCollected = true;
                break;
            case "SMBC_Volcanic_Planet":
                _volcanicRocketPartCollected = true;
                break;
            default:
                _forestRocketPartCollected = false;
                _icyRocketPartCollected = false;
                _volcanicRocketPartCollected = false;
                break;
        }
    }



    public void AddAxe()
    {
        _requireAxeCollected = true;
    }

    void AddKeyObjectOnPlayerHead()
    {
        if (_keyObject == null)
        {
            if (PlayerCanvas == null)
            {
                AssignPlayerCanvasRef?.Invoke();
            }
            _keyObject = Instantiate(PlayerCanvas);
        }


        if (_keyObject.transform.parent != NameCanvas)
        {
            _keyObject.transform.SetParent(NameCanvas.transform);
            _keyObject.transform.localPosition = Vector3.up * 18.5f;
        }

        _keyObject.ToggleKey(true);
        _keyObject.cameraMain = ReferencesForGamePlay.instance.playerControllerNew.ActiveCamera.transform;
        _keyObject.keyCounter.text = "x" + _keyCounter;

        if (_keyCounter == 0)
            Destroy(_keyObject);
    }

    public QuizData GetQuizData()
    {
        return _quizData;
    }

    public bool CheckForObjectCollectible(SMBCCollectibleType collectibleType)
    {
        switch (collectibleType)
        {
            case SMBCCollectibleType.DoorKey:
                return _requireKeyCollected;
            case SMBCCollectibleType.Axe:
                return _requireAxeCollected;
            case SMBCCollectibleType.None:
                return true;
            default:
                return false;
        }
    }
    private void BackToEarthWithDelay()
    {
        Invoke(nameof(BackToEarth), 3f);
    }

    private void BackToEarth()
    {
        GamePlayButtonEvents.OnExitButtonXANASummit?.Invoke();
    }

    public bool CheckRocketPartCollectedOrNot(string planetName)
    {
        switch (planetName)
        {
            case "SMBC_Forest_Planet":
                return _forestRocketPartCollected;
            case "SMBC_Icy_Planet":
                return _icyRocketPartCollected;
            case "SMBC_Volcanic_Planet":
                return _volcanicRocketPartCollected;
            default:
                return false;
        }
    }

    public bool CheckAllRocketPartIsCollected()
    {
        if (_forestRocketPartCollected && _icyRocketPartCollected && _volcanicRocketPartCollected)
            return true;
        return false;
    }
}

public enum SMBCCollectibleType
{
    DoorKey,
    Axe,
    RocketPart,
    None
}
