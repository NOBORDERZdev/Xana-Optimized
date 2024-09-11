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


    List<QuizComponent> quizComponents = new List<QuizComponent>();
    bool _isPotrait = false;
    QuizData quizData;
    int _keyCounter = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        BuilderEventManager.BuilderSceneOrientationChange += OrientationChange;
        //BuilderEventManager.AfterPlayerInstantiated += WorldLoaded;
        OrientationChange(false);
    }

    private void OnDisable()
    {
        quizComponents.Clear();
        BuilderEventManager.BuilderSceneOrientationChange -= OrientationChange;
        //BuilderEventManager.AfterPlayerInstantiated -= WorldLoaded;
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

    public void InitQuizComponent(QuizComponent quizComponent)
    {
        if (quizData == null)
            WorldLoaded();
        quizComponent.Init(quizData.WorldQuizComponentData);
    }

    void WorldLoaded()
    {
        CurrentWorldName = CurrentWorldName.Replace(" ", "_");
        quizData = QuizDataLoader.GetQuizData(CurrentWorldName);
    }

    public void AddKey()
    {
        _keyCounter++;
    }

}
