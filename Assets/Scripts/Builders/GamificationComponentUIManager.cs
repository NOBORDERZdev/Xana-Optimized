using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using Models;
using System.Globalization;

public class GamificationComponentUIManager : MonoBehaviour
{
    private void OnEnable()
    {
        //subscribe Narration event
        Debug.Log("Subscribe Event");
        BuilderEventManager.OnNarrationCollisionEnter += EnableNarrationUI;
        BuilderEventManager.OnNarrationCollisionExit += DisableNarrationUI;
        BuilderEventManager.OnRandomCollisionEnter += EnableRandomNumberUI;
        BuilderEventManager.OnRandomCollisionExit += DisableRandomNumberUI;
        BuilderEventManager.OnTimerLimitTriggerEnter += EnableTimeLimitUI;
        BuilderEventManager.OnTimerTriggerEnter += EnableTimeLimitUI;
        BuilderEventManager.OnTimerCountDownTriggerEnter += EnableTimerCountDownUI;
        BuilderEventManager.OnElapseTimeCounterTriggerEnter += EnableElapseTimeCounDownUI;
        BuilderEventManager.OnDisplayMessageCollisionEnter += EnableDisplayMessageUI;
        BuilderEventManager.OnHelpButtonCollisionEnter += EnableHelpButtonUI;
        BuilderEventManager.OnHelpButtonCollisionExit += DisableHelpButtonUI;
        BuilderEventManager.OnSituationChangerTriggerEnter += EnableSituationChangerUI;
        BuilderEventManager.OnBlindComponentTriggerEnter += EnableBlindComponentUI;
        BuilderEventManager.OnQuizComponentCollisionEnter += EnableQuizComponentUI;
        BuilderEventManager.OnSpecialItemComponentCollisionEnter += EnableSpecialItemUI;
        BuilderEventManager.OnAvatarInvisibilityComponentCollisionEnter += EnableAvatarInvisibilityUI;
        BuilderEventManager.OnNinjaMotionComponentCollisionEnter += EnableNinjaMotionUI;
        BuilderEventManager.OnThrowThingsComponentCollisionEnter += EnableThrowThingsUI;
        BuilderEventManager.OnHyperLinkPopupCollisionEnter += EnableHyperLinkPopupUI;
        BuilderEventManager.OnHyperLinkPopupCollisionExit += DisableHyperLinkPopupUI;

        BuilderEventManager.ResetComponentUI += DisableAllComponentUIObject;

        BuilderEventManager.ChangeNinja_ThrowUIPosition += ChangeNinja_ThrowUIPosition;
        BuilderEventManager.PositionUpdateOnOrientationChange += PositionUpdateOnOrientationChange;


        DisableThrowThingUI();
        DisableAllComponentUIObject(ComponentType.None);
    }
    private void OnDisable()
    {
        //unsubscribe Narration event
        Debug.Log("UnSubscribe Event");
        BuilderEventManager.OnNarrationCollisionEnter -= EnableNarrationUI;
        BuilderEventManager.OnNarrationCollisionExit -= DisableNarrationUI;
        BuilderEventManager.OnRandomCollisionEnter -= EnableRandomNumberUI;
        BuilderEventManager.OnRandomCollisionExit -= DisableRandomNumberUI;
        BuilderEventManager.OnTimerLimitTriggerEnter -= EnableTimeLimitUI;
        BuilderEventManager.OnTimerTriggerEnter -= EnableTimeLimitUI;
        BuilderEventManager.OnTimerCountDownTriggerEnter -= EnableTimerCountDownUI;
        BuilderEventManager.OnElapseTimeCounterTriggerEnter -= EnableElapseTimeCounDownUI;
        BuilderEventManager.OnDisplayMessageCollisionEnter -= EnableDisplayMessageUI;
        BuilderEventManager.OnHelpButtonCollisionEnter -= EnableHelpButtonUI;
        BuilderEventManager.OnHelpButtonCollisionExit -= DisableHelpButtonUI;
        BuilderEventManager.OnSituationChangerTriggerEnter -= EnableSituationChangerUI;
        BuilderEventManager.OnBlindComponentTriggerEnter -= EnableBlindComponentUI;
        BuilderEventManager.OnQuizComponentCollisionEnter -= EnableQuizComponentUI;
        BuilderEventManager.OnSpecialItemComponentCollisionEnter -= EnableSpecialItemUI;
        BuilderEventManager.OnAvatarInvisibilityComponentCollisionEnter -= EnableAvatarInvisibilityUI;
        BuilderEventManager.OnNinjaMotionComponentCollisionEnter -= EnableNinjaMotionUI;
        BuilderEventManager.OnThrowThingsComponentCollisionEnter -= EnableThrowThingsUI;

        BuilderEventManager.OnHyperLinkPopupCollisionEnter -= EnableHyperLinkPopupUI;
        BuilderEventManager.OnHyperLinkPopupCollisionExit -= DisableHyperLinkPopupUI;
        BuilderEventManager.ChangeNinja_ThrowUIPosition -= ChangeNinja_ThrowUIPosition;
        BuilderEventManager.PositionUpdateOnOrientationChange -= PositionUpdateOnOrientationChange;


        BuilderEventManager.ResetComponentUI -= DisableAllComponentUIObject;
    }

    public bool isPotrait;
    private void PositionUpdateOnOrientationChange()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        NinjaMotionUIButtonPanel2.transform.position = new Vector3(screenWidth, 0, 0);
        ThowThingsUIButtonPanel2.transform.position = new Vector3(screenWidth, 0, 0);
        NinjaMotionUIButtonPanel.transform.position = new Vector3(screenWidth, 0, 0);
        ThowThingsUIButtonPanel.transform.position = new Vector3(screenWidth, 0, 0);

        if (isPotrait)
            GamificationComponentData.instance.Ninja_Throw_InitPosY = NinjaMotionUIButtonPanel2.transform.localPosition;
        else
            GamificationComponentData.instance.Ninja_Throw_InitPosX = NinjaMotionUIButtonPanel.transform.localPosition;
    }

    //Narration Comopnent 
    public GameObject narrationUIParent;
    public TextMeshProUGUI narrationTextUI;
    float letterDelay = 0.1f;
    int storyCharCount = 0;
    bool isAgainCollided;
    public ScrollRect narrationScroll;
    public GameObject sliderNarrationUI;
    Coroutine StoryNarrationCoroutine;

    //Random Number Component
    public GameObject RandomNumberUIParent;
    public TextMeshProUGUI RandomNumberText;

    //Timer Limit Component
    public GameObject TimeLimitUIParent;
    public TextMeshProUGUI TimeLimitText;
    public GameObject TimeLimitPrefab;
    public Transform TimeLimitParent;

    //Elapse Time Component
    public GameObject ElapseTimeUIParent;
    public TextMeshProUGUI ElapseTimerText;

    //Countdown Component
    public GameObject TimerCountDownUIParent;
    public TextMeshProUGUI TimerCountDownText;

    //Display Messages Component
    public GameObject DisplayMessageParentUI;
    public TextMeshProUGUI DisplayMessageText;
    public TextMeshProUGUI DisplayMessageTimeText;

    //Help Button Component
    public GameObject HelpButtonParentUI;
    public HelpButtonComponentResizer helpButtonComponentResizer;
    public TextMeshProUGUI HelpButtonTitleText;
    public TextMeshProUGUI HelpText;
    public ScrollRect helpButtonScroll;
    public GameObject sliderHelpButtonUI;

    //Situation Changer Component
    public GameObject SituationChangerParentUI;
    public TextMeshProUGUI SituationChangerTimeText;

    //Blind Component
    public GameObject BlindComponentParentUI;
    public TextMeshProUGUI BlindComponentTimeText;

    //Narration Component
    void EnableNarrationUI(string narrationText, bool isStory)
    {
        DisableAllComponentUIObject(ComponentType.Narration);
        narrationUIParent.SetActive(true);
        if (!isStory)
        {
            if (StoryNarrationCoroutine != null)
                StopCoroutine(StoryNarrationCoroutine);
            isAgainCollided = true;
            StartCoroutine(WaitDelayStatement());
            narrationTextUI.text = narrationText;
            narrationScroll.enabled = true;
            sliderNarrationUI.SetActive(true);
        }
        else
        {
            storyCharCount = 0;
            narrationTextUI.text = "";
            if (StoryNarrationCoroutine == null)
                StoryNarrationCoroutine = StartCoroutine(StoryNarration(narrationText));
            else
            {
                StopCoroutine(StoryNarrationCoroutine);
                StoryNarrationCoroutine = StartCoroutine(StoryNarration(narrationText));
            }
        }
    }

    public void NarrationUILinesCount()
    {
        if (narrationTextUI.textInfo.lineCount > 4)
        {
            narrationScroll.enabled = true;
            sliderNarrationUI.SetActive(true);
        }
        else
        {
            narrationScroll.enabled = false;
            sliderNarrationUI.SetActive(false);
        }
    }
    IEnumerator StoryNarration(string msg)
    {
        #region
        isAgainCollided = true;
        yield return new WaitForSeconds(0.2f);
        isAgainCollided = false;
        #endregion
        while (storyCharCount < msg.Length && !isAgainCollided)
        {
            narrationTextUI.text += msg[storyCharCount];
            storyCharCount++;

            yield return new WaitForSeconds(letterDelay);
            StartCoroutine(WaitForScrollingOption());
        }
    }
    IEnumerator WaitForScrollingOption()
    {
        yield return new WaitForEndOfFrame();
        NarrationUILinesCount();
    }
    IEnumerator WaitDelayStatement()
    {
        yield return new WaitForSeconds(0.2f);
        isAgainCollided = false;
    }
    void DisableNarrationUI()
    {
        narrationUIParent.SetActive(false);
        narrationTextUI.text = "";
        if (StoryNarrationCoroutine != null)
            StopCoroutine(StoryNarrationCoroutine);
    }

    public void EnableRandomNumberUI(float r)
    {
        DisableAllComponentUIObject(ComponentType.RandomNumberGenerator);
        RandomNumberUIParent.SetActive(true);
        string s = TextLocalization.GetLocaliseTextByKey("Generated Number On This");
        RandomNumberText.text = s + " : " + r.ToString();
    }
    public void DisableRandomNumberUI()
    {
        RandomNumberUIParent.SetActive(false);
        RandomNumberText.text = "";
    }

    //Time Limit Component
    Coroutine TimeCoroutine;
    public void EnableTimeLimitUI(string purpose, float time)
    {
        DisableAllComponentUIObject(ComponentType.TimeLimit);
        TimeLimitUIParent.SetActive(true);
        if (TimeCoroutine == null)
        {
            TimeCoroutine = StartCoroutine(nameof(IETimeLimit), time);
        }
        else
        {
            if (TimeCoroutine != null)
                StopCoroutine(TimeCoroutine);
            TimeCoroutine = StartCoroutine(nameof(IETimeLimit), time);
        }
    }

    public IEnumerator IETimeLimit(float time)
    {
        while (time > 0)
        {
            time--;
            TimeLimitText.text = ConvertTimetoSecondsandMinute(time);
            yield return new WaitForSeconds(1);
        }
        yield return new WaitForSeconds(5f);
        DisableTimeLimitUI();
    }

    public void DisableTimeLimitUI()
    {
        TimeLimitUIParent.SetActive(false);
        TimeLimitText.text = "";
        if (TimeCoroutine != null)
            StopCoroutine(TimeCoroutine);
    }

    public Coroutine TimerCountdownCoroutine;
    public void EnableTimerCountDownUI(int time, bool isRunning)
    {
        DisableAllComponentUIObject(ComponentType.TimerCountDown);
        if (isRunning)
        {
            if (TimerCountdownCoroutine == null)
                TimerCountdownCoroutine = StartCoroutine(IETimerCountDown(time, isRunning));
            else
            {
                if (TimerCountdownCoroutine != null)
                    StopCoroutine(TimerCountdownCoroutine);
                TimerCountdownCoroutine = StartCoroutine(IETimerCountDown(time, isRunning));
            }
        }
        else
        {
            DisableTimerCounDownUI();
        }
    }
    public IEnumerator IETimerCountDown(int time, bool isRunning)
    {
        while (time >= 0 && isRunning)
        {
            TimerCountDownUIParent.SetActive(true);
            TimerCountDownText.text = ConvertTimetoSecondsandMinute(time + 1, true);

            yield return new WaitForSeconds(1f);
            time--;
            TimerCountDownText.text = ConvertTimetoSecondsandMinute(time + 1, true);
        }
        DisableTimerCounDownUI();
    }
    public void DisableTimerCounDownUI()
    {
        TimerCountDownUIParent.SetActive(false);
        TimerCountDownText.text = "00";

        if (TimerCountdownCoroutine != null)
            StopCoroutine(TimerCountdownCoroutine);
    }

    public Coroutine ElapsedTimerCoroutine;
    public void EnableElapseTimeCounDownUI(float time, bool isRunning)
    {
        //Debug.Log("EnableElapseTimeCounDownUI" + time);
        if (isRunning)
        {
            DisableAllComponentUIObject(ComponentType.ElapsedTime);
            ElapseTimeUIParent.SetActive(true);
            if (ElapsedTimerCoroutine == null)
            {
                ElapsedTimerCoroutine = StartCoroutine(IEElapsedTimer(time, isRunning));
            }
            else
            {
                StopCoroutine(ElapsedTimerCoroutine);
                ElapsedTimerCoroutine = StartCoroutine(IEElapsedTimer(time, isRunning));
            }
        }
        else
        {
            if (ElapsedTimerCoroutine != null)
                StopCoroutine(ElapsedTimerCoroutine);
            ElapsedTimerCoroutine = null;
            StartCoroutine(IEElapsedTimer(time, false));
            //DisableElapseTimeCounDownUI();
        }
    }
    public IEnumerator IEElapsedTimer(float time, bool isRunning)
    {
        while (time >= 0 && isRunning)
        {
            ElapseTimerText.text = ConvertTimetoSecondsandMinute(time);
            yield return new WaitForSeconds(1);
            time++;
        }
        yield return new WaitForSeconds(time);
        DisableElapseTimeCounDownUI();
    }
    public void DisableElapseTimeCounDownUI()
    {
        ElapseTimeUIParent.SetActive(false);
        ElapseTimerText.text = "00:00";
        if (ElapsedTimerCoroutine != null)
            StopCoroutine(ElapsedTimerCoroutine);
    }

    Coroutine EnableDisplayMessageCoroutine;
    public void EnableDisplayMessageUI(string DisplayMessage, float time, bool state)
    {
        DisableAllComponentUIObject(ComponentType.DisplayMessage);
        if (EnableDisplayMessageCoroutine == null)
        {
            EnableDisplayMessageCoroutine = StartCoroutine(IEEnableDisplayMessageUI(DisplayMessage, time, state));
        }
        else
        {
            StopCoroutine(EnableDisplayMessageCoroutine);
            EnableDisplayMessageCoroutine = StartCoroutine(IEEnableDisplayMessageUI(DisplayMessage, time, state));
        }
    }
    public IEnumerator IEEnableDisplayMessageUI(string DisplayMessage, float time, bool state)
    {
        //if (!DisplayMessageParentUI.activeInHierarchy)
        //{
        DisplayMessageText.text = DisplayMessage;
        DisplayMessageParentUI.SetActive(true);
        //yield return new WaitForSeconds(.1f);
        //}

        while (time > 0)
        {
            //float minutes = Mathf.FloorToInt(time / 60);
            //float seconds = Mathf.FloorToInt(time % 60);
            //DisplayMessageTimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            if (state)
                DisplayMessageTimeText.text = ConvertTimetoSecondsandMinute(time);
            else
                DisplayMessageTimeText.text = "";
            //CanvasComponenetsManager._instance.timeLeft.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            yield return new WaitForSeconds(1f);
            time--;
        }
        DisplayMessageParentUI.SetActive(false);
    }

    public void DisableDisplayMessageUI()
    {
        if (EnableDisplayMessageCoroutine != null)
            StopCoroutine(EnableDisplayMessageCoroutine);
        DisplayMessageParentUI.SetActive(false);
        DisplayMessageText.text = "";
        DisplayMessageTimeText.text = "00:00";
    }

    public void EnableHelpButtonUI(string helpButtonTitle, string HelpTexts, GameObject obj)
    {
        DisableAllComponentUIObject(ComponentType.HelpButton);
        helpButtonComponentResizer.target = obj.transform;
        helpButtonComponentResizer.isAlwaysOn = false;
        HelpButtonTitleText.text = helpButtonTitle;
        HelpText.text = "";
        if (HelpTexts.Length == 0)
        {
            HelpText.text = "Define Rules here !";
        }
        else
        {
            HelpText.text = HelpTexts + "\n";
        }
        //HelpButtonUILinesCount();
        HelpButtonParentUI.SetActive(true);
    }

    public void HelpButtonUILinesCount()
    {
        if (HelpText.textInfo.lineCount > 4)
        {
            helpButtonScroll.enabled = true;
            sliderHelpButtonUI.SetActive(true);
        }
        else
        {
            helpButtonScroll.enabled = false;
            sliderHelpButtonUI.SetActive(false);
        }
    }
    public void DisableHelpButtonUI()
    {
        helpButtonComponentResizer.target = null;
        HelpButtonParentUI.SetActive(false);
        HelpText.text = "";
    }

    public Coroutine SituationChangerCoroutine;
    public void EnableSituationChangerUI(float timer)
    {
        DisableAllComponentUIObject(ComponentType.SituationChanger);
        if (timer > 0)
        {
            if (SituationChangerCoroutine == null)
            {
                SituationChangerCoroutine = StartCoroutine(IESituationChanger(timer));
            }
            else
            {
                StopCoroutine(SituationChangerCoroutine);
                SituationChangerCoroutine = StartCoroutine(IESituationChanger(timer));
            }
        }
        else
        {
            SituationChangerParentUI.SetActive(false);
            if (SituationChangerCoroutine != null)
                StopCoroutine(SituationChangerCoroutine);
        }
    }
    public IEnumerator IESituationChanger(float timer)
    {
        while (timer > 0)
        {
            //SituationChangerTimeText.text = timer.ToString("00");
            timer--;
            SituationChangerTimeText.text = ConvertTimetoSecondsandMinute(timer);
            SituationChangerParentUI.SetActive(true);
            yield return new WaitForSeconds(1f);
        }
        TimeStats._intensityChangerStop?.Invoke();
        SituationChangerParentUI.SetActive(false);
        SituationChangerTimeText.text = "00:00";
    }

    public void DisableSituationChangerUI()
    {
        if (SituationChangerCoroutine != null)
            StopCoroutine(SituationChangerCoroutine);
        SituationChangerParentUI.SetActive(false);
        SituationChangerTimeText.text = "";
    }

    #region Quiz Component

    public TMP_Text quizButtonTextInformation;
    public TMP_Text numberOfQuestions;
    public TMP_Text correctText;
    public TMP_Text wrongText;
    public TMP_Text scorePercentage;

    public GameObject[] correctWrongImageObjects;
    public GameObject quizParentReference;
    public GameObject scoreCanvas;
    public GameObject quizComponentUI;
    public Button[] options = new Button[4];
    public Button nextButton;

    public Sprite wrongImage;
    public Sprite correctImage;

    int questionIndex;
    int numOfQuestions;
    int correct, wrong;
    int currentAnswer;
    readonly int inputFieldsPerQuestion = 5; //one question and four options

    Outline currentOutline;

    bool isOptionSelected = false;
    bool isFirstQuestion = true;
    public bool isDissapearing = false;

    private QuizComponentData quizComponentData = new();
    private TMP_Text nextButtonText;

    string confirm = "Confirm";
    string result = "Result";
    string next = "Next";
    QuizComponent quizComponent;

    void EnableQuizComponentUI(QuizComponent quizComponent, QuizComponentData quizComponentData)
    {
        confirm = "Confirm";
        result = "Result";
        next = "Next";
        DisableAllComponentUIObject(ComponentType.Quiz);
        quizComponentUI.SetActive(true);
        this.quizComponent = quizComponent;
        StartQuiz(quizComponentData);


    }
    public void QuizResultPopupClose()
    {
        if (quizComponent != null)
            CheckScorePercentage();
    }

    public void StartQuiz(QuizComponentData data)
    {
        for (int i = 0; i < options.Length; i++)
        {
            options[i].onClick.RemoveAllListeners();
        }

        nextButton.onClick.RemoveAllListeners();
        for (int i = 0; i < options.Length; i++)
        {
            int c = i;
            options[c].onClick.AddListener(delegate { OnSelectOption(c); });
            options[c].GetComponent<Outline>().enabled = false;
            correctWrongImageObjects[c].SetActive(false);
        }

        nextButton.onClick.AddListener(delegate { DisplayNextQuestion(); });

        this.quizComponentData = null;
        quizParentReference.SetActive(false);
        this.quizComponentData = data;

        SetInitialValues();

        if (scoreCanvas.activeInHierarchy)
            scoreCanvas.SetActive(false);

        DisplayNextQuestion();
        quizParentReference.SetActive(true);
    }

    private void SetInitialValues()
    {
        numOfQuestions = quizComponentData.answers.Count;

        questionIndex = 0;
        correct = 0;
        wrong = 0;

        nextButtonText = nextButton.GetComponentInChildren<TMP_Text>();
        confirm = TextLocalization.GetLocaliseTextByKey("Confirm");
        nextButtonText.text = confirm;

        isFirstQuestion = true;
        isOptionSelected = false;
    }

    private void CheckAnswer()
    {
        if (quizComponentData.answers[questionIndex] == currentAnswer)
        {
            UpdateQuizData(0);
        }
        else
        {
            currentOutline.enabled = false;
            UpdateQuizData(1);
        }

        questionIndex += 1;
        next = TextLocalization.GetLocaliseTextByKey("Next");
        result = TextLocalization.GetLocaliseTextByKey("Result");
        //Debug.Log("TextLocalization==>" + next + " " + result);

        nextButtonText.text = (questionIndex < numOfQuestions) ? next : result;
        SetButtonInteractability(true, false);
    }


    void OnSelectOption(int answer)
    {
        currentAnswer = answer;
        SetButtonInteractability(true, true);
        UpdateQuizData(2);

        if (!isOptionSelected)
        {
            isOptionSelected = true;
            confirm = TextLocalization.GetLocaliseTextByKey("Confirm");
            //Debug.Log("TextLocalization==>" + confirm);

            nextButtonText.text = confirm;
        }
    }

    private void DisplayNextQuestion()
    {
        if (nextButtonText.text == confirm)
        {
            if (!isOptionSelected)
            {
                if (isFirstQuestion)
                    ShowQuestion();
                else
                    Debug.Log("Please Select an Option First");

                isFirstQuestion = false;
            }
            else
            {
                CheckAnswer();
            }

            return;
        }

        if (nextButtonText.text == next)
        {
            ShowQuestion();
            return;
        }

        if (nextButtonText.text == result)
        {
            quizParentReference.SetActive(false);
            ShowScoreCanvasRoutine();
        }
    }

    void ShowQuestion()
    {
        SetButtonInteractability(true, true);
        confirm = TextLocalization.GetLocaliseTextByKey("Confirm");
        //Debug.Log("Confirm Localise " + confirm);

        nextButtonText.text = confirm;

        if (questionIndex < numOfQuestions)
        {
            string s = TextLocalization.GetLocaliseTextByKey("Question");
            string s2 = "/";// TextLocalization.GetLocaliseTextByKey("of");
            string s3 = TextLocalization.GetLocaliseTextByKey("Q");
            //Debug.Log("TextLocalization==>" + s + " " + s2 + " " + s3);

            numberOfQuestions.text = s + " " + (questionIndex + 1) + " " + s2 + " " + numOfQuestions;
            quizButtonTextInformation.text = s3 + ": " + quizComponentData.rewritingStringList[questionIndex * inputFieldsPerQuestion];

            for (int i = 1; i < inputFieldsPerQuestion; i++)
            {
                string sb = quizComponentData.rewritingStringList[i + (questionIndex * inputFieldsPerQuestion)];
                options[i - 1].GetComponentInChildren<TMP_Text>().text =
                    sb;
                if (!isPotrait)
                {
                    if (GameManager.currentLanguage == "ja" || CustomLocalization.forceJapanese || ContainsJapaneseText(sb))
                        options[i - 1].GetComponentInChildren<TMP_Text>().fontSize = 11.3f;
                    else
                        options[i - 1].GetComponentInChildren<TMP_Text>().fontSize = 12;
                }
            }
        }

        isOptionSelected = false;
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
        Sprite image = correctImage;

        //0 if the answer is correct, 1 if wrong, 2 is for selecting only (answer not yet confirmed)
        switch (option)
        {
            case 0:
                correct++;
                image = correctImage;
                colorString = "#36C34E";
                break;

            case 1:
                wrong++;
                image = wrongImage;
                break;

            case 2:
                colorString = "#008FFF";
                break;
        }

        if (option == 0 || option == 1)
        {
            correctWrongImageObjects[currentAnswer].SetActive(true);
        }

        correctWrongImageObjects[currentAnswer].GetComponent<Image>().sprite = image;

        Outline thisButtonOutine = options[currentAnswer].GetComponent<Outline>();

        if (ColorUtility.TryParseHtmlString(colorString, out Color color))
        {
            thisButtonOutine.effectColor = color;
        }

        if (option == 0 || option == 2)
        {
            thisButtonOutine.enabled = true;
            currentOutline = thisButtonOutine;
        }

        if (option == 1)
        {
            WrongAnswerUIAdjustments(correctImage, "#36C34E");
        }
    }

    private void WrongAnswerUIAdjustments(Sprite image, string colorString)
    {
        correctWrongImageObjects[quizComponentData.answers[questionIndex]].SetActive(true);
        correctWrongImageObjects[quizComponentData.answers[questionIndex]].GetComponent<Image>().sprite = image;

        Outline thisButtonOutine = options[quizComponentData.answers[questionIndex]].GetComponent<Outline>();

        if (ColorUtility.TryParseHtmlString(colorString, out Color color))
        {
            thisButtonOutine.effectColor = color;
        }

        thisButtonOutine.enabled = true;
        currentOutline = thisButtonOutine;

    }

    private void SetButtonInteractability(bool isOptions, bool isInteractable)
    {
        if (isOptions)
        {
            for (int i = 0; i < options.Length; i++)
            {
                options[i].transition = Selectable.Transition.None;
                options[i].interactable = isInteractable;
                if (isInteractable)
                {
                    options[i].GetComponent<Outline>().enabled = false;
                    correctWrongImageObjects[i].SetActive(false);
                }
            }
        }
    }

    private void ShowScoreCanvasRoutine()
    {
        scoreCanvas.SetActive(true);
        string s = TextLocalization.GetLocaliseTextByKey("Correct");
        string s2 = TextLocalization.GetLocaliseTextByKey("Wrong");
        string s3 = TextLocalization.GetLocaliseTextByKey("Correct Answer is");
        string s4 = TextLocalization.GetLocaliseTextByKey("Confirm");
        //Debug.Log("TextLocalization==>" + s + " " + s2 + " " + s3 + " " + s4);
        correctText.text = s + ": " + correct;
        wrongText.text = s2 + ": " + wrong;
        float percentage = (((float)correct / numOfQuestions) * 100);
        scorePercentage.text = s3 + " " + percentage.ToString("0.#") + "%";

        isFirstQuestion = true;
        isOptionSelected = false;

        nextButtonText.text = s4;
    }

    public void CheckScorePercentage()
    {
        StartCoroutine(CheckScorePercentageRoutine());
    }

    IEnumerator CheckScorePercentageRoutine()
    {
        float division = (((float)correct / numOfQuestions) * 100);

        if (division >= quizComponentData.correctAnswerRate)
        {
            isDissapearing = true;
            yield return new WaitForSeconds(0);
            isDissapearing = false;
            quizComponent.gameObject.SetActive(false);
        }
        quizComponent = null;
        ResetCredentials();
    }

    void ResetCredentials()
    {
        correct = 0;
        wrong = 0;
        questionIndex = 0;
        DisableQuizComponentUI();
    }

    void DisableQuizComponentUI()
    {
        quizComponentUI.SetActive(false);
    }
    #endregion

    #region Special Item Component
    public GameObject SpecialItemUIParent;
    public TextMeshProUGUI SpecialItemText;
    Coroutine SpecialItemCoroutine;

    public void EnableSpecialItemUI(float time)
    {
        DisableAllComponentUIObject(ComponentType.SpecialItem);
        SpecialItemUIParent.SetActive(true);

        if (SpecialItemCoroutine == null)
        {
            SpecialItemCoroutine = StartCoroutine(nameof(IESpecialItem), time);
        }
        else
        {
            if (SpecialItemCoroutine != null)
                StopCoroutine(SpecialItemCoroutine);
            SpecialItemCoroutine = StartCoroutine(nameof(IESpecialItem), time);
        }
    }

    public IEnumerator IESpecialItem(float time)
    {
        while (time > 0)
        {
            time--;
            SpecialItemText.text = ConvertTimetoSecondsandMinute(time);
            yield return new WaitForSeconds(1);
        }
        DisableSpecialItemUI();
    }

    public void DisableSpecialItemUI()
    {
        SpecialItemUIParent.SetActive(false);
        SpecialItemText.text = "";
        if (SpecialItemCoroutine != null)
            StopCoroutine(SpecialItemCoroutine);
    }
    #endregion

    #region Avatar Invisibility Component
    public GameObject AvatarInvisibilityUIParent;
    public TextMeshProUGUI AvatarInvisibilityText;
    Coroutine AvatarInvisibilityCoroutine;

    public void EnableAvatarInvisibilityUI(float time)
    {
        DisableAllComponentUIObject(ComponentType.AvatarInvisibility);
        AvatarInvisibilityUIParent.SetActive(true);

        if (AvatarInvisibilityCoroutine == null)
        {
            AvatarInvisibilityCoroutine = StartCoroutine(nameof(IEAvatarInvisibility), time);
        }
        else
        {
            if (AvatarInvisibilityCoroutine != null)
                StopCoroutine(AvatarInvisibilityCoroutine);
            AvatarInvisibilityCoroutine = StartCoroutine(nameof(IEAvatarInvisibility), time);
        }
    }

    public IEnumerator IEAvatarInvisibility(float time)
    {
        while (time > 0)
        {
            time--;
            AvatarInvisibilityText.text = ConvertTimetoSecondsandMinute(time);
            yield return new WaitForSeconds(1);
        }
        DisableAvatarInvisibilityUI();
    }

    public void DisableAvatarInvisibilityUI()
    {
        AvatarInvisibilityUIParent.SetActive(false);
        AvatarInvisibilityText.text = "";
        if (AvatarInvisibilityCoroutine != null)
            StopCoroutine(AvatarInvisibilityCoroutine);
    }
    #endregion

    #region Ninja Motion Component
    public GameObject NinjaMotionUIParent;

    public GameObject NinjaMotionUIButtonPanel;
    public GameObject NinjaMotionUIButtonPanel2;
    public TextMeshProUGUI NinjaMotionText;
    Coroutine NinjaMotionCoroutine;

    public void EnableNinjaMotionUI(float time)
    {
        DisableAllComponentUIObject(ComponentType.NinjaMotion);
        DisableThrowThingUI();
        NinjaMotionUIParent.SetActive(true);

        if (NinjaMotionCoroutine == null)
        {
            NinjaMotionCoroutine = StartCoroutine(nameof(IENinjaMotion), time);
        }
        else
        {
            if (NinjaMotionCoroutine != null)
                StopCoroutine(NinjaMotionCoroutine);
            NinjaMotionCoroutine = StartCoroutine(nameof(IENinjaMotion), time);
        }
    }

    public void AttackwithSword()
    {
        BuilderEventManager.OnAttackwithSword?.Invoke();
    }
    public void HideOpenSword()
    {
        BuilderEventManager.OnHideOpenSword?.Invoke();
    }
    public void AttckwithShuriken()
    {
        BuilderEventManager.OnAttackwithShuriken?.Invoke();
    }

    public IEnumerator IENinjaMotion(float time)
    {
        while (time > 0)
        {
            time--;
            NinjaMotionText.text = ConvertTimetoSecondsandMinute(time);
            yield return new WaitForSeconds(1);
        }
        DisableNinjaMotionUI();
    }

    public void DisableNinjaMotionUI()
    {
        NinjaMotionUIParent.SetActive(false);
        NinjaMotionText.text = "";
        if (NinjaMotionCoroutine != null)
            StopCoroutine(NinjaMotionCoroutine);
    }
    #endregion

    #region Throw Things Component
    public GameObject ThrowThingsUIParent;
    public GameObject ThowThingsUIButtonPanel;
    public GameObject ThowThingsUIButtonPanel2;

    public void EnableThrowThingsUI()
    {
        DisableAllComponentUIObject(ComponentType.ThrowThings);
        ThrowThingsUIParent.SetActive(true);
    }

    public void OnBallPositionSet()
    {
        BuilderEventManager.OnThowThingsPositionSet?.Invoke();
    }
    public void OnThrowBall()
    {
        BuilderEventManager.OnThrowBall?.Invoke();
    }

    public void DisableThrowThingUI()
    {
        ThrowThingsUIParent.SetActive(false);
    }
    #endregion

    void ChangeNinja_ThrowUIPosition(float value, bool state)
    {
        NinjaMotionUIButtonPanel.transform.DOKill();
        ThowThingsUIButtonPanel.transform.DOKill();
        NinjaMotionUIButtonPanel2.transform.DOKill();
        ThowThingsUIButtonPanel2.transform.DOKill();
        if (isPotrait && state)
        {
            //Portrait code here
            Vector3 position = NinjaMotionUIButtonPanel2.transform.localPosition;
            if (value > 0)
            {
                position = GamificationComponentData.instance.Ninja_Throw_InitPosY;
                NinjaMotionUIButtonPanel2.transform.localPosition = position;
                ThowThingsUIButtonPanel2.transform.localPosition = position;
            }
            else
            {
                position.y = value;
                NinjaMotionUIButtonPanel2.transform.DOLocalMove(position, 0);
                ThowThingsUIButtonPanel2.transform.DOLocalMove(position, 0);
            }
        }
        else
        {
            //LandscapeLeft code here
            if (value > 0)
                value = 0;

            Vector3 position = GamificationComponentData.instance.Ninja_Throw_InitPosX;
            value = GamificationComponentData.instance.Ninja_Throw_InitPosX.x + value;
            position.x = value;
            NinjaMotionUIButtonPanel.transform.localPosition = position;
            ThowThingsUIButtonPanel.transform.localPosition = position;
        }
    }

    #region HyperLink Popup
    public GameObject HyperLinkPopupUIParent;
    public HyperlinkPanelResizer hyperlinkPanelResizer;
    public TextMeshProUGUI hyperLinkPopupTitleText;
    public TextMeshProUGUI hyperLinkPopupText;
    string url;
    public void EnableHyperLinkPopupUI(string hyperLinkPopupTitle, string hyperLinkPopupTexts, string hyperLinkPopupURL, Transform obj)
    {
        HyperLinkPopupUIParent.SetActive(true);
        DisableAllComponentUIObject(ComponentType.HyperLinkPopup);
        hyperLinkPopupTitleText.text = hyperLinkPopupTitle;
        hyperLinkPopupText.text = "";
        hyperlinkPanelResizer.target = obj;
        url = hyperLinkPopupURL;
        if (hyperLinkPopupTexts.Length == 0)
        {
            hyperLinkPopupText.text = "Define Rules here !";
        }
        else
        {
            hyperLinkPopupText.text = hyperLinkPopupTexts + "\n";
        }
        //HelpButtonUILinesCount();
    }

    public void OnClickHyperLinkButton()
    {
        Application.OpenURL(url);
    }

    public void DisableHyperLinkPopupUI()
    {
        HyperLinkPopupUIParent.SetActive(false);
    }

    #endregion

    #region Blind Component
    public Coroutine BlindComponentCoroutine;
    public void EnableBlindComponentUI(float timer)
    {
        DisableAllComponentUIObject(ComponentType.SituationChanger);
        if (timer > 0)
        {
            if (BlindComponentCoroutine == null)
            {
                BlindComponentCoroutine = StartCoroutine(IEBlindComponent(timer));
            }
            else
            {
                StopCoroutine(BlindComponentCoroutine);
                BlindComponentCoroutine = StartCoroutine(IEBlindComponent(timer));
            }
        }
        else
        {
            BlindComponentParentUI.SetActive(false);
            if (BlindComponentCoroutine != null)
                StopCoroutine(BlindComponentCoroutine);
        }
    }
    public IEnumerator IEBlindComponent(float timer)
    {
        while (timer > 0)
        {
            //SituationChangerTimeText.text = timer.ToString("00");
            timer--;
            BlindComponentTimeText.text = ConvertTimetoSecondsandMinute(timer);
            BlindComponentParentUI.SetActive(true);
            yield return new WaitForSeconds(1f);
        }
        TimeStats._blindComponentStop?.Invoke();
        BlindComponentParentUI.SetActive(false);
        BlindComponentTimeText.text = "00:00";
    }

    public void DisableBlindComponentUI()
    {
        if (BlindComponentCoroutine != null)
            StopCoroutine(BlindComponentCoroutine);
        BlindComponentParentUI.SetActive(false);
        BlindComponentTimeText.text = "";
    }
    #endregion

    string ConvertTimetoSecondsandMinute(float time, bool onlySS = false)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        if (!onlySS)
            return string.Format("{0:00}:{1:00}", minutes, seconds);
        else
            return time.ToString("00");
    }

    void DisableAllComponentUIObject(ComponentType componentType)
    {
        if (componentType != ComponentType.SituationChanger)
            DisableSituationChangerUI();
        if (componentType != ComponentType.DisplayMessage)
            DisableDisplayMessageUI();
        if (componentType != ComponentType.TimerCountDown)
            DisableElapseTimeCounDownUI();
        if (componentType != ComponentType.HelpButton)
            DisableHelpButtonUI();
        if (componentType != ComponentType.Narration)
            DisableNarrationUI();
        if (componentType != ComponentType.RandomNumberGenerator)
            DisableRandomNumberUI();
        if (componentType != ComponentType.TimeLimit)
            DisableTimeLimitUI();
        if (componentType != ComponentType.TimerCountDown)
            DisableTimerCounDownUI();
        if (componentType != ComponentType.Quiz)
            DisableQuizComponentUI();
        if (componentType != ComponentType.SpecialItem)
            DisableSpecialItemUI();
        if (componentType != ComponentType.NinjaMotion)
            DisableNinjaMotionUI();
        if (componentType != ComponentType.AvatarInvisibility)
            DisableAvatarInvisibilityUI();
        if (componentType != ComponentType.ThrowThings)
            DisableThrowThingUI();
        if (componentType != ComponentType.HyperLinkPopup)
            DisableHyperLinkPopupUI();
        if (componentType != ComponentType.BlindComponent)
            DisableBlindComponentUI();
    }
}

public enum ComponentType
{
    None,
    TimeLimit,
    ElapsedTime,
    Narration,
    RandomNumberGenerator,
    TimerCountDown,
    SituationChanger,
    HelpButton,
    DisplayMessage,
    SpecialItem,
    Quiz,
    NinjaMotion,
    AvatarInvisibility,
    ThrowThings,
    HyperLinkPopup,
    BlindComponent
}