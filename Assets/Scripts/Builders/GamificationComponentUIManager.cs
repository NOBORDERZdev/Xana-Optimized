using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using DG.Tweening;
using Models;

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
        BuilderEventManager.ResetComponentUI += DisableAllComponentUIObject;


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

        BuilderEventManager.ResetComponentUI -= DisableAllComponentUIObject;
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
    public TextMeshProUGUI HelpButtonTitleText;
    public TextMeshProUGUI HelpText;
    public ScrollRect helpButtonScroll;
    public GameObject sliderHelpButtonUI;

    //Situation Changer Component
    public GameObject SituationChangerParentUI;
    public TextMeshProUGUI SituationChangerTimeText;

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
        RandomNumberText.text = "Generated Number On This : " + r.ToString();
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
        //Debug.LogError("EnableElapseTimeCounDownUI" + time);
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

    public void EnableHelpButtonUI(string helpButtonTitle, string HelpTexts)
    {
        DisableAllComponentUIObject(ComponentType.HelpButton);
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
    DisplayMessage
}