using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationBtn : MonoBehaviour
{
    public GameObject highlightButton;
    public GameObject m_EmotePanel;
    public GameObject JyosticksObject;
    public GameObject JumpObject;
    public GameObject BottomObject;
  

    Button btn;
    public bool isClose;

    public void Awake()
    {
        btn = GetComponent<Button>();
    }

    public void OnEnable()
    {
        btn.onClick.AddListener(OnAnimationClick);
        if (GamePlayButtonEvents.inst != null) GamePlayButtonEvents.inst.AllAnimsPanelUpdate += AllAnimsPanelUpdate;

        if (GamePlayButtonEvents.inst != null) EmoteAnimationPlay.AnimationStarted += OnAnimationPlay;
        if (GamePlayButtonEvents.inst != null) EmoteAnimationPlay.AnimationStopped += OnAnimationStoped;

        if (EmoteAnimationPlay.Instance.clearAnimation == null)
        {
            EmoteAnimationPlay.Instance.clearAnimation += ClearAnimations;
        }

    }


    public void OnDisable()
    {
        btn.onClick.RemoveListener(OnAnimationClick);
        if (GamePlayButtonEvents.inst != null) GamePlayButtonEvents.inst.AllAnimsPanelUpdate -= AllAnimsPanelUpdate;

        if (GamePlayButtonEvents.inst != null) EmoteAnimationPlay.AnimationStarted -= OnAnimationPlay;
        if (GamePlayButtonEvents.inst != null) EmoteAnimationPlay.AnimationStopped -= OnAnimationStoped;
        EmoteAnimationPlay.Instance.clearAnimation -= ClearAnimations;
    }

    private void AllAnimsPanelUpdate(bool value)
    {
        if (isClose)
        {
            gameObject.SetActive(value);
            if (!EmoteAnimationPlay.Instance.isAnimRunning && !EmoteAnimationPlay.Instance.isFetchingAnim)
            {
                gameObject.SetActive(false);
            }
        }
    }

    private void OnAnimationClick()
    {
        if (!PremiumUsersDetails.Instance.CheckSpecificItem("gesture button"))
        {
            //PremiumUsersDetails.Instance.PremiumUserUI.SetActive(true);
            print("Please Upgrade to Premium account");
            return;
        }
        else
        {
            print("Horayyy you have Access");
        }

        if (!isClose)
        {

            highlightButton.SetActive(true);
            GamePlayButtonEvents.inst.OpenAllAnims();
            //ReactScreen.Instance.HideReactionScreen();
            if (ReactScreen.Instance.reactionScreenParent.activeInHierarchy)
                ReactScreen.Instance.HideReactionScreen();

            if (ChangeOrientation_waqas._instance.isPotrait)
            {
                ChangeOrientation_waqas._instance.joystickInitPosY = JyosticksObject.transform.localPosition.y;
                //if (ChangeOrientation_waqas._instance.isPotrait)
                //    ChangeOrientation_waqas._instance.joystickInitPosY = JyosticksObject.transform.localPosition.y;
                // ReferrencesForDynamicMuseum.instance.RotateBtn.interactable = false;
                BottomObject.SetActive(false);
              
                m_EmotePanel.SetActive(true);
                if (m_EmotePanel != null)
                    m_EmotePanel.transform.DOLocalMoveY(-108f, 0.1f);

                JyosticksObject.transform.DOKill();
                JyosticksObject.transform.DOLocalMoveY(-50f, 0.1f);

                JumpObject.transform.DOKill();
                JumpObject.transform.DOLocalMoveY(-30f, 0.1f);
                //  ReferrencesForDynamicMuseum.instance.RotateBtn.interactable = true;
                BuilderEventManager.ChangeNinja_ThrowUIPosition?.Invoke(-225,true);
            }
            else
            {
                BuilderEventManager.ChangeNinja_ThrowUIPosition?.Invoke(-165,false);
            }
        }
        else
        {
            if (ReactScreen.Instance.reactionScreenParent.activeInHierarchy)
                ReactScreen.Instance.HideReactionScreen();
            //ReferrencesForDynamicMuseum.instance.RotateBtn.interactable = false;
            Debug.Log("this is else close  :----");
            ReactScreen.Instance.ClosePanel();
            ReactScreen.Instance.HideEmoteScreen();

            EmoteAnimationPlay.Instance.isEmoteActive = false;         // AH working
            EmoteAnimationPlay.Instance.lastAnimClickButton = null; // WaqasAhmad
            highlightButton.SetActive(false);
            GamePlayButtonEvents.inst.CloseEmoteSelectionPanel();
            EmoteAnimationPlay.Instance.StopAnimation(); // stoping animation is any action is performing.

            if (ChangeOrientation_waqas._instance.isPotrait)
            {
                JyosticksObject.transform.DOLocalMoveY(ChangeOrientation_waqas._instance.joystickInitPosY, 0.1f);
                JumpObject.transform.DOLocalMoveY(ChangeOrientation_waqas._instance.joystickInitPosY, 0.1f);
                //BuilderEventManager.ChangeNinja_ThrowUIPosition?.Invoke(-475);
            }
            else
            {
                BuilderEventManager.ChangeNinja_ThrowUIPosition?.Invoke(165,false);
            }
            //  ReferrencesForDynamicMuseum.instance.RotateBtn.interactable = true;
        }

        //StartCoroutine(DelayToOnInteractable());
    }

    IEnumerator DelayToOnInteractable()
    {
        yield return new WaitForSeconds(1f);
        btn.interactable = true;
    }

    public void OnAnimationPlay(string s)
    {
        if (highlightButton == null)
        {
            highlightButton = CanvasButtonsHandler.inst.AnimationBtnClose;
        }
        // Debug.Log("Animation start hua ");
        highlightButton.SetActive(true);
    }

    public void OnAnimationStoped(string s)
    {
        if (!EmoteAnimationPlay.Instance.isEmoteActive)
        {
            if (highlightButton != null && highlightButton.activeInHierarchy)
            {
                highlightButton.SetActive(false);
            }
        }


    }

    void ClearAnimations()
    {
        //isClose = true;
        EmoteAnimationPlay.Instance.StopAnimation();

        highlightButton.SetActive(false);
        GamePlayButtonEvents.inst.CloseEmoteSelectionPanel();
    }

}
