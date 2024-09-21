using System;
using System.Collections;
using System.Collections.Generic;
using Models;
using UnityEngine;
using UnityEngine.Video;

public static class BuilderEventManager
{
    public static Action<int, string> OnBuilderDataFetch;

    public static Action<APIURL, Action<bool>, bool> OnBuilderWorldLoad;
    public static Action<APIURL, bool> OnWorldTabChange;

    public static Action ApplySkyoxSettings;

    public static Action<float, float> ApplyPlayerProperties;

    public static Action AfterMapDataDownloaded;
    public static Action<string> XanaMapDataDownloaded;
    public static Action AfterPlayerInstantiated;

    public static Action AfterWorldInstantiated;
    public static Action AfterWorldOffcialWorldsInatantiated;

    public static Action ReSpawnPlayer;

    //Mesh Combiner
    public static Action CombineMeshes;

    //Orientation Changer
    public static Action<bool> BuilderSceneOrientationChange;
    //Gamification Module Events

    //Narration Component
    public static Action<string, bool, bool> OnNarrationCollisionEnter;
    public static Action OnNarrationCollisionExit;

    //Random Number Component
    public static Action<float> OnRandomCollisionEnter;
    public static Action OnRandomCollisionExit;

    //Time Limit Component
    public static Action<string, float> OnTimerLimitTriggerEnter;

    //Timer Component
    public static Action<string, float> OnTimerTriggerEnter;
    public static Action OnTimerLimitEnd;

    //Elapse Time Component
    public static Action<float, bool> OnElapseTimeCounterTriggerEnter;
    public static Action elapsedEndTime;

    //CountDown Component
    public static Action<int, bool> OnTimerCountDownTriggerEnter;

    //Display Message Component
    public static Action<string, float, bool> OnDisplayMessageCollisionEnter;

    //Door Key Component
    public static Action<string> OnDoorKeyCollisionEnter;

    //Help Button Component
    public static Action<string, string, GameObject> OnHelpButtonCollisionEnter;
    public static Action OnHelpButtonCollisionExit;

    //Situation Changer Component
    public static Action<float> OnSituationChangerTriggerEnter;
    public static Action DisableSituationLight;

    //Quiz Component
    public static Action<QuizComponent, QuizComponentData> OnQuizComponentCollisionEnter;
    public static Action OnQuizComponentColse;

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

    //Avatar change Component
    public static Action<float> OnAvatarChangeComponentTriggerEnter;
    public static Action<bool> StopAvatarChangeComponent;
    public static Action<bool> ChangeCameraHeight;

    //ChangeNinja_ThrowUIPosition
    public static Action<float, bool> ChangeNinja_ThrowUIPosition;
    public static Action PositionUpdateOnOrientationChange;

    //UI toggle
    public static Action<bool> UIToggle;
    public static Action<Constants.ItemComponentType, bool> ResetComponentUI;
    public static Action EnableWorldCanvasCamera;
    public static Action<bool> DisableAnimationsButtons;

    //Component Restriction
    public static Action<Constants.ItemComponentType> onComponentActivated;
    public static Action<IComponentBehaviour> AddItemComponent;
    public static Action RPCcallwhenPlayerJoin;

    //BGM sound manager
    public static Action<AudioPropertiesBGM> BGMDownloader;
    public static Action BGMStart;
    public static Action<float> BGMVolume;

    //UploadPropertyLoad
    public static Action<UploadProperties> UploadPropertiesData;
    public static Action UploadPropertiesInit;
    public static Action<string> YoutubeVideoLoadedCallback;


    //XANA Summit Specific Events 
    public static Action LoadSummitScene;
    public static Action<int, Vector3> LoadNewScene;
    public static Action<string, Vector3> LoadSceneByName;
    public static Action<int, string[]> AINPCActivated;
    public static Action<int> AINPCDeactivated;

    public static Action<VideoClip, Vector3> spaceXActivated;
    public static Action SpaceXDeactivated;
    public static Action<string> loadBGMDirectly;
    public static Action StopBGM;
    public static Action ResetSummit;

    //SMBC specific events
    public static Action<SMBCQuizComponent, QuizComponentData> OnSMBCQuizComponentCollisionEnter;
    public static Action OnSMBCQuizComponentColse;
    public static Action OnSMBCRocketCollected;
    public static Action OnSMBCQuizWrongAnswer;

    public static Action<string, string> OpenRedirectionPopup; 
}