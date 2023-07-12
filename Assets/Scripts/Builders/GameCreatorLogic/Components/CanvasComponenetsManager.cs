using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CanvasComponenetsManager : MonoBehaviour
{
    public static CanvasComponenetsManager _instance;
    [HideInInspector] public NarrationComponent narrationComponent;
    [HideInInspector] public DisplayMessagesComponent displayMessagesComponent;
    //[HideInInspector] public QuizComponent quizComponent;
    public bool stoppingCoroutine;
    private void Awake()
    {
        _instance = this;
        stoppingCoroutine = false;
    }

    public void OnDisplayMessagesCanvasClosed()
    {
        displayMessageParentReference.SetActive(false);
        this.displayMessagesComponent.isCoroutineRunning = false;
        this.displayMessagesComponent = null;
    }


    #region Help Canvas References
    public GameObject helpParentReference;
    public TMP_Text helpButtonTitle;
    public TMP_Text helpButtonTextInformation;
    #endregion


    #region Display Messages
    public GameObject displayMessageParentReference;
    public TMP_Text displayMessageTitle;
    public TMP_Text timeLeft;
    #endregion

    #region Narration Canvas References
    public GameObject narrationParentReference;
    public TMP_Text narrationTextInformation;
    #endregion

    #region Quiz
    public GameObject quizParentReference;
    public TMP_Text quizButtonTextInformation;
    public TMP_Text numberOfQuestions;
    public Button[] options = new Button[4];
    public GameObject scoreCanvas;
    public TMP_Text correctText;
    public TMP_Text wrongText;

    #endregion


}
