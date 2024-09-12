using DG.Tweening;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SMBCManager : MonoBehaviour
{
    public static SMBCManager Instance;

    //Orientation Changer
    public CanvasGroup LandscapeCanvas;
    public CanvasGroup PotraitCanvas;
    public string CurrentWorldName;
    public string ParentWorldName;
    public QuizDataLoader QuizDataLoader;
    public PlayerCanvas PlayerCanvas;

    //Name canvas
    internal Canvas NameCanvas;

    PlayerCanvas _keyObject;
    bool _isPotrait = false;
    bool _requireKeyCollected = false;
    bool _requireAxeCollected = false;
    QuizData _quizData;
    int _keyCounter = 0;

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
        BuilderEventManager.BuilderSceneOrientationChange += OrientationChange;
        OrientationChange(false);
    }

    private void OnDisable()
    {
        BuilderEventManager.BuilderSceneOrientationChange -= OrientationChange;
    }

    #region OrientationChange
    void OrientationChange(bool orientation)
    {
        StartCoroutine(ChangeOrientation(orientation));
    }

    IEnumerator ChangeOrientation(bool orientation)
    {
        _isPotrait = orientation;
        DisableUICanvas();
        yield return new WaitForSeconds(0.1f);
        if (_isPotrait)
        {
            PotraitCanvas.DOFade(1, 0.5f);
            PotraitCanvas.blocksRaycasts = true;
            PotraitCanvas.interactable = true;
        }
        else
        {
            LandscapeCanvas.DOFade(1, 0.5f);
            LandscapeCanvas.blocksRaycasts = true;
            LandscapeCanvas.interactable = true;
        }

        yield return new WaitForSeconds(0.5f);

        BuilderEventManager.PositionUpdateOnOrientationChange?.Invoke();
    }

    void UICanvasToggle(bool state)
    {
        if (state)
        {
            DisableUICanvas();
        }
        else
            StartCoroutine(ChangeOrientation(_isPotrait));
    }

    void DisableUICanvas()
    {
        LandscapeCanvas.DOKill();
        LandscapeCanvas.alpha = 0;
        LandscapeCanvas.blocksRaycasts = false;
        LandscapeCanvas.interactable = false;
        PotraitCanvas.DOKill();
        PotraitCanvas.alpha = 0;
        PotraitCanvas.blocksRaycasts = false;
        PotraitCanvas.interactable = false;
    }
    #endregion

    public void InitQuizComponent(SMBCQuizComponent quizComponent)
    {
        if (_quizData == null)
            WorldLoaded();
        quizComponent.Init(_quizData.WorldQuizComponentData);
    }

    void WorldLoaded()
    {
        CurrentWorldName = CurrentWorldName.Replace(" ", "_");
        _quizData = QuizDataLoader.GetQuizData(CurrentWorldName);
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
        if (_keyCounter > 1)
            _keyCounter--;
        AddKeyObjectOnPlayerHead();
    }

    public void AddRocketPart()
    {
        //_keyCounter++;
    }

    public void AddAxe()
    {
        _requireAxeCollected = true;
    }

    void AddKeyObjectOnPlayerHead()
    {
        if (_keyObject == null)
            _keyObject = Instantiate(PlayerCanvas);

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
            case SMBCCollectibleType.RocketPart:
                return _requireAxeCollected;
            case SMBCCollectibleType.None:
                return true;
            default:
                return false;
        }
    }
}

public enum SMBCCollectibleType
{
    DoorKey,
    Axe,
    RocketPart,
    None
}