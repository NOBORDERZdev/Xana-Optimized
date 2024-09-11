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

    bool _isPotrait = false;
    bool _requireKeyCollected = false;
    bool _requireAxeCollected = false;
    QuizData _quizData;
    int _keyCounter = 0;

    private void Awake()
    {
        Instance = this;
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
        if (_keyCounter >= 5)
            _requireKeyCollected = true;
    }

    public void RemoveKey()
    {
        if (_keyCounter > 1)
            _keyCounter--;
    }

    public void AddRocketPart()
    {
        //_keyCounter++;
    }

    public void AddAxe()
    {
        _requireAxeCollected = true;
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
            default:
                return false;
        }
    }
}

public enum SMBCCollectibleType
{
    DoorKey,
    Axe,
    RocketPart
}