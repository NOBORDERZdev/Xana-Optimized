using System;
using System.Collections;
using System.Collections.Generic;
using AIFLogger;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WebSocketSharp;

public class EmoteActionBtn : MonoBehaviour {
    Button actionBtn;
    GameObject highlightObj;
    ButtonClickAndHold btnClickNHold;
    [HideInInspector]
    public AnimationData animData;

    bool isAnimRunning;
    bool dragging = false;
    private DragDectector dragDectector;
    private string animName;
    public void Awake()
    {
        actionBtn = GetComponent<Button>();
        highlightObj = transform.Find("BtnHighlight").gameObject;
        btnClickNHold = GetComponent<ButtonClickAndHold>();
        dragDectector = transform.gameObject.AddComponent<DragDectector>();
    }

    public void OnEnable()
    {
        //if (actionBtn != null) actionBtn.onClick.AddListener(OnActionBtnClick);
        if (btnClickNHold != null)
        {
            btnClickNHold.Clicked += OnActionBtnClick;
            btnClickNHold.LongClicked += OnLongBtnClick;
        }
        if (GamePlayButtonEvents.inst != null)
        {
            GamePlayButtonEvents.inst.AnimationDataUpdated += LoadAnimData;
        }

        EmoteAnimationHandler.AnimationStarted += AnimationStarted;
        EmoteAnimationHandler.AnimationStopped += AnimationStopped;
        LoadAnimData(transform.parent.GetSiblingIndex());
         animName = PlayerPrefsUtility.GetEncryptedString(ConstantsGod.SELECTED_ANIMATION_NAME);
        if (animName != null)
        {
            if (animData != null && animData.animationName.Equals(animName))
            {
                isAnimRunning = EmoteAnimationHandler.Instance.isAnimRunning;
                highlightObj.SetActive(false);
            }
            else
            {
                isAnimRunning = false;
                highlightObj.SetActive(false);
            }
           
        }
        else
        {
            isAnimRunning = false;
            highlightObj.SetActive(false);
        }
        
    }

    public void OnDisable()
    {
        //if (actionBtn != null) actionBtn.onClick.RemoveListener(OnActionBtnClick);
        if (btnClickNHold != null)
        {
            btnClickNHold.LongClicked -= OnLongBtnClick;
            btnClickNHold.Clicked -= OnActionBtnClick;
        }
        if (GamePlayButtonEvents.inst != null)
        {
            GamePlayButtonEvents.inst.AnimationDataUpdated -= LoadAnimData;
        }
        EmoteAnimationHandler.AnimationStarted -= AnimationStarted;
        EmoteAnimationHandler.AnimationStopped -= AnimationStopped;
       

    }


    public void LateUpdate()
    {
        UpdateActionBtn();
    }

    void UpdateActionBtn()
    {
        if (EmoteAnimationHandler.Instance != null)
        {
            if(animData != null) btnClickNHold.interactable = !EmoteAnimationHandler.Instance.isFetchingAnim;
        }
        if (isAnimRunning)
        {
            //actionBtn.Select();
        }
        if (highlightObj != null) highlightObj.SetActive(isAnimRunning);
    }

    public void LoadAnimData(int index)
    {
        Debug.Log("Index==="+index+"tranform==="+ transform.parent.GetSiblingIndex());
        if (index != transform.parent.GetSiblingIndex()) return;

        string data = PlayerPrefsUtility.GetEncryptedString(ConstantsGod.ANIMATION_DATA + index);
        if (!data.IsNullOrEmpty())
        {
            AnimationData d = JsonUtility.FromJson<AnimationData>(data);
            if (animData != null && animData.animationName.Equals(d.animationName)) return;
            animData = d;
            btnClickNHold.interactable = true;
        }
    }

    Coroutine startAnim = null;
    

    public void OnActionBtnClick()
    {
        if (dragDectector.isDragging)
        {
            return;
        }

        if (animData == null || animData.animationName.IsNullOrEmpty())
        {
            print("No Animation Assigned");
            //when empty emote button is clicked open selection panel  ----- amit-21-06-2022
            BuilderEventManager.UIToggle?.Invoke(true);
            btnClickNHold.OnLongClick();
            return;
        }

        if (animData.isEmote)//rik
        {
            PlayerPrefs.SetString(ConstantsGod.ReactionThumb, animData.animationURL);
            ArrowManager.OnInvokeReactionButtonClickEvent(PlayerPrefs.GetString(ConstantsGod.ReactionThumb));
            return;
        }
        if (isAnimRunning && EmoteAnimationHandler.Instance.isAnimRunning)
        {
            //print("Stoping Animation");
            StopEmoteAnimation();
            return;
        }

        StopEmoteAnimation();
        //print("Start Animation");
        if(startAnim!= null) StopCoroutine(startAnim);
        startAnim = StartCoroutine(StartEmoteAnim());
    }

    IEnumerator StartEmoteAnim()
    {
        while(EmoteAnimationHandler.Instance.isAnimRunning)
        {
            yield return null;
        }
        if (animData != null)
        {
            EmoteAnimationHandler.remoteUrlAnimation = animData.animationURL;
            EmoteAnimationHandler.remoteUrlAnimationName = animData.animationName;
            EmoteAnimationHandler.Instance.Load(animData.animationURL, null);
            isAnimRunning = true;
            PlayerPrefsUtility.SetEncryptedString(ConstantsGod.SELECTED_ANIMATION_NAME, animData.animationName);
            //actionBtn.Select();
            try
            {
                GameplayEntityLoader.instance.leftJoyStick.transform.GetChild(0).GetComponent<OnScreenStick>().movementRange = 0;

            }
            catch (Exception e)
            {

            }
        }
    }

    private void OnDestroy()
    {
        PlayerPrefsUtility.SetEncryptedString(ConstantsGod.SELECTED_ANIMATION_NAME,"");
    }

    void StopEmoteAnimation()
    {
        // if (GamePlayButtonEvents.inst != null) GamePlayButtonEvents.inst.OnEmoteAnimationStop();
        EmoteAnimationHandler.Instance.StopAnimation();
    }

    private void OnLongBtnClick()
    {
        Debug.Log("is dragdector==="+ dragDectector.isDragging);

        if (dragDectector.isDragging)
            return;
        PlayerPrefsUtility.SetEncryptedInt(ConstantsGod.EMOTE_SELECTION_INDEX, transform.parent.GetSiblingIndex());
        GamePlayUIHandler.inst.OnOpenAnimationPanel();
    }

    private void AnimationStopped(string animName)
    {
        isAnimRunning = false;
        highlightObj.SetActive(false);
        PlayerPrefsUtility.SetEncryptedString(ConstantsGod.SELECTED_ANIMATION_NAME, "");
    }

    private void AnimationStarted(string animName)
    {
        //Debug.Log("animation call hua new  " + animName);
        if (animData != null && animData.animationName.Equals(animName))
        {
            isAnimRunning = EmoteAnimationHandler.Instance.isAnimRunning;
            highlightObj.SetActive(true);
        }
        else
        {
            isAnimRunning = false;
            highlightObj.SetActive(false);
        }
    }

   

    public void UnloadAnim()
    {
        animData = null;
    }

  

}
