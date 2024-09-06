using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using Models;
using System.Globalization;
using Photon.Pun;
using System.Text.RegularExpressions;

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
        BuilderEventManager.OnDoorKeyCollisionEnter += EnableDoorKeyUI;
        BuilderEventManager.OnHelpButtonCollisionEnter += EnableHelpButtonUI;
        BuilderEventManager.OnHelpButtonCollisionExit += DisableHelpButtonUI;
        BuilderEventManager.OnSituationChangerTriggerEnter += EnableSituationChangerUI;
        BuilderEventManager.OnBlindComponentTriggerEnter += EnableBlindComponentUI;
        BuilderEventManager.OnQuizComponentCollisionEnter += EnableQuizComponentUI;
        BuilderEventManager.OnQuizComponentColse += ResetCredentials;
        BuilderEventManager.OnSpecialItemComponentCollisionEnter += EnableSpecialItemUI;
        BuilderEventManager.OnAvatarInvisibilityComponentCollisionEnter += EnableAvatarInvisibilityUI;
        BuilderEventManager.OnNinjaMotionComponentCollisionEnter += EnableNinjaMotionUI;
        BuilderEventManager.OnThrowThingsComponentCollisionEnter += EnableThrowThingsUI;
        BuilderEventManager.OnThrowThingsComponentDisable += DisableThrowThingUI;
        BuilderEventManager.OnHyperLinkPopupCollisionEnter += EnableHyperLinkPopupUI;
        BuilderEventManager.OnHyperLinkPopupCollisionExit += DisableHyperLinkPopupUI;
        BuilderEventManager.OnAvatarChangeComponentTriggerEnter += EnableAvatarChangerComponentUI;
        BuilderEventManager.ResetComponentUI += DisableAllComponentUIObject;

        BuilderEventManager.ChangeNinja_ThrowUIPosition += ChangeNinja_ThrowUIPosition;
        BuilderEventManager.PositionUpdateOnOrientationChange += PositionUpdateOnOrientationChange;


        DisableThrowThingUI();
        DisableAllComponentUIObject(Constants.ItemComponentType.none);


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
        BuilderEventManager.OnDoorKeyCollisionEnter -= EnableDoorKeyUI;
        BuilderEventManager.OnHelpButtonCollisionEnter -= EnableHelpButtonUI;
        BuilderEventManager.OnHelpButtonCollisionExit -= DisableHelpButtonUI;
        BuilderEventManager.OnSituationChangerTriggerEnter -= EnableSituationChangerUI;
        BuilderEventManager.OnBlindComponentTriggerEnter -= EnableBlindComponentUI;
        BuilderEventManager.OnQuizComponentCollisionEnter -= EnableQuizComponentUI;
        BuilderEventManager.OnQuizComponentColse -= ResetCredentials;
        BuilderEventManager.OnSpecialItemComponentCollisionEnter -= EnableSpecialItemUI;
        BuilderEventManager.OnAvatarInvisibilityComponentCollisionEnter -= EnableAvatarInvisibilityUI;
        BuilderEventManager.OnNinjaMotionComponentCollisionEnter -= EnableNinjaMotionUI;
        BuilderEventManager.OnThrowThingsComponentCollisionEnter -= EnableThrowThingsUI;
        BuilderEventManager.OnThrowThingsComponentDisable -= DisableThrowThingUI;

        BuilderEventManager.OnHyperLinkPopupCollisionEnter -= EnableHyperLinkPopupUI;
        BuilderEventManager.OnHyperLinkPopupCollisionExit -= DisableHyperLinkPopupUI;
        BuilderEventManager.OnAvatarChangeComponentTriggerEnter -= EnableAvatarChangerComponentUI;
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

        //defaultFont = GamificationComponentData.instance.arialFont;

    }

    TMP_FontAsset defaultFont;

    //Narration Comopnent
    internal NarrationComponent narrationComponent;
    public GameObject narrationUIParent;
    public TextMeshProUGUI narrationTextUI;
    float letterDelay = 0.1f;
    int storyCharCount = 0;
    bool isAgainCollided;
    bool isStoryWritten;
    public ScrollRect narrationScroll;
    public GameObject sliderNarrationUI;
    Coroutine StoryNarrationCoroutine;
    public Button narrationUIClosebtn;
    public Button narrationUIDownTextbtn;
    float narrationtotalHeight;
    float singleLineHeight;

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

    //Door Key Component
    public GameObject DoorKeyParentUI;
    public TextMeshProUGUI DoorKeyText;

    //Help Button Component
    public GameObject HelpButtonParentUI;
    public HelpButtonComponentResizer helpButtonComponentResizer;
    public TextMeshProUGUI HelpButtonTitleText;
    public TextMeshProUGUI HelpText;
    public ScrollRect helpButtonScroll;
    public GameObject sliderHelpButtonUI;
    float helppopupTotalheight;
    float helppopupSingleLineHeight;

    //Situation Changer Component
    public GameObject SituationChangerParentUI;
    public TextMeshProUGUI SituationChangerTimeText;



    //Narration Component
    void EnableNarrationUI(string narrationText, bool isStory, bool closeNarration)
    {
        DisableAllComponentUIObject(Constants.ItemComponentType.NarrationComponent);
        isStoryWritten = true;
        Invoke(nameof(NarrationUILinesCount), 0.01f);
        narrationUIParent.SetActive(true);
        narrationUIClosebtn.gameObject.SetActive(closeNarration);

        if (!isStory)
        {
            if (StoryNarrationCoroutine != null)
                StopCoroutine(StoryNarrationCoroutine);
            isAgainCollided = true;
            //StartCoroutine(WaitDelayStatement());
            narrationTextUI.text = narrationText;
            narrationScroll.enabled = false;
            sliderNarrationUI.SetActive(false);
            isStoryWritten = false;
            Invoke(nameof(NarrationUILinesCount), 0.1f);
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
        narrationTextUI.rectTransform.parent.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);

        narrationScroll.enabled = false;
        sliderNarrationUI.SetActive(false);

        narrationtotalHeight = narrationTextUI.rectTransform.rect.height;

        // Get the number of lines in the text.
        int numberOfLines = narrationTextUI.textInfo.lineCount;
        // Calculate the single line height by dividing the total height by the number of lines.
        singleLineHeight = narrationtotalHeight / numberOfLines;

        narrationUIDownTextbtn.interactable = !isStoryWritten;
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
            if (defaultFont)
                narrationTextUI.font = defaultFont;
            storyCharCount++;

            yield return new WaitForSeconds(letterDelay);
            //StartCoroutine(WaitForScrollingOption());
        }
        isStoryWritten = false;
        NarrationUILinesCount();
    }
    IEnumerator WaitDelayStatement()
    {
        yield return new WaitForSeconds(0.2f);
        isAgainCollided = false;
    }
    public void DisplayDownText()
    {
        if (narrationScroll.content.anchoredPosition.y + singleLineHeight * 4 <= narrationtotalHeight)
        {
            narrationScroll.content.anchoredPosition += new Vector2(0, singleLineHeight);
        }
        else
        {
            DisableNarrationUI();
        }
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
        DisableAllComponentUIObject(Constants.ItemComponentType.RandomNumberComponent);
        RandomNumberUIParent.SetActive(true);
        if (defaultFont)
            RandomNumberText.font = defaultFont;
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
        DisableAllComponentUIObject(Constants.ItemComponentType.TimeLimitComponent, true);
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
        if (time > 0)
            BuilderEventManager.OnTimerLimitEnd += OnTimerLimitEnd;

    }

    public void OnTimerLimitEnd()
    {
        if (TimeCoroutine != null)
            StopCoroutine(TimeCoroutine);
    }

    public IEnumerator IETimeLimit(float time)
    {
        while (time > 0)
        {
            time--;
            TimeLimitText.text = ConvertTimetoSecondsandMinute(time);
            yield return new WaitForSeconds(1);
        }
        //yield return new WaitForSeconds(5f);
        DisableTimeLimitUI();
    }

    public void DisableTimeLimitUI()
    {
        TimeLimitUIParent.SetActive(false);
        TimeLimitText.text = "";
        if (TimeCoroutine != null)
            StopCoroutine(TimeCoroutine);
        BuilderEventManager.OnTimerLimitEnd -= OnTimerLimitEnd;
    }

    public Coroutine TimerCountdownCoroutine;
    public void EnableTimerCountDownUI(int time, bool isRunning)
    {
        DisableAllComponentUIObject(Constants.ItemComponentType.TimerCountdownComponent, true);
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
            TimerCountDownText.text = ConvertTimetoSecondsandMinute(time + 1);

            yield return new WaitForSeconds(1f);
            time--;
            TimerCountDownText.text = ConvertTimetoSecondsandMinute(time + 1);
        }
        DisableTimerCounDownUI();
    }
    public void DisableTimerCounDownUI()
    {
        TimerCountDownUIParent.SetActive(false);
        TimerCountDownText.text = "00:00";

        if (TimerCountdownCoroutine != null)
            StopCoroutine(TimerCountdownCoroutine);
    }

    public Coroutine ElapsedTimerCoroutine;
    public void EnableElapseTimeCounDownUI(float time, bool isRunning)
    {
        //Debug.LogError("EnableElapseTimeCounDownUI ==> " + time + "  " + isRunning);
        if (isRunning)
        {
            DisableAllComponentUIObject(Constants.ItemComponentType.ElapsedTimeComponent, true);
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
            BuilderEventManager.elapsedEndTime += ElapsedEndTime;

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

    public void ElapsedEndTime()
    {
        if (ElapsedTimerCoroutine != null)
            StopCoroutine(ElapsedTimerCoroutine);
    }
    public IEnumerator IEElapsedTimer(float time, bool isRunning)
    {
        if (isRunning)
            while (time >= 0)
            {
                ElapseTimerText.text = ConvertTimetoSecondsandMinute(time);
                yield return new WaitForSeconds(1);
                time++;
            }
        else
            yield return new WaitForSeconds(time);
        DisableElapseTimeCounDownUI();
    }
    public void DisableElapseTimeCounDownUI()
    {
        ElapseTimeUIParent.SetActive(false);
        ElapseTimerText.text = "00:00";
        if (ElapsedTimerCoroutine != null)
            StopCoroutine(ElapsedTimerCoroutine);
        BuilderEventManager.elapsedEndTime -= ElapsedEndTime;
    }

    Coroutine EnableDisplayMessageCoroutine;
    public void EnableDisplayMessageUI(string DisplayMessage, float time, bool state)
    {
        DisableAllComponentUIObject(Constants.ItemComponentType.DisplayMessagesComponent);
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

        DisplayMessageText.text = DisplayMessage;
        bool isJPText = CheckJapaneseDisplayMessage(DisplayMessage);
        if (isJPText)
            DisplayMessageText.font = GamificationComponentData.instance.hiraginoFont;
        else
            DisplayMessageText.font = GamificationComponentData.instance.orbitronFont;

        DisplayMessageParentUI.SetActive(true);

        while (time > 0)
        {
            DisplayMessageTimeText.text = "";
            DisplayMessageTimeText.transform.parent.gameObject.SetActive(false);
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
        DisplayMessageTimeText.transform.parent.gameObject.SetActive(true);
    }

    public void EnableHelpButtonUI(string helpButtonTitle, string HelpTexts, GameObject obj)
    {
        DisableAllComponentUIObject(Constants.ItemComponentType.HelpButtonComponent);
        helpButtonComponentResizer.target = obj.transform;
        helpButtonComponentResizer.isAlwaysOn = false;
        HelpButtonTitleText.text = helpButtonTitle;
        HelpText.text = "";
        //if (HelpTexts.Length == 0)
        //{
        //    HelpText.text = "Define Rules here !";
        //}
        //else
        //{
        //    HelpText.text = HelpTexts + "\n";
        //}
        helpButtonComponentResizer.titleText.text = HelpButtonTitleText.text;
        if (defaultFont)
        {
            HelpButtonTitleText.font = defaultFont;
            helpButtonComponentResizer.titleText.font = defaultFont;
        }
        //helpButtonComponentResizer.contentText.text = HelpText.text;
        helpButtonComponentResizer.msg = HelpTexts.Length == 0 ? "Define Rules here !" : HelpTexts + "\n";
        HelpButtonParentUI.SetActive(true);
        helpButtonComponentResizer.Init();
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
        if (timer > 0)
        {
            DisableAllComponentUIObject(Constants.ItemComponentType.SituationChangerComponent, true);
            //if (SituationChangerCoroutine == null)
            //{
            //    SituationChangerCoroutine = StartCoroutine(IESituationChanger(timer));
            //}
            //else
            //{
            //    StopCoroutine(SituationChangerCoroutine);
            //    SituationChangerCoroutine = StartCoroutine(IESituationChanger(timer));
            //}
            SituationChangerTimeText.text = ConvertTimetoSecondsandMinute(timer);
            SituationChangerParentUI.SetActive(true);
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
        //TimeStats._intensityChangerStop?.Invoke();
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

    public TextMeshProUGUI quizButtonTextInformation;
    public TextMeshProUGUI numberOfQuestions;
    public TextMeshProUGUI correctText;
    public TextMeshProUGUI wrongText;
    public TextMeshProUGUI scorePercentage;

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
    private TextMeshProUGUI nextButtonText;

    string confirm = "Confirm";
    string result = "Result";
    string next = "Next";
    QuizComponent quizComponent;

    void EnableQuizComponentUI(QuizComponent quizComponent, QuizComponentData quizComponentData)
    {
        confirm = "Confirm";
        result = "Result";
        next = "Next";
        DisableAllComponentUIObject(Constants.ItemComponentType.QuizComponent);
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

        nextButtonText = nextButton.GetComponentInChildren<TextMeshProUGUI>();
        confirm = TextLocalization.GetLocaliseTextByKey("Confirm");
        nextButtonText.text = confirm;
        if (defaultFont)
            nextButtonText.font = defaultFont;
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
        next = TextLocalization.GetLocaliseTextByKey("BuilderNext");
        result = TextLocalization.GetLocaliseTextByKey("Result");
        //Debug.Log("TextLocalization==>" + next + " " + result);

        nextButtonText.text = (questionIndex < numOfQuestions) ? next : result;
        if (defaultFont)
            nextButtonText.font = defaultFont;
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
            if (defaultFont)
                nextButtonText.font = defaultFont;
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
        if (defaultFont)
            nextButtonText.font = defaultFont;
        if (questionIndex < numOfQuestions)
        {
            string s = TextLocalization.GetLocaliseTextByKey("Question");
            string s2 = "/";// TextLocalization.GetLocaliseTextByKey("of");
            string s3 = TextLocalization.GetLocaliseTextByKey("Q");
            //Debug.Log("TextLocalization==>" + s + " " + s2 + " " + s3);

            numberOfQuestions.text = s + " " + (questionIndex + 1) + " " + s2 + " " + numOfQuestions;
            quizButtonTextInformation.text = s3 + ": " + quizComponentData.rewritingStringList[questionIndex * inputFieldsPerQuestion];

            if (defaultFont)
            {
                numberOfQuestions.font = defaultFont;
                quizButtonTextInformation.font = defaultFont;
            }

            for (int i = 1; i < inputFieldsPerQuestion; i++)
            {
                string sb = quizComponentData.rewritingStringList[i + (questionIndex * inputFieldsPerQuestion)];
                options[i - 1].GetComponentInChildren<TextMeshProUGUI>().text =
                    sb;

                if (defaultFont)
                    options[i - 1].GetComponentInChildren<TextMeshProUGUI>().font = defaultFont;
                if (!isPotrait)
                {
                    if (GameManager.currentLanguage == "ja" || LocalizationManager.forceJapanese || ContainsJapaneseText(sb))
                        options[i - 1].GetComponentInChildren<TextMeshProUGUI>().fontSize = 11.3f;
                    else
                        options[i - 1].GetComponentInChildren<TextMeshProUGUI>().fontSize = 12;
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
                ReferencesForGamePlay.instance.m_34player.GetComponent<SoundEffects>().PlaySoundEffects(SoundEffects.Sounds.QuizCorrect);

                colorString = "#36C34E";
                break;

            case 1:
                wrong++;
                image = wrongImage;
                ReferencesForGamePlay.instance.m_34player.GetComponent<SoundEffects>().PlaySoundEffects(SoundEffects.Sounds.QuizWrong);

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

        if (defaultFont)
        {
            correctText.font = defaultFont;
            wrongText.font = defaultFont;
            scorePercentage.font = defaultFont;
            nextButtonText.font = defaultFont;
        }
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
            if (GamificationComponentData.instance.withMultiplayer)
                GamificationComponentData.instance.photonView.RPC("GetObject", RpcTarget.All, quizComponent.GetComponent<XanaItem>().itemData.RuntimeItemID, Constants.ItemComponentType.none);
            else
                GamificationComponentData.instance.GetObjectwithoutRPC(quizComponent.GetComponent<XanaItem>().itemData.RuntimeItemID, Constants.ItemComponentType.none);
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

    internal void DisableQuizComponentUI()
    {
        quizComponentUI.SetActive(false);
        quizComponent = null;
    }
    #endregion

    #region Special Item Component
    public GameObject SpecialItemUIParent;
    public TextMeshProUGUI SpecialItemText;
    Coroutine SpecialItemCoroutine;

    public void EnableSpecialItemUI(float time)
    {
        DisableAllComponentUIObject(Constants.ItemComponentType.SpecialItemComponent, true);
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
        DisableAllComponentUIObject(Constants.ItemComponentType.BlindfoldedDisplayComponent, true);
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
        DisableAllComponentUIObject(Constants.ItemComponentType.NinjaComponent, true);
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
        DisableAllComponentUIObject(Constants.ItemComponentType.ThrowThingsComponent, true);
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
    public ScrollRect hyperLinkScrollView;
    public GameObject hyperLinkScrollbar;
    public Button hyperlinkDownArrowbtn;
    public Button hyperlinkBrowseURLbtn;
    string url;
    float hyperlinkTotalHeight;
    int hyperLinkCharCount = 0;
    float hyperLinkSingleLineHeight;
    Coroutine HyperLinkCoroutine;
    bool isAgainHyperLinkCollided;
    bool isHyperlinkWritten;


    public void EnableHyperLinkPopupUI(string hyperLinkPopupTitle, string hyperLinkPopupTexts, string hyperLinkPopupURL, Transform obj)
    {
        HyperLinkPopupUIParent.SetActive(true);
        DisableAllComponentUIObject(Constants.ItemComponentType.HyperLinkPopComponent);
        hyperLinkPopupTitleText.text = hyperLinkPopupTitle;
        if (defaultFont)
        {
            hyperLinkPopupTitleText.font = defaultFont;
            hyperlinkBrowseURLbtn.GetComponentInChildren<TextMeshProUGUI>().font = defaultFont;
        }
        hyperlinkPanelResizer.target = obj;
        url = hyperLinkPopupURL;
        string msg = hyperLinkPopupTexts.Length == 0 ? "Define Rules here !" : hyperLinkPopupTexts + "\n";

        hyperLinkPopupText.text = msg;
        isHyperlinkWritten = false;
        Invoke(nameof(HyperLinkUILinesCount), 0.1f);

        //hyperLinkCharCount = 0;
        //hyperLinkPopupText.text = "";
        //if (HyperLinkCoroutine == null)
        //    HyperLinkCoroutine = StartCoroutine(HyperLinkPopupCO(msg));
        //else
        //{
        //    StopCoroutine(HyperLinkCoroutine);
        //    HyperLinkCoroutine = StartCoroutine(HyperLinkPopupCO(msg));
        //}
    }

    IEnumerator HyperLinkPopupCO(string msg)
    {
        #region
        isAgainHyperLinkCollided = true;
        yield return new WaitForSeconds(0.2f);
        isAgainHyperLinkCollided = false;
        #endregion
        while (hyperLinkCharCount < msg.Length && !isAgainHyperLinkCollided)
        {
            hyperLinkPopupText.text += msg[hyperLinkCharCount];
            if (defaultFont)
                hyperLinkPopupText.font = defaultFont;

            hyperLinkCharCount++;

            yield return new WaitForSeconds(letterDelay);
        }
        isHyperlinkWritten = false;
        HyperLinkUILinesCount();
    }

    void HyperLinkUILinesCount()
    {
        hyperLinkPopupText.rectTransform.parent.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);

        hyperLinkScrollView.enabled = false;
        hyperLinkScrollbar.SetActive(false);


        hyperlinkTotalHeight = hyperLinkPopupText.rectTransform.rect.height;

        // Get the number of lines in the text.
        int numberOfLines = hyperLinkPopupText.textInfo.lineCount;
        // Calculate the single line height by dividing the total height by the number of lines.
        singleLineHeight = hyperlinkTotalHeight / numberOfLines;

        hyperlinkDownArrowbtn.interactable = !isHyperlinkWritten;
    }

    public void HyperLinkDownText()
    {
        if (hyperLinkScrollView.content.anchoredPosition.y + singleLineHeight * 4 <= hyperlinkTotalHeight)
        {
            hyperLinkScrollView.content.anchoredPosition += new Vector2(0, singleLineHeight);
        }
        else
        {
            DisableHyperLinkPopupUI();
        }
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
    public GameObject BlindComponentParentUI;
    public TextMeshProUGUI BlindComponentTimeText;
    public Coroutine BlindComponentCoroutine;

    public void EnableBlindComponentUI(float timer)
    {
        if (timer > 0)
        {
            DisableAllComponentUIObject(Constants.ItemComponentType.BlindComponent, true);
            //if (BlindComponentCoroutine == null)
            //{
            //    BlindComponentCoroutine = StartCoroutine(IEBlindComponent(timer));
            //}
            //else
            //{
            //    StopCoroutine(BlindComponentCoroutine);
            //    BlindComponentCoroutine = StartCoroutine(IEBlindComponent(timer));
            //}
            BlindComponentTimeText.text = ConvertTimetoSecondsandMinute(timer);
            BlindComponentParentUI.SetActive(true);
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
        //TimeStats._blindComponentStop?.Invoke();
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

    #region Avatar Changer Component
    public GameObject AvatarChangerComponentParentUI;
    public TextMeshProUGUI AvatarChangerComponentTimeText;
    public Coroutine AvatarChangerComponentCoroutine;

    public void EnableAvatarChangerComponentUI(float timer)
    {
        if (timer > 0)
        {
            DisableAllComponentUIObject(Constants.ItemComponentType.AvatarChangerComponent, true);
            if (AvatarChangerComponentCoroutine == null)
            {
                AvatarChangerComponentCoroutine = StartCoroutine(IEAvatarChangerComponent(timer));
            }
            else
            {
                StopCoroutine(AvatarChangerComponentCoroutine);
                AvatarChangerComponentCoroutine = StartCoroutine(IEAvatarChangerComponent(timer));
            }
        }
        else
        {
            AvatarChangerComponentParentUI.SetActive(false);
            if (AvatarChangerComponentCoroutine != null)
                StopCoroutine(AvatarChangerComponentCoroutine);
        }
    }
    public IEnumerator IEAvatarChangerComponent(float timer)
    {
        while (timer > 0)
        {
            timer--;
            AvatarChangerComponentTimeText.text = ConvertTimetoSecondsandMinute(timer);
            AvatarChangerComponentParentUI.SetActive(true);
            yield return new WaitForSeconds(1f);
        }
        AvatarChangerComponentParentUI.SetActive(false);
        AvatarChangerComponentTimeText.text = "00:00";
    }

    public void DisableAvatarChangerComponentUI()
    {
        if (AvatarChangerComponentCoroutine != null)
            StopCoroutine(AvatarChangerComponentCoroutine);
        AvatarChangerComponentParentUI.SetActive(false);
        AvatarChangerComponentTimeText.text = "";
    }
    #endregion

    #region DoorKey Component
    Coroutine EnableDoorKeyCoroutine;
    public void EnableDoorKeyUI(string DisplayMessage)
    {
        DisableAllComponentUIObject(Constants.ItemComponentType.DoorKeyComponent);
        if (EnableDoorKeyCoroutine == null)
        {
            EnableDoorKeyCoroutine = StartCoroutine(IEEnableDoorKeyUI(DisplayMessage));
        }
        else
        {
            StopCoroutine(EnableDoorKeyCoroutine);
            EnableDoorKeyCoroutine = StartCoroutine(IEEnableDoorKeyUI(DisplayMessage));
        }
    }
    public IEnumerator IEEnableDoorKeyUI(string DisplayMessage)
    {
        DisplayMessage = TextLocalization.GetLocaliseTextByKey(DisplayMessage);
        DoorKeyText.text = DisplayMessage;
        bool isJPText = CheckJapaneseDisplayMessage(DisplayMessage);
        //Debug.LogError(isJPText);
        if (isJPText)
            DoorKeyText.font = GamificationComponentData.instance.hiraginoFont;
        else
            DoorKeyText.font = GamificationComponentData.instance.orbitronFont;
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
        if (EnableDoorKeyCoroutine != null)
            StopCoroutine(EnableDoorKeyCoroutine);
        DoorKeyParentUI.SetActive(false);
        DoorKeyText.text = "";
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

    void DisableAllComponentUIObject(Constants.ItemComponentType componentType, bool isTimer = false)
    {
        if (ShouldDisableTimerComponents(componentType, isTimer))
        {
            DisableTimerComponents(componentType);
        }

        if (ShouldDisableNonTimerComponents(componentType, isTimer))
        {
            DisableNonTimerComponents(componentType);
        }
    }

    bool ShouldDisableTimerComponents(Constants.ItemComponentType componentType, bool isTimer)
    {
        return isTimer || componentType == Constants.ItemComponentType.none;
    }

    bool ShouldDisableNonTimerComponents(Constants.ItemComponentType componentType, bool isTimer)
    {
        return !isTimer || componentType == Constants.ItemComponentType.none;
    }

    void DisableTimerComponents(Constants.ItemComponentType componentType)
    {
        if (componentType != Constants.ItemComponentType.SituationChangerComponent)
            DisableSituationChangerUI();
        if (componentType != Constants.ItemComponentType.ElapsedTimeComponent)
            DisableElapseTimeCounDownUI();
        if (componentType != Constants.ItemComponentType.TimeLimitComponent)
            DisableTimeLimitUI();
        if (componentType != Constants.ItemComponentType.TimerCountdownComponent)
            DisableTimerCounDownUI();
        if (componentType != Constants.ItemComponentType.NinjaComponent)
            DisableNinjaMotionUI();
        if (componentType != Constants.ItemComponentType.SpecialItemComponent)
            DisableSpecialItemUI();
        if (componentType != Constants.ItemComponentType.BlindComponent)
            DisableBlindComponentUI();
        if (componentType != Constants.ItemComponentType.AvatarChangerComponent)
            DisableAvatarChangerComponentUI();
        if (componentType != Constants.ItemComponentType.BlindfoldedDisplayComponent)
            DisableAvatarInvisibilityUI();
    }

    void DisableNonTimerComponents(Constants.ItemComponentType componentType)
    {
        if (componentType != Constants.ItemComponentType.DisplayMessagesComponent)
            DisableDisplayMessageUI();
        if (componentType != Constants.ItemComponentType.HelpButtonComponent)
            DisableHelpButtonUI();
        if (componentType != Constants.ItemComponentType.NarrationComponent)
            DisableNarrationUI();
        if (componentType != Constants.ItemComponentType.RandomNumberComponent)
            DisableRandomNumberUI();
        if (componentType != Constants.ItemComponentType.QuizComponent)
            DisableQuizComponentUI();
        if (componentType != Constants.ItemComponentType.ThrowThingsComponent)
            DisableThrowThingUI();
        // if (componentType != Constants.ItemComponentType.HyperLinkPopComponent)
        //     DisableHyperLinkPopupUI();
        if (componentType != Constants.ItemComponentType.DoorKeyComponent)
            DisableDoorKeyUI();
    }

    bool CheckJapaneseDisplayMessage(string displayTitle)
    {
        Regex regex = new Regex(@"\p{IsHiragana}|\p{IsKatakana}|\p{IsCJKUnifiedIdeographs}");
        return regex.IsMatch(displayTitle);
    }
}