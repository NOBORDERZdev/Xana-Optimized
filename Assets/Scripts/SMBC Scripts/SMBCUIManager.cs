using Models;
using System.Collections;
using System.Globalization;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SMBCUIManager : MonoBehaviour
{

    public bool IsPotrait;
    TMP_FontAsset _defaultFont;

    private void OnEnable()
    {
        BuilderEventManager.OnNarrationCollisionEnter += EnableNarrationUI;
        BuilderEventManager.OnDoorKeyCollisionEnter += EnableDoorKeyUI;
        BuilderEventManager.OnSMBCQuizComponentCollisionEnter += EnableQuizComponentUI;
        BuilderEventManager.OnSMBCQuizComponentColse += ResetCredentials;
        SceneManager.sceneLoaded += ResetData;
    }

    private void OnDisable()
    {
        BuilderEventManager.OnNarrationCollisionEnter -= EnableNarrationUI;
        BuilderEventManager.OnDoorKeyCollisionEnter -= EnableDoorKeyUI;
        BuilderEventManager.OnSMBCQuizComponentCollisionEnter -= EnableQuizComponentUI;
        BuilderEventManager.OnQuizComponentColse -= ResetCredentials;
        SceneManager.sceneLoaded -= ResetData;
    }

    void ResetData(Scene scene, LoadSceneMode mode)
    {
        DisableUIObject();
        ResetCredentials();
    }


    #region Narration Component

    internal NarrationComponent NarrationComponent;
    public GameObject NarrationUIParent;
    public TextMeshProUGUI NarrationTextUI;
    public ScrollRect NarrationScroll;
    public GameObject SliderNarrationUI;
    public Button NarrationUIClosebtn;
    public Button NarrationUIDownTextbtn;

    float _letterDelay = 0.1f;
    float _narrationtotalHeight;
    float _singleLineHeight;
    int _storyCharCount = 0;
    bool _isAgainCollided;
    bool _isStoryWritten;
    Coroutine _storyNarrationCoroutine;

    void EnableNarrationUI(string narrationText, bool isStory, bool closeNarration)
    {
        DisableUIObject();
        _isStoryWritten = true;
        Invoke(nameof(NarrationUILinesCount), 0.01f);
        NarrationUIParent.SetActive(true);
        NarrationUIClosebtn.gameObject.SetActive(closeNarration);

        if (!isStory)
        {
            if (_storyNarrationCoroutine != null)
                StopCoroutine(_storyNarrationCoroutine);
            _isAgainCollided = true;
            //StartCoroutine(WaitDelayStatement());
            NarrationTextUI.text = narrationText;
            NarrationScroll.enabled = false;
            SliderNarrationUI.SetActive(false);
            _isStoryWritten = false;
            Invoke(nameof(NarrationUILinesCount), 0.1f);
        }
        else
        {
            _storyCharCount = 0;
            NarrationTextUI.text = "";
            if (_storyNarrationCoroutine == null)
                _storyNarrationCoroutine = StartCoroutine(StoryNarration(narrationText));
            else
            {
                StopCoroutine(_storyNarrationCoroutine);
                _storyNarrationCoroutine = StartCoroutine(StoryNarration(narrationText));
            }
        }

    }

    public void NarrationUILinesCount()
    {
        NarrationTextUI.rectTransform.parent.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);

        NarrationScroll.enabled = false;
        SliderNarrationUI.SetActive(false);

        _narrationtotalHeight = NarrationTextUI.rectTransform.rect.height;

        // Get the number of lines in the text.
        int numberOfLines = NarrationTextUI.textInfo.lineCount;
        // Calculate the single line height by dividing the total height by the number of lines.
        _singleLineHeight = _narrationtotalHeight / numberOfLines;

        NarrationUIDownTextbtn.interactable = !_isStoryWritten;
    }
    IEnumerator StoryNarration(string msg)
    {
        #region
        _isAgainCollided = true;
        yield return new WaitForSeconds(0.2f);
        _isAgainCollided = false;
        #endregion
        while (_storyCharCount < msg.Length && !_isAgainCollided)
        {
            NarrationTextUI.text += msg[_storyCharCount];
            if (_defaultFont)
                NarrationTextUI.font = _defaultFont;
            _storyCharCount++;

            yield return new WaitForSeconds(_letterDelay);
            //StartCoroutine(WaitForScrollingOption());
        }
        _isStoryWritten = false;
        NarrationUILinesCount();
    }
    IEnumerator WaitDelayStatement()
    {
        yield return new WaitForSeconds(0.2f);
        _isAgainCollided = false;
    }
    public void DisplayDownText()
    {
        if (NarrationScroll.content.anchoredPosition.y + _singleLineHeight * 4 <= _narrationtotalHeight)
        {
            NarrationScroll.content.anchoredPosition += new Vector2(0, _singleLineHeight);
        }
        else
        {
            DisableNarrationUI();
        }
    }
    void DisableNarrationUI()
    {
        NarrationUIParent.SetActive(false);
        NarrationTextUI.text = "";
        if (_storyNarrationCoroutine != null)
            StopCoroutine(_storyNarrationCoroutine);
    }
    #endregion


    #region Quiz Component

    public TextMeshProUGUI QuizButtonTextInformation;
    public TextMeshProUGUI NumberOfQuestions;
    public TextMeshProUGUI CorrectText;
    public TextMeshProUGUI WrongText;
    public TextMeshProUGUI ScorePercentage;

    public GameObject[] CorrectWrongImageObjects;
    public GameObject QuizParentReference;
    public GameObject ScoreCanvas;
    public GameObject QuizComponentUI;
    public Button[] Options = new Button[4];
    public Button NextButton;

    public Sprite WrongImage;
    public Sprite CorrectImage;
    public bool IsDissapearing = false;

    int _questionIndex;
    int _numOfQuestions;
    int _correct, _wrong;
    int _currentAnswer;
    readonly int _inputFieldsPerQuestion = 5; //one question and four options

    Outline _currentOutline;

    bool _isOptionSelected = false;
    bool _isFirstQuestion = true;

    QuizComponentData _quizComponentData = new();
    TextMeshProUGUI _nextButtonText;

    string _confirm = "Confirm";
    string _result = "Result";
    string _next = "Next";
    SMBCQuizComponent _quizComponent;

    void EnableQuizComponentUI(SMBCQuizComponent quizComponent, QuizComponentData quizComponentData)
    {
        _confirm = "Confirm";
        _result = "Result";
        _next = "Next";
        DisableUIObject();

        if (_questionIndex == _numOfQuestions - 1)
            _questionIndex = 0;

        QuizComponentUI.SetActive(true);
        this._quizComponent = quizComponent;
        StartQuiz(quizComponentData);
    }
    public void QuizResultPopupClose()
    {
        if (_quizComponent != null)
            CheckScorePercentage();
    }

    public void StartQuiz(QuizComponentData data)
    {

        for (int i = 0; i < Options.Length; i++)
        {
            Options[i].onClick.RemoveAllListeners();
        }

        NextButton.onClick.RemoveAllListeners();
        for (int i = 0; i < Options.Length; i++)
        {
            int c = i;
            Options[c].onClick.AddListener(delegate { OnSelectOption(c); });
            Options[c].GetComponent<Outline>().enabled = false;
            CorrectWrongImageObjects[c].SetActive(false);
        }

        NextButton.onClick.AddListener(delegate { DisplayNextQuestion(); });

        this._quizComponentData = null;
        QuizParentReference.SetActive(false);
        this._quizComponentData = data;

        if (_questionIndex == 0)
            SetInitialValues();

        if (ScoreCanvas.activeInHierarchy)
            ScoreCanvas.SetActive(false);

        DisplayNextQuestion();
        QuizParentReference.SetActive(true);
    }

    private void SetInitialValues()
    {
        _numOfQuestions = _quizComponentData.answers.Count;

        _questionIndex = 0;
        _correct = 0;
        _wrong = 0;

        _nextButtonText = NextButton.GetComponentInChildren<TextMeshProUGUI>();
        _confirm = TextLocalization.GetLocaliseTextByKey("Confirm");
        _nextButtonText.text = _confirm;
        if (_defaultFont)
            _nextButtonText.font = _defaultFont;
        _isFirstQuestion = true;
        _isOptionSelected = false;
    }

    private void CheckAnswer()
    {
        NextButton.interactable = false;
        if (_quizComponentData.answers[_questionIndex] == _currentAnswer)
        {
            UpdateQuizData(0);
        }
        else
        {
            _currentOutline.enabled = false;
            UpdateQuizData(1);
            Invoke(nameof(ResetCredentials), 2f);
            return;
        }

        _questionIndex += 1;
        _next = TextLocalization.GetLocaliseTextByKey("BuilderNext");
        _result = TextLocalization.GetLocaliseTextByKey("Result");
        //Debug.Log("TextLocalization==>" + next + " " + result);


        _nextButtonText.text = (_questionIndex < _numOfQuestions) ? _next : _result;
        if (_defaultFont)
            _nextButtonText.font = _defaultFont;

        new Delayed.Action(() =>
        {
            Debug.LogError("Action call ==>" + _questionIndex);
            if (_quizComponent.gameObject.name.Contains("Key"))
                SMBCManager.Instance.AddKey();

            if (_quizComponent.RequireCollectible == SMBCCollectibleType.DoorKey)
                SMBCManager.Instance.RemoveKey();
            _quizComponent.gameObject.SetActive(false);
            var quizData = SMBCManager.Instance.GetQuizData();
            Debug.Log(JsonUtility.ToJson(quizData));
            EnableNarrationUI(quizData.Explanation[_questionIndex], true, true);
            _nextButtonText.text = _confirm;
            ShowQuestion();
            NextButton.interactable = true;
        }, 0.5f);
    }


    void OnSelectOption(int answer)
    {
        _currentAnswer = answer;
        SetButtonInteractability(true, true);
        UpdateQuizData(2);

        if (!_isOptionSelected)
        {
            _isOptionSelected = true;
            _confirm = TextLocalization.GetLocaliseTextByKey("Confirm");
            //Debug.Log("TextLocalization==>" + confirm);

            _nextButtonText.text = _confirm;
            if (_defaultFont)
                _nextButtonText.font = _defaultFont;
        }
    }

    private void DisplayNextQuestion()
    {
        if (_nextButtonText.text == _confirm)
        {
            if (!_isOptionSelected)
            {
                if (_isFirstQuestion)
                    ShowQuestion();
                else
                    Debug.Log("Please Select an Option First");

                _isFirstQuestion = false;
            }
            else
            {
                CheckAnswer();
            }

            return;
        }

        if (_nextButtonText.text == _next)
        {
            ShowQuestion();
            return;
        }

        if (_nextButtonText.text == _result)
        {
            QuizParentReference.SetActive(false);
            ShowScoreCanvasRoutine();
        }
    }

    void ShowQuestion()
    {
        SetButtonInteractability(true, true);
        _confirm = TextLocalization.GetLocaliseTextByKey("Confirm");
        //Debug.Log("Confirm Localise " + confirm);

        _nextButtonText.text = _confirm;
        if (_defaultFont)
            _nextButtonText.font = _defaultFont;
        if (_questionIndex < _numOfQuestions)
        {
            string s = TextLocalization.GetLocaliseTextByKey("Question");
            string s2 = "/";// TextLocalization.GetLocaliseTextByKey("of");
            string s3 = TextLocalization.GetLocaliseTextByKey("Q");
            //Debug.Log("TextLocalization==>" + s + " " + s2 + " " + s3);

            //Debug.LogError("_questionIndex => " + _questionIndex);
            NumberOfQuestions.text = s + " " + (_questionIndex + 1) + " " + s2 + " " + _numOfQuestions;
            QuizButtonTextInformation.text = s3 + ": " + _quizComponentData.rewritingStringList[_questionIndex * _inputFieldsPerQuestion];

            if (_defaultFont)
            {
                NumberOfQuestions.font = _defaultFont;
                QuizButtonTextInformation.font = _defaultFont;
            }

            for (int i = 1; i < _inputFieldsPerQuestion; i++)
            {
                string sb = _quizComponentData.rewritingStringList[i + (_questionIndex * _inputFieldsPerQuestion)];
                Options[i - 1].GetComponentInChildren<TextMeshProUGUI>().text =
                    sb;

                if (_defaultFont)
                    Options[i - 1].GetComponentInChildren<TextMeshProUGUI>().font = _defaultFont;
                if (!IsPotrait)
                {
                    if (GameManager.currentLanguage == "ja" || LocalizationManager.forceJapanese || ContainsJapaneseText(sb))
                        Options[i - 1].GetComponentInChildren<TextMeshProUGUI>().fontSize = 11.3f;
                    else
                        Options[i - 1].GetComponentInChildren<TextMeshProUGUI>().fontSize = 12;
                }
            }
        }

        _isOptionSelected = false;
    }
    bool ContainsJapaneseText(string input)
    {
        foreach (char c in input)
        {
            // Check if the character belongs to the Unicode category of Japanese scripts
            UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(c);
            if (category == UnicodeCategory.OtherLetter || category == UnicodeCategory.LetterNumber)
            {
                return true;
            }
        }
        return false;
    }


    void UpdateQuizData(int option)
    {
        string colorString = "";
        Sprite image = CorrectImage;

        //0 if the answer is correct, 1 if wrong, 2 is for selecting only (answer not yet confirmed)
        switch (option)
        {
            case 0:
                _correct++;
                image = CorrectImage;
                ReferencesForGamePlay.instance.m_34player.GetComponent<SoundEffects>().PlaySoundEffects(SoundEffects.Sounds.QuizCorrect);

                colorString = "#36C34E";
                break;

            case 1:
                _wrong++;
                image = WrongImage;
                ReferencesForGamePlay.instance.m_34player.GetComponent<SoundEffects>().PlaySoundEffects(SoundEffects.Sounds.QuizWrong);

                break;

            case 2:
                colorString = "#008FFF";
                break;
        }

        if (option == 0 || option == 1)
        {
            CorrectWrongImageObjects[_currentAnswer].SetActive(true);
        }

        CorrectWrongImageObjects[_currentAnswer].GetComponent<Image>().sprite = image;

        Outline thisButtonOutine = Options[_currentAnswer].GetComponent<Outline>();

        if (ColorUtility.TryParseHtmlString(colorString, out Color color))
        {
            thisButtonOutine.effectColor = color;
        }

        if (option == 0 || option == 2)
        {
            thisButtonOutine.enabled = true;
            _currentOutline = thisButtonOutine;
        }

        if (option == 1)
        {
            WrongAnswerUIAdjustments(CorrectImage, "#36C34E");
        }
    }

    private void WrongAnswerUIAdjustments(Sprite image, string colorString)
    {
        CorrectWrongImageObjects[_quizComponentData.answers[_questionIndex]].SetActive(true);
        CorrectWrongImageObjects[_quizComponentData.answers[_questionIndex]].GetComponent<Image>().sprite = image;

        Outline thisButtonOutine = Options[_quizComponentData.answers[_questionIndex]].GetComponent<Outline>();

        if (ColorUtility.TryParseHtmlString(colorString, out Color color))
        {
            thisButtonOutine.effectColor = color;
        }

        thisButtonOutine.enabled = true;
        _currentOutline = thisButtonOutine;

    }

    private void SetButtonInteractability(bool isOptions, bool isInteractable)
    {
        if (isOptions)
        {
            for (int i = 0; i < Options.Length; i++)
            {
                Options[i].transition = Selectable.Transition.None;
                Options[i].interactable = isInteractable;
                if (isInteractable)
                {
                    Options[i].GetComponent<Outline>().enabled = false;
                    CorrectWrongImageObjects[i].SetActive(false);
                }
            }
        }
    }

    private void ShowScoreCanvasRoutine()
    {
        ScoreCanvas.SetActive(true);
        string s = TextLocalization.GetLocaliseTextByKey("Correct");
        string s2 = TextLocalization.GetLocaliseTextByKey("Wrong");
        string s3 = TextLocalization.GetLocaliseTextByKey("Correct Answer is");
        string s4 = TextLocalization.GetLocaliseTextByKey("Confirm");
        //Debug.Log("TextLocalization==>" + s + " " + s2 + " " + s3 + " " + s4);
        CorrectText.text = s + ": " + _correct;
        WrongText.text = s2 + ": " + _wrong;
        float percentage = (((float)_correct / _numOfQuestions) * 100);
        ScorePercentage.text = s3 + " " + percentage.ToString("0.#") + "%";

        _isFirstQuestion = true;
        _isOptionSelected = false;

        _nextButtonText.text = s4;

        if (_defaultFont)
        {
            CorrectText.font = _defaultFont;
            WrongText.font = _defaultFont;
            ScorePercentage.font = _defaultFont;
            _nextButtonText.font = _defaultFont;
        }
    }

    public void CheckScorePercentage()
    {
        StartCoroutine(CheckScorePercentageRoutine());
    }

    IEnumerator CheckScorePercentageRoutine()
    {
        float division = (((float)_correct / _numOfQuestions) * 100);

        if (division >= _quizComponentData.correctAnswerRate)
        {
            IsDissapearing = true;
            yield return new WaitForSeconds(0);
            IsDissapearing = false;
            _quizComponent.gameObject.SetActive(false);
        }
        _quizComponent = null;
        ResetCredentials();
    }

    void ResetCredentials()
    {
        _correct = 0;
        _wrong = 0;
        _questionIndex = 0;
        DisableQuizComponentUI();
    }

    internal void DisableQuizComponentUI()
    {
        QuizComponentUI.SetActive(false);
        _quizComponent = null;
    }
    #endregion

    #region Doorkey component
    public GameObject DoorKeyParentUI;
    public TextMeshProUGUI DoorKeyText;

    Coroutine _EnableDoorKeyCoroutine;
    public void EnableDoorKeyUI(string DisplayMessage)
    {
        DisableUIObject();
        if (_EnableDoorKeyCoroutine == null)
        {
            _EnableDoorKeyCoroutine = StartCoroutine(IEEnableDoorKeyUI(DisplayMessage));
        }
        else
        {
            StopCoroutine(_EnableDoorKeyCoroutine);
            _EnableDoorKeyCoroutine = StartCoroutine(IEEnableDoorKeyUI(DisplayMessage));
        }
    }
    public IEnumerator IEEnableDoorKeyUI(string DisplayMessage)
    {
        DisplayMessage = TextLocalization.GetLocaliseTextByKey(DisplayMessage);
        DoorKeyText.text = DisplayMessage;
        bool isJPText = CheckJapaneseDisplayMessage(DisplayMessage);
        //Debug.LogError(isJPText);
        DoorKeyParentUI.SetActive(true);

        float time = 5f;
        while (time > 0)
        {
            yield return new WaitForSeconds(1f);
            time--;
        }
        DoorKeyParentUI.SetActive(false);
    }

    public void DisableDoorKeyUI()
    {
        if (_EnableDoorKeyCoroutine != null)
            StopCoroutine(_EnableDoorKeyCoroutine);
        DoorKeyParentUI.SetActive(false);
        DoorKeyText.text = "";
    }
    #endregion

    bool CheckJapaneseDisplayMessage(string displayTitle)
    {
        Regex regex = new Regex(@"\p{IsHiragana}|\p{IsKatakana}|\p{IsCJKUnifiedIdeographs}");
        return regex.IsMatch(displayTitle);
    }

    void DisableUIObject()
    {
        DisableNarrationUI();
        DisableQuizComponentUI();
        DisableDoorKeyUI();
    }
}
