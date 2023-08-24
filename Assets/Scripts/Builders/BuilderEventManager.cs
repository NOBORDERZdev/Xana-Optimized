using System;
using System.Collections;
using System.Collections.Generic;
using Models;
using UnityEngine;

public static class BuilderEventManager
{
    public static Action<int, string> OnBuilderDataFetch;

    public static Action<APIURL, Action<bool>> OnBuilderWorldLoad;
    public static Action<APIURL, bool> OnWorldTabChange;

    public static Action ApplySkyoxSettings;

    public static Action<float, float> ApplyPlayerProperties;

    public static Action AfterPlayerInstantiated;
    public static Action ReSpawnPlayer;

    //Mesh Combiner
    public static Action CombineMeshes;

    //Orientation Changer
    public static Action<bool> BuilderSceneOrientationChange;
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
    public static Action<string, string, GameObject> OnHelpButtonCollisionEnter;
    public static Action OnHelpButtonCollisionExit;

    //Situation Changer Component
    public static Action<float> OnSituationChangerTriggerEnter;

    //Quiz Component
    public static Action<QuizComponent, QuizComponentData> OnQuizComponentCollisionEnter;

    //Special Item Component
    public static Action<float> OnSpecialItemComponentCollisionEnter;
    public static Action<float, float> SpecialItemPlayerPropertiesUpdate;

    //Avatar Invisibility Component
    public static Action<float> OnAvatarInvisibilityComponentCollisionEnter;
    public static Action ActivateAvatarInivisibility;
    public static Action DeactivateAvatarInivisibility;

    //Ninja Motion Component
    public static Action<float> OnNinjaMotionComponentCollisionEnter;
    public static Action OnAttackwithSword;
    public static Action OnAttackwithShuriken;
    public static Action OnHideOpenSword;

    //Throw Things Component
    public static Action OnThrowThingsComponentCollisionEnter;
    public static Action OnThrowThingsComponentDisable;
    public static Action OnThowThingsPositionSet;
    public static Action OnThrowBall;

    //HyperLinkPopup Component
    public static Action<string, string, string, Transform> OnHyperLinkPopupCollisionEnter;
    public static Action OnHyperLinkPopupCollisionExit;

    //Blind Component
    public static Action<float> OnBlindComponentTriggerEnter;
    public static Action<ComponentType> ResetComponentUI;

    //ChangeNinja_ThrowUIPosition
    public static Action<float,bool> ChangeNinja_ThrowUIPosition;
    public static Action PositionUpdateOnOrientationChange;

    //UI toggle
    public static Action<bool> UIToggle;

    public static Action EnableWorldCanvasCamera;
}