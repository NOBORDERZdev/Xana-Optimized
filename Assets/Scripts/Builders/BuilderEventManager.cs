using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BuilderEventManager
{
    public static Action<int, string> OnBuilderDataFetch;

    public static Action<APIURL, Action<bool>> OnBuilderWorldLoad;
    public static Action<APIURL, bool> OnWorldTabChange;

    public static Action ApplySkyoxSettings;

    public static Action<float, float> ApplyPlayerProperties;

    public static Action AfterPlayerInstantiated;

    //Gamification Module Events

    //Narration Component
    public static Action<string, bool> OnNarrationCollisionEnter;
    public static Action OnNarrationCollisionExit;

    //Random Number Component
    public static Action<float> OnRandomCollisionEnter;
    public static Action OnRandomCollisionExit;

    //Time Limit Component
    public static Action<string, float> OnTimerLimitTriggerEnter;

    //Timer Component
    public static Action<string, float> OnTimerTriggerEnter;

    //Elapse Time Component
    public static Action<float, bool> OnElapseTimeCounterTriggerEnter;

    //CountDown Component
    public static Action<int, bool> OnTimerCountDownTriggerEnter;

    //Display Message Component
    public static Action<string, float, bool> OnDisplayMessageCollisionEnter;

    //Help Button Component
    public static Action<string, string> OnHelpButtonCollisionEnter;
    public static Action OnHelpButtonCollisionExit;

    //Situation Changer Component
    public static Action<float> OnSituationChangerTriggerEnter;

    //Orientation Changer
    public static Action<bool> BuilderSceneOrientationChange;


    public static Action<ComponentType> ResetComponentUI;
}